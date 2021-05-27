// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    internal class ApplicationConfig
    {
        public static class PropertyNameCSharp
        {
            public const string EnableVisualStyles = "ApplicationVisualStyles";
            public const string FontFamily = "ApplicationFontName";
            public const string FontSize = "ApplicationFontSize";
            public const string HighDpiMode = "ApplicationHighDpiMode";
            public const string UseCompatibleTextRendering = "ApplicationUseCompatibleTextRendering";
        }

        public static class PropertyNameVisualBasic
        {
            public const string EnableVisualStyles = "EnableVisualStyles";
            public const string HighDpiMode = "HighDpiMode";
        }

        public bool EnableVisualStyles { get; set; }
        public string? FontFamily { get; set; }
        public float? FontSize { get; set; }
        public HighDpiMode HighDpiMode { get; set; }
        public bool UseCompatibleTextRendering { get; set; }
    }
}
