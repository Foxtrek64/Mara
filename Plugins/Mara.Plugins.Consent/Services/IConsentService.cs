//
//  IConsentService.cs
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
using Mara.Plugins.Consent.Models;
using Remora.Rest.Core;
using Remora.Results;

namespace Mara.Plugins.Consent.Services
{
    /// <summary>
    /// Provides access to utilities for determining, granting, and revoking consent.
    /// </summary>
    [PublicAPI]
    public interface IConsentService
    {
        /// <summary>
        /// Indicates whether the specified user has granted consent.
        /// </summary>
        /// <param name="userId">The unique id of the user.</param>
        /// <param name="cancellationToken">A cancellation token for this operation.</param>
        /// <returns>True if the user has granted consent; otherwise, false if consent has not been granted or has been revoked.</returns>
        public Task<bool> HasConsented(Snowflake userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the <see cref="ConsentStatus"/> for the specified user.
        /// </summary>
        /// <param name="userId">The unique id of the user.</param>
        /// <param name="cancellationToken">A cancellation token for this operation.</param>
        /// <returns>The <see cref="ConsentStatus"/> for the specified user.</returns>
        public Task<Result<ConsentStatus>> GetConsentStatus(Snowflake userId, CancellationToken cancellationToken);

        /// <summary>
        /// Records that the user has granted consent.
        /// </summary>
        /// <param name="userId">The ID of the user who is granting consent.</param>
        /// <param name="cancellationToken">A cancellation token for this operation.</param>
        /// <returns>A result indicating the success or failure of the operation.</returns>
        public Task<Result> GrantConsent(Snowflake userId, CancellationToken cancellationToken);

        /// <summary>
        /// Records that the user has revoked consent.
        /// </summary>
        /// <param name="userId">The ID of the user who is revoking consent.</param>
        /// <param name="cancellationToken">A cancellation token for this operation.</param>
        /// <returns>A result indicating the success or failure of the operation.</returns>
        public Task<Result> RevokeConsent(Snowflake userId, CancellationToken cancellationToken);

        /// <summary>
        /// Records that the user has denied consent.
        /// </summary>
        /// <param name="userId">The ID of the user who is denying consent.</param>
        /// <param name="cancellationToken">A cancellation token for this operation.</param>
        /// <returns>A result indicating success or failure of the operation.</returns>
        public Task<Result> DenyConsent(Snowflake userId, CancellationToken cancellationToken);
    }
}
