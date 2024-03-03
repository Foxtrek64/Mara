//
//  Priority.cs
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

using System.Drawing;
using JetBrains.Annotations;

namespace Mara.Plugins.HelpDesk.Models
{
    /// <summary>
    /// Represents a priority configuration.
    /// </summary>
    /// <param name="Id">The unique id of this priority.</param>
    /// <param name="DefaultName">The default (en-US) name of this priority.</param>
    /// <param name="Color">The color associated with this priority.</param>
    [PublicAPI]
    public sealed record class Priority
    (
        int Id,
        string DefaultName,
        Color Color
    );
}
