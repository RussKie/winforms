﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace System.Windows.Forms.SourceGenerators.Tests
{
    // Borrowed from https://github.com/dotnet/roslyn/blob/main/src/Compilers/Test/Core/Mocks/TestAdditionalText.cs

    public sealed class TestAdditionalText : AdditionalText
    {
        private readonly SourceText _text;

        public TestAdditionalText(string path, SourceText text)
        {
            Path = path;
            _text = text;
        }

        public TestAdditionalText(string text = "", Encoding? encoding = null, string path = "dummy")
            : this(path, new StringText(text, encoding))
        {
        }

        public override string Path { get; }

        public override SourceText GetText(CancellationToken cancellationToken = default) => _text;
    }
}

