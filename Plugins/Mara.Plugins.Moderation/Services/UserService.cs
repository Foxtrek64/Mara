//
//  UserService.cs
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

using System.Threading.Tasks;
using Mara.Plugins.Moderation.Models;
using Microsoft.EntityFrameworkCore;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Rest.Core;
using Remora.Results;

namespace Mara.Plugins.Moderation.Services
{
    /// <summary>
    /// A service for interacting with users.
    /// </summary>
    public sealed class UserService
    {
        private readonly ModerationContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="moderationContext">A database context for storing and retrieving user information.</param>
        public UserService(ModerationContext moderationContext)
        {
            _dbContext = moderationContext;
        }

        /// <summary>
        /// Retrieves information about a specified user using an <see cref="IUser"/> object.
        /// </summary>
        /// <param name="user">The user to receive information for.</param>
        /// <returns>A <see cref="UserInformation"/> model containing information about the user.</returns>
        public Task<Result<UserInformation>> GetUserInformation(IUser user)
            => GetUserInformation(user.ID);

        /// <summary>
        /// Retrieves information about a specified user using their snowflake id.
        /// </summary>
        /// <param name="snowflake">The user's unique id.</param>
        /// <returns>A <see cref="UserInformation"/> model containing information about the user.</returns>
        public async Task<Result<UserInformation>> GetUserInformation(Snowflake snowflake)
        {
            var result = await _dbContext.UserInfo.FirstOrDefaultAsync(x => x.Id == snowflake);

            if (result == default)
            {
                return new NotFoundError();
            }

            return Result<UserInformation>.FromSuccess(result!);
        }
    }
}
