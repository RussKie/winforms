// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace System.Windows.Forms.SourceGenerators.Tests
{
    public class ExternalHighDpiSettingsVerifierTests
    {
        [Fact]
        public void ExternalHighDpiSettingsVerifier_Verify_noop_if_no_manifest_file()
        {
            (Compilation _, ImmutableArray<Diagnostic> diagnostics) result = CompileCsharp(additionalFiles: Array.Empty<AdditionalText>());

            Assert.Equal(0, result.diagnostics.Length);
        }

        [Fact]
        public void ExternalHighDpiSettingsVerifier_Verify_noop_if_manifest_file_has_no_dpi_info()
        {
            TestAdditionalText manifestFile = new(File.ReadAllText(@"System\Windows\Forms\MockData\nodpi.manifest"), path: @"C:\temp\app.manifest");
            (Compilation _, ImmutableArray<Diagnostic> diagnostics) result = CompileCsharp(additionalFiles: new[] { manifestFile });

            Assert.Equal(0, result.diagnostics.Length);
        }

        [Fact]
        public void ExternalHighDpiSettingsVerifier_Verify_fail_if_manifest_file_corrupt()
        {
            TestAdditionalText manifestFile = new(File.ReadAllText(@"System\Windows\Forms\MockData\invalid.manifest"), path: @"C:\temp\app.manifest");
            (Compilation _, ImmutableArray<Diagnostic> diagnostics) result = CompileCsharp(additionalFiles: new[] { manifestFile });

            Assert.Equal(1, result.diagnostics.Length);
            Assert.Equal("CS8785", result.diagnostics[0].Descriptor.Id); // Compiler error
        }

        [Fact]
        public void ExternalHighDpiSettingsVerifier_Verify_warn_if_manifest_file_has_dpi_info()
        {
            TestAdditionalText manifestFile = new(File.ReadAllText(@"System\Windows\Forms\MockData\dpi.manifest"), path: @"C:\temp\app.manifest");
            (Compilation _, ImmutableArray<Diagnostic> diagnostics) result = CompileCsharp(additionalFiles: new[] { manifestFile });

            Assert.Equal(1, result.diagnostics.Length);
            Assert.Equal(DiagnosticDescriptors.s_migrateHighDpiSettings, result.diagnostics[0].Descriptor);
            Assert.Equal(Location.None, result.diagnostics[0].Location);
        }

        private (Compilation outputCompilation, ImmutableArray<Diagnostic> diagnostics) CompileCsharp(IEnumerable<AdditionalText> additionalFiles)
        {
            ISourceGenerator generator = new TestSourceGenerator();
            CSharpCompilation compilation = CSharpCompilation.Create("original");
            CSharpGeneratorDriver driver = CSharpGeneratorDriver.Create(new[] { generator }, additionalTexts: additionalFiles);
            driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

            return (outputCompilation, diagnostics);
        }

        private class TestSourceGenerator : ISourceGenerator
        {
            public void Execute(GeneratorExecutionContext context)
            {
                ExternalHighDpiSettingsVerifier verifier = new();
                verifier.Verify(context);
            }

            public void Initialize(GeneratorInitializationContext context)
            {
            }
        }
    }
}
