//
//  UserInformation.cs
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
using System.Collections.Generic;
using Remora.Rest.Core;

namespace Mara.Plugins.Moderation.Models
{
    /// <summary>
    /// Information about a particular user.
    /// </summary>
    public sealed class UserInformation
    {
        /// <summary>
        /// Gets the unique id of the user.
        /// </summary>
        public Snowflake Id { get; init; }

        /// <summary>
        /// Gets the time and date this user was first seen.
        /// </summary>
        public DateTimeOffset FirstSeen { get; init; }

        /// <summary>
        /// Gets or sets the date and time this user was most recently seen.
        /// </summary>
        public DateTimeOffset LastSeen { get; set; }

        /// <summary>
        /// Gets a list of infractions this user has been awarded.
        /// </summary>
        public IReadOnlyList<Infraction> Infractions => _infractions.AsReadOnly();

        /// <summary>
        /// A list of infractions belonging to this user.
        /// </summary>
        private readonly List<Infraction> _infractions = new();

        /// <summary>
        /// Adds a new infraction to the infractions for this user.
        /// </summary>
        /// <param name="infraction">The infraction to add.</param>
        // Internal to ensure consumers go through the UserService.
        internal void AddInfraction(Infraction infraction) => _infractions.Add(infraction);
    }
}
