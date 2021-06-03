// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace System.Windows.Forms.Analyzers
{
    internal static class DiagnosticDescriptors
    {
        private const string Category = "ApplicationConfiguration";

        public static readonly DiagnosticDescriptor s_errorUnsupportedProjectType
            = new(id: "WFAC001",
                  title: "Unsupported project type",
                  messageFormat: $"Only projects with 'OutputType={nameof(OutputKind.WindowsApplication)}' supported",
                  category: Category,
                  defaultSeverity: DiagnosticSeverity.Error,
                  isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor s_propertyCantBeSetToValue
            = new(id: "WFAC002",
                  title: "Unsupported property value",
                  messageFormat: "ArgumentException: Project property '{0}' cannot be set to '{1}'",
                  category: Category,
                  defaultSeverity: DiagnosticSeverity.Error,
                  isEnabledByDefault: true);

        internal static readonly DiagnosticDescriptor s_migrateHighDpiSettings_CSharp
           = new(id: "WFAC010",
                 title: "Unsupported high DPI configuration",
                 messageFormat: "Remove high DPI settings from {0} and configure via '{1}' project property",
                 category: Category,
                 defaultSeverity: DiagnosticSeverity.Warning,
                 isEnabledByDefault: true);
        internal static readonly DiagnosticDescriptor s_migrateHighDpiSettings_VB
           = new(id: "WFAC011",
                 title: "Unsupported high DPI configuration",
                 messageFormat: "Remove high DPI settings from {0} and configure via '{1}' property in Application Framework",
                 category: Category,
                 defaultSeverity: DiagnosticSeverity.Warning,
                 isEnabledByDefault: true);
    }
}
