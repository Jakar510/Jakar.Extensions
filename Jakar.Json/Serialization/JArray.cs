// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:56 AM

#nullable enable
using System.Globalization;
using Jakar.Extensions;



namespace Jakar.Json.Serialization;


public ref struct JArray
{
    public const char START = '[';
    public const char END   = ']';

    private readonly bool    _shouldIndent;
    internal         JWriter writer;


    public JArray( JWriter writer, bool shouldIndent )
    {
        this.writer   = writer;
        _shouldIndent = shouldIndent;
    }
    public JArray Empty()
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


    public JArray Add( char value )
    {
        writer.Indent()
              .Append( value )
              .Next();

        return this;
    }
    public JArray Add( short value )
    {
        writer.Indent()
              .Append( value )
              .Next();

        return this;
    }
    public JArray Add( ushort value )
    {
        writer.Indent()
              .Append( value )
              .Next();

        return this;
    }
    public JArray Add( int value )
    {
        writer.Indent()
              .Append( value )
              .Next();

        return this;
    }
    public JArray Add( uint value )
    {
        writer.Indent()
              .Append( value )
              .Next();

        return this;
    }
    public JArray Add( long value )
    {
        writer.Indent()
              .Append( value )
              .Next();

        return this;
    }
    public JArray Add( ulong value )
    {
        writer.Indent()
              .Append( value )
              .Next();

        return this;
    }
    public JArray Add( float value )
    {
        writer.Indent()
              .Append( value )
              .Next();

        return this;
    }
    public JArray Add( double value )
    {
        writer.Indent()
              .Append( value )
              .Next();

        return this;
    }
    public JArray Add( decimal value )
    {
        writer.Indent()
              .Append( value )
              .Next();

        return this;
    }


    public JArray Add( char? value ) => value.HasValue
                                            ? Add( value.Value )
                                            : Add();
    public JArray Add( short? value ) => value.HasValue
                                             ? Add( value.Value )
                                             : Add();
    public JArray Add( ushort? value ) => value.HasValue
                                              ? Add( value.Value )
                                              : Add();
    public JArray Add( int? value ) => value.HasValue
                                           ? Add( value.Value )
                                           : Add();
    public JArray Add( uint? value ) => value.HasValue
                                            ? Add( value.Value )
                                            : Add();
    public JArray Add( long? value ) => value.HasValue
                                            ? Add( value.Value )
                                            : Add();
    public JArray Add( ulong? value ) => value.HasValue
                                             ? Add( value.Value )
                                             : Add();
    public JArray Add( float? value ) => value.HasValue
                                             ? Add( value.Value )
                                             : Add();
    public JArray Add( double? value ) => value.HasValue
                                              ? Add( value.Value )
                                              : Add();
    public JArray Add( decimal? value ) => value.HasValue
                                               ? Add( value.Value )
                                               : Add();


    public JArray Add( string value ) => Add( value.AsSpan() );
    public JArray Add() => Add( JWriter.NULL );
    public JArray Add( ReadOnlySpan<char> value )
    {
        writer.Append( '"' )
              .Append( value )
              .Append( '"' )
              .Next();

        return this;
    }


    public JArray Add<T>( T value ) where T : struct, ISpanFormattable => Add( value,                                                 CultureInfo.CurrentCulture );
    public JArray Add<T>( T value, CultureInfo        culture ) where T : struct, ISpanFormattable => Add( value,                     default, culture );
    public JArray Add<T>( T value, ReadOnlySpan<char> format, CultureInfo culture ) where T : struct, ISpanFormattable => Add( value, format,  culture, 650 );
    public JArray Add<T>( T value, ReadOnlySpan<char> format, CultureInfo culture, int bufferSize ) where T : struct, ISpanFormattable
    {
        writer.Append( value, format, culture, bufferSize )
              .Next();

        return this;
    }


    public JArray Add<T>( T? value ) where T : struct, ISpanFormattable => Add( value,                                                 CultureInfo.CurrentCulture );
    public JArray Add<T>( T? value, CultureInfo        culture ) where T : struct, ISpanFormattable => Add( value,                     default, culture );
    public JArray Add<T>( T? value, ReadOnlySpan<char> format, CultureInfo culture ) where T : struct, ISpanFormattable => Add( value, format,  culture, 650 );
    public JArray Add<T>( T? value, ReadOnlySpan<char> format, CultureInfo culture, int bufferSize ) where T : struct, ISpanFormattable
    {
        writer.Append( value, format, culture, bufferSize )
              .Next();

        return this;
    }


    public JArray AddArray() => new(writer, true);
    public JObject AddObject() => new(writer, true);


    public JArray AddObjects( IEnumerable<IJsonizer>? value )
    {
        if ( value is null ) { return Empty(); }

        return AddObjects( new List<IJsonizer>( value ) );
    }
    public JArray AddObjects( ICollection<IJsonizer> collection )
    {
        if ( collection.Count == 0 ) { return Empty(); }


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
