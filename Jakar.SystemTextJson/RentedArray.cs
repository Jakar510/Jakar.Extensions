// Jakar.SystemTextJson
// 10/02/2026

namespace Jakar.Extensions;


/// <summary>
///     A read-only view of <see cref="Count"/> elements in an array rented from <see cref="ArrayPool{T}.Shared"/>.
///     <para> <see cref="Memory"/> and <see cref="Span"/> are exactly <see cref="Count"/> long; the rented array itself is never exposed. </para>
///     <para> Dispose exactly once when done (extra calls are no-ops). Don't keep <see cref="Memory"/>/<see cref="Span"/> past <see cref="Dispose"/>: the array goes back to the pool and is reused. Forgetting to dispose is safe; the array is simply garbage-collected. </para>
/// </summary>
/// <remarks> A class (not a struct) on purpose: copies of a struct owner could each return the same array, which corrupts the pool. Named <c> RentedArray </c> to stay clear of ZLinq's <c> PooledArray&lt;T&gt; </c>. </remarks>
[MustDisposeResource]
public sealed class RentedArray<T> : IMemoryOwner<T>
{
    private readonly bool _pooled;
    private          T[]? _array;


    public int  Count      { get; }
    public bool IsEmpty    => Count == 0;
    public bool IsDisposed => Volatile.Read(ref _array) is null;

    public Memory<T>         Memory     => new(Rented, 0, Count);
    public Span<T>           Span       => new(Rented, 0, Count);
    public ReadOnlySpan<T>   ReadOnly   => new(Rented, 0, Count);
    private T[]              Rented     => Volatile.Read(ref _array) ?? throw new ObjectDisposedException(nameof(RentedArray<T>));


    internal RentedArray( T[] array, int count, bool pooled )
    {
        Debug.Assert((uint)count <= (uint)array.Length);
        _array  = array;
        Count   = count;
        _pooled = pooled;
    }


    /// <summary> A new empty instance (not a shared singleton: disposing one must not affect another). </summary>
    [MustDisposeResource] public static RentedArray<T> CreateEmpty() => new([], 0, false);


    public Span<T>.Enumerator GetEnumerator() => Span.GetEnumerator();
    public T[]                ToArray()       => [.. Span];


    public void Dispose()
    {
        T[]? array = Interlocked.Exchange(ref _array, null); // idempotent and thread-safe: the array can never be returned twice
        if ( array is null || !_pooled ) { return; }

        RentedArrayBuilder<T>.Release(array, Count);
    }
}



/// <summary> Growable buffer over <see cref="ArrayPool{T}.Shared"/>. </summary>
/// <remarks>
///     Mutable struct: keep it in a local and never copy it. Don't use it in a <see langword="using"/> declaration (that makes the local read-only, so every mutating call
///     would act on a defensive copy); use try/finally.
/// </remarks>
internal struct RentedArrayBuilder<T>
{
    private const int MIN_CAPACITY = 16;

    private T[]? _array;
    private int  _count;


    public readonly int             Count => _count;
    public readonly ReadOnlySpan<T> Span  => new(_array, 0, _count);


    public void Add( T value )
    {
        T[]? array = _array;
        if ( array is null || _count == array.Length ) { array = Grow(); }

        array[_count++] = value;
    }


    /// <summary> Transfers ownership of the rented array to the returned <see cref="RentedArray{T}"/>. The builder is empty afterwards. </summary>
    public RentedArray<T> ToRentedArray()
    {
        T[]? array = _array;
        int  count = _count;
        _array = null;
        _count = 0;

        return array is null
                   ? RentedArray<T>.CreateEmpty()
                   : new RentedArray<T>(array, count, true);
    }


    /// <summary> Returns the array to the pool unless ownership was already transferred. Safe to call more than once. </summary>
    public void Dispose()
    {
        T[]? array = _array;
        _array = null;
        if ( array is not null ) { Release(array, _count); }

        _count = 0;
    }


    private T[] Grow()
    {
        T[]? old = _array;

        int newSize = old is null
                          ? MIN_CAPACITY
                          : (int)Math.Min((uint)old.Length * 2, (uint)Array.MaxLength);

        if ( old is not null && newSize <= old.Length ) { throw new OutOfMemoryException($"{nameof(RentedArrayBuilder<T>)} cannot grow beyond {Array.MaxLength} elements."); }

        T[] next = ArrayPool<T>.Shared.Rent(newSize);

        if ( old is not null )
        {
            old.AsSpan(0, _count).CopyTo(next);
            Release(old, _count);
        }

        return _array = next;
    }


    /// <summary> Clears only the slots that were written (cheaper than <c> clearArray: true </c>, which clears the whole rental), so the pool doesn't keep objects alive. </summary>
    internal static void Release( T[] array, int count )
    {
        if ( RuntimeHelpers.IsReferenceOrContainsReferences<T>() ) { array.AsSpan(0, count).Clear(); }

        ArrayPool<T>.Shared.Return(array);
    }
}
