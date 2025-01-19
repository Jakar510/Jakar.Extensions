using System.IO;
using System.IO.Compression;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Serilog.Configuration;
using Serilog.Context;
using Serilog.Debugging;
using Serilog.Enrichers.Span;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Async;
using Serilog.Sinks.File;
using Serilog.Sinks.SystemConsole.Themes;
using ILogger = Microsoft.Extensions.Logging.ILogger;



namespace Jakar.Extensions.Serilog;


[SuppressMessage( "ReSharper", "AsyncVoidLambda" ), SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" ), SuppressMessage( "ReSharper", "StaticMemberInGenericType" ), SuppressMessage( "ReSharper", "CollectionNeverQueried.Local" )]
public sealed class Serilogger : ISerilogger, IAsyncDisposable
{
    public const            string                CONSOLE_DEFAULT_OUTPUT_TEMPLATE     = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const            string                DEBUG_DEFAULT_DEBUG_OUTPUT_TEMPLATE = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const            long                  DEFAULT_FILE_SIZE_LIMIT_BYTES       = 1L * 1024 * 1024 * 1024; // 1GB
    public const            int                   DEFAULT_RETAINED_FILE_COUNT_LIMIT   = 31;                      // A long month of logs
    public const            string                SEQ_API_KEY                         = "EhpGe4rqAbSEph5OpA4j";
    public const            string                SEQ_API_KEY_NAME                    = "X-Seq-ApiKey";
    public const            string                SEQ_API_URL                         = "http://192.168.2.12:5341";
    public const            string                SEQ_API_URL_FULL                    = $"{SEQ_API_URL}/ingest/otlp";
    public const            string                SEQ_BUFFER_DIRECTORY                = "SeqBuffer";
    public const            string                SHARED_NAME                         = "Serilogger";
    private static          Guid?                 _deviceID;
    private static          long?                 _deviceIDLong;
    private static readonly object[]              _noPropertyValues = [];
    public static readonly  Guid                  DebugID           = Guid.Parse( "5C2064EF-F418-48AB-9C3D-536DADCE6E88" );
    public static readonly  FileData[]            Empty             = [];
    private readonly        SerilogLoggerProvider _provider;
    private                 bool                  _canDebug = true;
    private                 IDebugSettings        _settings;
    private                 LocalFile?            _screenShotAddress;


    public static bool ApplyThemeToRedirectedOutput { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public static Guid DeviceID
    {
        get
        {
            if ( Instance?.CanDebug is true ) { return DebugID; }

            if ( _deviceID.HasValue ) { return _deviceID.Value; }

            _deviceID = SHARED_NAME.GetPreference( nameof(DeviceID), Guid.NewGuid() );
            Activity.Current?.SetTag( nameof(DeviceID), _deviceID.ToString() );
            return _deviceID.Value;
        }
        set
        {
            _deviceID = value;
            SHARED_NAME.SetPreference( nameof(DeviceID), value );
            Activity.Current?.SetTag( nameof(DeviceID), value.ToString() );
        }
    }
    public static long DeviceIDLong
    {
        get
        {
            if ( _deviceIDLong.HasValue ) { return _deviceIDLong.Value; }

            _deviceIDLong = SHARED_NAME.GetPreference( nameof(DeviceIDLong), 0L );
            Activity.Current?.SetTag( nameof(DeviceIDLong), _deviceIDLong.ToString() );
            return _deviceIDLong.Value;
        }
        set
        {
            _deviceIDLong = value;
            SHARED_NAME.SetPreference( nameof(DeviceIDLong), value );
            Activity.Current?.SetTag( nameof(DeviceIDLong), value.ToString() );
        }
    }
    public static   string                     DeviceName   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = string.Empty;
    public static   FileLifecycleHooks?        Hooks        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public static   Serilogger?                Instance     { get;                                                      private set; }
    public static   LoggingLevelSwitch         LoggingLevel { get; } = new(LogEventLevel.Verbose);
    public static   IAsyncLogEventSinkMonitor? Monitor      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public static   object?                    SyncRoot     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public static   ConsoleTheme?              Theme        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public          Activity                   Activity     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public required Guid                       AppID        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; } = Guid.Empty;
    public required string                     AppName      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; } = string.Empty;
    public required AppVersion                 AppVersion   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public bool CanDebug
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _canDebug;
        set
        {
            _canDebug = value;

            LoggingLevel.MinimumLevel = value
                                            ? LogEventLevel.Verbose
                                            : LogEventLevel.Information;
        }
    }
    public bool                     CannotDebug       { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = Debugger.IsAttached is false;
    Guid IDeviceID.                 DeviceID          => DeviceID;
    string IDeviceName.             DeviceName        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => DeviceName; }
    public bool                     Disabled          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Enabled is false; }
    public bool                     Enabled           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Settings.EnableApi; }
    public bool                     EnableDebugEvents { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public DebugLogEvent.Collection Events            { get; } = [];
    public bool                     IsValid           => DeviceID.IsValidID() || DeviceIDLong != 0;
    public Logger                   Logger            { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public MessageEvent.Collection  Messages          { get; } = [];


    public required FilePaths Paths { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public LocalFile? ScreenShotAddress
    {
        get => _screenShotAddress ??= Paths.Cache.Join( IFilePaths.SCREEN_SHOT_FILE );
        set
        {
            _screenShotAddress?.Dispose();
            _screenShotAddress = value?.SetTemporary();
        }
    }
    public          ReadOnlyMemory<byte>                                     ScreenShotData     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get;              internal set; }
    public          IDebugSettings                                           Settings           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _settings; init => _settings = value; }
    public required Func<CancellationToken, ValueTask<ReadOnlyMemory<byte>>> TakeScreenShot     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get;              init; }
    public required Func<EventDetails, EventDetails>                         UpdateEventDetails { [MethodImpl( MethodImplOptions.AggressiveInlining )] get;              init; }
    static Serilogger() => SelfLog.Enable( static message =>
                                           {
                                               // System.Diagnostics.Debug.WriteLine( message );
                                               // Console.WriteLine( message );
                                               Console.Error.WriteLine( message );
                                           } );
    private static ValueTask<ReadOnlyMemory<byte>> TakeEmptyScreenShot( CancellationToken  arg )     => new(ReadOnlyMemory<byte>.Empty);
    private static EventDetails                    DefaultUpdateEventDetails( EventDetails details ) => details;
    public static Serilogger Create<TApp, TDebugSettings>( LocalDirectory? currentDirectory, Action<LoggerConfiguration>? addNativeLogs = null )
        where TDebugSettings : class, IDebugSettings<TDebugSettings>, IDebugSettings
        where TApp : IAppID => Create<TApp, TDebugSettings>( currentDirectory, DefaultUpdateEventDetails, addNativeLogs );
    public static Serilogger Create<TApp, TDebugSettings>( LocalDirectory? currentDirectory, Func<EventDetails, EventDetails>? updateEventDetails = null, Action<LoggerConfiguration>? addNativeLogs = null )
        where TDebugSettings : class, IDebugSettings<TDebugSettings>, IDebugSettings
        where TApp : IAppID => Create<TApp, TDebugSettings>( currentDirectory, TakeEmptyScreenShot, updateEventDetails, addNativeLogs );
    public static Serilogger Create<TApp, TDebugSettings>( LocalDirectory? currentDirectory, Func<CancellationToken, ValueTask<ReadOnlyMemory<byte>>>? takeScreenShot = null, Func<EventDetails, EventDetails>? updateEventDetails = null, Action<LoggerConfiguration>? addNativeLogs = null )
        where TDebugSettings : class, IDebugSettings<TDebugSettings>, IDebugSettings
        where TApp : IAppID
    {
        currentDirectory ??= Environment.CurrentDirectory;
        FilePaths paths = new(currentDirectory);
        return Create<TApp, TDebugSettings>( paths, takeScreenShot, updateEventDetails, addNativeLogs );
    }
    public static Serilogger Create<TApp, TDebugSettings>( FilePaths paths, Func<CancellationToken, ValueTask<ReadOnlyMemory<byte>>>? takeScreenShot = null, Func<EventDetails, EventDetails>? updateEventDetails = null, Action<LoggerConfiguration>? addNativeLogs = null )
        where TDebugSettings : class, IDebugSettings<TDebugSettings>, IDebugSettings
        where TApp : IAppID
    {
        LoggerConfiguration builder = new();

        builder.MinimumLevel.Verbose();
        builder.MinimumLevel.Override( nameof(Microsoft), LogEventLevel.Warning );
        builder.MinimumLevel.Override( nameof(System),    LogEventLevel.Warning );

        builder.Enrich.WithProperty( Constants.SourceContextPropertyName, TApp.AppName );
        builder.Enrich.WithProperty( nameof(TApp.AppID),                  TApp.AppID.ToString() );
        builder.Enrich.WithProperty( nameof(TApp.AppVersion),             TApp.AppVersion.ToString() );
        builder.Enrich.FromLogContext();
        builder.Enrich.With<AppContextEnricher>();
        builder.Enrich.With<FilePathsEnricher>();

        builder.Enrich.WithSpan( new SpanOptions
                                 {
                                     IncludeBaggage       = true,
                                     IncludeOperationName = true,
                                     IncludeTags          = true,
                                     IncludeTraceFlags    = true
                                 } );

        builder.WriteTo.Async( ConfigureFileSink, 10000, true, Monitor );

        // ConfigureConsoleSink( builder.WriteTo );
        ConfigureDebugSink( builder.WriteTo );

    #if DEBUG
        builder.WriteTo.Seq( "http://192.168.2.12:5341",
                             LogEventLevel.Verbose,
                             1000,
                             TimeSpan.FromSeconds( 2 ),
                             "EhpGe4rqAbSEph5OpA4j",
                             null, // seqBuffer,
                             null,
                             1024 * 1024,
                             null,
                             null,
                             null,
                             100_000,
                             null, // new CompactJsonFormatter(),
                             CultureInfo.InvariantCulture );
    #endif

        /*
        #if DEBUG
        #pragma warning disable RemoteLogging
            builder.WriteTo.Sink( RemoteLogging.Current );
        #pragma warning restore RemoteLogging
        #endif
        */

        addNativeLogs?.Invoke( builder );

        Activity activity = new(TApp.AppName) { DisplayName = TApp.AppName };
        activity.SetStartTime( DateTime.UtcNow );
        activity.SetIdFormat( ActivityIdFormat.Hierarchical );
        activity.SetStatus( ActivityStatusCode.Ok );
        activity.SetTag( nameof(AppName),    TApp.AppName );
        activity.SetTag( nameof(AppID),      TApp.AppID.ToString() );
        activity.SetTag( nameof(AppVersion), TApp.AppVersion.ToString() );
        activity.SetTag( nameof(DeviceID),   DeviceID.ToString() );
        activity.SetTag( nameof(DebugID),    DebugID.ToString() );
        activity.Start();


        Serilogger logger = new(activity, builder.CreateLogger(), TDebugSettings.FromPreferences())
                            {
                                TakeScreenShot     = takeScreenShot,
                                UpdateEventDetails = updateEventDetails,
                                AppName            = TApp.AppName,
                                AppVersion         = TApp.AppVersion,
                                AppID              = TApp.AppID,
                                Paths              = paths,
                                CanDebug           = Debugger.IsAttached
                            };

        return logger.ClearCache();

        void ConfigureFileSink( LoggerSinkConfiguration sink ) => sink.File( new CompactJsonFormatter(),
                                                                             paths.LogsFile.FullPath,
                                                                             LogEventLevel.Verbose,
                                                                             DEFAULT_FILE_SIZE_LIMIT_BYTES,
                                                                             null,
                                                                             false,
                                                                             false,
                                                                             null,
                                                                             RollingInterval.Day,
                                                                             true,
                                                                             DEFAULT_RETAINED_FILE_COUNT_LIMIT,
                                                                             Encoding.Default,
                                                                             Hooks,
                                                                             TimeSpan.FromDays( 90 ) );
    }
    private Serilogger( Activity activity, Logger logger, IDebugSettings settings )
    {
        Activity.Current = Activity = activity;
        _settings        = settings;
        Log.Logger       = Logger = logger;
        _provider        = new SerilogLoggerProvider( this, true );
        System.Diagnostics.Debug.Assert( IsValid, $"{SHARED_NAME} is invalid" );
        Instance = (Serilogger)this;
    }
    public void Dispose()
    {
        ScreenShotData = ReadOnlyMemory<byte>.Empty;
        Events.Dispose();
        _provider.Dispose();
        _screenShotAddress?.Dispose();
        Paths.Dispose();
        Activity.Dispose();
        Logger.Dispose();
    }
    public async ValueTask DisposeAsync()
    {
        ScreenShotData = ReadOnlyMemory<byte>.Empty;
        await CastAndDispose( Events );
        await _provider.DisposeAsync();
        if ( _screenShotAddress is not null ) { await CastAndDispose( _screenShotAddress ); }

        Paths.Dispose();
        Activity.Dispose();
        await Logger.DisposeAsync();

        return;

        static async ValueTask CastAndDispose( IDisposable resource )
        {
            if ( resource is IAsyncDisposable resourceAsyncDisposable ) { await resourceAsyncDisposable.DisposeAsync(); }
            else { resource.Dispose(); }
        }
    }


    public static void ConfigureConsoleSink( LoggerSinkConfiguration sink ) => sink.Console( LogEventLevel.Information, CONSOLE_DEFAULT_OUTPUT_TEMPLATE, CultureInfo.InvariantCulture, null, LogEventLevel.Error, Theme, ApplyThemeToRedirectedOutput, SyncRoot );
    public static void ConfigureDebugSink( LoggerSinkConfiguration   sink ) => sink.Debug( LogEventLevel.Verbose, DEBUG_DEFAULT_DEBUG_OUTPUT_TEMPLATE, CultureInfo.InvariantCulture );
    public Serilogger ClearCache()
    {
        foreach ( LocalFile file in Paths.Cache.GetFiles() ) { file.Delete(); }

        return (Serilogger)this;
    }


    public static Serilogger      Get( IServiceProvider         provider )                                                         => Instance ?? provider.GetRequiredService<Serilogger>();
    public static ILoggerProvider GetProvider( IServiceProvider provider )                                                         => Get( provider );
    void ILoggerFactory.          AddProvider( ILoggerProvider  provider )                                                         { }
    public ILogger                CreateLogger( string          categoryName )                                                     => _provider.CreateLogger( categoryName );
    public Logger<T>              CreateLogger<T>()                                                                                => new(this);
    public void                   SetScopeProvider( IExternalScopeProvider scopeProvider )                                         => _provider.SetScopeProvider( scopeProvider );
    void ILogEventSink.           Emit( LogEvent                           logEvent )                                              => ((ILogEventSink)Logger).Emit( logEvent );
    public void                   HandleException<T>( T                    _, Exception exception )                                => HandleException( _, exception, Empty );
    public void                   HandleException<T>( T                    _, Exception exception, params FileData[] attachments ) => Task.Run( async () => await HandleExceptionAsync( _, exception, attachments ) );
    public ValueTask              HandleExceptionAsync<T>( T               _, Exception exception ) => HandleExceptionAsync( _, exception, Empty );
    public async ValueTask HandleExceptionAsync<T>( T _, Exception exception, params FileData[] attachments )
    {
        System.Diagnostics.Debug.WriteLine( exception.ToString() );
        if ( Disabled ) { return; }

        if ( exception is OperationCanceledException or CredentialsException ) { return; }

        Func<CancellationToken, ValueTask<ReadOnlyMemory<byte>>> takeScreenShot = TakeScreenShot;

        ScreenShotData = Settings.TakeScreenshotOnError
                             ? await takeScreenShot( CancellationToken.None )
                             : ReadOnlyMemory<byte>.Empty;

        TrackError( _, exception, AppState(), attachments );
    }


    public void TrackEvent<T>( T _, [CallerMemberName] string caller = BaseRecord.EMPTY )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace( caller );
        if ( IsEnabled( LogEventLevel.Verbose ) is false ) { return; }

        Debug( "[{ClassName}.{Caller}.{EventId}]", typeof(T).Name, caller, caller.GetHashCode() );
        if ( EnableDebugEvents ) { Events.Add( DebugLogEvent.Create<T>( caller, LogEventLevel.Debug, null, caller ) ); }
    }
    public void TrackEvent<T>( T _, EventDetails properties, [CallerMemberName] string caller = BaseRecord.EMPTY )
    {
        if ( Settings.EnableApi is false || Settings.EnableCrashes is false ) { return; }

        ArgumentException.ThrowIfNullOrWhiteSpace( caller );
        if ( IsEnabled( LogEventLevel.Verbose ) is false ) { return; }

        Debug( "[{ClassName}.{Caller}.{EventId}]", typeof(T).Name, caller, caller.GetHashCode() );
        if ( EnableDebugEvents ) { Events.Add( DebugLogEvent.Create<T>( caller, LogEventLevel.Debug, properties, caller ) ); }
    }
    public void TrackEvent<T>( T _, string eventType, EventDetails? properties, [CallerMemberName] string caller = BaseRecord.EMPTY )
    {
        if ( Settings.EnableApi is false || Settings.EnableCrashes is false ) { return; }

        ArgumentException.ThrowIfNullOrWhiteSpace( caller );
        ArgumentException.ThrowIfNullOrWhiteSpace( eventType );
        if ( IsEnabled( LogEventLevel.Verbose ) is false ) { return; }

        using ( Disposables disposables = new() )
        {
            if ( properties is not null )
            {
                foreach ( (string key, string? value) in properties ) { disposables.Add( LogContext.PushProperty( key, value ) ); }
            }

            Debug( "[{ClassName}.{Caller}.{EventId}] {EventType}", typeof(T).Name, caller, eventType.GetHashCode(), eventType );
        }

        if ( EnableDebugEvents ) { Events.Add( DebugLogEvent.Create( typeof(T).Name, eventType, LogEventLevel.Debug, properties, caller ) ); }
    }
    public void TrackError<T>( T _, Exception exception, EventDetails details, IEnumerable<FileData>? attachments, [CallerMemberName] string caller = BaseRecord.EMPTY )
    {
        if ( Settings.EnableApi is false || Settings.EnableCrashes is false ) { return; }

        using Disposables disposables = new();
        disposables.Add( LogContext.PushProperty( nameof(exception),                 exception ) );
        disposables.Add( LogContext.PushProperty( $"{exception.GetType().Name}.txt", $"\n\n{exception}\n\n" ) );
        disposables.Add( LogContext.PushProperty( nameof(EventDetails),              details.ToPrettyJson() ) );

        if ( attachments is not null )
        {
            foreach ( IDisposable disposable in attachments.Select( AppLoggers.AddFileToLogContext ) ) { disposables.Add( disposable ); }
        }

        TrackError( _, exception, caller );
    }
    public void TrackError<T>( T _, Exception exception, [CallerMemberName] string caller = BaseRecord.EMPTY )
    {
        if ( Settings.EnableApi is false || Settings.EnableCrashes is false ) { return; }

        ArgumentException.ThrowIfNullOrWhiteSpace( caller );
        if ( IsEnabled( LogEventLevel.Error ) is false ) { return; }

        Error( exception, "[{ClassName}.{Caller}.{EventId}] {Message}", typeof(T).Name, caller, exception.Message.GetHashCode(), exception.Message );
        if ( EnableDebugEvents ) { Messages.Enqueue( MessageEvent.Create( exception ) ); }
    }
    public async ValueTask SaveFeedBackAppState( Dictionary<string, string> feedback, string key = "feedback" )
    {
        Dictionary<string, object?> result = new() { [key] = feedback };
        if ( Settings.IncludeAppStateOnError ) { EventDetails.AddAppState( in result, AppName ); }

        await Paths.FeedbackFile.WriteAsync( result.ToPrettyJson() );
    }
    public EventDetails AppState()
    {
        Func<EventDetails, EventDetails> updateEventDetails = UpdateEventDetails;
        return updateEventDetails( EventDetails.AppState( AppName ) );
    }
    public async ValueTask<bool> BufferScreenShot( CancellationToken token = default )
    {
        Func<CancellationToken, ValueTask<ReadOnlyMemory<byte>>> takeScreenShot = TakeScreenShot;
        ReadOnlyMemory<byte>                                     data           = ScreenShotData = await takeScreenShot( token );
        return data.IsEmpty is false;
    }
    public async ValueTask<LocalFile?> GetScreenShot( CancellationToken token = default )
    {
        Func<CancellationToken, ValueTask<ReadOnlyMemory<byte>>> takeScreenShot = TakeScreenShot;
        ReadOnlyMemory<byte>                                     screenShot     = await takeScreenShot( token );
        return await WriteScreenShot( screenShot, token );
    }
    public async ValueTask<LocalFile?> WriteScreenShot( CancellationToken token = default ) => await WriteScreenShot( ScreenShotData, token );
    public async ValueTask<LocalFile?> WriteScreenShot( ReadOnlyMemory<byte> memory, CancellationToken token = default )
    {
        if ( memory.IsEmpty ) { return null; }

        LocalFile file = Paths.ScreenShot;
        await file.WriteAsync( memory, token );
        return file;
    }
    public static void SetDeviceID( Guid deviceID ) => DeviceID = deviceID;
    public static void SetDeviceID( long deviceID ) => DeviceIDLong = deviceID;
    public void SetSettings<TDebugSettings>( TDebugSettings settings )
        where TDebugSettings : class, IDebugSettings<TDebugSettings>, IDebugSettings
    {
        _settings = settings.Clone();
        _settings.SetPreferences();
    }
    public static async Task<LocalFile?> ZipLogsToFile()
    {
        LocalFile? file = Instance?.Paths.ZipLogsFile.SetTemporary();
        if ( file is null ) { return null; }

        ReadOnlyMemory<byte> data = await ZipLogsAsync();
        await file.WriteAsync( data );
        return file;
    }
    public static async Task<LocalFile?> ZipCacheToFile()
    {
        LocalFile? file = Instance?.Paths.AppCacheZipFile.SetTemporary();
        if ( file is null ) { return null; }

        ReadOnlyMemory<byte> data = await ZipCacheAsync();
        await file.WriteAsync( data );
        return file;
    }
    public static async Task<LocalFile?> ZipDataToFile()
    {
        LocalFile? file = Instance?.Paths.AppDataZipFile.SetTemporary();
        if ( file is null ) { return null; }

        ReadOnlyMemory<byte> data = await ZipCacheAsync();
        await file.WriteAsync( data );
        return file;
    }
    public static ReadOnlyMemory<byte>       ZipLogs()       => Zip( Instance?.Paths.Logs );
    public static Task<ReadOnlyMemory<byte>> ZipLogsAsync()  => Task.Run( ZipLogs );
    public static ReadOnlyMemory<byte>       ZipCache()      => Zip( Instance?.Paths.Cache );
    public static Task<ReadOnlyMemory<byte>> ZipCacheAsync() => Task.Run( ZipCache );
    public static ReadOnlyMemory<byte>       ZipData()       => Zip( Instance?.Paths.AppData );
    public static Task<ReadOnlyMemory<byte>> ZipDataAsync()  => Task.Run( ZipData );
    public static ReadOnlyMemory<byte> Zip( LocalDirectory? directory )
    {
        if ( directory is null || directory.DoesNotExist ) { return ReadOnlyMemory<byte>.Empty; }

        System.Diagnostics.Debug.Assert( directory.Exists );
        using MemoryStream destination = new(1024);

        ZipFile.CreateFromDirectory( directory.FullPath, destination, CompressionLevel.SmallestSize, true, Encoding.Default );

        return destination.ToArray();
    }


    /*
public static async Task<ReadOnlyMemory<byte>> ZipAsync( LocalDirectory directory, CancellationToken token )
{
    const CompressionLevel COMPRESSION_LEVEL = CompressionLevel.SmallestSize;
    Debug.Assert( directory.Exists );
    using MemoryStream destination = new(1024);
    using ZipArchive   archive     = new ZipArchive( destination, ZipArchiveMode.Create, true, Encoding.Default );

    foreach ( LocalDirectory subFolder in directory.GetSubFolders() )
    {
        ZipArchiveEntry entry = archive.CreateEntryFromFile( subFolder.FullPath, subFolder.Name, COMPRESSION_LEVEL );
        entry.LastWriteTime = subFolder.Info.LastWriteTimeUtc;

        foreach ( LocalFile file in subFolder.GetFiles() )
        {
            using MemoryStream source    = await file.ReadAsync().AsStream( token );
            ZipArchiveEntry    fileEntry = entry.Archive.CreateEntry( file.Name, COMPRESSION_LEVEL );
            fileEntry.LastWriteTime = file.Info.LastWriteTimeUtc;
            await using Stream target = fileEntry.Open();
            await source.CopyToAsync( target, token );
        }
    }

    foreach ( LocalFile file in directory.GetFiles() )
    {
        using MemoryStream source    = await file.ReadAsync().AsStream( token );
        ZipArchiveEntry    fileEntry = archive.CreateEntry( file.Name, COMPRESSION_LEVEL );
        fileEntry.LastWriteTime = file.Info.LastWriteTimeUtc;
        await using Stream target = fileEntry.Open();
        await source.CopyToAsync( target, token );
    }

    return destination.ToArray();

    static async Task AddFiles( ZipArchive archive, LocalDirectory directory, CancellationToken token )
    {
        foreach ( LocalFile file in directory.GetFiles() ) { await AddFile( archive.CreateEntry( file.Name, COMPRESSION_LEVEL ), file, token ); }
    }

    static async Task AddFilesToEntry( ZipArchiveEntry entry, LocalDirectory directory, CancellationToken token )
    {
        foreach ( LocalFile file in directory.GetFiles() )
        {
            entry.LastWriteTime = file.Info.LastWriteTimeUtc;
            await AddFile( entry.CreateEntry( file.Name, COMPRESSION_LEVEL ), file, token );
        }
    }


    static async Task AddFile( ZipArchiveEntry entry, LocalFile file, CancellationToken token )
    {
        using MemoryStream source = await file.ReadAsync().AsStream( token );
        entry.LastWriteTime = file.Info.LastWriteTimeUtc;
        await using Stream target = entry.Open();
        await source.CopyToAsync( target, token );
    }
}
private static unsafe string EntryFromPath( ReadOnlySpan<char> path, bool appendPathSeparator = false )
{
    // Remove leading separators.
    int nonSlash = path.IndexOfAnyExcept( '/', '\\' );
    if ( nonSlash < 0 ) { nonSlash = path.Length; }

    path = path.Slice( nonSlash );

    // Replace \ with /, and append a separator if necessary.

    if ( path.IsEmpty )
    {
        return appendPathSeparator
                   ? "/"
                   : string.Empty;
    }

#pragma warning disable CS8500         // takes address of managed type
    ReadOnlySpan<char> tmpPath = path; // avoid address exposing the span and impacting the other code in the method that uses it

    return string.Create( appendPathSeparator
                              ? tmpPath.Length + 1
                              : tmpPath.Length,
                          (appendPathSeparator, RosPtr: (IntPtr)(&tmpPath)),
                          static ( Span<char> dest, (bool appendPathSeparator, IntPtr RosPtr) state ) =>
                          {
                              var path = *(ReadOnlySpan<char>*)state.RosPtr;
                              path.CopyTo( dest );
                              if ( state.appendPathSeparator ) { dest[^1] = '/'; }

                              // To ensure tar files remain compatible with Unix, and per the ZIP File Format Specification 4.4.17.1,
                              // all slashes should be forward slashes.
                              dest.Replace( '\\', '/' );
                          } );
#pragma warning restore CS8500
}
*/



    #region Serilog.ILogger

    /// <summary> Create a Logger that enriches log events via the provided enrichers. </summary>
    /// <param name="enricher"> Enricher that applies in the context. </param>
    /// <returns> A Logger that will enrich log events as specified. </returns>
    public global::Serilog.ILogger ForContext( ILogEventEnricher enricher ) => Logger.ForContext( enricher );
    /// <summary> Create a Logger that enriches log events via the provided enrichers. </summary>
    /// <param name="enrichers"> Enrichers that apply in the context. </param>
    /// <returns> A Logger that will enrich log events as specified. </returns>
    public global::Serilog.ILogger ForContext( IEnumerable<ILogEventEnricher> enrichers ) => Logger.ForContext( enrichers );
    /// <summary> Create a Logger that enriches log events with the specified property. </summary>
    /// <param name="propertyName"> The name of the property. Must be non-empty. </param>
    /// <param name="value"> The property value. </param>
    /// <param name="destructureObjects"> If <see langword="true"/>, the value will be serialized as a structured object if possible; if <see langword="false"/>, the object will be recorded as a scalar or simple array. </param>
    /// <returns> A Logger that will enrich log events as specified. </returns>
    public global::Serilog.ILogger ForContext( string propertyName, object? value, bool destructureObjects = false ) => Logger.ForContext( propertyName, value, destructureObjects );
    /// <summary> Create a Logger that marks log events as being from the specified source type. </summary>
    /// <typeparam name="TSource"> Type generating log messages in the context. </typeparam>
    /// <returns> A Logger that will enrich log events as specified. </returns>
    public global::Serilog.ILogger ForContext<TSource>() => ForContext( typeof(TSource) );
    /// <summary> Create a Logger that marks log events as being from the specified source type. </summary>
    /// <param name="source"> Type generating log messages in the context. </param>
    /// <returns> A Logger that will enrich log events as specified. </returns>
    public global::Serilog.ILogger ForContext( Type source ) => Logger.ForContext( source );
    /// <summary> Write an event to the log. </summary>
    /// <param name="logEvent"> The event to write. </param>
    public void Write( LogEvent logEvent ) => Logger.Write( logEvent );
    /// <summary> Write a log event with the specified level. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write( LogEventLevel level, [StructuredMessageTemplate] string messageTemplate ) => Logger.Write( level, messageTemplate );
    /// <summary> Write a log event with the specified level. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write<T>( LogEventLevel level, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Logger.Write( level, messageTemplate, propertyValue );
    /// <summary> Write a log event with the specified level. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write<T0, T1>( LogEventLevel level, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Logger.Write( level, messageTemplate, propertyValue0, propertyValue1 );
    /// <summary> Write a log event with the specified level. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write<T0, T1, T2>( LogEventLevel level, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Logger.Write( level, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );
    /// <summary> Write a log event with the specified level. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="messageTemplate"> </param>
    /// <param name="propertyValues"> </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write( LogEventLevel level, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Logger.Write( level, messageTemplate, propertyValues );
    /// <summary> Write a log event with the specified level and associated exception. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write( LogEventLevel level, Exception? exception, [StructuredMessageTemplate] string messageTemplate ) => Logger.Write( level, exception, messageTemplate );
    /// <summary> Write a log event with the specified level and associated exception. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write<T>( LogEventLevel level, Exception? exception, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Logger.Write( level, exception, messageTemplate, propertyValue );
    /// <summary> Write a log event with the specified level and associated exception. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write<T0, T1>( LogEventLevel level, Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Logger.Write( level, exception, messageTemplate, propertyValue0, propertyValue1 );
    /// <summary> Write a log event with the specified level and associated exception. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write<T0, T1, T2>( LogEventLevel level, Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Logger.Write( level, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );
    /// <summary> Write a log event with the specified level and associated exception. </summary>
    /// <param name="level"> The level of the event. </param>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Write( LogEventLevel level, Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Logger.Write( level, exception, messageTemplate, propertyValues );
    /// <summary> Determine if events at the specified level will be passed through to the log sinks. </summary>
    /// <param name="level"> Level to check. </param>
    /// <returns> <see langword="true"/> if the level is enabled; otherwise, <see langword="false"/>. </returns>
    public bool IsEnabled( LogEventLevel level ) => Logger.IsEnabled( level );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose( [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Verbose, messageTemplate, _noPropertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose<T>( [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Verbose, messageTemplate, propertyValue );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose<T0, T1>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Verbose, messageTemplate, propertyValue0, propertyValue1 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose<T0, T1, T2>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Verbose, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose("Staring into space, wondering if we're alone.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose( [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Verbose( (Exception?)null, messageTemplate, propertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose( Exception? exception, [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Verbose, exception, messageTemplate, _noPropertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose<T>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Verbose, exception, messageTemplate, propertyValue );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose<T0, T1>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Verbose, exception, messageTemplate, propertyValue0, propertyValue1 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose<T0, T1, T2>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Verbose, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Verbose"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Verbose(ex, "Staring into space, wondering where this comet came from.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Verbose( Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Write( LogEventLevel.Verbose, exception, messageTemplate, propertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug( [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Debug, messageTemplate, _noPropertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug<T>( [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Debug, messageTemplate, propertyValue );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug<T0, T1>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Debug, messageTemplate, propertyValue0, propertyValue1 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug<T0, T1, T2>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Debug, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug("Starting up at {StartedAt}.", DateTime.Now);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug( [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Debug( (Exception?)null, messageTemplate, propertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug( Exception? exception, [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Debug, exception, messageTemplate, _noPropertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug<T>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Debug, exception, messageTemplate, propertyValue );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug<T0, T1>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Debug, exception, messageTemplate, propertyValue0, propertyValue1 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug<T0, T1, T2>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Debug, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Debug"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Debug(ex, "Swallowing a mundane exception.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Debug( Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Write( LogEventLevel.Debug, exception, messageTemplate, propertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information( [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Information, messageTemplate, _noPropertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information<T>( [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Information, messageTemplate, propertyValue );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information<T0, T1>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Information, messageTemplate, propertyValue0, propertyValue1 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information<T0, T1, T2>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Information, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information("Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information( [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Information( (Exception?)null, messageTemplate, propertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information( Exception? exception, [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Information, exception, messageTemplate, _noPropertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information<T>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Information, exception, messageTemplate, propertyValue );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information<T0, T1>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Information, exception, messageTemplate, propertyValue0, propertyValue1 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information<T0, T1, T2>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Information, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Information"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Information(ex, "Processed {RecordCount} records in {TimeMS}.", records.Length, sw.ElapsedMilliseconds);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Information( Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Write( LogEventLevel.Information, exception, messageTemplate, propertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning( [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Warning, messageTemplate, _noPropertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning<T>( [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Warning, messageTemplate, propertyValue );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning<T0, T1>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Warning, messageTemplate, propertyValue0, propertyValue1 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning<T0, T1, T2>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Warning, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning("Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning( [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Warning( (Exception?)null, messageTemplate, propertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning( Exception? exception, [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Warning, exception, messageTemplate, _noPropertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning<T>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Warning, exception, messageTemplate, propertyValue );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning<T0, T1>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Warning, exception, messageTemplate, propertyValue0, propertyValue1 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning<T0, T1, T2>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Warning, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Warning"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Warning(ex, "Skipped {SkipCount} records.", skippedRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Warning( Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Write( LogEventLevel.Warning, exception, messageTemplate, propertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error( [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Error, messageTemplate, _noPropertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error<T>( [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Error, messageTemplate, propertyValue );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error<T0, T1>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Error, messageTemplate, propertyValue0, propertyValue1 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error<T0, T1, T2>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Error, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error("Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error( [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Error( (Exception?)null, messageTemplate, propertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error( Exception? exception, [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Error, exception, messageTemplate, _noPropertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error<T>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Error, exception, messageTemplate, propertyValue );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error<T0, T1>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Error, exception, messageTemplate, propertyValue0, propertyValue1 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error<T0, T1, T2>( Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Error, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Error( Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Write( LogEventLevel.Error, exception, messageTemplate, propertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal("Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal( [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Fatal, messageTemplate, _noPropertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal("Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal<T>( [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Fatal, messageTemplate, propertyValue );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal("Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal<T0, T1>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Fatal, messageTemplate, propertyValue0, propertyValue1 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal("Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal<T0, T1, T2>( [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Fatal, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception. </summary>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal("Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal( string messageTemplate, params object?[]? propertyValues ) => Fatal( (Exception?)null, messageTemplate, propertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal( Exception? exception, [StructuredMessageTemplate] string messageTemplate ) => Write( LogEventLevel.Fatal, exception, messageTemplate, _noPropertyValues );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal<T>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T propertyValue ) => Write( LogEventLevel.Fatal, exception, messageTemplate, propertyValue );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal<T0, T1>( Exception? exception, [StructuredMessageTemplate] string messageTemplate, T0 propertyValue0, T1 propertyValue1 ) => Write( LogEventLevel.Fatal, exception, messageTemplate, propertyValue0, propertyValue1 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValue0"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue1"> Object positionally formatted into the message template. </param>
    /// <param name="propertyValue2"> Object positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal<T0, T1, T2>( Exception? exception, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2 ) => Write( LogEventLevel.Fatal, exception, messageTemplate, propertyValue0, propertyValue1, propertyValue2 );
    /// <summary> Write a log event with the <see cref="LogEventLevel.Fatal"/> level and associated exception. </summary>
    /// <param name="exception"> Exception related to the event. </param>
    /// <param name="messageTemplate"> Message template describing the event. </param>
    /// <param name="propertyValues"> Objects positionally formatted into the message template. </param>
    /// <example>
    ///     <code>
    /// Log.Fatal(ex, "Process terminating.");
    /// </code>
    /// </example>
    [MessageTemplateFormatMethod( nameof(messageTemplate) )]
    public void Fatal( Exception? exception, [StructuredMessageTemplate] string messageTemplate, params object?[]? propertyValues ) => Write( LogEventLevel.Fatal, exception, messageTemplate, propertyValues );

    #endregion
}
