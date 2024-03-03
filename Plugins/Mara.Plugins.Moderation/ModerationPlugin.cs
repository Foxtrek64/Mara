//
//  ModerationPlugin.cs
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
using Mara.Plugins.Moderation.Services;
using Microsoft.Extensions.DependencyInjection;
using Remora.Plugins.Abstractions;
using Remora.Results;

namespace Mara.Plugins.Moderation
{
    /// <summary>
    /// A plugin which provides for manual automation through commands.
    /// </summary>
    public sealed class ModerationPlugin : PluginDescriptor, IMigratablePlugin
    {
        /// <inheritdoc/>
        public override string Name => "Moderation";

        /// <inheritdoc/>
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);

        /// <inheritdoc/>
        public override string Description => "Provides extended moderation features for staff members.";

        /// <inheritdoc/>
        public override Result ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<GuildService>();
            serviceCollection.AddScoped<UserService>();

            return Result.FromSuccess();
        }

        /// <inheritdoc/>
        public override ValueTask<Result> InitializeAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
        {
            return base.InitializeAsync(serviceProvider, ct);
        }

        /// <inheritdoc />
        public Task<Result> MigrateAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
        {
            return Task.FromResult(Result.FromSuccess());
        }
    }
}
