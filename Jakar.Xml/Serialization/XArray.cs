// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:56 AM

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;



namespace Jakar.Xml.Serialization;


[SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable")]
public readonly ref struct XArray
{
    private readonly ReadOnlySpan<char> _name;
    private readonly XContext           _context;
    private readonly StringBuilder      _sb;


    public XArray( in ReadOnlySpan<char> name, in XContext context, in StringBuilder sb )
    {
        _name    = name;
        _context = context;
        _sb      = sb;
        _sb.Append('[');
    }
    private void AddBlock( in ReadOnlySpan<char> name, in ReadOnlySpan<char> value )
    {
        _sb.Append('<');
        _sb.Append(name);
        _sb.Append('>');
        _sb.Append(value);
        _sb.Append("</");
        _sb.Append(name);
        _sb.Append('>');

        if ( _context.Indent(_sb) ) { NewLine(); }
    }


    public XArray Add( in char value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in char? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in short value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in short? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in ushort value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in ushort? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in int value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in int? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in uint value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in uint? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in long value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in long? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in ulong value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in ulong? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in float value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in float? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in double value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in double? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in decimal value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in decimal? value )
    {
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XArray Add( in string value ) => Add(value.AsSpan());
    public XArray Add( in ReadOnlySpan<char> value )
    {
        _sb.Append('"');
        _sb.Append(value);
        _sb.Append('"');
        _sb.Append(',');
        return this;
    }


    public XArray Add<T>( in T value ) where T : struct, ISpanFormattable => Add(value,                                                       CultureInfo.CurrentCulture);
    public XArray Add<T>( in T value, in CultureInfo        culture ) where T : struct, ISpanFormattable => Add(value,                        default, culture);
    public XArray Add<T>( in T value, in ReadOnlySpan<char> format, in CultureInfo culture ) where T : struct, ISpanFormattable => Add(value, format,  culture, 650);
    public XArray Add<T>( in T value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : struct, ISpanFormattable
    {
        Span<char> buffer = stackalloc char[bufferSize];

        if ( !value.TryFormat(buffer, out int charsWritten, format, culture) ) { throw new InvalidOperationException($"Can't format value: '{value}'"); }

        _sb.Append(buffer[..charsWritten]);
        _sb.Append(',');
        return this;
    }


    public XArray Add<T>( in T? value ) where T : struct, ISpanFormattable => Add(value,                                                       CultureInfo.CurrentCulture);
    public XArray Add<T>( in T? value, in CultureInfo        culture ) where T : struct, ISpanFormattable => Add(value,                        default, culture);
    public XArray Add<T>( in T? value, in ReadOnlySpan<char> format, in CultureInfo culture ) where T : struct, ISpanFormattable => Add(value, format,  culture, 650);
    public XArray Add<T>( in T? value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : struct, ISpanFormattable
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


    public XArray AddArray( in   ReadOnlySpan<char> name ) => new(name, _context, _sb);
    public XObject AddObject( in ReadOnlySpan<char> name ) => new(name, _context, _sb);


    private void NewLine() => _sb.Append('\n');
    public void Dispose()
    {
        _sb.Append(']');
        _sb.Append(',');
    }
}
