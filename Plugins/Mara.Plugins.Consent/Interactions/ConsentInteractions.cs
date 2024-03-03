//
//  ConsentInteractions.cs
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
using JetBrains.Annotations;
using Mara.Plugins.Consent.Services;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Interactivity;
using Remora.Rest.Core;
using Remora.Results;

namespace Mara.Plugins.Consent.Interactions
{
    /// <summary>
    /// Provides handlers for buttons in an interaction prompt.
    /// </summary>
    [UsedImplicitly]
    public sealed class ConsentInteractions
    (
        IConsentService consentService,
        InteractionContext context
    )
        : InteractionGroup
    {
        /// <summary>
        /// Handles granting consent.
        /// </summary>
        /// <returns>A task containing the result of the <see cref="IConsentService.GrantConsent"/> operation.</returns>
        [Button(ConsentConstants.GrantConsentButtonId)]
        public async Task<Result> GrantConsentButtonPressedAsync()
        {
            if (context.TryGetUserID(out Snowflake userId))
            {
                return await consentService.GrantConsent(userId, CancellationToken);
            }

            return new NotFoundError("Could not retrieve user id from context.");
        }

        /// <summary>
        /// Handles denying consent.
        /// </summary>
        /// <returns>A task containing the result of the <see cref="IConsentService.DenyConsent"/> operation.</returns>
        [Button(ConsentConstants.DenyConsentButtonId)]
        public async Task<Result> DenyConsentButtonPressedAsync()
        {
            if (context.TryGetUserID(out Snowflake userId))
            {
                return await consentService.DenyConsent(userId, CancellationToken);
            }

            return new NotFoundError("Could not retrieve user id from context.");
        }
    }
}
