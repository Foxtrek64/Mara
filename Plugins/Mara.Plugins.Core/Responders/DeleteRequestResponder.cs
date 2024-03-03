//
//  DeleteRequestResponder.cs
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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
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
    [UsedImplicitly]
    public sealed class DeleteRequestResponder : LoggingEventResponderBase<MessageReactionAdd>
    {
        private const string XEmoji = "❌";

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
        protected override async Task<Result> HandleAsync(MessageReactionAdd gatewayEvent, CancellationToken cancellationToken = default)
        {
            // If the reaction wasn't ❌, skip.
            if (!gatewayEvent.Emoji.Name.Equals(XEmoji))
            {
                return Result.FromSuccess();
            }

            var messageResult = await _channelApi.GetChannelMessageAsync
            (
                gatewayEvent.ChannelID,
                gatewayEvent.MessageID,
                cancellationToken
            );

            if (!messageResult.IsDefined(out var message))
            {
                return Result.FromError(messageResult);
            }

            // If the message has no deletable embeds, skip
            if (message.Embeds.All(x => x.Footer != CoreConstants.DismissibleEmbedFooter))
            {
                return Result.FromSuccess();
            }

            // At least one embed is deletable. Delete the message.
            return await _channelApi.DeleteMessageAsync
            (
                gatewayEvent.ChannelID,
                gatewayEvent.MessageID,
                reason: "User requested deletion.",
                cancellationToken
            );
        }
    }
}
