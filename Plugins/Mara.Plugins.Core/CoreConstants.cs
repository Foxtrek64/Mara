//
//  CoreConstants.cs
//
//  Author:
//       LuzFaltex Contributors
//
//  ISC License
//
//  Copyright (c) 2021 LuzFaltex
//
//  Permission to use, copy, modify, and/or distribute this software for any
//  purpose with or without fee is hereby granted, provided that the above
//  copyright notice and this permission notice appear in all copies.
//
//  THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
//  WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
//  MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
//  ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
//  WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
//  ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
//  OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
//

using Remora.Discord.API.Objects;

namespace Mara.Plugins.Core
{
    /// <summary>
    /// A set of constants for the Core plugin.
    /// </summary>
    public static class CoreConstants
    {
        /// <summary>
        /// Gets a default footer that is used on dismissable embeds.
        /// </summary>
        public static readonly EmbedFooter DismissableEmbedFooter = new("React with ❌ to remove this embed");

        /// <summary>
        /// A collection of keys for IMemoryCache objects.
        /// </summary>
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
