// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:56 AM

using System.Numerics;



namespace Jakar.Json.Serialization;


public ref struct JArray
{
    public const char    START = '[';
    public const char    END   = ']';
    private      JWriter _writer;


    public JArray( JWriter writer )
    {
        _writer = writer;
        Begin();
    }
    public readonly JArray Empty()
    {
        _writer.Append( START ).Append( JWriter.SPACE ).Append( END ).FinishBlock();

        return this;
    }
    public JArray Begin()
    {
        _writer.StartBlock( START );
        return this;
    }
    public JArray Complete()
    {
        _writer.FinishBlock( END );
        return this;
    }


    // public readonly JArray Add( char value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JArray Add( short value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JArray Add( ushort value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JArray Add( int value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JArray Add( uint value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JArray Add( long value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JArray Add( ulong value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JArray Add( float value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JArray Add( double value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    // public readonly JArray Add( decimal value )
    // {
    //     writer.Indent()
    //           .Append( value )
    //           .Next();
    //
    //     return this;
    // }
    //
    //
    // public readonly JArray Add( char? value ) => value.HasValue
    //                                                  ? Add( value.Value )
    //                                                  : Add();
    // public readonly JArray Add( short? value ) => value.HasValue
    //                                                   ? Add( value.Value )
    //                                                   : Add();
    // public readonly JArray Add( ushort? value ) => value.HasValue
    //                                                    ? Add( value.Value )
    //                                                    : Add();
    // public readonly JArray Add( int? value ) => value.HasValue
    //                                                 ? Add( value.Value )
    //                                                 : Add();
    // public readonly JArray Add( uint? value ) => value.HasValue
    //                                                  ? Add( value.Value )
    //                                                  : Add();
    // public readonly JArray Add( long? value ) => value.HasValue
    //                                                  ? Add( value.Value )
    //                                                  : Add();
    // public readonly JArray Add( ulong? value ) => value.HasValue
    //                                                   ? Add( value.Value )
    //                                                   : Add();
    // public readonly JArray Add( float? value ) => value.HasValue
    //                                                   ? Add( value.Value )
    //                                                   : Add();
    // public readonly JArray Add( double? value ) => value.HasValue
    //                                                    ? Add( value.Value )
    //                                                    : Add();
    // public readonly JArray Add( decimal? value ) => value.HasValue
    //                                                     ? Add( value.Value )
    //                                                     : Add();


    public readonly JArray Add( string value ) => Add( value.AsSpan() );
    public readonly JArray Add()               => Add( JWriter.NULL );
    public readonly JArray Add( ReadOnlySpan<char> value )
    {
        _writer.Append( JWriter.QUOTE ).Append( value ).Append( JWriter.QUOTE ).Next();

        return this;
    }


    public readonly JArray Add<TValue>( TValue? value )
        where TValue : ISpanFormattable => Add( value, CultureInfo.CurrentCulture );
    public readonly JArray Add<TValue>( TValue? value, IFormatProvider? culture )
        where TValue : ISpanFormattable => Add( value, default, culture );
    public readonly JArray Add<TValue>( TValue? value, ReadOnlySpan<char> format, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        _writer.Append( value, format, provider );
        return this;
    }
    public readonly JArray Add<TValue>( TValue? value, int bufferSize, ReadOnlySpan<char> format, IFormatProvider? provider = null )
        where TValue : ISpanFormattable
    {
        if ( value is not null ) { _writer.Append( value, format, bufferSize, provider ).Next(); }

        return this;
    }


    public readonly JArray AddNumber<TValue>( TValue? value )
        where TValue : struct, INumber<TValue>, ISpanFormattable => AddNumber( value, CultureInfo.CurrentCulture );
    public readonly JArray AddNumber<TValue>( TValue? value, IFormatProvider? culture )
        where TValue : struct, INumber<TValue>, ISpanFormattable => AddNumber( value, default, culture );
    public readonly JArray AddNumber<TValue>( TValue? value, ReadOnlySpan<char> format, IFormatProvider? provider = null )
        where TValue : struct, INumber<TValue>, ISpanFormattable
    {
        if ( value is null ) { return this; }

        return AddNumber( value.Value, format, provider );
    }
    public readonly JArray AddNumber<TValue>( TValue? value, int bufferSize, ReadOnlySpan<char> format, IFormatProvider? provider = null )
        where TValue : struct, INumber<TValue>, ISpanFormattable
    {
        if ( value is null ) { return this; }

        return AddNumber( value.Value, bufferSize, format, provider );
    }


    public readonly JArray AddNumber<TValue>( TValue value )
        where TValue : struct, INumber<TValue>, ISpanFormattable => AddNumber( value, CultureInfo.CurrentCulture );
    public readonly JArray AddNumber<TValue>( TValue value, IFormatProvider? culture )
        where TValue : struct, INumber<TValue>, ISpanFormattable => AddNumber( value, default, culture );
    public readonly JArray AddNumber<TValue>( TValue value, ReadOnlySpan<char> format, IFormatProvider? provider = null )
        where TValue : struct, INumber<TValue>, ISpanFormattable
    {
        _writer.Append( value, format, provider );
        return this;
    }
    public readonly JArray AddNumber<TValue>( TValue value, int bufferSize, ReadOnlySpan<char> format, IFormatProvider? provider = null )
        where TValue : struct, INumber<TValue>, ISpanFormattable
    {
        _writer.AppendValue( value, format, bufferSize, provider ).Next();

        return this;
    }


    public JArray  AddArray()  => new(_writer);
    public JObject AddObject() => new(_writer);


    public JArray AddObjects( IReadOnlyCollection<IJsonizer>? collection )
    {
        if ( collection is null || collection.Count == 0 ) { return Empty(); }

        Begin();

        foreach ( (int index, IJsonizer item) in collection.Enumerate( 0 ) )
        {
            JObject node = AddObject();
            item.Serialize( ref node );

            if ( index < collection.Count ) { _writer.Next(); }
        }

        return Complete();
    }
}
