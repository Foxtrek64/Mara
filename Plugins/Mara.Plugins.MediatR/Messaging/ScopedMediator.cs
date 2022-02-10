//
//  ScopedMediator.cs
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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Mara.Plugins.Mediator.Messaging
{
    /// <summary>
    /// An <see cref="IMediator"/> which creates a new scope for each request.
    /// </summary>
    public sealed class ScopedMediator : IScopedMediator
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedMediator"/> class.
        /// </summary>
        /// <param name="serviceScopeFactory">An <see cref="IServiceScopeFactory"/> from which to create scopes.</param>
        public ScopedMediator(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <inheritdoc/>
        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return Send(scope, request, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<TResponse> Send<TResponse>(IServiceScope scope, IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var service = scope.ServiceProvider.GetRequiredService<IMediator>();
            return await service.Send(request, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return Send(scope, request, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<object?> Send(IServiceScope scope, object request, CancellationToken cancellationToken)
        {
            var service = scope.ServiceProvider.GetRequiredService<IMediator>();
            return await service.Send(request, cancellationToken);
        }

        /// <inheritdoc/>
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return Publish(scope, notification, cancellationToken);
        }

        /// <inheritdoc />
        public async Task Publish<TNotification>(IServiceScope scope, TNotification notification, CancellationToken cancellationToken) where TNotification : INotification
        {
            var service = scope.ServiceProvider.GetRequiredService<IMediator>();
            await service.Publish(notification, cancellationToken);
        }

        /// <inheritdoc/>
        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return Publish(scope, notification, cancellationToken);
        }

        /// <inheritdoc />
        public async Task Publish(IServiceScope scope, object notification, CancellationToken cancellationToken)
        {
            var service = scope.ServiceProvider.GetRequiredService<IMediator>();
            await service.Publish(notification, cancellationToken);
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return CreateStream(scope, request, cancellationToken);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<TResponse> CreateStream<TResponse>(IServiceScope scope, IStreamRequest<TResponse> request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var service = scope.ServiceProvider.GetRequiredService<IMediator>();
            await foreach (var response in service.CreateStream(request, cancellationToken))
            {
                yield return response;
            }
        }

        /// <inheritdoc/>
        public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return CreateStream(scope, request, cancellationToken);
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<object?> CreateStream(IServiceScope scope, object request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var service = scope.ServiceProvider.GetRequiredService<IMediator>();
            await foreach (var response in service.CreateStream(request, cancellationToken))
            {
                yield return response;
            }
        }
    }
}
