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



    extension<TResult>( IEnumerable<Task<TResult>> self )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")]
        public Task<Task<TResult>> WhenAny() => Task.WhenAny(self.GetInternalArray());

        [MethodImpl(MethodImplOptions.AggressiveInlining)] [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")]
        public Task<TResult[]> WhenAll() => Task.WhenAll(self.GetInternalArray());

        public async IAsyncEnumerable<TResult> WheneverAny( [EnumeratorCancellation] CancellationToken token = default )
        {
            List<Task<TResult>> list = [..self];

            while ( token.ShouldContinue() && list.Count > 0 )
            {
                Task<TResult> task = await Task.WhenAny(list);
                list.Remove(task);
                yield return await task;
            }
        }
    }



    extension( IEnumerable<Task> self )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")]
        public void WaitAll( CancellationToken token = default ) => self.WhenAll()
                                                                        .Wait(token);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")]
        public int WaitAny( CancellationToken token = default ) => Task.WaitAny(self.ToArray(), token);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")]
        public Task WhenAny() => Task.WhenAny(self.GetInternalArray());

        [MethodImpl(MethodImplOptions.AggressiveInlining)] [RequiresDynamicCode("Jakar.Extensions.ArrayExtensions.ArrayAccessor<TElement>.GetCollectionGetter()")]
        public Task WhenAll() => Task.WhenAll(self.GetInternalArray());
    }



    extension( IEnumerable<ValueTask> self )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void WaitAll( CancellationToken token = default ) => Task.WaitAll(self.Select(static x => x.AsTask())
                                                                                                                                        .ToArray(),
                                                                                                                                    token);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int WaitAny( CancellationToken token = default ) => Task.WaitAny(self.Select(static x => x.AsTask())
                                                                                                                                       .ToArray(),
                                                                                                                                   token);
      
        /// <summary>
        ///     <see href="https://stackoverflow.com/a/63141544/9530917"/>
        /// </summary>
        /// <exception cref="AggregateException"> </exception>
        public async ValueTask WhenAll() => await Task.WhenAll(self.Select(static x => x.AsTask()));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Task WhenAny() => Task.WhenAny(self.Select(static x => x.AsTask()));
    }



    extension<TValue>( IEnumerable<ValueTask<TValue>> self )
    {
        public async IAsyncEnumerable<TValue> WheneverAny( [EnumeratorCancellation] CancellationToken token = default )
        {
            await foreach ( TValue result in self.Select(static x => x.AsTask())
                                                 .WheneverAny(token) ) { yield return result; }
        }
        public async ValueTask<TValue[]> WhenAll() => await Task.WhenAll(self.Select(static x => x.AsTask()));
        public async ValueTask<TValue> WhenAny()
        {
            Task<TValue> result = await Task.WhenAny(self.Select(static x => x.AsTask()));
            return await result;
        }
    }



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



    extension( CancellationToken self )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Task               AsTask()               => Task.FromCanceled(self);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Task<TResult>      AsTask<TResult>()      => Task.FromCanceled<TResult>(self);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ValueTask          AsValueTask()          => ValueTask.FromCanceled(self);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ValueTask<TResult> AsValueTask<TResult>() => ValueTask.FromCanceled<TResult>(self);
    }



    extension( Exception self )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Task               AsTask()               => Task.FromException(self);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Task<TResult>      AsTask<TResult>()      => Task.FromException<TResult>(self);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ValueTask          AsValueTask()          => ValueTask.FromException(self);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ValueTask<TResult> AsValueTask<TResult>() => ValueTask.FromException<TResult>(self);
    }



    extension<TResult>( TResult result )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public Task<TResult>      AsTask()      => Task.FromResult(result);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ValueTask<TResult> AsValueTask() => ValueTask.FromResult(result);
    }
}
