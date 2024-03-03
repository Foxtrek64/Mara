//
//  IAutomationAction.cs
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

using System.Threading.Tasks;
using Remora.Results;

#pragma warning disable SA1402 // File may only contain a single type

namespace Mara.Plugins.Automation
{
    /// <summary>
    /// A marker interface for automation actions.
    /// </summary>
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
    /// <remarks>
    /// An automation action is a task which is executed based on a trigger.
    /// Implementing classes should use the Action suffix.
    /// The built-in triggers take the form of Discord event handlers forwarded to MediatR.
    /// Plugins may register their own automation actions to provide automation functionality.
    /// For example, a moderation plugin may provide a DeleteMessageAction, which
    /// could be combined with a MessageCreated event could be used to make a word filter.
    /// </remarks>
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
