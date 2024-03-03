//
//  OperationContextExtensions.cs
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
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Rest.Core;

namespace Mara.Common.Extensions
{
    /// <summary>
    /// Provides a set of extensions for <see cref="IOperationContext"/>.
    /// </summary>
    public static class OperationContextExtensions
    {
        /// <summary>
        /// Gets a guild id or throws.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The guild id.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no guild is specified.</exception>
        public static Snowflake GetGuildId(this IOperationContext context)
        {
            if (context.TryGetGuildID(out Snowflake guildId))
            {
                return guildId;
            }

            throw new InvalidOperationException("Context does not specify a guild id.");
        }

        /// <summary>
        /// Gets a user id or throws.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The guild id.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no user is specified.</exception>
        public static Snowflake GetUserId(this IOperationContext context)
        {
            if (context.TryGetUserID(out Snowflake userId))
            {
                return userId;
            }

            throw new InvalidOperationException("Context does not specify a user id.");
        }
    }
}
