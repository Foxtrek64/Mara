//
//  RedditPost.cs
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
using JetBrains.Annotations;
using Remora.Rest.Core;

namespace Mara.Plugins.BetterEmbeds.Models.Reddit
{
    /// <inheritdoc />
    [PublicAPI]
    public record RedditPost
    (
        string Id,
        string Title,
        string Subreddit,
        string Author,
        string Url,
        string Permalink,
        Optional<string> Text,
        int Score,
        double UpvoteRatio,
        DateTime PostDate,
        Optional<string> PostFlair,
        Optional<Media> Media,
        bool IsVideo,
        Optional<string> PostHint,
        string WhitelistStatus,
        string Thumbnail,
        Optional<int?> ThumbnailWidth,
        Optional<int?> ThumbnailHeight
    ) : IRedditPost;
}
