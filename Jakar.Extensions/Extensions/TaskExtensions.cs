#nullable enable
using System.Threading.Tasks.Dataflow;



namespace Jakar.Extensions;


public static class Tasks
{
    /// <summary> Creates a cancellable delay from the given <paramref name="delay"/> </summary>
    public static Task Delay( this TimeSpan delay, in CancellationToken token ) => Task.Delay(delay, token);


    public static void CallSynchronously( this Task task ) => task.ConfigureAwait(false)
                                                                  .GetAwaiter()
                                                                  .GetResult();
    public static T CallSynchronously<T>( this Task<T> task ) => task.ConfigureAwait(false)
                                                                     .GetAwaiter()
                                                                     .GetResult();
    public static void CallSynchronously( this ValueTask task ) => task.ConfigureAwait(false)
                                                                       .GetAwaiter()
                                                                       .GetResult();
    public static T CallSynchronously<T>( this ValueTask<T> task ) => task.ConfigureAwait(false)
                                                                          .GetAwaiter()
                                                                          .GetResult();


    public static bool WaitSynchronously( this Task task, CancellationToken token = default )
    {
        task.Wait(token);
        return task.IsCompletedSuccessfully;
    }
    public static T WaitSynchronously<T>( this Task<T> task, CancellationToken token = default )
    {
        task.Wait(token);
        return task.Result;
    }


    public static bool WaitSynchronously( this ValueTask task )
    {
        task.GetAwaiter()
            .GetResult();

        return task.IsCompletedSuccessfully;
    }
    public static T WaitSynchronously<T>( this ValueTask<T> task )
    {
        task.GetAwaiter()
            .GetResult();

        return task.Result;
    }


    public static Task WhenAny( this                         IEnumerable<Task>          tasks ) => Task.WhenAny(tasks);
    public static Task<Task<TResult>> WhenAny<TResult>( this IEnumerable<Task<TResult>> tasks ) => Task.WhenAny(tasks);
    public static Task WhenAll( this                         IEnumerable<Task>          tasks ) => Task.WhenAll(tasks);
    public static Task<TResult[]> WhenAll<TResult>( this     IEnumerable<Task<TResult>> tasks ) => Task.WhenAll(tasks);
    public static void WaitAll( this                         IEnumerable<Task>          tasks, CancellationToken token = default ) => Task.WaitAll(tasks.ToArray(), token);
    public static int WaitAny( this                          IEnumerable<Task>          tasks, CancellationToken token = default ) => Task.WaitAny(tasks.ToArray(), token);


    public static async Task TaskFromCanceled( this                    CancellationToken token ) => await Task.FromCanceled(token);
    public static async Task<TResult> TaskFromCanceled<TResult>( this  CancellationToken token ) => await Task.FromCanceled<TResult>(token);
    public static async Task TaskFromException( this                   Exception         e ) => await Task.FromException(e);
    public static async Task<TResult> TaskFromException<TResult>( this Exception         e ) => await Task.FromException<TResult>(e);
    public static async Task<TResult> TaskFromResult<TResult>( this    TResult           result ) => await Task.FromResult(result);


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

    /// <summary>
    /// <seealso href = "https://gist.github.com/scattered-code/1a4a01c3f3a24ebce293a6e7b4451254" />
    /// <para> Also a debugging improvement: <see href = "https://youtu.be/gW19LaAYczI?t=497" /> </para> </summary>
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


    /// <summary> <seealso href = "https://gist.github.com/scattered-code/b834bbc355a9ee710e3147321d6f985a" /> </summary>
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
