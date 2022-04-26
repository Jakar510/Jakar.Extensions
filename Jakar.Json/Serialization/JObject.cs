// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:59 AM

using System.Globalization;



namespace Jakar.Json.Serialization;


public ref struct JObject
{
    private JWriter _writer;


    public JObject( ref JWriter writer )
    {
        _writer = writer;
        _writer.StartBlock('{');
    }


    public JObject Null( in ReadOnlySpan<char> key )
    {
        _writer.Indent().Append(key).Append('=').Null().Next();
        return this;
    }


    public JObject Add( in ReadOnlySpan<char> key, in char value )
    {
        _writer.Indent().Append(key).Append('=').Append(value).Next();
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in short value )
    {
        _writer.Indent().Append(key).Append('=').Append(value).Next();
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in ushort value )
    {
        _writer.Indent().Append(key).Append('=').Append(value).Next();
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in int value )
    {
        _writer.Indent().Append(key).Append('=').Append(value).Next();
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in uint value )
    {
        _writer.Indent().Append(key).Append('=').Append(value).Next();
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in long value )
    {
        _writer.Indent().Append(key).Append('=').Append(value).Next();
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in ulong value )
    {
        _writer.Indent().Append(key).Append('=').Append(value).Next();
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in float value )
    {
        _writer.Indent().Append(key).Append('=').Append(value).Next();
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in double value )
    {
        _writer.Indent().Append(key).Append('=').Append(value).Next();
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in decimal value )
    {
        _writer.Indent().Append(key).Append('=').Append(value).Next();
        return this;
    }


    public JObject Add( in ReadOnlySpan<char> key, in char? value ) => value.HasValue
                                                                           ? Add(key, value.Value)
                                                                           : Null(key);
    public JObject Add( in ReadOnlySpan<char> key, in short? value ) => value.HasValue
                                                                            ? Add(key, value.Value)
                                                                            : Null(key);
    public JObject Add( in ReadOnlySpan<char> key, in ushort? value ) => value.HasValue
                                                                             ? Add(key, value.Value)
                                                                             : Null(key);
    public JObject Add( in ReadOnlySpan<char> key, in int? value ) => value.HasValue
                                                                          ? Add(key, value.Value)
                                                                          : Null(key);
    public JObject Add( in ReadOnlySpan<char> key, in uint? value ) => value.HasValue
                                                                           ? Add(key, value.Value)
                                                                           : Null(key);
    public JObject Add( in ReadOnlySpan<char> key, in long? value ) => value.HasValue
                                                                           ? Add(key, value.Value)
                                                                           : Null(key);
    public JObject Add( in ReadOnlySpan<char> key, in ulong? value ) => value.HasValue
                                                                            ? Add(key, value.Value)
                                                                            : Null(key);
    public JObject Add( in ReadOnlySpan<char> key, in float? value ) => value.HasValue
                                                                            ? Add(key, value.Value)
                                                                            : Null(key);
    public JObject Add( in ReadOnlySpan<char> key, in double? value ) => value.HasValue
                                                                             ? Add(key, value.Value)
                                                                             : Null(key);
    public JObject Add( in ReadOnlySpan<char> key, in decimal? value ) => value.HasValue
                                                                              ? Add(key, value.Value)
                                                                              : Null(key);


    public JObject Add( in ReadOnlySpan<char> key, in string value ) => Add(key, value.AsSpan());
    public JObject Add( in ReadOnlySpan<char> key, in ReadOnlySpan<char> value )
    {
        _writer.Append(key).Append('=').Append('"').Append(value).Append('"').Next();
        return this;
    }


    public JObject Add( in ReadOnlySpan<char> key, in ISpanFormattable value, in int bufferSize ) => Add(key,                         value, bufferSize, CultureInfo.CurrentCulture);
    public JObject Add( in ReadOnlySpan<char> key, in ISpanFormattable value, in int bufferSize, in CultureInfo culture ) => Add(key, value, bufferSize, default, culture);
    public JObject Add( in ReadOnlySpan<char> key, in ISpanFormattable value, in int bufferSize, in ReadOnlySpan<char> format, in CultureInfo culture )
    {
        _writer.Indent().Append(key).Append('=').Append('"').Append(value, format, culture, bufferSize).Append('"').Next();
        return this;
    }


    public JObject Add<T>( in ReadOnlySpan<char> key, in T? value, in int bufferSize ) where T : struct, ISpanFormattable => Add(key,                         value, bufferSize, CultureInfo.CurrentCulture);
    public JObject Add<T>( in ReadOnlySpan<char> key, in T? value, in int bufferSize, in CultureInfo culture ) where T : struct, ISpanFormattable => Add(key, value, bufferSize, default, culture);
    public JObject Add<T>( in ReadOnlySpan<char> key, in T? value, in int bufferSize, in ReadOnlySpan<char> format, in CultureInfo culture ) where T : struct, ISpanFormattable
    {
        _writer.Indent().Append(key).Append('=').Append('"').Append(value, format, culture, bufferSize).Append('"').Next();
        return this;
    }


    public JObject Add( in KeyValuePair<string, string>  pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, char>    pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, short>   pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, ushort>  pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, int>     pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, uint>    pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, long>    pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, ulong>   pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, float>   pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, double>  pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, decimal> pair ) => Add(pair.Key, pair.Value);


    public JObject Add( in KeyValuePair<string, DateOnly>         pair ) => Add(pair.Key,                           pair.Value, 100);
    public JObject Add( in KeyValuePair<string, TimeOnly>         pair ) => Add(pair.Key,                           pair.Value, 100);
    public JObject Add( in KeyValuePair<string, TimeSpan>         pair ) => Add(pair.Key,                           pair.Value, 100);
    public JObject Add( in KeyValuePair<string, DateTimeOffset>   pair ) => Add(pair.Key,                           pair.Value, 100);
    public JObject Add( in KeyValuePair<string, DateTime>         pair ) => Add(pair.Key,                           pair.Value, 100);
    public JObject Add( in KeyValuePair<string, ISpanFormattable> pair, in int bufferSize = 1000 ) => Add(pair.Key, pair.Value, bufferSize);


    public JObject Add( in DictionaryEntry pair, in int bufferSize = 1000 )
    {
        ReadOnlySpan<char> k = pair.Key.ToString();

        return pair.Value switch
               {
                   null               => Null(k),
                   string v           => Add(k, v),
                   char v             => Add(k, v),
                   short v            => Add(k, v, 10),
                   ushort v           => Add(k, v, 10),
                   int v              => Add(k, v, 20),
                   uint v             => Add(k, v, 20),
                   long v             => Add(k, v, 30),
                   ulong v            => Add(k, v, 30),
                   float v            => Add(k, v, 3100),
                   double v           => Add(k, v, 6100),
                   decimal v          => Add(k, v, 2100),
                   DateOnly v         => Add(k, v, 100),
                   TimeOnly v         => Add(k, v, 100),
                   TimeSpan v         => Add(k, v, 100),
                   DateTimeOffset v   => Add(k, v, 100),
                   DateTime v         => Add(k, v, 100),
                   ISpanFormattable v => Add(k, v, bufferSize),
                   _                  => Add(k, pair.Value.ToString() ?? "null")
               };
    }


    public JArray AddArray( in ReadOnlySpan<char> key )
    {
        _writer.Indent().Append(key).Append('=');
        return new JArray(ref _writer);
    }
    public JObject AddObject( in ReadOnlySpan<char> key )
    {
        _writer.Indent().Append(key).Append('=');
        return new JObject(ref _writer);
    }


    public void Dispose() => _writer.FinishBlock('}');
}
