namespace Jakar.Extensions;



/*
[SuppressMessage( "ReSharper", "RedundantArgumentDefaultValue" )]
public sealed class FileLoggerProvider( IOptions<FileLoggerProviderOptions> options ) : ILoggerProvider, IHostedService
{
    public const     int                       BUFFER_SIZE    = 4096;
    public const     string                    FILE_EXTENSION = ".log";
    private readonly ConcurrentStack<LogEvent> _queue         = [];
    private readonly FileLoggerProviderOptions _options       = options.Value;
    private          Stream?                   _stream;


    public LogLevel MinLevel { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = LogLevel.Debug;


    public ILogger CreateLogger( string categoryName ) => new Logger( this, categoryName );

    public void Dispose()
    {
        _queue.Clear();
        _stream?.Dispose();
        _stream = null;
    }


    public async Task StartAsync( CancellationToken token )
    {
        while ( token.ShouldContinue() )
        {
            if ( _queue.IsEmpty is false )
            {
                if ( _options.Mode.IsT0 ) { await Start( _options.Mode.AsT0 ).ConfigureAwait( false ); }
                else
                {
                    FileLoggerRolloverOptions options = _options.Mode.AsT1;
                    LocalFile                 current = options.Directory.Join( GetFileName( _options.AppName ), _options.Encoding );
                    await Start( current, options ).ConfigureAwait( false );
                }
            }

            await Task.Delay( 100, token ).ConfigureAwait( false );
        }
    }
    public Task StopAsync( CancellationToken token ) => Start( _options.File );


    private async Task Start( LocalFile current, FileLoggerRolloverOptions? rolloverOptions = null )
    {
        if ( rolloverOptions.HasValue ) { UpdateFiles( rolloverOptions.Value ); }

        await using Stream       stream = current.OpenWrite( FileMode.Append, BUFFER_SIZE );
        await using StreamWriter writer = new(stream, _options.Encoding, BUFFER_SIZE, true);

        if ( rolloverOptions.HasValue )
        {
            while ( _queue.TryPeek( out LogEvent? log ) )
            {
                string message = _options.Formatter( log );
                if ( stream.Length + message.Length < rolloverOptions.Value.MaxSize ) { return; }

                _queue.TryPop( out _ );
                await writer.WriteLineAsync( message ).ConfigureAwait( false );
            }
        }
        else
        {
            while ( _queue.TryPop( out LogEvent? log ) )
            {
                string message = _options.Formatter( log );
                await writer.WriteLineAsync( message ).ConfigureAwait( false );
            }
        }
    }


    private static string GetFileName( scoped in ReadOnlySpan<char> appName ) => appName.IsNullOrWhiteSpace()
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



    public sealed record LogEvent( DateTimeOffset TimeStamp, LogLevel Level, EventId EventId, string Message, string Category )
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
}
*/
