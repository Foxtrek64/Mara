//
//  ConsentCondition.cs
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
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mara.Plugins.Consent.Attributes;
using Mara.Plugins.Consent.Errors;
using Mara.Plugins.Consent.Services;
using Remora.Commands.Conditions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Results;

namespace Mara.Plugins.Consent.Conditions
{
    /// <summary>
    /// Represents a condition which ensures a user has granted consent for the collection, storage, and use of their
    /// personal information.
    /// </summary>
    /// <param name="consentService">An instance of <see cref="IConsentService"/>.</param>
    [UsedImplicitly]
    public sealed class ConsentCondition(IConsentService consentService) : ICondition<RequiresConsentAttribute, IUser>
    {
        /// <inheritdoc/>
        public async ValueTask<Result> CheckAsync(RequiresConsentAttribute attribute, IUser user, CancellationToken ct = default)
        {
            // If the command does not require consent, return success.
            if (attribute.RequiresConsent == false)
            {
                return Result.FromSuccess();
            }

            var grantedConsent = await consentService.HasConsented(user.ID, ct);

            return grantedConsent
                ? Result.FromSuccess()
                : Result.FromError(new LackOfConsentError(user));
        }
    }
}
