//
//  CorePlugin.cs
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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Mara.Plugins.Core.Commands;
using Mara.Plugins.Core.Responders;
using Microsoft.Extensions.DependencyInjection;
using Remora.Commands.Extensions;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Gateway.Extensions;
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
        public override string Name => "Core";

        /// <inheritdoc />
        public override Version Version => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);

        /// <inheritdoc />
        public override string Description => "Provides core functionality for the bot.";

        /// <inheritdoc />
        public override Result ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddResponder<ReadyResponder>();
            serviceCollection.AddResponder<SlashCommandRegistrationResponder>();
            serviceCollection.AddResponder<UnknownEventResponder>();
            serviceCollection.AddResponder<DeleteRequestResponder>();
            serviceCollection.AddPostExecutionEvent<PostExectionResponder>();

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
