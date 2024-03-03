//
//  CoreKeyHelpers.cs
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

using System.Text;
using JetBrains.Annotations;
using Remora.Discord.Caching;

namespace Mara.Plugins.Core
{
    /// <summary>
    /// Provides a set of cache keys for the Core plugin.
    /// </summary>
    [PublicAPI]
    public static class CoreKeyHelpers
    {
        /// <summary>
        /// Creates a cache key for the startup time of the bot.
        /// </summary>
        public record StartupTimeCacheKey() : KeyHelpers.CurrentUserCacheKey
        {
            /// <inheritdoc />
            protected override StringBuilder AppendToString(StringBuilder stringBuilder)
                => base.AppendToString(stringBuilder).Append(":StartupTime");
        }
    }
}
