//
//  MaraBot.cs
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

namespace Mara.Runtime
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

                /*
                var migrateResult = await pluginTree.MigrateAsync(_scope.ServiceProvider, stoppingToken);
                if (migrateResult.IsSuccess)
                {
                    return migrateResult;
                }
                */

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
