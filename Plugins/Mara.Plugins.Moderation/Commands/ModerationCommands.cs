//
//  ModerationCommands.cs
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
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mara.Common.Errors;
using Mara.Common.Extensions;
using Mara.Plugins.Moderation.Models;
using Mara.Plugins.Moderation.Services;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Conditions;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Rest.Core;
using Remora.Results;

namespace Mara.Plugins.Moderation.Commands
{
    /// <summary>
    /// A set of commands relating to user moderation.
    /// </summary>
    [PublicAPI]
    public class ModerationCommands : CommandGroup
    {
        private readonly GuildService _guildService;
        private readonly UserService _userService;
        private readonly IDiscordRestGuildAPI _guildApi;
        private readonly ICommandContext _context;
        private readonly IFeedbackService _feedbackService;

        private readonly FeedbackMessageOptions _ephemeralMessage = new(MessageFlags: MessageFlags.Ephemeral);

        /// <summary>
        /// Initializes a new instance of the <see cref="ModerationCommands"/> class.
        /// </summary>
        /// <param name="guildService">The guild service.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="guildApi">The guild api.</param>
        /// <param name="context">The command context.</param>
        /// <param name="feedbackService">The feedback service.</param>
        public ModerationCommands
        (
            GuildService guildService,
            UserService userService,
            IDiscordRestGuildAPI guildApi,
            ICommandContext context,
            IFeedbackService feedbackService
            )
        {
            _guildService = guildService;
            _userService = userService;
            _guildApi = guildApi;
            _context = context;
            _feedbackService = feedbackService;
        }

        /// <summary>
        /// Mutes a user.
        /// </summary>
        /// <param name="target">The user to mute.</param>
        /// <param name="reason">The reason for the mute.</param>
        /// <param name="duration">How long the mute should last. If not specified, the mute must be revoked manually.</param>
        /// <returns>A result indicating success or failure.</returns>
        [Command("mute")]
        [Description("Mutes a user.")]
        [RequireContext(ChannelContext.Guild)]
        [RequireDiscordPermission(DiscordPermission.ModerateMembers)]
        [CommandType(ApplicationCommandType.ChatInput)]
        public async Task<Result> MuteCommand(IUser target, string? reason, TimeSpan? duration = null)
        {
            if (!_context.TryGetGuildID(out var guildId))
            {
                return new GuildRequiredError("This command must be executed in a guild context.");
            }
            var muteRoleResult = await _guildService.GetMuteRoleIdAsync(guildId);
            if (!muteRoleResult.IsDefined(out var roleId))
            {
                return new NotFoundError("Could not find a mute role for this guild. Has one been configured?");
            }

            // Mute the user
            var muteUserResult = await _guildApi.AddGuildMemberRoleAsync(guildId, target.ID, roleId, reason.AsOptional());
            if (!muteUserResult.IsSuccess)
            {
                return muteUserResult;
            }

            // Record the infraction
            var infractionResult = await _userService.CreateInfractionRecord
            (
                userId: target.ID,
                infractionKind: InfractionKind.Mute,
                guildId: guildId,
                moderator: _context.GetUserId(),
                reason: reason,
                expiration: duration is not null ? DateTimeOffset.UtcNow + duration : null
            );

            if (infractionResult.IsSuccess)
            {
                _ = await _feedbackService.SendContextualSuccessAsync
                (
                    $"User {target.Username} was muted.",
                    _context.GetUserId(),
                    _ephemeralMessage,
                    CancellationToken
                );

                return Result.FromSuccess();
            }

            _ = await _feedbackService.SendContextualErrorAsync
            (
                $"Unable to mute {target.Username}: {infractionResult.Error.Message}",
                _context.GetUserId(),
                _ephemeralMessage,
                CancellationToken
            );
            return Result.FromError(infractionResult);
        }
    }
}
