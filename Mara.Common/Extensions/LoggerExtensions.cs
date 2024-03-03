//
//  LoggerExtensions.cs
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

using System;
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
            => LogError(logger, error, null);

        /// <summary>
        /// Formats and writes an error log message.
        /// </summary>
        /// <typeparam name="T">The object this logger writes for.</typeparam>
        /// <param name="logger">The logger.</param>
        /// <param name="error">An <see cref="IResultError"/> representing the problem that occurred.</param>
        /// <param name="message">An optional message to log. Defaults to <paramref name="error.Message"/>.</param>
        public static void LogError<T>(this ILogger<T> logger, IResultError error, string? message)
        {
            Exception? exception = error is ExceptionError exceptionError
                ? exceptionError.Exception
                : null;

            logger.LogError(exception, "{Message}", message ?? error.Message);
        }
    }
}
