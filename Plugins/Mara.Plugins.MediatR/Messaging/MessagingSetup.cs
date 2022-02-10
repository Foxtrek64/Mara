//
//  MessagingSetup.cs
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

using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Mara.Plugins.Mediator.Messaging
{
    /// <summary>
    /// Extension methods for <see cref="IServiceCollection"/> to add scoped messaging.
    /// </summary>
    public static class MessagingSetup
    {
        /// <summary>
        /// Adds a scoped messaging service to the current <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TEntryPoint">The entry point, typically Program.cs.</typeparam>
        /// <typeparam name="TMediator">A scoped mediator instance.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> register with.</param>
        /// <returns>The current <see cref="IServiceCollection"/>, for chaining.</returns>
        public static IServiceCollection AddMessagingScoped<TEntryPoint, TMediator>(this IServiceCollection services)
            where TEntryPoint : class
            where TMediator : IMediator
            => services.AddMediatR(cfg => cfg.Using<TMediator>().AsScoped(), typeof(TEntryPoint));
    }
}
