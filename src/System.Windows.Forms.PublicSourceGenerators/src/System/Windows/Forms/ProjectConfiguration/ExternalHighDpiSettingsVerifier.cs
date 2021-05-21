// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace System.Windows.Forms
{
    internal sealed class ExternalHighDpiSettingsVerifier
    {
        public void Verify(GeneratorExecutionContext context)
        {
            foreach (AdditionalText additionalFile in context.AdditionalFiles)
            {
                if (additionalFile.Path.EndsWith(".manifest", StringComparison.OrdinalIgnoreCase))
                {
                    VerifyAppManifest(context, additionalFile);
                    break;
                }
            }
        }

        private void VerifyAppManifest(GeneratorExecutionContext context, AdditionalText appManifest)
        {
            SourceText? appManifestText = appManifest.GetText();
            //if (appManifestText is null)
            //{
            //    return;
            //}

            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_migrateHighDpiSettings,
                                                       Location.None,
                                                       appManifest.Path,
                                                       ProjectConfigurationInfo.PropertyName.HighDpiMode));
        }
    }
}
