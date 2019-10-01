﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        [DllImport(Libraries.Comctl32, ExactSpelling = true)]
        public static extern int TaskDialogIndirect(
            IntPtr pTaskConfig,
            [Out] out int pnButton,
            [Out] out int pnRadioButton,
            [MarshalAs(UnmanagedType.Bool), Out] out bool pfVerificationFlagChecked);
    }
}
