// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:56 AM

using System.Numerics;



namespace Jakar.Json.Serialization;


public ref struct JsonArray
{
    public const char    START = '[';
    public const char    END   = ']';
    private      JWriter __writer;


    public JsonArray( JWriter writer )
    {
        __writer = writer;
        Begin();
    }
    public readonly JsonArray Empty()
    {
        __writer.Append(START)
                .Append(JWriter.SPACE)
                .Append(END)
                .FinishBlock();

        return this;
    }
    public JsonArray Begin()
    {
        __writer.StartBlock(START);
        return this;
    }
    public JsonArray Complete()
    {
        __writer.FinishBlock(END);
        return this;
    }


    // public readonly JsonArray Add( char value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JsonArray Add( short value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JsonArray Add( ushort value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JsonArray Add( int value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JsonArray Add( uint value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JsonArray Add( long value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JsonArray Add( ulong value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JsonArray Add( float value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JsonArray Add( double value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JsonArray Add( decimal value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    //
    //
    // public readonly JsonArray Add( char? value ) => value.HasValue
    //                                                  ? Add( value.Value )
    //                                                  : Add();
    // public readonly JsonArray Add( short? value ) => value.HasValue
    //                                                   ? Add( value.Value )
    //                                                   : Add();
    // public readonly JsonArray Add( ushort? value ) => value.HasValue
    //                                                    ? Add( value.Value )
    //                                                    : Add();
    // public readonly JsonArray Add( int? value ) => value.HasValue
    //                                                 ? Add( value.Value )
    //                                                 : Add();
    // public readonly JsonArray Add( uint? value ) => value.HasValue
    //                                                  ? Add( value.Value )
    //                                                  : Add();
    // public readonly JsonArray Add( long? value ) => value.HasValue
    //                                                  ? Add( value.Value )
    //                                                  : Add();
    // public readonly JsonArray Add( ulong? value ) => value.HasValue
    //                                                   ? Add( value.Value )
    //                                                   : Add();
    // public readonly JsonArray Add( float? value ) => value.HasValue
    //                                                   ? Add( value.Value )
    //                                                   : Add();
    // public readonly JsonArray Add( double? value ) => value.HasValue
    //                                                    ? Add( value.Value )
    //                                                    : Add();
    // public readonly JsonArray Add( decimal? value ) => value.HasValue
    //                                                     ? Add( value.Value )
    //                                                     : Add();


    public readonly JsonArray Add( string value ) => Add(value.AsSpan());
    public readonly JsonArray Add()               => Add(JWriter.NULL);
    public readonly JsonArray Add( ReadOnlySpan<char> value )
    {
        __writer.Append(JWriter.QUOTE)
                .Append(value)
                .Append(JWriter.QUOTE)
                .Next();

        return this;
    }


    public readonly JsonArray Add<TValue>( TValue? value )
        where TValue : ISpanFormattable => Add(value, CultureInfo.CurrentCulture);
    public readonly JsonArray Add<TValue>( TValue? value, IFormatProvider? culture )
        where TValue : ISpanFormattable => Add(value, default, culture);
    public readonly JsonArray Add<TValue>( TValue? value, ReadOnlySpan<char> format, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        __writer.Append(value, format, provider);
        return this;
    }
    public readonly JsonArray Add<TValue>( TValue? value, int bufferSize, ReadOnlySpan<char> format, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        if ( value is not null )
        {
            __writer.Append(value, format, bufferSize, provider)
                    .Next();
        }

        return this;
    }


    public readonly JsonArray AddNumber<TValue>( TValue? value )
        where TValue : struct, INumber<TValue>, ISpanFormattable => AddNumber(value, CultureInfo.CurrentCulture);
    public readonly JsonArray AddNumber<TValue>( TValue? value, IFormatProvider? culture )
        where TValue : struct, INumber<TValue>, ISpanFormattable => AddNumber(value, default, culture);
    public readonly JsonArray AddNumber<TValue>( TValue? value, ReadOnlySpan<char> format, IFormatProvider? provider = null )
        where TValue : struct, INumber<TValue>, ISpanFormattable
    {
        if ( value is null ) { return this; }

        return AddNumber(value.Value, format, provider);
    }
    public readonly JsonArray AddNumber<TValue>( TValue? value, int bufferSize, ReadOnlySpan<char> format, IFormatProvider? provider = null )
        where TValue : struct, INumber<TValue>, ISpanFormattable
    {
        if ( value is null ) { return this; }

        return AddNumber(value.Value, bufferSize, format, provider);
    }


    public readonly JsonArray AddNumber<TValue>( TValue value )
        where TValue : struct, INumber<TValue>, ISpanFormattable => AddNumber(value, CultureInfo.CurrentCulture);
    public readonly JsonArray AddNumber<TValue>( TValue value, IFormatProvider? culture )
        where TValue : struct, INumber<TValue>, ISpanFormattable => AddNumber(value, default, culture);
    public readonly JsonArray AddNumber<TValue>( TValue value, ReadOnlySpan<char> format, IFormatProvider? provider = null )
        where TValue : struct, INumber<TValue>, ISpanFormattable
    {
        __writer.Append(value, format, provider);
        return this;
    }
    public readonly JsonArray AddNumber<TValue>( TValue value, int bufferSize, ReadOnlySpan<char> format, IFormatProvider? provider = null )
        where TValue : struct, INumber<TValue>, ISpanFormattable
    {
        __writer.AppendValue(value, format, bufferSize, provider)
                .Next();

        return this;
    }


    public JsonArray  AddArray()  => new(__writer);
    public JsonObject AddObject() => new(__writer);


    public JsonArray AddObjects( IReadOnlyCollection<IJsonizer>? collection )
    {
        if ( collection is null || collection.Count == 0 ) { return Empty(); }

        Begin();

        foreach ( ( int index, IJsonizer item ) in collection.Enumerate(0) )
        {
            JsonObject node = AddObject();
            item.Serialize(ref node);

            if ( index < collection.Count ) { __writer.Next(); }
        }

        return Complete();
    }
}
