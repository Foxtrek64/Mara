//
//  UnknownEventResponder.cs
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
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Mara.Plugins.Core.Responders
{
    /// <summary>
    /// An event responder which logs unknown events.
    /// </summary>
    /// <param name="logger">A logger for this event responder.</param>
    [UsedImplicitly]
    public sealed class UnknownEventResponder
    (
        ILogger<UnknownEventResponder> logger
    )
        : IResponder<UnknownEvent>
    {
        /// <inheritdoc/>
        public Task<Result> RespondAsync(UnknownEvent gatewayEvent, CancellationToken ct = default)
        {
            logger.LogInformation("Captured unknown event: {Event}", gatewayEvent.Data);
            return Task.FromResult(Result.FromSuccess());
        }
    }
}
