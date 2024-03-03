//
//  InfoCommand.cs
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
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mara.Plugins.Core;
using Mara.Plugins.Moderation.Services;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Extensions.Embeds;
using Remora.Discord.Extensions.Formatting;
using Remora.Rest.Core;
using Remora.Results;

namespace Mara.Plugins.Moderation.Commands
{
    /// <summary>
    /// Commands which return information about a particular user.
    /// </summary>
    [UsedImplicitly]
    public sealed class InfoCommand : CommandGroup
    {
        private readonly FeedbackService _feedbackService;
        private readonly IDiscordRestUserAPI _userApi;
        private readonly IDiscordRestGuildAPI _guildApi;
        private readonly UserService _userService;
        private readonly ICommandContext _context;

        private readonly FeedbackMessageOptions _ephemeralMessage = new(MessageFlags: MessageFlags.Ephemeral);

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
        public async Task<Result> ShowUserInfoChatAsync([Autocomplete, DiscordTypeHint(TypeHint.User)] IUser user, Snowflake guildId = default)
        {
            var buildEmbedResult = await BuildUserInfoEmbed(user, guildId);

            if (!buildEmbedResult.IsSuccess)
            {
                if (_context.TryGetUserID(out Snowflake userId))
                {
                    _ = await _feedbackService.SendContextualErrorAsync
                        (buildEmbedResult.Error.Message, userId, _ephemeralMessage, CancellationToken);
                }

                return Result.FromError(buildEmbedResult);
            }

            var sendEmbedResult = await _feedbackService.SendContextualEmbedAsync
                (buildEmbedResult.Entity, _ephemeralMessage, CancellationToken);
            return sendEmbedResult.IsSuccess
                ? Result.FromSuccess()
                : Result.FromError(sendEmbedResult);
        }

        /// <summary>
        /// Returns information about a user. Executed from the user's context menu.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token for this operation.</param>
        /// <returns>A result indicating success or failure of sending a contextual embed.</returns>
        [Command("User Info")]
        [Description("Returns information about a user.")]
        [CommandType(ApplicationCommandType.User)]
        public async Task<Result> ShowUserInfoMenuAsync()
        {
            if (!_context.TryGetUserID(out Snowflake userId))
            {
                return new NotFoundError("Could not locate the user associated with this action.");
            }

            var userResult = await _userApi.GetUserAsync(userId, CancellationToken);

            if (!userResult.IsSuccess)
            {
                return Result.FromError(userResult);
            }

            IUser user = userResult.Entity;

            var buildEmbedResult = _context.TryGetGuildID(out Snowflake guildId)
                ? await BuildUserInfoEmbed(user, guildId)
                : await BuildUserInfoEmbed(user, default);

            if (!buildEmbedResult.IsSuccess)
            {
                return Result.FromError(buildEmbedResult);
            }

            var sendEmbedResult = await _feedbackService.SendContextualEmbedAsync
                (buildEmbedResult.Entity, ct: CancellationToken);

            return sendEmbedResult.IsSuccess
                ? Result.FromSuccess()
                : Result.FromError(sendEmbedResult);
        }

        private async Task<Result<Embed>> BuildUserInfoEmbed(IUser user, Snowflake guildId = default)
        {
            var userInfo = await _userService.GetUserInformation(user.ID);

            if (!userInfo.IsSuccess)
            {
                return Result<Embed>.FromError(userInfo);
            }

            var embedBuilder = new EmbedBuilder()
                .WithAuthor(user);

            embedBuilder.WithThumbnailUrl(embedBuilder.Author?.IconUrl ?? string.Empty);

            var userInformation = new StringBuilder()
                .AppendLine($"- ID: {user.ID.Value}")
                .AppendLine($"- Profile: {Mention.User(user)}")
                .AppendLine($"- First Seen: {Markdown.Timestamp(userInfo.Entity.FirstSeen, TimestampStyle.LongDateTime)}")
                .AppendLine($"- Last Seen: {Markdown.Timestamp(userInfo.Entity.LastSeen, TimestampStyle.RelativeTime)}");
            embedBuilder.AddField("❯ User Information", userInformation.ToString());

            var memberInfo = new StringBuilder()
                .AppendLine($"Created: {Markdown.Timestamp(user.ID.Timestamp, TimestampStyle.LongDateTime)}");

            Result<DateTimeOffset> joinDateResult = default;

            if (guildId == default(Snowflake))
            {
                _ = _context.TryGetGuildID(out guildId);
            }

            if (guildId != default(Snowflake))
            {
                joinDateResult = await GetJoinDate(user, guildId);
            }

            if (joinDateResult.IsSuccess)
            {
                memberInfo.AppendLine($"Joined: {Markdown.Timestamp(joinDateResult.Entity, TimestampStyle.RelativeTime)}");
            }

            embedBuilder.WithCurrentTimestamp();

            return embedBuilder.Build();
        }

        private async Task<Result<DateTimeOffset>> GetJoinDate(IUser user, Snowflake guildId)
        {
            // TODO: Use Cache
            var guildMember = await _guildApi.GetGuildMemberAsync(guildId, user.ID, CancellationToken);

            return guildMember.IsSuccess
                ? guildMember.Entity.JoinedAt
                : Result<DateTimeOffset>.FromError(guildMember);
        }
    }
}
