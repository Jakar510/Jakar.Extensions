// Jakar.Extensions :: Jakar.Xml
// 04/21/2022  6:24 PM

namespace Jakar.Xml.Deserialization;


public ref struct XDocument
{
    private readonly ReadOnlySpan<char> __xml;
    private          ReadOnlySpan<char> __span;
    private          long               __startIndex = 0;
    private          long               __endIndex   = 0;
    public           XNode              Current { get; } = default;


    public XDocument() : this( default ) { }
    public XDocument( ReadOnlySpan<char> xml )
    {
        if ( xml.IsEmpty ) { throw new ArgumentNullException( nameof(xml) ); }

        __span = __xml = xml;
    }


    public XDocument GetEnumerator() => this;
    public void      Reset()         => __span = __xml;
    public bool      MoveNext()      => false;
    public void      Dispose()       { }
}
