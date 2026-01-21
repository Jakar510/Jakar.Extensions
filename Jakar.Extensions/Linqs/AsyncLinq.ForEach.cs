namespace Jakar.Extensions;


public static partial class AsyncLinq
{
    extension<TKey, TElement>( IDictionary<TKey, TElement> dict )
    {
        public async Task ForEachAsync( Func<TKey, TElement, Task> action )
        {
            foreach ( ( TKey key, TElement value ) in dict )
            {
                await action(key, value)
                   .ConfigureAwait(false);
            }
        }
        public async Task ForEachAsync( Func<TElement, Task> action )
        {
            foreach ( TElement value in dict.Values )
            {
                await action(value)
                   .ConfigureAwait(false);
            }
        }
        public async Task ForEachAsync( Func<TKey, Task> action )
        {
            foreach ( TKey key in dict.Keys )
            {
                await action(key)
                   .ConfigureAwait(false);
            }
        }
    }



    extension<TElement>( IEnumerable<TElement> source )
    {
        public async Task ForEachParallelAsync( Func<TElement, Task> body, int? maxDegreeOfParallelism = null )
        {
            await Task.WhenAll(Partitioner.Create(source)
                                          .GetPartitions(maxDegreeOfParallelism ?? Environment.ProcessorCount)
                                          .AsParallel()
                                          .Select(awaitPartition))
                      .ConfigureAwait(false);

            return;

            async Task awaitPartition( IEnumerator<TElement> partition )
            {
                using ( partition )
                {
                    while ( partition.MoveNext() )
                    {
                        await body(partition.Current)
                           .ConfigureAwait(false);
                    }
                }
            }
        }
        public async Task ForEachParallelAsync( Func<TElement, ValueTask> body, int? maxDegreeOfParallelism = null )
        {
            await Task.WhenAll(Partitioner.Create(source)
                                          .GetPartitions(maxDegreeOfParallelism ?? Environment.ProcessorCount)
                                          .AsParallel()
                                          .Select(awaitPartition))
                      .ConfigureAwait(false);

            return;

            async Task awaitPartition( IEnumerator<TElement> partition )
            {
                using ( partition )
                {
                    while ( partition.MoveNext() )
                    {
                        await body(partition.Current)
                           .ConfigureAwait(false);
                    }
                }
            }
        }
        public async Task ForEachParallelAsync( Func<TElement, CancellationToken, Task> body, CancellationToken token, int? maxDegreeOfParallelism = null )
        {
            await Task.WhenAll(Partitioner.Create(source)
                                          .GetPartitions(maxDegreeOfParallelism ?? Environment.ProcessorCount)
                                          .AsParallel()
                                          .Select(awaitPartition))
                      .ConfigureAwait(false);

            return;

            async Task awaitPartition( IEnumerator<TElement> partition )
            {
                using ( partition )
                {
                    while ( partition.MoveNext() )
                    {
                        await body(partition.Current, token)
                           .ConfigureAwait(false);
                    }
                }
            }
        }
        public async Task ForEachParallelAsync( Func<TElement, CancellationToken, ValueTask> body, CancellationToken token, int? maxDegreeOfParallelism = null )
        {
            await Task.WhenAll(Partitioner.Create(source)
                                          .GetPartitions(maxDegreeOfParallelism ?? Environment.ProcessorCount)
                                          .AsParallel()
                                          .Select(awaitPartition))
                      .ConfigureAwait(false);

            return;

            async Task awaitPartition( IEnumerator<TElement> partition )
            {
                using ( partition )
                {
                    while ( partition.MoveNext() )
                    {
                        await body(partition.Current, token)
                           .ConfigureAwait(false);
                    }
                }
            }
        }

        public async Task ForEachAsync( Func<TElement, Task> action )
        {
            foreach ( TElement item in source )
            {
                await action(item)
                   .ConfigureAwait(false);
            }
        }
        public async ValueTask ForEachAsync( Func<TElement, ValueTask> action )
        {
            foreach ( TElement item in source )
            {
                await action(item)
                   .ConfigureAwait(false);
            }
        }

