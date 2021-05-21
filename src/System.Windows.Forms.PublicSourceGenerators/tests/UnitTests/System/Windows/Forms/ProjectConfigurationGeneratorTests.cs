// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ApprovalTests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace System.Windows.Forms.SourceGenerators.Tests
{
    public class ProjectConfigurationGeneratorTests
    {
        [Conditional("DEBUG")]
        [Fact]
        public void ProjectConfigurationGenerator_emit_InfoDiagnostic_if_opt_out()
        {
            string source = @"
namespace People
{
    class C
    {
        void Start()
        {
        }
    }
}";

            (Compilation _, ImmutableArray<Diagnostic> diagnostics) result = CompileCsharp(source, OutputKind.DynamicallyLinkedLibrary);

            Assert.Equal(1, result.diagnostics.Length);
            Assert.Equal("WFPC-DBG", result.diagnostics[0].Id);
            Assert.Equal(DiagnosticSeverity.Info, result.diagnostics[0].Severity);
        }

        [Fact]
        public void ProjectConfigurationGenerator_fail_if_multiple_callsites()
        {
            string source = @"
namespace People
{
    class C
    {
        void Start()
        {
             ProjectConfiguration.Initialize();
             ProjectConfiguration.Initialize();
        }
    }
}";

            (Compilation _, ImmutableArray<Diagnostic> diagnostics) result = CompileCsharp(source, OutputKind.DynamicallyLinkedLibrary);

            Assert.Equal(2, result.diagnostics.Length);
            Assert.Equal(DiagnosticDescriptors.s_duplicateProjectConfigurationInitialize, result.diagnostics[0].Descriptor);
            Assert.Equal(new TextSpan(89, 33), result.diagnostics[0].Location.SourceSpan);
            Assert.Equal(DiagnosticDescriptors.s_duplicateProjectConfigurationInitialize, result.diagnostics[1].Descriptor);
            Assert.Equal(new TextSpan(138, 33), result.diagnostics[1].Location.SourceSpan);
        }

        [Fact]
        public void ProjectConfigurationGenerator_fail_if_project_type_unsupported()
        {
            string source = @"
namespace People
{
    class C
    {
        void Start()
        {
             ProjectConfiguration.Initialize();
        }
    }
}";

            (Compilation _, ImmutableArray<Diagnostic> diagnostics) result = CompileCsharp(source, OutputKind.DynamicallyLinkedLibrary);

            Assert.Equal(1, result.diagnostics.Length);
            Assert.Equal(DiagnosticDescriptors.s_errorUnsupportedProjectType, result.diagnostics[0].Descriptor);
            Assert.Equal(Location.None, result.diagnostics[0].Location);
        }

        [Fact]
        public void ProjectConfigurationGenerator_pass_if_project_type_WindowsApplication()
        {
            string source = @"
namespace People
{
    class C
    {
        void Start()
        {
             ProjectConfiguration.Initialize();
        }
    }
}";

            (Compilation _, ImmutableArray<Diagnostic> diagnostics) result = CompileCsharp(source, OutputKind.WindowsApplication);

            Assert.Equal(0, result.diagnostics.Length);
        }

        [Fact]
        public void ProjectConfigurationGenerator_fail_if_contains_manifest_with_dpi_config()
        {
            string source = @"
namespace People
{
    class C
    {
        void Start()
        {
             ProjectConfiguration.Initialize();
        }
    }
}";

            (Compilation outputCompilation, ImmutableArray<Diagnostic> diagnostics) result = CompileCsharp(source, OutputKind.WindowsApplication);
            Assert.Equal(0, result.diagnostics.Length);

            string output = result.outputCompilation.SyntaxTrees.Skip(1).First().ToString();

            Approvals.Verify(output);
        }

        [Fact]
        public void ProjectConfigurationGenerator_emit_correct_default_bootstrap()
        {
            string source = @"
namespace People
{
    class C
    {
        void Start()
        {
             ProjectConfiguration.Initialize();
        }
    }
}";

            (Compilation outputCompilation, ImmutableArray<Diagnostic> diagnostics) result = CompileCsharp(source, OutputKind.WindowsApplication);
            Assert.Equal(0, result.diagnostics.Length);

            string output = result.outputCompilation.SyntaxTrees.Skip(1).First().ToString();

            Approvals.Verify(output);
        }

        private (Compilation outputCompilation, ImmutableArray<Diagnostic> diagnostics) CompileCsharp(string source, OutputKind outputKind)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            List<MetadataReference> references = new List<MetadataReference>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (!assembly.IsDynamic)
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            CSharpCompilation compilation = CSharpCompilation.Create("original",
                                                                     new SyntaxTree[] { syntaxTree },
                                                                     references,
                                                                     new CSharpCompilationOptions(outputKind));

            ISourceGenerator generator = new ProjectConfigurationGenerator();

            CSharpGeneratorDriver driver = CSharpGeneratorDriver.Create(generator
                );
            driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

            return (outputCompilation, diagnostics);
        }
    }
}
