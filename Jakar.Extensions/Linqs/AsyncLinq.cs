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


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsEmpty( this ICollection collection ) => collection.Count == 0;



    extension<TElement>( IAsyncEnumerable<TElement> self )
    {
        public ValueTask<HashSet<TElement>> ToHashSet( CancellationToken token = default ) => self.ToHashSet(EqualityComparer<TElement>.Default, token);

        public async ValueTask<HashSet<TElement>> ToHashSet( EqualityComparer<TElement> comparer, CancellationToken token = default )
        {
            HashSet<TElement> list = new(comparer);

            await foreach ( TElement element in self.WithCancellation(token)
                                                    .ConfigureAwait(false) ) { list.Add(element); }

            return list;
        }
       
        public async ValueTask<TElement[]> ToArray( int initialCapacity = DEFAULT_CAPACITY, CancellationToken token = default )
        {
            List<TElement> array = await self.ToList(initialCapacity, token)
                                             .ConfigureAwait(false);

            return array.ToArray();
        }
       
        public async ValueTask<List<TElement>> ToList( int initialCapacity = DEFAULT_CAPACITY, CancellationToken token = default )
        {
            List<TElement> list = new(initialCapacity);

            await foreach ( TElement element in self.WithCancellation(token)
                                                    .ConfigureAwait(false) ) { list.Add(element); }

            return list;
        }
      
        public async ValueTask<ImmutableArray<TElement>> ToImmutableArray( int initialCapacity = DEFAULT_CAPACITY, CancellationToken token = default )
        {
            List<TElement> list = new(initialCapacity);

            await foreach ( TElement element in self.WithCancellation(token)
                                                    .ConfigureAwait(false) ) { list.Add(element); }

            return [..list];
        }
    }



    public static List<char>     ToList( this           string                        sequence ) => sequence.ToList(sequence.Length);
    public static List<TElement> ToList<TElement>( this IReadOnlyCollection<TElement> sequence ) => sequence.ToList(sequence.Count);
    public static List<TElement> ToList<TElement>( this IEnumerable<TElement> sequence, int initialCapacity )
    {
        List<TElement> array = new(initialCapacity);
        foreach ( ( int i, TElement item ) in sequence.Enumerate(0) ) { array[i] = item; }

        return array;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TElement[] GetArray<TElement>( this int length ) => GC.AllocateUninitializedArray<TElement>(length);


    public static TElement[] ToArray<TElement>( IEnumerable<TElement> sequence )
        where TElement : IEquatable<TElement>
    {
        switch ( sequence )
        {
            case List<TElement> list: { return list.ToArray(); }

            case Collection<TElement> list:
            {
                TElement[] array = list.Count.GetArray<TElement>();
                list.CopyTo(array, 0);
                return array;
            }

            case ImmutableArray<TElement> immutable:
            {
                TElement[] array = immutable.Length.GetArray<TElement>();
                immutable.CopyTo(array, 0);
                return array;
            }

            case TElement[] sourceArray: { return sourceArray.ToArray(); }

            case IReadOnlyList<TElement> collection: { return collection.ToArray(); }

            case IReadOnlyCollection<TElement> collection:
            {
                TElement[] array = collection.Count.GetArray<TElement>();
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
    public static char[]     ToArray( this           string                        sequence ) => sequence.ToArray(sequence.Length);
    public static TElement[] ToArray<TElement>( this IReadOnlyCollection<TElement> sequence ) => sequence.ToArray(sequence.Count);
    public static TElement[] ToArray<TElement>( ICollection<TElement>              sequence ) => sequence.ToArray(sequence.Count);
    public static TElement[] ToArray<TElement>( this IEnumerable<TElement> sequence, int capacity )
    {
        TElement[] array = capacity.GetArray<TElement>();
        foreach ( ( int i, TElement item ) in sequence.Enumerate(0) ) { array[i] = item; }

        return array;
    }
    public static TElement[] ToArray<TElement>( this IReadOnlyList<TElement> source )
    {
        TElement[] array = source.Count.GetArray<TElement>();
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



    extension<TElement>( TElement[] array )
        where TElement : IComparable<TElement>
    {
        public TElement[] Sorted()
        {
            Array.Sort((Array)array);
            return array;
        }
        public TElement[] Sorted( Comparer<TElement> comparer )
        {
            Array.Sort(array, comparer);
            return array;
        }
        public TElement[] Sorted( Comparison<TElement> comparer )
        {
            Array.Sort(array, comparer);
            return array;
        }
    }



    extension<TElement>( IAsyncEnumerable<TElement> source )
        where TElement : IEquatable<TElement>
    {
        public async ValueTask<ObservableCollection<TElement>> ToObservableCollection( int initialCapacity = DEFAULT_CAPACITY, CancellationToken token = default )
        {
            ObservableCollection<TElement> list = new(initialCapacity);

            await foreach ( TElement element in source.WithCancellation(token)
                                                      .ConfigureAwait(false) )
            {
                await list.AddAsync(element, token)
                          .ConfigureAwait(false);
            }

            return list;
        }
        public async ValueTask<ConcurrentObservableCollection<TElement>> ToConcurrentObservableCollection( int initialCapacity = DEFAULT_CAPACITY, CancellationToken token = default )
        {
            ConcurrentObservableCollection<TElement> list = new(initialCapacity);

            await foreach ( TElement element in source.WithCancellation(token)
                                                      .ConfigureAwait(false) )
            {
                await list.AddAsync(element, token)
                          .ConfigureAwait(false);
            }

            return list;
        }
    }
}
