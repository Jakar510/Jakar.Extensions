using Jakar.Extensions.SpanAndMemory;
using Newtonsoft.Json.Linq;



namespace Jakar.Json.Deserialization;


[SuppressMessage("ReSharper", "InconsistentNaming")]
public readonly ref struct JNode
{
    private readonly ReadOnlySpan<char> _span;
    public JNode( in ReadOnlySpan<char> span ) => _span = span;


    public NodeEnumerator GetChildren() => new(_span);


    // public static JToken Parse( string json, JsonLoadSettings? settings )
    // {
    //     JsonReader reader = new JsonTextReader(new StringReader(json));
    //     reader = new JsonTextReader(new StreamReader(json));
    //
    //     using ( reader )
    //     {
    //         JToken t = JToken.Load(reader, settings);
    //
    //         while ( reader.Read() )
    //         {
    //             // Any content encountered here other than a comment will throw in the reader.
    //         }
    //
    //         return t;
    //     }
    // }



    /*
{
    "test": {
        "one": 1,
        "two": 2
    }
}

    
[
    "test": {
        "one": 1,
        "two": 2
    }
]
     */
    public ref struct NodeEnumerator
    {
        private readonly ReadOnlySpan<char> _json;
        private          ReadOnlySpan<char> _span;
        public           JNode              Current { get; private set; } = default;


        public NodeEnumerator( in ReadOnlySpan<char> span ) => _json = _span = span;
        public NodeEnumerator GetEnumerator() => this;
        public void Reset() => _span = _json;


        public bool MoveNext()
        {
            _span = _span.Trim();

            if ( Spans.IsNullOrWhiteSpace(_span) )
            {
                Current = default;
                return false;
            }

            if ( _span.StartsWith('{') && _span.EndsWith('}') )
            {
                var start = _span.IndexOf('{');
            }
            else if ( _span.StartsWith('[') && _span.EndsWith(']') ) { }


            return false;
        }
        public void Dispose() { }
    }
}
