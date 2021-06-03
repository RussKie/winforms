// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace System.Windows.Forms.Analyzers.Generators
{
    [Generator]
    internal class ApplicationConfigurationGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not ApplicationConfigurationSyntaxReceiver syntaxReceiver)
            {
                throw new InvalidOperationException("We were given the wrong syntax receiver.");
            }

            if (syntaxReceiver.Nodes.Count == 0)
            {
                return;
            }

            if (context.Compilation.Options.OutputKind != OutputKind.WindowsApplication)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.s_errorUnsupportedProjectType, Location.None));
                return;
            }

            ApplicationConfig? projectConfig = ProjectFileReader.ReadApplicationConfig(context);
            if (projectConfig is null)
            {
                return;
            }

            string? code = ApplicationConfigurationInitializeBuilder.GenerateInitialize(projectNamespace: GetUserProjectNamespace(syntaxReceiver.Nodes[0]), projectConfig);
            if (code is not null)
            {
                context.AddSource("ApplicationConfiguration.g.cs", code);
            }
        }

        private string GetUserProjectNamespace(SyntaxNode node)
        {
            string ns = "SourceGenerated";

            // TODO: what namespace do top-level programs have?
            if (node.Ancestors().FirstOrDefault(a => a is NamespaceDeclarationSyntax) is NamespaceDeclarationSyntax namespaceSyntax)
            {
                ns = namespaceSyntax.Name.ToString();
            }

            return ns;
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ApplicationConfigurationSyntaxReceiver());
        }
    }
}
