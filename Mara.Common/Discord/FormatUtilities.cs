//
//  FormatUtilities.cs
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

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1615 // Element return value should be documented
#pragma warning disable SA1611 // Element parameters should be documented

namespace Mara.Common.Discord
{
    [Obsolete("Use Remora.Discord.Extensions.Formatting")]
    public static class FormatUtilities
    {
        // Characters which need escaping
        private static readonly string[] SensitiveCharacters = { "\\", "*", "_", "~", "`", "|", ">" };

        /// <summary>
        /// Sanitizes the string, escaping any markdown sequences.
        /// </summary>
        /// <param name="text">The text to sanitize.</param>
        /// <returns>Sanitized text.</returns>
        public static string Sanitize(string text)
            => SensitiveCharacters.Aggregate(text, (current, unsafeChar) => current.Replace(unsafeChar, $@"\{unsafeChar}"));

        public static string SanitizeAllMentions(string text)
        {
            var everyoneSanitized = SanitizeEveryone(text);
            var userSanitized = SanitizeUserMentions(everyoneSanitized);
            var roleSanitized = SanitizeRoleMentions(userSanitized);

            return roleSanitized;
        }

        public static string SanitizeEveryone(string text)
            => text.Replace("@everyone", "@\x200beveryone")
                   .Replace("@here", "@\x200bhere");

        public static string SanitizeUserMentions(string text)
            => UserMentionRegex.Replace(text, "<@\x200b${Id}>");

        public static string SanitizeRoleMentions(string text)
            => RoleMentionRegex.Replace(text, "<@&\x200b${Id}>");

        public static bool ContainsSpoiler(string text)
            => ContainsSpoilerRegex.IsMatch(text);

        private static readonly Regex UserMentionRegex = new("<@!?(?<Id>[0-9]+)>", RegexOptions.Compiled);

        private static readonly Regex RoleMentionRegex = new("<@&(?<Id>[0-9]+)>", RegexOptions.Compiled);

        private static readonly Regex ContainsSpoilerRegex = new(@"\|\|.+\|\|", RegexOptions.Compiled);

        public static string Mention(IUser user)
            => $"<@{user.ID.Value}>";

        public static string Mention(IRole role)
            => $"<@&{role.ID.Value}>";

        public static string Mention(IChannel channel)
            => $"<#{channel.ID.Value}>";

        /// <summary>Returns a markdown-formatted string with bold formatting.</summary>
        /// <param name="text">The text to bold.</param>
        /// <returns>The text, bolded.</returns>
        public static string Bold(string text) => $"**{text}**";

        /// <summary> Returns a markdown-formatted string with italics formatting.</summary>
        /// <param name="text">The text to italicize.</param>
        /// <returns>The text, italicized.</returns>
        public static string Italics(string text) => $"*{text}*";

        /// <summary> Returns a markdown-formatted string with underline formatting. </summary>
        public static string Underline(string text) => $"__{text}__";

        /// <summary> Returns a markdown-formatted string with strikethrough formatting. </summary>
        public static string Strikethrough(string text) => $"~~{text}~~";

        /// <summary> Returns a string with spoiler formatting. </summary>
        public static string Spoiler(string text) => $"||{text}||";

        /// <summary> Returns a markdown-formatted URL. Only works in <see cref="Embed"/> descriptions and fields. </summary>
        public static string Url(string url) => $"[{url}]({url})";

        /// <summary> Returns a markdown-formatted URL. Only works in <see cref="Embed"/> descriptions and fields. </summary>
        public static string Url(string text, string url) => $"[{text}]({url})";

        /// <summary> Escapes a URL so that a preview is not generated. </summary>
        public static string EscapeUrl(string url) => $"<{url}>";

        /// <summary>
        /// Returns a markdown-formatted string with codeblock formatting.
        /// </summary>
        /// <param name="text">The text to wrap in a code block.</param>
        /// <param name="language">The code language. Ignored for single-line code blocks.</param>
        public static string Code(string text, string? language = null)
            => language is not null || text.Contains('\n')
                ? $"```{language ?? string.Empty}\n{text}\n```"
                : $"`{text}`";

        /// <summary>
        /// Returns a markdown timestamp which renders the specified date and time in the user's timezone and locale.
        /// </summary>
        /// <param name="timeStamp">The date and time to display.</param>
        /// <param name="style">What format to display it in.</param>
        /// <seealso href="https://discord.com/developers/docs/reference#message-formatting-timestamp-styles">Discord Message Formatting Timestamp Styles</seealso>
        public static string DynamicTimeStamp(DateTime timeStamp, TimeStampStyle style)
            => DynamicTimeStamp(new DateTimeOffset(timeStamp), style);

        /// <inheritdoc cref="DynamicTimeStamp(DateTime,TimeStampStyle)"/>
        public static string DynamicTimeStamp(DateTimeOffset timeStamp, TimeStampStyle style = TimeStampStyle.ShortDateTime)
        {
            char format = style switch
            {
                TimeStampStyle.ShortTime => 't',
                TimeStampStyle.LongTime => 'T',
                TimeStampStyle.ShortDate => 'd',
                TimeStampStyle.LongDate => 'D',
                TimeStampStyle.ShortDateTime => 'f',
                TimeStampStyle.LongDateTime => 'F',
                TimeStampStyle.RelativeTime => 'R',
                _ => 'f'
            };

            return $"<t:{timeStamp.ToUnixTimeSeconds()}:{format}>";
        }

        /// <summary>
        /// Determines the output style for the Dynamic Time Stamp.
        /// </summary>
        public enum TimeStampStyle
        {
            /// <summary>
            /// 16:20.
            /// </summary>
            ShortTime,

            /// <summary>
            /// 16:20:30.
            /// </summary>
            LongTime,

            /// <summary>
            /// 20/04/2021.
            /// </summary>
            ShortDate,

            /// <summary>
            /// 20 April 2021.
            /// </summary>
            LongDate,

            /// <summary>
            /// 20 April 2021 16:20.
            /// </summary>
            ShortDateTime,

            /// <summary>
            /// Tuesday, 20 April 2021 16:20.
            /// </summary>
            LongDateTime,

            /// <summary>
            /// 2 months ago.
            /// </summary>
            RelativeTime
        }
    }
}
