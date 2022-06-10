#if NET6_0


// Jakar.Extensions :: Jakar.Mapper
// 06/09/2022  3:53 PM



namespace Jakar.Mapper;


[Serializable]
public readonly struct Pair
{
    private readonly ReadOnlyMemory<char> _key;   // Do not rename (binary serialization)
    private readonly object?              _value; // Do not rename (binary serialization)

    public ReadOnlySpan<char> Key   => _key.AsSpan();
    public object?            Value => _value;


    public Pair( KeyValuePair<string, object?> pair ) : this(pair.Key, pair.Value) { }
    public Pair( in ReadOnlySpan<char>         key, object? value ) : this(key.AsMemory(), value) { }
    public Pair( in ReadOnlyMemory<char> key, object? value )
    {
        _key   = key;
        _value = value;
    }


    public override string ToString() => $"{_key} : {_value}";
    public KeyValuePair<string, object?> ToKeyValuePair() => new(Key.ToString(), _value);
}
#endif
