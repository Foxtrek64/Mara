//
//  RedditEmbedHandler.cs
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
using Mara.Plugins.BetterEmbeds.API;
using Mara.Plugins.BetterEmbeds.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Results;

namespace Mara.Plugins.BetterEmbeds.MessageHandlers
{
    /// <summary>
    /// Handles messages containing reddit post links.
    /// </summary>
    public sealed class RedditEmbedHandler : BetterEmbedHandlerBase
    {
        private static readonly Regex UrlRegex = new
        (
            @"(?<Prelink>\S+\s+\S*)?(?<OpenBrace><)?https?://(?:(?:www)\.)?reddit\.com/r/(?<Subreddit>[A-Za-z0-9_]+)/comments/(?<PostId>[a-z0-9]+)/(?<PostName>[a-z0-9_]+)?(?:/?(?:\?[a-z_=&]+)?)?(?<CloseBrace>>)?(?<PostLink>\S*\s+\S+)?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant
        );

        private readonly IDiscordRestChannelAPI _channelApi;
        private readonly IRedditRestAPI _redditApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedditEmbedHandler"/> class.
        /// </summary>
        /// <param name="logger">A <see cref="ILogger{TCategoryName}"/> for this instance.</param>
        /// <param name="channelApi">The <see cref="IDiscordRestChannelAPI"/>.</param>
        /// <param name="redditApi">The <see cref="IRedditRestAPI"/>.</param>
        /// <param name="jsonOptions">The <see cref="JsonSerializerOptions"/>.</param>
        public RedditEmbedHandler
        (
            ILogger<RedditEmbedHandler> logger,
            IDiscordRestChannelAPI channelApi,
            IRedditRestAPI redditApi,
            IOptions<JsonSerializerOptions> jsonOptions
        )
        : base(logger, UrlRegex, channelApi, jsonOptions)
        {
            _channelApi = channelApi;
            _redditApi = redditApi;
        }

        /// <inheritdoc />
        public override async ValueTask<Result<IEmbed>> BuildEmbedAsync(Match match, IMessage message, CancellationToken cancellationToken = default)
        {
            var subreddit = match.Groups["Subreddit"].Value;
            var postId = match.Groups["PostId"].Value;

            // Get the channel to send the reddit embed in
            var channelResult = await _channelApi.GetChannelAsync(message.ChannelID, cancellationToken);
            if (!channelResult.IsDefined(out var channel))
            {
                return Result<IEmbed>.FromError(channelResult);
            }

            // Ensure channel is guild channel
            if (!channel.GuildID.IsDefined())
            {
                return new GuildRequiredError("Destination channel must be a guild text channel.");
            }

            // If we can't determine whether the channels is nsfw, assume it's not.
            if (!channel.IsNsfw.IsDefined(out bool isChannelNsfw))
            {
                isChannelNsfw = false;
            }

            // Get the reddit post
            var redditResult = await _redditApi.GetRedditPostAsync(subreddit, postId, false, cancellationToken);
            if (!redditResult.IsDefined(out var redditPost))
            {
                return Result<IEmbed>.FromError(redditResult);
            }

            // Get the reddit user
            var userResult = await _redditApi.GetRedditUserAsync(redditPost.Author, false, cancellationToken);
            if (!userResult.IsDefined(out var user))
            {
                return Result<IEmbed>.FromError(userResult);
            }

            // Get a new embed builder
            var embedBuilder = new RedditEmbedBuilder(redditPost, user, isChannelNsfw);
            return embedBuilder.Build();
        }
    }
}
