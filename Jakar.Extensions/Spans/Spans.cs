namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "OutParameterValueIsAlwaysDiscarded.Global" )]
public static partial class Spans
{
    [Pure] public static Span<byte>         AsBytes( this scoped ref readonly Span<char>         span ) => MemoryMarshal.AsBytes( span );
    [Pure] public static ReadOnlySpan<byte> AsBytes( this scoped ref readonly ReadOnlySpan<char> span ) => MemoryMarshal.AsBytes( span );


    [Pure]
    public static bool IsNullOrWhiteSpace( this scoped ref readonly Span<char> span )
    {
        ReadOnlySpan<char> value = span;
        return value.IsNullOrWhiteSpace();
    }

    [Pure]
    public static bool IsNullOrWhiteSpace( this scoped ref readonly ReadOnlySpan<char> span )
    {
        if ( span.IsEmpty ) { return true; }

        foreach ( char t in span )
        {
            if ( !char.IsWhiteSpace( t ) ) { return false; }
        }

        return true;
    }
    [Pure]
    public static bool IsNullOrWhiteSpace( this scoped ref readonly Buffer<char> value )
    {
        ReadOnlySpan<char> span = value.Span;
        return span.IsNullOrWhiteSpace();
    }
    [Pure]
    public static bool IsNullOrWhiteSpace( this scoped ref readonly ValueStringBuilder value )
    {
        ReadOnlySpan<char> span = value.Span;
        return span.IsNullOrWhiteSpace();
    }


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
    public static bool SequenceEqualAny( this scoped ref readonly ReadOnlySpan<string> left, params ReadOnlySpan<string> right )
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
    public static bool SequenceEqual( this scoped ref readonly ReadOnlySpan<string> left, params ReadOnlySpan<string> right )
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
    public static int LastIndexOf<T>( this scoped ref readonly ReadOnlySpan<T> value, T c, int endIndex )
        where T : IEquatable<T> =>
        endIndex < 0 || endIndex >= value.Length
            ? value.LastIndexOf( c )
            : value[..endIndex].LastIndexOf( c );


    [Pure] public static EnumerateEnumerator<T> Enumerate<T>( this scoped ref readonly ReadOnlySpan<T> span )                 => new(span);
    [Pure] public static EnumerateEnumerator<T> Enumerate<T>( this scoped ref readonly ReadOnlySpan<T> span, int startIndex ) => new(startIndex, span);


    [Pure, MustDisposeResource]
    public static LinkSpan<T> AsBuffer<T>( this scoped ref readonly Span<T> span, int length )
        where T : IEquatable<T>
    {
        Guard.IsLessThanOrEqualTo( length, span.Length, nameof(length) );
        return new LinkSpan<T>( span[..length] );
    }
    
    [Pure, MustDisposeResource]
    public static LinkSpan<T> AsBuffer<T>( this scoped ref readonly ReadOnlySpan<T> span, int length )
        where T : IEquatable<T>
    {
        Guard.IsLessThanOrEqualTo( length, span.Length, nameof(length) );
        return new LinkSpan<T>( span[..length] );
    }
    
    [Pure, MustDisposeResource]
    public static LinkSpan<T> AsBuffer<T>( this scoped ref readonly Span<T> span )
        where T : IEquatable<T> => new(span);
    
    [Pure, MustDisposeResource]
    public static LinkSpan<T> AsBuffer<T>( this scoped ref readonly ReadOnlySpan<T> span )
        where T : IEquatable<T> => new(span);


    public static void CopyTo<T>( this scoped ref readonly ReadOnlySpan<T> value, ref Span<T> buffer )
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );
        value.CopyTo( buffer );
    }
    public static void CopyTo<T>( this scoped ref readonly ReadOnlySpan<T> value, ref Span<T> buffer, T defaultValue )
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );
        value.CopyTo( buffer );

        buffer[value.Length..].Fill( defaultValue );

        // for ( int i = value.Length; i < buffer.Length; i++ ) { buffer[i] = defaultValue; }
    }


    public static bool TryCopyTo<T>( this scoped ref readonly ReadOnlySpan<T> value, ref Span<T> buffer )
    {
        Guard.IsInRangeFor( value.Length - 1, buffer, nameof(buffer) );
        return value.TryCopyTo( buffer );
    }
    public static bool TryCopyTo<T>( this scoped ref readonly ReadOnlySpan<T> value, ref Span<T> buffer, T defaultValue )
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
        Span<T> span = [arg0];

        return MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), span.Length );
    }
    public static Span<T> Create<T>( T arg0, T arg1 )
    {
        Span<T> span = [arg0, arg1];
        return MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), span.Length );
    }
    public static Span<T> Create<T>( T arg0, T arg1, T arg2 )
    {
        Span<T> span = [arg0, arg1, arg2];
        return MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), span.Length );
    }
    public static Span<T> Create<T>( T arg0, T arg1, T arg2, T arg3 )
    {
        Span<T> span = [arg0, arg1, arg2, arg3];
        return MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), span.Length );
    }
    public static Span<T> Create<T>( T arg0, T arg1, T arg2, T arg3, T arg4 )
    {
        Span<T> span = [arg0, arg1, arg2, arg3, arg4];
        return MemoryMarshal.CreateSpan( ref span.GetPinnableReference(), span.Length );
    }


    [Pure]
    public static EnumerateEnumerator<T> Enumerate<T, TNumber>( this scoped ref readonly ReadOnlySpan<T> span )
        where TNumber : struct, INumber<TNumber> => new(span);


    public static void QuickSort<T>( ref Span<T> span, Comparison<T> comparison )
    {
        if ( span.Length <= 1 ) { return; }

        QuickSort( ref span, 0, span.Length - 1, comparison );
    }
    public static void QuickSort<T>( ref Span<T> span, int left, int right, Comparison<T> comparison )
    {
        if ( left >= right ) { return; }

        int pivotIndex = Partition( ref span, left, right, comparison );
        QuickSort( ref span, left, pivotIndex - 1, comparison );
        QuickSort( ref span, pivotIndex       + 1, right, comparison );

        return;

        static int Partition( ref readonly Span<T> span, int left, int right, Comparison<T> comparison )
        {
            T   pivot = span[right];
            int i     = left - 1;

            for ( int j = left; j < right; j++ )
            {
                if ( comparison( span[j], pivot ) >= 0 ) { continue; }

                i++;
                Swap( ref span[i], ref span[j] );
            }

            Swap( ref span[i + 1], ref span[right] );
            return i + 1;
        }

        static void Swap( ref T left, ref T right ) => (left, right) = (right, left);
    }
}
