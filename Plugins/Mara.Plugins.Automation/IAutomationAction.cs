//
//  IAutomationAction.cs
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

using Remora.Results;

#pragma warning disable SA1402 // File may only contain a single type

namespace Mara.Plugins.Automation
{
    /// <summary>
    /// Represents a base type which may be used to create automation actions.
    /// </summary>
    /// <remarks>
    /// An automation action is a task which is executed based on a trigger.
    /// Implementing classes should use the Action suffix.
    /// The built-in triggers take the form of Discord event handlers forwarded to MediatR.
    /// Plugins may register their own automation actions to provide automation functionality.
    /// For example, a moderation plugin may provide a DeleteMessageAction, which
    /// could be combined with a MessageCreated event could be used to make a word filter.
    /// </remarks>
    public interface IAutomationAction
    {
        /// <summary>
        /// Executes the automation action.
        /// </summary>
        /// <returns>An automation result which may or may not have succeeded.</returns>
        ValueTask<IResult> Execute();
    }

    /// <summary>
    /// Represents a base type which may be used to create automation actions.
    /// </summary>
    /// <typeparam name="TResult">The return type for the execute operation.</typeparam>
    public interface IAutomationAction<TResult> : IAutomationAction
    {
        /// <summary>
        /// Executes the automation action.
        /// </summary>
        /// <returns>An automation result which may or may not have succeeded.</returns>
        new ValueTask<Result<TResult>> Execute();
    }
}
