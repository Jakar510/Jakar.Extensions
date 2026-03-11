namespace Jakar.Extensions;


public static class Buffers
{
    /// <summary> Resize the internal buffer either by doubling current buffer size or by adding <paramref name="additionalRequestedCapacity"/> to <see cref="Length"/> whichever is greater. </summary>
    /// <param name="additionalRequestedCapacity"> the requested new size of the buffer. </param>
    /// <param name="self"> </param>
    [Pure] [MustDisposeResource] public static Buffer<TValue> Grow<TValue>( [HandlesResourceDisposal] this ref readonly Buffer<TValue> self, uint additionalRequestedCapacity )
    {
        self.ThrowIfReadOnly();
        Guard.IsInRange(additionalRequestedCapacity, 1, int.MaxValue);
        int capacity = GetLength((uint)self.Capacity, additionalRequestedCapacity);

        using ( self )
        {
            // ReSharper disable once NotDisposedResource
            Buffer<TValue> buffer = new(capacity);
            self.Values.CopyTo(buffer.Span);
            return buffer;
        }
    }



    extension<TValue>( [MustDisposeResource] ref Buffer<TValue> self )
    {
        public void EnsureCapacity( int additionalRequestedCapacity )
        {
            uint capacity = (uint)additionalRequestedCapacity;
            if ( (uint)self.Length + capacity <= (uint)self.Capacity ) { return; }

            self = self.Grow(capacity);
        }


        public void Advance( int count )
        {
            self.EnsureCapacity(count);
            self.Length += count;
        }
        public Memory<TValue> GetMemory( int sizeHint = 0 )
        {
            self.EnsureCapacity(sizeHint);
            return self.Memory;
        }
        public Span<TValue> GetSpan( int sizeHint = 0 )
        {
            self.EnsureCapacity(sizeHint);
            return self.Next;
        }


        public void Insert( int start, TValue value, int count = 1 )
        {
            self.EnsureCapacity(count);
            self.InsertInternal(start, value, count);
        }
        public void Insert( int start, params ReadOnlySpan<TValue> values )
        {
            self.EnsureCapacity(values.Length);
            self.InsertInternal(start, values);
        }


        public void Add( TValue value )
        {
            self.EnsureCapacity(1);
            self.AddInternal(value);
        }
        public void Add( TValue value, int count )
        {
            self.EnsureCapacity(count);
            self.AddInternal(value, count);
        }
        public void Add( params ReadOnlySpan<TValue> values )
        {
            self.EnsureCapacity(values.Length);
            self.AddInternal(values);
        }
        public void AddRange( IEnumerable<TValue> enumerable )
        {
            using ArrayBuffer<TValue> buffer = ArrayBuffer<TValue>.Create(enumerable);

            self.EnsureCapacity(buffer.Length);
            self.AddInternal(buffer);
        }
    }



    public static int GetLength( in ulong capacity, in ulong requestedCapacity )
    {
        Guard.IsGreaterThan(requestedCapacity, capacity);
        Guard.IsLessThanOrEqualTo(requestedCapacity, int.MaxValue);

        ulong result = Math.Max(requestedCapacity, capacity * 2);
        return (int)Math.Min(result, int.MaxValue);
    }
}
