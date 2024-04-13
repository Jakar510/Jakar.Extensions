using Microsoft.Extensions.Hosting;



namespace Jakar.Extensions.Loggers;


#if NET8_0_OR_GREATER



[Experimental( nameof(FileLoggerProvider) )]
public sealed class FileLoggerProviderOptions : IOptions<FileLoggerProviderOptions>
{
    public const string                                           FILE_NAME = "App.logs";
    public       string?                                          AppName   { get;                                                      set; }
    public       TimeSpan                                         DelayTime { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = TimeSpan.FromSeconds( 5 );
    public       Encoding                                         Encoding  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = Encoding.Default;
    public       Func<FileLoggerProvider.LogEvent, string>        Formatter { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = log => log.ToString();
    public       OneOf<LocalFile, FileLoggerRolloverOptions>      Mode      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = new LocalFile( FILE_NAME );
    FileLoggerProviderOptions IOptions<FileLoggerProviderOptions>.Value     => this;
}



[Experimental( nameof(FileLoggerProvider) )] public readonly record struct FileLoggerRolloverOptions( LocalDirectory Directory, TimeSpan? LifeSpan, int MaxFiles = 10, long MaxSize = 10_485_760 );



[Experimental( nameof(FileLoggerProvider) )]
public sealed class FileLoggerProvider( IOptions<LoggerFilterOptions> filter, IOptions<FileLoggerProviderOptions> options, ILogger<FileLoggerProvider> logger ) : ILoggerProvider, IHostedService
{
    public const     string                      FILE_EXTENSION = ".log";
    private readonly ILogger<FileLoggerProvider> _logger        = logger;
    public const     int                         BUFFER_SIZE    = 4096;
    private readonly ConcurrentStack<LogEvent>   _queue         = [];
    private readonly FileLoggerProviderOptions   _options       = options.Value;
    private readonly Locker                      _locker        = Locker.Default;
    private readonly LoggerFilterOptions         _filter        = filter.Value;
    private          Stream?                     _stream;

    public LogLevel MinLevel { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _filter.MinLevel; }


    public ILogger CreateLogger( string categoryName ) => new Logger( this, categoryName );

    public void Dispose()
    {
        _queue.Clear();
        _stream?.Dispose();
        _stream = null;
    }


    public async Task StartAsync( CancellationToken token )
    {
        using PeriodicTimer timer = new(_options.DelayTime);

        using ( await _locker.EnterAsync( token ) )
        {
            while ( token.ShouldContinue() )
            {
                await timer.WaitForNextTickAsync( token ).ConfigureAwait( false );
                await StartAsync( _options.Mode ).ConfigureAwait( false );
            }
        }
    }


    private Task StartAsync( scoped in OneOf<LocalFile, FileLoggerRolloverOptions> mode ) => mode.IsT1
                                                                                                 ? StartRollover( _options.Encoding, mode.AsT1 )
                                                                                                 : StartSingle( new LocalFile( mode.AsT0.FullPath, _options.Encoding ) );
    private async Task StartRollover( Encoding encoding, FileLoggerRolloverOptions options )
    {
        LocalDirectory directory = options.Directory;

        try
        {
            UpdateFiles( options );
            LocalFile                current = directory.Join( GetFileName( _options.AppName ), encoding );
            await using Stream       stream  = current.OpenWrite( FileMode.Append );
            await using StreamWriter writer  = new(stream, encoding, BUFFER_SIZE, true);

            while ( _queue.TryPeek( out LogEvent log ) )
            {
                string message = log.ToString();
                if ( stream.Length + message.Length < options.MaxSize ) { return; }

                _queue.TryPop( out _ );
                await writer.WriteLineAsync( message ).ConfigureAwait( false );
            }
        }
        catch ( TaskCanceledException ) { }
        catch ( Exception e ) { _logger.LogCritical( e, "{Caller}", nameof(FileLoggerProvider) ); }
    }
    private async Task StartSingle( LocalFile current )
    {
        await using Stream       stream = current.OpenWrite( FileMode.Append );
        await using StreamWriter writer = new(stream, current.FileEncoding, 4096, true);

        while ( _queue.TryPop( out LogEvent log ) )
        {
            string message = log.ToString();
            await writer.WriteLineAsync( message ).ConfigureAwait( false );
        }
    }
    public async Task StopAsync( CancellationToken token )
    {
        using ( await _locker.EnterAsync( token ) ) { await StartAsync( _options.Mode ).ConfigureAwait( false ); }
    }


    static string GetFileName( scoped in ReadOnlySpan<char> appName ) => appName.IsNullOrWhiteSpace()
                                                                             ? $"{DateTimeOffset.UtcNow}{FILE_EXTENSION}"
                                                                             : $"{appName}_{DateTimeOffset.UtcNow}{FILE_EXTENSION}";


    public static bool TryParse( scoped ReadOnlySpan<char> name, out DateTimeOffset date )
    {
        if ( name.EndsWith( FILE_EXTENSION, StringComparison.OrdinalIgnoreCase ) is false ) { name = name[..^FILE_EXTENSION.Length]; }

        return DateTimeOffset.TryParse( name, out date );
    }

    public static void UpdateFiles( scoped in FileLoggerRolloverOptions options )
    {
        SortedDictionary<DateTimeOffset, LocalFile> files = new(ValueSorter<DateTimeOffset>.Default);
        foreach ( LocalFile file in options.Directory.GetFiles() ) { files.Add( file.CreationTimeUtc, file ); }

        if ( files.Count < options.MaxFiles ) { return; }

        foreach ( DateTimeOffset date in files.Keys.Take( files.Count - options.MaxFiles ) ) { files.Remove( date ); }
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
            _provider._queue.Push( LogEvent.Create( logLevel, eventId, message, _categoryName ) );
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
