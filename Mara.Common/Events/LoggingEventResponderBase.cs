//
//  LoggingEventResponderBase.cs
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
    /// <typeparam name="TGatewayEvent">An <see cref="IGatewayEvent"/> this responder should handle.</typeparam>
    public abstract class LoggingEventResponderBase<TGatewayEvent> : IResponder<TGatewayEvent>
        where TGatewayEvent : IGatewayEvent
    {
        private readonly ILogger<LoggingEventResponderBase<TGatewayEvent>> _logger;
        private static readonly string EventTypeName = typeof(TGatewayEvent).Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingEventResponderBase{TEvent}"/> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger{TCategoryName}"/> for this entity.</param>
        protected LoggingEventResponderBase(ILogger<LoggingEventResponderBase<TGatewayEvent>> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<Result> RespondAsync(TGatewayEvent gatewayEvent, CancellationToken ct = default)
        {
            _logger.LogDebug("Handling {Event}", EventTypeName);

            var sw = Stopwatch.StartNew();
            var response = await Handle(gatewayEvent, ct);
            sw.Stop();

            _logger.LogDebug("Handled {Response} in {Elapsed}", EventTypeName, sw.Elapsed.Humanize(precision: 5));
            return response;
        }

        /// <summary>
        /// Override this method to provide logic for handling the specified <paramref name="gatewayEvent"/>.
        /// </summary>
        /// <param name="gatewayEvent">The gateway event to handle.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A <see cref="Result"/> indicating success for failure of the operation.</returns>
        protected abstract Task<Result> Handle(TGatewayEvent gatewayEvent, CancellationToken cancellationToken = default);
    }
}
