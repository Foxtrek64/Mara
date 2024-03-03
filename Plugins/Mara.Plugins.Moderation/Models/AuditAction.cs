//
//  AuditAction.cs
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

namespace Mara.Plugins.Moderation.Models
{
    /// <summary>
    /// Represents an action taken during an audit, such as a channel name change or the user that was banned.
    /// </summary>
    public sealed class AuditAction
    {
        /// <summary>
        /// Gets or sets the unique identifier for this item.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a reference back to the audit this action belongs to.
        /// </summary>
        public Audit Audit { get; set; } = new();
    }
}
