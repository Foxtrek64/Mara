//
//  IScopedMediator.cs
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

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Mara.Plugins.Mediator.Messaging
{
    /// <summary>
    /// An <see cref="IMediator"/> designed for sending scoped requests and notifications.
    /// </summary>
    public interface IScopedMediator : IMediator
    {
        /// <summary>
        /// Asynchronously send a request to a single handler.
        /// </summary>
        /// <typeparam name="TResponse">The type of response to expect.</typeparam>
        /// <param name="scope">A service scope to reuse for the request.</param>
        /// <param name="request">The request itself.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A task that represents the send operation. The task result contains the handler response.</returns>
        public Task<TResponse> Send<TResponse>(IServiceScope scope, IRequest<TResponse> request, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously send a request to a single handler.
        /// </summary>
        /// <param name="scope">A service scope to reuse for the request.</param>
        /// <param name="request">The request itself.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A task that represents the send operation. The task result contains the handler response.</returns>
        public Task<object?> Send(IServiceScope scope, object request, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously sends a request to multiple handlers.
        /// </summary>
        /// <typeparam name="TNotification">The type of notification to send.</typeparam>
        /// <param name="scope">A service scope to reuse for the request.</param>
        /// <param name="notification">The notification itself.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A task that represents the notification operation. The task result contains the handler response.</returns>
        public Task Publish<TNotification>(IServiceScope scope, TNotification notification, CancellationToken cancellationToken) where TNotification : INotification;

        /// <summary>
        /// Asynchronously sends a request to multiple handlers.
        /// </summary>
        /// <param name="scope">A service scope to reuse for the request.</param>
        /// <param name="notification">The notification itself.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A task that represents the notification operation. The task result contains the handler response.</returns>
        public Task Publish(IServiceScope scope, object notification, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a stream via a single stream handler.
        /// </summary>
        /// <typeparam name="TResponse">The type of response to expect.</typeparam>
        /// <param name="scope">A service scope to reuse for the request.</param>
        /// <param name="request">The request itself.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A task that represents the send operation. The task result contains the handler response.</returns>
        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IServiceScope scope, IStreamRequest<TResponse> request, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a stream via an object request to a stream handler.
        /// </summary>
        /// <param name="scope">A service scope to reuse for the request.</param>
        /// <param name="request">The request itself.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A task that represents the send operation. The task result contains the handler response.</returns>
        public IAsyncEnumerable<object?> CreateStream(IServiceScope scope, object request, CancellationToken cancellationToken);
    }
}
