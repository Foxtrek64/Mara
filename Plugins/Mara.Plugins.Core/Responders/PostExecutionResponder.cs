//
//  PostExecutionResponder.cs
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
using JetBrains.Annotations;
using Mara.Common.Discord.Feedback.Errors;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Discord.Commands.Services;
using Remora.Results;

namespace Mara.Plugins.Core.Responders
{
    /// <summary>
    /// A post-execution event responder which handles alerting the user to errors.
    /// </summary>
    /// <param name="feedbackService">A feedback service.</param>
    [UsedImplicitly]
    public sealed class PostExecutionResponder(FeedbackService feedbackService) : IPostExecutionEvent
    {
        /// <inheritdoc/>
        public async Task<Result> AfterExecutionAsync(ICommandContext context, IResult commandResult, CancellationToken ct = default)
        {
            if (commandResult.IsSuccess)
            {
                return Result.FromSuccess();
            }

            if (commandResult.Error is not UserError userError)
            {
                return Result.FromSuccess();
            }

            // TODO: Expand upon this responder!
            if (context.TryGetUserID(out var userId))
            {
                if (userError.IsError)
                {
                    await feedbackService.SendContextualErrorAsync(userError.Message, userId, ct: ct);
                }
                else
                {
                    await feedbackService.SendContextualWarningAsync(userError.Message, userId, ct: ct);
                }

                return Result.FromSuccess();
            }

            return Result.FromSuccess();
        }
    }
}
