namespace Jakar.Json.Deserialization;


[SuppressMessage( "ReSharper", "InconsistentNaming" )]
public readonly ref struct JNode( in ReadOnlyMemory<char> span )
{
    private readonly ReadOnlyMemory<char> _span = span;


    public NodeEnumerator GetChildren() => new(_span);



    public ref struct NodeEnumerator
    {
        private readonly ReadOnlyMemory<char> _json;
        private          ReadOnlyMemory<char> _span;
        public           JNode                Current { get; private set; } = default;


        public NodeEnumerator( ReadOnlyMemory<char> span ) => _json = _span = span;
        public NodeEnumerator GetEnumerator() => this;
        public void           Reset()         => _span = _json;


        public bool MoveNext()
        {
            _span = _span.Trim();
            ReadOnlySpan<char> span = _span.Span;

            if ( span.IsNullOrWhiteSpace() )
            {
                Current = default;
                return false;
            }

            if ( span.Contains( '{' ) && span.EndsWith('}' ) )
            {
                int start = _span.Span.IndexOf( '{' );
            }

            // else if ( MemoryExtensions.EndsWith( span, '[' ) && MemoryExtensions.EndsWith( span, ']' ) ) { }


            return false;
        }
        public void Dispose() { }
    }
}
