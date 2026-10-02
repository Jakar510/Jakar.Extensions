namespace Jakar.Extensions;


public static class Buffers
{
    /// <summary> Returns a buffer with room for at least <paramref name="additionalRequestedCapacity"/> more elements after its length (doubling when that is larger), keeping the written elements. </summary>
    /// <remarks> <paramref name="self"/>'s array is returned to the pool; use the returned buffer from now on. Prefer <see cref="EnsureCapacity{TValue}"/>, which grows in place. </remarks>
    /// <param name="additionalRequestedCapacity"> the number of elements needed after the current length. </param>
    /// <param name="self"> </param>
    [Pure] [MustDisposeResource] public static Buffer<TValue> Grow<TValue>( [HandlesResourceDisposal] this ref readonly Buffer<TValue> self, uint additionalRequestedCapacity )
    {
        Guard.IsInRange(additionalRequestedCapacity, 1, int.MaxValue);
        Buffer<TValue> buffer = self;
        buffer.Grow((int)additionalRequestedCapacity);
        return buffer;
    }



    extension<TValue>( [MustDisposeResource] ref Buffer<TValue> self )
    {
        /// <summary> Ensures room for <paramref name="additionalRequestedCapacity"/> more elements after <see cref="Buffer{TValue}.Length"/>, growing in place. </summary>
        public void EnsureCapacity( int additionalRequestedCapacity ) => self.EnsureCapacityFor(additionalRequestedCapacity);


        public void Advance( int count )
        {
            self.EnsureCapacityFor(count);
            self.Length += count;
        }
        /// <summary> Free memory after <see cref="Buffer{TValue}.Length"/> with room for at least <paramref name="sizeHint"/> (minimum 1) elements. </summary>
        public Memory<TValue> GetMemory( int sizeHint = 0 )
        {
            self.EnsureCapacityFor(Math.Max(sizeHint, 1));
            return self.FreeMemory;
        }
        /// <summary> Free span after <see cref="Buffer{TValue}.Length"/> with room for at least <paramref name="sizeHint"/> (minimum 1) elements. </summary>
        public Span<TValue> GetSpan( int sizeHint = 0 )
        {
            self.EnsureCapacityFor(Math.Max(sizeHint, 1));
            return self.Next;
        }


        public void Insert( int start, TValue value, int count = 1 )
        {
            self.EnsureCapacityFor(count);
            self.InsertInternal(start, value, count);
        }
        public void Insert( int start, params ReadOnlySpan<TValue> values )
        {
            self.EnsureCapacityFor(values.Length);
            self.InsertInternal(start, values);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Add( TValue value ) => self.Append(value);
        public void Add( TValue value, int count )
        {
            self.EnsureCapacityFor(count);
            self.AddInternal(value, count);
        }
        public void Add( params ReadOnlySpan<TValue> values )
        {
            self.EnsureCapacityFor(values.Length);
            self.AddInternal(values);
        }
        public void AddRange( IEnumerable<TValue> enumerable ) => self.AddRangeInternal(enumerable);
    }



    public static int GetLength( in ulong capacity, in ulong requestedCapacity )
    {
        Guard.IsGreaterThan(requestedCapacity, capacity);
        Guard.IsLessThanOrEqualTo(requestedCapacity, int.MaxValue);

        ulong result = Math.Max(requestedCapacity, capacity * 2);
        return (int)Math.Min(result, int.MaxValue);
    }
}
