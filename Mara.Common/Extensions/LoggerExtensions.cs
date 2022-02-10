//
//  LoggerExtensions.cs
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

using Microsoft.Extensions.Logging;
using Remora.Results;

namespace Mara.Common.Extensions
{
    /// <summary>
    /// Provides extensions for <see cref="Microsoft.Extensions.Logging"/> to allow directly logging an <see cref="IResultError"/>.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Formats and writes an error log message.
        /// </summary>
        /// <typeparam name="T">The object this logger writes for.</typeparam>
        /// <param name="logger">The logger.</param>
        /// <param name="error">An <see cref="IResultError"/> representing the problem that occurred.</param>
        public static void LogError<T>(this ILogger<T> logger, IResultError error)
        {
            var exception = error is ExceptionError exceptionError
                ? exceptionError.Exception
                : null;

            logger.LogError(exception, "An error occurred: {Message}", error.Message);
        }
    }
}
