//
//  Infraction.cs
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

#pragma warning disable CS8618 // Set value of non-nullable property

namespace Mara.Plugins.Moderation.Models
{
    /// <summary>
    /// An EF model representing a user infraction.
    /// </summary>
    public sealed class Infraction
    {
        /// <summary>
        /// Gets or sets the unique identifier of this infraction.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets the unique id of the user being awarded punishment.
        /// </summary>
        public Snowflake User { get; init; }

        /// <summary>
        /// Gets the guild id in which this infraction took place.
        /// </summary>
        public Snowflake GuildId { get; init; }

        /// <summary>
        /// Gets the date and time the infraction took place.
        /// </summary>
        public DateTimeOffset TimeStamp { get; init; }

        /// <summary>
        /// Gets the kind of action was taken by the responsible moderator.
        /// </summary>
        public InfractionKind InfractionKind { get; init; }

        /// <summary>
        /// Gets the date and time at which a temporary punishment expires. If the punishment is not temporary, this will be null.
        /// </summary>
        public DateTimeOffset? PunishmentExpiration { get; init; }

        /// <summary>
        /// Gets the unique id of the responsible for resolving the issue.
        /// </summary>
        public Snowflake ResponsibleModerator { get; init; }

        /// <summary>
        /// Gets or sets additional information about this infraction.
        /// </summary>
        public string? Reason { get; set; } = null;

        /// <summary>
        /// Gets a value indicating whether this infraction was rescinded.
        /// </summary>
        public bool Rescinded { get; internal set; } = false;

        /// <summary>
        /// Gets the unique id of the user who rescinded this infraction.
        /// </summary>
        public Snowflake? RescindingUser { get; internal set; }

        /// <summary>
        /// Gets the <see cref="UserInformation"/> this infraction belongs to, for setting up the relationship.
        /// </summary>
        public UserInformation UserInformation { get; init; }
    }
}
