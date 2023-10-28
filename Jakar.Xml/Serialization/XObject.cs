// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:59 AM

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;



namespace Jakar.Xml.Serialization;


[ SuppressMessage( "ReSharper", "PossiblyImpureMethodCallOnReadonlyVariable" ) ]
public ref struct XObject
{
    private readonly ReadOnlySpan<char> _name;
    private          XWriter            _writer;


    public XObject( ReadOnlySpan<char> key, XWriter context )
    {
        _name   = key;
        _writer = context;
    }

    public XObject Init()
    {
        _writer.StartBlock( _name );
        return this;
    }
    public XObject Init( XAttributeBuilder builder )
    {
        _writer.StartBlock( _name, builder );
        return this;
    }


    public XObject Null( ReadOnlySpan<char> key )
    {
        _writer.Indent( key ).Append( XWriter.NULL ).Next( key );

        return this;
    }
    public XObject Add( ReadOnlySpan<char> key, char value )
    {
        _writer.Indent( key ).Append( value ).Next( key );

        return this;
    }
    public XObject Add( ReadOnlySpan<char> key, short value )
    {
        _writer.Indent( key ).Append( value ).Next( key );

        return this;
    }
    public XObject Add( ReadOnlySpan<char> key, ushort value )
    {
        _writer.Indent( key ).Append( value ).Next( key );

        return this;
    }
    public XObject Add( ReadOnlySpan<char> key, int value )
    {
        _writer.Indent( key ).Append( value ).Next( key );

        return this;
    }
    public XObject Add( ReadOnlySpan<char> key, uint value )
    {
        _writer.Indent( key ).Append( value ).Next( key );

        return this;
    }
    public XObject Add( ReadOnlySpan<char> key, long value )
    {
        _writer.Indent( key ).Append( value ).Next( key );

        return this;
    }
    public XObject Add( ReadOnlySpan<char> key, ulong value )
    {
        _writer.Indent( key ).Append( value ).Next( key );

        return this;
    }
    public XObject Add( ReadOnlySpan<char> key, float value )
    {
        _writer.Indent( key ).Append( value ).Next( key );

        return this;
    }
    public XObject Add( ReadOnlySpan<char> key, double value )
    {
        _writer.Indent( key ).Append( value ).Next( key );

        return this;
    }
    public XObject Add( ReadOnlySpan<char> key, decimal value )
    {
        _writer.Indent( key ).Append( value ).Next( key );

        return this;
    }


    public XObject Add( ReadOnlySpan<char> key, char? value ) => value.HasValue
                                                                     ? Add( key, value.Value )
                                                                     : Null( key );
    public XObject Add( ReadOnlySpan<char> key, short? value ) => value.HasValue
                                                                      ? Add( key, value.Value )
                                                                      : Null( key );
    public XObject Add( ReadOnlySpan<char> key, ushort? value ) => value.HasValue
                                                                       ? Add( key, value.Value )
                                                                       : Null( key );
    public XObject Add( ReadOnlySpan<char> key, int? value ) => value.HasValue
                                                                    ? Add( key, value.Value )
                                                                    : Null( key );
    public XObject Add( ReadOnlySpan<char> key, uint? value ) => value.HasValue
                                                                     ? Add( key, value.Value )
                                                                     : Null( key );
    public XObject Add( ReadOnlySpan<char> key, long? value ) => value.HasValue
                                                                     ? Add( key, value.Value )
                                                                     : Null( key );
    public XObject Add( ReadOnlySpan<char> key, ulong? value ) => value.HasValue
                                                                      ? Add( key, value.Value )
                                                                      : Null( key );
    public XObject Add( ReadOnlySpan<char> key, float? value ) => value.HasValue
                                                                      ? Add( key, value.Value )
                                                                      : Null( key );
    public XObject Add( ReadOnlySpan<char> key, double? value ) => value.HasValue
                                                                       ? Add( key, value.Value )
                                                                       : Null( key );
    public XObject Add( ReadOnlySpan<char> key, decimal? value ) => value.HasValue
                                                                        ? Add( key, value.Value )
                                                                        : Null( key );


    public XObject Add( ReadOnlySpan<char> key, string value ) => Add( key, value.AsSpan() );
    public XObject Add( ReadOnlySpan<char> key, ReadOnlySpan<char> value )
    {
        _writer.Indent( key ).Append( value ).Next( key );

        return this;
    }


    public XObject Add( ReadOnlySpan<char> key, ISpanFormattable value, int bufferSize )                      => Add( key, value, bufferSize, CultureInfo.CurrentCulture );
    public XObject Add( ReadOnlySpan<char> key, ISpanFormattable value, int bufferSize, CultureInfo culture ) => Add( key, value, bufferSize, default, culture );
    public XObject Add( ReadOnlySpan<char> key, ISpanFormattable value, int bufferSize, ReadOnlySpan<char> format, CultureInfo culture )
    {
        _writer.Indent( key ).Append( value, format, culture, bufferSize ).Next( key );

        return this;
    }


    public XObject Add<T>( ReadOnlySpan<char> key, T? value, int bufferSize ) where T : struct, ISpanFormattable                      => Add( key, value, bufferSize, CultureInfo.CurrentCulture );
    public XObject Add<T>( ReadOnlySpan<char> key, T? value, int bufferSize, CultureInfo culture ) where T : struct, ISpanFormattable => Add( key, value, bufferSize, default, culture );
    public XObject Add<T>( ReadOnlySpan<char> key, T? value, int bufferSize, ReadOnlySpan<char> format, CultureInfo culture ) where T : struct, ISpanFormattable
    {
        _writer.Indent( key ).Append( value, format, culture, bufferSize ).Next( key );

        return this;
    }


    public XObject Add( KeyValuePair<string, string>  pair ) => Add( pair.Key, pair.Value );
    public XObject Add( KeyValuePair<string, char>    pair ) => Add( pair.Key, pair.Value );
    public XObject Add( KeyValuePair<string, short>   pair ) => Add( pair.Key, pair.Value );
    public XObject Add( KeyValuePair<string, ushort>  pair ) => Add( pair.Key, pair.Value );
    public XObject Add( KeyValuePair<string, int>     pair ) => Add( pair.Key, pair.Value );
    public XObject Add( KeyValuePair<string, uint>    pair ) => Add( pair.Key, pair.Value );
    public XObject Add( KeyValuePair<string, long>    pair ) => Add( pair.Key, pair.Value );
    public XObject Add( KeyValuePair<string, ulong>   pair ) => Add( pair.Key, pair.Value );
    public XObject Add( KeyValuePair<string, float>   pair ) => Add( pair.Key, pair.Value );
    public XObject Add( KeyValuePair<string, double>  pair ) => Add( pair.Key, pair.Value );
    public XObject Add( KeyValuePair<string, decimal> pair ) => Add( pair.Key, pair.Value );


    public XObject Add( KeyValuePair<string, DateOnly>         pair )                        => Add( pair.Key, pair.Value, 100 );
    public XObject Add( KeyValuePair<string, TimeOnly>         pair )                        => Add( pair.Key, pair.Value, 100 );
    public XObject Add( KeyValuePair<string, TimeSpan>         pair )                        => Add( pair.Key, pair.Value, 100 );
    public XObject Add( KeyValuePair<string, DateTimeOffset>   pair )                        => Add( pair.Key, pair.Value, 100 );
    public XObject Add( KeyValuePair<string, DateTime>         pair )                        => Add( pair.Key, pair.Value, 100 );
    public XObject Add( KeyValuePair<string, ISpanFormattable> pair, int bufferSize = 1000 ) => Add( pair.Key, pair.Value, bufferSize );


    public XObject Add( DictionaryEntry pair, int bufferSize = 1000 )
    {
        ReadOnlySpan<char> k = pair.Key.ToString();

        return pair.Value switch
               {
                   null               => Null( k ),
                   string v           => Add( k, v ),
                   char v             => Add( k, v ),
                   short v            => Add( k, v, 10 ),
                   ushort v           => Add( k, v, 10 ),
                   int v              => Add( k, v, 20 ),
                   uint v             => Add( k, v, 20 ),
                   long v             => Add( k, v, 30 ),
                   ulong v            => Add( k, v, 30 ),
                   float v            => Add( k, v, 350 ),
                   double v           => Add( k, v, 650 ),
                   decimal v          => Add( k, v, 200 ),
                   DateOnly v         => Add( k, v, 100 ),
                   TimeOnly v         => Add( k, v, 100 ),
                   TimeSpan v         => Add( k, v, 100 ),
                   DateTimeOffset v   => Add( k, v, 100 ),
                   DateTime v         => Add( k, v, 100 ),
                   ISpanFormattable v => Add( k, v, bufferSize ),
                   _                  => Add( k, pair.Value.ToString() ?? XWriter.NULL )
               };
    }


    public XArray  AddArray( ReadOnlySpan<char>  key ) => new(key, _writer);
    public XObject AddObject( ReadOnlySpan<char> key ) => new(key, _writer);


    public void Dispose() => _writer.FinishBlock( _name );
}
