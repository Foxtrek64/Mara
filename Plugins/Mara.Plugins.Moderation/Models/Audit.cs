//
//  Audit.cs
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

using System.Collections.Generic;
using Remora.Rest.Core;

namespace Mara.Plugins.Moderation.Models
{
    /// <summary>
    /// Represents an action that takes place on a server.
    /// </summary>
    public sealed class Audit
    {
        /// <summary>
        /// Gets or sets the audit's unique id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the id of the Guild where this event took place.
        /// </summary>
        public Snowflake GuildId { get; set; }

        /// <summary>
        /// Gets or sets the user or bot responsible for the action.
        /// </summary>
        public Snowflake Source { get; set; }

        /// <summary>
        /// Gets or sets the kind of event that took place.
        /// </summary>
        public EventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the target of the action.
        /// </summary>
        public Snowflake Target { get; set; }

        /// <summary>
        /// Gets or sets a collection of actions taken during this change.
        /// </summary>
        public List<AuditAction> AuditActions { get; set; } = new();

        /// <summary>
        /// Gets or sets the change number.
        /// </summary>
        public int? ChangeNumber { get; set; }

        /// <summary>
        /// Gets or sets a user-provided comment regarding the audit entry.
        /// </summary>
        public string? Comment { get; set; }
    }
}
