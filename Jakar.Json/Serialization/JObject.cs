// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:59 AM

using System.Collections;
using System.Globalization;
using System.Text;



namespace Jakar.Json.Serialization;


public readonly ref struct JObject
{
    private readonly JContext      _context;
    private readonly StringBuilder _sb;


    public JObject( in JContext context, in StringBuilder sb )
    {
        _context = context;
        _sb      = sb;
        _sb.Append('{');
    }


    /// <summary>
    /// Adds "null" value
    /// </summary>
    public JObject Add( in ReadOnlySpan<char> key )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append("null");
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in char value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in char? value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in short value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in short? value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in ushort value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in ushort? value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in int value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in int? value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in uint value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in uint? value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in long? value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in long value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in ulong value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in ulong? value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in float? value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in float value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in double value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in double? value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in decimal value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in decimal? value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in string value ) => Add(key, value.AsSpan());
    public JObject Add( in ReadOnlySpan<char> key, in ReadOnlySpan<char> value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append('"');
        _sb.Append(value);
        _sb.Append('"');
        _sb.Append(',');
        return this;
    }
    public JObject Add( in ReadOnlySpan<char> key, in object? value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }


    public JObject Add<T>( in ReadOnlySpan<char> key, in T value ) where T : ISpanFormattable => Add(key,                                                       value, CultureInfo.CurrentCulture);
    public JObject Add<T>( in ReadOnlySpan<char> key, in T value, in CultureInfo        culture ) where T : ISpanFormattable => Add(key,                        value, default, culture);
    public JObject Add<T>( in ReadOnlySpan<char> key, in T value, in ReadOnlySpan<char> format, in CultureInfo culture ) where T : ISpanFormattable => Add(key, value, format,  culture, 1000);
    public JObject Add<T>( in ReadOnlySpan<char> key, in T value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : ISpanFormattable
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append('"');
        Span<char> buffer = stackalloc char[bufferSize];

        if ( !value.TryFormat(buffer, out int charsWritten, format, culture) ) { throw new InvalidOperationException($"Can't format value: '{value}'"); }

        _sb.Append(buffer[..charsWritten]);
        _sb.Append('"');
        _sb.Append(',');
        return this;
    }


    public JObject Add<T>( in ReadOnlySpan<char> key, in T? value ) where T : struct, ISpanFormattable => Add(key,                                                       value, CultureInfo.CurrentCulture);
    public JObject Add<T>( in ReadOnlySpan<char> key, in T? value, in CultureInfo        culture ) where T : struct, ISpanFormattable => Add(key,                        value, default, culture);
    public JObject Add<T>( in ReadOnlySpan<char> key, in T? value, in ReadOnlySpan<char> format, in CultureInfo culture ) where T : struct, ISpanFormattable => Add(key, value, format,  culture, 650);
    public JObject Add<T>( in ReadOnlySpan<char> key, in T? value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : struct, ISpanFormattable
    {
        _sb.Append(key);
        _sb.Append('=');

        if ( value.HasValue )
        {
            _sb.Append('"');
            Span<char> buffer = stackalloc char[bufferSize];

            if ( !value.Value.TryFormat(buffer, out int charsWritten, format, culture) ) { throw new InvalidOperationException($"Can't format value: '{value}'"); }

            _sb.Append(buffer[..charsWritten]);
            _sb.Append('"');
        }
        else { _sb.Append("null"); }

        _sb.Append(',');
        return this;
    }


    public JObject Add( in KeyValuePair<string, string>           pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, char>             pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, short>            pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, ushort>           pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, int>              pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, uint>             pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, long>             pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, ulong>            pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, float>            pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, double>           pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, decimal>          pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in KeyValuePair<string, ISpanFormattable> pair ) => Add(pair.Key, pair.Value);
    public JObject Add( in DictionaryEntry pair )
    {
        ReadOnlySpan<char> k = pair.Key.ToString();

        return pair.Value switch
               {
                   null               => Add(k),
                   string v           => Add(k, v),
                   char v             => Add(k, v),
                   short v            => Add(k, v),
                   ushort v           => Add(k, v),
                   int v              => Add(k, v),
                   uint v             => Add(k, v),
                   long v             => Add(k, v),
                   ulong v            => Add(k, v),
                   float v            => Add(k, v),
                   double v           => Add(k, v),
                   decimal v          => Add(k, v),
                   DateOnly v         => Add(k, v),
                   TimeOnly v         => Add(k, v),
                   TimeSpan v         => Add(k, v),
                   DateTimeOffset v   => Add(k, v),
                   DateTime v         => Add(k, v),
                   ISpanFormattable v => Add(k, v),
                   _                  => Add(k, pair.Value)
               };
    }


    public JArray AddArray( in ReadOnlySpan<char> key )
    {
        _sb.Append(key);
        _sb.Append('=');
        return new JArray(_context, _sb);
    }
    public JObject AddObject( in ReadOnlySpan<char> key )
    {
        _sb.Append(key);
        _sb.Append('=');
        return new JObject(_context, _sb);
    }


    private void NewLine() => _sb.Append('\n');
    public void Dispose()
    {
        _sb.Append('}');
        _sb.Append(',');
    }
}
