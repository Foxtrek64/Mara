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

using System.Threading;
using System.Threading.Tasks;
using Mara.Common.Events;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Gateway.Commands;
using Remora.Discord.API.Objects;
using Remora.Discord.Gateway;
using Remora.Results;

namespace Mara.Plugins.Core.Responders
{
    /// <summary>
    /// Handles post-startup tasks, such as setting the bot presence and registering global slash commands.
    /// </summary>
    public sealed class ReadyResponder : LoggingEventResponderBase<IReady>
    {
        private readonly ILogger _logger;
        private readonly DiscordGatewayClient _gatewayClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadyResponder"/> class.
        /// </summary>
        /// <param name="logger">A logger.</param>
        /// <param name="gatewayClient">A gateway client.</param>
        public ReadyResponder
        (
            ILogger<ReadyResponder> logger,
            DiscordGatewayClient gatewayClient
        )
            : base(logger)
        {
            _logger = logger;
            _gatewayClient = gatewayClient;
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

            return Task.FromResult(Result.FromSuccess());
        }
    }
}
