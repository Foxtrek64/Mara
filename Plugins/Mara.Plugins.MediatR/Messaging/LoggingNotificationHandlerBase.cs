//
//  LoggingNotificationHandlerBase.cs
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

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Mara.Plugins.Mediator.Messaging
{
    /// <summary>
    /// A base type for a notification handler which automatically logs the the starting and stopping of handling.
    /// </summary>
    /// <typeparam name="TNotification">The type of notification to handle.</typeparam>
    public abstract class LoggingNotificationHandlerBase<TNotification> : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        private readonly ILogger<LoggingNotificationHandlerBase<TNotification>> _logger;
        private static readonly string NotificationTypeName = typeof(TNotification).Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingNotificationHandlerBase{TNotification}"/> class.
        /// </summary>
        /// <param name="logger">A logger for this instance.</param>
        protected LoggingNotificationHandlerBase(ILogger<LoggingNotificationHandlerBase<TNotification>> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task Handle(TNotification notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {Request}", NotificationTypeName);

            var sw = Stopwatch.StartNew();
            await HandleAsync(notification, cancellationToken);
            sw.Stop();

            _logger.LogInformation("Handled {Response} in {Elapsed}", NotificationTypeName, sw.Elapsed.Humanize(precision: 5));
        }

        /// <summary>
        /// Handles a notification asynchronously.
        /// </summary>
        /// <param name="request">The notification.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A <see cref="ValueTask"/> representing the result of the operation.</returns>
        public abstract ValueTask HandleAsync(TNotification request, CancellationToken cancellationToken);
    }
}
