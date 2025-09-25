// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:59 AM

using System.Numerics;



namespace Jakar.Json.Serialization;


public ref struct JsonObject
{
    public const char    START = '{';
    public const char    END   = '}';
    private      JWriter __writer;


    public JsonObject( JWriter writer )
    {
        __writer = writer;
        Begin();
    }
    public readonly void Empty() => __writer.Append( START ).Append( JWriter.SPACE ).Append( END ).FinishBlock();
    public JsonObject Begin()
    {
        __writer.StartBlock( START );
        return this;
    }
    public void Complete() => __writer.FinishBlock( END );


    private readonly JWriter Start( ReadOnlySpan<char> key ) => __writer.Indent().Append( JWriter.QUOTE ).Append( key ).Append( JWriter.QUOTE ).Append( JWriter.COLON ).Append( JWriter.SPACE );
    public readonly JsonObject Null( ReadOnlySpan<char> key )
    {
        Start( key ).Null().Next();

        return this;
    }


    public readonly JsonObject Add( string value, [CallerArgumentExpression( nameof(value) )] string? key = null ) => Add( key.AsSpan(), value.AsSpan() );
    public readonly JsonObject Add( ReadOnlySpan<char> key, ReadOnlySpan<char> value )
    {
        Start( key ).Append( JWriter.QUOTE ).Append( value ).Append( JWriter.QUOTE ).Next();

        return this;
    }
    public readonly JsonObject Add( ReadOnlySpan<char> key, char value )
    {
        Start( key ).Append( JWriter.QUOTE ).Append( value ).Append( JWriter.QUOTE ).Next();

        return this;
    }


    public readonly JsonObject Add<TValue>( TValue? value, [CallerArgumentExpression( nameof(value) )] string? key = null )
        where TValue : ISpanFormattable => Add( value, CultureInfo.CurrentCulture, key );
    public readonly JsonObject Add<TValue>( TValue? value, IFormatProvider? provider, [CallerArgumentExpression( nameof(value) )] string? key = null )
        where TValue : ISpanFormattable => Add( value, JWriter.GetDefaultFormat<TValue>(), provider, key );
    public readonly JsonObject Add<TValue>( TValue? value, ReadOnlySpan<char> format, IFormatProvider? provider, [CallerArgumentExpression( nameof(value) )] string? key = null )
        where TValue : ISpanFormattable
    {
        if ( value is null ) { Start( key ).Null().Next(); }
        else { Start( key ).Append( JWriter.QUOTE ).Append( value, format, provider ).Append( JWriter.QUOTE ).Next(); }

        return this;
    }


    public readonly JsonObject AddValue<TValue>( TValue? value, int bufferSize, [CallerArgumentExpression( nameof(value) )] string? key = null )
        where TValue : struct, ISpanFormattable => AddValue( value, bufferSize, CultureInfo.CurrentCulture, key );
    public readonly JsonObject AddValue<TValue>( TValue? value, int bufferSize, IFormatProvider? provider, [CallerArgumentExpression( nameof(value) )] string? key = null )
        where TValue : struct, ISpanFormattable =>
        AddValue( value, bufferSize, default, provider, key );
    public readonly JsonObject AddValue<TValue>( TValue? value, int bufferSize, ReadOnlySpan<char> format, IFormatProvider? provider, [CallerArgumentExpression( nameof(value) )] string? key = null )
        where TValue : struct, ISpanFormattable
    {
        Start( key ).Append( JWriter.QUOTE ).AppendValue( value, format, bufferSize, provider ).Append( JWriter.QUOTE ).Next();

        return this;
    }

    public readonly JsonObject AddNumber<TValue>( TValue? value, [CallerArgumentExpression( nameof(value) )] string? key = null )
        where TValue : struct, INumber<TValue>, ISpanFormattable => AddNumber( value, CultureInfo.CurrentCulture, key );
    public readonly JsonObject AddNumber<TValue>( TValue? value, IFormatProvider? provider, [CallerArgumentExpression( nameof(value) )] string? key = null )
        where TValue : struct, INumber<TValue>, ISpanFormattable =>
        AddNumber( value, JWriter.GetDefaultFormat<TValue>(), provider, key );
    public readonly JsonObject AddNumber<TValue>( TValue? value, ReadOnlySpan<char> format, IFormatProvider? provider, [CallerArgumentExpression( nameof(value) )] string? key = null )
        where TValue : struct, INumber<TValue>, ISpanFormattable
    {
        if ( value is null ) { Start( key ).Null().Next(); }
        else { Start( key ).AppendValue( value, format, provider ).Next(); }


        return this;
    }

    public readonly JsonObject AddNumber<TValue>( TValue value, [CallerArgumentExpression( nameof(value) )] string? key = null )
        where TValue : struct, INumber<TValue>, ISpanFormattable => AddNumber( value, CultureInfo.CurrentCulture, key );
    public readonly JsonObject AddNumber<TValue>( TValue value, IFormatProvider? provider, [CallerArgumentExpression( nameof(value) )] string? key = null )
        where TValue : struct, INumber<TValue>, ISpanFormattable =>
        AddNumber( value, JWriter.GetDefaultFormat<TValue>(), provider, key );
    public readonly JsonObject AddNumber<TValue>( TValue value, ReadOnlySpan<char> format, IFormatProvider? provider, [CallerArgumentExpression( nameof(value) )] string? key = null )
        where TValue : struct, INumber<TValue>, ISpanFormattable
    {
        Start( key ).AppendValue( value, format, provider ).Next();

        return this;
    }


    public readonly JsonObject Add( KeyValuePair<string, string>         pair ) => Add( pair.Value, pair.Key );
    public readonly JsonObject Add( KeyValuePair<string, DateOnly>       pair ) => Add( pair.Value, pair.Key );
    public readonly JsonObject Add( KeyValuePair<string, TimeOnly>       pair ) => Add( pair.Value, pair.Key );
    public readonly JsonObject Add( KeyValuePair<string, TimeSpan>       pair ) => Add( pair.Value, pair.Key );
    public readonly JsonObject Add( KeyValuePair<string, DateTimeOffset> pair ) => Add( pair.Value, pair.Key );
    public readonly JsonObject Add( KeyValuePair<string, DateTime>       pair ) => Add( pair.Value, pair.Key );
    public readonly JsonObject Add<TValue>( KeyValuePair<string, TValue> pair )
        where TValue : ISpanFormattable => Add( pair.Value, pair.Key );
    public readonly JsonObject AddNumber<TValue>( KeyValuePair<string, TValue> pair )
        where TValue : struct, INumber<TValue>, ISpanFormattable => AddNumber( pair.Value, pair.Key );
    public readonly JsonObject AddNumber<TValue>( KeyValuePair<string, TValue?> pair )
        where TValue : struct, INumber<TValue>, ISpanFormattable => AddNumber( pair.Value, pair.Key );


    public readonly JsonObject Add( in DictionaryEntry pair )
    {
        string key = pair.Key.ToString() ?? throw new ArgumentNullException( nameof(pair), nameof(DictionaryEntry.Key) );

        return pair.Value switch
               {
                   null               => Null( key ),
                   string v           => Add( key, v ),
                   char v             => Add( key, v ),
                   ISpanFormattable v => Add( v,   key ),
                   _                  => Add( key, pair.Value.ToString() ?? JWriter.NULL )
               };
    }


    public readonly JArray AddArray( ReadOnlySpan<char> key ) => Start( key ).AddArray();
    public readonly JsonObject AddArray( IReadOnlyCollection<IJsonizer>? value, [CallerArgumentExpression( nameof(value) )] string? key = null )
    {
        AddArray( key ).AddObjects( value );

        return this;
    }


    public readonly JsonObject AddObject( ReadOnlySpan<char> key ) => Start( key ).AddObject();
    public readonly JsonObject AddObject( IDictionary value, [CallerArgumentExpression( nameof(value) )] string? key = null )
    {
        JsonObject node = AddObject( key );

        if ( value.Count == 0 )
        {
            node.Empty();
            return this;
        }


        node.Begin();

        foreach ( DictionaryEntry pair in value )
        {
            node.Add( pair );
            node.__writer.Next();
        }

        node.Complete();
        return this;
    }
    public readonly JsonObject AddObject( IReadOnlyDictionary<string, IJsonizer> value, [CallerArgumentExpression( nameof(value) )] string? key = null )
    {
        JsonObject node = AddObject( key );

        if ( value.Count == 0 )
        {
            node.Empty();
            return this;
        }


        node.Begin();

        foreach ( (int index, KeyValuePair<string, IJsonizer> pair) in value.Enumerate( 0 ) )
        {
            (string? k, IJsonizer? jsonizer) = pair;
            JsonObject item = node.AddObject( k );
            jsonizer.Serialize( ref item );
            item.__writer.Next();

            if ( index < value.Count ) { node.__writer.Next(); }
        }

        node.Complete();
        return this;
    }
    public readonly JsonObject AddObject( IReadOnlyCollection<KeyValuePair<string, IJsonizer>> value, [CallerArgumentExpression( nameof(value) )] string? key = null )
    {
        JsonObject node = AddObject( key );

        if ( value.Count == 0 )
        {
            node.Empty();
            return this;
        }


        node.Begin();

        foreach ( (int index, (string? k, IJsonizer? jsonizer)) in value.Enumerate( 0 ) )
        {
            JsonObject item = node.AddObject( k );
            jsonizer.Serialize( ref item );
            item.__writer.Next();

            if ( index < value.Count ) { node.__writer.Next(); }
        }

        node.Complete();
        return this;
    }
}
