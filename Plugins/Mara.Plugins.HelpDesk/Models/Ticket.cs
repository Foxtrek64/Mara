//
//  Ticket.cs
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

using Remora.Rest.Core;

namespace Mara.Plugins.HelpDesk.Models
{
    /// <summary>
    /// Represents a support ticket.
    /// </summary>
    /// <param name="Title">The title of the ticket.</param>
    /// <param name="Body">The original body of the ticket.</param>
    /// <param name="Owner">The id of the owner who opened the ticket.</param>
    /// <param name="Priority">The priority of the ticket.</param>
    public sealed record class Ticket
    (
        string Title,
        string Body,
        Snowflake Owner,
        Priority Priority
    )
    {
        /// <summary>
        /// Gets the unique id of this ticket.
        /// </summary>
        public int Id { get; init; }
    }
}
