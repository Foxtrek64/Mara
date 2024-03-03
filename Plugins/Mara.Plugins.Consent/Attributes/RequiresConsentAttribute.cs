//
//  RequiresConsentAttribute.cs
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
using JetBrains.Annotations;
using Remora.Commands.Conditions;

namespace Mara.Plugins.Consent.Attributes
{
    /// <summary>
    /// Identifies a command that collects, stores, or modifies personal information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    [PublicAPI]
    public sealed class RequiresConsentAttribute : ConditionAttribute
    {
        /// <summary>
        /// Gets a value indicating whether the specified command or group requires consent to collect data.
        /// </summary>
        public bool RequiresConsent { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresConsentAttribute"/> class.
        /// </summary>
        /// <param name="requiresConsent">A value indicating whether the specified command or group requires consent to
        /// collect data.</param>
        public RequiresConsentAttribute(bool requiresConsent = true)
        {
            RequiresConsent = requiresConsent;
        }
    }
}
