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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mara.Common.Extensions;
using Mara.WebAdmin.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Remora.Plugins.Abstractions;
using Remora.Plugins.Services;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace Mara.WebAdmin
{
    /// <summary>
    /// The main entry point of the application.
    /// </summary>
    public sealed class Program
    {
        private const string LogOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}";

        private static async Task<int> Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Environment.EnvironmentName = Environment.GetEnvironmentVariable
                    ("ASPNETCORE_ENVIRONMENT") ?? "Production";

            builder.Configuration.AddEnvironmentVariables("Mara_");

            builder.Configuration.AddJsonFile("appsettings.json", true);
            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true);

            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>();
            }

            // Configure Logging
            Serilog.Core.Logger seriLogger = new LoggerConfiguration()
                                             .MinimumLevel.Verbose()
                                             .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                             .ReadFrom.Configuration(builder.Configuration)
                                             .Enrich.FromLogContext()
                                             .WriteTo.File
                                             (
                                                 Path.Combine("./Logs/Execution.log"),
                                                 outputTemplate: LogOutputTemplate,
                                                 rollingInterval: RollingInterval.Day
                                             )
                                             .WriteTo.Console
                                            (
                                                outputTemplate: LogOutputTemplate,
                                                theme: AnsiConsoleTheme.Code
                                            )
                                             // .WriteTo.SiteLog()
                                             .CreateLogger();
            builder.Logging.AddSerilog(seriLogger);
            Log.Logger = seriLogger;

            // Add services to the container.
            builder.Services.AddRazorComponents()
                   .AddInteractiveServerComponents();

            // Add Mara.
            builder.Services.AddMara(Log.Logger);

            await using var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
               .AddInteractiveServerRenderMode();

            try
            {
                await using var serviceScope = app.Services.CreateAsyncScope();

                // Get all plugins and run migrations.
                var tasks = serviceScope
                            .ServiceProvider
                            .GetRequiredService<PluginService>()
                            .LoadPlugins()
                            .OfType<IMigratablePlugin>()
                            .Select(plugin => plugin.MigrateAsync(serviceScope.ServiceProvider));
                await Task.WhenAll(tasks);

                await app.RunAsync();
                return 0;
            }
            catch (Exception e)
            {
                Log.ForContext<Program>()
                   .Fatal(e, "Host terminated unexpectedly.");

                if (Debugger.IsAttached && Environment.UserInteractive)
                {
                    Console.WriteLine(Environment.NewLine + "Press any key to exit...");
                    Console.ReadKey(true);
                }

                return e.HResult;
            }
        }
    }
}
