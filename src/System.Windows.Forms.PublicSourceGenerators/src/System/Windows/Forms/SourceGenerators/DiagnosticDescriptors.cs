// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace System.Windows.Forms
{
    internal static class DiagnosticDescriptors
    {
        public static readonly DiagnosticDescriptor s_errorUnsupportedProjectType
            = new(id: "WF0101",
                  title: "Unsupported project type",
                  messageFormat: $"Only {nameof(OutputKind.WindowsApplication)} supported",
                  category: nameof(ProjectConfigurationGenerator),
                  defaultSeverity: DiagnosticSeverity.Error,
                  isEnabledByDefault: true);
    }
}
