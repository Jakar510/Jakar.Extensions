using System.Collections;
using System.Text;
using Jakar.Json.Deserialization;



namespace Jakar.Json.Serialization;


public readonly ref struct JWriter
{
    private readonly JContext      _context;
    private readonly StringBuilder _sb = new();

    public JWriter() : this(new JContext()) { }
    public JWriter( in JContext context ) => _context = context;


    public JWriter Add( in JObject parent, in ReadOnlySpan<char> key, in IEnumerable<IJsonizer> enumerable )
    {
        using JArray node = parent.AddArray(key);
        return Add(node, enumerable);
    }
    public JWriter Add( in JArray parent, in IEnumerable<IJsonizer> enumerable )
    {
        foreach ( IJsonizer item in enumerable )
        {
            using JObject node = parent.AddObject();
            item.Serialize(node);
        }

        return this;
    }
    public JWriter Add( in JObject parent, in ReadOnlySpan<char> key, in IDictionary dictionary )
    {
        using JObject node = parent.AddObject(key);

        foreach ( DictionaryEntry pair in dictionary ) { node.Add(pair); }

        return this;
    }
    public JWriter Add( in JObject parent, in ReadOnlySpan<char> key, in IDictionary<string, IJsonizer> dictionary )
    {
        using JObject node = parent.AddObject(key);

        foreach ( ( string? k, IJsonizer? value ) in dictionary )
        {
            using JObject item = node.AddObject(k);
            value.Serialize(item);
        }

        return this;
    }


    public override string ToString() => _sb.ToString();

    // public void Dispose() => _context.Dispose();
}
