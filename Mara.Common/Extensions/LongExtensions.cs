//
//  LongExtensions.cs
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

using System.Runtime.InteropServices;
using System.Text;

namespace Mara.Common.Extensions
{
    /// <summary>
    /// Provides extensions to <see cref="long"/>.
    /// </summary>
    public static class LongExtensions
    {
        [DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern long StrFormatByteSize(long fileSize, StringBuilder buffer, int bufferSize);

        /// <summary>
        /// Converts a long value into a formatted string representing file size.
        /// </summary>
        /// <param name="value">The long to convert.</param>
        /// <returns>A formatted file size string.</returns>
        public static string ToFileSize(this long value)
        {
            StringBuilder sb = new(11);
            StrFormatByteSize(value, sb, sb.Capacity);

            return sb.ToString();
        }
    }
}
