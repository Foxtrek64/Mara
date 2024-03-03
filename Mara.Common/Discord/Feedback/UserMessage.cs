//
//  UserMessage.cs
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

using System.Drawing;
using Remora.Discord.Extensions.Embeds;

#pragma warning disable SA1402

namespace Mara.Common.Discord.Feedback
{
    /// <summary>
    /// Acts as a base class for user-facing messages.
    /// </summary>
    public abstract record UserMessage(string Message, Color Color);

    /// <summary>
    /// Represents a confirmation message.
    /// </summary>
    public record ConfirmationMessage(string Message) : UserMessage(Message, Color.Cyan);

    /// <summary>
    /// Represents an error message.
    /// </summary>
    public record ErrorMessage(string Message) : UserMessage(Message, Color.Red);

    /// <summary>
    /// Represents a warning message.
    /// </summary>
    public record WarningMessage(string Message) : UserMessage(Message, Color.OrangeRed);

    /// <summary>
    /// Represents an informational message.
    /// </summary>
    public record InfoMessage(string Message) : UserMessage(Message, EmbedConstants.DefaultColour);

    /// <summary>
    /// Represents a question or prompt.
    /// </summary>
    public record PromptMessage(string Message) : UserMessage(Message, Color.MediumPurple);
}
