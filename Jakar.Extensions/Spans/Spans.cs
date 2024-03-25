namespace Jakar.Extensions;



[SuppressMessage( "ReSharper", "OutParameterValueIsAlwaysDiscarded.Global" )]
public static partial class Spans
{
    [Pure] public static Span<byte>         AsBytes( this Span<char>         span ) => MemoryMarshal.AsBytes( span );
    [Pure] public static ReadOnlySpan<byte> AsBytes( this ReadOnlySpan<char> span ) => MemoryMarshal.AsBytes( span );


    [Pure]
    public static bool IsNullOrWhiteSpace( this Span<char> span )
    {
        ReadOnlySpan<char> value = span;
        return value.IsNullOrWhiteSpace();
    }

    [Pure]
    public static bool IsNullOrWhiteSpace( this ReadOnlySpan<char> span )
    {
        if ( span.IsEmpty ) { return true; }

        foreach ( char t in span )
        {
            if ( !char.IsWhiteSpace( t ) ) { return false; }
        }

        return true;
    }
    [Pure] public static bool IsNullOrWhiteSpace( this Buffer<char>       span ) => IsNullOrWhiteSpace( span.Span );
    [Pure] public static bool IsNullOrWhiteSpace( this ValueStringBuilder span ) => IsNullOrWhiteSpace( span.Span );


    [Pure]
    public static bool IsNullOrWhiteSpace( this ReadOnlyMemory<char> memory ) =>
        memory.IsEmpty ||
        Parallel.For( 0,
                      memory.Length,
                      ( i, state ) =>
                      {
                          if ( !char.IsWhiteSpace( memory.Span[i] ) ) { state.Stop(); }
                      } )
                .IsCompleted;
    [Pure]
    public static bool IsNullOrWhiteSpace( this Memory<char> memory ) =>
        memory.IsEmpty ||
        Parallel.For( 0,
                      memory.Length,
                      ( i, state ) =>
                      {
                          if ( !char.IsWhiteSpace( memory.Span[i] ) ) { state.Stop(); }
                      } )
                .IsCompleted;


    [Pure]
    public static bool SequenceEqualAny( this ReadOnlySpan<string> left, in ReadOnlySpan<string> right )
    {
        if ( left.Length != right.Length ) { return false; }

        string[] leftSpan  = ArrayPool<string>.Shared.Rent( left.Length );
        string[] rightSpan = ArrayPool<string>.Shared.Rent( right.Length );

        try
        {
            left.CopyTo( leftSpan );
            right.CopyTo( rightSpan );
            Array.Sort( leftSpan );
            Array.Sort( rightSpan );

            return SequenceEqual( leftSpan, rightSpan );
        }
        finally
        {
            ArrayPool<string>.Shared.Return( leftSpan );
            ArrayPool<string>.Shared.Return( rightSpan );
        }
    }

    [Pure]
    public static bool SequenceEqual( this ReadOnlySpan<string> left, in ReadOnlySpan<string> right )
    {
        if ( left.Length != right.Length ) { return false; }

        for ( int i = 0; i < left.Length; i++ )
        {
            ReadOnlySpan<char> x = left[i];
            ReadOnlySpan<char> y = right[i];

            if ( x.SequenceEqual( y ) ) { continue; }

            return false;
        }

        return true;
    }

    [Pure]
    public static int LastIndexOf<T>( this ReadOnlySpan<T> value, T c, int endIndex )
        where T : IEquatable<T> =>
        endIndex < 0 || endIndex >= value.Length
            ? value.LastIndexOf( c )
            : value[..endIndex].LastIndexOf( c );


    [Pure] public static EnumerateEnumerator<T> Enumerate<T>( this ReadOnlySpan<T> span, int index = 0 ) => new(span, index);


