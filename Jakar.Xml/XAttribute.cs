﻿// Jakar.Extensions :: Jakar.Xml
// 04/21/2022  7:23 PM

using System;
using System.Diagnostics.CodeAnalysis;
using Jakar.Extensions.SpanAndMemory;



namespace Jakar.Xml;


[SuppressMessage("ReSharper", "InconsistentNaming")]
public readonly ref struct XAttribute
{
    private readonly ReadOnlySpan<char> _span;
    public readonly  ReadOnlySpan<char> Key;
    public readonly  ReadOnlySpan<char> Value;
    public readonly  bool               IsNameSpace;

    public XAttribute( ReadOnlySpan<char> span )
    {
        if ( span.StartsWith(Constants.OPEN_START) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }

        if ( span.Contains(Constants.OPEN_END) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }

        if ( span.EndsWith(Constants.CLOSE_START) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }

        if ( span.EndsWith(Constants.CLOSE_END) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }


        _span       = span;
        Key         = span;
        Value       = span;
        IsNameSpace = Key.Contains(Constants.XMLS, StringComparison.OrdinalIgnoreCase);
    }
}
