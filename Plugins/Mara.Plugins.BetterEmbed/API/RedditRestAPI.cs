//
//  RedditRestAPI.cs
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
using System.Threading;
using System.Threading.Tasks;
using Mara.Plugins.BetterEmbeds.Models.Reddit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Remora.Rest;
using Remora.Results;

namespace Mara.Plugins.BetterEmbeds.API
{
    /// <inheritdoc />
    public sealed class RedditRestAPI : IRedditRestAPI
    {
        public const string PostUrl = "https://www.reddit.com/r/{0}/comments/{1}/.json";
        public const string ProfileUrl = "https://www.reddit.com/user/{0}/about.json";

        private readonly JsonSerializerOptions _serializerOptions;
        private readonly ILogger<RedditRestAPI> _logger;
        private readonly IRestHttpClient _restClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedditRestAPI"/> class.
        /// </summary>
        /// <param name="restClient">The rest client.</param>
        /// <param name="serializerOptions">Serialization options.</param>
        /// <param name="logger">A logger</param>
        public RedditRestAPI(IRestHttpClient restClient, IOptions<JsonSerializerOptions> serializerOptions, ILogger<RedditRestAPI> logger)
        {
            _restClient = restClient;
            _serializerOptions = serializerOptions.Value;
            _logger = logger;
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
