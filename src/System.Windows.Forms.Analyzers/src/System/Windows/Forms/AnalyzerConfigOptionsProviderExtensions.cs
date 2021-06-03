// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace System.Windows.Forms.Analyzers
{
    internal static class AnalyzerConfigOptionsProviderExtensions
    {
        /// <summary>
        /// Attempts to read a value for the requested MSBuild property.
        /// </summary>
        /// <param name="analyzerConfigOptions">The global optins.</param>
        /// <param name="name">The name of the property to read the value for.</param>
        /// <param name="defaultValue">The value to return, if property is absent.</param>
        /// <returns></returns>
        public static string GetMSBuildProperty(this AnalyzerConfigOptionsProvider analyzerConfigOptions, string name, string defaultValue = "")
        {
            analyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{name}", out var value);
            return value ?? defaultValue;
        }
    }
}
