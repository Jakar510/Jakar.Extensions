// Jakar.Extensions :: Jakar.Xml
// 04/21/2022  6:24 PM

using System;



namespace Jakar.Xml.Deserialization;


public ref struct XDocument
{
    private readonly ReadOnlySpan<char> _xml;
    private          ReadOnlySpan<char> _span;
    private          long               _startIndex = 0;
    private          long               _endIndex   = 0;
    public           XNode              Current { get; } = default;


    public XDocument() : this( default ) { }
    public XDocument( ReadOnlySpan<char> xml )
    {
        if ( xml.IsEmpty ) { throw new ArgumentNullException( nameof(xml) ); }

        _span = _xml = xml;
    }


    public XDocument GetEnumerator() => this;
    public void      Reset()         => _span = _xml;
    public bool      MoveNext()      => false;
    public void      Dispose()       { }
}
