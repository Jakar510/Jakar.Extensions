﻿// Jakar.Extensions :: Jakar.Extensions
// 06/10/2022  10:17 AM

namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <see href="https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7/#vectorization"/>
///     </para>
/// </summary>
public static partial class Spans
{
#if NET8_0_OR_GREATER
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool ContainsAny<T>( scoped in ReadOnlySpan<T> span, SearchValues<T> value )
        where T : IEquatable<T> => span.ContainsAny( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool ContainsAnyExcept<T>( scoped in ReadOnlySpan<T> span, SearchValues<T> value )
        where T : IEquatable<T> => span.ContainsAnyExcept( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int IndexOfAnyExcept<T>( scoped in ReadOnlySpan<T> span, SearchValues<T> value )
        where T : IEquatable<T> => span.IndexOfAnyExcept( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int IndexOfAny<T>( scoped in ReadOnlySpan<T> span, SearchValues<T> value )
        where T : IEquatable<T> => span.IndexOfAny( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int LastIndexOfAny<T>( scoped in ReadOnlySpan<T> span, SearchValues<T> value )
        where T : IEquatable<T> => span.LastIndexOfAny( value );

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static int LastIndexOfAnyExcept<T>( scoped in ReadOnlySpan<T> span, SearchValues<T> value )
        where T : IEquatable<T> => span.LastIndexOfAnyExcept( value );
#endif


    public static bool Contains( this Span<char>         span, scoped in ReadOnlySpan<char> value ) => MemoryExtensions.Contains( span, value, StringComparison.Ordinal );
    public static bool Contains( this ReadOnlySpan<char> span, scoped in ReadOnlySpan<char> value ) => span.Contains( value, StringComparison.Ordinal );


    public static bool Contains<T>(

    #if NETSTANDARD2_1
        this
        #endif
            Span<T> span,
        T value
    )
        where T : IEquatable<T>
    {
    #if NETSTANDARD2_1
        foreach ( T item in span )
        {
            if ( item.Equals( value ) ) { return true; }
        }

        return false;
    #else
        return span.Contains( value );
    #endif
    }
    public static bool Contains<T>(
    #if NETSTANDARD2_1
        this
        #endif
            ReadOnlySpan<T> span,
        T value
    )
        where T : IEquatable<T>
    {
    #if NETSTANDARD2_1
        foreach ( T item in span )
        {
            if ( item.Equals( value ) ) { return true; }
        }

        return false;
    #else
        return span.Contains( value );
    #endif
    }


    public static bool Contains<T>( this Span<T> span, scoped in T value, IEqualityComparer<T> comparer )
    {
        ReadOnlySpan<T> temp = span;
        return temp.Contains( value, comparer );
    }
    public static bool Contains<T>( this ReadOnlySpan<T> span, scoped in T value, IEqualityComparer<T> comparer )
    {
        if ( span.IsEmpty ) { return false; }

        foreach ( T x in span )
        {
            if ( comparer.Equals( x, value ) ) { return true; }
        }

        return false;
    }


    public static bool Contains<T>( this Span<T> span, scoped in ReadOnlySpan<T> value, IEqualityComparer<T> comparer )
    {
        ReadOnlySpan<T> temp = span;
        return temp.Contains( value, comparer );
    }
    public static bool Contains<T>( this ReadOnlySpan<T> span, scoped in ReadOnlySpan<T> value, IEqualityComparer<T> comparer )
    {
        Debug.Assert( comparer is not null );
        if ( span.IsEmpty || value.IsEmpty ) { return false; }

        if ( value.Length > span.Length ) { return false; }

    #if NET6_0_OR_GREATER
        if ( value.Length == span.Length ) { return span.SequenceEqual( value, comparer ); }
    #endif

        for ( int i = 0; i < span.Length || i + value.Length < span.Length; i++ )
        {
            ReadOnlySpan<T> raw = span.Slice( i, value.Length );

        #if NET6_0_OR_GREATER
            if ( raw.SequenceEqual( value, comparer ) ) { return true; }
        #else
            for ( int j = 0; j < raw.Length; j++ )
            {
                if ( comparer.Equals( raw[j], value[j] ) ) { return true; }
            }
        #endif
        }

        return false;
    }


    public static bool Contains<T>( this ReadOnlySpan<T> span, scoped in ReadOnlySpan<T> value )
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

    public static bool ContainsAll<T>( this ReadOnlySpan<T> span, scoped in ReadOnlySpan<T> values )
        where T : IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( span.Contains( c ) is false ) { return false; }
        }

        return true;
    }

    public static bool ContainsAny<T>( this ReadOnlySpan<T> span, scoped in ReadOnlySpan<T> values )
        where T : IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( span.Contains( c ) ) { return true; }
        }

        return false;
    }

