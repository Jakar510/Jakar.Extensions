namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static async Task ForEachAsync<TElement>( this IEnumerable<TElement> source, Func<TElement, Task> action )
    {
        foreach ( TElement item in source ) { await action( item ); }
    }


    public static async Task ForEachAsync<TKey, TElement>( this IDictionary<TKey, TElement> dict, Func<TKey, TElement, Task> action )
    {
        foreach ( (TKey key, TElement value) in dict ) { await action( key, value ); }
    }


    public static async Task ForEachAsync<TKey, TElement>( this IDictionary<TKey, TElement> dict, Func<TElement, Task> action )
    {
        foreach ( TElement value in dict.Values ) { await action( value ); }
    }


    public static async Task ForEachAsync<TKey, TElement>( this IDictionary<TKey, TElement> dict, Func<TKey, Task> action )
    {
        foreach ( TKey key in dict.Keys ) { await action( key ); }
    }


    public static async Task ForEachAsync<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, Task> action, CancellationToken token = default )
    {
        await foreach ( TElement item in source.WithCancellation( token ) ) { await action( item ); }
    }


    public static async Task ForEachParallelAsync<TElement>( this IEnumerable<TElement> source, Func<TElement, Task> body, int? maxDegreeOfParallelism = default )
    {
        async Task AwaitPartition( IEnumerator<TElement> partition )
        {
            using ( partition )
            {
                while ( partition.MoveNext() ) { await body( partition.Current ); }
            }
        }


        await Task.WhenAll( Partitioner.Create( source )
                                       .GetPartitions( maxDegreeOfParallelism ?? Environment.ProcessorCount )
                                       .AsParallel()
                                       .Select( AwaitPartition ) );
    }
    public static async Task ForEachParallelAsync<TElement>( this IEnumerable<TElement> source, Func<TElement, ValueTask> body, int? maxDegreeOfParallelism = default )
    {
        async Task AwaitPartition( IEnumerator<TElement> partition )
        {
            using ( partition )
            {
                while ( partition.MoveNext() ) { await body( partition.Current ); }
            }
        }


        await Task.WhenAll( Partitioner.Create( source )
                                       .GetPartitions( maxDegreeOfParallelism ?? Environment.ProcessorCount )
                                       .AsParallel()
                                       .Select( AwaitPartition ) );
    }
    public static async Task ForEachParallelAsync<TElement>( this IEnumerable<TElement> source, Func<TElement, CancellationToken, Task> body, CancellationToken token, int? maxDegreeOfParallelism = default )
    {
        async Task AwaitPartition( IEnumerator<TElement> partition )
        {
            using ( partition )
            {
                while ( partition.MoveNext() ) { await body( partition.Current, token ); }
            }
        }


        await Task.WhenAll( Partitioner.Create( source )
                                       .GetPartitions( maxDegreeOfParallelism ?? Environment.ProcessorCount )
                                       .AsParallel()
                                       .Select( AwaitPartition ) );
    }
    public static async Task ForEachParallelAsync<TElement>( this IEnumerable<TElement> source, Func<TElement, CancellationToken, ValueTask> body, CancellationToken token, int? maxDegreeOfParallelism = default )
    {
        async Task AwaitPartition( IEnumerator<TElement> partition )
        {
            using ( partition )
            {
                while ( partition.MoveNext() ) { await body( partition.Current, token ); }
            }
        }


        await Task.WhenAll( Partitioner.Create( source )
                                       .GetPartitions( maxDegreeOfParallelism ?? Environment.ProcessorCount )
                                       .AsParallel()
                                       .Select( AwaitPartition ) );
    }


    public static async Task ForEachParallelAsync<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, Task> action, CancellationToken token = default, int? maxDegreeOfParallelism = default, TaskScheduler? scheduler = default )
    {
        var options = new ExecutionDataflowBlockOptions
                      {
                          MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded,
                      };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        var block = new ActionBlock<TElement>( action, options );

        await foreach ( TElement item in source.WithCancellation( token ) ) { block.Post( item ); }

        block.Complete();
        await block.Completion;
    }
    public static async Task ForEachParallelAsync<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, CancellationToken, Task> action, CancellationToken token, int? maxDegreeOfParallelism = default, TaskScheduler? scheduler = default )
    {
        var options = new ExecutionDataflowBlockOptions
                      {
                          MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded,
                      };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        async Task AwaitItem( TElement item ) => await action( item, token );

        var block = new ActionBlock<TElement>( AwaitItem, options );

        await foreach ( TElement item in source.WithCancellation( token ) ) { block.Post( item ); }

        block.Complete();
        await block.Completion;
    }
    public static async Task ForEachParallelAsync<TElement>( this IAsyncEnumerable<TElement>              source,
                                                             Func<TElement, CancellationToken, ValueTask> action,
                                                             CancellationToken                            token,
                                                             int?                                         maxDegreeOfParallelism = default,
                                                             TaskScheduler?                               scheduler              = default
    )
    {
        var options = new ExecutionDataflowBlockOptions
                      {
                          MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded,
                      };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        async Task AwaitItem( TElement item ) => await action( item, token );

        var block = new ActionBlock<TElement>( AwaitItem, options );

        await foreach ( TElement item in source.WithCancellation( token ) ) { block.Post( item ); }

        block.Complete();
        await block.Completion;
    }


    public static async Task ForEachParallelAsync( this IEnumerable<Task> source, int? maxDegreeOfParallelism = default )
    {
        static async Task AwaitPartition( IEnumerator<Task> partition )
        {
            using ( partition )
            {
                while ( partition.MoveNext() ) { await partition.Current; }
            }
        }


        await Task.WhenAll( Partitioner.Create( source )
                                       .GetPartitions( maxDegreeOfParallelism ?? Environment.ProcessorCount )
                                       .AsParallel()
                                       .Select( AwaitPartition ) );
    }
    public static async Task ForEachParallelAsync( this IAsyncEnumerable<Task> source, CancellationToken token = default, int? maxDegreeOfParallelism = default, TaskScheduler? scheduler = default )
    {
        var options = new ExecutionDataflowBlockOptions
                      {
                          MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded,
                      };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }


        var block = new ActionBlock<Task>( x => x, options );
        await foreach ( Task item in source.WithCancellation( token ) ) { block.Post( item ); }

        block.Complete();
        await block.Completion;
    }


    public static async Task<IReadOnlyCollection<TElement>> WhenAllParallelAsync<TElement>( this IEnumerable<Task<TElement>> source, int? maxDegreeOfParallelism = default )
    {
        var results = new ConcurrentBag<TElement>();

        async Task AwaitPartition( IEnumerator<Task<TElement>> partition )
        {
            using ( partition )
            {
                while ( partition.MoveNext() )
                {
                    TElement item = await partition.Current;
                    results.Add( item );
                }
            }
        }


        Task tasks = Task.WhenAll( Partitioner.Create( source )
                                              .GetPartitions( maxDegreeOfParallelism ?? Environment.ProcessorCount )
                                              .AsParallel()
                                              .Select( AwaitPartition ) );

        await tasks;
        return results;
    }
    public static async Task<IReadOnlyCollection<TElement>> WhenAllParallelAsync<TElement>( this IAsyncEnumerable<Task<TElement>> source, CancellationToken token = default, int? maxDegreeOfParallelism = default, TaskScheduler? scheduler = default )
    {
        var results = new ConcurrentBag<TElement>();

        async Task AwaitTask( Task<TElement> task )
        {
            Debug.Assert( results != null, nameof(results) + " != null" );
            results.Add( await task );
        }

        var options = new ExecutionDataflowBlockOptions
                      {
                          MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded,
                      };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        var block = new ActionBlock<Task<TElement>>( AwaitTask, options );
        await foreach ( Task<TElement> item in source.WithCancellation( token ) ) { block.Post( item ); }

        block.Complete();
        await block.Completion;
        return results;
    }


    public static async Task<IReadOnlyCollection<TElement>> WhenAllParallelAsync<TElement>( IAsyncEnumerable<TElement>                        source,
                                                                                            Func<TElement, CancellationToken, Task<TElement>> action,
                                                                                            CancellationToken                                 token,
                                                                                            int?                                              maxDegreeOfParallelism = default,
                                                                                            TaskScheduler?                                    scheduler              = default
    )
    {
        var results = new ConcurrentBag<TElement>();

        var options = new ExecutionDataflowBlockOptions
                      {
                          MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded,
                      };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        async Task AwaitItem( TElement item )
        {
            TElement result = await action( item, token );
            results.Add( result );
        }

        var block = new ActionBlock<TElement>( AwaitItem, options );

        await foreach ( TElement item in source.WithCancellation( token ) ) { block.Post( item ); }

        block.Complete();
        await block.Completion;
        return results;
    }
    public static async Task<IReadOnlyCollection<TElement>> WhenAllParallelAsync<TElement>( IAsyncEnumerable<TElement>                             source,
                                                                                            Func<TElement, CancellationToken, ValueTask<TElement>> action,
                                                                                            CancellationToken                                      token,
                                                                                            int?                                                   maxDegreeOfParallelism = default,
                                                                                            TaskScheduler?                                         scheduler              = default
    )
    {
        var results = new ConcurrentBag<TElement>();

        var options = new ExecutionDataflowBlockOptions
                      {
                          MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded,
                      };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        async Task AwaitItem( TElement item )
        {
            TElement result = await action( item, token );
            results.Add( result );
        }

        var block = new ActionBlock<TElement>( AwaitItem, options );

        await foreach ( TElement item in source.WithCancellation( token ) ) { block.Post( item ); }

        block.Complete();
        await block.Completion;
        return results;
    }
    public static async ValueTask ForEachAsync<TElement>( this IEnumerable<TElement> source, Func<TElement, ValueTask> action )
    {
        foreach ( TElement item in source ) { await action( item ); }
    }
    public static async ValueTask ForEachAsync<TKey, TElement>( this IDictionary<TKey, TElement> dict, Func<TKey, TElement, ValueTask> action )
    {
        foreach ( (TKey key, TElement value) in dict ) { await action( key, value ); }
    }
    public static async ValueTask ForEachAsync<TKey, TElement>( this IDictionary<TKey, TElement> dict, Func<TElement, ValueTask> action )
    {
        foreach ( TElement value in dict.Values ) { await action( value ); }
    }
    public static async ValueTask ForEachAsync<TKey, TElement>( this IDictionary<TKey, TElement> dict, Func<TKey, ValueTask> action )
    {
        foreach ( TKey key in dict.Keys ) { await action( key ); }
    }
    public static async ValueTask ForEachAsync<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask> action, CancellationToken token = default )
    {
        await foreach ( TElement item in source.WithCancellation( token ) ) { await action( item ); }
    }
    /// <summary> If <paramref name="source"/> is an <see cref="List{TElement}"/> , items should not be added or removed while the calling. </summary>
    /// <typeparam name="TElement"> </typeparam>
    public static void ForEach<TElement>( this IEnumerable<TElement> source, Action<TElement> action )
    {
    #if NET6_0_OR_GREATER
        if ( source is List<TElement> list )
        {
            foreach ( TElement x in CollectionsMarshal.AsSpan( list ) ) { action( x ); }

            return;
        }

    #endif

        foreach ( TElement item in source ) { action( item ); }
    }


    public static void ForEach<TKey, TElement>( this IDictionary<TKey, TElement> dict, Action<TKey, TElement> action )
    {
        foreach ( (TKey key, TElement value) in dict ) { action( key, value ); }
    }
    public static void ForEach<TKey, TElement>( this IDictionary<TKey, TElement> dict, Action<TKey> action )
    {
        foreach ( TKey key in dict.Keys ) { action( key ); }
    }
    public static void ForEach<TKey, TElement>( this IDictionary<TKey, TElement> dict, Action<TElement> action )
    {
        foreach ( TElement value in dict.Values ) { action( value ); }
    }
    public static void ForEachParallel<TElement>( this IEnumerable<TElement> source, Action<TElement> action ) => source.AsParallel()
                                                                                                                        .ForAll( action );
}
