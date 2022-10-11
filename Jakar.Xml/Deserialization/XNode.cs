// Jakar.Extensions :: Jakar.Xml
// 04/21/2022  6:20 PM

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Jakar.Extensions;



namespace Jakar.Xml.Deserialization;


[SuppressMessage( "ReSharper", "InconsistentNaming" )]
public readonly ref struct XNode
{
    private readonly ReadOnlySpan<char> _xml;
    public           bool               HasAttributes => !Attributes.IsEmpty;

    public ReadOnlySpan<char> Name => _xml[1.._xml.IndexOf( '>' )]
       .Trim();

    public ReadOnlySpan<char> Attributes => _xml[(Name.Length + 1).._xml.IndexOf( '>' )]
       .Trim();

    public ReadOnlySpan<char> StartTag => _xml[..(_xml.IndexOf( '>' ) + 1)]
       .Trim();

    public ReadOnlySpan<char> EndTag
    {
        get
        {
            ReadOnlySpan<char> name   = Name;
            Span<char>         buffer = stackalloc char[name.Length + 3];
            buffer[0] = '<';
            buffer[1] = '/';
            for (int i = 0; i < name.Length; i++) { buffer[i + 2] = name[i]; }

            buffer[^1] = '>';
            return default;
        }
    }

    public ReadOnlySpan<char> XMLS
    {
        get
        {
            ReadOnlySpan<char> attributes = Attributes;
            if (!attributes.Contains( Constants.XMLS_TAG )) { return default; }

            int                typeStart = attributes.IndexOf( Constants.XMLS_TAG ) + Constants.XMLS_TAG.Length;
            ReadOnlySpan<char> temp      = attributes[typeStart..];
            return temp[..temp.IndexOf( '"' )];
        }
    }

    public ReadOnlySpan<char> Content => _xml.Slice( StartTag.Length, _xml.Length - EndTag.Length );


    public XNode( in ReadOnlySpan<char> xml )
    {
        if (xml.IsEmpty) { throw new ArgumentNullException( nameof(xml) ); }

        if (!xml.StartsWith( '<' )) { throw new FormatException( "Must start with '<'" ); }

        if (!xml.Contains( '>' )) { throw new FormatException( "Must contain '<'" ); }


        if (!xml.Contains( "</" )) { throw new FormatException( "Must contain '</'" ); }

        if (!xml.EndsWith( '>' )) { throw new FormatException( "Must End with '>'" ); }


        _xml = xml;
    }


    public AttributeEnumerator GetAttributes() => new(Attributes);
    public NodeEnumerator GetChildren() => new(Content);


    public IReadOnlyDictionary<string, string> ToAttributeCollection()
    {
        var attributes = new Dictionary<string, string>();

        foreach (JAttribute attribute in GetAttributes())
        {
            KeyValuePair<string, string> pair = attribute.ToPair();
            attributes.Add( pair.Key, pair.Value );
        }

        return attributes;
    }



    public ref struct AttributeEnumerator
    {
        private readonly ReadOnlySpan<char> _xml;
        private          ReadOnlySpan<char> _span;
        public           JAttribute         Current { get; private set; } = default;


        public AttributeEnumerator( in ReadOnlySpan<char> span )
        {
            if (span.IsEmpty) { throw new ArgumentNullException( nameof(span) ); }

            if (span.Contains( '<' )) { throw new FormatException( $"Cannot start with {'<'}" ); }

            if (span.Contains( '>' )) { throw new FormatException( $"Cannot start with {'<'}" ); }

            if (span.Contains( "</" )) { throw new FormatException( $"Cannot start with {'<'}" ); }

            if (span.Contains( '>' )) { throw new FormatException( $"Cannot start with {'<'}" ); }

            _xml = _span = span;
        }

        public AttributeEnumerator GetEnumerator() => this;
        public void Reset() => _span = _xml;


        public bool MoveNext()
        {
            if (_span.IsNullOrWhiteSpace())
            {
                Current = default;
                return false;
            }


            int                start = _span.IndexOf( ' ' );
            ReadOnlySpan<char> temp  = _span[start..];
            temp = temp[..temp.IndexOf( ' ' )];

            Current = new JAttribute( temp );
            return true;
        }
        public void Dispose() { }
    }



    public ref struct NodeEnumerator
    {
        private readonly ReadOnlySpan<char> _xml;
        private          ReadOnlySpan<char> _span;
        public           XNode              Current { get; } = default;


        public NodeEnumerator( in ReadOnlySpan<char> span ) => _xml = _span = span;
        public NodeEnumerator GetEnumerator() => this;
        public void Reset() => _span = _xml;


        public bool MoveNext() => false;
        public void Dispose() { }
    }
}
