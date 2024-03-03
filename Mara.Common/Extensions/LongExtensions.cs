//
//  LongExtensions.cs
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
        [Obsolete("Use ByteSize.ToString()", error: true)]
        public static string ToFileSize(this long value)
        {
            StringBuilder sb = new(11);
            StrFormatByteSize(value, sb, sb.Capacity);

            return sb.ToString();
        }
    }
}
