﻿//
//  MessageExtensions.cs
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

using System.Threading.Tasks;
using Mara.Common.Discord;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Results;

namespace Mara.Common.Extensions
{
    /// <summary>
    /// A set of message extensions.
    /// </summary>
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
            => message.Author.IsBot.HasValue && message.Author.IsBot.Value;

        /// <summary>
        /// Indicates whether this message came from Discord.
        /// </summary>
        /// <param name="message">The message to test.</param>
        /// <returns><c>True</c> if the message was sent by Discord; otherwise, <c>False</c>.</returns>
        public static bool IsSystemMessage(this IMessage message)
            => message.Author.IsSystem.HasValue && message.Author.IsSystem.Value;

        private const string JumpUrl = "https://discord.com/channels/{0}/{1}/{2}";

        /// <summary>
        /// Gets a jump url for a specific message.
        /// </summary>
        /// <param name="message">The message to get a jump url for.</param>
        /// <param name="channelApi">The channel api to perform lookups with.</param>
        /// <returns>A string representing the path to a message.</returns>
        public static async Task<Result<string>> GetJumpUrlAsync(this IMessage message, IDiscordRestChannelAPI channelApi)
        {
            var channelResult = await channelApi.GetChannelAsync(message.ChannelID);

            if (!channelResult.IsSuccess || channelResult.Entity is null)
            {
                return Result<string>.FromError(channelResult);
            }

            string jumpUrl = string.Format(JumpUrl, message.GuildID.Value, message.ChannelID.Value, message.ID.Value);

            return Result<string>.FromSuccess(FormatUtilities.Url($"#{(channelResult.Entity.Name.HasValue ? channelResult.Entity.Name.Value : message.ChannelID.ToString())} (click here)", jumpUrl));
        }
    }
}
