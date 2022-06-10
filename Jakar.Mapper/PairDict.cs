using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Toolkit.HighPerformance.Enumerables;


#if NET6_0


// Jakar.Extensions :: Jakar.Mapper
// 06/09/2022  3:53 PM



namespace Jakar.Mapper;


/// <summary> A <see langword="ref"/> <see langword="struct"/> that iterates items from arbitrary memory locations. </summary>
public readonly ref struct PairDict
{
    private readonly Pair[] _array;
    public           int    Length => _array.Length;


    public ref Pair this[ in ReadOnlySpan<char> name ]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref this[IndexOf(name)];
    }
    public ref Pair this[ int index ]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if ( (uint)index >= (uint)Length ) { throw new OutOfRangeException(nameof(index), index); }

            return ref _array[index];
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal PairDict( ReadOnlySpan<Pair> source ) : this(source.Length)
    {
        foreach ( ReadOnlySpanEnumerable<Pair>.Item pair in source.Enumerate() ) { _array[pair.Index] = pair.Value; }
    }
    internal PairDict( int minSize ) => _array = ArrayPool<Pair>.Shared.Rent(minSize);
    public void Dispose() => ArrayPool<Pair>.Shared.Return(_array, true);


    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int IndexOf( in ReadOnlySpan<char> name )
    {
        foreach ( SpanEnumerable<Pair>.Item pair in _array.Enumerate() )
        {
            if ( pair.Value.Key == name ) { return pair.Index; }
        }

        throw new KeyNotFoundException(name.ToString());
    }


    [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] public ReadOnlySpanEnumerable<Pair> GetEnumerator() => new(_array);


    /// <summary>
    /// Copies the contents of this <see cref="RefEnumerable{Pair}"/> into a destination <see cref="Span{Pair}"/> instance.
    /// </summary>
    /// <param name="destination">The destination <see cref="Span{Pair}"/> instance.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="destination"/> is shorter than the source <see cref="RefEnumerable{Pair}"/> instance.
    /// </exception>
    public void CopyTo( in Span<Pair> destination ) => _array.CopyTo(destination);


    /// <summary>
    /// Attempts to copy the current <see cref="RefEnumerable{Pair}"/> instance to a destination <see cref="Span{Pair}"/>.
    /// </summary>
    /// <param name="destination">The target <see cref="Span{Pair}"/> of the copy operation.</param>
    /// <returns>Whether or not the operation was successful.</returns>
    public bool TryCopyTo( in Span<Pair> destination )
    {
        int length = Length;

        if ( destination.Length < length ) { return false; }

        CopyTo(destination);
        return true;
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
    public Pair[] ToArray() => _array.ToArray();
}
#endif
