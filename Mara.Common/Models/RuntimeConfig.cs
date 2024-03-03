//
//  RuntimeConfig.cs
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

namespace Mara.Common.Models
{
    /// <summary>
    /// Represents the primary configuration file for the bot.
    /// </summary>
    [PublicAPI]
    public sealed record RuntimeConfig : IConfigurationModule
    {
        /// <inheritdoc />
        public string SectionName => "Mara:Runtime";

        /// <summary>
        /// Gets the Discord Bot Token for authenticating with the Discord API.
        /// </summary>
        public required string BotToken { get; init; }

        /// <summary>
        /// Gets the OAuth Client Id.
        /// </summary>
        public required string ClientId { get; init; }

        /// <summary>
        /// Gets the OAuth Client Secret.
        /// </summary>
        public required string ClientSecret { get; init; }

        /// <summary>
        /// Gets the bot url.
        /// </summary>
        public required string BotWebsiteUrl { get; init; }
    }
}
