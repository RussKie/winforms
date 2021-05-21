// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace System.Windows.Forms
{
    internal class ProjectConfigurationSyntaxReceiver : ISyntaxReceiver
    {
        public List<SyntaxNode> Nodes { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
#pragma warning disable SA1513 // Closing brace should be followed by blank line
            if (syntaxNode is InvocationExpressionSyntax
                {
                    ArgumentList:
                    {
                        Arguments: { Count: < 1 } arguments
                    },
                    Expression: MemberAccessExpressionSyntax
                    {
                        Name:
                        {
                            Identifier:
                            {
                                ValueText: "Initialize"
                            }
                        },
                        Expression: MemberAccessExpressionSyntax  // For: SourceGenerated.ProjectConfiguration.Initialize()
                        {
                            Name:
                            {
                                Identifier:
                                {
                                    ValueText: "ProjectConfiguration"
                                }
                            }
                        } or IdentifierNameSyntax                 // For: ProjectConfiguration.Initialize() with a using statement
                        {
                            Identifier:
                            {
                                ValueText: "ProjectConfiguration"
                            }
                        }
                    }
                })
            {
                Nodes.Add(syntaxNode);
            }
#pragma warning restore SA1513 // Closing brace should be followed by blank line
        }
    }
}
