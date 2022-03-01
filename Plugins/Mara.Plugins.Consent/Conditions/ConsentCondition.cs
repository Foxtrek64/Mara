//
//  ConsentCondition.cs
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

using System;
using System.Threading;
using System.Threading.Tasks;
using Mara.Plugins.Consent.Attributes;
using Mara.Plugins.Consent.Errors;
using Mara.Plugins.Consent.Services;
using Remora.Commands.Conditions;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Results;

namespace Mara.Plugins.Consent.Conditions
{
    /// <summary>
    /// Represents a condition which ensures a user has granted consent for the collection, storage, and use of their personal information.
    /// </summary>
    public sealed class ConsentCondition : ICondition<RequiresConsentAttribute, IUser>
    {
        private readonly IConsentService _consentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsentCondition"/> class.
        /// </summary>
        /// <param name="consentService">An instance of <see cref="IConsentService"/>.</param>
        public ConsentCondition(IConsentService consentService)
        {
            _consentService = consentService;
        }

        /// <inheritdoc/>
        public async ValueTask<Result> CheckAsync(RequiresConsentAttribute attribute, IUser data, CancellationToken ct = default)
        {
            // If the command does not require consent, return success.
            if (attribute.RequiresConsent == false)
            {
                return Result.FromSuccess();
            }

            var grantedConsent = await _consentService.GrantedConsent(data.ID);

            return grantedConsent
                ? Result.FromSuccess()
                : Result.FromError(new LackOfConsentError(data));
        }
    }
}
