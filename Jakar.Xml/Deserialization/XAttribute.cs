// Jakar.Extensions :: Jakar.Xml
// 04/21/2022  7:23 PM

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Jakar.Extensions;


#nullable enable
namespace Jakar.Xml.Deserialization;


[SuppressMessage("ReSharper", "InconsistentNaming")]
public readonly ref struct JAttribute
{
    private readonly ReadOnlySpan<char> _span;
    public readonly  ReadOnlySpan<char> Key;
    public readonly  ReadOnlySpan<char> Value;
    public readonly  bool               IsNameSpace;


    public JAttribute( in ReadOnlySpan<char> span )
    {
        if ( span.StartsWith('<') ) { throw new FormatException($"Cannot start with {'<'}"); }

        if ( span.Contains('>') ) { throw new FormatException($"Cannot start with {'<'}"); }

        if ( span.EndsWith("</") ) { throw new FormatException($"Cannot start with {'<'}"); }

        if ( span.EndsWith('>') ) { throw new FormatException($"Cannot start with {'<'}"); }


        _span       = span;
        Key         = span;
        Value       = span;
        IsNameSpace = Key.Contains(Constants.XMLS, StringComparison.OrdinalIgnoreCase);
    }

    public KeyValuePair<string, string> ToPair() => new(Key.ToString(), Value.ToString());
}
