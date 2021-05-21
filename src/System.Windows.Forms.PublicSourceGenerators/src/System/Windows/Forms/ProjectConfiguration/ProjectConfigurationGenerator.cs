// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace System.Windows.Forms
{
    [Generator]
    internal class ProjectConfigurationGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not ProjectConfigurationSyntaxReceiver syntaxReceiver)
            {
                throw new InvalidOperationException("We were given the wrong syntax receiver.");
            }

            if (!HasValidSyntaxNode(context, syntaxReceiver))
            {
                return;
            }

            if (!IsSupportedProjectType(context))
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_errorUnsupportedProjectType, Location.None));
                return;
            }

            new ExternalHighDpiSettingsVerifier().Verify(context);

            ProjectConfigurationInfo? projectConfig = ReadApplicationConfig(context);
            if (projectConfig is null)
            {
                return;
            }

            string? code = ProjectConfigurationInitializeBuilder.GenerateInitialize(projectNamespace: GetNamespace(syntaxReceiver.Nodes[0]), projectConfig);
            if (code is not null)
            {
                context.AddSource("ProjectConfiguration.g.cs", code);
            }
        }

        private string GetNamespace(SyntaxNode node)
        {
            string ns = "SourceGenerated";

            // TODO: what namespace do top-level programs have?
            if (node.Ancestors().FirstOrDefault(a => a is NamespaceDeclarationSyntax) is NamespaceDeclarationSyntax namespaceSyntax)
            {
                ns = namespaceSyntax.Name.ToString();
            }

            return ns;
        }

        private bool HasValidSyntaxNode(GeneratorExecutionContext context, ProjectConfigurationSyntaxReceiver syntaxReceiver)
        {
#if DEBUG
            if (syntaxReceiver.Nodes.Count == 0)
            {
                context.ReportDiagnostic(Diagnostic.Create("WFPC-DBG", nameof(ProjectConfigurationGenerator),
                    $"Opted out of ProjectConfiguration.Initialize experience", DiagnosticSeverity.Info, DiagnosticSeverity.Info, true, 4));
                return false;
            }
#endif
            if (syntaxReceiver.Nodes.Count > 1)
            {
                foreach (SyntaxNode node in syntaxReceiver.Nodes)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_duplicateProjectConfigurationInitialize, node.GetLocation()));
                }

                return false;
            }

            // We have exactly one node - all good
            return true;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ProjectConfigurationSyntaxReceiver());
        }

        private bool IsSupportedProjectType(GeneratorExecutionContext context)
            => context.Compilation.Options.OutputKind == OutputKind.WindowsApplication;

        private ProjectConfigurationInfo? ReadApplicationConfig(GeneratorExecutionContext context)
        {
            ProjectFileReader configurationReader = new();
            if (!configurationReader.TryReadEnableVisualStyles(context, out bool enableVisualStyles) ||
                !configurationReader.TryReadFontSize(context, out float? fontSize) ||
                !configurationReader.TryReadHighDpiMode(context, out HighDpiMode highDpiMode))
            {
                return null;
            }

            ProjectConfigurationInfo projectConfig = new()
            {
                EnableVisualStyles = enableVisualStyles,
                FontFamily = context.GetMSBuildProperty(ProjectConfigurationInfo.PropertyName.FontFamily, /* we want null */null!),
                FontSize = fontSize,
                HighDpiMode = highDpiMode,
            };

            return projectConfig;
        }
    }
}
