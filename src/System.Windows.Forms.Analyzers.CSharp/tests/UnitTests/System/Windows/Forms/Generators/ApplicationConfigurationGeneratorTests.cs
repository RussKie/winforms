// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms.Analyzers.Tests;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.Analyzers.Generators.Tests
{
    [UsesVerify]
    public partial class ApplicationConfigurationGeneratorTests
    {
        private readonly ITestOutputHelper _output;

        public ApplicationConfigurationGeneratorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public Task ProjectConfigurationGenerator_fail_if_project_type_unsupported()
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

            GeneratorDriver result = CompileCsharp(source, OutputKind.WindowsApplication);

            return Verifier.Verify(result);
        }

        [Fact]
        public Task ProjectConfigurationGenerator_pass_if_project_type_WindowsApplication()
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

            GeneratorDriver result = CompileCsharp(source, OutputKind.WindowsApplication);

            return Verifier.Verify(result);
        }

        [Fact]
        public Task ProjectConfigurationGenerator_emit_correct_default_bootstrap()
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

            GeneratorDriver result = CompileCsharp(source, OutputKind.WindowsApplication);

            return Verifier.Verify(result);
        }

        private GeneratorDriver CompileCsharp(string source, OutputKind outputKind, CompilerAnalyzerConfigOptions configOptions = null)
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

            return driver.RunGenerators(compilation);
        }
    }
}
