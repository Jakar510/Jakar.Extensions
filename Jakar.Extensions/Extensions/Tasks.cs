﻿namespace Jakar.Extensions;


public static partial class Tasks
{
    /// <summary> Creates a cancellable delay from the given <paramref name="delay"/> </summary>
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static Task Delay( this TimeSpan delay, CancellationToken token = default ) => Task.Delay( delay, token );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void CallSynchronously( this    Task         task ) => task.ConfigureAwait( false ).GetAwaiter().GetResult();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static T    CallSynchronously<T>( this Task<T>      task ) => task.ConfigureAwait( false ).GetAwaiter().GetResult();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void CallSynchronously( this    ValueTask    task ) => task.ConfigureAwait( false ).GetAwaiter().GetResult();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static T    CallSynchronously<T>( this ValueTask<T> task ) => task.ConfigureAwait( false ).GetAwaiter().GetResult();

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool WaitSynchronously( this Task task, CancellationToken token = default )
    {
        task.Wait( token );
        return task.IsCompletedSuccessfully;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static T WaitSynchronously<T>( this Task<T> task, CancellationToken token = default )
    {
        task.Wait( token );
        return task.Result;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool WaitSynchronously( this ValueTask task )
    {
        task.ConfigureAwait( false ).GetAwaiter().GetResult();

        return task.IsCompletedSuccessfully;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static T WaitSynchronously<T>( this ValueTask<T> task )
    {
        task.ConfigureAwait( false ).GetAwaiter().GetResult();

        return task.Result;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task    Run( this    Func<Task>                            func, CancellationToken token = default ) => Task.Run( func,                                 token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task    Run( this    Func<ValueTask>                       func, CancellationToken token = default ) => Task.Run( async () => await func(),             token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task<T> Run<T>( this Func<Task<T>>                         func, CancellationToken token = default ) => Task.Run( async () => await func(),             token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task<T> Run<T>( this Func<ValueTask<T>>                    func, CancellationToken token = default ) => Task.Run( async () => await func(),             token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task    Run( this    Func<CancellationToken, Task>         func, CancellationToken token = default ) => Task.Run( new Caller( func, token ).Execute,    token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task    Run( this    Func<CancellationToken, ValueTask>    func, CancellationToken token = default ) => Task.Run( new Caller( func, token ).Execute,    token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task<T> Run<T>( this Func<CancellationToken, Task<T>>      func, CancellationToken token = default ) => Task.Run( new Caller<T>( func, token ).Execute, token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task<T> Run<T>( this Func<CancellationToken, ValueTask<T>> func, CancellationToken token = default ) => Task.Run( new Caller<T>( func, token ).Execute, token );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task                WhenAny( this          IEnumerable<Task>          tasks )                                    => Task.WhenAny( tasks.ToArray() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task<Task<TResult>> WhenAny<TResult>( this IEnumerable<Task<TResult>> tasks )                                    => Task.WhenAny( tasks.ToArray() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task                WhenAll( this          IEnumerable<Task>          tasks )                                    => Task.WhenAll( tasks.ToArray() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task<TResult[]>     WhenAll<TResult>( this IEnumerable<Task<TResult>> tasks )                                    => Task.WhenAll( tasks.ToArray() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void                WaitAll( this          IEnumerable<Task>          tasks, CancellationToken token = default ) => tasks.WhenAll().Wait( token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static int                 WaitAny( this          IEnumerable<Task>          tasks, CancellationToken token = default ) => Task.WaitAny( tasks.ToArray(), token );


    /// <summary>
    ///     <see href="https://stackoverflow.com/a/63141544/9530917"/>
    /// </summary>
    /// <exception cref="AggregateException"> </exception>
    public static async ValueTask WhenAll( this IEnumerable<ValueTask> tasks ) => await Task.WhenAll( tasks.Select( x => x.AsTask() ) );
    public static async ValueTask<TResult[]> WhenAll<TResult>( this IEnumerable<ValueTask<TResult>> tasks ) => await Task.WhenAll( tasks.Select( x => x.AsTask() ) );


    public static async ValueTask WhenAll( this IEnumerable<Func<CancellationToken, ValueTask>> funcs, CancellationToken token = default )
    {
    #if NET6_0_OR_GREATER
        var options = new ParallelOptions
                      {
                          CancellationToken = token,
                          MaxDegreeOfParallelism = Environment.ProcessorCount
                      };

        await Parallel.ForEachAsync( funcs, options, ExecutorAsync );

        static ValueTask ExecutorAsync( Func<CancellationToken, ValueTask> task, CancellationToken token ) => task( token );
    #else
        ValueTask[] tasks = funcs.ToArray( x => x( token ) );
        await tasks.WhenAll();
    #endif
    }
    /// <summary>
    ///     <see href="https://stackoverflow.com/a/63141544/9530917"/>
    /// </summary>
    /// <exception cref="AggregateException"> </exception>
    public static async ValueTask<TResult[]> WhenAll<TResult>( this IEnumerable<Func<CancellationToken, ValueTask<TResult>>> funcs, CancellationToken token = default )
    {
    #if NET8_0_OR_GREATER
        var options = new ParallelOptions
                      {
                          CancellationToken = token,
                          MaxDegreeOfParallelism = Environment.ProcessorCount
                      };

        var results = new ConcurrentBag<TResult>();
        Func<CancellationToken, ValueTask<TResult>>[] tasks = funcs.ToArray();
        await Parallel.ForAsync( 0, tasks.Length, options, Executor );

        return results.ToArray();

        async ValueTask Executor( int i, CancellationToken token )
        {
            Func<CancellationToken, ValueTask<TResult>> task = tasks[i];
            TResult                                     result = await task( token ).ConfigureAwait( false );
            results.Add( result );
        }
    #else
        ValueTask<TResult>[] tasks = funcs.ToArray( x => x( token ) );
        return await tasks.WhenAll();
    #endif
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task                WhenAny( this          IEnumerable<ValueTask>          tasks )                                    => Task.WhenAny( tasks.Select( x => x.AsTask() ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task<Task<TResult>> WhenAny<TResult>( this IEnumerable<ValueTask<TResult>> tasks )                                    => Task.WhenAny( tasks.Select( x => x.AsTask() ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void                WaitAll( this          IEnumerable<ValueTask>          tasks, CancellationToken token = default ) => Task.WaitAll( tasks.Select( x => x.AsTask() ).ToArray(), token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static int                 WaitAny( this          IEnumerable<ValueTask>          tasks, CancellationToken token = default ) => Task.WaitAny( tasks.Select( x => x.AsTask() ).ToArray(), token );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task          TaskFromCanceled( this           CancellationToken token )  => Task.FromCanceled( token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task<TResult> TaskFromCanceled<TResult>( this  CancellationToken token )  => Task.FromCanceled<TResult>( token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task          TaskFromException( this          Exception         e )      => Task.FromException( e );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task<TResult> TaskFromException<TResult>( this Exception         e )      => Task.FromException<TResult>( e );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Task<TResult> TaskFromResult<TResult>( this    TResult           result ) => Task.FromResult( result );


#if NETSTANDARD2_1
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ValueTask          ValueTaskFromCanceled( this           CancellationToken token )  => new(token.TaskFromCanceled());
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ValueTask<TResult> ValueTaskFromCanceled<TResult>( this  CancellationToken token )  => new(token.TaskFromCanceled<TResult>());
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ValueTask          ValueTaskFromException( this          Exception         e )      => new(e.TaskFromException());
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ValueTask<TResult> ValueTaskFromException<TResult>( this Exception         e )      => new(e.TaskFromException<TResult>());
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ValueTask<TResult> ValueTaskFromResult<TResult>( this    TResult           result ) => new(result);

#else
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ValueTask          ValueTaskFromCanceled( this           CancellationToken token )  => ValueTask.FromCanceled( token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ValueTask<TResult> ValueTaskFromCanceled<TResult>( this  CancellationToken token )  => ValueTask.FromCanceled<TResult>( token );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ValueTask          ValueTaskFromException( this          Exception         e )      => ValueTask.FromException( e );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ValueTask<TResult> ValueTaskFromException<TResult>( this Exception         e )      => ValueTask.FromException<TResult>( e );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ValueTask<TResult> ValueTaskFromResult<TResult>( this    TResult           result ) => ValueTask.FromResult( result );

#endif
}
