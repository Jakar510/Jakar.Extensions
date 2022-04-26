// Jakar.Extensions :: Jakar.Xml
// 04/21/2022  6:20 PM

using System.Diagnostics.CodeAnalysis;
using Jakar.Extensions.SpanAndMemory;



namespace Jakar.Json.Deserialization;


[SuppressMessage("ReSharper", "InconsistentNaming")]
public readonly ref struct JNode
{
    private readonly ReadOnlySpan<char> _span;
    public JNode( in ReadOnlySpan<char> span ) => _span = span;


    public NodeEnumerator GetChildren() => new(_span);



    public ref struct NodeEnumerator
    {
        private readonly ReadOnlySpan<char> _json;
        private          ReadOnlySpan<char> _span;
        public           JNode              Current { get; private set; } = default;


        public NodeEnumerator( in ReadOnlySpan<char> span ) => _json = _span = span;
        public NodeEnumerator GetEnumerator() => this;
        public void Reset() => _span = _json;


        public bool MoveNext() { return false; }
        public void Dispose() { }
    }
}
