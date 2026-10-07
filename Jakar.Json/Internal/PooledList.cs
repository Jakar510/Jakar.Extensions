// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary> Scratch storage for elements while a collection is being read: pooled, so the final collection is allocated once, at its exact size (SPEC.md §6). </summary>
internal ref struct JsonPooledList<T>
{
    private T[]? __array;
    private int  __count;


    public readonly int             Count => __count;
    public readonly ReadOnlySpan<T> Span  => __array.AsSpan(0, __count);

    /// <summary> Mutable view (for sorting in place). </summary>
    public readonly Span<T> Items => __array.AsSpan(0, __count);


    public JsonPooledList( int capacity ) => __array = capacity > 0
                                                           ? ArrayPool<T>.Shared.Rent(capacity)
                                                           : null;


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Add( T item )
    {
        T[]? array = __array;

        if ( array is null || __count == array.Length ) { array = Grow(); }

        array[__count++] = item;
    }

    [MethodImpl(MethodImplOptions.NoInlining)] private T[] Grow()
    {
        T[]? old = __array;

        T[] larger = ArrayPool<T>.Shared.Rent(old is null
                                                  ? 16
                                                  : old.Length * 2);

        if ( old is not null )
        {
            old.AsSpan(0, __count).CopyTo(larger);
            ArrayPool<T>.Shared.Return(old, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        }

        return __array = larger;
    }

    public T[] ToArray() => __count == 0
                                ? []
                                : Span.ToArray();

    public void Dispose()
    {
        T[]? array = __array;
        __array = null;
        __count = 0;
        if ( array is not null ) { ArrayPool<T>.Shared.Return(array, RuntimeHelpers.IsReferenceOrContainsReferences<T>()); }
    }
}
