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
public sealed class FileLoggerProvider( IOptions<LoggerFilterOptions> filter, IOptions<FileLoggerProviderOptions> options ) : ILoggerProvider, IHostedService
{
    public const     int                       BUFFER_SIZE    = 4096;
    public const     string                    FILE_EXTENSION = ".logs";
    private readonly ConcurrentQueue<LogEvent> _queue         = [];
    private readonly FileLoggerProviderOptions _options       = options.Value;
    private readonly Locker                    _locker        = Locker.Default;
    private readonly LoggerFilterOptions       _filter        = filter.Value;
    private          Stream?                   _stream;


    private FileLoggerRolloverOptions? _Rollover { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _options.Rollover; }
    public  LogLevel                   MinLevel  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _filter.MinLevel; }


    public ILogger CreateLogger( string categoryName ) => new Logger( this, categoryName );

    public void Dispose()
    {
        _queue.Clear();
        _stream?.Dispose();
        _stream = null;
    }

    private static SortedDictionary<DateTimeOffset, LocalFile> GetFiles( scoped in FileLoggerRolloverOptions options )
    {
        SortedDictionary<DateTimeOffset, LocalFile> files = new(ValueSorter<DateTimeOffset>.Default);
        foreach ( LocalFile file in options.Directory.GetFiles() ) { files.Add( file.CreationTimeUtc, file ); }

        if ( files.Count < options.MaxFiles ) { return files; }

        foreach ( DateTimeOffset date in files.Keys.Take( files.Count - options.MaxFiles ) ) { files.Remove( date ); }

        return files;
    }
    public static bool TryParse( scoped ReadOnlySpan<char> name, out DateTimeOffset date )
    {
        if ( name.EndsWith( FILE_EXTENSION, StringComparison.OrdinalIgnoreCase ) is false ) { name = name[..^FILE_EXTENSION.Length]; }

        return DateTimeOffset.TryParse( name, out date );
    }
    public async Task StartAsync( CancellationToken token )
    {
        if ( _Rollover.HasValue ) { await Rollover( _options.Encoding, _options.DelayTime, _Rollover.Value, token ); }
        else { await SingleFile( _options.Encoding, _options.DelayTime, token ); }
    }
    private async Task Rollover( Encoding encoding, TimeSpan delay, FileLoggerRolloverOptions options, CancellationToken token )
    {
        using PeriodicTimer timer = new(delay);

        using ( await _locker.EnterAsync( token ) )
        {
            // LocalDirectory.Watcher watcher   = new(directory);
            LocalDirectory                              directory = options.Directory;
            SortedDictionary<DateTimeOffset, LocalFile> files     = GetFiles( options );

            LocalFile current = new($"{DateTimeOffset.UtcNow}{FILE_EXTENSION}");
            files.Add( current.CreationTimeUtc, current );

            await using Stream       stream = current.OpenWrite( FileMode.Append );
            await using StreamWriter writer = new(stream, encoding, BUFFER_SIZE, true);
            CancellationTokenSource  source = CancellationTokenSource.CreateLinkedTokenSource( token );

            while ( token.ShouldContinue() )
            {
                await timer.WaitForNextTickAsync( token ).ConfigureAwait( false );

                // ReSharper disable once MethodHasAsyncOverload
                if ( current.Info.Length > options.MaxSize ) { source.Cancel(); }

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
