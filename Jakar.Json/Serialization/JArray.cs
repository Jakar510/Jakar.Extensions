// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:56 AM

using System.Globalization;
using Jakar.Extensions.Collections;



namespace Jakar.Json.Serialization;


public ref struct JArray
{
    public const char START = '[';
    public const char END   = ']';

    private readonly bool    _shouldIndent;
    internal         JWriter writer;


    public JArray( ref JWriter writer, in bool shouldIndent )
    {
        this.writer   = writer;
        _shouldIndent = shouldIndent;
    }
    public JArray Empty()
    {
        writer.Append(START).Append(' ').Append(END).FinishBlock();
        return this;
    }
    public JArray Begin()
    {
        writer.StartBlock(START, _shouldIndent);
        return this;
    }
    public JArray Complete()
    {
        writer.FinishBlock(END);
        return this;
    }


    public JArray Add( in char value )
    {
        writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in short value )
    {
        writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in ushort value )
    {
        writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in int value )
    {
        writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in uint value )
    {
        writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in long value )
    {
        writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in ulong value )
    {
        writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in float value )
    {
        writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in double value )
    {
        writer.Indent().Append(value).Next();
        return this;
    }
    public JArray Add( in decimal value )
    {
        writer.Indent().Append(value).Next();
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
        writer.Append('"').Append(value).Append('"').Next();
        return this;
    }


    public JArray Add<T>( in T value ) where T : struct, ISpanFormattable => Add(value,                                                       CultureInfo.CurrentCulture);
    public JArray Add<T>( in T value, in CultureInfo        culture ) where T : struct, ISpanFormattable => Add(value,                        default, culture);
    public JArray Add<T>( in T value, in ReadOnlySpan<char> format, in CultureInfo culture ) where T : struct, ISpanFormattable => Add(value, format,  culture, 650);
    public JArray Add<T>( in T value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : struct, ISpanFormattable
    {
        writer.Append(value, format, culture, bufferSize).Next();
        return this;
    }


    public JArray Add<T>( in T? value ) where T : struct, ISpanFormattable => Add(value,                                                       CultureInfo.CurrentCulture);
    public JArray Add<T>( in T? value, in CultureInfo        culture ) where T : struct, ISpanFormattable => Add(value,                        default, culture);
    public JArray Add<T>( in T? value, in ReadOnlySpan<char> format, in CultureInfo culture ) where T : struct, ISpanFormattable => Add(value, format,  culture, 650);
    public JArray Add<T>( in T? value, in ReadOnlySpan<char> format, in CultureInfo culture, in int bufferSize ) where T : struct, ISpanFormattable
    {
        writer.Append(value, format, culture, bufferSize).Next();
        return this;
    }


    public JArray AddArray() => new(ref writer, true);
    public JObject AddObject() => new(ref writer, true);


    public JArray AddObjects( in IEnumerable<IJsonizer>? value )
    {
        if ( value is null ) { return Empty(); }

        return AddObjects(new List<IJsonizer>(value));
    }
    public JArray AddObjects( in ICollection<IJsonizer> collection )
    {
        if ( collection.Count == 0 ) { return Empty(); }


        Begin();

        foreach ( ( int index, IJsonizer item ) in collection.Enumerate() )
        {
            JObject node = AddObject();
            item.Serialize(ref node);

            if ( index < collection.Count ) { node.writer.Next(); }
        }

        return Complete();
    }
}
