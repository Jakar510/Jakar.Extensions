using System.Diagnostics.CodeAnalysis;
using Microsoft.Toolkit.HighPerformance.Enumerables;


#if NET6_0


// Jakar.Extensions :: Jakar.Mapper
// 06/09/2022  3:53 PM



namespace Jakar.Mapper;


/// <summary> A <see langword="ref"/> <see langword="struct"/> that iterates items from arbitrary memory locations. </summary>
public readonly ref struct PairDict
{
    /// <summary>
    /// The target <see cref="object"/> instance, if present.
    /// </summary>
    internal readonly object? instance;

    /// <summary>
    /// The initial offset within <see cref="instance"/>.
    /// </summary>
    internal readonly IntPtr offset;

    /// <summary>
    /// The distance between items in the sequence to enumerate.
    /// </summary>
    /// <remarks>The distance refers to <typeparamref name="Pair"/> items, not byte offset.</remarks>
    internal readonly int step;


    /// <summary>
    /// Gets the total available length for the sequence.
    /// </summary>
    public int Length { [MethodImpl(MethodImplOptions.AggressiveInlining)] get; }

    /// <summary>
    /// Gets the element at the specified zero-based index.
    /// </summary>
    /// <param name="index">The zero-based index of the element.</param>
    /// <returns>A reference to the element at the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException">
    /// Thrown when <paramref name="index"/> is invalid.
    /// </exception>
    public ref Pair this[ int index ]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ( (uint)index >= (uint)Length ) { throw new OutOfRangeException(nameof(index), index); }

        #if SPAN_RUNTIME_SUPPORT
                ref Pair r0 = ref MemoryMarshal.GetReference(this.Span);
        #else
            ref Pair r0 = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<Pair>(instance, this.offset);
        #endif
            nint     offset = (nint)(uint)index * (nint)(uint)step;
            ref Pair ri     = ref Unsafe.Add(ref r0, offset);

            return ref ri;
        }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="RefEnumerable{Pair}"/> struct.
    /// </summary>
    /// <param name="instance">The target <see cref="object"/> instance.</param>
    /// <param name="offset">The initial offset within <see paramref="instance"/>.</param>
    /// <param name="length">The number of items in the sequence.</param>
    /// <param name="step">The distance between items in the sequence to enumerate.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal PairDict( object? instance, IntPtr offset, int length, int step )
    {
        this.instance = instance;
        this.offset   = offset;
        Length        = length;
        this.step     = step;
    }


    [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] public Enumerator GetEnumerator() => new(instance, offset, Length, step);


    /// <summary>
    /// Clears the contents of the current <see cref="RefEnumerable{Pair}"/> instance.
    /// </summary>
    public void Clear()
    {
        ref Pair r0     = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<Pair>(instance, offset);
        int      length = Length;

        RefEnumerableHelper.Clear(ref r0, (nint)(uint)length, (nint)(uint)step);
    }

    /// <summary>
    /// Copies the contents of this <see cref="RefEnumerable{Pair}"/> into a destination <see cref="RefEnumerable{Pair}"/> instance.
    /// </summary>
    /// <param name="destination">The destination <see cref="RefEnumerable{Pair}"/> instance.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="destination"/> is shorter than the source <see cref="RefEnumerable{Pair}"/> instance.
    /// </exception>
    public void CopyTo( PairDict destination )
    {
        ref Pair sourceRef      = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<Pair>(instance,             offset);
        ref Pair destinationRef = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<Pair>(destination.instance, destination.offset);
        int      sourceLength   = Length, destinationLength = destination.Length;

        if ( (uint)destinationLength < (uint)sourceLength ) { Throw_DestinationTooShort(); }

        RefEnumerableHelper.CopyTo(ref sourceRef, ref destinationRef, (nint)(uint)sourceLength, (nint)(uint)step, (nint)(uint)destination.step);
    }

    /// <summary>
    /// Attempts to copy the current <see cref="RefEnumerable{Pair}"/> instance to a destination <see cref="RefEnumerable{Pair}"/>.
    /// </summary>
    /// <param name="destination">The target <see cref="RefEnumerable{Pair}"/> of the copy operation.</param>
    /// <returns>Whether or not the operation was successful.</returns>
    public bool TryCopyTo( RefEnumerable<Pair> destination )
    {
        int sourceLength = Length, destinationLength = destination.Length;

        if ( destinationLength >= sourceLength )
        {
            CopyTo(destination);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Copies the contents of this <see cref="RefEnumerable{Pair}"/> into a destination <see cref="Span{Pair}"/> instance.
    /// </summary>
    /// <param name="destination">The destination <see cref="Span{Pair}"/> instance.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="destination"/> is shorter than the source <see cref="RefEnumerable{Pair}"/> instance.
    /// </exception>
    public void CopyTo( Span<Pair> destination )
    {
        ref Pair sourceRef = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<Pair>(instance, offset);
        int      length    = Length;
        if ( (uint)destination.Length < (uint)length ) { Throw_DestinationTooShort(); }

        ref Pair destinationRef = ref destination.DangerousGetReference();

        RefEnumerableHelper.CopyTo(ref sourceRef, ref destinationRef, (nint)(uint)length, (nint)(uint)step);
    }

    /// <summary>
    /// Attempts to copy the current <see cref="RefEnumerable{Pair}"/> instance to a destination <see cref="Span{Pair}"/>.
    /// </summary>
    /// <param name="destination">The target <see cref="Span{Pair}"/> of the copy operation.</param>
    /// <returns>Whether or not the operation was successful.</returns>
    public bool TryCopyTo( Span<Pair> destination )
    {
    #if SPAN_RUNTIME_SUPPORT
            int length = this.Span.Length;
    #else
        int length = Length;
    #endif

        if ( destination.Length >= length )
        {
            CopyTo(destination);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Copies the contents of a source <see cref="ReadOnlySpan{Pair}"/> into the current <see cref="RefEnumerable{Pair}"/> instance.
    /// </summary>
    /// <param name="source">The source <see cref="ReadOnlySpan{Pair}"/> instance.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the current <see cref="RefEnumerable{Pair}"/> is shorter than the source <see cref="ReadOnlySpan{Pair}"/> instance.
    /// </exception>
    internal void CopyFrom( ReadOnlySpan<Pair> source )
    {
    #if SPAN_RUNTIME_SUPPORT
            if (this.Step == 1)
            {
                source.CopyTo(this.Span);
    
                return;
            }
    
            ref Pair destinationRef = ref this.Span.DangerousGetReference();
            int destinationLength = this.Span.Length;
    #else
        ref Pair destinationRef    = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<Pair>(instance, offset);
        int      destinationLength = Length;
    #endif
        ref Pair sourceRef    = ref source.DangerousGetReference();
        int      sourceLength = source.Length;

        if ( (uint)destinationLength < (uint)sourceLength ) { Throw_DestinationTooShort(); }

        RefEnumerableHelper.CopyFrom(ref sourceRef, ref destinationRef, (nint)(uint)sourceLength, (nint)(uint)step);
    }

    /// <summary>
    /// Attempts to copy the source <see cref="ReadOnlySpan{Pair}"/> into the current <see cref="RefEnumerable{Pair}"/> instance.
    /// </summary>
    /// <param name="source">The source <see cref="ReadOnlySpan{Pair}"/> instance.</param>
    /// <returns>Whether or not the operation was successful.</returns>
    public bool TryCopyFrom( ReadOnlySpan<Pair> source )
    {
    #if SPAN_RUNTIME_SUPPORT
            int length = this.Span.Length;
    #else
        int length = Length;
    #endif

        if ( length >= source.Length )
        {
            CopyFrom(source);

            return true;
        }

        return false;
    }

    /// <summary>
    /// Fills the elements of this <see cref="RefEnumerable{Pair}"/> with a specified value.
    /// </summary>
    /// <param name="value">The value to assign to each element of the <see cref="RefEnumerable{Pair}"/> instance.</param>
    public void Fill( Pair value )
    {
    #if SPAN_RUNTIME_SUPPORT
            if (this.Step == 1)
            {
                this.Span.Fill(value);
    
                return;
            }
    
            ref Pair r0 = ref this.Span.DangerousGetReference();
            int length = this.Span.Length;
    #else
        ref Pair r0     = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<Pair>(instance, offset);
        int      length = Length;
    #endif

        RefEnumerableHelper.Fill(ref r0, (nint)(uint)length, (nint)(uint)step, value);
    }


    /// <summary>
    /// Returns a <typeparamref name="Pair"/> array with the values in the target row.
    /// </summary>
    /// <returns>A <typeparamref name="Pair"/> array with the values in the target row.</returns>
    /// <remarks>
    /// This method will allocate a new <typeparamref name="Pair"/> array, so only
    /// use it if you really need to copy the target items in a new memory location.
    /// </remarks>
    [Pure]
    public Pair[] ToArray()
    {
        int length = Length;

        if ( length == 0 ) { return Array.Empty<Pair>(); }

        var array = new Pair[length];

        CopyTo(array);

        return array;
    }



    /// <summary>
    /// A custom enumerator type to traverse items within a <see cref="RefEnumerable{Pair}"/> instance.
    /// </summary>
    public ref struct Enumerator
    {
        private readonly object? _instance;
        private readonly IntPtr  _offset;
        private readonly int     _length;
        private readonly int     _step;
        private          int     _position;


        public readonly ref Pair Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                ref Pair r0 = ref RuntimeHelpers.GetObjectDataAtOffsetOrPointerReference<Pair>(_instance, _offset);

                // Here we just offset by shifting down as if we were traversing a 2D array with a single column, with the width of each row represented by the step, the height represented by the current position, and with only the first element of each row being inspected.
                // We can perform all the indexing operations in this type as nint, as the maximum offset is guaranteed never to exceed the maximum value, since on 32 bit architectures it's not possible to allocate that much memory anyway.
                nint     offset = (nint)(uint)_position * (nint)(uint)_step;
                ref Pair ri     = ref Unsafe.Add(ref r0, offset);

                return ref ri;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> struct.
        /// </summary>
        /// <param name="instance">The target <see cref="object"/> instance.</param>
        /// <param name="offset">The initial offset within <see paramref="instance"/>.</param>
        /// <param name="length">The number of items in the sequence.</param>
        /// <param name="step">The distance between items in the sequence to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator( object? instance, IntPtr offset, int length, int step )
        {
            _instance = instance;
            _offset   = offset;
            _length   = length;
            _step     = step;
            _position = -1;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool MoveNext() => ++_position < _length;
    }



    /// <summary>
    /// Throws an <see cref="ArgumentException"/> when the target span is too short.
    /// </summary>
    [DoesNotReturn]
    private static void Throw_DestinationTooShort() => throw new ArgumentException("The target span is too short to copy all the current items to");
}
#endif
