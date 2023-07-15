#nullable enable
namespace Jakar.Json.Deserialization;


[SuppressMessage( "ReSharper", "InconsistentNaming" )]
public readonly ref struct JNode
{
    private readonly ReadOnlyMemory<char> _span;
    public JNode( in ReadOnlyMemory<char> span ) => _span = span;


    public NodeEnumerator GetChildren() => new(_span);



    public ref struct NodeEnumerator
    {
        private readonly ReadOnlyMemory<char> _json;
        private          ReadOnlyMemory<char> _span;
        public           JNode                Current { get; private set; } = default;


        public NodeEnumerator( ReadOnlyMemory<char> span ) => _json = _span = span;
        public NodeEnumerator GetEnumerator() => this;
        public void Reset() => _span = _json;


        public bool MoveNext()
        {
            _span = _span.Trim();

            if ( _span.Span.IsNullOrWhiteSpace() )
            {
                Current = default;
                return false;
            }

            if ( _span.Span.StartsWith( '{' ) && _span.Span.EndsWith( '}' ) )
            {
                int start = _span.Span.IndexOf( '{' );
            }
            else if ( _span.Span.StartsWith( '[' ) && _span.Span.EndsWith( ']' ) ) { }


            return false;
        }
        public void Dispose() { }
    }
}
