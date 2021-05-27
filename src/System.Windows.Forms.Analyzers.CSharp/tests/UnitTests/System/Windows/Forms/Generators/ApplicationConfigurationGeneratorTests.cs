// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms.Generators;
using ApprovalTests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.Analyzers.Tests
{
    public partial class ApplicationConfigurationGeneratorTests
    {
        private readonly ITestOutputHelper _output;

        public ApplicationConfigurationGeneratorTests(ITestOutputHelper output)
        {
            _output = output;
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
             ApplicationConfiguration.Initialize();
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
             ApplicationConfiguration.Initialize();
        }
    }
}";

            (Compilation _, ImmutableArray<Diagnostic> diagnostics) result = CompileCsharp(source, OutputKind.WindowsApplication);

            Assert.Equal(0, result.diagnostics.Length);
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
             ApplicationConfiguration.Initialize();
        }
    }
}";

            (Compilation outputCompilation, ImmutableArray<Diagnostic> diagnostics) result = CompileCsharp(source, OutputKind.WindowsApplication);

            if (!result.diagnostics.IsEmpty)
            {
                foreach (Diagnostic d in result.diagnostics)
                {
                    _output.WriteLine(d.ToString());
                }
            }

            Assert.Equal(0, result.diagnostics.Length);

            string output = result.outputCompilation.SyntaxTrees.Skip(1).First().ToString();
            Approvals.Verify(output);
        }

        private (Compilation outputCompilation, ImmutableArray<Diagnostic> diagnostics) CompileCsharp(string source, OutputKind outputKind, CompilerAnalyzerConfigOptions configOptions = null)
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

            CSharpParseOptions regularParseOptions = new(kind: SourceCodeKind.Regular, documentationMode: DocumentationMode.Parse);
            CompilerAnalyzerConfigOptionsProvider analyzerConfigOptionsProvider = configOptions is null
                ? CompilerAnalyzerConfigOptionsProvider.Empty
                : new(ImmutableDictionary<object, AnalyzerConfigOptions>.Empty, configOptions);
            GeneratorDriver driver = CSharpGeneratorDriver.Create(parseOptions: regularParseOptions,
                                                                  generators: ImmutableArray.Create(new ApplicationConfigurationGenerator()),
                                                                  optionsProvider: analyzerConfigOptionsProvider);

            CSharpCompilation compilation = CSharpCompilation.Create("original",
                                                                     new SyntaxTree[] { syntaxTree },
                                                                     references,
                                                                     new CSharpCompilationOptions(outputKind));

            driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

            return (outputCompilation, diagnostics);
        }
    }
}
