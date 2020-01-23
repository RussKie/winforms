﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///   Specifies how <see cref="TaskDialogCustomButton"/> instances are to be
    ///   displayed in a task dialog.
    /// </summary>
    public enum TaskDialogCustomButtonStyle
    {
        /// <summary>
        ///   Custom buttons should be displayed as normal buttons.
        /// </summary>
        Default = 0,

        /// <summary>
        ///   Custom buttons should be displayed as command links.
        /// </summary>
        CommandLinks = 1,

        /// <summary>
        ///   Custom buttons should be displayed as command links, but without an icon.
        /// </summary>
        CommandLinksNoIcon = 2
    }
}
