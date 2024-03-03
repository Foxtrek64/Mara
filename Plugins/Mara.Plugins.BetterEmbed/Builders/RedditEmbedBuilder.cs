//
//  RedditEmbedBuilder.cs
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
using System.ComponentModel;
using System.Drawing;
using Mara.Common.Discord.Feedback.Errors;
using Mara.Plugins.BetterEmbeds.API;
using Mara.Plugins.BetterEmbeds.Models.Reddit;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Extensions.Builder;
using Remora.Discord.Extensions.Embeds;
using Remora.Results;

namespace Mara.Plugins.BetterEmbeds.Builders
{
    /// <summary>
    /// A service which builds an embed based off a reddit post.
    /// </summary>
    internal sealed class RedditEmbedBuilder : BuilderBase<IEmbed>
    {
        /// <summary>
        /// Gets the <see cref="IRedditPost"/> to use to build the embed.
        /// </summary>
        public IRedditPost RedditPost { get; }

        /// <summary>
        /// Gets the <see cref="IRedditUser"/> who created the post.
        /// </summary>
        public IRedditUser RedditUser { get; }

        /// <summary>
        /// Gets a value indicating whether an embed should be built if the reddit post is not safe for work.
        /// </summary>
        public bool AllowNSFW { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedditEmbedBuilder"/> class.
        /// </summary>
        /// <param name="redditPost">The post to use to build the embed.</param>
        /// <param name="redditUser">The user who created the post.</param>
        /// <param name="allowNSFW">Whether to build an embed for a NSFW post.</param>
        public RedditEmbedBuilder(IRedditPost redditPost, IRedditUser redditUser, bool allowNSFW)
        {
            RedditPost = redditPost;
            RedditUser = redditUser;
            AllowNSFW = allowNSFW;
        }

        /// <inheritdoc/>
        public override Result<IEmbed> Build()
        {
            // Ensure the embed can be built.
            var validateResult = Validate();
            if (!validateResult.IsSuccess)
            {
                return Result<IEmbed>.FromError(validateResult);
            }

            var userUrl = string.Format(RedditRestAPI.ProfileUrl, RedditPost.Author);
            var userIconUrl = new Uri(RedditUser.IconImage).GetLeftPart(UriPartial.Path);

            var embedBuilder = new EmbedBuilder()
                .WithTitle(RedditPost.Title)
                .WithUrl(string.Format(RedditRestAPI.PostUrl, RedditPost.Subreddit, RedditPost.Id))
                .WithAuthor(RedditPost.Author, userUrl, userIconUrl)
                .WithFooter
                (
                    $"Posted on {RedditPost.Subreddit}",
                    RedditRestAPI.RedditFavicon
                )
                .WithTimestamp(RedditPost.PostDate)
                .WithColour(Color.DimGray);

            embedBuilder.AddField("Score", $"{RedditPost.Score} ({RedditPost.UpvoteRatio * 100}%)", true);

            if (RedditPost.PostFlair.IsDefined(out var flair))
            {
                if (flair.Contains(':'))
                {
                    var parts = flair.Split(':', StringSplitOptions.TrimEntries);
                    embedBuilder.AddField($"{parts[0]}:", parts[1], true);
                }
                else
                {
                    embedBuilder.AddField("Post Flair:", flair, true);
                }
            }

            var buildResult = embedBuilder.Build();

            return buildResult.IsDefined(out var embed)
                ? embed
                : Result<IEmbed>.FromError(buildResult);
        }

        /// <summary>
        /// Ensures that the embed can be built based on the NSFW status of the post.
        /// </summary>
        /// <returns>True if the embed can be built; otherwise, false.</returns>
        public override Result Validate()
        {
            // Is the post flagged NSFW
            if (RedditPost.WhitelistStatus.Contains("nsfw") && !AllowNSFW)
            {
                return new NotSafeForWorkError("The reddit post is flagged as NSFW but the receiving channel is not.");
            }

            return Result.FromSuccess();
        }
    }
}
