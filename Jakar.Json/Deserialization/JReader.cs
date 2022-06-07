// Jakar.Extensions :: Jakar.Xml
// 04/21/2022  6:24 PM


using Newtonsoft.Json.Linq;
using JObject = Jakar.Json.Serialization.JObject;



#nullable enable
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

        _span = _xml = xml.Trim();
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
    public void Reset() => _span = _xml;
    public bool MoveNext() { return false; }
    public void Dispose() { }
}
