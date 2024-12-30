// Jakar.Extensions :: Jakar.Extensions
// 05/17/2022  4:16 PM

namespace Jakar.Database;


public abstract class Service : ObservableClass, IAsyncDisposable, IValidator
{
    private readonly Synchronized<bool> _isAlive = new(false);
    public           string             ClassName { get; }
    public           string             FullName  { get; }


    public virtual bool IsAlive
    {
        get => _isAlive.Value;
        protected set
        {
            _isAlive.Value = value;
            OnPropertyChanged();
            OnPropertyChanged( nameof(IsValid) );
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


    public static Task Delay( in double   days,    in CancellationToken token ) => Delay( TimeSpan.FromDays( days ),       token );
    public static Task Delay( in float    minutes, in CancellationToken token ) => Delay( TimeSpan.FromMinutes( minutes ), token );
    public static Task Delay( in long     seconds, in CancellationToken token ) => Delay( TimeSpan.FromSeconds( seconds ), token );
    public static Task Delay( in int      ms,      in CancellationToken token ) => Delay( TimeSpan.FromMilliseconds( ms ), token );
    public static Task Delay( in TimeSpan delay,   in CancellationToken token ) => delay.Delay( token );


#if NET6_0_OR_GREATER
    [StackTraceHidden, DoesNotReturn]
#endif

    protected virtual void ThrowDisabled( Exception? inner = null, [CallerMemberName] string? caller = null ) => throw new ApiDisabledException( $"{ClassName}.{caller}", inner );


#if NET6_0_OR_GREATER
    [StackTraceHidden, DoesNotReturn]
#endif

    protected void ThrowDisposed( Exception? inner = null, [CallerMemberName] string? caller = null ) => throw new ObjectDisposedException( $"{ClassName}.{caller}", inner );
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
        ServiceThread thread = new ServiceThread( service, logger, token );
        thread.Start();
        return thread;
    }



    public sealed class ServiceThread : Service
    {
        private readonly CancellationToken        _token;
        private readonly IHostedService           _service;
        private readonly ILogger                  _logger;
        private readonly Thread                   _thread;
        private          CancellationTokenSource? _source;


        public ServiceThread( IHostedService service, ILoggerFactory factory, CancellationToken token = default ) : this( service, factory.CreateLogger<ServiceThread>(), token ) { }
        public ServiceThread( IHostedService service, ILogger logger, CancellationToken token = default )
        {
            _service = service;
            _logger  = logger;
            _token   = token;

            _thread = new Thread( ThreadStart )
                      {
                          Name         = $"{nameof(ServiceThread)}.{service.GetType().Name}",
                          IsBackground = true
                      };
        }
        public bool Stop( in TimeSpan timeout )
        {
            _source?.Cancel();
            _source?.Dispose();
            return _thread.Join( timeout );
        }
        public override ValueTask DisposeAsync()
        {
            Stop();
            return default;
        }


        public void Start()
        {
            if ( IsAlive ) { return; }

            _thread.Start();
        }


        public void Stop()
        {
            _source?.Cancel();
            _source?.Dispose();
            _thread.Join();
        }
        private async void ThreadStart()
        {
            if ( IsAlive ) { return; }

            if ( _source is not null )
            {
                await _source.CancelAsync().ConfigureAwait( false );
                _source.Dispose();
            }

            _source = new CancellationTokenSource();

            await using ( _token.Register( _source.Cancel ) )
            {
                try
                {
                    IsAlive = true;

                    try { await _service.StartAsync( _source.Token ).ConfigureAwait( false ); }
                    finally { await _service.StopAsync( CancellationToken.None ).ConfigureAwait( false ); }
                }
                catch ( TaskCanceledException ) { }
                catch ( Exception e ) { Log.ServiceError( _logger, e, this, _service ); }
                finally
                {
                    Log.ServiceStopped( _logger, this, _service, _token );
                    IsAlive = false;
                    _source = null;
                }
            }
        }
    }
}
