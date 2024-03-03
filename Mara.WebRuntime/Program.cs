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
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.EntityFrameworkCore;
using Mara.Common;
using Mara.Common.Extensions;
using Mara.Common.Models;
using Mara.WebRuntime.Areas.Identity;
using Mara.WebRuntime.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

namespace Mara.WebRuntime
{
    /// <summary>
    /// The main entry point of the application.
    /// </summary>
    public class Program
    {
        private const string LogOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

        private static readonly string AppDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LuzFaltex", "Mara");

        private static readonly DateTimeOffset StartupTime = DateTimeOffset.Now;

        private static readonly string LogDir = Path.Combine(AppDir, "Logs");

        private static async Task<int> Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            System.Globalization.CultureInfo.CurrentUICulture = System.Globalization.CultureInfo.CurrentCulture;

            var hostBuilder = WebApplication.CreateBuilder(args);

            // Configuration
            hostBuilder.Configuration.AddEnvironmentVariables("Mara_");

            hostBuilder.Configuration.AddJsonFile("appsettings.json", true);
            hostBuilder.Configuration.AddJsonFile($"appsettings.{hostBuilder.Environment.EnvironmentName}.json", true);
            hostBuilder.Configuration.AddJsonFile(Path.Combine(AppDir, "appsettings.json"), true);
            hostBuilder.Configuration.AddJsonFile(Path.Combine(AppDir, $"appsettings.{hostBuilder.Environment.EnvironmentName}.json"), true);

            if (hostBuilder.Environment.IsDevelopment())
            {
                hostBuilder.Configuration.AddUserSecrets<Program>();
            }

            // Logging
            Serilog.Core.Logger seriLogger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .ReadFrom.Configuration(hostBuilder.Configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: LogOutputTemplate, theme: AnsiConsoleTheme.Code)
                .WriteTo.File(Path.Combine(LogDir, "Execution_.log"), outputTemplate: LogOutputTemplate, rollingInterval: RollingInterval.Day)
                .CreateLogger();

            hostBuilder.Logging.AddSerilog(seriLogger);
            Log.Logger = seriLogger;

            // Plugins
            var pluginServiceOptions = new PluginServiceOptions(new List<string> { "./Plugins" });
            var pluginService = new PluginService(Options.Create(pluginServiceOptions));

            var pluginTree = pluginService.LoadPluginTree();
            var configurePluginsResult = pluginTree.ConfigureServices(hostBuilder.Services);

            if (!configurePluginsResult.IsSuccess)
            {
                switch (configurePluginsResult.Error)
                {
                    case ExceptionError exe:
                        Log.Fatal(exe.Exception, "Failed to load plugins: {Error}", exe.Message);
                        return exe.Exception.HResult;
                    case AggregateError ae:
                    {
                        var sb = new StringBuilder();
                        foreach (var error in ae.Errors)
                        {
                            if (error.IsSuccess)
                            {
                                continue;
                            }
                            sb.AppendLine($"  {error.Error!.Message}");
                        }
                        Log.Fatal("Failed to load plugins!\n{ChildMessages}", sb.ToString());
                        return 1;
                    }
                    default:
                        Log.Fatal("Failed to load plugins: {Error}", configurePluginsResult.Error.Message);
                        return 1;
                }
            }

            // Services
            hostBuilder.Services
                    .AddConfigurationModule<RuntimeConfig>()
                    .PostConfigure(
                        options =>
                        {
                            if (string.IsNullOrWhiteSpace(options.BotWebsiteUrl))
                            {
                                options = options with { BotWebsiteUrl = "localhost" };
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
            hostBuilder.Services.AddSingleton(pluginService);
            hostBuilder.Services.AddDiscordRest();
            hostBuilder.Services.AddDiscordGateway(x => x.GetRequiredService<IOptions<RuntimeConfig>>().Value.BotToken);
            hostBuilder.Services.AddDiscordCommands(enableSlash: true);

            hostBuilder.Services.AddMemoryCache();

            hostBuilder.Services.Configure<DiscordGatewayClientOptions>(x =>
                x.Intents |=
                    GatewayIntents.DirectMessages |
                    GatewayIntents.GuildBans |
                    GatewayIntents.GuildEmojisAndStickers |
                    GatewayIntents.GuildIntegrations |
                    GatewayIntents.GuildInvites |
                    GatewayIntents.GuildMembers |
                    GatewayIntents.GuildMessageReactions |
                    GatewayIntents.GuildMessages |
                    GatewayIntents.GuildScheduledEvents |
                    GatewayIntents.Guilds |
                    GatewayIntents.GuildWebhooks |
                    GatewayIntents.MessageContents);

            hostBuilder.Services.AddDiscordCaching();

            // Web stuff
            hostBuilder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                string connectionString = hostBuilder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' may not be null or empty.");
                options.UseSqlServer(connectionString);
            });
            hostBuilder.Services.AddDatabaseDeveloperPageExceptionFilter();
            hostBuilder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            hostBuilder.Services.AddRazorPages();
            hostBuilder.Services.AddServerSideBlazor();
            hostBuilder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

            // hostBuilder.Services.AddAuthentication().AddOAuth("foo", x => { });

            // Hangfire
            hostBuilder.Services.AddHangfire
            (
                (serviceProvider, configuration) =>
                {
                    configuration.UseEFCoreStorage
                                 (
                                     () => serviceProvider
                                           .GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
                                           .CreateDbContext(),
                                     new EFCoreStorageOptions
                                     {
                                         CountersAggregationInterval = new TimeSpan(0, 5, 0),
                                         DistributedLockTimeout = new TimeSpan(0, 10, 0),
                                         JobExpirationCheckInterval = new TimeSpan(0, 30, 0),
                                         QueuePollInterval = new TimeSpan(0, 0, 15),
                                         Schema = "hangfire",
                                         SlidingInvisibilityTimeout = new TimeSpan(0, 5, 0)
                                     }
                                 )
                                 .UseDatabaseCreator();
                }
            );

            // Add Hosted Service
            hostBuilder.Services.AddHostedService<MaraBot>();

            await using WebApplication host = hostBuilder.Build();

            try
            {
                await using AsyncServiceScope serviceScope = host.Services.CreateAsyncScope();

                // Run migrations
                ApplicationDbContext dbContext =
                    serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync();

                IEnumerable<IMigratablePlugin> plugins =
                    serviceScope.ServiceProvider
                                .GetRequiredService<PluginService>()
                                .LoadPlugins()
                                .OfType<IMigratablePlugin>();

                foreach (var plugin in plugins)
                {
                    await plugin.MigrateAsync(serviceScope.ServiceProvider);
                }

                // Configure the HTTP Request pipeline
                if (host.Environment.IsDevelopment())
                {
                    host.UseMigrationsEndPoint();
                }
                else
                {
                    host.UseExceptionHandler("/Error");
                    host.UseHsts();
                }

                host.UseHttpsRedirection();

                host.UseStaticFiles();

                host.UseRouting();

                host.UseAuthentication();
                host.UseAuthorization();

                host.MapControllers();
                host.MapBlazorHub();
                host.MapFallbackToPage("/_Host");

                await host.RunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.ForContext<Program>()
                   .Fatal(ex, "Host terminated unexpectedly");

                if (Debugger.IsAttached && Environment.UserInteractive)
                {
                    Console.WriteLine(Environment.NewLine + "Press any key to exit...");
                    Console.ReadKey(true);
                }

                return ex.HResult;
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }
    }
}
