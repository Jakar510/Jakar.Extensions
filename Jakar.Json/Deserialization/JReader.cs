// Jakar.Extensions :: Jakar.Xml
// 04/21/2022  6:24 PM


namespace Jakar.Json.Deserialization;


public ref struct JReader
{
    private readonly ReadOnlyMemory<char> _json;
    private          ReadOnlyMemory<char> _span;
    private          long                 _startIndex = default;
    private          long                 _endIndex   = default;
    public           JNode                Current { get; private set; } = default;


    public JReader( in ReadOnlyMemory<char> json )
    {
        if ( json.IsEmpty ) { throw new ArgumentNullException( nameof(json) ); }

        _span = _json = json.Trim();
    }


    // public static JToken Parse( string json, JsonLoadSettings? settings, JsonReader? reader = default )
    // {
    //     // reader ??= new JTokenReader(JToken.FromObject(new Version(1, 2, 3, 4)));
    //     reader ??= new JsonTextReader(new StreamReader(json));
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
    "foo": {
        "one": 1,
        "two": 2
    }
    "bar": [
        "random": {
            "one": 1,
            "two": 2
        }
    ]
}
     */
    public JReader GetEnumerator() => this;
    public void    Reset()         => _span = _json;
    public bool    MoveNext()      => false;
    public void    Dispose()       { }
}
