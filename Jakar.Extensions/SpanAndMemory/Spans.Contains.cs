// Jakar.Extensions :: Jakar.Extensions
// 06/10/2022  10:17 AM

namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <see href="https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7/#vectorization"/>
///     </para>
/// </summary>
public static partial class Spans
{
    public static bool Contains( this Span<char>         span, ReadOnlySpan<char> value ) => MemoryExtensions.Contains( span, value, StringComparison.Ordinal );
    public static bool Contains( this ReadOnlySpan<char> span, ReadOnlySpan<char> value ) => span.Contains( value, StringComparison.Ordinal );


    public static bool Contains<T>(

    #if NETSTANDARD2_1
        this
        #endif
            Span<T> span,
        T value
    ) where T : IEquatable<T>
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
    ) where T : IEquatable<T>
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

    public static bool Contains<T>( this Span<T> span, ReadOnlySpan<T> value ) where T : IEquatable<T>
    {
        if ( value.Length > span.Length ) { return false; }

        if ( value.Length == span.Length ) { return span.SequenceEqual( value ); }


        for ( int i = 0; i < span.Length || i + value.Length < span.Length; i++ )
        {
            if ( span.Slice( i, value.Length )
                     .SequenceEqual( value ) ) { return true; }
        }

        return false;
    }

    public static bool Contains<T>( this ReadOnlySpan<T> span, ReadOnlySpan<T> value ) where T : IEquatable<T>
    {
        if ( value.Length > span.Length ) { return false; }

        if ( value.Length == span.Length ) { return span.SequenceEqual( value ); }


        for ( int i = 0; i < span.Length || i + value.Length < span.Length; i++ )
        {
            if ( span.Slice( i, value.Length )
                     .SequenceEqual( value ) ) { return true; }
        }

        return false;
    }


    public static bool ContainsAll<T>( this Span<T> span, ReadOnlySpan<T> values ) where T : IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( !span.Contains( c ) ) { return false; }
        }

        return true;
    }

    public static bool ContainsAll<T>( this ReadOnlySpan<T> span, ReadOnlySpan<T> values ) where T : IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( !span.Contains( c ) ) { return false; }
        }

        return true;
    }


    public static bool ContainsAny<T>( this Span<T> span, ReadOnlySpan<T> values ) where T : IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( span.Contains( c ) ) { return true; }
        }

        return false;
    }

    public static bool ContainsAny<T>( this ReadOnlySpan<T> span, ReadOnlySpan<T> values ) where T : IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( span.Contains( c ) ) { return true; }
        }

        return false;
    }


    public static bool ContainsAny( this Span<char> span, ReadOnlySpan<char> values )
    {
        foreach ( char c in values )
        {
            if ( span.Contains( c ) ) { return true; }
        }

        return false;
    }

    public static bool ContainsAny( this ReadOnlySpan<char> span, ReadOnlySpan<char> values )
    {
        foreach ( char c in values )
        {
            if ( span.Contains( c ) ) { return true; }
        }

        return false;
    }


    public static bool ContainsNone<T>( this ReadOnlySpan<T> span, ReadOnlySpan<T> values ) where T : IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( span.Contains( c ) ) { return false; }
        }

        return true;
    }

    public static bool ContainsNone<T>( this Span<T> span, ReadOnlySpan<T> values ) where T : IEquatable<T>
    {
        foreach ( T c in values )
        {
            if ( span.Contains( c ) ) { return false; }
        }

        return true;
    }


    public static bool ContainsNone( this Span<char> span, ReadOnlySpan<char> values )
    {
        foreach ( char c in values )
        {
            if ( span.Contains( c ) ) { return false; }
        }

        return true;
    }

    public static bool ContainsNone( this ReadOnlySpan<char> span, ReadOnlySpan<char> values )
    {
        foreach ( char c in values )
        {
            if ( span.Contains( c ) ) { return false; }
        }

        return true;
    }


    public static bool EndsWith<T>( this Span<T> span, T value ) where T : IEquatable<T> => !span.IsEmpty && span[^1]
                                                                                               .Equals( value );

    public static bool EndsWith<T>( this ReadOnlySpan<T> span, T value ) where T : IEquatable<T> => !span.IsEmpty && span[^1]
                                                                                                       .Equals( value );


    public static bool StartsWith<T>( this Span<T> span, T value ) where T : IEquatable<T> => !span.IsEmpty && span[0]
                                                                                                 .Equals( value );

    public static bool StartsWith<T>( this ReadOnlySpan<T> span, T value ) where T : IEquatable<T> => !span.IsEmpty && span[0]
                                                                                                         .Equals( value );

    public static int Count<T>( this Span<T> span, T value ) where T : IEquatable<T>
    {
        int result = 0;

        foreach ( T v in span )
        {
            if ( v.Equals( value ) ) { result++; }
        }

        return result;
    }

    public static int Count<T>( this ReadOnlySpan<T> span, T value ) where T : IEquatable<T>
    {
        int result = 0;

        foreach ( T v in span )
        {
            if ( v.Equals( value ) ) { result++; }
        }

        return result;
    }
    public static ReadOnlySpan<T> Create<T>( T arg0 ) where T : unmanaged
    {
        Span<T> span = stackalloc T[1];
        span[0] = arg0;
        return MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), span.Length );
    }
    public static ReadOnlySpan<T> Create<T>( T arg0, T arg1 ) where T : unmanaged
    {
        Span<T> span = stackalloc T[2];
        span[0] = arg0;
        span[1] = arg1;
        return MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), span.Length );
    }
    public static ReadOnlySpan<T> Create<T>( T arg0, T arg1, T arg2 ) where T : unmanaged
    {
        Span<T> span = stackalloc T[3];
        span[0] = arg0;
        span[1] = arg1;
        span[2] = arg2;
        return MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), span.Length );
    }
    public static ReadOnlySpan<T> Create<T>( T arg0, T arg1, T arg2, T arg3 ) where T : unmanaged
    {
        Span<T> span = stackalloc T[4];
        span[0] = arg0;
        span[1] = arg1;
        span[2] = arg2;
        span[3] = arg3;
        return MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), span.Length );
    }
    public static ReadOnlySpan<T> Create<T>( T arg0, T arg1, T arg2, T arg3, T arg4 ) where T : unmanaged
    {
        Span<T> span = stackalloc T[5];
        span[0] = arg0;
        span[1] = arg1;
        span[2] = arg2;
        span[3] = arg3;
        span[4] = arg4;
        return MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(), span.Length );
    }


    // TODO: prep for DotNet 7
    // https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7/#:~:text=also%20add%20a-,Vector256,-%3CT%3E
    // public static bool Contains<TValue>( this ReadOnlySpan<TValue> span, TValue value ) where TValue : struct
    // {
    //     if ( Vector.IsHardwareAccelerated && span.Length >= Vector<TValue>.Count )
    //     {
    //         ref TValue current = ref MemoryMarshal.GetReference(span);
    //
    //         if ( Vector256.IsHardwareAccelerated && span.Length >= Vector256<TValue>.Count )
    //         {
    //             Vector256<TValue> target = Vector256.Create(value);
    //             ref TValue        endMinusOneVector = ref Unsafe.Add(ref current, span.Length - Vector256<TValue>.Count);
    //
    //             do
    //             {
    //                 if ( Vector.EqualsAny(target, Vector256.LoadUnsafe(ref current)) ) { return true; }
    //
    //                 current = ref Unsafe.Add(ref current, Vector256<TValue>.Count);
    //             } while ( Unsafe.IsAddressLessThan(ref current, ref endMinusOneVector) );
    //
    //             if ( Vector256.EqualsAny(target, Vector256.LoadUnsafe(ref endMinusOneVector)) ) { return true; }
    //         }
    //         else
    //         {
    //             Vector128<TValue> target            = new Vector<TValue>(value).AsVector128();
    //             ref TValue        endMinusOneVector = ref Unsafe.Add(ref current, span.Length - Vector128<TValue>.Count);
    //
    //             do
    //             {
    //                 if ( Vector128.EqualsAny(target, Vector128.LoadUnsafe(ref current)) ) { return true; }
    //
    //                 current = ref Unsafe.Add(ref current, Vector128<TValue>.Count);
    //             } while ( Unsafe.IsAddressLessThan(ref current, ref endMinusOneVector) );
    //
    //             if ( Vector128.EqualsAny(target, Vector128.LoadUnsafe(ref endMinusOneVector)) ) { return true; }
    //         }
    //     }
    //
    //
    //     foreach ( TValue item in span )
    //     {
    //         if ( value.Equals(item) ) { return true; }
    //     }
    //
    //     return false;
    // }
}
