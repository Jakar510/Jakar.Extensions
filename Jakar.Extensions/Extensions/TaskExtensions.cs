#nullable enable
namespace Jakar.Extensions;


public static class Tasks
{
    /// <summary> Creates a cancellable delay from the given <paramref name="delay"/> </summary>
    public static Task Delay( this TimeSpan delay, in CancellationToken token = default ) => Task.Delay(delay, token);


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


    public static Task WhenAny( this                         IEnumerable<ValueTask>          tasks ) => Task.WhenAny(tasks.Select(x => x.AsTask()));
    public static Task<Task<TResult>> WhenAny<TResult>( this IEnumerable<ValueTask<TResult>> tasks ) => Task.WhenAny(tasks.Select(x => x.AsTask()));
    public static Task WhenAll( this                         IEnumerable<ValueTask>          tasks ) => Task.WhenAll(tasks.Select(x => x.AsTask()));
    public static Task<TResult[]> WhenAll<TResult>( this     IEnumerable<ValueTask<TResult>> tasks ) => Task.WhenAll(tasks.Select(x => x.AsTask()));
    public static void WaitAll( this IEnumerable<ValueTask> tasks, CancellationToken token = default ) => Task.WaitAll(tasks.Select(x => x.AsTask())
                                                                                                                            .ToArray(),
                                                                                                                       token);
    public static int WaitAny( this IEnumerable<ValueTask> tasks, CancellationToken token = default ) => Task.WaitAny(tasks.Select(x => x.AsTask())
                                                                                                                           .ToArray(),
                                                                                                                      token);


    public static Task TaskFromCanceled( this                    CancellationToken token ) => Task.FromCanceled(token);
    public static Task<TResult> TaskFromCanceled<TResult>( this  CancellationToken token ) => Task.FromCanceled<TResult>(token);
    public static Task TaskFromException( this                   Exception         e ) => Task.FromException(e);
    public static Task<TResult> TaskFromException<TResult>( this Exception         e ) => Task.FromException<TResult>(e);
    public static Task<TResult> TaskFromResult<TResult>( this    TResult           result ) => Task.FromResult(result);


#if NETSTANDARD2_1
    public static ValueTask ValueTaskFromCanceled( this                    CancellationToken token ) => new(token.TaskFromCanceled());
    public static ValueTask<TResult> ValueTaskFromCanceled<TResult>( this  CancellationToken token ) => new(token.TaskFromCanceled<TResult>());
    public static ValueTask ValueTaskFromException( this                   Exception         e ) => new(e.TaskFromException());
    public static ValueTask<TResult> ValueTaskFromException<TResult>( this Exception         e ) => new(e.TaskFromException<TResult>());
    public static ValueTask<TResult> ValueTaskFromResult<TResult>( this    TResult           result ) => new(result);

#else
    public static ValueTask ValueTaskFromCanceled( this                    CancellationToken token ) => ValueTask.FromCanceled(token);
    public static ValueTask<TResult> ValueTaskFromCanceled<TResult>( this  CancellationToken token ) => ValueTask.FromCanceled<TResult>(token);
    public static ValueTask ValueTaskFromException( this                   Exception         e ) => ValueTask.FromException(e);
    public static ValueTask<TResult> ValueTaskFromException<TResult>( this Exception         e ) => ValueTask.FromException<TResult>(e);
    public static ValueTask<TResult> ValueTaskFromResult<TResult>( this    TResult           result ) => ValueTask.FromResult(result);

#endif
}
