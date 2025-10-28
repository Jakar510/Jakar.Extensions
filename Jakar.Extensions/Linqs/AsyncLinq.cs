namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <see cref="Enumerable"/>
///     </para>
///     <para> AsyncEnumerable </para>
///     <para>
///         <seealso href="https://gist.github.com/scattered-code/b834bbc355a9ee710e3147321d6f985a"/>
///     </para>
///     <para> Also a debugging improvement: <see href="https://youtu.be/gW19LaAYczI?t=497"/> </para>
/// </summary>
public static partial class AsyncLinq
{
    public static AsyncEnumerator<TElement, TElement[]> AsAsyncEnumerable<TElement>( this IEnumerable<TElement>         source, CancellationToken token = default ) => new(source.ToArray(), token);
    public static AsyncEnumerator<TElement, TElement[]> AsAsyncEnumerable<TElement>( this IReadOnlyCollection<TElement> source, CancellationToken token = default ) => new(source.ToArray(source.Count), token);
    public static AsyncEnumerator<TElement, TList> AsAsyncEnumerable<TElement, TList>( this TList source, CancellationToken token = default )
        where TList : IReadOnlyList<TElement> => new(source, token);


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool                         IsEmpty( this             ICollection                collection )                                => collection.Count == 0;
    public static                                                    ValueTask<HashSet<TElement>> ToHashSet<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default ) => source.ToHashSet(EqualityComparer<TElement>.Default, token);
    public static async ValueTask<HashSet<TElement>> ToHashSet<TElement>( this IAsyncEnumerable<TElement> source, EqualityComparer<TElement> comparer, CancellationToken token = default )
    {
        HashSet<TElement> list = new(comparer);
        await foreach ( TElement element in source.WithCancellation(token) ) { list.Add(element); }

        return list;
    }


    public static List<char>     ToList( this           string                        sequence ) => ToList(sequence, sequence.Length);
    public static List<TElement> ToList<TElement>( this IReadOnlyCollection<TElement> sequence ) => ToList(sequence, sequence.Count);
    public static List<TElement> ToList<TElement>( this IEnumerable<TElement> sequence, int initialCapacity )
    {
        List<TElement> array = new(initialCapacity);
        foreach ( ( int i, TElement item ) in sequence.Enumerate(0) ) { array[i] = item; }

        return array;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TElement[] GetArray<TElement>( this int length ) => GC.AllocateUninitializedArray<TElement>(length);


    public static async ValueTask<TElement[]> ToArray<TElement>( this IAsyncEnumerable<TElement> sequence, int initialCapacity = DEFAULT_CAPACITY, CancellationToken token = default )
    {
        List<TElement> array = await sequence.ToList(initialCapacity, token);
        return array.ToArray();
    }
    public static TElement[] ToArray<TElement>( IEnumerable<TElement> sequence )
        where TElement : IEquatable<TElement>
    {
        switch ( sequence )
        {
            case List<TElement> list: { return list.ToArray(); }

            case Collection<TElement> list:
            {
                TElement[] array = GetArray<TElement>(list.Count);
                list.CopyTo(array, 0);
                return array;
            }

            case ImmutableArray<TElement> immutable:
            {
                TElement[] array = GetArray<TElement>(immutable.Length);
                immutable.CopyTo(array, 0);
                return array;
            }

            case TElement[] sourceArray: { return sourceArray.ToArray(); }

            case IReadOnlyList<TElement> collection: { return collection.ToArray(); }

            case IReadOnlyCollection<TElement> collection:
            {
                TElement[] array = GetArray<TElement>(collection.Count);
                foreach ( ( int i, TElement item ) in collection.Enumerate(0) ) { array[i] = item; }

                return array;
            }

            default:
            {
                using Buffer<TElement> builder = new();
                foreach ( TElement equatable in sequence ) { builder.Add(equatable); }

                return builder.Values.ToArray();
            }
        }
    }
    public static char[]     ToArray( this           string                        sequence ) => ToArray(sequence, sequence.Length);
    public static TElement[] ToArray<TElement>( this IReadOnlyCollection<TElement> sequence ) => ToArray(sequence, sequence.Count);
    public static TElement[] ToArray<TElement>( ICollection<TElement>              sequence ) => ToArray(sequence, sequence.Count);
    public static TElement[] ToArray<TElement>( this IEnumerable<TElement> sequence, int capacity )
    {
        TElement[] array = GetArray<TElement>(capacity);
        foreach ( ( int i, TElement item ) in sequence.Enumerate(0) ) { array[i] = item; }

        return array;
    }
    public static TElement[] ToArray<TElement>( this IReadOnlyList<TElement> source )
    {
        TElement[] array = GetArray<TElement>(source.Count);
        for ( int i = 0; i < array.Length; i++ ) { array[i] = source[i]; }

        return array;
    }
    public static TResult[] ToArray<TElement, TResult>( this IEnumerable<TElement> sequence, Func<TElement, TResult> func )
        where TResult : IEquatable<TResult>
    {
        using Buffer<TResult> buffer = new();
        foreach ( TElement item in sequence ) { buffer.Add(func(item)); }

        return buffer.Span.ToArray();
    }
    public static TResult[] ToArray<TElement, TResult>( this ReadOnlySpan<TElement> sequence, Func<TElement, TResult> func )
        where TResult : IEquatable<TResult>
    {
        using Buffer<TResult> buffer = new(sequence.Length);
        foreach ( TElement item in sequence ) { buffer.Add(func(item)); }

        return buffer.Span.ToArray();
    }


    public static TElement[] Sorted<TElement>( this TElement[] array )
        where TElement : IComparable<TElement>
    {
        Array.Sort(array);
        return array;
    }
    public static TElement[] Sorted<TElement>( this TElement[] array, Comparer<TElement> comparer )
        where TElement : IComparable<TElement>
    {
        Array.Sort(array, comparer);
        return array;
    }
    public static TElement[] Sorted<TElement>( this TElement[] array, Comparison<TElement> comparer )
        where TElement : IComparable<TElement>
    {
        Array.Sort(array, comparer);
        return array;
    }


    public static async ValueTask<List<TElement>> ToList<TElement>( this IAsyncEnumerable<TElement> source, int initialCapacity = DEFAULT_CAPACITY, CancellationToken token = default )
    {
        List<TElement> list = new(initialCapacity);
        await foreach ( TElement element in source.WithCancellation(token) ) { list.Add(element); }

        return list;
    }
    public static async ValueTask<ObservableCollection<TElement>> ToObservableCollection<TElement>( this IAsyncEnumerable<TElement> source, int initialCapacity = DEFAULT_CAPACITY, CancellationToken token = default )
        where TElement : IEquatable<TElement>
    {
        ObservableCollection<TElement> list = new(initialCapacity);
        await foreach ( TElement element in source.WithCancellation(token) ) { await list.AddAsync(element, token); }

        return list;
    }
    public static async ValueTask<ConcurrentObservableCollection<TElement>> ToConcurrentObservableCollection<TElement>( this IAsyncEnumerable<TElement> source, int initialCapacity = DEFAULT_CAPACITY, CancellationToken token = default )
        where TElement : IEquatable<TElement>
    {
        ConcurrentObservableCollection<TElement> list = new(initialCapacity);
        await foreach ( TElement element in source.WithCancellation(token) ) { await list.AddAsync(element, token); }

        return list;
    }
}
