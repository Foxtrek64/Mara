//
//  SlashCommandRegistrationResponder.cs
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

using System.Threading;
using System.Threading.Tasks;
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
    public sealed class SlashCommandRegistrationResponder : IResponder<IGuildCreate>
    {
        private readonly SlashService _slashService;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlashCommandRegistrationResponder"/> class.
        /// </summary>
        /// <param name="slashService">The slash service.</param>
        /// <param name="logger">A logger.</param>
        public SlashCommandRegistrationResponder(SlashService slashService, ILogger<SlashCommandRegistrationResponder> logger)
        {
            _slashService = slashService;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<Result> RespondAsync(IGuildCreate gatewayEvent, CancellationToken cancellationToken = default)
        {
            // For debug only
            var guildId = new Snowflake(861515006067998731, DiscordConstants.DiscordEpoch);

            if (gatewayEvent.ID != guildId)
            {
                return Result.FromSuccess();
            }

            // Load slash commands
            var checkSlashService = _slashService.SupportsSlashCommands();

            if (checkSlashService.IsSuccess)
            {
                var updateSlash = await _slashService.UpdateSlashCommandsAsync(guildId, ct: cancellationToken);
                if (!updateSlash.IsSuccess)
                {
                    _logger.LogWarning("Failed to update slash commands: {Reason}", updateSlash.Error.Message);

                    return updateSlash;
                }
            }
            else
            {
                _logger.LogWarning("The registered commands of the bot don't support slash commands: {Reason}", checkSlashService.Error.Message);
                return checkSlashService;
            }

            return Result.FromSuccess();
        }
    }
}
