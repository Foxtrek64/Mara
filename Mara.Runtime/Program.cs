//
//  Program.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Mara.Common;
using Mara.Common.Discord.Feedback;
using Mara.Common.Extensions;
using Mara.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.Caching.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Rest.Extensions;
using Remora.Plugins.Abstractions;
using Remora.Plugins.Services;
using Remora.Results;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Mara.Runtime
{
    /// <summary>
    /// The application entry point.
    /// </summary>
    public class Program
    {
        private const string DevEnvVar = "DOTNET_ENVIRONMENT";
        private const string LogOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

        private static readonly string AppDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LuzFaltex", "Mara");

        private static readonly string LogDir = Path.Combine(AppDir, "..\\Logs");

        /// <summary>
        /// The app entry point.
        /// </summary>
        /// <returns>An integer representing the result of the async operation. Zero indicates success while non-zero results indicate failure.</returns>
        public static async Task<int> Main()
        {
            var environment = Environment.GetEnvironmentVariable(DevEnvVar) ?? "Production";

            var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            Console.WriteLine($"Mara version {fvi.FileVersion}");
            Console.WriteLine(fvi.LegalCopyright);
            Console.WriteLine("For internal use only.");

            var hostBuilder = new HostBuilder()
                .UseEnvironment(environment)
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddEnvironmentVariables("Mara_");

                    builder.AddJsonFile("appsettings.json", true);
                    builder.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true);

                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        builder.AddUserSecrets<Program>();
                    }
                })
                .UseDefaultServiceProvider(x => x.ValidateScopes = true)
                .ConfigureLogging((context, builder) =>
                {
                    Serilog.Core.Logger seriLogger = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .ReadFrom.Configuration(context.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console(outputTemplate: LogOutputTemplate, theme: AnsiConsoleTheme.Code)
                        .WriteTo.File(Path.Combine(LogDir, "Execution_.log"), outputTemplate: LogOutputTemplate, rollingInterval: RollingInterval.Day)
                        .CreateLogger();

                    builder.AddSerilog(seriLogger);
                    Log.Logger = seriLogger;
                })
                .ConfigureServices((context, services) =>
                {
                    var pluginServiceOptions = new PluginServiceOptions(new List<string>() { "./Plugins" });
                    var pluginService = new PluginService(Options.Create(pluginServiceOptions));

                    var plugins = pluginService.LoadPluginTree();
                    var configurePluginsResult = plugins.ConfigureServices(services);
                    // var configurePluginsResult = Result.FromSuccess();
                    if (!configurePluginsResult.IsSuccess)
                    {
                        if (configurePluginsResult.Error is AggregateError aggregateError)
                        {
                            var sb = new StringBuilder();
                            foreach (var error in aggregateError.Errors)
                            {
                                if (error.IsSuccess)
                                {
                                    continue;
                                }
                                sb.AppendLine($"  {error.Error!.Message}");
                            }
                            Log.Error("Failed to load plugins!\n{ChildMessages}", sb.ToString());
                        }
                        else
                        {
                            Log.Error("Failed to load plugins: {error}", configurePluginsResult.Error.Message);
                        }

                        if (configurePluginsResult.Error is ExceptionError exe)
                        {
                            Console.WriteLine(exe.Exception.ToString());
                        }
                    }

                    services.AddConfigurationModule<RuntimeConfig>()
                            .PostConfigure(
                                options =>
                                {
                                    if (string.IsNullOrWhiteSpace(options.BotWebsiteUrl))
                                    {
                                        options = options with { BotWebsiteUrl = "https://localhost" };
                                    }
                                })
                            .Validate
                            (
                                x => !string.IsNullOrWhiteSpace(x.BotToken),
                                "Discord token must not be null or white space."
                            )
                            .Validate
                            (
                                x => Uri.TryCreate(x.BotWebsiteUrl, UriKind.Absolute, out _),
                                "Website URL must not be null or white space."
                            );
                    services.AddSingleton(pluginService);

                    services.AddDiscordRest();
                    services.AddDiscordGateway(x => x.GetRequiredService<IOptions<RuntimeConfig>>().Value.BotToken);
                    services.AddDiscordCommands(enableSlash: true);
                    services.AddDiscordCaching();

                    services.AddMemoryCache();
                    services.AddHostedService<MaraBot>();

                    services.Configure<DiscordGatewayClientOptions>(x
                        => x.Intents |=
                            GatewayIntents.DirectMessages |
                            GatewayIntents.GuildBans |
                            GatewayIntents.GuildEmojisAndStickers |
                            GatewayIntents.GuildIntegrations |
                            GatewayIntents.GuildInvites |
                            GatewayIntents.GuildMembers |
                            GatewayIntents.GuildMessageReactions |
                            GatewayIntents.GuildMessages |
                            GatewayIntents.Guilds |
                            GatewayIntents.GuildWebhooks);
                });

            hostBuilder = (Debugger.IsAttached && Environment.UserInteractive)
                ? hostBuilder.UseConsoleLifetime()
                : hostBuilder.UseWindowsService();

            using var host = hostBuilder.Build();

            try
            {
                using var serviceScope = host.Services.CreateAsyncScope();

                // Get all plugins and run migrations
                var plugins = serviceScope.ServiceProvider.GetRequiredService<PluginService>().LoadPlugins().OfType<IMigratablePlugin>();
                // var plugins = Array.Empty<IMigratablePlugin>();
                foreach (var plugin in plugins)
                {
                    await plugin.MigrateAsync(serviceScope.ServiceProvider);
                }

                await host.RunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.ForContext<Program>()
                    .Fatal(ex, "Host terminated unexpectedly.");

                if (Debugger.IsAttached && Environment.UserInteractive)
                {
                    Console.WriteLine(Environment.NewLine + "Press any key to exit...");
                    Console.ReadKey(true);
                }

                return ex.HResult;
            }
        }
    }
}
