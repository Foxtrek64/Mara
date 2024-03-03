//
//  Consent.cs
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

namespace Mara.Plugins.Consent.Models
{
    /// <summary>
    /// A database model which represents a user consent.
    /// </summary>
    public sealed class Consent
    {
        /// <summary>
        /// Gets the unique ID of the user.
        /// </summary>
        public Snowflake Id { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether the user has given consent.
        /// </summary>
        /// <remarks>True = consent explicitly granted. False = consent explicitly denied. Null = Pending response.</remarks>
        public ConsentStatus GivesConsent { get; set; }
    }
}
