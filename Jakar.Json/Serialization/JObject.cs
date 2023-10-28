// Jakar.Extensions :: Jakar.Xml
// 04/26/2022  9:59 AM

using System.Numerics;
using System.Runtime.CompilerServices;



namespace Jakar.Json.Serialization;


public ref struct JObject
{
    public const char    START = '{';
    public const char    END   = '}';
    private      JWriter _writer;


    public JObject( JWriter writer )
    {
        _writer = writer;
        Begin();
    }
    public readonly void Empty() => _writer.Append( START ).Append( JWriter.SPACE ).Append( END ).FinishBlock();
    public JObject Begin()
    {
        _writer.StartBlock( START );
        return this;
    }
    public void Complete() => _writer.FinishBlock( END );


    private readonly JWriter Start( ReadOnlySpan<char> key ) => _writer.Indent().Append( JWriter.QUOTE ).Append( key ).Append( JWriter.QUOTE ).Append( JWriter.COLON ).Append( JWriter.SPACE );
    public readonly JObject Null( ReadOnlySpan<char> key )
    {
        Start( key ).Null().Next();

        return this;
    }


    public readonly JObject Add( string value, [ CallerArgumentExpression( nameof(value) ) ] string? key = default ) => Add( key.AsSpan(), value.AsSpan() );
    public readonly JObject Add( ReadOnlySpan<char> key, ReadOnlySpan<char> value )
    {
        Start( key ).Append( JWriter.QUOTE ).Append( value ).Append( JWriter.QUOTE ).Next();

        return this;
    }
    public readonly JObject Add( ReadOnlySpan<char> key, char value )
    {
        Start( key ).Append( JWriter.QUOTE ).Append( value ).Append( JWriter.QUOTE ).Next();

        return this;
    }


    public readonly JObject Add<T>( T? value, [ CallerArgumentExpression( nameof(value) ) ] string? key = default ) where T : ISpanFormattable => Add( value, CultureInfo.CurrentCulture, key );
    public readonly JObject Add<T>( T? value, IFormatProvider? provider, [ CallerArgumentExpression( nameof(value) ) ] string? key = default ) where T : ISpanFormattable => Add( value, JWriter.GetDefaultFormat<T>(), provider, key );
    public readonly JObject Add<T>( T? value, ReadOnlySpan<char> format, IFormatProvider? provider, [ CallerArgumentExpression( nameof(value) ) ] string? key = default ) where T : ISpanFormattable
    {
        if ( value is null ) { Start( key ).Null().Next(); }
        else { Start( key ).Append( JWriter.QUOTE ).Append( value, format, provider ).Append( JWriter.QUOTE ).Next(); }

        return this;
    }


    public readonly JObject AddValue<T>( T? value, int bufferSize, [ CallerArgumentExpression( nameof(value) ) ] string? key = default ) where T : struct, ISpanFormattable => AddValue( value, bufferSize, CultureInfo.CurrentCulture, key );
    public readonly JObject AddValue<T>( T? value, int bufferSize, IFormatProvider? provider, [ CallerArgumentExpression( nameof(value) ) ] string? key = default ) where T : struct, ISpanFormattable =>
        AddValue( value, bufferSize, default, provider, key );
    public readonly JObject AddValue<T>( T? value, int bufferSize, ReadOnlySpan<char> format, IFormatProvider? provider, [ CallerArgumentExpression( nameof(value) ) ] string? key = default ) where T : struct, ISpanFormattable
    {
        Start( key ).Append( JWriter.QUOTE ).AppendValue( value, format, bufferSize, provider ).Append( JWriter.QUOTE ).Next();

        return this;
    }

    public readonly JObject AddNumber<T>( T? value, [ CallerArgumentExpression( nameof(value) ) ] string? key = default ) where T : struct, INumber<T>, ISpanFormattable => AddNumber( value, CultureInfo.CurrentCulture, key );
    public readonly JObject AddNumber<T>( T? value, IFormatProvider? provider, [ CallerArgumentExpression( nameof(value) ) ] string? key = default ) where T : struct, INumber<T>, ISpanFormattable =>
        AddNumber( value, JWriter.GetDefaultFormat<T>(), provider, key );
    public readonly JObject AddNumber<T>( T? value, ReadOnlySpan<char> format, IFormatProvider? provider, [ CallerArgumentExpression( nameof(value) ) ] string? key = default ) where T : struct, INumber<T>, ISpanFormattable
    {
        if ( value is null ) { Start( key ).Null().Next(); }
        else { Start( key ).AppendValue( value, format, provider ).Next(); }


        return this;
    }

    public readonly JObject AddNumber<T>( T value, [ CallerArgumentExpression( nameof(value) ) ] string? key = default ) where T : struct, INumber<T>, ISpanFormattable => AddNumber( value, CultureInfo.CurrentCulture, key );
    public readonly JObject AddNumber<T>( T value, IFormatProvider? provider, [ CallerArgumentExpression( nameof(value) ) ] string? key = default ) where T : struct, INumber<T>, ISpanFormattable =>
        AddNumber( value, JWriter.GetDefaultFormat<T>(), provider, key );
    public readonly JObject AddNumber<T>( T value, ReadOnlySpan<char> format, IFormatProvider? provider, [ CallerArgumentExpression( nameof(value) ) ] string? key = default ) where T : struct, INumber<T>, ISpanFormattable
    {
        Start( key ).AppendValue( value, format, provider ).Next();

        return this;
    }


    public readonly JObject Add( KeyValuePair<string, string>         pair )                                                => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, DateOnly>       pair )                                                => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, TimeOnly>       pair )                                                => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, TimeSpan>       pair )                                                => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, DateTimeOffset> pair )                                                => Add( pair.Value, pair.Key );
    public readonly JObject Add( KeyValuePair<string, DateTime>       pair )                                                => Add( pair.Value, pair.Key );
    public readonly JObject Add<T>( KeyValuePair<string, T>           pair ) where T : ISpanFormattable                     => Add( pair.Value, pair.Key );
    public readonly JObject AddNumber<T>( KeyValuePair<string, T>     pair ) where T : struct, INumber<T>, ISpanFormattable => AddNumber( pair.Value, pair.Key );
    public readonly JObject AddNumber<T>( KeyValuePair<string, T?>    pair ) where T : struct, INumber<T>, ISpanFormattable => AddNumber( pair.Value, pair.Key );


    public readonly JObject Add( in DictionaryEntry pair )
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
    public readonly JObject AddArray( IReadOnlyCollection<IJsonizer>? value, [ CallerArgumentExpression( nameof(value) ) ] string? key = default )
    {
        AddArray( key ).AddObjects( value );

        return this;
    }


    public readonly JObject AddObject( ReadOnlySpan<char> key ) => Start( key ).AddObject();
    public readonly JObject AddObject( IDictionary value, [ CallerArgumentExpression( nameof(value) ) ] string? key = default )
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
            node._writer.Next();
        }

        node.Complete();
        return this;
    }
    public readonly JObject AddObject( IReadOnlyDictionary<string, IJsonizer> value, [ CallerArgumentExpression( nameof(value) ) ] string? key = default )
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
            item._writer.Next();

            if ( index < value.Count ) { node._writer.Next(); }
        }

        node.Complete();
        return this;
    }
    public readonly JObject AddObject( IReadOnlyCollection<KeyValuePair<string, IJsonizer>> value, [ CallerArgumentExpression( nameof(value) ) ] string? key = default )
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
            item._writer.Next();

            if ( index < value.Count ) { node._writer.Next(); }
        }

        node.Complete();
        return this;
    }
}
