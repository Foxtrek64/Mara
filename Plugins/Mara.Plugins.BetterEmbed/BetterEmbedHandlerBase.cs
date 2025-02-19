﻿//
//  BetterEmbedHandlerBase.cs
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

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Mara.Common.Events;
using Mara.Common.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Results;

namespace Mara.Plugins.BetterEmbeds
{
    /// <summary>
    /// Provides base functionality for handling message embeds.
    /// </summary>
    public abstract class BetterEmbedHandlerBase : LoggingEventResponderBase<IMessageCreate>
    {
        private readonly Regex _urlRegex;
        private readonly IDiscordRestChannelAPI _channelApi;
        private readonly ILogger<BetterEmbedHandlerBase> _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="BetterEmbedHandlerBase"/> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}"/> for this instance.</param>
        /// <param name="urlRegex">A <see cref="Regex"/> for filtering the contents of the message.</param>
        /// <param name="channelApi">The <see cref="IDiscordRestChannelAPI"/>.</param>
        /// <param name="jsonSerializerOptions">Json Serialization Options.</param>
        protected BetterEmbedHandlerBase(ILogger<BetterEmbedHandlerBase> logger, Regex urlRegex, IDiscordRestChannelAPI channelApi, IOptions<JsonSerializerOptions> jsonSerializerOptions)
            : base(logger)
        {
            _logger = logger;
            _urlRegex = urlRegex;
            _channelApi = channelApi;
            _jsonSerializerOptions = jsonSerializerOptions.Value;
        }

        /// <inheritdoc/>
        protected override async Task<Result> HandleAsync(IMessageCreate gatewayEvent, CancellationToken cancellationToken = default)
        {
            // We don't care about system messages or calls
            if (gatewayEvent.Type != MessageType.Default)
            {
                return Result.FromSuccess();
            }

            if (!gatewayEvent.IsUserMessage())
            {
                return Result.FromSuccess();
            }

            var matches = _urlRegex.Matches(gatewayEvent.Content);

            var embeds = new List<IEmbed>();
            bool hasPrePostText = false;

            foreach (Match match in matches)
            {
                // If the link is surrounded with <>, skip. The user specifically wanted no embed.
                if (match.Groups["OpenBrace"].Success && match.Groups["CloseBrace"].Success)
                {
                    continue;
                }

                // Build the embed
                var buildEmbed = await BuildEmbedAsync(match, gatewayEvent, cancellationToken);

                if (!buildEmbed.IsSuccess)
                {
                    // TODO: Return error embed
                    return Result.FromError(buildEmbed);
                }

                embeds.Add(buildEmbed.Entity);

                // If there is any text on either side of the URLs, record that.
                if (!match.Groups["Prelink"].Success ||
                    !match.Groups["Postlink"].Success)
                {
                    hasPrePostText = true;
                }
            }

            if (!embeds.Any())
            {
                return Result.FromSuccess();
            }

            _logger.LogTrace("Serialized Embed: {Embed}", JsonSerializer.Serialize(embeds[0], _jsonSerializerOptions));

            // Post the embed(s)
            var postEmbed = await _channelApi.CreateMessageAsync(
                gatewayEvent.ChannelID,
                embeds: embeds,
                ct: cancellationToken
            );

            if (!postEmbed.IsSuccess)
            {
                return Result.FromError(postEmbed);
            }

            // If the post is only comprised of links, delete the invoking method.
            if (!hasPrePostText)
            {
                await _channelApi.DeleteMessageAsync(gatewayEvent.ChannelID, gatewayEvent.ID, "Deleting invocation method.", cancellationToken);
            }

            return Result.FromSuccess();
        }

        /// <summary>
        /// Builds an embed for the service.
        /// </summary>
        /// <param name="match">A <see cref="Match"/> representing a link to the requested service.</param>
        /// <param name="message">The message containing the link.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A <see cref="Result{TEntity}"/> containing an <see cref="IEmbed"/> or a reason why the operation failed.</returns>
        protected abstract ValueTask<Result<IEmbed>> BuildEmbedAsync(Match match, IMessage message, CancellationToken cancellationToken = default);
    }
}
