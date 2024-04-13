using Microsoft.Extensions.Hosting;



namespace Jakar.Extensions.Loggers;


#if NET8_0_OR_GREATER



[Experimental( nameof(FileLoggerProvider) )]
public sealed class FileLoggerProviderOptions : IOptions<FileLoggerProviderOptions>
{
    public const string                                           FILE_NAME = "App.logs";
    public       TimeSpan                                         DelayTime { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = TimeSpan.FromSeconds( 5 );
    public       Encoding                                         Encoding  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = Encoding.Default;
    public       Func<FileLoggerProvider.LogEvent, string>        Formatter { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = log => log.ToString();
    public       LocalFile                                        Path      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = FILE_NAME;
    public       FileLoggerRolloverOptions?                       Rollover  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    FileLoggerProviderOptions IOptions<FileLoggerProviderOptions>.Value     => this;
}



[Experimental( nameof(FileLoggerProvider) )] public readonly record struct FileLoggerRolloverOptions( LocalDirectory Directory, TimeSpan? LifeSpan, int MaxFiles = 10, long MaxSize = 10_485_760 );



[Experimental( nameof(FileLoggerProvider) )]
public sealed partial class FileLoggerProvider( IOptions<LoggerFilterOptions> filter, IOptions<FileLoggerProviderOptions> options ) : ILoggerProvider, IHostedService
{
    private readonly ConcurrentQueue<LogEvent> _queue   = [];
    private readonly FileLoggerProviderOptions _options = options.Value;
    private readonly Locker                    _locker  = Locker.Default;
    private readonly LoggerFilterOptions       _filter  = filter.Value;
    private          Stream?                   _stream;


    private FileLoggerRolloverOptions? _Rollover { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _options.Rollover; }
    public  LogLevel                   MinLevel  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _filter.MinLevel; }


    [GeneratedRegex( @"^(\d{4})-(0[1-9]|1[0-2])-(0[1-9]|[12]\d|3[01])T([01]\d|2[0-3]):([0-5]\d):([0-5]\d)(\.\d{1,7})?([+-]([01]\d|2[0-3]):([0-5]\d)|Z).log$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline, 200 )]
    public static partial Regex GetDateTimeOffsetRegex();


    [GeneratedRegex( @"\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01]).log$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline, 200 )]
    public static partial Regex GetDateRegex();


    public ILogger CreateLogger( string categoryName ) => new Logger( this, categoryName );

    public void Dispose()
    {
        _queue.Clear();
        _stream?.Dispose();
        _stream = null;
    }


    public async Task StartAsync( CancellationToken token )
    {
        if ( _Rollover.HasValue ) { await Rollover( _options.Encoding, _options.DelayTime, _Rollover.Value, token ); }
        else { await SingleFile( _options.Encoding, _options.DelayTime, token ); }
    }
    private async Task Rollover( Encoding encoding, TimeSpan delay, FileLoggerRolloverOptions options, CancellationToken token )
    {
        using PeriodicTimer timer = new(delay);
        DateTimeOffset      now   = DateTimeOffset.UtcNow;

        using ( await _locker.EnterAsync( token ) )
        {
            // LocalDirectory.Watcher watcher   = new(directory);
            LocalDirectory  directory = options.Directory;
            List<LocalFile> files     = [..directory.GetFiles()];

            if ( files.Count >= options.MaxFiles )
            {
                files.Sort( static ( a, b ) => a.LastAccess.CompareTo( b.LastAccess ) );
                int obsolete = files.Count - options.MaxFiles;
                for ( int i = 0; i < obsolete; i++ ) { files[i].Delete(); }

                files.RemoveRange( 0, obsolete );
            }

            await using Stream       stream = _options.Path.OpenWrite( FileMode.Append );
            await using StreamWriter writer = new(stream, encoding, 4096, true);

            while ( token.ShouldContinue() )
            {
                await timer.WaitForNextTickAsync( token ).ConfigureAwait( false );

                while ( _queue.TryDequeue( out LogEvent log ) )
                {
                    string message = log.ToString();
                    await writer.WriteLineAsync( message ).ConfigureAwait( false );
                }
            }
        }
    }
    private async Task SingleFile( Encoding encoding, TimeSpan delay, CancellationToken token )
    {
        using PeriodicTimer timer = new(delay);

        using ( await _locker.EnterAsync( token ) )
        {
            await using Stream       stream = _options.Path.OpenWrite( FileMode.Append );
            await using StreamWriter writer = new(stream, encoding, 4096, true);

            while ( token.ShouldContinue() )
            {
                await timer.WaitForNextTickAsync( token ).ConfigureAwait( false );

                while ( _queue.TryDequeue( out LogEvent log ) )
                {
                    string message = log.ToString();
                    await writer.WriteLineAsync( message ).ConfigureAwait( false );
                }
            }
        }
    }
    public async Task StopAsync( CancellationToken token )
    {
        using ( await _locker.EnterAsync( token ) )
        {
            await using Stream       stream = _options.Path.OpenWrite( FileMode.Append );
            await using StreamWriter writer = new(stream, _options.Encoding, 4096, true);

            while ( _queue.TryDequeue( out LogEvent log ) )
            {
                string message = log.ToString();
                await writer.WriteLineAsync( message ).ConfigureAwait( false );
            }
        }
    }



    public readonly record struct LogEvent( DateTimeOffset TimeStamp, LogLevel Level, EventId EventId, string Message, string Category )
    {
        public static   LogEvent Create( LogLevel level, EventId eventId, string message, string categoryName ) => new(DateTimeOffset.UtcNow, level, eventId, message, categoryName);
        public override string   ToString() => $"[ {TimeStamp} - {nameof(Category)}: {Category} - {nameof(Level)}: {Level} - {nameof(EventId)}: {EventId} ]: {Message}";
    }



    public sealed class Logger( FileLoggerProvider provider, string categoryName ) : ILogger
    {
        private readonly FileLoggerProvider _provider     = provider;
        private readonly string             _categoryName = categoryName;


        public IDisposable BeginScope<TState>( TState state )
            where TState : notnull => NullScope.Instance;

        public bool IsEnabled( LogLevel logLevel ) => logLevel > _provider.MinLevel;
        public void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter )
        {
            if ( IsEnabled( logLevel ) is false ) { return; }

            string message = formatter( state, exception );
            _provider._queue.Enqueue( LogEvent.Create( logLevel, eventId, message, _categoryName ) );
        }
    }



    /// <summary> An empty scope without any logic </summary>
    public sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();
        private NullScope() { }
        public void Dispose() { }
    }
}



#endif
