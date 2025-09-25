// Jakar.Extensions :: Jakar.Extensions
// 05/17/2022  4:16 PM

using ILogger = Microsoft.Extensions.Logging.ILogger;



namespace Jakar.Database;


public abstract class Service : BaseClass, IAsyncDisposable, IValidator
{
    private readonly SynchronizedValue<bool> __isAlive = new(false);
    public           string                  ClassName { get; }
    public           string                  FullName  { get; }


    public virtual bool IsAlive
    {
        get => __isAlive.Value;
        protected set
        {
            __isAlive.Value = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsValid));
        }
    }
    public virtual bool IsValid => IsAlive;


    protected Service()
    {
        Type type = GetType();
        ClassName = type.Name;
        FullName  = type.AssemblyQualifiedName ?? type.FullName ?? ClassName;
    }


    public virtual ValueTask DisposeAsync() => default;


    public static Task Delay( in double   days,    in CancellationToken token ) => Delay(TimeSpan.FromDays(days),       token);
    public static Task Delay( in float    minutes, in CancellationToken token ) => Delay(TimeSpan.FromMinutes(minutes), token);
    public static Task Delay( in long     seconds, in CancellationToken token ) => Delay(TimeSpan.FromSeconds(seconds), token);
    public static Task Delay( in int      ms,      in CancellationToken token ) => Delay(TimeSpan.FromMilliseconds(ms), token);
    public static Task Delay( in TimeSpan delay,   in CancellationToken token ) => delay.Delay(token);


#if NET6_0_OR_GREATER
    [StackTraceHidden, DoesNotReturn]
#endif

    protected virtual void ThrowDisabled( Exception? inner = null, [CallerMemberName] string? caller = null ) => throw new ApiDisabledException($"{ClassName}.{caller}", inner);


#if NET6_0_OR_GREATER
    [StackTraceHidden, DoesNotReturn]
#endif

    protected void ThrowDisposed( Exception? inner = null, [CallerMemberName] string? caller = null ) => throw new ObjectDisposedException($"{ClassName}.{caller}", inner);
}



public abstract class HostedService : Service, IHostedService
{
    public abstract Task StartAsync( CancellationToken token );
    public abstract Task StopAsync( CancellationToken  token );
}



public static class HostedServiceExtensions
{
    public static ServiceThread StartInThread( this IHostedService service, ILogger<ServiceThread> logger, CancellationToken token )
    {
        ServiceThread thread = new(service, logger, token);
        thread.Start();
        return thread;
    }



    public sealed class ServiceThread : Service
    {
        private readonly CancellationToken        __token;
        private readonly IHostedService           __service;
        private readonly ILogger                  __logger;
        private readonly Thread                   __thread;
        private          CancellationTokenSource? __source;


        public ServiceThread( IHostedService service, ILoggerFactory factory, CancellationToken token = default ) : this(service, factory.CreateLogger<ServiceThread>(), token) { }
        public ServiceThread( IHostedService service, ILogger logger, CancellationToken token = default )
        {
            __service = service;
            __logger  = logger;
            __token   = token;

            __thread = new Thread(ThreadStart)
                       {
                           Name         = $"{nameof(ServiceThread)}.{service.GetType().Name}",
                           IsBackground = true
                       };
        }
        public bool Stop( in TimeSpan timeout )
        {
            __source?.Cancel();
            __source?.Dispose();
            return __thread.Join(timeout);
        }
        public override ValueTask DisposeAsync()
        {
            Stop();
            return default;
        }


        public void Start()
        {
            if ( IsAlive ) { return; }

            __thread.Start();
        }


        public void Stop()
        {
            __source?.Cancel();
            __source?.Dispose();
            __thread.Join();
        }
        private async void ThreadStart()
        {
            if ( IsAlive ) { return; }

            if ( __source is not null )
            {
                await __source.CancelAsync().ConfigureAwait(false);
                __source.Dispose();
            }

            __source = new CancellationTokenSource();

            await using ( __token.Register(__source.Cancel) )
            {
                try
                {
                    IsAlive = true;

                    try { await __service.StartAsync(__source.Token).ConfigureAwait(false); }
                    finally { await __service.StopAsync(CancellationToken.None).ConfigureAwait(false); }
                }
                catch ( TaskCanceledException ) { }
                catch ( Exception e ) { DbLog.ServiceError(__logger, e, this, __service); }
                finally
                {
                    DbLog.ServiceStopped(__logger, this, __service, __token);
                    IsAlive  = false;
                    __source = null;
                }
            }
        }
    }
}
