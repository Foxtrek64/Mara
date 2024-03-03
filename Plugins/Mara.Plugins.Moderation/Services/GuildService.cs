//
//  GuildService.cs
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
using Mara.Plugins.Moderation.Models;
using Microsoft.EntityFrameworkCore;
using Remora.Rest.Core;
using Remora.Results;

namespace Mara.Plugins.Moderation.Services
{
    /// <summary>
    /// A service for interacting with the guild configuration.
    /// </summary>
    public class GuildService
    {
        private readonly ModerationContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuildService"/> class.
        /// </summary>
        /// <param name="moderationContext">A database context for storing and retrieving user information.</param>
        public GuildService(ModerationContext moderationContext)
        {
            _dbContext = moderationContext;
        }

        /// <summary>
        /// Finds a guild configuration with the provided guild id.
        /// </summary>
        /// <param name="guildId">The guild id to search for.</param>
        /// <param name="cancellationToken">the cancellation token for this operation.</param>
        /// <returns>A result indicating success or failure.</returns>
        private async Task<Result<GuildConfiguration>> GetGuildConfiguration(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            var config = await _dbContext.GuildConfigurations.FirstOrDefaultAsync(x => x.GuildId == guildId, cancellationToken);
            return config ?? Result<GuildConfiguration>.FromError(new NotFoundError($"Could not find a guild configuration for guild id '{guildId}'."));
        }

        /// <summary>
        /// Gets the currently configured mute role id, if available.
        /// </summary>
        /// <param name="guildId">The id of the guild.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A result containing a nullable snowflake.</returns>
        public async Task<Result<Snowflake>> GetMuteRoleIdAsync(Snowflake guildId, CancellationToken cancellationToken = default)
        {
            var configResult = await GetGuildConfiguration(guildId, cancellationToken);
            if (!configResult.IsSuccess)
            {
                return Result<Snowflake>.FromError(configResult);
            }

            return configResult.Entity.MuteRoleId ??
                Result<Snowflake>.FromError(new InvalidOperationError("Mute role id has not been configured."));
        }

        /// <summary>
        /// Sets the id of the mute role for the specified guild.
        /// </summary>
        /// <param name="guildId">The id of the guild to configure.</param>
        /// <param name="muteRoleId">The id of the role to apply when muting people.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A result indicating success or failure of the operation.</returns>
        public async Task<Result> SetMuteRoleId(Snowflake guildId, Snowflake muteRoleId, CancellationToken cancellationToken)
        {
            var entity = await GetGuildConfiguration(guildId, cancellationToken);
            if (!entity.IsSuccess)
            {
                return Result.FromError(entity);
            }

            var config = entity.Entity;
            config.MuteRoleId = muteRoleId;
            _ = _dbContext.SaveChangesAsync(cancellationToken);
            return Result.FromSuccess();
        }
    }
}
