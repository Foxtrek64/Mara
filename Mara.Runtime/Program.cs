//
//  Program.cs
//
//  Author:
//       LuzFaltex Contributors
//
//  ISC License
//
//  Copyright (c) 2021 LuzFaltex
//
//  Permission to use, copy, modify, and/or distribute this software for any
//  purpose with or without fee is hereby granted, provided that the above
//  copyright notice and this permission notice appear in all copies.
//
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
//  WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
//  MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
//  ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
//  WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
//  ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
//  OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Mara.Common.Discord.Feedback;
using Mara.Common.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Extensions;
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
                    builder.AddJsonFile($"appsettings{context.HostingEnvironment.EnvironmentName}.json", true);
                    builder.AddJsonFile(Path.Combine(AppDir, "appsettings.json"), true);
                    builder.AddJsonFile(Path.Combine(AppDir, $"..\\appsettings{context.HostingEnvironment.EnvironmentName}.json"), true);

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

                    services.Configure<MaraConfig>(context.Configuration);
                    services.AddSingleton(pluginService);

                    Debug.Assert(!string.IsNullOrEmpty(context.Configuration[nameof(MaraConfig.DiscordToken)]), $"The '{nameof(MaraConfig.DiscordToken)}' must not be null or empty.");

                    services.AddDiscordGateway(x => context.Configuration[nameof(MaraConfig.DiscordToken)]);
                    services.AddDiscordCommands(enableSlash: true);

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
