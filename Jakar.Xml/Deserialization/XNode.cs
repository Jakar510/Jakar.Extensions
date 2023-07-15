// Jakar.Extensions :: Jakar.Xml
// 04/21/2022  6:20 PM

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Jakar.Extensions;



namespace Jakar.Xml.Deserialization;


[SuppressMessage( "ReSharper", "InconsistentNaming" )]
public readonly ref struct XNode
{
    private readonly ReadOnlyMemory<char> _xml;


    public bool HasAttributes => !Attributes.IsEmpty;

    public ReadOnlyMemory<char> Name => _xml[1.._xml.Span.IndexOf( '>' )]
       .Trim();

    public ReadOnlyMemory<char> Attributes => _xml[(Name.Length + 1).._xml.Span.IndexOf( '>' )]
       .Trim();

    public ReadOnlyMemory<char> StartTag => _xml[..(_xml.Span.IndexOf( '>' ) + 1)]
       .Trim();

    public ReadOnlySpan<char> EndTag
    {
        get
        {
            ReadOnlySpan<char> name   = Name.Span;
            Span<char>         buffer = stackalloc char[name.Length + 3];
            buffer[0] = '<';
            buffer[1] = '/';
            for ( int i = 0; i < name.Length; i++ ) { buffer[i + 2] = name[i]; }

            buffer[^1] = '>';
            return MemoryMarshal.CreateReadOnlySpan( ref buffer.GetPinnableReference(), buffer.Length );
        }
    }

    public ReadOnlyMemory<char> XMLS
    {
        get
        {
            var attributes = Attributes.Span;
            if ( !attributes.Contains( Constants.XMLS_TAG ) ) { return default; }

            int typeStart = attributes.IndexOf( Constants.XMLS_TAG ) + Constants.XMLS_TAG.Length;
            var temp      = attributes[typeStart..];
            return Attributes[..temp.IndexOf( '"' )];
        }
    }

    public ReadOnlyMemory<char> Content => _xml.Slice( StartTag.Length, _xml.Length - EndTag.Length );


    public XNode( ReadOnlyMemory<char> xml )
    {
        var span = xml.Span;
        if ( xml.IsEmpty ) { throw new ArgumentNullException( nameof(xml) ); }

        if ( !span.StartsWith( '<' ) ) { throw new FormatException( "Must start with '<'" ); }

        if ( !span.Contains( '>' ) ) { throw new FormatException( "Must contain '<'" ); }

        if ( !span.Contains( "</" ) ) { throw new FormatException( "Must contain '</'" ); }

        if ( !span.EndsWith( '>' ) ) { throw new FormatException( "Must End with '>'" ); }


        _xml = xml;
    }


    public AttributeEnumerator GetAttributes() => new(Attributes);
    public NodeEnumerator GetChildren() => new(Content);


    public IReadOnlyDictionary<string, string> ToAttributeCollection()
    {
        var attributes = new Dictionary<string, string>();

        foreach ( JAttribute attribute in GetAttributes() )
        {
            KeyValuePair<string, string> pair = attribute.ToPair();
            attributes.Add( pair.Key, pair.Value );
        }

        return attributes;
    }



    public ref struct AttributeEnumerator
    {
        private readonly ReadOnlyMemory<char> _xml;
        private          ReadOnlyMemory<char> _span;
        public           JAttribute           Current { get; private set; } = default;


        public AttributeEnumerator( ReadOnlyMemory<char> memory )
        {
            var span = memory.Span;
            if ( span.IsEmpty ) { throw new ArgumentNullException( nameof(span) ); }

            if ( span.Contains( '<' ) ) { throw new FormatException( $"Cannot start with {'<'}" ); }

            if ( span.Contains( '>' ) ) { throw new FormatException( $"Cannot start with {'<'}" ); }

            if ( span.Contains( "</" ) ) { throw new FormatException( $"Cannot start with {'<'}" ); }

            if ( span.Contains( '>' ) ) { throw new FormatException( $"Cannot start with {'<'}" ); }

            _xml = _span = memory;
        }

        public AttributeEnumerator GetEnumerator() => this;
        public void Reset() => _span = _xml;


        public bool MoveNext()
        {
            if ( _span.Span.IsNullOrWhiteSpace() )
            {
                Current = default;
                return false;
            }


            int                  start = _span.Span.IndexOf( ' ' );
            ReadOnlyMemory<char> temp  = _span[start..];
            temp = temp[..temp.Span.IndexOf( ' ' )];

            Current = new JAttribute( temp );
            return true;
        }
        public void Dispose() { }
    }



    public ref struct NodeEnumerator
    {
        private readonly ReadOnlyMemory<char> _xml;
        private          ReadOnlyMemory<char> _span;
        public           XNode                Current { get; } = default;


        public NodeEnumerator( ReadOnlyMemory<char> span ) => _xml = _span = span;
        public NodeEnumerator GetEnumerator() => this;
        public void Reset() => _span = _xml;


        public bool MoveNext() => false;
        public void Dispose() { }
    }
}
