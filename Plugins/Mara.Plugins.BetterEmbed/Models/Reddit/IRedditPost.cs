//
//  IRedditPost.cs
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
using Remora.Rest.Core;

namespace Mara.Plugins.BetterEmbeds.Models.Reddit
{
    /// <summary>
    /// A post on Reddit.
    /// </summary>
    public interface IRedditPost
    {
        /// <summary>
        /// Gets the unique id of the post.
        /// </summary>
        public string Id { get; init; }

        /// <summary>
        /// Gets the title of the post.
        /// </summary>
        public string Title { get; init; }

        /// <summary>
        /// Gets the name of the subreddit this post belongs to.
        /// </summary>
        public string Subreddit { get; init; }

        /// <summary>
        /// Gets the name of the author of the post.
        /// </summary>
        public string Author { get; init; }

        /// <summary>
        /// Gets the URI portion of the link, beginning with r/.
        /// </summary>
        public string Url { get; init; }

        /// <summary>
        /// Gets the permalink to the post.
        /// </summary>
        public string Permalink { get; init; }

        /// <summary>
        /// Gets the text of the post, if any.
        /// </summary>
        public Optional<string> Text { get; init; }

        /// <summary>
        /// Gets the current score of the post.
        /// </summary>
        public int Score { get; init; }

        /// <summary>
        /// Gets a ratio of upvotes to downvotes.
        /// </summary>
        public double UpvoteRatio { get; init; }

        /// <summary>
        /// Gets the date the post was created.
        /// </summary>
        public DateTime PostDate { get; init; }

        /// <summary>
        /// Gets the post's flair, if any.
        /// </summary>
        public Optional<string> PostFlair { get; init; }

        /// <summary>
        /// Gets the embedded media of a post, if any.
        /// </summary>
        public Optional<Media> Media { get; init; }

        /// <summary>
        /// Gets a value indicating whether a post contains a video.
        /// </summary>
        public bool IsVideo { get; init; }

        /// <summary>
        /// Gets a hint indicating what kind of media is contained in the post.
        /// <list type="table">
        ///     <item>
        ///         <term>Not available or null</term>
        ///         <description>Text</description>
        ///     </item>
        ///     <item>
        ///         <term>rich:video</term>
        ///         <description><see cref="OEmbed.OEmbed"/></description>
        ///     </item>
        ///     <item>
        ///         <term>hosted:video</term>
        ///         <description><see cref="RedditVideo"/></description>
        ///     </item>
        /// </list>
        /// </summary>
        public Optional<string> PostHint { get; init; }

        /// <summary>
        /// Gets a flag indicating whether the post is NSFW or a spoiler.
        /// </summary>
        public string WhitelistStatus { get; init; }

        /// <summary>
        /// Gets the thumbnail for the post.
        /// </summary>
        public string Thumbnail { get; init; }

        /// <summary>
        /// Gets the width of the thumbnail.
        /// </summary>
        public Optional<int?> ThumbnailWidth { get; init; }

        /// <summary>
        /// Gets the height of the thumbnail.
        /// </summary>
        public Optional<int?> ThumbnailHeight { get; init; }
    }
}
