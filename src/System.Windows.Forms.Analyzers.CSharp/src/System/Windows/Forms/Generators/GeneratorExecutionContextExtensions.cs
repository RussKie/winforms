// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace System.Windows.Forms.Generators
{
    internal static class GeneratorExecutionContextExtensions
    {
        /// <summary>
        /// Attempts to read a value for the requested MSBuild property.
        /// </summary>
        /// <param name="context">The compilation.</param>
        /// <param name="name">The name of the property to read the value for.</param>
        /// <param name="defaultValue">The value to return, if property is absent.</param>
        /// <returns></returns>
        public static string GetMSBuildProperty(this GeneratorExecutionContext context, string name, string defaultValue = "")
        {
            context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{name}", out var value);
            return value ?? defaultValue;
        }
    }
}
