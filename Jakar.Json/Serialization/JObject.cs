// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:59 AM

using System.Globalization;
using System.Runtime.CompilerServices;
using Jakar.Extensions;


#nullable enable
namespace Jakar.Json.Serialization;


public ref struct JObject
{
    public const char START = '{';
    public const char END   = '}';

    private readonly bool    _shouldIndent;
    internal         JWriter writer;


    public JObject( ref JWriter writer, in bool shouldIndent )
    {
        _shouldIndent = shouldIndent;
        this.writer   = writer;
    }
    public void Empty() => writer.Append(START).Append(' ').Append(END).FinishBlock();
    public JObject Begin()
    {
        writer.StartBlock(START, _shouldIndent);
        return this;
    }
    public void Complete() => writer.FinishBlock(END);


    private JWriter Start( in ReadOnlySpan<char> key ) => writer.Indent().Append('"').Append(key).Append('"').Append(':').Append(' ');
    public JObject Null( in ReadOnlySpan<char> key )
    {
        Start(key).Null().Next();
        return this;
    }


    public JObject Add( in char value, [CallerArgumentExpression("value")] string key = "" )
    {
        Start(key).Append(value).Next();
        return this;
    }
    public JObject Add( in short value, [CallerArgumentExpression("value")] string key = "" )
    {
        Start(key).Append(value).Next();
        return this;
    }
    public JObject Add( in ushort value, [CallerArgumentExpression("value")] string key = "" )
    {
        Start(key).Append(value).Next();
        return this;
    }
    public JObject Add( in int value, [CallerArgumentExpression("value")] string key = "" )
    {
        Start(key).Append(value).Next();
        return this;
    }
    public JObject Add( in uint value, [CallerArgumentExpression("value")] string key = "" )
    {
        Start(key).Append(value).Next();
        return this;
    }
    public JObject Add( in long value, [CallerArgumentExpression("value")] string key = "" )
    {
        Start(key).Append(value).Next();
        return this;
    }
    public JObject Add( in ulong value, [CallerArgumentExpression("value")] string key = "" )
    {
        Start(key).Append(value).Next();
        return this;
    }
    public JObject Add( in float value, [CallerArgumentExpression("value")] string key = "" )
    {
        Start(key).Append(value).Next();
        return this;
    }
    public JObject Add( in double value, [CallerArgumentExpression("value")] string key = "" )
    {
        Start(key).Append(value).Next();
        return this;
    }
    public JObject Add( in decimal value, [CallerArgumentExpression("value")] string key = "" )
    {
        Start(key).Append(value).Next();
        return this;
    }


    public JObject Add( in char? value, [CallerArgumentExpression("value")] string key = "" ) => value.HasValue
                                                                                                     ? Add(value.Value, key)
                                                                                                     : Null(key);
    public JObject Add( in short? value, [CallerArgumentExpression("value")] string key = "" ) => value.HasValue
                                                                                                      ? Add(value.Value, key)
                                                                                                      : Null(key);
    public JObject Add( in ushort? value, [CallerArgumentExpression("value")] string key = "" ) => value.HasValue
                                                                                                       ? Add(value.Value, key)
                                                                                                       : Null(key);
    public JObject Add( in int? value, [CallerArgumentExpression("value")] string key = "" ) => value.HasValue
                                                                                                    ? Add(value.Value, key)
                                                                                                    : Null(key);
    public JObject Add( in uint? value, [CallerArgumentExpression("value")] string key = "" ) => value.HasValue
                                                                                                     ? Add(value.Value, key)
                                                                                                     : Null(key);
    public JObject Add( in long? value, [CallerArgumentExpression("value")] string key = "" ) => value.HasValue
                                                                                                     ? Add(value.Value, key)
                                                                                                     : Null(key);
    public JObject Add( in ulong? value, [CallerArgumentExpression("value")] string key = "" ) => value.HasValue
                                                                                                      ? Add(value.Value, key)
                                                                                                      : Null(key);
    public JObject Add( in float? value, [CallerArgumentExpression("value")] string key = "" ) => value.HasValue
                                                                                                      ? Add(value.Value, key)
                                                                                                      : Null(key);
    public JObject Add( in double? value, [CallerArgumentExpression("value")] string key = "" ) => value.HasValue
                                                                                                       ? Add(value.Value, key)
                                                                                                       : Null(key);
    public JObject Add( in decimal? value, [CallerArgumentExpression("value")] string key = "" ) => value.HasValue
                                                                                                        ? Add(value.Value, key)
                                                                                                        : Null(key);


    public JObject Add( in string value, [CallerArgumentExpression("value")] string key = "" ) => Add(key.AsSpan(), value.AsSpan());
    public JObject Add( in ReadOnlySpan<char> key, in ReadOnlySpan<char> value )
    {
        Start(key).Append('"').Append(value).Append('"').Next();
        return this;
    }


    public JObject Add( in ISpanFormattable value, in int bufferSize, [CallerArgumentExpression("value")] string      key                                                     = "" ) => Add(value, bufferSize, CultureInfo.CurrentCulture, key);
    public JObject Add( in ISpanFormattable value, in int bufferSize, in                                  CultureInfo culture, [CallerArgumentExpression("value")] string key = "" ) => Add(value, bufferSize, default,                    culture, key);

    public JObject Add( in ISpanFormattable value, in int bufferSize, in ReadOnlySpan<char> format, in CultureInfo culture, [CallerArgumentExpression("value")] string key = "" )
    {
        Start(key).Append('"').Append(value, format, culture, bufferSize).Append('"').Next();
        return this;
    }


    public JObject Add<T>( in T? value, in int bufferSize, [CallerArgumentExpression("value")] string key = "" ) where T : struct, ISpanFormattable => Add(value, bufferSize, CultureInfo.CurrentCulture, key);
    public JObject Add<T>( in T? value, in int bufferSize, in CultureInfo culture, [CallerArgumentExpression("value")] string key = "" ) where T : struct, ISpanFormattable => Add(value, bufferSize, default, culture, key);

    public JObject Add<T>( in T? value, in int bufferSize, in ReadOnlySpan<char> format, in CultureInfo culture, [CallerArgumentExpression("value")] string key = "" ) where T : struct, ISpanFormattable
    {
        Start(key).Append('"').Append(value, format, culture, bufferSize).Append('"').Next();
        return this;
    }


    public JObject Add( in KeyValuePair<string, string>  pair ) => Add(pair.Value, pair.Key);
    public JObject Add( in KeyValuePair<string, char>    pair ) => Add(pair.Value, pair.Key);
    public JObject Add( in KeyValuePair<string, short>   pair ) => Add(pair.Value, pair.Key);
    public JObject Add( in KeyValuePair<string, ushort>  pair ) => Add(pair.Value, pair.Key);
    public JObject Add( in KeyValuePair<string, int>     pair ) => Add(pair.Value, pair.Key);
    public JObject Add( in KeyValuePair<string, uint>    pair ) => Add(pair.Value, pair.Key);
    public JObject Add( in KeyValuePair<string, long>    pair ) => Add(pair.Value, pair.Key);
    public JObject Add( in KeyValuePair<string, ulong>   pair ) => Add(pair.Value, pair.Key);
    public JObject Add( in KeyValuePair<string, float>   pair ) => Add(pair.Value, pair.Key);
    public JObject Add( in KeyValuePair<string, double>  pair ) => Add(pair.Value, pair.Key);
    public JObject Add( in KeyValuePair<string, decimal> pair ) => Add(pair.Value, pair.Key);


    public JObject Add( in KeyValuePair<string, DateOnly>         pair ) => Add(pair.Value,                           100,        pair.Key);
    public JObject Add( in KeyValuePair<string, TimeOnly>         pair ) => Add(pair.Value,                           100,        pair.Key);
    public JObject Add( in KeyValuePair<string, TimeSpan>         pair ) => Add(pair.Value,                           100,        pair.Key);
    public JObject Add( in KeyValuePair<string, DateTimeOffset>   pair ) => Add(pair.Value,                           100,        pair.Key);
    public JObject Add( in KeyValuePair<string, DateTime>         pair ) => Add(pair.Value,                           100,        pair.Key);
    public JObject Add( in KeyValuePair<string, ISpanFormattable> pair, in int bufferSize = 1000 ) => Add(pair.Value, bufferSize, pair.Key);


    public JObject Add( in DictionaryEntry pair, in int bufferSize = 1000 )
    {
        var k = pair.Key.ToString();
        ArgumentNullException.ThrowIfNull(k);

        return pair.Value switch
               {
                   null               => Null(k),
                   string v           => Add(k.AsSpan(), v.AsSpan()),
                   char v             => Add(v),
                   short v            => Add(v,                               10,         k),
                   ushort v           => Add(v,                               10,         k),
                   int v              => Add(v,                               20,         k),
                   uint v             => Add(v,                               20,         k),
                   long v             => Add(v,                               30,         k),
                   ulong v            => Add(v,                               30,         k),
                   float v            => Add(v,                               350,        k),
                   double v           => Add(v,                               650,        k),
                   decimal v          => Add(v,                               300,        k),
                   DateOnly v         => Add(v,                               100,        k),
                   TimeOnly v         => Add(v,                               100,        k),
                   TimeSpan v         => Add(v,                               100,        k),
                   DateTimeOffset v   => Add(v,                               100,        k),
                   DateTime v         => Add(v,                               100,        k),
                   ISpanFormattable v => Add(v,                               bufferSize, k),
                   _                  => Add(pair.Value.ToString() ?? "null", k)
               };
    }


    public JArray AddArray( in ReadOnlySpan<char> key ) => Start(key).AddArray();
    public JObject AddArray( in IEnumerable<IJsonizer>? value, [CallerArgumentExpression("value")] string key = "" )
    {
        AddArray(key).AddObjects(value);
        return this;
    }


    public JObject AddObject( in ReadOnlySpan<char> key ) => Start(key).AddObject();
    public JObject AddObject( in IDictionary value, [CallerArgumentExpression("value")] string key = "" )
    {
        JObject node = AddObject(key);

        if ( value.Count == 0 )
        {
            node.Empty();
            return this;
        }


        node.Begin();

        foreach ( DictionaryEntry pair in value )
        {
            node.Add(pair);
            node.writer.Next();
        }

        node.Complete();
        return this;
    }
    public JObject AddObject( in IDictionary<string, IJsonizer> value, [CallerArgumentExpression("value")] string key = "" )
    {
        JObject node = AddObject(key);

        if ( value.Count == 0 )
        {
            node.Empty();
            return this;
        }


        node.Begin();

        foreach ( ( int index, string? k, IJsonizer? jsonizer ) in value.Enumerate(0) )
        {
            JObject item = node.AddObject(k);
            jsonizer.Serialize(ref item);
            item.writer.Next();

            if ( index < value.Count ) { node.writer.Next(); }
        }

        node.Complete();
        return this;
    }
    public JObject AddObject( in ICollection<KeyValuePair<string, IJsonizer>> value, [CallerArgumentExpression("value")] string key = "" )
    {
        JObject node = AddObject(key);

        if ( value.Count == 0 )
        {
            node.Empty();
            return this;
        }


        node.Begin();

        foreach ( ( int index, ( string? k, IJsonizer? jsonizer ) ) in value.Enumerate(0) )
        {
            JObject item = node.AddObject(k);
            jsonizer.Serialize(ref item);
            item.writer.Next();

            if ( index < value.Count ) { node.writer.Next(); }
        }

        node.Complete();
        return this;
    }
}
