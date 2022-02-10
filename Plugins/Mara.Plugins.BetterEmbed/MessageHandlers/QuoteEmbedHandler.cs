//
//  QuoteEmbedHandler.cs
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

using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Mara.Common.Results;
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
        public override async ValueTask<Result<IEmbed>> BuildEmbedAsync(Match match, IMessage message, CancellationToken cancellationToken = default)
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

            // The destination channel must be in a guild.
            if (!message.GuildID.HasValue)
            {
                return new GuildRequiredError("Quoted messages must be sent to a guild channel.");
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
