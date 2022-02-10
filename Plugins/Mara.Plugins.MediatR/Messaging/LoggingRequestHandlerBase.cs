﻿//
//  LoggingRequestHandlerBase.cs
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
using MediatR;
using Microsoft.Extensions.Logging;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace Mara.Plugins.Mediator.Messaging
{
    /// <summary>
    /// A base type for a request handler which automatically logs the the starting and stopping of handling.
    /// </summary>
    /// <typeparam name="TRequest">The type of request to handle.</typeparam>
    /// <typeparam name="TResponse">The type of response to return.</typeparam>
    public abstract class LoggingRequestHandlerBase<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingRequestHandlerBase<TRequest, TResponse>> _logger;
        private static readonly string RequestTypeName = typeof(TRequest).Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingRequestHandlerBase{TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="logger">A logger for this instance.</param>
        protected LoggingRequestHandlerBase(ILogger<LoggingRequestHandlerBase<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling {Request}", RequestTypeName);

            var sw = Stopwatch.StartNew();
            var response = await HandleAsync(request, cancellationToken);
            sw.Stop();

            _logger.LogInformation("Handled {Response} in {Elapsed}", RequestTypeName, sw.Elapsed.Humanize(precision: 5));
            return response;
        }

        /// <summary>
        /// Handles a request asynchronously.
        /// </summary>
        /// <param name="request">The notification.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A <see cref="ValueTask"/> representing the result of the operation.</returns>
        public abstract ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}
