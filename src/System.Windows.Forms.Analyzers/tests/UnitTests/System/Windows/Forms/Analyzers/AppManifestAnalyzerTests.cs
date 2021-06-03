﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = System.Windows.Forms.Analyzers.Tests.CSharpAnalyzerVerifier<
    System.Windows.Forms.Analyzers.AppManifestAnalyzer>;
using VerifyVB = System.Windows.Forms.Analyzers.Tests.VisualBasicAnalyzerVerifier<
    System.Windows.Forms.Analyzers.AppManifestAnalyzer>;

namespace System.Windows.Forms.Analyzers.Tests
{
    public class AppManifestAnalyzerTests
    {
        private const string CSharCode = @"
    namespace ConsoleApplication1
    {
        class {|#0:TypeName|}
        {   
        }
    }";
        private const string VbCode = @"
Namespace ConsoleApplication1
    Class {|#0:TypeName|}
    End Class
End Namespace";

        [Fact]
        public async Task AppManifestAnalyzer_noop_if_no_manifest_file()
        {
            await new VerifyCS.Test
            {
                TestCode = CSharCode,
                TestState =
                {
                    AdditionalFiles = { }
                }
            }.RunAsync();
        }

        [Fact]
        public async Task AppManifestAnalyzer_noop_if_manifest_file_has_no_dpi_info()
        {
            StringText manifestFile = new(File.ReadAllText(@"System\Windows\Forms\Analyzers\MockData\nodpi.manifest"), null);
            await new VerifyCS.Test
            {
                TestCode = CSharCode,
                TestState =
                {
                     AdditionalFiles = { (@"C:\temp\app.manifest", manifestFile) }
                }
            }.RunAsync();
        }

        [Fact]
        public async Task AppManifestAnalyzer_noop_if_manifest_file_corrupt()
        {
            StringText manifestFile = new(File.ReadAllText(@"System\Windows\Forms\Analyzers\MockData\invalid.manifest"), null);
            await new VerifyCS.Test
            {
                TestCode = CSharCode,
                TestState =
                {
                     AdditionalFiles = { (@"C:\temp\app.manifest", manifestFile) }
                }
            }.RunAsync();
        }

        [Fact]
        public async Task AppManifestAnalyzer_warn_if_manifest_file_has_dpi_info_CSharp()
        {
            const string manifestFilePath = @"C:\temp\app.manifest";
            StringText manifestFile = new(File.ReadAllText(@"System\Windows\Forms\Analyzers\MockData\dpi.manifest"), null);
            await new VerifyCS.Test
            {
                TestCode = CSharCode,
                TestState =
                {
                     AdditionalFiles = { (manifestFilePath, manifestFile) }
                },
                ExpectedDiagnostics =
                {
                    new DiagnosticResult(DiagnosticDescriptors.s_migrateHighDpiSettings_CSharp)
                        .WithArguments(manifestFilePath, ApplicationConfig.PropertyNameCSharp.HighDpiMode)
                }
            }.RunAsync();
        }

        [Fact]
        public async Task AppManifestAnalyzer_warn_if_manifest_file_has_dpi_info_VB()
        {
            const string manifestFilePath = @"C:\temp\app.manifest";
            StringText manifestFile = new(File.ReadAllText(@"System\Windows\Forms\Analyzers\MockData\dpi.manifest"), null);
            await new VerifyVB.Test
            {
                TestCode = VbCode,
                TestState =
                {
                     AdditionalFiles = { (manifestFilePath, manifestFile) }
                },
                ExpectedDiagnostics =
                {
                    new DiagnosticResult(DiagnosticDescriptors.s_migrateHighDpiSettings_VB)
                        .WithArguments(manifestFilePath, ApplicationConfig.PropertyNameVisualBasic.HighDpiMode)
                }
            }.RunAsync();
        }
    }
}
