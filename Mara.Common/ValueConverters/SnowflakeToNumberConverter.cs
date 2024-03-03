//
//  SnowflakeToNumberConverter.cs
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

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Remora.Rest.Core;
using DiscordConstants = Remora.Discord.API.Constants;

namespace Mara.Common.ValueConverters
{
    /// <summary>
    /// Converts a Snowflake unto a ulong and back.
    /// </summary>
    [PublicAPI]
    public sealed class SnowflakeToNumberConverter : ValueConverter<Snowflake, ulong>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnowflakeToNumberConverter"/> class.
        /// </summary>
        public SnowflakeToNumberConverter()
            : base
            (
                sf => sf.Value,
                value => new Snowflake(value, DiscordConstants.DiscordEpoch)
            )
        {
        }
    }
}
