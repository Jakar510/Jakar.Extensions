namespace Jakar.Extensions;
#nullable enable



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
    public static AsyncEnumerator<TElement> AsAsyncEnumerable<TElement>( this IEnumerable<TElement> source, CancellationToken token = default ) => source is IReadOnlyList<TElement> list
                                                                                                                                                       ? list.AsAsyncEnumerable( token )
                                                                                                                                                       : source.ToArray()
                                                                                                                                                               .AsAsyncEnumerable( token );
    public static AsyncEnumerator<TElement> AsAsyncEnumerable<TElement>( this IReadOnlyList<TElement>    source, CancellationToken token = default ) => new(source, token);
    public static bool IsEmpty( this                                          ICollection                collection ) => collection.Count == 0;
    public static ValueTask<HashSet<TElement>> ToHashSet<TElement>( this      IAsyncEnumerable<TElement> source, CancellationToken token = default ) => source.ToHashSet( EqualityComparer<TElement>.Default, token );
    public static async ValueTask<HashSet<TElement>> ToHashSet<TElement>( this IAsyncEnumerable<TElement> source, IEqualityComparer<TElement> comparer, CancellationToken token = default )
    {
        var list = new HashSet<TElement>( comparer );
        await foreach ( TElement element in source.WithCancellation( token ) ) { list.Add( element ); }

        return list;
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
