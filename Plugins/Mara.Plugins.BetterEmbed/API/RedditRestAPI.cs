//
//  RedditRestAPI.cs
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

using System.Threading;
using System.Threading.Tasks;
using Mara.Plugins.BetterEmbeds.Models.Reddit;
using Remora.Rest;
using Remora.Results;

namespace Mara.Plugins.BetterEmbeds.API
{
    /// <inheritdoc />
    public sealed class RedditRestAPI : IRedditRestAPI
    {
        /// <summary>
        /// A format string for a reddit post url.
        /// </summary>
        internal const string PostUrl = "https://www.reddit.com/r/{0}/comments/{1}";

        /// <summary>
        /// A format string for a reddit profile url.
        /// </summary>
        internal const string ProfileUrl = "https://www.reddit.com/user/{0}";

        /// <summary>
        /// The link to the favicon used for reddit.com.
        /// </summary>
        internal const string RedditFavicon = "https://www.redditstatic.com/desktop2x/img/favicon/android-icon-192x192.png";

        private readonly IRestHttpClient _restClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedditRestAPI"/> class.
        /// </summary>
        /// <param name="restClient">The rest client.</param>
        public RedditRestAPI(IRestHttpClient restClient)
        {
            _restClient = restClient;
        }

        /// <inheritdoc />
        public async Task<Result<IRedditPost>> GetRedditPostAsync
        (
            string subredditName,
            string postId,
            bool allowNullReturn = false,
            CancellationToken cancellationToken = default
        )
        {
            var redditUrl = string.Format(PostUrl, subredditName, postId);

            return await _restClient.GetAsync<IRedditPost>
            (
                redditUrl,
                "$[0].data.children[0].data",
                x => x.Build(),
                allowNullReturn,
                cancellationToken
            );
        }

        /// <inheritdoc />
        public async Task<Result<IRedditUser>> GetRedditUserAsync
        (
            string username,
            bool allowNullReturn = false,
            CancellationToken cancellationToken = default
        )
        {
            var redditUrl = string.Format(ProfileUrl, username);

            return await _restClient.GetAsync<IRedditUser>
            (
                redditUrl,
                "$.data.subreddit",
                x => x.Build(),
                allowNullReturn,
                cancellationToken
            );
        }
    }
}
