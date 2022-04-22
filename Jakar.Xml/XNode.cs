// Jakar.Extensions :: Jakar.Xml
// 04/21/2022  6:20 PM

using System;
using System.Diagnostics.CodeAnalysis;
using Jakar.Extensions.SpanAndMemory;
using Jakar.Extensions.Strings;



namespace Jakar.Xml;


[SuppressMessage("ReSharper", "InconsistentNaming")]
public readonly ref struct XNode
{
    private readonly ReadOnlySpan<char> _xml;
    public readonly  ReadOnlySpan<char> Name;
    public readonly  ReadOnlySpan<char> Type;
    private readonly ReadOnlySpan<char> _attributes;


    public XNode( in ReadOnlySpan<char> xml )
    {
        Validate(xml, out ReadOnlySpan<char> name, out ReadOnlySpan<char> attributes, out ReadOnlySpan<char> type);
        _xml = xml;

        Name        = name;
        _attributes = attributes;
        Type        = type;
    }
    private static void Validate( in ReadOnlySpan<char> xml, out ReadOnlySpan<char> name, out ReadOnlySpan<char> attributes, out ReadOnlySpan<char> type )
    {
        if ( xml.IsEmpty ) { throw new ArgumentNullException(nameof(xml)); }

        if ( !xml.StartsWith(Constants.OPEN_START) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }

        if ( !xml.Contains(Constants.CLOSE_START) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }

        int nameEndIndex = xml.IndexOf(Constants.SPACE);
        if ( nameEndIndex < 0 ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }


        name       = xml[1..nameEndIndex];
        attributes = xml[nameEndIndex..xml.IndexOf(Constants.CLOSE_START)].Trim();

        if ( attributes.Contains(Constants.XMLS_TAG) )
        {
            int                start = attributes.IndexOf(Constants.XMLS_TAG) + Constants.XMLS_TAG.Length;
            ReadOnlySpan<char> temp  = attributes[start..];
            type = temp[..temp.IndexOf('"')];
        }
        else { type = default; }


        ReadOnlySpan<char> end = xml[( xml.Length - nameEndIndex - 3 )..];

        if ( !end.EndsWith(Constants.CLOSE_END) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }

        if ( !end.Contains(Constants.OPEN_END) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }
    }

    public AttributeEnumerator GetAttributes() => new(_attributes);
    public NodeEnumerator GetChildren() => new(_xml);



    public ref struct AttributeEnumerator
    {
        private readonly ReadOnlySpan<char>        _xml;
        private          ReadOnlySpan<char>        _span;
        private readonly SpanSplitEnumerator<char> _enumerator;
        public           XAttribute                Current { get; private set; } = default;


        public AttributeEnumerator( in ReadOnlySpan<char> span )
        {
            if ( span.IsEmpty ) { throw new ArgumentNullException(nameof(span)); }

            if ( span.Contains(Constants.OPEN_START) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }

            if ( span.Contains(Constants.OPEN_END) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }

            if ( span.Contains(Constants.CLOSE_START) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }

            if ( span.Contains(Constants.CLOSE_END) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }

            _xml        = _span = span;
            _enumerator = span.SplitOn(' ');
        }
        public AttributeEnumerator GetEnumerator() => this;
        public void Reset() => _span = _xml;


        public bool MoveNext() { return false; }
        public void Dispose() { }
    }



    public ref struct NodeEnumerator
    {
        private readonly ReadOnlySpan<char> _xml;
        private          ReadOnlySpan<char> _span;
        public           XNode              Current { get; private set; } = default;


        public NodeEnumerator( in ReadOnlySpan<char> span ) => _xml = _span = span;
        public NodeEnumerator GetEnumerator() => this;
        public void Reset() => _span = _xml;


        public bool MoveNext() { return false; }
        public void Dispose() { }
    }
}
