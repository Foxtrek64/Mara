//
//  CorePlugin.cs
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
using Mara.Common.Extensions;
using Mara.Plugins.Core.Commands;
using Mara.Plugins.Core.Models;
using Mara.Plugins.Core.Responders;
using Microsoft.Extensions.DependencyInjection;
using Remora.Commands.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Interactivity.Extensions;
using Remora.Plugins.Abstractions;
using Remora.Results;

namespace Mara.Plugins.Core
{
    /// <summary>
    /// Represents core functionality.
    /// </summary>
    public class CorePlugin : PluginDescriptor
    {
        /// <inheritdoc />
        public override string Name => "Mara.Plugins.Core";

        /// <inheritdoc />
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);

        /// <inheritdoc />
        public override string Description => "Provides core functionality for the bot.";

        /// <inheritdoc />
        public override Result ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddConfigurationModule<CoreConfig>();

            serviceCollection.AddResponder<ReadyResponder>();
            serviceCollection.AddResponder<SlashCommandRegistrationResponder>();
            serviceCollection.AddResponder<UnknownEventResponder>();
            serviceCollection.AddResponder<DeleteRequestResponder>();
            serviceCollection.AddPostExecutionEvent<PostExecutionResponder>();

            serviceCollection.AddInteractivity();

            serviceCollection.AddCommandTree(Name)
                .WithCommandGroup<AboutCommand>()
                .Finish();

            return Result.FromSuccess();
        }

        /// <inheritdoc />
        public override ValueTask<Result> InitializeAsync(IServiceProvider serviceProvider, CancellationToken ct = default)
        {
            return base.InitializeAsync(serviceProvider, ct);
        }
    }
}
