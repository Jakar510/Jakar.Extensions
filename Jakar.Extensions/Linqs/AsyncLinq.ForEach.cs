using System.Threading.Tasks.Dataflow;



namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    public static void ForEach<TElement>( this IEnumerable<TElement> list, Action<TElement> action )
    {
        foreach ( TElement item in list ) { action(item); }
    }
    public static void ForEachParallel<TElement>( this IEnumerable<TElement> list, Action<TElement> action ) => list.AsParallel()
                                                                                                                    .ForAll(action);


    public static async Task ForEachAsync<TElement>( this IEnumerable<TElement> list, Func<TElement, Task> action, bool continueOnCapturedContext = true )
    {
        foreach ( TElement item in list )
        {
            await action(item)
               .ConfigureAwait(continueOnCapturedContext);
        }
    }
    public static async ValueTask ForEachAsync<TElement>( this IEnumerable<TElement> list, Func<TElement, ValueTask> action, bool continueOnCapturedContext = true )
    {
        foreach ( TElement item in list )
        {
            await action(item)
               .ConfigureAwait(continueOnCapturedContext);
        }
    }


    public static void ForEach<TKey, TElement>( this IDictionary<TKey, TElement> dict, Action<TKey, TElement> action )
    {
        foreach ( ( TKey key, TElement value ) in dict ) { action(key, value); }
    }


    public static async Task ForEachAsync<TKey, TElement>( this IDictionary<TKey, TElement> dict, Func<TKey, TElement, Task> action, bool continueOnCapturedContext = true )
    {
        foreach ( ( TKey key, TElement value ) in dict )
        {
            await action(key, value)
               .ConfigureAwait(continueOnCapturedContext);
        }
    }
    public static async ValueTask ForEachAsync<TKey, TElement>( this IDictionary<TKey, TElement> dict, Func<TKey, TElement, ValueTask> action, bool continueOnCapturedContext = true )
    {
        foreach ( ( TKey key, TElement value ) in dict )
        {
            await action(key, value)
               .ConfigureAwait(continueOnCapturedContext);
        }
    }


    public static async Task ForEachAsync<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, Task> action, bool continueOnCapturedContext = true, CancellationToken token = default )
    {
        await foreach ( TElement item in source.WithCancellation(token) )
        {
            await action(item)
               .ConfigureAwait(continueOnCapturedContext);
        }
    }
    public static async ValueTask ForEachAsync<TElement>( this IAsyncEnumerable<TElement> source, Func<TElement, ValueTask> action, bool continueOnCapturedContext = true, CancellationToken token = default )
    {
        await foreach ( TElement item in source.WithCancellation(token) )
        {
            await action(item)
               .ConfigureAwait(continueOnCapturedContext);
        }
    }


    public static async Task ForEachParallelAsync( this IEnumerable<Task> source, int? degreeOfParallelism = default, bool continueOnCapturedContext = true )
    {
        async Task AwaitPartition( IEnumerator<Task> partition )
        {
            using ( partition )
            {
                while ( partition.MoveNext() )
                {
                    if ( partition.Current is null ) { throw new NullReferenceException(nameof(partition.Current)); }

                    await partition.Current.ConfigureAwait(continueOnCapturedContext);
                }
            }
        }


        Task tasks = Task.WhenAll(Partitioner.Create(source)
                                             .GetPartitions(degreeOfParallelism ?? Environment.ProcessorCount)
                                             .AsParallel()
                                             .Select(AwaitPartition));

        try { await tasks; }
        catch ( Exception e )
        {
            if ( tasks.Exception is null ) { throw new NullReferenceException("Unknown error has occurred. Task.Exception is null.", e); }

            throw tasks.Exception;
        }
    }


    public static async Task ForEachParallelAsync<T>( this IEnumerable<T> source, Func<T, Task> body, int? degreeOfParallelism = default, bool continueOnCapturedContext = true )
    {
        async Task AwaitPartition( IEnumerator<T> partition )
        {
            using ( partition )
            {
                while ( partition.MoveNext() )
                {
                    await body(partition.Current)
                       .ConfigureAwait(continueOnCapturedContext);
                }
            }
        }


        Task tasks = Task.WhenAll(Partitioner.Create(source)
                                             .GetPartitions(degreeOfParallelism ?? Environment.ProcessorCount)
                                             .AsParallel()
                                             .Select(AwaitPartition));

        try { await tasks; }
        catch ( Exception e )
        {
            if ( tasks.Exception is null ) { throw new NullReferenceException("Unknown error has occurred. Task.Exception is null.", e); }

            throw tasks.Exception;
        }
    }
    public static async Task ForEachParallelAsync<T>( this IEnumerable<T> source, Func<T, ValueTask> body, int? degreeOfParallelism = default, bool continueOnCapturedContext = true )
    {
        async Task AwaitPartition( IEnumerator<T> partition )
        {
            using ( partition )
            {
                while ( partition.MoveNext() )
                {
                    await body(partition.Current)
                       .ConfigureAwait(continueOnCapturedContext);
                }
            }
        }


        Task tasks = Task.WhenAll(Partitioner.Create(source)
                                             .GetPartitions(degreeOfParallelism ?? Environment.ProcessorCount)
                                             .AsParallel()
                                             .Select(AwaitPartition));

        try { await tasks; }
        catch ( Exception e )
        {
            if ( tasks.Exception is null ) { throw new NullReferenceException("Unknown error has occurred. Task.Exception is null.", e); }

            throw tasks.Exception;
        }
    }
    public static async Task ForEachParallelAsync<T>( this IEnumerable<T> source, Func<T, CancellationToken, Task> body, CancellationToken token, int? degreeOfParallelism = default, bool continueOnCapturedContext = true )
    {
        async Task AwaitPartition( IEnumerator<T> partition )
        {
            using ( partition )
            {
                while ( partition.MoveNext() )
                {
                    await body(partition.Current, token)
                       .ConfigureAwait(continueOnCapturedContext);
                }
            }
        }


        Task tasks = Task.WhenAll(Partitioner.Create(source)
                                             .GetPartitions(degreeOfParallelism ?? Environment.ProcessorCount)
                                             .AsParallel()
                                             .Select(AwaitPartition));

        try { await tasks; }
        catch ( Exception e )
        {
            if ( tasks.Exception is null ) { throw new NullReferenceException("Unknown error has occurred. Task.Exception is null.", e); }

            throw tasks.Exception;
        }
    }
    public static async Task ForEachParallelAsync<T>( this IEnumerable<T> source, Func<T, CancellationToken, ValueTask> body, CancellationToken token, int? degreeOfParallelism = default, bool continueOnCapturedContext = true )
    {
        async Task AwaitPartition( IEnumerator<T> partition )
        {
            using ( partition )
            {
                while ( partition.MoveNext() )
                {
                    await body(partition.Current, token)
                       .ConfigureAwait(continueOnCapturedContext);
                }
            }
        }


        Task tasks = Task.WhenAll(Partitioner.Create(source)
                                             .GetPartitions(degreeOfParallelism ?? Environment.ProcessorCount)
                                             .AsParallel()
                                             .Select(AwaitPartition));

        try { await tasks; }
        catch ( Exception e )
        {
            if ( tasks.Exception is null ) { throw new NullReferenceException("Unknown error has occurred. Task.Exception is null.", e); }

            throw tasks.Exception;
        }
    }


    public static async Task ForEachParallelAsync<T>( this IAsyncEnumerable<T> source, Func<T, Task> action, int? maxDegreeOfParallelism = default, TaskScheduler? scheduler = null )
    {
        var options = new ExecutionDataflowBlockOptions
                      {
                          MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded
                      };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        var block = new ActionBlock<T>(action, options);

        await foreach ( T item in source ) { block.Post(item); }

        block.Complete();
        await block.Completion;
    }
    public static async Task ForEachParallelAsync<T>( this IAsyncEnumerable<T> source, Func<T, Task> action, CancellationToken token, int? maxDegreeOfParallelism = default, TaskScheduler? scheduler = null )
    {
        var options = new ExecutionDataflowBlockOptions
                      {
                          MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded
                      };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        var block = new ActionBlock<T>(action, options);

        await foreach ( T item in source.WithCancellation(token) ) { block.Post(item); }

        block.Complete();
        await block.Completion;
    }
    public static async Task ForEachParallelAsync<T>( this IAsyncEnumerable<T> source, Func<T, CancellationToken, Task> action, CancellationToken token, int? maxDegreeOfParallelism = default, TaskScheduler? scheduler = null )
    {
        var options = new ExecutionDataflowBlockOptions
                      {
                          MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded
                      };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        async Task AwaitItem( T item ) => await action(item, token);

        var block = new ActionBlock<T>(AwaitItem, options);

        await foreach ( T item in source.WithCancellation(token) ) { block.Post(item); }

        block.Complete();
        await block.Completion;
    }
    public static async Task ForEachParallelAsync<T>( this IAsyncEnumerable<T> source, Func<T, CancellationToken, ValueTask> action, CancellationToken token, int? maxDegreeOfParallelism = default, TaskScheduler? scheduler = null )
    {
        var options = new ExecutionDataflowBlockOptions
                      {
                          MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded
                      };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        async Task AwaitItem( T item ) => await action(item, token);

        var block = new ActionBlock<T>(AwaitItem, options);

        await foreach ( T item in source.WithCancellation(token) ) { block.Post(item); }

        block.Complete();
        await block.Completion;
    }


    public static async Task<IReadOnlyCollection<T>> WhenAllParallelAsync<T>( this IEnumerable<Task<T>> source, int? degreeOfParallelism = default, bool continueOnCapturedContext = true )
    {
        var results = new ConcurrentBag<T>();

        async Task AwaitPartition( IEnumerator<Task<T>> partition )
        {
            using ( partition )
            {
                while ( partition.MoveNext() )
                {
                    if ( partition.Current is null ) { throw new NullReferenceException(nameof(partition.Current)); }

                    T item = await partition.Current.ConfigureAwait(continueOnCapturedContext);
                    results.Add(item);
                }
            }
        }


        Task tasks = Task.WhenAll(Partitioner.Create(source)
                                             .GetPartitions(degreeOfParallelism ?? Environment.ProcessorCount)
                                             .AsParallel()
                                             .Select(AwaitPartition));

        try
        {
            await tasks;
            return results;
        }
        catch ( Exception e )
        {
            if ( tasks.Exception is null ) { throw new NullReferenceException("Unknown error has occurred. Task.Exception is null.", e); }

            throw tasks.Exception;
        }
    }

    public static async Task<IReadOnlyCollection<T>> WhenAllParallelAsync<T>( IAsyncEnumerable<T> source, Func<T, CancellationToken, Task<T>> action, CancellationToken token, int? maxDegreeOfParallelism = default, TaskScheduler? scheduler = null )
    {
        var results = new ConcurrentBag<T>();

        var options = new ExecutionDataflowBlockOptions
                      {
                          MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded
                      };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        async Task AwaitItem( T item )
        {
            T result = await action(item, token);
            results.Add(result);
        }

        var block = new ActionBlock<T>(AwaitItem, options);

        await foreach ( T item in source.WithCancellation(token) ) { block.Post(item); }

        block.Complete();
        await block.Completion;
        return results;
    }
    public static async Task<IReadOnlyCollection<T>> WhenAllParallelAsync<T>( IAsyncEnumerable<T>                      source,
                                                                              Func<T, CancellationToken, ValueTask<T>> action,
                                                                              CancellationToken                        token,
                                                                              int?                                     maxDegreeOfParallelism = default,
                                                                              TaskScheduler?                           scheduler              = null
    )
    {
        var results = new ConcurrentBag<T>();

        var options = new ExecutionDataflowBlockOptions
                      {
                          MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded
                      };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        async Task AwaitItem( T item )
        {
            T result = await action(item, token);
            results.Add(result);
        }

        var block = new ActionBlock<T>(AwaitItem, options);

        await foreach ( T item in source.WithCancellation(token) ) { block.Post(item); }

        block.Complete();
        await block.Completion;
        return results;
    }
}
