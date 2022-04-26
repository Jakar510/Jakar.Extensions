// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:56 AM

using System.Globalization;
using System.Text;



namespace Jakar.Json.Serialization;


public readonly ref struct JArray
{
    private readonly JContext      _context;
    private readonly StringBuilder _sb;


    public JArray( in JContext context, in StringBuilder sb )
    {
        _context = context;
        _sb      = sb;
        _sb.Append('[');
    }


    public JArray Add( in char value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in char? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in short value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in short? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in ushort value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in ushort? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in int value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in int? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in uint value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in uint? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in long value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in long? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in ulong value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in ulong? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in float value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in float? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in double value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in double? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in decimal value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in decimal? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public JArray Add( in string value ) => Add(value.AsSpan());
    public JArray Add( in ReadOnlySpan<char> value )
    {
        _sb.Append('"');
        _sb.Append(value);
        _sb.Append('"');
        _sb.Append(',');
        return this;
    }


    public JArray Add<T>( in T value ) where T : struct, ISpanFormattable => Add(value,                                                       CultureInfo.CurrentCulture);
    public JArray Add<T>( in T value, in CultureInfo        culture ) where T : struct, ISpanFormattable => Add(value,                        default, culture);
    public JArray Add<T>( in T value, in ReadOnlySpan<char> format, in CultureInfo culture ) where T : struct, ISpanFormattable => Add(value, format,  culture, 650);
    public JArray Add<T>( in T value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : struct, ISpanFormattable
    {
        Span<char> buffer = stackalloc char[bufferSize];

        if ( !value.TryFormat(buffer, out int charsWritten, format, culture) ) { throw new InvalidOperationException($"Can't format value: '{value}'"); }

        _sb.Append(buffer[..charsWritten]);
        _sb.Append(',');
        return this;
    }


    public JArray Add<T>( in T? value ) where T : struct, ISpanFormattable => Add(value,                                                       CultureInfo.CurrentCulture);
    public JArray Add<T>( in T? value, in CultureInfo        culture ) where T : struct, ISpanFormattable => Add(value,                        default, culture);
    public JArray Add<T>( in T? value, in ReadOnlySpan<char> format, in CultureInfo culture ) where T : struct, ISpanFormattable => Add(value, format,  culture, 650);
    public JArray Add<T>( in T? value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : struct, ISpanFormattable
    {
        if ( value.HasValue )
        {
            Span<char> buffer = stackalloc char[bufferSize];

            if ( !value.Value.TryFormat(buffer, out int charsWritten, format, culture) ) { throw new InvalidOperationException($"Can't format value: '{value}'"); }

            _sb.Append(buffer[..charsWritten]);
        }
        else { _sb.Append("null"); }

        _sb.Append(',');
        return this;
    }


    public JArray AddArray() => new(_context, _sb);
    public JObject AddObject() => new(_context, _sb);


    private void NewLine() => _sb.Append('\n');
    public void Dispose()
    {
        _sb.Append(']');
        _sb.Append(',');
    }
}
