//
//  ModerationPlugin.cs
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
using Mara.Plugins.Moderation;
using Microsoft.Extensions.DependencyInjection;
using Remora.Plugins.Abstractions;
using Remora.Plugins.Abstractions.Attributes;
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
            return base.ConfigureServices(serviceCollection);
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
