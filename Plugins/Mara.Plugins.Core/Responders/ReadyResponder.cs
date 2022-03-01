//
//  ReadyResponder.cs
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
using System.Threading;
using System.Threading.Tasks;
using Mara.Common.Events;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Objects;
using Remora.Discord.Gateway;
using Remora.Results;

using CacheKeys = Mara.Plugins.Core.CoreConstants.CacheKeys;

namespace Mara.Plugins.Core.Responders
{
    /// <summary>
    /// Handles post-startup tasks, such as setting the bot presence and registering global slash commands.
    /// </summary>
    public sealed class ReadyResponder : LoggingEventResponderBase<IReady>
    {
        private readonly ILogger _logger;
        private readonly DiscordGatewayClient _gatewayClient;
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadyResponder"/> class.
        /// </summary>
        /// <param name="logger">A logger.</param>
        /// <param name="gatewayClient">A gateway client.</param>
        /// <param name="memoryCache">A memory cache which will store the ready event.</param>
        public ReadyResponder
        (
            ILogger<ReadyResponder> logger,
            DiscordGatewayClient gatewayClient,
            IMemoryCache memoryCache)
            : base(logger)
        {
            _logger = logger;
            _gatewayClient = gatewayClient;
            _memoryCache = memoryCache;
        }

        /// <inheritdoc/>
        protected override Task<Result> Handle(IReady gatewayEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bot started!");

            // Set status
            var updatePresence = new UpdatePresence
                (
                    ClientStatus.Online,
                    IsAFK: false,
                    Since: null,
                    new[] { new Activity("anime", ActivityType.Watching) }
                );
            _gatewayClient.SubmitCommand(updatePresence);

            // Add the ready event information to the memory cache.
            _logger.LogDebug("Caching the IReady event entries.");
            _memoryCache.Set(CacheKeys.BotUser, gatewayEvent.User);
            _memoryCache.Set(CacheKeys.CurrentApplication, gatewayEvent.Application);
            _memoryCache.Set(CacheKeys.ShardNumber, gatewayEvent.Shard);
            _memoryCache.Set(CacheKeys.StartupTime, DateTimeOffset.UtcNow);

            // Startup Guilds
            // These should be removed after the first use
            _memoryCache.Set(CacheKeys.StartupGuilds, gatewayEvent.Guilds);

            return Task.FromResult(Result.FromSuccess());
        }
    }
}
