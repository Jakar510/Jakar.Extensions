// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:59 AM

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;



namespace Jakar.Xml.Serialization;


[SuppressMessage("ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable")]
public readonly ref struct XObject
{
    private readonly ReadOnlySpan<char> _name;
    private readonly XContext           _context;
    private readonly StringBuilder      _sb;


    public XObject( in ReadOnlySpan<char> name, in XContext context, in StringBuilder sb )
    {
        _name    = name;
        _context = context;
        _sb      = sb;
        _sb.Append('<');
        _sb.Append(_name);
        _sb.Append('>');
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


    public XObject Add( in ReadOnlySpan<char> key, in char value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in char? value )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in short value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in short? value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in ushort value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in ushort? value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in int value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in int? value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in uint value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in uint? value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in long? value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in long value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in ulong value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in ulong? value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in float? value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in float value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in double value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in double? value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in decimal value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in decimal? value, in ReadOnlySpan<char> format = default, in CultureInfo? culture = default )
    {
        _sb.Append(key);
        _sb.Append('=');
        _sb.Append(value);
        _sb.Append(',');
        return this;
    }
    public XObject Add( in ReadOnlySpan<char> key, in string value ) => Add(key, value.AsSpan());
    public XObject Add( in ReadOnlySpan<char> key, in ReadOnlySpan<char> value )
    {
        AddBlock(key, value);
        return this;
    }


    public XObject Add<T>( in ReadOnlySpan<char> key, in T value ) where T : struct, ISpanFormattable => Add(key,                                                       value, CultureInfo.CurrentCulture);
    public XObject Add<T>( in ReadOnlySpan<char> key, in T value, in CultureInfo        culture ) where T : struct, ISpanFormattable => Add(key,                        value, default, culture);
    public XObject Add<T>( in ReadOnlySpan<char> key, in T value, in ReadOnlySpan<char> format, in CultureInfo culture ) where T : struct, ISpanFormattable => Add(key, value, format,  culture, 1000);
    public XObject Add<T>( in ReadOnlySpan<char> key, in T value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : struct, ISpanFormattable
    {
        Span<char> buffer = stackalloc char[bufferSize];

        if ( !value.TryFormat(buffer, out int charsWritten, format, culture) ) { throw new InvalidOperationException($"Can't format value: '{value}'"); }

        AddBlock(key, buffer[..charsWritten]);
        return this;
    }


    public XObject Add<T>( in ReadOnlySpan<char> key, in T? value ) where T : struct, ISpanFormattable => Add(key,                                                       value, CultureInfo.CurrentCulture);
    public XObject Add<T>( in ReadOnlySpan<char> key, in T? value, in CultureInfo        culture ) where T : struct, ISpanFormattable => Add(key,                        value, default, culture);
    public XObject Add<T>( in ReadOnlySpan<char> key, in T? value, in ReadOnlySpan<char> format, in CultureInfo culture ) where T : struct, ISpanFormattable => Add(key, value, format,  culture, 650);
    public XObject Add<T>( in ReadOnlySpan<char> key, in T? value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : struct, ISpanFormattable
    {
        if ( value.HasValue )
        {
            Span<char> buffer = stackalloc char[bufferSize];

            if ( !value.Value.TryFormat(buffer, out int charsWritten, format, culture) ) { throw new InvalidOperationException($"Can't format value: '{value}'"); }

            AddBlock(key, buffer[..charsWritten]);
        }
        else { AddBlock(key, default); }

        return this;
    }


    public XArray AddArray( in ReadOnlySpan<char> key )
    {
        _sb.Append(key);
        _sb.Append('=');
        return new XArray(_context, _sb);
    }
    public XObject AddObject( in ReadOnlySpan<char> key )
    {
        _sb.Append(key);
        _sb.Append('=');
        return new XObject(_context, _sb);
    }


    private void NewLine() => _sb.Append('\n');
    public void Dispose()
    {
        _sb.Append("</");
        _sb.Append(_name);
        _sb.Append('>');
    }
}
