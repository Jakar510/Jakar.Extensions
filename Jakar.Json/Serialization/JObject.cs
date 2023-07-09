// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:59 AM

#nullable enable
using System.Runtime.CompilerServices;



namespace Jakar.Json.Serialization;


public ref struct JObject
{
    public const     char    START = '{';
    public const     char    END   = '}';
    private readonly bool    _shouldIndent;
    internal         JWriter writer;


    public JObject( JWriter writer, bool shouldIndent )
    {
        _shouldIndent = shouldIndent;
        this.writer   = writer;
        Begin();
    }
    public readonly void Empty() => writer.Append( START )
                                          .Append( ' ' )
                                          .Append( END )
                                          .FinishBlock();
    public JObject Begin()
    {
        writer.StartBlock( START, _shouldIndent );
        return this;
    }
    public void Complete() => writer.FinishBlock( END );


    private readonly JWriter Start( ReadOnlySpan<char> key ) => writer.Indent()
                                                                      .Append( '"' )
                                                                      .Append( key )
                                                                      .Append( '"' )
                                                                      .Append( ':' )
                                                                      .Append( ' ' );
    public readonly JObject Null( ReadOnlySpan<char> key )
    {
        Start( key )
           .Null()
           .Next();

        return this;
    }


    public readonly JObject Add( string value, [CallerArgumentExpression( nameof(value) )] string key = "" ) => Add( key.AsSpan(), value.AsSpan() );
    public readonly JObject Add( ReadOnlySpan<char> key, ReadOnlySpan<char> value )
    {
        Start( key )
           .Append( '"' )
           .Append( value )
           .Append( '"' )
           .Next();

        return this;
    }


    public readonly JObject Add<T>( T? value, [CallerArgumentExpression( nameof(value) )] string key                                                              = "" ) where T : ISpanFormattable => Add( value, CultureInfo.CurrentCulture, key );
    public readonly JObject Add<T>( T? value, IFormatProvider?                                   provider, [CallerArgumentExpression( nameof(value) )] string key = "" ) where T : ISpanFormattable => Add( value, default, provider, key );
    public readonly JObject Add<T>( T? value, ReadOnlySpan<char> format, IFormatProvider? provider, [CallerArgumentExpression( nameof(value) )] string key = "" ) where T : ISpanFormattable
    {
        Start( key )
           .Append( '"' )
           .Append( value, format, provider )
           .Append( '"' )
           .Next();

        return this;
    }


    public readonly JObject Add<T>( T? value, int bufferSize, [CallerArgumentExpression( nameof(value) )] string key = "" ) where T : struct, ISpanFormattable => Add( value, bufferSize, CultureInfo.CurrentCulture, key );
    public readonly JObject Add<T>( T? value, int bufferSize, IFormatProvider? provider, [CallerArgumentExpression( nameof(value) )] string key = "" ) where T : struct, ISpanFormattable => Add( value, bufferSize, default, provider, key );
    public readonly JObject Add<T>( T? value, int bufferSize, ReadOnlySpan<char> format, IFormatProvider? provider, [CallerArgumentExpression( nameof(value) )] string key = "" ) where T : struct, ISpanFormattable
    {
        Start( key )
           .Append( '"' )
           .Append( value, format, bufferSize, provider )
           .Append( '"' )
           .Next();

        return this;
    }


    public readonly JObject Add( KeyValuePair<string, string>  pair ) => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, char>    pair ) => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, short>   pair ) => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, ushort>  pair ) => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, int>     pair ) => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, uint>    pair ) => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, long>    pair ) => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, ulong>   pair ) => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, float>   pair ) => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, double>  pair ) => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, decimal> pair ) => Add( pair.Value, pair.Key );


    public readonly JObject Add( KeyValuePair<string, DateOnly>       pair ) => Add( pair.Value,                            pair.Key );
    public readonly JObject Add( KeyValuePair<string, TimeOnly>       pair ) => Add( pair.Value,                            pair.Key );
    public readonly JObject Add( KeyValuePair<string, TimeSpan>       pair ) => Add( pair.Value,                            pair.Key );
    public readonly JObject Add( KeyValuePair<string, DateTimeOffset> pair ) => Add( pair.Value,                            pair.Key );
    public readonly JObject Add( KeyValuePair<string, DateTime>       pair ) => Add( pair.Value,                            pair.Key );
    public readonly JObject Add<T>( KeyValuePair<string, T>           pair ) where T : ISpanFormattable => Add( pair.Value, pair.Key );


    public readonly JObject Add( DictionaryEntry pair )
    {
        string? k = pair.Key.ToString();
        if ( string.IsNullOrWhiteSpace( k ) ) { throw new ArgumentNullException( $"{nameof(DictionaryEntry)}.{nameof(DictionaryEntry.Key)} cannot be null or empty." ); }


        return pair.Value switch
               {
                   null               => Null( k ),
                   string v           => Add( k.AsSpan(), v.AsSpan() ),
                   char v             => Add( v ),
                   short v            => Add( v,                               k ),
                   ushort v           => Add( v,                               k ),
                   int v              => Add( v,                               k ),
                   uint v             => Add( v,                               k ),
                   long v             => Add( v,                               k ),
                   ulong v            => Add( v,                               k ),
                   float v            => Add( v,                               k ),
                   double v           => Add( v,                               k ),
                   decimal v          => Add( v,                               k ),
                   DateOnly v         => Add( v,                               k ),
                   TimeOnly v         => Add( v,                               k ),
                   TimeSpan v         => Add( v,                               k ),
                   DateTimeOffset v   => Add( v,                               k ),
                   DateTime v         => Add( v,                               k ),
                   ISpanFormattable v => Add( v,                               k ),
                   _                  => Add( pair.Value.ToString() ?? "null", k ),
               };
    }


    public readonly JArray AddArray( ReadOnlySpan<char> key ) => Start( key )
       .AddArray();
    public readonly JObject AddArray( IReadOnlyCollection<IJsonizer>? value, [CallerArgumentExpression( nameof(value) )] string key = "" )
    {
        AddArray( key )
           .AddObjects( value );

        return this;
    }


    public readonly JObject AddObject( ReadOnlySpan<char> key ) => Start( key )
       .AddObject();
    public readonly JObject AddObject( IDictionary value, [CallerArgumentExpression( nameof(value) )] string key = "" )
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
    public readonly JObject AddObject( IReadOnlyDictionary<string, IJsonizer> value, [CallerArgumentExpression( nameof(value) )] string key = "" )
    {
        JObject node = AddObject( key );

        if ( value.Count == 0 )
        {
            node.Empty();
            return this;
        }


        node.Begin();

        foreach ( (int index, KeyValuePair<string, IJsonizer> pair) in value.Enumerate( 0 ) )
        {
            (string? k, IJsonizer? jsonizer) = pair;
            JObject item = node.AddObject( k );
            jsonizer.Serialize( ref item );
            item.writer.Next();

            if ( index < value.Count ) { node.writer.Next(); }
        }

        node.Complete();
        return this;
    }
    public readonly JObject AddObject( IReadOnlyCollection<KeyValuePair<string, IJsonizer>> value, [CallerArgumentExpression( nameof(value) )] string key = "" )
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
