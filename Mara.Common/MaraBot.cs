//
//  MaraBot.cs
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
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mara.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Discord.Gateway;
using Remora.Plugins.Services;
using Remora.Results;

namespace Mara.Common
{
    /// <summary>
    /// Encapsulates the main runtime of the bot.
    /// </summary>
    public sealed class MaraBot : BackgroundService
    {
        private readonly DiscordGatewayClient _discordClient;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<MaraBot> _logger;
        private readonly PluginService _pluginService;

        private IServiceScope? _scope = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaraBot"/> class.
        /// </summary>
        /// <param name="discordClient">A <see cref="DiscordGatewayClient"/>.</param>
        /// <param name="scopeFactory">An <see cref="IServiceScopeFactory"/> for creating service scopes.</param>
        /// <param name="applicationLifetime">The <see cref="IHostApplicationLifetime"/> managing the lifetime of the app.</param>
        /// <param name="logger">A scoped logger.</param>
        /// <param name="pluginService">The plugin service.</param>
        public MaraBot
        (
            DiscordGatewayClient discordClient,
            IServiceScopeFactory scopeFactory,
            IHostApplicationLifetime applicationLifetime,
            ILogger<MaraBot> logger,
            PluginService pluginService
        )
        {
            _discordClient = discordClient;
            _scopeFactory = scopeFactory;
            _applicationLifetime = applicationLifetime;
            _logger = logger;
            _pluginService = pluginService;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            var initResult = await InitializeAsync(stoppingToken);
            if (!initResult.IsSuccess)
            {
                _logger.LogError(initResult.Error);
                return;
            }

            _logger.LogInformation("Logging into Discord and starting the client.");

            var runResult = await _discordClient.RunAsync(stoppingToken);

            if (!runResult.IsSuccess)
            {
                _logger.LogCritical("A critical error has occurred: {Error}", runResult.Error!.Message);
            }
        }

        private async Task<Result> InitializeAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Initializing bot service...");

            try
            {
                // Create new scope for this session
                _scope = _scopeFactory.CreateScope();

                // Register the OnStopping method with the cancellation token
                stoppingToken.Register(OnStopping);

                // Load plugins
                var pluginTree = _pluginService.LoadPluginTree();

                var initResult = await pluginTree.InitializeAsync(_scope.ServiceProvider, stoppingToken);
                if (!initResult.IsSuccess)
                {
                    if (initResult.Error is AggregateError aggregateError)
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
                        _logger.LogError("{Message}:\n{ChildMessages}", initResult.Error.Message, sb.ToString());
                    }
                    else
                    {
                        _logger.LogError("An error occurred while initializing plugins: {error}", initResult.Error.Message);
                    }

                    return initResult;
                }

                var migrateResult = await pluginTree.MigrateAsync(_scope.ServiceProvider, stoppingToken);
                if (migrateResult.IsSuccess)
                {
                    return migrateResult;
                }

                return Result.FromSuccess();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An error occurred while attempting to start the background service.");

                OnStopping();

                return ex;
            }
        }

        private void OnStopping()
        {
            _logger.LogInformation("Stopping background service.");
            try
            {
                _applicationLifetime.StopApplication();
            }
            finally
            {
                _scope = null;
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            try
            {
                base.Dispose();
            }
            finally
            {
                _scope?.Dispose();
            }
        }
    }
}
