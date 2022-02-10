//
//  UnknownEventResponder.cs
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
using Remora.Discord.API.Gateway.Events;
using Remora.Discord.Gateway.Responders;
using Remora.Results;

namespace Mara.Plugins.Core.Responders
{
    /// <summary>
    /// An event responder which logs unknown events.
    /// </summary>
    public sealed class UnknownEventResponder : IResponder<UnknownEvent>
    {
        private readonly ILogger<UnknownEventResponder> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownEventResponder"/> class.
        /// </summary>
        /// <param name="logger">A logger for this event responder.</param>
        public UnknownEventResponder(ILogger<UnknownEventResponder> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc/>
        public Task<Result> RespondAsync(UnknownEvent gatewayEvent, CancellationToken ct = default)
        {
            _logger.LogInformation("Captured unknown event: {Event}", gatewayEvent.Data);
            return Task.FromResult(Result.FromSuccess());
        }
    }
}
