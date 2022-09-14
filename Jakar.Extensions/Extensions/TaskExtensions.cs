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
}
