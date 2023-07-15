// Jakar.Extensions :: Jakar.Xml
// 04/21/2022  7:23 PM

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Jakar.Extensions;



namespace Jakar.Xml.Deserialization;


[SuppressMessage( "ReSharper", "InconsistentNaming" )]
public readonly ref struct JAttribute
{
    private readonly ReadOnlyMemory<char> _span;
    public readonly  ReadOnlyMemory<char> Key;
    public readonly  ReadOnlyMemory<char> Value;
    public readonly  bool                 IsNameSpace;


    public JAttribute( ReadOnlyMemory<char> memory )
    {
        var span = memory.Span;
        if ( span.StartsWith( '<' ) ) { throw new FormatException( $"Cannot start with {'<'}" ); }

        if ( span.Contains( '>' ) ) { throw new FormatException( $"Cannot start with {'<'}" ); }

        if ( span.EndsWith( "</" ) ) { throw new FormatException( $"Cannot start with {'<'}" ); }

        if ( span.EndsWith( '>' ) ) { throw new FormatException( $"Cannot start with {'<'}" ); }


        _span       = memory;
        Key         = memory;
        Value       = memory;
        IsNameSpace = span.Contains( Constants.XMLS, StringComparison.OrdinalIgnoreCase );
    }

    public KeyValuePair<string, string> ToPair() => new(Key.ToString(), Value.ToString());
}
