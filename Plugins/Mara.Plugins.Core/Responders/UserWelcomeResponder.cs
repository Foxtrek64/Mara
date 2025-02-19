﻿//
//  UserWelcomeResponder.cs
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
using Mara.Common.Events;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Gateway.Events;
using Remora.Results;

namespace Mara.Plugins.Core.Responders
{
    /// <summary>
    /// Responds to <see cref="GuildMemberAdd"/> to welcome members to the server.
    /// </summary>
    /// <param name="logger">A logger for this instance.</param>
    [UsedImplicitly]
    public class UserWelcomeResponder
    (
        ILogger<UserWelcomeResponder> logger
    )
        : LoggingEventResponderBase<GuildMemberAdd>(logger)
    {
        /// <inheritdoc />
        protected override Task<Result> HandleAsync(GuildMemberAdd gatewayEvent, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
