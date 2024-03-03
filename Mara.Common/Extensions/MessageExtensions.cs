//
//  MessageExtensions.cs
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

using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Extensions.Formatting;
using Remora.Rest.Core;
using Remora.Results;

namespace Mara.Common.Extensions
{
    /// <summary>
    /// A set of message extensions.
    /// </summary>
    [PublicAPI]
    public static class MessageExtensions
    {
        /// <summary>
        /// Returns a boolean indicating whether a message was sent by a user.
        /// </summary>
        /// <param name="message">The message to test.</param>
        /// <returns><c>True</c> if the message was sent by a user; otherwise, <c>False</c>.</returns>
        public static bool IsUserMessage(this IMessage message)
            => !message.IsWebhookMessage() && !message.IsBotMessage() && !message.IsSystemMessage();

        /// <summary>
        /// Indicates whether this message came from a webhook.
        /// </summary>
        /// <param name="message">The message to test.</param>
        /// <returns><c>True</c> if the message was sent by a webhook; otherwise, <c>False</c>.</returns>
        public static bool IsWebhookMessage(this IMessage message)
            => message.WebhookID.HasValue;

        /// <summary>
        /// Indicates whether this message came from a bot.
        /// </summary>
        /// <param name="message">The message to test.</param>
        /// <returns><c>True</c> if the message was sent by a bot; otherwise, <c>False</c>.</returns>
        public static bool IsBotMessage(this IMessage message)
            => message.Author.IsBot is { HasValue: true, Value: true };

        /// <summary>
        /// Indicates whether this message came from Discord.
        /// </summary>
        /// <param name="message">The message to test.</param>
        /// <returns><c>True</c> if the message was sent by Discord; otherwise, <c>False</c>.</returns>
        public static bool IsSystemMessage(this IMessage message)
            => message.Author.IsSystem is { HasValue: true, Value: true };

        private const string JumpUrl = "https://discord.com/channels/{0}/{1}/{2}";

        /// <summary>
        /// Gets a jump url for a specific message.
        /// </summary>
        /// <param name="message">The message to get a jump url for.</param>
        /// <param name="channelApi">The channel api to perform lookups with.</param>
        /// <returns>A string representing the path to a message.</returns>
        public static async Task<Result<string>> GetJumpUrlAsync
        (
            this IMessage message,
            IDiscordRestChannelAPI channelApi
        )
        {
            Result<IChannel> channelResult = await channelApi.GetChannelAsync(message.ChannelID);

            if (!channelResult.IsSuccess)
            {
                return Result<string>.FromError(channelResult);
            }

            Optional<Snowflake> guildId = channelResult.Entity.GuildID;
            return guildId.HasValue
                ? string.Format(JumpUrl, guildId.Value.Value, message.ChannelID.Value, message.ID.Value)
                : string.Format(JumpUrl, "@me", message.ChannelID.Value, message.ID.Value);
        }
    }
}
