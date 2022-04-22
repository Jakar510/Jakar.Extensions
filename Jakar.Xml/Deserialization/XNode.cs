// Jakar.Extensions :: Jakar.Xml
// 04/21/2022  6:20 PM

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Jakar.Extensions.SpanAndMemory;
using Jakar.Extensions.Strings;



namespace Jakar.Xml;


[SuppressMessage("ReSharper", "InconsistentNaming")]
public readonly ref struct XNode
{
    private readonly ReadOnlySpan<char> _xml;
    public readonly  ReadOnlySpan<char> Name;
    private readonly ReadOnlySpan<char> _attributes;
    private readonly ReadOnlySpan<char> _content;


    public XNode( in ReadOnlySpan<char> xml )
    {
        _xml = xml;
        Validate(_xml, out ReadOnlySpan<char> start, out ReadOnlySpan<char> end, out Name, out _attributes);
        _content = _xml.Slice(start.Length, _xml.Length - end.Length);
    }


    public ReadOnlySpan<char> XMLS()
    {
        if ( _attributes.Contains(Constants.XMLS_TAG) )
        {
            int                typeStart = _attributes.IndexOf(Constants.XMLS_TAG) + Constants.XMLS_TAG.Length;
            ReadOnlySpan<char> temp      = _attributes[typeStart..];
            return temp[..temp.IndexOf('"')];
        }

        return default;
    }
    private static void Validate( in ReadOnlySpan<char> xml, out ReadOnlySpan<char> start, out ReadOnlySpan<char> end, out ReadOnlySpan<char> name, out ReadOnlySpan<char> attributes )
    {
        if ( xml.IsEmpty ) { throw new ArgumentNullException(nameof(xml)); }

        if ( !xml.StartsWith(Constants.OPEN_START) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }

        if ( !xml.Contains(Constants.CLOSE_START) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }

        int nameEndIndex = xml.IndexOf(Constants.SPACE);
        if ( nameEndIndex < 0 ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }


        name = xml[1..nameEndIndex].Trim();
        int close = xml.IndexOf(Constants.CLOSE_END);
        start      = xml[..( close + 1 )].Trim();
        attributes = xml[nameEndIndex..close].Trim();


        end = xml[( xml.Length - nameEndIndex - Constants.CLOSE_START.Length - 1 )..];

        if ( !end.EndsWith(Constants.CLOSE_END) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }

        if ( !end.Contains(Constants.OPEN_END) ) { throw new FormatException($"Cannot start with {Constants.OPEN_START}"); }
    }

    public AttributeEnumerator GetAttributes() => new(_attributes);
    public NodeEnumerator GetChildren() => new(_xml);


    public IReadOnlyDictionary<string, string> ToAttributeCollection()
    {
        var attributes = new Dictionary<string, string>();

        foreach ( XAttribute attribute in GetAttributes() )
        {
            KeyValuePair<string, string> pair = attribute.ToPair();
            attributes.Add(pair.Key, pair.Value);
        }

        // for ( var i = 0; i < node.Attributes.Count; i++ )
        // {
        //     XmlAttribute attribute = node.Attributes[i];
        //     attributes[attribute.Name] = attribute.InnerText;
        // }

        return attributes;
    }



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