        /// <summary> If <paramref name="source"/> is an <see cref="List{TElement}"/> , items should not be added or removed while the calling. </summary>
        public void ForEach( Action<TElement> action )
        {
            switch ( source )
            {
                case List<TElement> list:
                    list.AsSpan()
                        .ForEach(action);

                    return;

                case TElement[] array:
                    array.AsSpan()
                         .ForEach(action);

                    return;

                default:
                    foreach ( TElement item in source ) { action(item); }

                    return;
            }
        }
        public void ForEachParallel( Action<TElement> action ) => source.AsParallel()
                                                                        .ForAll(action);
    }



    extension<TElement>( IAsyncEnumerable<TElement> source )
    {
        public async Task ForEachParallelAsync( Func<TElement, Task> action, int? maxDegreeOfParallelism = null, TaskScheduler? scheduler = null )
        {
            ExecutionDataflowBlockOptions options = new() { MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded };

            if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

            ActionBlock<TElement> block = new(action, options);

            await foreach ( TElement item in source.ConfigureAwait(false) ) { block.Post(item); }

            block.Complete();
            await block.Completion.ConfigureAwait(false);
        }
        public async Task ForEachParallelAsync( Func<TElement, CancellationToken, Task> action, CancellationToken token, int? maxDegreeOfParallelism = null, TaskScheduler? scheduler = null )
        {
            ExecutionDataflowBlockOptions options = new() { MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded };

            if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

            ActionBlock<TElement> block = new(awaitItem, options);

            await foreach ( TElement item in source.WithCancellation(token)
                                                   .ConfigureAwait(false) ) { block.Post(item); }

            block.Complete();
            await block.Completion.ConfigureAwait(false);
            return;

            async Task awaitItem( TElement item ) => await action(item, token)
                                                        .ConfigureAwait(false);
        }
        public async Task ForEachParallelAsync( Func<TElement, CancellationToken, ValueTask> action, CancellationToken token, int? maxDegreeOfParallelism = null, TaskScheduler? scheduler = null )
        {
            ExecutionDataflowBlockOptions options = new() { MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded };

            if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

            ActionBlock<TElement> block = new(awaitItem, options);

            await foreach ( TElement item in source.WithCancellation(token)
                                                   .ConfigureAwait(false) ) { block.Post(item); }

            block.Complete();
            await block.Completion.ConfigureAwait(false);
            return;

            async Task awaitItem( TElement item ) => await action(item, token)
                                                        .ConfigureAwait(false);
        }
        public async Task ForEachAsync( Func<TElement, Task> action )
        {
            await foreach ( TElement item in source.ConfigureAwait(false) )
            {
                await action(item)
                   .ConfigureAwait(false);
            }
        }
        public async ValueTask ForEachAsync( Func<TElement, ValueTask> action )
        {
            await foreach ( TElement item in source.ConfigureAwait(false) )
            {
                await action(item)
                   .ConfigureAwait(false);
            }
        }
    }



    public static async Task ForEachParallelAsync( this IEnumerable<Task> source, int? maxDegreeOfParallelism = null )
    {
        await Task.WhenAll(Partitioner.Create(source)
                                      .GetPartitions(maxDegreeOfParallelism ?? Environment.ProcessorCount)
                                      .AsParallel()
                                      .Select(awaitPartition))
                  .ConfigureAwait(false);

        return;

        static async Task awaitPartition( IEnumerator<Task> partition )
        {
            using ( partition )
            {
                while ( partition.MoveNext() ) { await partition.Current; }
            }
        }
    }
    public static async Task ForEachParallelAsync( this IAsyncEnumerable<Task> source, int? maxDegreeOfParallelism = null, TaskScheduler? scheduler = null )
    {
        ExecutionDataflowBlockOptions options = new() { MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }


        ActionBlock<Task> block = new(x => x, options);
        await foreach ( Task item in source.ConfigureAwait(false) ) { block.Post(item); }

        block.Complete();
        await block.Completion.ConfigureAwait(false);
    }


