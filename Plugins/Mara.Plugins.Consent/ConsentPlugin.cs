//
//  ConsentPlugin.cs
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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Mara.Plugins.Consent.Conditions;
using Mara.Plugins.Consent.Interactions;
using Mara.Plugins.Consent.Responders;
using Mara.Plugins.Consent.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Remora.Commands.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Interactivity.Extensions;
using Remora.Plugins.Abstractions;
using Remora.Results;

namespace Mara.Plugins.Consent
{
    /// <summary>
    /// A plugin which keeps track of user consents on a global basis.
    /// </summary>
    [UsedImplicitly]
    public sealed class ConsentPlugin : PluginDescriptor, IMigratablePlugin
    {
        /// <inheritdoc/>
        public override string Name => "Consent";

        /// <inheritdoc />
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);

        /// <inheritdoc/>
        public override string Description => "Retrieves and stores user consent on a global basis.";

        /// <inheritdoc/>
        public override Result ConfigureServices(IServiceCollection serviceCollection)
        {
            try
            {
                serviceCollection.AddTransient<IConsentService, ConsentService>();
                serviceCollection.AddCondition<ConsentCondition>();
                serviceCollection.AddPostExecutionEvent<ConsentPostCommandResponder>();
                serviceCollection.AddInteractionGroup<ConsentInteractions>();

                return Result.FromSuccess();
            }
            catch (Exception e)
            {
                return e;
            }
        }

        /// <inheritdoc/>
        public async Task<Result> MigrateAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
        {
            ConsentContext context = serviceProvider.GetRequiredService<ConsentContext>();
            await using IDbContextTransaction transaction = await context.Database.BeginTransactionAsync(ct);

            try
            {
                await context.Database.MigrateAsync(ct);
                await transaction.CommitAsync(ct);

                return Result.FromSuccess();
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}
