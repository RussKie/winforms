// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace System.Windows.Forms
{
    internal static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor s_errorUnsupportedProjectType
            = new(id: "WFPC001",
                  title: "Unsupported project type",
                  messageFormat: $"Only projects with 'OutputType={nameof(OutputKind.WindowsApplication)}' supported",
                  category: nameof(ProjectConfigurationGenerator),
                  defaultSeverity: DiagnosticSeverity.Error,
                  isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor s_propertyCantBeSetToValue
            = new(id: "WFPC002",
                  title: "Unsupported property value",
                  messageFormat: "ArgumentException: Project property '{0}' cannot be set to '{1}'",
                  category: nameof(ProjectConfigurationGenerator),
                  defaultSeverity: DiagnosticSeverity.Error,
                  isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor s_migrateHighDpiSettings
            = new(id: "WFPC003",
                  title: "Unsupported high DPI configuration",
                  messageFormat: "Remove high DPI settings from {0} and configure via '{1}' project property",
                  category: nameof(ProjectConfigurationGenerator),
                  defaultSeverity: DiagnosticSeverity.Error,
                  isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor s_duplicateProjectConfigurationInitialize
            = new(id: "WFPC010",
                  title: "Invalid code",
                  messageFormat: "ProjectConfiguration.Initialize can only be used once per project",
                  category: nameof(ProjectConfigurationGenerator),
                  defaultSeverity: DiagnosticSeverity.Error,
                  isEnabledByDefault: true);
    }
}
