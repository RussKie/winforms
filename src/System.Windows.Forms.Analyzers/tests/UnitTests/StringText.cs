﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis.Text;

namespace System.Windows.Forms.Analyzers.Tests
{
    // Borrowed from https://github.com/dotnet/roslyn/blob/main/src/Compilers/Core/Portable/Text/StringText.cs

    /// <summary>
    /// Implementation of SourceText based on a <see cref="string"/> input
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal sealed class StringText : SourceText
    {
        private readonly string _source;
        private readonly Encoding? _encodingOpt;

        internal StringText(
            string source,
            Encoding? encodingOpt,
            ImmutableArray<byte> checksum = default(ImmutableArray<byte>),
            SourceHashAlgorithm checksumAlgorithm = SourceHashAlgorithm.Sha1)
            : base(checksum, checksumAlgorithm)
        {
            _source = source;
            _encodingOpt = encodingOpt;
        }

        public override Encoding? Encoding => _encodingOpt;

        /// <summary>
        /// Underlying string which is the source of this <see cref="StringText"/>instance
        /// </summary>
        public string Source => _source;

        /// <summary>
        /// The length of the text represented by <see cref="StringText"/>.
        /// </summary>
        public override int Length => _source.Length;

        /// <summary>
        /// Returns a character at given position.
        /// </summary>
        /// <param name="position">The position to get the character from.</param>
        /// <returns>The character.</returns>
        /// <exception cref="ArgumentOutOfRangeException">When position is negative or
        /// greater than <see cref="Length"/>.</exception>
        public override char this[int position]
        {
            get
            {
                // NOTE: we are not validating position here as that would not
                //       add any value to the range check that string accessor performs anyways.

                return _source[position];
            }
        }

        /// <summary>
        /// Provides a string representation of the StringText located within given span.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">When given span is outside of the text range.</exception>
        public override string ToString(TextSpan span)
        {
            if (span.End > Source.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(span));
            }

            if (span.Start == 0 && span.Length == Length)
            {
                return Source;
            }

            return Source.Substring(span.Start, span.Length);
        }

        public override void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
        {
            Source.CopyTo(sourceIndex, destination, destinationIndex, count);
        }

        public override void Write(TextWriter textWriter, TextSpan span, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (span.Start == 0 && span.End == Length)
            {
                textWriter.Write(Source);
            }
            else
            {
                base.Write(textWriter, span, cancellationToken);
            }
        }
    }
}

