//
//  SlashCommandRegistrationResponder.cs
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
using Mara.Common.Events;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway.Responders;
using Remora.Rest.Core;
using Remora.Results;

using DiscordConstants = Remora.Discord.API.Constants;

namespace Mara.Plugins.Core.Responders
{
    /// <summary>
    /// Registers server-level slash commands.
    /// </summary>
    /// <param name="slashService">The slash service.</param>
    /// <param name="logger">A logger for this instance.</param>
    [UsedImplicitly]
    public sealed class SlashCommandRegistrationResponder
    (
        SlashService slashService,
        ILogger<SlashCommandRegistrationResponder> logger
    )
        : LoggingEventResponderBase<IGuildCreate>(logger)
    {
        /// <inheritdoc />
       protected override async Task<Result> HandleAsync(IGuildCreate gatewayEvent, CancellationToken cancellationToken = default)
        {
            // Load slash commands
            Result updateSlashCommandsResult = gatewayEvent.Guild.IsT0
                ? await slashService.UpdateSlashCommandsAsync(gatewayEvent.Guild.AsT0.ID, ct: cancellationToken)
                : Result.FromSuccess();

            if (updateSlashCommandsResult.IsSuccess)
            {
                return Result.FromSuccess();
            }

            logger.LogWarning("Failed to update slash commands: {Reason}", updateSlashCommandsResult.Error.Message);
            return updateSlashCommandsResult;
        }
    }
}
