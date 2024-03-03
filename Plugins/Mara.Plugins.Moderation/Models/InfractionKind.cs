//
//  InfractionKind.cs
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

namespace Mara.Plugins.Moderation.Models
{
    /// <summary>
    /// What type of punishment was awarded in the infraction.
    /// </summary>
    public enum InfractionKind
    {
        /// <summary>
        /// The user received a written warning.
        /// </summary>
        Warn = 1,

        /// <summary>
        /// The user was muted, either temporarily or permanently.
        /// </summary>
        Mute = 2,

        /// <summary>
        /// The user was kicked from the guild.
        /// </summary>
        Kick = 4,

        /// <summary>
        /// The user was soft banned, restricting them to an appeals channel.
        /// </summary>
        SoftBan = 8,

        /// <summary>
        /// The user was banned using Discord's banishment system.
        /// </summary>
        Ban = 16,

        /// <summary>
        /// The user received a ban from all communities managed by the bot.
        /// </summary>
        GlobalBan = 32,
    }
}
