namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "OutParameterValueIsAlwaysDiscarded.Global")]
public static partial class Spans
{
    [Pure] public static Span<byte>         AsBytes( this scoped ref readonly Span<char>         span ) => MemoryMarshal.AsBytes(span);
    [Pure] public static ReadOnlySpan<byte> AsBytes( this scoped ref readonly ReadOnlySpan<char> span ) => MemoryMarshal.AsBytes(span);


    [Pure] public static bool IsNullOrWhiteSpace( this scoped ref readonly Span<char> span )
    {
        ReadOnlySpan<char> value = span;
        return value.IsNullOrWhiteSpace();
    }

    [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsNullOrWhiteSpace( this scoped ref readonly ReadOnlySpan<char> span )
    {
        if ( span.IsEmpty ) { return true; }

        int length = span.Length;

        // For very short spans, scalar loop is usually faster.
        if ( length < Vector<ushort>.Count )
        {
            for ( int s = 0; s < length; s++ )
            {
                char c = span[s];
                if ( IsAsciiWhiteSpace(c) ) { continue; }

                // If non-ascii, defer to full Unicode check
                if ( c > 127 )
                {
                    if ( !char.IsWhiteSpace(c) ) { return false; }
                }
                else { return false; }
            }

            return true;
        }

        // Cast char span to ushort span so numeric SIMD operations work on 16-bit lanes.
        ReadOnlySpan<ushort> uSpan     = MemoryMarshal.Cast<char, ushort>(span);
        int                  simdCount = Vector<ushort>.Count;

        // Pre-built vectors for comparisons
        Vector<ushort> vSpace        = new(' ');
        Vector<ushort> vTab          = new('\t');
        Vector<ushort> vCr           = new('\r');
        Vector<ushort> vLf           = new('\n');
        Vector<ushort> vFf           = new('\f');
        Vector<ushort> vVt           = new('\v');
        Vector<ushort> vAsciiMax     = new(127);
        ref ushort     baseRef       = ref MemoryMarshal.GetReference(uSpan);
        int            i             = 0;
        int            lastSimdStart = length - simdCount;

        // SIMD loop
        for ( ; i <= lastSimdStart; i += simdCount )
        {
            // load Vector<ushort> from the span (unaligned-safe)
            ref ushort     blockRef     = ref Unsafe.Add(ref baseRef, i);
            ref byte       blockAsBytes = ref Unsafe.As<ushort, byte>(ref blockRef);
            Vector<ushort> vector       = Unsafe.ReadUnaligned<Vector<ushort>>(ref blockAsBytes);
            Vector<ushort> isUnicode    = Vector.GreaterThan(vector, vAsciiMax); // If any lane > 127 then this block contains non-ASCII; do scalar Unicode-accurate check.

            if ( isUnicode.AnyTrue() )
            {
                // Scalar-check this block (Unicode-correct)
                int scalarEnd = i + simdCount;

                for ( int s = i; s < scalarEnd; s++ )
                {
                    char c = span[s];
                    if ( !char.IsWhiteSpace(c) ) { return false; }
                }

                continue; // continue with next SIMD block
            }

            // ASCII-only block: check against known ASCII whitespace characters
            Vector<ushort> eq = Vector.Equals(vector, vSpace);
            eq = Vector.BitwiseOr(eq, Vector.Equals(vector, vTab));
            eq = Vector.BitwiseOr(eq, Vector.Equals(vector, vCr));
            eq = Vector.BitwiseOr(eq, Vector.Equals(vector, vLf));
            eq = Vector.BitwiseOr(eq, Vector.Equals(vector, vFf));
            eq = Vector.BitwiseOr(eq, Vector.Equals(vector, vVt));

            // If any lane is zero in eq => that lane was not a matched whitespace
            if ( !eq.AllTrue() ) { return false; }
        }

        // Tail scalar check
        for ( ; i < length; i++ )
        {
            char c = span[i];

            if ( c <= 127 )
            {
                if ( !IsAsciiWhiteSpace(c) ) { return false; }
            }
            else
            {
                if ( !char.IsWhiteSpace(c) ) { return false; }
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] private static bool IsAsciiWhiteSpace( this char c ) => c == ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\f' || c == '\v';


    // Returns true if all lanes of 'mask' are non-zero (i.e., true)
    extension( ref readonly Vector<ushort> mask )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private bool AllTrue()
        {
            int count = Vector<ushort>.Count;

            for ( int j = 0; j < count; j++ )
            {
                ushort lane = mask[j];

                if ( lane == 0 ) // lane false
                {
                    return false;
                }
            }

            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] private bool AnyTrue()
        {
            int count = Vector<ushort>.Count;

            for ( int j = 0; j < count; j++ )
            {
                ushort lane = mask[j];
                if ( lane != 0 ) { return true; }
            }

            return false;
        }
    }

    // Returns true if any lane of 'mask' is non-zero


    [Pure] public static bool IsNullOrWhiteSpace( this scoped ref readonly Buffer<char> value )
    {
        ReadOnlySpan<char> span = value.Span;
        return span.IsNullOrWhiteSpace();
    }

    [Pure] public static bool IsNullOrWhiteSpace( this scoped ref readonly ValueStringBuilder value )
    {
        ReadOnlySpan<char> span = value.Span;
        return span.IsNullOrWhiteSpace();
    }


    [Pure] public static bool IsNullOrWhiteSpace( this ReadOnlyMemory<char> memory ) =>
        memory.IsEmpty ||
        Parallel.For(0,
                     memory.Length,
                     ( i, state ) =>
                     {
                         if ( !char.IsWhiteSpace(memory.Span[i]) ) { state.Stop(); }
                     })
                .IsCompleted;

    [Pure] public static bool IsNullOrWhiteSpace( this Memory<char> memory ) =>
        memory.IsEmpty ||
        Parallel.For(0,
                     memory.Length,
                     ( i, state ) =>
                     {
                         if ( !char.IsWhiteSpace(memory.Span[i]) ) { state.Stop(); }
                     })
                .IsCompleted;


    extension( scoped ref readonly ReadOnlySpan<string> left )
    {
        [Pure] public bool SequenceEqualAny( params ReadOnlySpan<string> right )
        {
            if ( left.Length != right.Length ) { return false; }

            string[] leftSpan  = ArrayPool<string>.Shared.Rent(left.Length);
            string[] rightSpan = ArrayPool<string>.Shared.Rent(right.Length);

            try
            {
                left.CopyTo(leftSpan);
                right.CopyTo(rightSpan);
                Array.Sort(leftSpan);
                Array.Sort(rightSpan);

                return leftSpan.SequenceEqual(rightSpan);
            }
            finally
            {
                ArrayPool<string>.Shared.Return(leftSpan);
                ArrayPool<string>.Shared.Return(rightSpan);
            }
        }
        [Pure] public bool SequenceEqual( params ReadOnlySpan<string> right )
        {
            if ( left.Length != right.Length ) { return false; }

            for ( int i = 0; i < left.Length; i++ )
            {
                ReadOnlySpan<char> x = left[i];
                ReadOnlySpan<char> y = right[i];

                if ( x.SequenceEqual(y) ) { continue; }

                return false;
            }

            return true;
        }
    }



    [Pure] public static int LastIndexOf<TValue>( this scoped ref readonly ReadOnlySpan<TValue> value, TValue c, int endIndex )
        where TValue : IEquatable<TValue> =>
        endIndex < 0 || endIndex >= value.Length
            ? value.LastIndexOf(c)
            : value[..endIndex]
               .LastIndexOf(c);


    extension<TValue>( scoped ref readonly ReadOnlySpan<TValue> span )
    {
        [Pure] public EnumerateEnumerator<TValue> Enumerate()                 => new(span);
        [Pure] public EnumerateEnumerator<TValue> Enumerate( int startIndex ) => new(startIndex, span);
        public void CopyTo( ref Span<TValue> buffer )
        {
            Guard.IsInRangeFor(span.Length - 1, buffer, nameof(buffer));
            span.CopyTo(buffer);
        }
        public void CopyTo( ref Span<TValue> buffer, TValue defaultValue )
        {
            Guard.IsInRangeFor(span.Length - 1, buffer, nameof(buffer));
            span.CopyTo(buffer);

            buffer[span.Length..]
               .Fill(defaultValue);

            // for ( int i = value.Length; i < buffer.Length; i++ ) { buffer[i] = defaultValue; }
        }
        public bool TryCopyTo( ref Span<TValue> buffer )
        {
            Guard.IsInRangeFor(span.Length - 1, buffer, nameof(buffer));
            return span.TryCopyTo(buffer);
        }
        public bool TryCopyTo( ref Span<TValue> buffer, TValue defaultValue )
        {
            Guard.IsInRangeFor(span.Length - 1, buffer, nameof(buffer));

            if ( !span.TryCopyTo(buffer) ) { return false; }

            if ( buffer.Length > span.Length )
            {
                buffer[span.Length..]
                   .Fill(defaultValue);
            }

            return true;
        }
    }



    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Span<TValue> CreateSpan<TValue>( int size ) => size.GetArray<TValue>();

    public static Span<TValue> Create<TValue>( int size )
        where TValue : unmanaged
    {
        if ( size > 250 ) { return CreateSpan<TValue>(size); }

        Span<TValue> span = stackalloc TValue[size];
        return MemoryMarshal.CreateSpan(ref span.GetPinnableReference(), span.Length);
    }

    public static Span<TValue> Create<TValue>( TValue arg0 )
    {
        Span<TValue> span = [arg0];

        return MemoryMarshal.CreateSpan(ref span.GetPinnableReference(), span.Length);
    }

    public static Span<TValue> Create<TValue>( TValue arg0, TValue arg1 )
    {
        Span<TValue> span = [arg0, arg1];
        return MemoryMarshal.CreateSpan(ref span.GetPinnableReference(), span.Length);
    }

    public static Span<TValue> Create<TValue>( TValue arg0, TValue arg1, TValue arg2 )
    {
        Span<TValue> span = [arg0, arg1, arg2];
        return MemoryMarshal.CreateSpan(ref span.GetPinnableReference(), span.Length);
    }

    public static Span<TValue> Create<TValue>( TValue arg0, TValue arg1, TValue arg2, TValue arg3 )
    {
        Span<TValue> span = [arg0, arg1, arg2, arg3];
        return MemoryMarshal.CreateSpan(ref span.GetPinnableReference(), span.Length);
    }

    public static Span<TValue> Create<TValue>( TValue arg0, TValue arg1, TValue arg2, TValue arg3, TValue arg4 )
    {
        Span<TValue> span = [arg0, arg1, arg2, arg3, arg4];
        return MemoryMarshal.CreateSpan(ref span.GetPinnableReference(), span.Length);
    }


    [Pure] public static EnumerateEnumerator<TValue> Enumerate<TValue, TNumber>( this scoped ref readonly ReadOnlySpan<TValue> span )
        where TNumber : struct, INumber<TNumber> => new(span);


    public static void QuickSort<TValue>( ref Span<TValue> span, Comparison<TValue> comparison )
    {
        if ( span.Length <= 1 ) { return; }

        QuickSort(ref span, 0, span.Length - 1, comparison);
    }

    public static void QuickSort<TValue>( ref Span<TValue> span, int left, int right, Comparison<TValue> comparison )
    {
        if ( left >= right ) { return; }

        int pivotIndex = partition(ref span, left, right, comparison);
        QuickSort(ref span, left, pivotIndex - 1, comparison);
        QuickSort(ref span, pivotIndex       + 1, right, comparison);

        return;

        static int partition( ref readonly Span<TValue> span, int left, int right, Comparison<TValue> comparison )
        {
            TValue pivot = span[right];
            int    i     = left - 1;

            for ( int j = left; j < right; j++ )
            {
                if ( comparison(span[j], pivot) >= 0 ) { continue; }

                i++;
                swap(ref span[i], ref span[j]);
            }

            swap(ref span[i + 1], ref span[right]);
            return i + 1;
        }

        static void swap( ref TValue left, ref TValue right ) => ( left, right ) = ( right, left );
    }
}
