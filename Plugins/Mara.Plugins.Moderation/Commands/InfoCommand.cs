//
//  InfoCommand.cs
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
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Mara.Common.Discord;
using Mara.Plugins.Core;
using Mara.Plugins.Moderation.Services;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Extensions.Embeds;
using Remora.Rest.Core;
using Remora.Results;

namespace Mara.Plugins.Moderation.Commands
{
    /// <summary>
    /// Commands which return information about a particular user.
    /// </summary>
    public sealed class InfoCommand : CommandGroup
    {
        private readonly FeedbackService _feedbackService;
        private readonly IDiscordRestUserAPI _userApi;
        private readonly IDiscordRestGuildAPI _guildApi;
        private readonly UserService _userService;
        private readonly ICommandContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoCommand"/> class.
        /// </summary>
        /// <param name="feedbackService">The user feedback service.</param>
        /// <param name="userApi">The user api.</param>
        /// <param name="guildApi">The guild api.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="context">The command context.</param>
        public InfoCommand
        (
            FeedbackService feedbackService,
            IDiscordRestUserAPI userApi,
            IDiscordRestGuildAPI guildApi,
            UserService userService,
            ICommandContext context
        )
        {
            _feedbackService = feedbackService;
            _userApi = userApi;
            _guildApi = guildApi;
            _userService = userService;
            _context = context;
        }

        /// <summary>
        /// Returns information about a specified user.
        /// </summary>
        /// <param name="user">The user to retrieve information about.</param>
        /// <param name="guildId">If available, the guild the user is in.</param>
        /// <returns>A result indicating success or failure of sending a contextual embed.</returns>
        [Command("info")]
        [Description("Returns information about a user.")]
        [CommandType(ApplicationCommandType.ChatInput)]
        public async Task<IResult> ShowUserInfoChatAsync(IUser user, Snowflake? guildId = null)
        {
            var buildEmbedResult = await BuildUserInfoEmbed(user, guildId);

            if (!buildEmbedResult.IsSuccess)
            {
                return buildEmbedResult;
            }

            return await _feedbackService.SendContextualEmbedAsync(buildEmbedResult.Entity, ct: CancellationToken);
        }

        /// <summary>
        /// Returns information about a user. Executed from the user's context menu.
        /// </summary>
        /// <returns>A result indicating success or failure of sending a contextual embed.</returns>
        [Command("User Info")]
        [Description("Returns information about a user.")]
        [CommandType(ApplicationCommandType.User)]
        public async Task<IResult> ShowUserInfoMenuAsync()
        {
            var user = _context.User;

            var buildEmbedResult = _context.GuildID.HasValue
                ? await BuildUserInfoEmbed(user, _context.GuildID.Value)
                : await BuildUserInfoEmbed(user, null);

            if (!buildEmbedResult.IsSuccess)
            {
                return buildEmbedResult;
            }

            return await _feedbackService.SendContextualEmbedAsync(buildEmbedResult.Entity, ct: CancellationToken);
        }

        private async Task<Result<Embed>> BuildUserInfoEmbed(IUser user, Snowflake? guildId)
        {
            var userInfo = await _userService.GetUserInformation(user);

            if (!userInfo.IsSuccess)
            {
                return Result<Embed>.FromError(userInfo);
            }

            var embedBuilder = new EmbedBuilder()
                .WithAuthor(user);

            embedBuilder.WithThumbnailUrl(embedBuilder.Author?.IconUrl ?? string.Empty);

            var userInformation = new StringBuilder()
                .AppendLine($"ID: {user.ID.Value}")
                .AppendLine($"Profile: {FormatUtilities.Mention(user)}")
                .AppendLine($"First Seen: {FormatUtilities.DynamicTimeStamp(userInfo.Entity.FirstSeen, FormatUtilities.TimeStampStyle.LongDateTime)}")
                .AppendLine($"Last Seen: {FormatUtilities.DynamicTimeStamp(userInfo.Entity.LastSeen, FormatUtilities.TimeStampStyle.RelativeTime)}");
            embedBuilder.AddField("❯ User Information", userInformation.ToString());

            var memberInfo = new StringBuilder()
                .AppendLine($"Created: {user.ID.Timestamp}");

            Result<DateTimeOffset> joinDate = default;

            if (guildId is null && _context.GuildID.HasValue)
            {
                guildId = _context.GuildID.Value;
            }

            if (guildId is not null)
            {
                joinDate = await GetJoinDate(user, guildId.Value);
            }

            if (joinDate.IsSuccess)
            {
                memberInfo.AppendLine($"Joined: {FormatUtilities.DynamicTimeStamp(joinDate.Entity, FormatUtilities.TimeStampStyle.RelativeTime)}");
            }

            embedBuilder.WithFooter(CoreConstants.DismissableEmbedFooter);
            embedBuilder.WithCurrentTimestamp();

            return embedBuilder.Build();
        }

        private async Task<Result<DateTimeOffset>> GetJoinDate(IUser user, Snowflake guildId)
        {
            var guildMember = await _guildApi.GetGuildMemberAsync(guildId, user.ID, CancellationToken);

            return guildMember.IsSuccess
                ? guildMember.Entity.JoinedAt
                : Result<DateTimeOffset>.FromError(guildMember);
        }
    }
}
