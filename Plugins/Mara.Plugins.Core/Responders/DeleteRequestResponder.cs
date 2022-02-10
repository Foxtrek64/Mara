//
//  DeleteRequestResponder.cs
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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mara.Common.Events;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Gateway.Events;
using Remora.Discord.Rest.API;
using Remora.Results;

namespace Mara.Plugins.Core.Responders
{
    /// <summary>
    /// A responder which handles removing eligible embeds flagged with the X emoji.
    /// </summary>
    public sealed class DeleteRequestResponder : LoggingEventResponderBase<MessageReactionAdd>
    {
        private readonly DiscordRestChannelAPI _channelApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteRequestResponder"/> class.
        /// </summary>
        /// <param name="channelApi">An instance of <see cref="DiscordRestChannelAPI"/>.</param>
        /// <param name="logger">A logger for this event handler.</param>
        public DeleteRequestResponder(DiscordRestChannelAPI channelApi, ILogger<DeleteRequestResponder> logger)
            : base(logger)
        {
            _channelApi = channelApi;
        }

        /// <inheritdoc/>
        protected override async Task<Result> Handle(MessageReactionAdd gatewayEvent, CancellationToken cancellationToken = default)
        {
            // If the reaction wasn't ❌, skip.
            if (!gatewayEvent.Emoji.Name.Equals("❌"))
            {
                return Result.FromSuccess();
            }

            var messageResult = await _channelApi.GetChannelMessageAsync
            (
                gatewayEvent.ChannelID,
                gatewayEvent.MessageID,
                cancellationToken
            );

            if (!messageResult.IsSuccess)
            {
                return Result.FromError(messageResult);
            }

            var message = messageResult.Entity!;

            // If the message has no deletable embeds, skip
            if (message.Embeds.All(x => x.Footer != CoreConstants.DismissableEmbedFooter))
            {
                return Result.FromSuccess();
            }

            // At least one embed is deletable. Delete the message.
            return await _channelApi.DeleteMessageAsync
            (
                gatewayEvent.ChannelID,
                gatewayEvent.MessageID,
                "User requested deletion.",
                cancellationToken
            );
        }
    }
}
