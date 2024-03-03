//
//  LoggingEventResponderBase.cs
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
using Humanizer;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace Mara.Common.Events
{
    /// <summary>
    /// An abstract <see cref="IResponder{TGatewayEvent}"/> which logs event lifetime to debug channels.
    /// </summary>
    /// <param name="logger">An <see cref="ILogger{TCategoryName}"/> for this entity.</param>
    /// <typeparam name="TGatewayEvent">An <see cref="IGatewayEvent"/> this responder should handle.</typeparam>
    public abstract class LoggingEventResponderBase<TGatewayEvent>(ILogger<LoggingEventResponderBase<TGatewayEvent>> logger)
        : IResponder<TGatewayEvent>
        where TGatewayEvent : IGatewayEvent
    {
        private static readonly string EventTypeName = typeof(TGatewayEvent).Name;

        /// <inheritdoc />
        public async Task<Result> RespondAsync(TGatewayEvent gatewayEvent, CancellationToken ct = default)
        {
            logger.LogDebug("Handling {Event}", EventTypeName);

            var sw = Stopwatch.StartNew();
            Result response = await HandleAsync(gatewayEvent, ct);
            sw.Stop();

            logger.LogDebug("Handled {Response} in {Elapsed}", EventTypeName, sw.Elapsed.Humanize(precision: 5));
            return response;
        }

        /// <summary>
        /// Override this method to provide logic for handling the specified <paramref name="gatewayEvent"/>.
        /// </summary>
        /// <param name="gatewayEvent">The gateway event to handle.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A <see cref="Result"/> indicating success for failure of the operation.</returns>
        protected abstract Task<Result> HandleAsync(TGatewayEvent gatewayEvent, CancellationToken cancellationToken = default);
    }
}
