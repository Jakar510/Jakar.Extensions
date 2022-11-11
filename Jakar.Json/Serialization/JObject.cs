// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:59 AM

#nullable enable
using System.Globalization;
using System.Runtime.CompilerServices;
using Jakar.Extensions;



namespace Jakar.Json.Serialization;


public ref struct JObject
{
    public const char START = '{';
    public const char END   = '}';

    private readonly bool    _shouldIndent;
    internal         JWriter writer;


    public JObject( ref JWriter writer, bool shouldIndent )
    {
        _shouldIndent = shouldIndent;
        this.writer   = writer;
    }
    public void Empty() => writer.Append( START )
                                 .Append( ' ' )
                                 .Append( END )
                                 .FinishBlock();
    public JObject Begin()
    {
        writer.StartBlock( START, _shouldIndent );
        return this;
    }
    public void Complete() => writer.FinishBlock( END );


    private JWriter Start( ReadOnlySpan<char> key ) => writer.Indent()
                                                             .Append( '"' )
                                                             .Append( key )
                                                             .Append( '"' )
                                                             .Append( ':' )
                                                             .Append( ' ' );
    public JObject Null( ReadOnlySpan<char> key )
    {
        Start( key )
           .Null()
           .Next();

        return this;
    }


    public JObject Add( char value, [CallerArgumentExpression( "value" )] string key = "" )
    {
        Start( key )
           .Append( value )
           .Next();

        return this;
    }
    public JObject Add( short value, [CallerArgumentExpression( "value" )] string key = "" )
    {
        Start( key )
           .Append( value )
           .Next();

        return this;
    }
    public JObject Add( ushort value, [CallerArgumentExpression( "value" )] string key = "" )
    {
        Start( key )
           .Append( value )
           .Next();

        return this;
    }
    public JObject Add( int value, [CallerArgumentExpression( "value" )] string key = "" )
    {
        Start( key )
           .Append( value )
           .Next();

        return this;
    }
    public JObject Add( uint value, [CallerArgumentExpression( "value" )] string key = "" )
    {
        Start( key )
           .Append( value )
           .Next();

        return this;
    }
    public JObject Add( long value, [CallerArgumentExpression( "value" )] string key = "" )
    {
        Start( key )
           .Append( value )
           .Next();

        return this;
    }
    public JObject Add( ulong value, [CallerArgumentExpression( "value" )] string key = "" )
    {
        Start( key )
           .Append( value )
           .Next();

        return this;
    }
    public JObject Add( float value, [CallerArgumentExpression( "value" )] string key = "" )
    {
        Start( key )
           .Append( value )
           .Next();

        return this;
    }
    public JObject Add( double value, [CallerArgumentExpression( "value" )] string key = "" )
    {
        Start( key )
           .Append( value )
           .Next();

        return this;
    }
    public JObject Add( decimal value, [CallerArgumentExpression( "value" )] string key = "" )
    {
        Start( key )
           .Append( value )
           .Next();

        return this;
    }


    public JObject Add( char? value, [CallerArgumentExpression( "value" )] string key = "" ) => value.HasValue
                                                                                                    ? Add( value.Value, key )
                                                                                                    : Null( key );
    public JObject Add( short? value, [CallerArgumentExpression( "value" )] string key = "" ) => value.HasValue
                                                                                                     ? Add( value.Value, key )
                                                                                                     : Null( key );
    public JObject Add( ushort? value, [CallerArgumentExpression( "value" )] string key = "" ) => value.HasValue
                                                                                                      ? Add( value.Value, key )
                                                                                                      : Null( key );
    public JObject Add( int? value, [CallerArgumentExpression( "value" )] string key = "" ) => value.HasValue
                                                                                                   ? Add( value.Value, key )
                                                                                                   : Null( key );
    public JObject Add( uint? value, [CallerArgumentExpression( "value" )] string key = "" ) => value.HasValue
                                                                                                    ? Add( value.Value, key )
                                                                                                    : Null( key );
    public JObject Add( long? value, [CallerArgumentExpression( "value" )] string key = "" ) => value.HasValue
                                                                                                    ? Add( value.Value, key )
                                                                                                    : Null( key );
    public JObject Add( ulong? value, [CallerArgumentExpression( "value" )] string key = "" ) => value.HasValue
                                                                                                     ? Add( value.Value, key )
                                                                                                     : Null( key );
    public JObject Add( float? value, [CallerArgumentExpression( "value" )] string key = "" ) => value.HasValue
                                                                                                     ? Add( value.Value, key )
                                                                                                     : Null( key );
    public JObject Add( double? value, [CallerArgumentExpression( "value" )] string key = "" ) => value.HasValue
                                                                                                      ? Add( value.Value, key )
                                                                                                      : Null( key );
    public JObject Add( decimal? value, [CallerArgumentExpression( "value" )] string key = "" ) => value.HasValue
                                                                                                       ? Add( value.Value, key )
                                                                                                       : Null( key );


    public JObject Add( string value, [CallerArgumentExpression( "value" )] string key = "" ) => Add( key.AsSpan(), value.AsSpan() );
    public JObject Add( ReadOnlySpan<char> key, ReadOnlySpan<char> value )
    {
        Start( key )
           .Append( '"' )
           .Append( value )
           .Append( '"' )
           .Next();

        return this;
    }


    public JObject Add( ISpanFormattable value, int bufferSize, [CallerArgumentExpression( "value" )] string key                                                       = "" ) => Add( value, bufferSize, CultureInfo.CurrentCulture, key );
    public JObject Add( ISpanFormattable value, int bufferSize, CultureInfo                                  culture, [CallerArgumentExpression( "value" )] string key = "" ) => Add( value, bufferSize, default,                    culture, key );

    public JObject Add( ISpanFormattable value, int bufferSize, ReadOnlySpan<char> format, CultureInfo culture, [CallerArgumentExpression( "value" )] string key = "" )
    {
        Start( key )
           .Append( '"' )
           .Append( value, format, culture, bufferSize )
           .Append( '"' )
           .Next();

        return this;
    }


    public JObject Add<T>( T? value, int bufferSize, [CallerArgumentExpression( "value" )] string key = "" ) where T : struct, ISpanFormattable => Add( value, bufferSize, CultureInfo.CurrentCulture, key );
    public JObject Add<T>( T? value, int bufferSize, CultureInfo                                  culture, [CallerArgumentExpression( "value" )] string key = "" ) where T : struct, ISpanFormattable => Add( value, bufferSize, default, culture, key );

    public JObject Add<T>( T? value, int bufferSize, ReadOnlySpan<char> format, CultureInfo culture, [CallerArgumentExpression( "value" )] string key = "" ) where T : struct, ISpanFormattable
    {
        Start( key )
           .Append( '"' )
           .Append( value, format, culture, bufferSize )
           .Append( '"' )
           .Next();

        return this;
    }


    public JObject Add( KeyValuePair<string, string>  pair ) => Add( pair.Value, pair.Key );
    public JObject Add( KeyValuePair<string, char>    pair ) => Add( pair.Value, pair.Key );
    public JObject Add( KeyValuePair<string, short>   pair ) => Add( pair.Value, pair.Key );
    public JObject Add( KeyValuePair<string, ushort>  pair ) => Add( pair.Value, pair.Key );
    public JObject Add( KeyValuePair<string, int>     pair ) => Add( pair.Value, pair.Key );
    public JObject Add( KeyValuePair<string, uint>    pair ) => Add( pair.Value, pair.Key );
    public JObject Add( KeyValuePair<string, long>    pair ) => Add( pair.Value, pair.Key );
    public JObject Add( KeyValuePair<string, ulong>   pair ) => Add( pair.Value, pair.Key );
    public JObject Add( KeyValuePair<string, float>   pair ) => Add( pair.Value, pair.Key );
    public JObject Add( KeyValuePair<string, double>  pair ) => Add( pair.Value, pair.Key );
    public JObject Add( KeyValuePair<string, decimal> pair ) => Add( pair.Value, pair.Key );


    public JObject Add( KeyValuePair<string, DateOnly>         pair ) => Add( pair.Value,                        100,        pair.Key );
    public JObject Add( KeyValuePair<string, TimeOnly>         pair ) => Add( pair.Value,                        100,        pair.Key );
    public JObject Add( KeyValuePair<string, TimeSpan>         pair ) => Add( pair.Value,                        100,        pair.Key );
    public JObject Add( KeyValuePair<string, DateTimeOffset>   pair ) => Add( pair.Value,                        100,        pair.Key );
    public JObject Add( KeyValuePair<string, DateTime>         pair ) => Add( pair.Value,                        100,        pair.Key );
    public JObject Add( KeyValuePair<string, ISpanFormattable> pair, int bufferSize = 1000 ) => Add( pair.Value, bufferSize, pair.Key );


    public JObject Add( DictionaryEntry pair, int bufferSize = 1000 )
    {
        string? k = pair.Key.ToString();
        ArgumentNullException.ThrowIfNull( k );

        return pair.Value switch
               {
                   null               => Null( k ),
                   string v           => Add( k.AsSpan(), v.AsSpan() ),
                   char v             => Add( v ),
                   short v            => Add( v,                               10,         k ),
                   ushort v           => Add( v,                               10,         k ),
                   int v              => Add( v,                               20,         k ),
                   uint v             => Add( v,                               20,         k ),
                   long v             => Add( v,                               30,         k ),
                   ulong v            => Add( v,                               30,         k ),
                   float v            => Add( v,                               350,        k ),
                   double v           => Add( v,                               650,        k ),
                   decimal v          => Add( v,                               300,        k ),
                   DateOnly v         => Add( v,                               100,        k ),
                   TimeOnly v         => Add( v,                               100,        k ),
                   TimeSpan v         => Add( v,                               100,        k ),
                   DateTimeOffset v   => Add( v,                               100,        k ),
                   DateTime v         => Add( v,                               100,        k ),
                   ISpanFormattable v => Add( v,                               bufferSize, k ),
                   _                  => Add( pair.Value.ToString() ?? "null", k )
               };
    }


    public JArray AddArray( ReadOnlySpan<char> key ) => Start( key )
       .AddArray();
    public JObject AddArray( IEnumerable<IJsonizer>? value, [CallerArgumentExpression( "value" )] string key = "" )
    {
        AddArray( key )
           .AddObjects( value );

        return this;
    }


    public JObject AddObject( ReadOnlySpan<char> key ) => Start( key )
       .AddObject();
    public JObject AddObject( IDictionary value, [CallerArgumentExpression( "value" )] string key = "" )
    {
        JObject node = AddObject( key );

        if ( value.Count == 0 )
        {
            node.Empty();
            return this;
        }


        node.Begin();

        foreach ( DictionaryEntry pair in value )
        {
            node.Add( pair );
            node.writer.Next();
        }

        node.Complete();
        return this;
    }
    public JObject AddObject( IDictionary<string, IJsonizer> value, [CallerArgumentExpression( "value" )] string key = "" )
    {
        JObject node = AddObject( key );

        if ( value.Count == 0 )
        {
            node.Empty();
            return this;
        }


        node.Begin();

        foreach ( (int index, string? k, IJsonizer? jsonizer) in value.Enumerate( 0 ) )
        {
            JObject item = node.AddObject( k );
            jsonizer.Serialize( ref item );
            item.writer.Next();

            if ( index < value.Count ) { node.writer.Next(); }
        }

        node.Complete();
        return this;
    }
    public JObject AddObject( ICollection<KeyValuePair<string, IJsonizer>> value, [CallerArgumentExpression( "value" )] string key = "" )
    {
        JObject node = AddObject( key );

        if ( value.Count == 0 )
        {
            node.Empty();
            return this;
        }


        node.Begin();

        foreach ( (int index, (string? k, IJsonizer? jsonizer)) in value.Enumerate( 0 ) )
        {
            JObject item = node.AddObject( k );
            jsonizer.Serialize( ref item );
            item.writer.Next();

            if ( index < value.Count ) { node.writer.Next(); }
        }

        node.Complete();
        return this;
    }
}
