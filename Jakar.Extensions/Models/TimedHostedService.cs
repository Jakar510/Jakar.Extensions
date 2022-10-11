// // TrueLogic :: TrueKeep.Client.Models
// // 04/05/2022  6:25 PM
//
// using System.Threading.Channels;
//
//
//
// namespace Jakar.Extensions;
//
//
// /// <summary>
// ///     <para>
// ///         <see href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio"/>
// ///     </para>
// /// </summary>
// public abstract class TimedHostedService<T> : Service, IHostedService where T : TimedHostedService<T>
// {
//     private readonly ILogger<T> _logger;
//     private          Timer      _timer = null!;
//
//     protected TimedHostedService( ILogger<T> logger ) => _logger = logger;
//
//     protected abstract void DoWork( object? state );
//     public Task StartAsync( TimeSpan period, CancellationToken token )
//     {
//         _logger.LogInformation("Timed Hosted Service running.");
//
//         _timer = new Timer(DoWork, null, TimeSpan.Zero, period);
//
//         return Task.CompletedTask;
//     }
//     protected override void Dispose( bool disposing )
//     {
//         if ( disposing ) { _timer.Dispose(); }
//     }
//
//     public Task StartAsync( CancellationToken token ) => StartAsync(TimeSpan.FromSeconds(5), token);
//
//     public Task StopAsync( CancellationToken token )
//     {
//         _timer.Change(Timeout.Infinite, 0);
//
//         return Task.CompletedTask;
//     }
// }
//
//
//
// /// <summary>
// ///     <para>
// ///         <see href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio"/>
// ///     </para>
// /// </summary>
// public sealed class BackgroundTaskQueue
// {
//     private readonly Channel<Func<CancellationToken, ValueTask>> _queue;
//
//     public BackgroundTaskQueue( int capacity )
//     {
//         // Capacity should be set based on the expected application load and number of concurrent threads accessing the queue.            
//         // BoundedChannelFullMode.Wait will cause calls to WriteAsync() to return a task, which completes only when space became available.
//         // This leads to backpressure, in case too many publishers/calls start accumulating.
//         var options = new BoundedChannelOptions(capacity)
//                       {
//                           FullMode = BoundedChannelFullMode.Wait
//                       };
//
//         _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
//     }
//
//     public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync( CancellationToken cancellationToken )
//     {
//         Func<CancellationToken, ValueTask> workItem = await _queue.Reader.ReadAsync(cancellationToken);
//
//         return workItem;
//     }
//     public async ValueTask QueueBackgroundWorkItemAsync( Func<CancellationToken, ValueTask> workItem )
//     {
//         if ( workItem == null ) { throw new ArgumentNullException(nameof(workItem)); }
//
//         await _queue.Writer.WriteAsync(workItem);
//     }
// }
//
//
//
// /// <summary>
// ///     <para>
// ///         <see href="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=visual-studio"/>
// ///     </para>
// /// </summary>
// public sealed class QueuedHostedService : Service, IHostedService
// {
//     private readonly ILogger<QueuedHostedService> _logger;
//     public           BackgroundTaskQueue          TaskQueue { get; }
//
//
//     public QueuedHostedService( BackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger )
//     {
//         TaskQueue = taskQueue;
//         _logger   = logger;
//     }
//     protected override void Dispose( bool disposing ) { }
//     public override async ValueTask DisposeAsync() { }
//
//
//     public async Task StartAsync( CancellationToken token )
//     {
//         _logger.LogInformation("Queued Hosted Service is running.");
//
//         while ( !token.IsCancellationRequested )
//         {
//             Func<CancellationToken, ValueTask> workItem = await TaskQueue.DequeueAsync(token);
//
//             try { await workItem(token); }
//             catch ( Exception ex ) { _logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem)); }
//         }
//     }
//
//     public async Task StopAsync( CancellationToken token )
//     {
//         _logger.LogInformation("Queued Hosted Service is stopping.");
//
//         await ValueTask.CompletedTask;
//     }
// }



