//
//  ReadyResponder.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mara.Common.Events;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Caching;
using Remora.Discord.Caching.Services;
using Remora.Discord.Rest.Extensions;
using Remora.Results;

// using CacheKeys = Mara.Plugins.Core.CoreConstants.CacheKeys;

namespace Mara.Plugins.Core.Responders
{
    /// <summary>
    /// Handles post-startup tasks, such as setting the bot presence and registering global slash commands.
    /// </summary>
    /// <param name="logger">A logger.</param>
    /// <param name="cacheService">The Discord memory cache..</param>
    [UsedImplicitly]
    public sealed class ReadyResponder
    (
        ILogger<ReadyResponder> logger,
        CacheService cacheService
    )
        : LoggingEventResponderBase<IReady>(logger)
    {
        /// <inheritdoc/>
        protected override async Task<Result> HandleAsync(IReady gatewayEvent, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Bot started!");

            // Add the ready event information to the memory cache.
            logger.LogDebug("Caching the IReady event entries");
            await cacheService.CacheAsync(new KeyHelpers.CurrentUserCacheKey(), gatewayEvent.User, cancellationToken);
            await cacheService.CacheAsync(new KeyHelpers.CurrentApplicationCacheKey(), gatewayEvent.Application, cancellationToken);
            await cacheService.CacheAsync(new CoreKeyHelpers.StartupTimeCacheKey(), DateTimeOffset.UtcNow.ToISO8601String(), cancellationToken);

            return Result.FromSuccess();
        }
    }
}
