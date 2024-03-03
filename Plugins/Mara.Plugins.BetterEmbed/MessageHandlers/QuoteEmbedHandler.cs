//
//  QuoteEmbedHandler.cs
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

using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Mara.Common.Errors;
using Mara.Plugins.BetterEmbeds.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Rest.Core;
using Remora.Results;

using DiscordConstants = Remora.Discord.API.Constants;

namespace Mara.Plugins.BetterEmbeds.MessageHandlers
{
    /// <summary>
    /// Handles messages containing Discord Message links.
    /// </summary>
    public sealed class QuoteEmbedHandler : BetterEmbedHandlerBase
    {
        private static readonly Regex UrlRegex = new
        (
            @"(?<Prelink>\S+\s+\S*)?(?<OpenBrace><)?https?://(?:(?:ptb|canary|www)\.)?discord(app)?\.com/channels/(?<GuildId>\d+)/(?<ChannelId>\d+)/(?<MessageId>\d+)/?(?<CloseBrace>>)?(?<Postlink>\S*\s+\S+)?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant
        );

        private readonly IDiscordRestChannelAPI _channelApi;
        private readonly IDiscordRestGuildAPI _guildApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteEmbedHandler"/> class.
        /// </summary>
        /// <param name="logger">A <see cref="ILogger{TCategoryName}"/> for this instance.</param>
        /// <param name="channelApi">The <see cref="IDiscordRestChannelAPI"/>.</param>
        /// <param name="guildApi">The <see cref="IDiscordRestGuildAPI"/>.</param>
        /// <param name="jsonOptions">The <see cref="JsonSerializerOptions"/>.</param>
        public QuoteEmbedHandler
        (
            ILogger<QuoteEmbedHandler> logger,
            IDiscordRestChannelAPI channelApi,
            IDiscordRestGuildAPI guildApi,
            IOptions<JsonSerializerOptions> jsonOptions)
            : base(logger, UrlRegex, channelApi, jsonOptions)
        {
            _channelApi = channelApi;
            _guildApi = guildApi;
        }

        /// <inheritdoc />
        protected override async ValueTask<Result<IEmbed>> BuildEmbedAsync(Match match, IMessage message, CancellationToken cancellationToken = default)
        {
            if (!ulong.TryParse(match.Groups["GuildId"].Value, out var sourceGuildId))
            {
                return new RegexError(message.Content, "Guild Id not found.");
            }

            if (!ulong.TryParse(match.Groups["ChannelId"].Value, out var sourceChannelId))
            {
                return new RegexError(message.Content, "Channel Id not found.");
            }

            if (!ulong.TryParse(match.Groups["MessageId"].Value, out var sourceMessageId))
            {
                return new RegexError(message.Content, "Message Id not found.");
            }

            // Get the channel the quoted message comes from
            var sourceChannelSnowflake = new Snowflake(sourceChannelId);
            var channelResult = await _channelApi.GetChannelAsync(sourceChannelSnowflake, cancellationToken);

            // Get the destination channel
            var destChannelResult = sourceChannelSnowflake.Equals(message.ChannelID)
                ? channelResult
                : await _channelApi.GetChannelAsync(message.ChannelID, cancellationToken); // Avoids second API call if they're the same channel.

            if (!channelResult.IsDefined(out var sourceChannel))
            {
                return Result<IEmbed>.FromError(channelResult);
            }
            if (!destChannelResult.IsDefined(out var destChannel))
            {
                return Result<IEmbed>.FromError(channelResult);
            }

            // Try to get the quoted message
            var quotedMessageResult =
                await _channelApi.GetChannelMessageAsync(sourceChannelSnowflake, new Snowflake(sourceMessageId), cancellationToken);
            if (!quotedMessageResult.IsDefined(out var quotedMessage))
            {
                return Result<IEmbed>.FromError(quotedMessageResult);
            }

            // Try to get the source guild of the quoted message
            var quotedMessageGuildResult =
                await _guildApi.GetGuildAsync(new Snowflake(sourceGuildId, DiscordConstants.DiscordEpoch), false, cancellationToken);
            if (!quotedMessageGuildResult.IsDefined(out var quotedMessageGuild))
            {
                return Result<IEmbed>.FromError(quotedMessageGuildResult);
            }

            var embedBuilder = new QuoteEmbedBuilder(quotedMessage, message.Author, sourceChannel, destChannel, quotedMessageGuild);

            return embedBuilder.Build();
        }
    }
}
