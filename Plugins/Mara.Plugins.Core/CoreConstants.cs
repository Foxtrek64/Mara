//
//  CoreConstants.cs
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
using Remora.Discord.API.Objects;

namespace Mara.Plugins.Core
{
    /// <summary>
    /// A set of constants for the Core plugin.
    /// </summary>
    public static class CoreConstants
    {
        /// <summary>
        /// Gets a default footer that is used on dismissible embeds.
        /// </summary>
        /// <remarks>
        /// React with ❌ to remove this embed.
        /// </remarks>
        public static readonly EmbedFooter DismissibleEmbedFooter = new("React with ❌ to remove this embed.");

        /// <summary>
        /// A collection of keys for IMemoryCache objects.
        /// </summary>
        [Obsolete("Use Remora Caching", true)]
        public static class CacheKeys
        {
            /// <summary>
            /// A cache key for storing and retrieving the current bot user.
            /// </summary>
            public const string BotUser = nameof(BotUser);

            /// <summary>
            /// A cache key for storing and retrieving the current application.
            /// </summary>
            public const string CurrentApplication = nameof(CurrentApplication);

            /// <summary>
            /// A cache key for storing and retrieving the current shard number.
            /// </summary>
            public const string ShardNumber = nameof(ShardNumber);

            /// <summary>
            /// A cache key for storing and retrieving the bot's startup time.
            /// </summary>
            public const string StartupTime = nameof(StartupTime);

            /// <summary>
            /// A cache key for storing and retrieving the guilds the bot is a member of at startup.
            /// </summary>
            public const string StartupGuilds = nameof(StartupGuilds);
        }
    }
}