    public static async Task<IReadOnlyCollection<TElement>> WhenAllParallelAsync<TElement>( this IEnumerable<Task<TElement>> source, int? maxDegreeOfParallelism = null )
    {
        ConcurrentBag<TElement>? results = new();


        Task tasks = Task.WhenAll(Partitioner.Create(source)
                                             .GetPartitions(maxDegreeOfParallelism ?? Environment.ProcessorCount)
                                             .AsParallel()
                                             .Select(awaitPartition));

        await tasks.ConfigureAwait(false);
        return results;

        async Task awaitPartition( IEnumerator<Task<TElement>> partition )
        {
            using ( partition )
            {
                while ( partition.MoveNext() )
                {
                    TElement item = await partition.Current;
                    results.Add(item);
                }
            }
        }
    }
    public static async Task<IReadOnlyCollection<TElement>> WhenAllParallelAsync<TElement>( this IAsyncEnumerable<Task<TElement>> source, int? maxDegreeOfParallelism = null, TaskScheduler? scheduler = null )
    {
        ConcurrentBag<TElement>? results = new();

        ExecutionDataflowBlockOptions options = new() { MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        ActionBlock<Task<TElement>> block = new(awaitTask, options);
        await foreach ( Task<TElement> item in source.ConfigureAwait(false) ) { block.Post(item); }

        block.Complete();
        await block.Completion.ConfigureAwait(false);
        return results;

        async Task awaitTask( Task<TElement> task )
        {
            Debug.Assert(results != null, nameof(results) + " != null");
            results.Add(await task.ConfigureAwait(false));
        }
    }


    public static async Task<IReadOnlyCollection<TElement>> WhenAllParallelAsync<TElement>( IAsyncEnumerable<TElement> source, Func<TElement, CancellationToken, Task<TElement>> action, CancellationToken token, int? maxDegreeOfParallelism = null, TaskScheduler? scheduler = null )
    {
        ConcurrentBag<TElement>? results = new();

        ExecutionDataflowBlockOptions options = new() { MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        ActionBlock<TElement> block = new(awaitItem, options);

        await foreach ( TElement item in source.WithCancellation(token)
                                               .ConfigureAwait(false) ) { block.Post(item); }

        block.Complete();
        await block.Completion.ConfigureAwait(false);
        return results;

        async Task awaitItem( TElement item )
        {
            TElement result = await action(item, token)
                                 .ConfigureAwait(false);

            results.Add(result);
        }
    }
    public static async Task<IReadOnlyCollection<TElement>> WhenAllParallelAsync<TElement>( IAsyncEnumerable<TElement> source, Func<TElement, CancellationToken, ValueTask<TElement>> action, CancellationToken token, int? maxDegreeOfParallelism = null, TaskScheduler? scheduler = null )
    {
        ConcurrentBag<TElement>?      results = [];
        ExecutionDataflowBlockOptions options = new() { MaxDegreeOfParallelism = maxDegreeOfParallelism ?? DataflowBlockOptions.Unbounded };

        if ( scheduler is not null ) { options.TaskScheduler = scheduler; }

        ActionBlock<TElement> block = new(awaitItem, options);

        await foreach ( TElement item in source.WithCancellation(token)
                                               .ConfigureAwait(false) ) { block.Post(item); }

        block.Complete();
        await block.Completion.ConfigureAwait(false);
        return results;

        async Task awaitItem( TElement item )
        {
            TElement result = await action(item, token)
                                 .ConfigureAwait(false);

            results.Add(result);
        }
    }



    extension<TKey, TElement>( IDictionary<TKey, TElement> dict )
    {
        public async ValueTask ForEachAsync( Func<TKey, TElement, ValueTask> action )
        {
            foreach ( ( TKey key, TElement value ) in dict )
            {
                await action(key, value)
                   .ConfigureAwait(false);
            }
        }
        public async ValueTask ForEachAsync( Func<TElement, ValueTask> action )
        {
            foreach ( TElement value in dict.Values )
            {
                await action(value)
                   .ConfigureAwait(false);
            }
        }
        public async ValueTask ForEachAsync( Func<TKey, ValueTask> action )
        {
            foreach ( TKey key in dict.Keys )
            {
                await action(key)
                   .ConfigureAwait(false);
            }
        }
    }



    public static void ForEach<TElement>( this ReadOnlySpan<TElement> source, Action<TElement> action )
    {
        foreach ( ref readonly TElement item in source ) { action(item); }
    }



    extension<TKey, TElement>( IDictionary<TKey, TElement> dict )
    {
        public void ForEach( Action<TKey, TElement> action )
        {
            foreach ( ( TKey key, TElement value ) in dict ) { action(key, value); }
        }
        public void ForEach( Action<TKey> action )
        {
            foreach ( TKey key in dict.Keys ) { action(key); }
        }
        public void ForEach( Action<TElement> action )
        {
            foreach ( TElement value in dict.Values ) { action(value); }
        }
    }
}
