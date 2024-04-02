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
    public static AsyncEnumerator<TElement, TElement[]> AsAsyncEnumerable<TElement>( this IReadOnlyCollection<TElement> source, CancellationToken token = default ) => new(source.ToArray( source.Count ), token);
    public static AsyncEnumerator<TElement, TList> AsAsyncEnumerable<TElement, TList>( this TList source, CancellationToken token = default )
        where TList : IReadOnlyList<TElement> => new(source, token);


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool                         IsEmpty( this             ICollection                collection )                                => collection.Count == 0;
    public static                                                      ValueTask<HashSet<TElement>> ToHashSet<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default ) => source.ToHashSet( EqualityComparer<TElement>.Default, token );
    public static async ValueTask<HashSet<TElement>> ToHashSet<TElement>( this IAsyncEnumerable<TElement> source, IEqualityComparer<TElement> comparer, CancellationToken token = default )
    {
        var list = new HashSet<TElement>( comparer );
        await foreach ( TElement element in source.WithCancellation( token ) ) { list.Add( element ); }

        return list;
    }


    public static List<char> ToList( this    string                 sequence ) => ToList( sequence, sequence.Length );
    public static List<T>    ToList<T>( this IReadOnlyCollection<T> sequence ) => ToList( sequence, sequence.Count );
    public static List<T> ToList<T>( this IEnumerable<T> sequence, int count )
    {
        List<T> array = new(count);
        foreach ( (int i, T item) in sequence.Enumerate( 0 ) ) { array[i] = item; }

        return array;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static T[] GetArray<T>( int count )
    {
        Guard.IsGreaterThanOrEqualTo( count, 0 );

    #if NET6_0_OR_GREATER
        return GC.AllocateUninitializedArray<T>( count );
    #else
        return new T[count];
    #endif
    }
    public static async ValueTask<T[]> ToArray<T>( this IAsyncEnumerable<T> sequence, int count )
    {
        T[] array = GetArray<T>( count );
        await foreach ( (int index, T? value) in sequence.Enumerate( 0 ) ) { array[index] = value; }

        return array;
    }
    public static T[] ToArray<T>( IEnumerable<T> sequence )
        where T : IEquatable<T>
    {
        switch ( sequence )
        {
            case List<T> list:
            {
                T[] array = GetArray<T>( list.Count );
                list.CopyTo( array, 0 );
                return array;
            }

            case Collection<T> list:
            {
                T[] array = GetArray<T>( list.Count );
                list.CopyTo( array, 0 );
                return array;
            }

            case T[] sourceArray:
            {
                T[] array = GetArray<T>( sourceArray.Length );
                Array.Copy( sourceArray, 0, array, 0, sourceArray.Length );
                return array;
            }

            case ImmutableArray<T> immutable:
            {
                T[] array = GetArray<T>( immutable.Length );
                immutable.CopyTo( array, 0 );
                return array;
            }

            case IReadOnlyList<T> collection:
            {
                T[] array = GetArray<T>( collection.Count );
                for ( int i = 0; i < collection.Count; i++ ) { array[i] = collection[i]; }

                return array;
            }

            case IReadOnlyCollection<T> collection:
            {
                T[] array = GetArray<T>( collection.Count );
                foreach ( (int i, T item) in collection.Enumerate( 0 ) ) { array[i] = item; }

                return array;
            }

            default:
            {
                using var builder = new Buffer<T>();
                foreach ( T equatable in sequence ) { builder.Append( equatable ); }

                return builder.Span.ToArray();
            }
        }
    }
    public static char[] ToArray( this    string                 sequence ) => ToArray( sequence, sequence.Length );
    public static T[]    ToArray<T>( this IReadOnlyCollection<T> sequence ) => ToArray( sequence, sequence.Count );
    public static T[]    ToArray<T>( ICollection<T>              sequence ) => ToArray( sequence, sequence.Count );
    public static T[] ToArray<T>( this IEnumerable<T> sequence, int count )
    {
        T[] array = GetArray<T>( count );
        foreach ( (int i, T item) in sequence.Enumerate( 0 ) ) { array[i] = item; }

        return array;
    }
    public static TElement[] ToArray<TElement>( this IReadOnlyList<TElement> source )
    {
    #if NET6_0_OR_GREATER
        TElement[] array = GC.AllocateUninitializedArray<TElement>( source.Count );
    #else
        var array = new TElement[source.Count];
    #endif

        for ( int i = 0; i < array.Length; i++ ) { array[i] = source[i]; }

        return array;
    }
    public static TResult[] ToArray<T, TResult>( this IEnumerable<T> sequence, Func<T, TResult> func )
        where TResult : IEquatable<TResult>
    {
        using var buffer = new Buffer<TResult>();
        foreach ( T item in sequence ) { buffer.Append( func( item ) ); }

        return buffer.Span.ToArray();
    }
    public static TResult[] ToArray<T, TResult>( this ReadOnlySpan<T> sequence, Func<T, TResult> func )
        where TResult : IEquatable<TResult>
    {
        using var buffer = new Buffer<TResult>();
        foreach ( T item in sequence ) { buffer.Append( func( item ) ); }

        return buffer.Span.ToArray();
    }


    public static T[] Sorted<T>( this T[] array )
        where T : IComparable<T>
    {
        Array.Sort( array );
        return array;
    }
    public static T[] Sorted<T>( this T[] array, IComparer<T> comparer )
        where T : IComparable<T>
    {
        Array.Sort( array, comparer );
        return array;
    }
    public static T[] Sorted<T>( this T[] array, Comparison<T> comparer )
        where T : IComparable<T>
    {
        Array.Sort( array, comparer );
        return array;
    }


    public static async ValueTask<List<TElement>> ToList<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        var list = new List<TElement>();
        await foreach ( TElement element in source.WithCancellation( token ) ) { list.Add( element ); }

        return list;
    }
    public static async ValueTask<ObservableCollection<TElement>> ToObservableCollection<TElement>( this IAsyncEnumerable<TElement> source, CancellationToken token = default )
    {
        var list = new ObservableCollection<TElement>();
        await foreach ( TElement element in source.WithCancellation( token ) ) { list.Add( element ); }

        return list;
    }
}
