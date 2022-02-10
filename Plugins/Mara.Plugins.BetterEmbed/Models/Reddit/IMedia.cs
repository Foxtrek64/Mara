﻿//
//  IMedia.cs
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

using Remora.Rest.Core;

namespace Mara.Plugins.BetterEmbeds.Models.Reddit
{
    /// <summary>
    /// A media object which contains a video, either hosted locally or externally.
    /// </summary>
    public interface IMedia
    {
        /// <summary>
        /// Gets the <see cref="RedditVideo"/>.
        /// </summary>
        Optional<RedditVideo> RedditVideo { get; init; }

        /// <summary>
        /// Gets an <see cref="OEmbed.OEmbed"/> containing an external video.
        /// </summary>
        Optional<OEmbed.OEmbed> ExternalVideo { get; init; }
    }
}
