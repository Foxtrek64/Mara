//
//  IConsentService.cs
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

using System.Threading.Tasks;
using Remora.Rest.Core;

namespace Mara.Plugins.Consent.Services
{
    /// <summary>
    /// Provides access to utilities for determining, granting, and revoking consent.
    /// </summary>
    public interface IConsentService
    {
        /// <summary>
        /// Indicates whether the specified user has granted consent.
        /// </summary>
        /// <param name="userId">The unique id of the user.</param>
        /// <returns>True if the user has granted consent; otherwise, false if consent has not been granted or has been revoked.</returns>
        public Task<bool> GrantedConsent(Snowflake userId);
    }
}
