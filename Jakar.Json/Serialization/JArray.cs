// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:56 AM

using System.Globalization;



namespace Jakar.Json.Serialization;


public ref struct JArray
{
    private JWriter _writer;


    public JArray( ref JWriter writer )
    {
        _writer = writer;
        _writer.StartBlock('[');
    }


    public JArray Add( in char value )
    {
        _writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in short value )
    {
        _writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in ushort value )
    {
        _writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in int value )
    {
        _writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in uint value )
    {
        _writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in long value )
    {
        _writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in ulong value )
    {
        _writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in float value )
    {
        _writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in double value )
    {
        _writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in decimal value )
    {
        _writer.Indent().Append(value).Next();
        return this;
    }


    public JArray Add( in char? value ) => value.HasValue
                                               ? Add(value.Value)
                                               : Add();
    public JArray Add( in short? value ) => value.HasValue
                                                ? Add(value.Value)
                                                : Add();
    public JArray Add( in ushort? value ) => value.HasValue
                                                 ? Add(value.Value)
                                                 : Add();
    public JArray Add( in int? value ) => value.HasValue
                                              ? Add(value.Value)
                                              : Add();
    public JArray Add( in uint? value ) => value.HasValue
                                               ? Add(value.Value)
                                               : Add();
    public JArray Add( in long? value ) => value.HasValue
                                               ? Add(value.Value)
                                               : Add();
    public JArray Add( in ulong? value ) => value.HasValue
                                                ? Add(value.Value)
                                                : Add();
    public JArray Add( in float? value ) => value.HasValue
                                                ? Add(value.Value)
                                                : Add();
    public JArray Add( in double? value ) => value.HasValue
                                                 ? Add(value.Value)
                                                 : Add();
    public JArray Add( in decimal? value ) => value.HasValue
                                                  ? Add(value.Value)
                                                  : Add();


    public JArray Add( in string value ) => Add(value.AsSpan());
    public JArray Add() => Add(JWriter.NULL);
    public JArray Add( in ReadOnlySpan<char> value )
    {
        _writer.Append('"').Append(value).Append('"').Next();
        return this;
    }


    public JArray Add<T>( in T value ) where T : struct, ISpanFormattable => Add(value,                                                       CultureInfo.CurrentCulture);
    public JArray Add<T>( in T value, in CultureInfo        culture ) where T : struct, ISpanFormattable => Add(value,                        default, culture);
    public JArray Add<T>( in T value, in ReadOnlySpan<char> format, in CultureInfo culture ) where T : struct, ISpanFormattable => Add(value, format,  culture, 650);
    public JArray Add<T>( in T value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : struct, ISpanFormattable
    {
        _writer.Append(value, format, culture, bufferSize).Next();
        return this;
    }


    public JArray Add<T>( in T? value ) where T : struct, ISpanFormattable => Add(value,                                                       CultureInfo.CurrentCulture);
    public JArray Add<T>( in T? value, in CultureInfo        culture ) where T : struct, ISpanFormattable => Add(value,                        default, culture);
    public JArray Add<T>( in T? value, in ReadOnlySpan<char> format, in CultureInfo culture ) where T : struct, ISpanFormattable => Add(value, format,  culture, 650);
    public JArray Add<T>( in T? value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : struct, ISpanFormattable
    {
        _writer.Append(value, format, culture, bufferSize).Next();
        return this;
    }


    public JArray AddArray() => new(ref _writer);
    public JObject AddObject() => new(ref _writer);


    public void Dispose() => _writer.FinishBlock(']');
}
