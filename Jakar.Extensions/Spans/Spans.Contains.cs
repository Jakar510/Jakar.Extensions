// Jakar.Extensions :: Jakar.Extensions
// 06/10/2022  10:17 AM

using System.Runtime.Intrinsics;



namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <see href="https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7/#vectorization"/>
///     </para>
/// </summary>
public static partial class Spans
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool ContainsAny<T>( scoped ref readonly ReadOnlySpan<T> span, SearchValues<T> value )
        where T : IEquatable<T> => span.ContainsAny( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool ContainsAnyExcept<T>( scoped ref readonly ReadOnlySpan<T> span, SearchValues<T> value )
        where T : IEquatable<T> => span.ContainsAnyExcept( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int IndexOfAnyExcept<T>( scoped ref readonly ReadOnlySpan<T> span, SearchValues<T> value )
        where T : IEquatable<T> => span.IndexOfAnyExcept( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int IndexOfAny<T>( scoped ref readonly ReadOnlySpan<T> span, SearchValues<T> value )
        where T : IEquatable<T> => span.IndexOfAny( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int LastIndexOfAny<T>( scoped ref readonly ReadOnlySpan<T> span, SearchValues<T> value )
        where T : IEquatable<T> => span.LastIndexOfAny( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int LastIndexOfAnyExcept<T>( scoped ref readonly ReadOnlySpan<T> span, SearchValues<T> value )
        where T : IEquatable<T> => span.LastIndexOfAnyExcept( value );


    // https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7/#:~:text=also%20add%20a-,Vector256,-%3CT%3E
    public static bool Contains<T>( this scoped ref readonly ReadOnlySpan<T> span, T value )
        where T : IEquatable<T>
    {
        if ( Vector.IsHardwareAccelerated is false || span.Length < Vector<T>.Count ) { return MemoryExtensions.Contains( span, value ); }

        Vector<T> target            = Vector.Create( value );
        ref T     current           = ref MemoryMarshal.GetReference( span );
        ref T     endMinusOneVector = ref Unsafe.Add( ref current, span.Length - Vector<T>.Count );

        do
        {
            if ( Vector.EqualsAny( target, Vector.LoadUnsafe( ref current ) ) ) { return true; }

            current = ref Unsafe.Add( ref current, Vector<T>.Count );
        }
        while ( Unsafe.IsAddressLessThan( ref current, ref endMinusOneVector ) );

        return Vector.EqualsAny( target, Vector.LoadUnsafe( ref endMinusOneVector ) );
    }


    // https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7/#:~:text=also%20add%20a-,Vector256,-%3CT%3E
    public static bool Contains<T>( this scoped ref readonly ReadOnlySpan<T> span, params ReadOnlySpan<T> values )
        where T : IEquatable<T>
    {
        if ( Vector.IsHardwareAccelerated is false || span.Length < Vector<T>.Count && values.Length < Vector<T>.Count ) { return span.ContainsAny( values ); }

        Vector<T> target            = Vector.Create( values );
        ref T     current           = ref MemoryMarshal.GetReference( span );
        ref T     endMinusOneVector = ref Unsafe.Add( ref current, span.Length - Vector<T>.Count );

        do
        {
            if ( Vector.EqualsAny( target, Vector.LoadUnsafe( ref current ) ) ) { return true; }

            current = ref Unsafe.Add( ref current, Vector<T>.Count );
        }
        while ( Unsafe.IsAddressLessThan( ref current, ref endMinusOneVector ) );

        return Vector.EqualsAny( target, Vector.LoadUnsafe( ref endMinusOneVector ) );
    }


    public static bool Contains( this scoped ref readonly Span<char>         span, params ReadOnlySpan<char> value ) => MemoryExtensions.Contains( span, value, StringComparison.Ordinal );
    public static bool Contains( this scoped ref readonly ReadOnlySpan<char> span, params ReadOnlySpan<char> value ) => span.Contains( value, StringComparison.Ordinal );


    public static bool Contains<T>( this scoped ref readonly Span<T> span, scoped ref readonly T value, IEqualityComparer<T> comparer )
    {
        ReadOnlySpan<T> values = span;
        return values.Contains( in value, comparer );
    }
    public static bool Contains<T>( this scoped ref readonly ReadOnlySpan<T> span, scoped ref readonly T value, IEqualityComparer<T> comparer )
    {
        if ( span.IsEmpty ) { return false; }

        foreach ( T x in span )
        {
            if ( comparer.Equals( x, value ) ) { return true; }
        }

        return false;
    }


    public static bool Contains<T>( this scoped ref readonly Span<T> span, scoped ref readonly ReadOnlySpan<T> value, IEqualityComparer<T> comparer )
    {
        ReadOnlySpan<T> values = span;
        return values.Contains( in value, comparer );
    }
    public static bool Contains<T>( this scoped ref readonly ReadOnlySpan<T> span, scoped ref readonly ReadOnlySpan<T> value, IEqualityComparer<T> comparer )
    {
        Debug.Assert( comparer is not null );
        if ( span.IsEmpty || value.IsEmpty ) { return false; }

        if ( value.Length > span.Length ) { return false; }

        if ( value.Length == span.Length ) { return span.SequenceEqual( value, comparer ); }

        for ( int i = 0; i < span.Length || i + value.Length < span.Length; i++ )
        {
            ReadOnlySpan<T> raw = span.Slice( i, value.Length );
            if ( raw.SequenceEqual( value, comparer ) ) { return true; }
        }

        return false;
    }


    public static bool ContainsExact<T>( scoped ref readonly ReadOnlySpan<T> span, params ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        if ( value.Length > span.Length ) { return false; }

        if ( value.Length == span.Length ) { return span.SequenceEqual( value ); }


        for ( int i = 0; i < span.Length || i + value.Length < span.Length; i++ )
        {
            if ( span.Slice( i, value.Length ).SequenceEqual( value ) ) { return true; }
        }

        return false;
    }


    public static bool ContainsAll<T>( this scoped ref readonly ReadOnlySpan<T> span, params ReadOnlySpan<T> values )
        where T : IEquatable<T>
    {
        if ( Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count )
        {
            Vector<T> source = Vector.Create( span );
            if ( values.Length >= Vector<T>.Count ) { return Vector.EqualsAll( source, Vector.Create( values ) ); }

            using LinkSpan<Vector<T>> vectors = values.GetVectors();

            foreach ( Vector<T> vector in vectors.ReadOnlySpan )
            {
                if ( Vector.EqualsAll( source, vector ) ) { return true; }
            }
        }

        foreach ( T c in values )
        {
            if ( span.Contains( c ) is false ) { return false; }
        }

        return true;
    }


    public static bool ContainsAny<T>( this scoped ref readonly ReadOnlySpan<T> span, params ReadOnlySpan<T> values )
        where T : IEquatable<T>
    {
        if ( Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count )
        {
            Vector<T> source = Vector.Create( span );
            if ( values.Length >= Vector<T>.Count ) { return Vector.EqualsAny( source, Vector.Create( values ) ); }

            using LinkSpan<Vector<T>> vectors = values.GetVectors();

            foreach ( Vector<T> vector in vectors.ReadOnlySpan )
            {
                if ( Vector.EqualsAny( source, vector ) ) { return true; }
            }
        }

        foreach ( T c in values )
        {
            if ( span.Contains( c ) ) { return true; }
        }

        return false;
    }


    public static bool ContainsNone<T>( this scoped ref readonly ReadOnlySpan<T> span, params ReadOnlySpan<T> values )
        where T : IEquatable<T>
    {
        if ( Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count )
        {
            Vector<T> source = Vector.Create( span );
            if ( values.Length >= Vector<T>.Count ) { return Vector.EqualsAny( source, Vector.Create( values ) ); }

            using LinkSpan<Vector<T>> vectors = values.GetVectors();

            foreach ( Vector<T> vector in vectors.ReadOnlySpan )
            {
                if ( Vector.EqualsAny( source, vector ) ) { return false; }
            }
        }

        foreach ( T c in values )
        {
            if ( span.Contains( c ) ) { return false; }
        }

        return true;
    }


    [Pure, MustDisposeResource]
    public static LinkSpan<Vector<T>> GetVectors<T>( this scoped ref readonly ReadOnlySpan<T> values )
        where T : IEquatable<T>
    {
        LinkSpan<Vector<T>> vectors = new(values.Length);
        for ( int i = 0; i < vectors.Length; i++ ) { vectors[i] = Vector.Create( values[i] ); }

        return vectors;
    }


    public static bool EndsWith<T>( scoped ref readonly Span<T> span, T value )
        where T : IEquatable<T> => span.IsEmpty is false && span[^1].Equals( value );
    public static bool EndsWith<T>( scoped ref readonly ReadOnlySpan<T> span, T value )
        where T : IEquatable<T> => span.IsEmpty is false && span[^1].Equals( value );
    public static bool EndsWith<T>( scoped ref readonly Span<T> span, params ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> temp = span;
        return temp.EndsWith( value );
    }
    public static bool EndsWith<T>( scoped ref readonly ReadOnlySpan<T> span, params ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        if ( span.IsEmpty ) { return false; }

        if ( span.Length < value.Length ) { return false; }

        ReadOnlySpan<T> temp = span.Slice( span.Length - value.Length, value.Length );

        for ( int i = 0; i < value.Length; i++ )
        {
            if ( temp[i].Equals( value[i] ) is false ) { return false; }
        }

        return true;
    }


    public static bool StartsWith<T>( scoped ref readonly Span<T> span, T value )
        where T : IEquatable<T> => span.IsEmpty is false && span[0].Equals( value );
    public static bool StartsWith<T>( scoped ref readonly ReadOnlySpan<T> span, T value )
        where T : IEquatable<T> => span.IsEmpty is false && span[0].Equals( value );
    public static bool StartsWith<T>( scoped ref readonly Span<T> span, params ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> temp = span;
        return temp.StartsWith( value );
    }
    public static bool StartsWith<T>( scoped ref readonly ReadOnlySpan<T> span, params ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        if ( span.IsEmpty ) { return false; }

        if ( span.Length < value.Length ) { return false; }

        ReadOnlySpan<T> temp = span[..value.Length];

        for ( int i = 0; i < value.Length; i++ )
        {
            if ( temp[i].Equals( value[i] ) is false ) { return false; }
        }

        return true;
    }
}
