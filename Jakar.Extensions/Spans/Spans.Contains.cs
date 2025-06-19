// Jakar.Extensions :: Jakar.Extensions
// 06/10/2022  10:17 AM

using System.Runtime.Intrinsics;



namespace Jakar.Extensions;


public static partial class Spans
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool ContainsAny<TValue>( scoped ref readonly ReadOnlySpan<TValue> span, SearchValues<TValue> value )
        where TValue : IEquatable<TValue> => span.ContainsAny( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool ContainsAnyExcept<TValue>( scoped ref readonly ReadOnlySpan<TValue> span, SearchValues<TValue> value )
        where TValue : IEquatable<TValue> => span.ContainsAnyExcept( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int IndexOfAnyExcept<TValue>( scoped ref readonly ReadOnlySpan<TValue> span, SearchValues<TValue> value )
        where TValue : IEquatable<TValue> => span.IndexOfAnyExcept( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int IndexOfAny<TValue>( scoped ref readonly ReadOnlySpan<TValue> span, SearchValues<TValue> value )
        where TValue : IEquatable<TValue> => span.IndexOfAny( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int LastIndexOfAny<TValue>( scoped ref readonly ReadOnlySpan<TValue> span, SearchValues<TValue> value )
        where TValue : IEquatable<TValue> => span.LastIndexOfAny( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int LastIndexOfAnyExcept<TValue>( scoped ref readonly ReadOnlySpan<TValue> span, SearchValues<TValue> value )
        where TValue : IEquatable<TValue> => span.LastIndexOfAnyExcept( value );


    // https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7/#:~:text=also%20add%20a-,Vector256,-%3CT%3E
    public static bool Contains<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, TValue value )
        where TValue : IEquatable<TValue>
    {
        if ( Vector.IsHardwareAccelerated && Vector<TValue>.IsSupported && span.Length >= Vector<TValue>.Count )
        {
            Vector<TValue> source = Vector.Create( span );
            return Vector.EqualsAll( source, Vector.Create( value ) );
        }

        return MemoryExtensions.Contains( span, value );
    }


    public static bool Contains( this scoped ref readonly Span<char>         span, params ReadOnlySpan<char> value ) => MemoryExtensions.Contains( span, value, StringComparison.Ordinal );
    public static bool Contains( this scoped ref readonly ReadOnlySpan<char> span, params ReadOnlySpan<char> value ) => span.Contains( value, StringComparison.Ordinal );


    public static bool Contains<TValue>( this scoped ref readonly Span<TValue> span, scoped ref readonly TValue value, IEqualityComparer<TValue> comparer )
    {
        ReadOnlySpan<TValue> values = span;
        return values.Contains( in value, comparer );
    }
    public static bool Contains<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, scoped ref readonly TValue value, IEqualityComparer<TValue> comparer )
    {
        if ( span.IsEmpty ) { return false; }

        foreach ( TValue x in span )
        {
            if ( comparer.Equals( x, value ) ) { return true; }
        }

        return false;
    }


    public static bool Contains<TValue>( this scoped ref readonly Span<TValue> span, scoped ref readonly ReadOnlySpan<TValue> value, IEqualityComparer<TValue> comparer )
    {
        ReadOnlySpan<TValue> values = span;
        return values.Contains( in value, comparer );
    }
    public static bool Contains<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, scoped ref readonly ReadOnlySpan<TValue> value, IEqualityComparer<TValue> comparer )
    {
        Debug.Assert( comparer is not null );
        if ( span.IsEmpty || value.IsEmpty ) { return false; }

        if ( value.Length > span.Length ) { return false; }

        if ( value.Length == span.Length ) { return span.SequenceEqual( value, comparer ); }

        for ( int i = 0; i < span.Length || i + value.Length < span.Length; i++ )
        {
            ReadOnlySpan<TValue> raw = span.Slice( i, value.Length );
            if ( raw.SequenceEqual( value, comparer ) ) { return true; }
        }

        return false;
    }


    public static bool ContainsExact<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, params ReadOnlySpan<TValue> value )
        where TValue : IEquatable<TValue>
    {
        if ( value.Length > span.Length ) { return false; }

        if ( value.Length == span.Length ) { return span.SequenceEqual( value ); }

        for ( int i = 0; i < span.Length || i + value.Length < span.Length; i++ )
        {
            if ( span.Slice( i, value.Length ).SequenceEqual( value ) ) { return true; }
        }

        return false;
    }


    public static bool ContainsAll<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, params ReadOnlySpan<TValue> values )
        where TValue : IEquatable<TValue>
    {
        /*
        if ( Vector.IsHardwareAccelerated && span.Length >= Vector<TValue>.Count )
        {
            Vector<TValue> source = Vector.Create( span );
            if ( values.Length >= Vector<TValue>.Count ) { return Vector.EqualsAll( source, Vector.Create( values ) ); }

            using LinkSpan<Vector<TValue>> vectors = values.GetVectors();

            foreach ( Vector<TValue> vector in vectors.ReadOnlySpan )
            {
                if ( Vector.EqualsAll( source, vector ) ) { return true; }
            }
        }
        */

        foreach ( TValue c in values )
        {
            if ( span.Contains( c ) is false ) { return false; }
        }

        return true;
    }


    public static bool ContainsAny<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, params ReadOnlySpan<TValue> values )
        where TValue : IEquatable<TValue>
    {
        /*
        if ( Vector.IsHardwareAccelerated && span.Length >= Vector<TValue>.Count )
        {
            Vector<TValue> source = Vector.Create( span );
            if ( values.Length >= Vector<TValue>.Count ) { return Vector.EqualsAny( source, Vector.Create( values ) ); }

            using LinkSpan<Vector<TValue>> vectors = values.GetVectors();

            foreach ( Vector<TValue> vector in vectors.ReadOnlySpan )
            {
                if ( Vector.EqualsAny( source, vector ) ) { return true; }
            }
        }
        */

        foreach ( TValue c in values )
        {
            if ( span.Contains( c ) ) { return true; }
        }

        return false;
    }


    public static bool ContainsNone<TValue>( this scoped ref readonly ReadOnlySpan<TValue> span, params ReadOnlySpan<TValue> values )
        where TValue : IEquatable<TValue>
    {
        /*
        if ( Vector.IsHardwareAccelerated && span.Length >= Vector<TValue>.Count )
        {
            Vector<TValue> source = Vector.Create( span );
            if ( values.Length >= Vector<TValue>.Count ) { return Vector.EqualsAny( source, Vector.Create( values ) ); }

            using LinkSpan<Vector<TValue>> vectors = values.GetVectors();

            foreach ( Vector<TValue> vector in vectors.ReadOnlySpan )
            {
                if ( Vector.EqualsAny( source, vector ) ) { return false; }
            }
        }
        */

        foreach ( TValue c in values )
        {
            if ( span.Contains( c ) ) { return false; }
        }

        return true;
    }


    [Pure, MustDisposeResource]
    public static ReadOnlySpan<Vector<TValue>> GetVectors<TValue>( this scoped ref readonly ReadOnlySpan<TValue> values )
        where TValue : IEquatable<TValue>
    {
        Span<Vector<TValue>> vectors = GC.AllocateUninitializedArray<Vector<TValue>>( values.Length );
        for ( int i = 0; i < vectors.Length; i++ ) { vectors[i] = Vector.Create( values[i] ); }

        return vectors;
    }


    public static bool EndsWith<TValue>( scoped ref readonly Span<TValue> span, TValue value )
        where TValue : IEquatable<TValue> => span.IsEmpty is false && span[^1].Equals( value );
    public static bool EndsWith<TValue>( scoped ref readonly ReadOnlySpan<TValue> span, TValue value )
        where TValue : IEquatable<TValue> => span.IsEmpty is false && span[^1].Equals( value );
    public static bool EndsWith<TValue>( scoped ref readonly Span<TValue> span, params ReadOnlySpan<TValue> value )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> temp = span;
        return temp.EndsWith( value );
    }
    public static bool EndsWith<TValue>( scoped ref readonly ReadOnlySpan<TValue> span, params ReadOnlySpan<TValue> value )
        where TValue : IEquatable<TValue>
    {
        if ( span.IsEmpty ) { return false; }

        if ( span.Length < value.Length ) { return false; }

        ReadOnlySpan<TValue> temp = span.Slice( span.Length - value.Length, value.Length );

        for ( int i = 0; i < value.Length; i++ )
        {
            if ( temp[i].Equals( value[i] ) is false ) { return false; }
        }

        return true;
    }


    public static bool StartsWith<TValue>( scoped ref readonly Span<TValue> span, TValue value )
        where TValue : IEquatable<TValue> => span.IsEmpty is false && span[0].Equals( value );
    public static bool StartsWith<TValue>( scoped ref readonly ReadOnlySpan<TValue> span, TValue value )
        where TValue : IEquatable<TValue> => span.IsEmpty is false && span[0].Equals( value );
    public static bool StartsWith<TValue>( scoped ref readonly Span<TValue> span, params ReadOnlySpan<TValue> value )
        where TValue : IEquatable<TValue>
    {
        ReadOnlySpan<TValue> temp = span;
        return temp.StartsWith( value );
    }
    public static bool StartsWith<TValue>( scoped ref readonly ReadOnlySpan<TValue> span, params ReadOnlySpan<TValue> value )
        where TValue : IEquatable<TValue>
    {
        if ( span.IsEmpty ) { return false; }

        if ( span.Length < value.Length ) { return false; }

        ReadOnlySpan<TValue> temp = span[..value.Length];

        for ( int i = 0; i < value.Length; i++ )
        {
            if ( temp[i].Equals( value[i] ) is false ) { return false; }
        }

        return true;
    }
}
