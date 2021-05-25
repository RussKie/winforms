// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xml;
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
            SourceText? appManifestXml = appManifest.GetText(context.CancellationToken);
            if (appManifestXml is null)
            {
                return;
            }

            // If the manifest file is corrupt - let the build fail
            XmlDocument doc = new();
            doc.LoadXml(appManifestXml.ToString());

            XmlNamespaceManager nsmgr = new(doc.NameTable);
            nsmgr.AddNamespace("v1", "urn:schemas-microsoft-com:asm.v1");
            nsmgr.AddNamespace("v3", "urn:schemas-microsoft-com:asm.v3");
            nsmgr.AddNamespace("v3ws", "http://schemas.microsoft.com/SMI/2005/WindowsSettings");

            if (doc.DocumentElement.SelectSingleNode("//v3:application/v3:windowsSettings/v3ws:dpiAware", nsmgr) is not null)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_migrateHighDpiSettings,
                                                           Location.None,
                                                           appManifest.Path,
                                                           ProjectConfigurationInfo.PropertyName.HighDpiMode));
            }
        }
    }
}
