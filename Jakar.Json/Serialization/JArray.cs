// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:56 AM

#nullable enable
namespace Jakar.Json.Serialization;


public ref struct JArray
{
    public const     char    START = '[';
    public const     char    END   = ']';
    private readonly bool    _shouldIndent;
    internal         JWriter writer;


    public JArray( JWriter writer, bool shouldIndent )
    {
        this.writer   = writer;
        _shouldIndent = shouldIndent;
        Begin();
    }
    public readonly JArray Empty()
    {
        writer.Append( START )
              .Append( ' ' )
              .Append( END )
              .FinishBlock();

        return this;
    }
    public JArray Begin()
    {
        writer.StartBlock( START, _shouldIndent );
        return this;
    }
    public JArray Complete()
    {
        writer.FinishBlock( END );
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
    public readonly JArray Add() => Add( JWriter.NULL );
    public readonly JArray Add( ReadOnlySpan<char> value )
    {
        writer.Append( '"' )
              .Append( value )
              .Append( '"' )
              .Next();

        return this;
    }


    public readonly JArray Add<T>( T? value ) where T : ISpanFormattable => Add( value,                           CultureInfo.CurrentCulture );
    public readonly JArray Add<T>( T? value, IFormatProvider? culture ) where T : ISpanFormattable => Add( value, default, culture );
    public readonly JArray Add<T>( T? value, ReadOnlySpan<char> format, IFormatProvider? provider = default ) where T : ISpanFormattable
    {
        writer.Append( value, format, provider );
        return this;
    }
    public readonly JArray Add<T>( T? value, int bufferSize, ReadOnlySpan<char> format, IFormatProvider? provider = default ) where T : struct, ISpanFormattable
    {
        writer.Append( value, format, bufferSize, provider )
              .Next();

        return this;
    }


    public readonly JArray AddArray() => new(writer, true);
    public readonly JObject AddObject() => new(writer, true);


    public JArray AddObjects( IReadOnlyCollection<IJsonizer>? collection )
    {
        if ( collection is null || collection.Count == 0 ) { return Empty(); }


        Begin();

        foreach ( (int index, IJsonizer item) in collection.Enumerate( 0 ) )
        {
            JObject node = AddObject();
            item.Serialize( ref node );

            if ( index < collection.Count ) { node.writer.Next(); }
        }

        return Complete();
    }
}
