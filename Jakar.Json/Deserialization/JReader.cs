// Jakar.Extensions :: Jakar.Xml
// 04/21/2022  6:24 PM


namespace Jakar.Json.Deserialization;


public ref struct JReader
{
    private readonly ReadOnlySpan<char> _xml;
    private          ReadOnlySpan<char> _span;
    private          long               _startIndex = default;
    private          long               _endIndex   = default;
    public           JNode              Current { get; private set; } = default;

    public JReader( in ReadOnlySpan<char> xml )
    {
        if ( xml.IsEmpty ) { throw new ArgumentNullException(nameof(xml)); }

        _span = _xml = xml;
    }


    public JReader GetEnumerator() => this;
    public void Reset() => _span = _xml;
    public bool MoveNext() { return false; }
    public void Dispose() { }
}
