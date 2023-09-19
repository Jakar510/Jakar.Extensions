// Jakar.Extensions :: Jakar.Extensions.Hosting
// 09/13/2022  12:20 PM


using Microsoft.Extensions.Hosting;



namespace Jakar.Extensions;


public static class HostedServiceExtensions
{
    public static ServiceThread StartInThread( this IHostedService service, ILogger<ServiceThread> logger, CancellationToken token )
    {
        var thread = new ServiceThread( service, logger, token );
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
                          IsBackground = true,
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

            _source?.Cancel();
            _source?.Dispose();
            _source = new CancellationTokenSource();

            await using ( _token.Register( _source.Cancel ) )
            {
                try
                {
                    IsAlive = true;

                    try { await _service.StartAsync( _source.Token ); }
                    finally { await _service.StopAsync( default ); }
                }
                catch ( TaskCanceledException ) { }
                catch ( Exception e )
                {
                    _logger.LogCritical( e,
                                         "{ClassName}.{Caller} -> '{ServiceName}'",
                                         nameof(ServiceThread),
                                         nameof(ThreadStart),
                                         _service.GetType()
                                                 .FullName );
                }
                finally
                {
                    _logger.LogDebug( "{ClassName}.{Caller} -> '{ServiceName}' -> {Cancelled}",
                                      nameof(ServiceThread),
                                      nameof(ThreadStart),
                                      _service.GetType()
                                              .FullName,
                                      _token.IsCancellationRequested );

                    IsAlive = false;
                    _source = default;
                }
            }
        }
    }
}
