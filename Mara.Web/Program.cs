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
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mara.Common.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

#pragma warning disable CS1591, SA1600 // Program and Main don't need documenting.

namespace Mara.Web
{
    public class Program
    {
        private const string DevEnvVar = "DOTNET_ENVIRONMENT";
        private const string LogOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

        private static readonly string AppDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LuzFaltex", "Mara");

        private static readonly string LogDir = Path.Combine(AppDir, "..\\Logs");

        public static async Task<int> Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable(DevEnvVar) ?? "Production";

            /*
            if (Environment.UserInteractive)
            {
                var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

                Console.WriteLine($"Starting Mara version {fvi.FileVersion}");
                Console.WriteLine(fvi.LegalCopyright);
                Console.WriteLine("For internal use only.");
            }
            */

            var builder = WebAssemblyHostBuilder
                .CreateDefault(args);

            // builder.UseEnvironment(environment);

            builder.Configuration
                .AddEnvironmentVariables("Mara_")
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.{builder.HostEnvironment.Environment}.json", true)
                .AddJsonFile(Path.Combine(AppDir, "appsettings.json"), true)
                .AddJsonFile(Path.Combine(AppDir, $"appsettings.{builder.HostEnvironment.Environment}.json"), true);

            if (builder.HostEnvironment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>();
            }

            // builder.UseDefaultServiceProvider(x => x.ValidateScopes = true);

            Serilog.Core.Logger seriLogger = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .ReadFrom.Configuration(builder.Configuration)
                        .Enrich.FromLogContext()
                        .WriteTo.Console(outputTemplate: LogOutputTemplate, theme: AnsiConsoleTheme.Code)
                        .WriteTo.File(Path.Combine(LogDir, "Execution_.log"), outputTemplate: LogOutputTemplate, rollingInterval: RollingInterval.Day)
                        .CreateLogger();

            builder.Logging.AddSerilog(seriLogger, true);
            Log.Logger = seriLogger;

            var pluginServiceOptions = new PluginServiceOptions(new List<string>() { "./Plugins" });
            var pluginService = new PluginService(Options.Create(pluginServiceOptions));

            var plugins = pluginService.LoadPluginTree();
            var configurePluginResult = plugins.ConfigureServices(builder.Services);
            if (!configurePluginResult.IsSuccess)
            {
                if (configurePluginResult.Error is AggregateError aggregateError)
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
                    Log.ForContext<Program>().Error("Failed to load plugins!{NewLine}{ChildMessages}", Environment.NewLine, sb.ToString());

                    return 1;
                }
                else if (configurePluginResult.Error is ExceptionError exe)
                {
                    Log.ForContext<Program>().Error(exe.Exception, "Failed to load plugins: {Error}", exe.Message);

                    return exe.Exception.HResult;
                }
                else
                {
                    Log.ForContext<Program>().Error("Failed to load plugins: {Error}", configurePluginResult.Error.Message);

                    return 1;
                }
            }

            builder.Services
                .AddOptions<MaraConfig>()
                .Configure<IConfiguration>((options, config) => config.Bind(options));
            // builder.Services.Configure<MaraConfig>(builder.Configuration);
            builder.Services.AddSingleton(pluginService);

            var discordToken = builder.Configuration[nameof(MaraConfig.DiscordToken)];
            if (string.IsNullOrEmpty(discordToken))
            {
                Log.ForContext<Program>().Error("The Discord Token for the bot must not be null or empty.");
                return 1;
            }

            builder.Services.AddDiscordGateway(x => discordToken);
            builder.Services.AddDiscordCommands(enableSlash: true);

            builder.Services.AddMemoryCache();
            // builder.Services.AddHostedService<MaraBot>();

            builder.Services.Configure<DiscordGatewayClientOptions>(x =>
                x.Intents |=
                    GatewayIntents.DirectMessages |
                    GatewayIntents.GuildBans |
                    GatewayIntents.GuildEmojisAndStickers |
                    GatewayIntents.GuildIntegrations |
                    GatewayIntents.GuildInvites |
                    GatewayIntents.GuildMembers |
                    GatewayIntents.GuildMessageReactions |
                    GatewayIntents.GuildMessages |
                    GatewayIntents.Guilds |
                    GatewayIntents.GuildWebhooks |
                    GatewayIntents.MessageContents);

            // Web Stuff
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await using var host = builder.Build();

            try
            {
                // TODO: Run migrations before starting bot.
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
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
