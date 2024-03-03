//
//  UserService.cs
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
using System.Threading;
using System.Threading.Tasks;
using Mara.Plugins.Moderation.Models;
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
        /// Registers a new user.
        /// </summary>
        /// <param name="userId">The unique id of the user.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A result indicating success or failure.</returns>
        public async Task<Result> RegisterNewUser(Snowflake userId, CancellationToken cancellationToken = default)
        {
            var userInfo = new UserInformation
            {
                Id = userId,
                FirstSeen = DateTimeOffset.UtcNow,
                LastSeen = DateTimeOffset.UtcNow
            };

            _ = await _dbContext.UserInfo.AddAsync(userInfo, cancellationToken);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.FromSuccess();
        }

        /// <summary>
        /// Retrieves information about a specified user using their snowflake id.
        /// </summary>
        /// <param name="userId">The user's unique id.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A <see cref="UserInformation"/> model containing information about the user.</returns>
        public async Task<Result<UserInformation>> GetUserInformation(Snowflake userId, CancellationToken cancellationToken = default)
        {
            var result = await _dbContext.UserInfo.FindAsync(new object?[] { userId }, cancellationToken: cancellationToken);

            return result is { }
                ? result
                : new NotFoundError();
        }

        /// <summary>
        /// Updates the last seen date of the user to the current UTC timestamp.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A result indicating success or failure.</returns>
        public async Task<Result> UpdateLastSeen(Snowflake userId, CancellationToken cancellationToken = default)
        {
            var userInfoResult = await GetUserInformation(userId, cancellationToken);
            if (!userInfoResult.IsSuccess)
            {
                return Result.FromError(userInfoResult);
            }

            var user = userInfoResult.Entity;
            user.LastSeen = DateTimeOffset.UtcNow;
            _ = _dbContext.SaveChangesAsync(cancellationToken);
            return Result.FromSuccess();
        }

        /// <summary>
        /// Creates a record of an infraction against the user.
        /// </summary>
        /// <param name="userId">The target of the infraction.</param>
        /// <param name="infractionKind">The kind of infraction.</param>
        /// <param name="guildId">The guild in which the infraction took place.</param>
        /// <param name="moderator">The moderator responsible for filing the infraction.</param>
        /// <param name="reason">Details about the infraction or situation.</param>
        /// <param name="expiration">If the punishment is one of <see cref="InfractionKind.Mute"/>, <see cref="InfractionKind.Ban"/>, <see cref="InfractionKind.SoftBan"/>, or <see cref="InfractionKind.GlobalBan"/>, this sets the amount of time before the infraction is automatically rescinded.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A result indicating whether the infraction record was successfully created.</returns>
        public async Task<Result<Infraction>> CreateInfractionRecord
        (
            Snowflake userId,
            InfractionKind infractionKind,
            Snowflake guildId,
            Snowflake moderator,
            string? reason = null,
            DateTimeOffset? expiration = null,
            CancellationToken cancellationToken = default
        )
        {
            var user = await _dbContext.UserInfo.FindAsync(new[] { userId }, cancellationToken) ??
                new UserInformation
                {
                    Id = userId,
                    FirstSeen = DateTimeOffset.UtcNow,
                    LastSeen = DateTimeOffset.UtcNow
                };

            var infraction = new Infraction()
            {
                User = userId,
                InfractionKind = infractionKind,
                GuildId = guildId,
                ResponsibleModerator = moderator,
                Reason = reason,
                PunishmentExpiration =
                    infractionKind is InfractionKind.Mute or
                                      InfractionKind.Ban or
                                      InfractionKind.SoftBan or
                                      InfractionKind.GlobalBan
                        ? expiration
                        : null,
                Rescinded = false,
                RescindingUser = null,
                TimeStamp = DateTimeOffset.UtcNow,
                UserInformation = user
            };

            user.AddInfraction(infraction);
            _ = await _dbContext.SaveChangesAsync(cancellationToken);

            return infraction;
        }

        /// <summary>
        /// Flags an infraction as rescinded. This action may be performed automatically.
        /// </summary>
        /// <param name="infractionId">The unique id of the infraction.</param>
        /// <param name="moderator">The moderator rescinding the infraction.</param>
        /// <param name="cancellationToken">A cancellation token for this operation.</param>
        /// <returns>A <see cref="Result"/> containing the infraction that was modified.</returns>
        public async Task<Result<Infraction>> RescindInfraction
        (
            int infractionId,
            Snowflake moderator,
            CancellationToken cancellationToken
        )
        {
            var infraction = await _dbContext.Infractions.FindAsync(new { infractionId }, cancellationToken);

            if (infraction is null)
            {
                return new NotFoundError($"Could not find an infraction with id {infractionId}");
            }

            infraction.Rescinded = true;
            infraction.RescindingUser = moderator;

            _ = await _dbContext.SaveChangesAsync(cancellationToken);

            return infraction;
        }
    }
}
