namespace Jakar.Extensions;


public static partial class Tasks
{
    /// <summary> Creates a cancellable delay from the given <paramref name="delay"/> </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Task Delay( this TimeSpan delay, CancellationToken token = default ) => Task.Delay(delay, token);


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void CallSynchronously( this Task task ) => task.GetAwaiter()
                                                                                                                     .GetResult();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TValue CallSynchronously<TValue>( this Task<TValue> task ) => task.GetAwaiter()
                                                                                                                                       .GetResult();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void CallSynchronously( this ValueTask task ) => task.GetAwaiter()
                                                                                                                          .GetResult();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TValue CallSynchronously<TValue>( this ValueTask<TValue> task ) => task.GetAwaiter()
                                                                                                                                            .GetResult();


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool WaitSynchronously( this Task task, CancellationToken token = default )
    {
        task.Wait(token);
        return task.IsCompletedSuccessfully;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TValue WaitSynchronously<TValue>( this Task<TValue> task, CancellationToken token = default )
    {
        task.Wait(token);
        return task.Result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool WaitSynchronously( this ValueTask task )
    {
        task.GetAwaiter()
            .GetResult();

        return task.IsCompletedSuccessfully;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static TValue WaitSynchronously<TValue>( this ValueTask<TValue> task )
    {
        task.GetAwaiter()
            .GetResult();

        return task.Result;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Task         Run( this         Func<Task>                                 func, CancellationToken token = default ) => Task.Run(func,                                    token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Task         Run( this         Func<ValueTask>                            func, CancellationToken token = default ) => Task.Run(new Caller(func, token).Execute,         token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Task<TValue> Run<TValue>( this Func<Task<TValue>>                         func, CancellationToken token = default ) => Task.Run(func,                                    token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Task<TValue> Run<TValue>( this Func<ValueTask<TValue>>                    func, CancellationToken token = default ) => Task.Run(new Caller<TValue>(func, token).Execute, token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Task         Run( this         Func<CancellationToken, Task>              func, CancellationToken token = default ) => Task.Run(new Caller(func, token).Execute,         token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Task         Run( this         Func<CancellationToken, ValueTask>         func, CancellationToken token = default ) => Task.Run(new Caller(func, token).Execute,         token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Task<TValue> Run<TValue>( this Func<CancellationToken, Task<TValue>>      func, CancellationToken token = default ) => Task.Run(new Caller<TValue>(func, token).Execute, token);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Task<TValue> Run<TValue>( this Func<CancellationToken, ValueTask<TValue>> func, CancellationToken token = default ) => Task.Run(new Caller<TValue>(func, token).Execute, token);


    [MethodImpl(MethodImplOptions.AggressiveInlining)] [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")]
    public static Task WhenAny( this IEnumerable<Task> tasks ) => Task.WhenAny(tasks.GetInternalArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)] [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")]
    public static Task<Task<TResult>> WhenAny<TResult>( this IEnumerable<Task<TResult>> tasks ) => Task.WhenAny(tasks.GetInternalArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)] [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")]
    public static Task WhenAll( this IEnumerable<Task> tasks ) => Task.WhenAll(tasks.GetInternalArray());

    [MethodImpl(MethodImplOptions.AggressiveInlining)] [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")]
    public static Task<TResult[]> WhenAll<TResult>( this IEnumerable<Task<TResult>> tasks ) => Task.WhenAll(tasks.GetInternalArray());

    extension( IEnumerable<Task> tasks )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")]
        public void WaitAll( CancellationToken token = default ) => tasks.WhenAll()
                                                                         .Wait(token);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")]
        public int WaitAny( CancellationToken token = default ) => Task.WaitAny(tasks.ToArray(), token);
    }



    public static async IAsyncEnumerable<TValue> WheneverAny<TValue>( this IEnumerable<ValueTask<TValue>> tasks, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach ( TValue result in tasks.Select(static x => x.AsTask())
                                              .WheneverAny(token) ) { yield return result; }
    }
    public static async IAsyncEnumerable<TValue> WheneverAny<TValue>( this IEnumerable<Task<TValue>> tasks, [EnumeratorCancellation] CancellationToken token = default )
    {
        List<Task<TValue>> list = [..tasks];

        while ( token.ShouldContinue() && list.Count > 0 )
        {
            Task<TValue> task = await Task.WhenAny(list);
            list.Remove(task);
            yield return await task;
        }
    }


    /// <summary>
    ///     <see href="https://stackoverflow.com/a/63141544/9530917"/>
    /// </summary>
    /// <exception cref="AggregateException"> </exception>
    public static async ValueTask WhenAll( this IEnumerable<ValueTask> tasks ) => await Task.WhenAll(tasks.Select(static x => x.AsTask()));
    public static async ValueTask<TResult[]> WhenAll<TResult>( this IEnumerable<ValueTask<TResult>> tasks ) => await Task.WhenAll(tasks.Select(static x => x.AsTask()));


    public static async ValueTask WhenAll( this IEnumerable<Func<CancellationToken, ValueTask>> funcs, CancellationToken token = default )
    {
        ParallelOptions options = new()
                                  {
                                      CancellationToken      = token,
                                      MaxDegreeOfParallelism = Environment.ProcessorCount
                                  };

        await Parallel.ForEachAsync(funcs, options, executorAsync);
        return;

        static ValueTask executorAsync( Func<CancellationToken, ValueTask> task, CancellationToken token ) => task(token);
    }


    /// <summary>
    ///     <see href="https://stackoverflow.com/a/63141544/9530917"/>
    /// </summary>
    /// <exception cref="AggregateException"> </exception>
    public static async ValueTask<TResult[]> WhenAll<TResult>( this IEnumerable<Func<CancellationToken, ValueTask<TResult>>> funcs, CancellationToken token = default )
    {
        ParallelOptions options = new()
                                  {
                                      CancellationToken      = token,
                                      MaxDegreeOfParallelism = Environment.ProcessorCount
                                  };

        ConcurrentBag<TResult>                        results = [];
        Func<CancellationToken, ValueTask<TResult>>[] tasks   = funcs.ToArray();
        await Parallel.ForAsync(0, tasks.Length, options, executor);

        return results.ToArray();

        async ValueTask executor( int i, CancellationToken cancellationToken )
        {
            Func<CancellationToken, ValueTask<TResult>> task   = tasks[i];
            TResult                                     result = await task(cancellationToken);
            results.Add(result);
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Task                WhenAny( this          IEnumerable<ValueTask>          tasks ) => Task.WhenAny(tasks.Select(static x => x.AsTask()));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Task<Task<TResult>> WhenAny<TResult>( this IEnumerable<ValueTask<TResult>> tasks ) => Task.WhenAny(tasks.Select(static x => x.AsTask()));
    extension( IEnumerable<ValueTask> tasks )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void WaitAll( CancellationToken token = default ) => Task.WaitAll(tasks.Select(static x => x.AsTask())
                                                                                                                                         .ToArray(),
                                                                                                                                    token);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int WaitAny( CancellationToken token = default ) => Task.WaitAny(tasks.Select(static x => x.AsTask())
                                                                                                                                        .ToArray(),
                                                                                                                                   token);
    }



    extension( CancellationToken               token )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Task          AsTask()          => Task.FromCanceled(token);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Task<TResult> AsTask<TResult>() => Task.FromCanceled<TResult>(token);
    }



    extension( Exception               e )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Task          AsTask()          => Task.FromException(e);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Task<TResult> AsTask<TResult>() => Task.FromException<TResult>(e);
    }



    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Task<TResult> AsTask<TResult>( this TResult   result ) => Task.FromResult(result);


    extension( CancellationToken token )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ValueTask          AsValueTask()          => ValueTask.FromCanceled(token);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ValueTask<TResult> AsValueTask<TResult>() => ValueTask.FromCanceled<TResult>(token);
    }



    extension( Exception         e )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ValueTask          AsValueTask()          => ValueTask.FromException(e);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ValueTask<TResult> AsValueTask<TResult>() => ValueTask.FromException<TResult>(e);
    }



    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static ValueTask<TResult> AsValueTask<TResult>( this TResult           result ) => ValueTask.FromResult(result);
}
