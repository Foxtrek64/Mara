//
//  ServiceCollectionExtensions.cs
//
//  Author:
//       LuzFaltex Contributors <support@luzfaltex.com>
//
//  Copyright (c) LuzFaltex, LLC.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Text;
using JetBrains.Annotations;
using Mara.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Objects;
using Remora.Discord.Caching.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Rest.Extensions;
using Remora.Plugins.Services;
using Remora.Results;
using Serilog;

namespace Mara.Common.Extensions
{
    /// <summary>
    /// Adds a set of extensions for <see cref="IServiceCollection"/>.
    /// </summary>
    [PublicAPI]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Shared functionality for adding the Mara Discord bot to any service host.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="logger">A logger for this instance.</param>
        /// <returns>The modified service collection, for chaining.</returns>
        public static IServiceCollection AddMara(this IServiceCollection services, ILogger logger)
        {
            var pluginServiceOptions = new PluginServiceOptions(["./Plugins."]);
            var pluginService = new PluginService(Options.Create(pluginServiceOptions));

            var plugins = pluginService.LoadPluginTree();
            var configurePluginsResult = plugins.ConfigureServices(services);

            if (!configurePluginsResult.IsSuccess)
            {
                if (configurePluginsResult.Error is AggregateError ae)
                {
                    var sb = new StringBuilder();
                    foreach (var error in ae.Errors)
                    {
                        if (error.IsSuccess)
                        {
                            continue;
                        }

                        sb.AppendLine($"\t{error.Error!.Message}");
                    }
                    logger.Error("Failed to load plugins!\n{ChildMessages}", sb.ToString());
                }
                else
                {
                    logger.Error("Failed to load plugins: {Error}", configurePluginsResult.Error.Message);
                }
            }

            services.AddConfigurationModule<RuntimeConfig>()
                    .PostConfigure
                    (
                        options =>
                        {
                            if (string.IsNullOrWhiteSpace(options.BotWebsiteUrl))
                            {
                                options = options with { BotWebsiteUrl = "https://localhost" };
                            }
                        }
                    )
                    .Validate
                    (
                        x => !string.IsNullOrWhiteSpace(x.BotToken),
                        "Discord token must not be null or white space"
                    )
                    .Validate
                    (
                        x => Uri.TryCreate(x.BotWebsiteUrl, UriKind.Absolute, out _),
                        "Website URL must be a valid URI."
                    )
                    .Validate
                    (
                        x => !string.IsNullOrWhiteSpace(x.ClientId),
                        "Bot Client Id must not be null or white space."
                    )
                    .Validate
                    (
                        x => !string.IsNullOrWhiteSpace(x.ClientSecret),
                        "Bot Client Secret must not be null or white space."
                    );

            services.AddSingleton(pluginService);

            services.AddDiscordRest();
            services.AddDiscordGateway(x => x.GetRequiredService<IOptions<RuntimeConfig>>().Value.BotToken);
            services.AddDiscordCommands(enableSlash: true);
            services.AddDiscordCaching();

            services.AddHostedService<MaraBot>();

            services.Configure<DiscordGatewayClientOptions>
            (
                options =>
                {
                    options.Intents |=
                        GatewayIntents.DirectMessages |
                        GatewayIntents.GuildBans |
                        GatewayIntents.GuildEmojisAndStickers |
                        GatewayIntents.GuildIntegrations |
                        GatewayIntents.GuildInvites |
                        GatewayIntents.GuildMembers |
                        GatewayIntents.GuildMessageReactions |
                        GatewayIntents.GuildMessages |
                        GatewayIntents.Guilds |
                        GatewayIntents.GuildWebhooks;
                    options.Presence = new UpdatePresence
                    (
                        UserStatus.Online,
                        IsAFK: false,
                        Since: null,
                        new[] { new Activity(Name: "anime", Type: ActivityType.Watching) }
                    );
                }
            );

            return services;
        }

        /// <summary>
        /// Binds the provided configuration module to the host's <see cref="IConfiguration"/>.
        /// </summary>
        /// <param name="services">The service collection to modify.</param>
        /// <typeparam name="TConfigurationModule">The type of <see cref="IConfigurationModule"/> to bind.</typeparam>
        /// <returns>An <see cref="OptionsBuilder{TOptions}"/> to allow for further configuration or validation.</returns>
        public static OptionsBuilder<TConfigurationModule> AddConfigurationModule<TConfigurationModule>(this IServiceCollection services)
            where TConfigurationModule : class, IConfigurationModule
            => services.AddOptions<TConfigurationModule>()
                       .Configure<IConfiguration>
                       (
                           (options, configuration) =>
                           {
                               configuration.Bind(options.SectionName, options);
                           }
                       );
    }
}
