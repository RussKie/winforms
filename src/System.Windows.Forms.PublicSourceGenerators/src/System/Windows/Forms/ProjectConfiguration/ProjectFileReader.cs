// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace System.Windows.Forms
{
    internal sealed class ProjectFileReader
    {
        private const string AbsentValue = "!@#$%";

        public bool TryReadEnableVisualStyles(GeneratorExecutionContext context, out bool enableVisualStyles)
        {
            enableVisualStyles = true;
            string value = context.GetMSBuildProperty(ProjectConfigurationInfo.PropertyName.EnableVisualStyles, AbsentValue);
            if (value != AbsentValue)
            {
                if (string.IsNullOrWhiteSpace(value) ||
                    !bool.TryParse(value, out enableVisualStyles))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_propertyCantBeSetToValue,
                                                               Location.None,
                                                               ProjectConfigurationInfo.PropertyName.EnableVisualStyles,
                                                               value));
                    enableVisualStyles = false;
                    return false;
                }
            }

            return true;
        }

        public bool TryReadFontSize(GeneratorExecutionContext context, out float? fontSize)
        {
            fontSize = null;
            string value = context.GetMSBuildProperty(ProjectConfigurationInfo.PropertyName.FontSize, AbsentValue);
            if (value != AbsentValue)
            {
                if (string.IsNullOrWhiteSpace(value) ||
                    !float.TryParse(value, out float _fontSize) ||
                    _fontSize < 1)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_propertyCantBeSetToValue,
                                                               Location.None,
                                                               ProjectConfigurationInfo.PropertyName.FontSize,
                                                               value));
                    fontSize = null;
                    return false;
                }

                fontSize = _fontSize;
            }

            return true;
        }

        public bool TryReadHighDpiMode(GeneratorExecutionContext context, out HighDpiMode highDpiMode)
        {
            highDpiMode = HighDpiMode.PerMonitorV2;
            string value = context.GetMSBuildProperty(ProjectConfigurationInfo.PropertyName.HighDpiMode, AbsentValue);
            if (value != AbsentValue)
            {
                if (string.IsNullOrWhiteSpace(value) ||
                    !Enum.TryParse(value, true, out highDpiMode))
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_propertyCantBeSetToValue,
                                                               Location.None,
                                                               ProjectConfigurationInfo.PropertyName.HighDpiMode,
                                                               value));
                    highDpiMode = 0;
                    return false;
                }
            }

            return true;
        }
    }
}
