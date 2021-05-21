// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    internal class ProjectConfigurationInfo
    {
        public static class PropertyName
        {
            public const string EnableVisualStyles = "ApplicationVisualStyles";
            public const string FontFamily = "ApplicationFontName";
            public const string FontSize = "ApplicationFontSize";
            public const string HighDpiMode = "ApplicationHighDpiMode";
        }

        public bool EnableVisualStyles { get; set; }
        public string? FontFamily { get; set; }
        public float? FontSize { get; set; }
        public HighDpiMode HighDpiMode { get; set; }
    }
}
