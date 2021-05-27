// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace System.Windows.Forms.Generators.ApplicationConfiguration
{
    internal static class ProjectFileReader
    {
        public static ApplicationConfig? ReadApplicationConfig(GeneratorExecutionContext context)
        {
            if (!TryReadBool(context, ApplicationConfig.PropertyNameCSharp.EnableVisualStyles, defaultValue: true, out bool enableVisualStyles) ||
                !TryReadBool(context, ApplicationConfig.PropertyNameCSharp.UseCompatibleTextRendering, defaultValue: false, out bool useCompatibleTextRendering) ||
                !TryReadFontSize(context, out float? fontSize) ||
                !TryReadHighDpiMode(context, out HighDpiMode highDpiMode))
            {
                return null;
            }

            ApplicationConfig projectConfig = new()
            {
                EnableVisualStyles = enableVisualStyles,
                FontFamily = context.GetMSBuildProperty(ApplicationConfig.PropertyNameCSharp.FontFamily, /* we want null */null!),
                FontSize = fontSize,
                HighDpiMode = highDpiMode,
                UseCompatibleTextRendering = useCompatibleTextRendering
            };

            return projectConfig;
        }

        private static bool TryReadBool(GeneratorExecutionContext context, string propertyName, bool defaultValue, out bool value)
        {
            value = defaultValue;
            string rawValue = context.GetMSBuildProperty(propertyName);
            if (!string.IsNullOrEmpty(rawValue))
            {
                if (string.IsNullOrWhiteSpace(rawValue) ||
                    !bool.TryParse(rawValue, out value))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_propertyCantBeSetToValue,
                                                               Location.None,
                                                               propertyName,
                                                               value));
                    value = defaultValue;
                    return false;
                }
            }

            return true;
        }

        private static bool TryReadFontSize(GeneratorExecutionContext context, out float? fontSize)
        {
            fontSize = null;
            string rawValue = context.GetMSBuildProperty(ApplicationConfig.PropertyNameCSharp.FontSize);
            if (!string.IsNullOrEmpty(rawValue))
            {
                if (string.IsNullOrWhiteSpace(rawValue) ||
                    !float.TryParse(rawValue, out float _fontSize) ||
                    _fontSize < 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_propertyCantBeSetToValue,
                                                               Location.None,
                                                               ApplicationConfig.PropertyNameCSharp.FontSize,
                                                               rawValue));
                    fontSize = null;
                    return false;
                }

                fontSize = _fontSize;
            }

            return true;
        }

        private static bool TryReadHighDpiMode(GeneratorExecutionContext context, out HighDpiMode highDpiMode)
        {
            highDpiMode = HighDpiMode.PerMonitorV2;
            string rawValue = context.GetMSBuildProperty(ApplicationConfig.PropertyNameCSharp.HighDpiMode);
            if (!string.IsNullOrEmpty(rawValue))
            {
                if (string.IsNullOrWhiteSpace(rawValue) ||
                    !Enum.TryParse(rawValue, true, out highDpiMode))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_propertyCantBeSetToValue,
                                                               Location.None,
                                                               ApplicationConfig.PropertyNameCSharp.HighDpiMode,
                                                               rawValue));
                    highDpiMode = 0;
                    return false;
                }
            }

            return true;
        }
    }
}
