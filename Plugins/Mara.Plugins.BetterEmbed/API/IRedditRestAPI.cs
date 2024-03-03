//
//  IRedditRestAPI.cs
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
using Remora.Results;

namespace Mara.Plugins.BetterEmbeds.API
{
    /// <summary>
    /// Provides a RestClient wrapper which accesses the Reddit API.
    /// </summary>
    public interface IRedditRestAPI
    {
        /// <summary>
        /// Gets a post using the subreddit and post id.
        /// </summary>
        /// <param name="subredditName">The subreddit this post belongs to.</param>
        /// <param name="postId">The unique id of this post.</param>
        /// <param name="allowNullReturn">Whether or not to allow an empty return value.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<Result<IRedditPost>> GetRedditPostAsync
        (
            string subredditName,
            string postId,
            bool allowNullReturn = false,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets a reddit user by username.
        /// </summary>
        /// <param name="username">The user's username.</param>
        /// <param name="allowNullReturn">Whether or not to allow an empty return value.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        Task<Result<IRedditUser>> GetRedditUserAsync
        (
            string username,
            bool allowNullReturn = false,
            CancellationToken cancellationToken = default
        );
    }
}
