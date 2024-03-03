//
//  ConsentService.cs
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
using Mara.Plugins.Consent.Models;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Caching;
using Remora.Discord.Caching.Services;
using Remora.Rest.Core;
using Remora.Results;

namespace Mara.Plugins.Consent.Services
{
    /// <summary>
    /// Provides a service for reviewing and updating consent records.
    /// </summary>
    /// <param name="consentContext">A database context for this instance.</param>
    /// <param name="logger">A logger for this instance.</param>
    public sealed class ConsentService
    (
        ConsentContext consentContext,
        ILogger<ConsentContext> logger,
        CacheService cacheService
    )
        : IConsentService
    {
        /// <inheritdoc />
        public async Task<bool> HasConsented(Snowflake userId, CancellationToken cancellationToken)
        {
            var consentRecordResult = await GetConsentStatus(userId, cancellationToken);

            if (!consentRecordResult.IsDefined(out ConsentStatus consentStatus))
            {
                return false;
            }

            return consentStatus == ConsentStatus.Granted;
        }

        /// <inheritdoc />
        public async Task<Result<ConsentStatus>> GetConsentStatus(Snowflake userId, CancellationToken cancellationToken)
        {
            var consentRecordResult = await cacheService.TryGetValueAsync<Models.Consent>
                (new ConsentCacheKey(userId), cancellationToken);

            if (!consentRecordResult.IsDefined(out Models.Consent? consentRecord))
            {
                consentRecord = await consentContext.ConsentClaims.FindAsync([userId], cancellationToken);
            }

            return consentRecord is null
                ? new NotFoundError("Could not locate a consent record for this user.")
                : consentRecord.GivesConsent;
        }

        /// <inheritdoc />
        public Task<Result> GrantConsent(Snowflake userId, CancellationToken cancellationToken)
            => SetConsent(userId, ConsentStatus.Granted, cancellationToken);

        /// <inheritdoc />
        public Task<Result> RevokeConsent(Snowflake userId, CancellationToken cancellationToken)
            => SetConsent(userId, ConsentStatus.Revoked, cancellationToken);

        /// <inheritdoc />
        public Task<Result> DenyConsent(Snowflake userId, CancellationToken cancellationToken)
            => SetConsent(userId, ConsentStatus.Denied, cancellationToken);

        private async Task<Result> SetConsent(Snowflake userId, ConsentStatus consentStatus, CancellationToken cancellationToken)
        {
            var userResult = await cacheService.TryGetValueAsync<IUser>(new KeyHelpers.UserCacheKey(userId), cancellationToken);

            string consentType = consentStatus switch
            {
                ConsentStatus.Granted => "GDPR User Consent",
                ConsentStatus.Denied => "GDPR User Consent Denial",
                ConsentStatus.Revoked => "GDPR User Consent Revocation",
                _ => "Pending GDPR User Consent"
            };

            if (userResult.IsDefined(out IUser? user))
            {
                    logger.LogInformation
                    (
                        "Recording {ConsentType} for {UserUsername} ({UserId}): User clicked Grant Consent button",
                        consentType,
                        user.Username,
                        userId
                    );
            }
            else
            {
                logger.LogInformation
                (
                    "Recording {ConsentType} for {UserId}: User clicked Grant Consent button",
                    consentType,
                    userId
                );
            }

            Models.Consent? consentRecord = await consentContext.ConsentClaims.FindAsync([userId], cancellationToken);

            if (consentRecord is null)
            {
                consentRecord = new Models.Consent()
                {
                    Id = userId,
                    GivesConsent = consentStatus
                };

                await consentContext.ConsentClaims.AddAsync(consentRecord, cancellationToken);
            }
            else
            {
                consentRecord.GivesConsent = consentStatus;
            }

            await consentContext.SaveChangesAsync(cancellationToken);

            await cacheService.CacheAsync(new ConsentCacheKey(userId), consentRecord, cancellationToken);

            return Result.FromSuccess();
        }
    }
}
