// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace System.Windows.Forms
{
    [Generator]
    internal class ProjectConfigurationGenerator : ISourceGenerator
    {
        public void Execute1(GeneratorExecutionContext context)
        {
            Debugger.Launch();
            Debug.WriteLine(context);
        }

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

#if DEBUG
            context.ReportDiagnostic(Diagnostic.Create("DBG", nameof(ProjectConfigurationGenerator),
                "project.json found :)",
                severity: DiagnosticSeverity.Info,
                defaultSeverity: DiagnosticSeverity.Info,
                isEnabledByDefault: true,
                warningLevel: 4));
#endif

            ProjectConfigurationInfo? projectConfig = new(); 
            string? code = ProjectConfigurationInitializeGenerator.GenerateInitialize(projectNamespace: GetNamespace(syntaxReceiver.Nodes[0]), projectConfig);
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
            if (syntaxReceiver.Nodes.Count == 0)
            {
#if DEBUG
                context.ReportDiagnostic(Diagnostic.Create("DBG", nameof(ProjectConfigurationGenerator),
                    $"Nothing to do", DiagnosticSeverity.Warning, DiagnosticSeverity.Warning, true, 4));
#endif
                return false;
            }
            else if (syntaxReceiver.Nodes.Count != 1)
            {
                foreach (SyntaxNode node in syntaxReceiver.Nodes)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create("PC0", nameof(ProjectConfigurationGenerator),
                            $"ProjectConfiguration.Initialize can only be used once per project",
                            severity: DiagnosticSeverity.Error,
                            defaultSeverity: DiagnosticSeverity.Error,
                            isEnabledByDefault: true,
                            warningLevel: 0,
                            location: node.GetLocation()));
                }

                return false;
            }

            return true;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ProjectConfigurationSyntaxReceiver());
        }

        private bool IsSupportedProjectType(GeneratorExecutionContext context)
            => context.Compilation.Options.OutputKind == OutputKind.WindowsApplication;

        private static bool TryGetProjectRootPath(GeneratorExecutionContext context, out string? projectRootPath)
            => context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.projectdir", out projectRootPath) &&
               !string.IsNullOrEmpty(projectRootPath);
    }
}
