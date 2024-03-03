//
//  UpdateUserResponder.cs
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

using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mara.Common.Extensions;
using Mara.Plugins.Moderation.Models;
using Mara.Plugins.Moderation.Services;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Gateway.Responders;
using Remora.Rest.Core;
using Remora.Results;

namespace Mara.Plugins.Moderation.Responders
{
    /// <summary>
    /// Triggers an update to <see cref="UserInformation.LastSeen"/> for the user who triggers this action.
    /// </summary>
    [UsedImplicitly]
    public class UpdateUserResponder : IResponder<IGuildMemberAdd>,
                                       IResponder<IGuildMemberUpdate>,
                                       IResponder<IGuildMemberRemove>,
                                       IResponder<IMessageCreate>,
                                       IResponder<IMessageUpdate>,
                                       IResponder<IMessageDelete>,
                                       IResponder<IMessageReactionAdd>,
                                       IResponder<IMessageReactionRemove>
    {
        private readonly UserService _userService;
        private readonly IDiscordRestChannelAPI _channelApi;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUserResponder"/> class.
        /// </summary>
        /// <param name="userService">The user service.</param>
        /// <param name="channelApi">The channel api.</param>
        public UpdateUserResponder(UserService userService, IDiscordRestChannelAPI channelApi)
        {
            _userService = userService;
            _channelApi = channelApi;
        }

        /// <inheritdoc />
        public Task<Result> RespondAsync(IGuildMemberAdd gatewayEvent, CancellationToken ct = default)
            => gatewayEvent.User.HasValue
                ? UpdateLastSeen(gatewayEvent.User.Value.ID, ct)
                : Task.FromResult(Result.FromError(new InvalidOperationError("The user who joined was not provided in the gateway event.")));

        /// <inheritdoc />
        public Task<Result> RespondAsync(IGuildMemberUpdate gatewayEvent, CancellationToken ct = default)
            => UpdateLastSeen(gatewayEvent.User.ID, ct);

        /// <inheritdoc />
        public Task<Result> RespondAsync(IGuildMemberRemove gatewayEvent, CancellationToken ct = default)
            => UpdateLastSeen(gatewayEvent.User.ID, ct);

        /// <inheritdoc />
        public Task<Result> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
            => gatewayEvent.IsUserMessage()
                ? UpdateLastSeen(gatewayEvent.Author.ID, ct)
                : Task.FromResult(Result.FromError(new InvalidOperationError("Message was sent by a bot, webhook, or the system. Cannot update user.")));

        /// <inheritdoc />
        public async Task<Result> RespondAsync(IMessageUpdate gatewayEvent, CancellationToken ct = default)
        {
            // ID and ChannelID are always present despite being optional.
            var messageResult = await _channelApi.GetChannelMessageAsync(gatewayEvent.ChannelID.Value, gatewayEvent.ID.Value, ct);

            if (!messageResult.IsSuccess)
            {
                return Result.FromError(messageResult);
            }

            var message = messageResult.Entity;

            if (!message.IsUserMessage())
            {
                return new InvalidOperationError("Message was sent by a bot, webhook, or the system. Cannot update user.");
            }

            return await UpdateLastSeen(message.Author.ID, ct);
        }

        /// <inheritdoc />
        public Task<Result> RespondAsync(IMessageDelete gatewayEvent, CancellationToken ct = default)
        {
            // TODO: Pull from message cache.
            return Task.FromResult(Result.FromSuccess());
        }

        /// <inheritdoc />
        public Task<Result> RespondAsync(IMessageReactionAdd gatewayEvent, CancellationToken ct = default)
            => UpdateLastSeen(gatewayEvent.UserID, ct);

        /// <inheritdoc />
        public Task<Result> RespondAsync(IMessageReactionRemove gatewayEvent, CancellationToken ct = default)
            => UpdateLastSeen(gatewayEvent.UserID, ct);

        private Task<Result> UpdateLastSeen(Snowflake userId, CancellationToken cancellationToken = default)
            => _userService.UpdateLastSeen(userId, cancellationToken);
    }
}
