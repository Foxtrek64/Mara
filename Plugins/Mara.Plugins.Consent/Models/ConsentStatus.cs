//
//  ConsentStatus.cs
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

namespace Mara.Plugins.Consent.Models
{
    /// <summary>
    /// Enumerates consent status states.
    /// </summary>
    public enum ConsentStatus
    {
        /// <summary>
        /// User has not been asked consent yet.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// User has granted consent.
        /// </summary>
        Granted = 1,

        /// <summary>
        /// User has denied consent.
        /// </summary>
        Denied = 2,

        /// <summary>
        /// User has revoked consent.
        /// </summary>
        Revoked = 3
    }
}