    [Pure]
    public static Span<T> AsSpan<T>( this Span<T> span, int length )
    {
        Guard.IsLessThanOrEqualTo( length, span.Length, nameof(length) );
        return MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), length );
    }
    [Pure]
    public static Span<T> AsSpan<T>( this ReadOnlySpan<T> span, int length )
    {
        Guard.IsLessThanOrEqualTo( length, span.Length, nameof(length) );
        return MemoryMarshal.CreateSpan( ref MemoryMarshal.GetReference( span ), length );
    }
    [Pure] public static Span<T>         AsSpan<T>( this         ReadOnlySpan<T> span ) => MemoryMarshal.CreateSpan( ref MemoryMarshal.GetReference( span ), span.Length );
    [Pure] public static Span<T>         AsSpan<T>( this         Span<T>         span ) => MemoryMarshal.CreateSpan( ref span.GetPinnableReference(),        span.Length );
    [Pure] public static ReadOnlySpan<T> AsReadOnlySpan<T>( this Span<T>         span ) => MemoryMarshal.CreateReadOnlySpan( ref span.GetPinnableReference(),        span.Length );
    [Pure] public static ReadOnlySpan<T> AsReadOnlySpan<T>( this ReadOnlySpan<T> span ) => MemoryMarshal.CreateReadOnlySpan( ref MemoryMarshal.GetReference( span ), span.Length );


    public static void CopyTo<T>( this ReadOnlySpan<T> value, ref Span<T> buffer )
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );
        value.CopyTo( buffer );
    }
    public static void CopyTo<T>( this ReadOnlySpan<T> value, ref Span<T> buffer, T defaultValue )
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );
        value.CopyTo( buffer );

        buffer[value.Length..].Fill( defaultValue );

        // for ( int i = value.Length; i < buffer.Length; i++ ) { buffer[i] = defaultValue; }
    }


    public static bool TryCopyTo<T>( this ReadOnlySpan<T> value, ref Span<T> buffer )
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );
        return value.TryCopyTo( buffer );
    }
    public static bool TryCopyTo<T>( this ReadOnlySpan<T> value, ref Span<T> buffer, T defaultValue )
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );

        if ( !value.TryCopyTo( buffer ) ) { return false; }

        if ( buffer.Length > value.Length ) { buffer[value.Length..].Fill( defaultValue ); }

        return true;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Span<T> CreateSpan<T>( int size ) => AsyncLinq.GetArray<T>( size );
    public static Span<T> CreateValue<T>( int size )
        where T : unmanaged
    {
        if ( size > 250 ) { return CreateSpan<T>( size ); }

        Span<T> span = stackalloc T[size];
        return MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), span.Length );
    }
    public static Span<T> Create<T>( T arg0 )
    {
    #if NETSTANDARD2_1
        Span<T> span = new T[1]
                       {
                           arg0
                       };
    #else
        Span<T> span = [arg0];

    #endif
        return MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), span.Length );
    }
    public static Span<T> Create<T>( T arg0, T arg1 )
    {
    #if NETSTANDARD2_1
        Span<T> span = new T[2]
                       {
                           arg0,
                           arg1
                       };
    #else
        Span<T> span = [arg0, arg1];

    #endif
        return MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), span.Length );
    }
    public static Span<T> Create<T>( T arg0, T arg1, T arg2 )
    {
    #if NETSTANDARD2_1
        Span<T> span = new T[3]
                       {
                           arg0,
                           arg1,
                           arg2
                       };
    #else
        Span<T> span = [arg0, arg1, arg2];

    #endif
        return MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), span.Length );
    }
    public static Span<T> Create<T>( T arg0, T arg1, T arg2, T arg3 )
    {
    #if NETSTANDARD2_1
        Span<T> span = new T[4]
                       {
                           arg0,
                           arg1,
                           arg2,
                           arg3
                       };
    #else
        Span<T> span = [arg0, arg1, arg2, arg3];

    #endif
        return MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), span.Length );
    }
    public static Span<T> Create<T>( T arg0, T arg1, T arg2, T arg3, T arg4 )
    {
    #if NETSTANDARD2_1
        Span<T> span = new T[5]
                       {
                           arg0,
                           arg1,
                           arg2,
                           arg3,
                           arg4
                       };
    #else
        Span<T> span = [arg0, arg1, arg2, arg3, arg4];

    #endif
        return MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), span.Length );
    }


#if NET7_0_OR_GREATER
    [Pure]
    public static EnumerateEnumerator<T, TNumber> Enumerate<T, TNumber>( this ReadOnlySpan<T> span )
        where TNumber : struct, INumber<TNumber> => Enumerate( span, TNumber.Zero );
    [Pure]
    public static EnumerateEnumerator<T, TNumber> Enumerate<T, TNumber>( this ReadOnlySpan<T> span, TNumber index )
        where TNumber : struct, INumber<TNumber> => new(span, index);
#endif
}