    public static bool ContainsAny( this ReadOnlySpan<char> span, scoped in ReadOnlySpan<char> values )
    {
        foreach ( char c in values )
        {
            if ( span.Contains( c ) ) { return true; }
        }

        return false;
    }

    public static bool ContainsNone<T>( this ReadOnlySpan<T> span, scoped in ReadOnlySpan<T> values )
        where T : IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( span.Contains( c ) ) { return false; }
        }

        return true;
    }

    public static bool ContainsNone( this ReadOnlySpan<char> span, scoped in ReadOnlySpan<char> values )
    {
        foreach ( char c in values )
        {
            if ( span.Contains( c ) ) { return false; }
        }

        return true;
    }

    public static bool EndsWith<T>( this Span<T> span, T value )
        where T : IEquatable<T> => span.IsEmpty is false && span[^1].Equals( value );
    public static bool EndsWith<T>( this ReadOnlySpan<T> span, T value )
        where T : IEquatable<T> => span.IsEmpty is false && span[^1].Equals( value );
    public static bool EndsWith<T>( this Span<T> span, in ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> temp = span;
        return temp.EndsWith( value );
    }
    public static bool EndsWith<T>( this ReadOnlySpan<T> span, in ReadOnlySpan<T> value )
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


    public static bool StartsWith<T>( this Span<T> span, T value )
        where T : IEquatable<T> => span.IsEmpty is false && span[0].Equals( value );
    public static bool StartsWith<T>( this ReadOnlySpan<T> span, T value )
        where T : IEquatable<T> => span.IsEmpty is false && span[0].Equals( value );
    public static bool StartsWith<T>( this Span<T> span, scoped in ReadOnlySpan<T> value )
        where T : IEquatable<T>
    {
        ReadOnlySpan<T> temp = span;
        return temp.StartsWith( value );
    }
    public static bool StartsWith<T>( this ReadOnlySpan<T> span, scoped in ReadOnlySpan<T> value )
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


    // TODO: prep for DotNet 7
    // https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7/#:~:text=also%20add%20a-,Vector256,-%3CT%3E
    // public static bool Contains<T>( this ReadOnlySpan<T> span, T value ) where T : struct
    // {
    //     if ( Vector.IsHardwareAccelerated && span.Length >= Vector<T>.Count )
    //     {
    //         ref T current = ref MemoryMarshal.GetReference(span);
    //
    //         if ( Vector256.IsHardwareAccelerated && span.Length >= Vector256<T>.Count )
    //         {
    //             Vector256<T> target = Vector256.Create(value);
    //             ref T        endMinusOneVector = ref Unsafe.Add(ref current, span.Length - Vector256<T>.Count);
    //
    //             do
    //             {
    //                 if ( Vector.EqualsAny(target, Vector256.LoadUnsafe(ref current)) ) { return true; }
    //
    //                 current = ref Unsafe.Add(ref current, Vector256<T>.Count);
    //             } while ( Unsafe.IsAddressLessThan(ref current, ref endMinusOneVector) );
    //
    //             if ( Vector256.EqualsAny(target, Vector256.LoadUnsafe(ref endMinusOneVector)) ) { return true; }
    //         }
    //         else
    //         {
    //             Vector128<T> target            = new Vector<T>(value).AsVector128();
    //             ref T        endMinusOneVector = ref Unsafe.Add(ref current, span.Length - Vector128<T>.Count);
    //
    //             do
    //             {
    //                 if ( Vector128.EqualsAny(target, Vector128.LoadUnsafe(ref current)) ) { return true; }
    //
    //                 current = ref Unsafe.Add(ref current, Vector128<T>.Count);
    //             } while ( Unsafe.IsAddressLessThan(ref current, ref endMinusOneVector) );
    //
    //             if ( Vector128.EqualsAny(target, Vector128.LoadUnsafe(ref endMinusOneVector)) ) { return true; }
    //         }
    //     }
    //
    //
    //     foreach ( T item in span )
    //     {
    //         if ( value.Equals(item) ) { return true; }
    //     }
    //
    //     return false;
    // }
}
