using System.IO;
using System.IO.Compression;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
public abstract partial class Serilogger<TSerilogger, TSeriloggerSettings, TApp> : ISerilogger, IAsyncDisposable
    where TSerilogger : Serilogger<TSerilogger, TSeriloggerSettings, TApp>, ICreateSerilogger<TSerilogger>
    where TSeriloggerSettings : class, ICreateSeriloggerSettings<TSeriloggerSettings>
    where TApp : IAppID
{
    public const            string                            CONSOLE_DEFAULT_OUTPUT_TEMPLATE     = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const            string                            DEBUG_DEFAULT_DEBUG_OUTPUT_TEMPLATE = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
    public const            long                              DEFAULT_FILE_SIZE_LIMIT_BYTES       = 1L * 1024 * 1024 * 1024; // 1GB
    public const            int                               DEFAULT_RETAINED_FILE_COUNT_LIMIT   = 31;                      // A long month of logs
    public const            string                            SEQ_API_KEY_NAME                    = "X-Seq-ApiKey";
    public const            string                            SEQ_BUFFER_DIRECTORY                = "SeqBuffer";
    public static readonly  string                            SharedName                          = typeof(TSerilogger).Name;
    private static          Guid?                             _deviceID;
    private static          long?                             _deviceIDLong;
    private static readonly object[]                          _noPropertyValues = [];
    public static readonly  FileData[]                        Empty             = [];
    private readonly        SerilogLoggerProvider             _provider;
    private readonly        Synchronized<TSeriloggerSettings> _settings;
    private                 LocalFile?                        _screenShotAddress;


    public static bool ApplyThemeToRedirectedOutput { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public static Guid DebugID                      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = Guid.NewGuid();
    public static Guid DeviceID
    {
        get
        {
            if ( ISerilogger.Instance?.Settings.IsDebuggable is true ) { return DebugID; }

            if ( _deviceID.HasValue ) { return _deviceID.Value; }

            _deviceID = SharedName.GetPreference( nameof(DeviceID), Guid.NewGuid() );
            Activity.Current?.SetTag( nameof(DeviceID), _deviceID.ToString() );
            return _deviceID.Value;
        }
        set
        {
            _deviceID = value;
            SharedName.SetPreference( nameof(DeviceID), value );
            Activity.Current?.SetTag( nameof(DeviceID), value.ToString() );
        }
    }
    public static long DeviceIDLong
    {
        get
        {
            if ( _deviceIDLong.HasValue ) { return _deviceIDLong.Value; }

            _deviceIDLong = SharedName.GetPreference( nameof(DeviceIDLong), 0L );
            Activity.Current?.SetTag( nameof(DeviceIDLong), _deviceIDLong.ToString() );
            return _deviceIDLong.Value;
        }
        set
        {
            _deviceIDLong = value;
            SharedName.SetPreference( nameof(DeviceIDLong), value );
            Activity.Current?.SetTag( nameof(DeviceIDLong), value.ToString() );
        }
    }
    public static string                     DeviceName        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = string.Empty;
    public static FileLifecycleHooks?        Hooks             { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public static IAsyncLogEventSinkMonitor? Monitor           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public static object?                    SyncRoot          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public static ConsoleTheme?              Theme             { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public        ActivitySource             ActivitySource    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public        Activity                   Activity          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; protected set; }
    public        Guid                       AppID             { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = TApp.AppID;
    public        string                     AppName           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = TApp.AppName;
    public        AppVersion                 AppVersion        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; } = TApp.AppVersion;
    public        bool                       CannotDebug       { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Settings.EnableApi is false; }
    Guid IDeviceID.                          DeviceID          => DeviceID;
    string IDeviceName.                      DeviceName        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => DeviceName; }
    public bool                              Disabled          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Enabled is false; }
    public bool                              Enabled           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Settings.EnableApi; }
    public bool                              EnableDebugEvents { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public DebugLogEvent.Collection          Events            { get; } = [];
    public bool                              IsValid           => DeviceID.IsValidID() || DeviceIDLong != 0;
    public Logger                            Logger            { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public MessageEvent.Collection           Messages          { get; } = [];
    public LocalFile? ScreenShotAddress
    {
        get => _screenShotAddress ??= Settings.Paths.Cache.Join( IFilePaths.SCREEN_SHOT_FILE );
        set
        {
            _screenShotAddress?.Dispose();
            _screenShotAddress = value?.SetTemporary();
        }
    }
    public          ReadOnlyMemory<byte>                                     ScreenShotData     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get;                    protected internal set; }
    public          TSeriloggerSettings                                      Settings           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _settings.Value; set => _settings.Value = value; }
    public required Func<CancellationToken, ValueTask<ReadOnlyMemory<byte>>> TakeScreenShot     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get;                    init; }
    public required Func<EventDetails, EventDetails>                         UpdateEventDetails { [MethodImpl( MethodImplOptions.AggressiveInlining )] get;                    init; }
    ISeriloggerSettings ISerilogger.                                         Settings           => Settings;


    static Serilogger() => SelfLog.Enable( static message =>
                                           {
                                               // System.Diagnostics.Debug.WriteLine( message );
                                               // Console.WriteLine( message );
                                               Console.Error.WriteLine( message );
                                           } );
    protected Serilogger( ActivitySource source, Logger logger, IFilePaths paths )
    {
        ActivitySource = source;
        ActivitySource.AddActivityListener( GetActivityListener() );
        Activity   = GetActivity( AppName );
        Log.Logger = Logger = logger;
        _settings  = new Synchronized<TSeriloggerSettings>( paths.FromPreferences<TSeriloggerSettings>() );
        _provider  = new SerilogLoggerProvider( this, true );
        System.Diagnostics.Debug.Assert( IsValid, $"{SharedName} is invalid" );
        ISerilogger.Instance = this;
    }
    public void Dispose()
    {
        ScreenShotData = ReadOnlyMemory<byte>.Empty;
        Events.Dispose();
        _provider.Dispose();
        _screenShotAddress?.Dispose();
        Settings.Dispose();
        Activity.Dispose();
        Logger.Dispose();

        GC.SuppressFinalize( this );
    }
    public async ValueTask DisposeAsync()
    {
        ScreenShotData = ReadOnlyMemory<byte>.Empty;
        await CastAndDispose( Events );
        await _provider.DisposeAsync();
        if ( _screenShotAddress is not null ) { await CastAndDispose( _screenShotAddress ); }

        Settings.Dispose();
        Activity.Dispose();
        await Logger.DisposeAsync();

        GC.SuppressFinalize( this );
        return;

        static async ValueTask CastAndDispose( IDisposable resource )
        {
            if ( resource is IAsyncDisposable resourceAsyncDisposable ) { await resourceAsyncDisposable.DisposeAsync(); }
            else { resource.Dispose(); }
        }
    }


    public static TSerilogger Create( IServiceProvider provider ) => Create( provider.GetRequiredService<IOptions<SeriloggerOptions>>().Value );
    public static TSerilogger Create( SeriloggerOptions options )
    {
        ActivitySource source = options.ActivitySource ??= new ActivitySource( TApp.AppName, TApp.AppVersion.ToString() );
        return Create( source, options );
    }
    public static TSerilogger Create( ActivitySource source, SeriloggerOptions options )
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

        // builder.WriteTo.Async( ConfigureFileSink, 10000, true, Monitor );

        ConfigureDebugSink( builder.WriteTo );

        options.AddNativeLogs?.Invoke( builder );

        if ( options.RemoteLogServer is not null ) { builder.WriteTo.Sink( RemoteLogger.Create( options.RemoteLogServer ) ); }

        if ( options.SeqLogServer is not null ) { builder.WriteTo.Seq( options.SeqLogServer.OriginalString, apiKey: options.SeqApiKey, formatProvider: CultureInfo.CurrentCulture ); }

        TSerilogger logger = TSerilogger.Create( source, builder.CreateLogger(), options );


        return logger.ClearCache();

        /*
        void ConfigureFileSink( LoggerSinkConfiguration sink ) => sink.File( new CompactJsonFormatter(),
                                                                             options.Paths.LogsFile.FullPath,
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
                                                                             */
    }


    public static void ConfigureConsoleSink( LoggerSinkConfiguration sink ) => sink.Console( LogEventLevel.Information, CONSOLE_DEFAULT_OUTPUT_TEMPLATE, CultureInfo.InvariantCulture, null, LogEventLevel.Error, Theme, ApplyThemeToRedirectedOutput, SyncRoot );
    public static void ConfigureDebugSink( LoggerSinkConfiguration   sink ) => sink.Debug( LogEventLevel.Verbose, DEBUG_DEFAULT_DEBUG_OUTPUT_TEMPLATE, CultureInfo.InvariantCulture );
    public TSerilogger ClearCache()
    {
        foreach ( LocalFile file in Settings.Paths.Cache.GetFiles() ) { file.Delete(); }

        return (TSerilogger)this;
    }


    public        Activity GetActivity( Activity?      parent = null, ActivityKind kind   = ActivityKind.Internal )                           => GetActivity( ActivitySource, parent,       kind );
    public static Activity GetActivity( ActivitySource source,        Activity?    parent = null, ActivityKind kind = ActivityKind.Internal ) => GetActivity( source,         TApp.AppName, TApp.AppName, TApp.AppID, TApp.AppVersion, parent, kind );
    public        Activity GetActivity( string         name,          Activity?    parent = null, ActivityKind kind = ActivityKind.Internal ) => GetActivity( ActivitySource, name,         AppName,      AppID,      AppVersion,      parent, kind );
    public static Activity GetActivity( ActivitySource source, string name, string appName, Guid appID, AppVersion appVersion, Activity? parent = null, ActivityKind kind = ActivityKind.Internal )
    {
        parent ??= Activity.Current;
        ActivityContext parentContext = parent?.Context ?? new ActivityContext();

        Activity activity = source.CreateActivity( name, kind, parentContext ) ??
                            new Activity( name )
                            {
                                DisplayName = name,
                            };

        activity.SetStartTime( DateTime.UtcNow );
        activity.SetIdFormat( ActivityIdFormat.Hierarchical );
        activity.SetStatus( ActivityStatusCode.Ok );
        activity.SetTag( nameof(AppName),    appName );
        activity.SetTag( nameof(AppID),      appID.ToString() );
        activity.SetTag( nameof(AppVersion), appVersion.ToString() );
        activity.SetTag( nameof(DeviceID),   DeviceID.ToString() );
        activity.SetTag( nameof(DebugID),    DebugID.ToString() );
        activity.Start();
        return activity;
    }


    public ActivityListener GetActivityListener() => new()
                                                     {
                                                         ActivityStarted     = ActivityStarted,
                                                         ActivityStopped     = ActivityStopped,
                                                         ExceptionRecorder   = ExceptionRecorder,
                                                         SampleUsingParentId = SampleUsingParentId,
                                                         Sample              = Sample,
                                                         ShouldListenTo      = ShouldListenTo
                                                     };


    protected virtual bool                   ShouldListenTo( ActivitySource                                    source )   => true;
    protected virtual ActivitySamplingResult SampleUsingParentId( ref ActivityCreationOptions<string>          options )  => ActivitySamplingResult.None;
    protected virtual ActivitySamplingResult Sample( ref              ActivityCreationOptions<ActivityContext> options )  => ActivitySamplingResult.None;
    protected virtual void                   ActivityStarted( Activity                                         activity ) { TrackEvent( activity.DisplayName ); }
    protected virtual void                   ActivityStopped( Activity                                         activity ) { TrackEvent( activity.DisplayName ); }
    protected virtual void ExceptionRecorder( Activity activity, Exception exception, ref TagList tags )
    {
        Dictionary<string, object?> dictionary = new(tags.Count);
        foreach ( (string? key, object? value) in tags ) { dictionary[key] = value; }

        Error( exception, "{ActivityOperationName} : {ActivityDisplayName} | {@Tags}", activity.OperationName, activity.DisplayName, dictionary );
    }


    public static TSerilogger     Get( IServiceProvider         provider )                                                         => provider.GetRequiredService<TSerilogger>();
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

        ScreenShotData = Settings.TakeScreenshotOnError
                             ? await TakeScreenShot( CancellationToken.None )
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


    public void TrackError<T>( T _, Exception exception, EventDetails? details, IEnumerable<FileData>? attachments, [CallerMemberName] string caller = BaseRecord.EMPTY )
    {
        if ( Settings.EnableApi is false || Settings.EnableCrashes is false ) { return; }

        using Disposables disposables = new();
        disposables.Add( LogContext.PushProperty( nameof(exception),                 exception ) );
        disposables.Add( LogContext.PushProperty( $"{exception.GetType().Name}.txt", $"\n\n{exception}\n\n" ) );
        disposables.Add( LogContext.PushProperty( nameof(EventDetails),              details?.ToPrettyJson() ) );

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

        await Settings.Paths.FeedbackFile.WriteAsync( result.ToPrettyJson() );
    }


    public EventDetails AppState() => UpdateEventDetails( EventDetails.AppState( AppName ) );


    public async ValueTask<bool> BufferScreenShot( CancellationToken token = default )
    {
        ReadOnlyMemory<byte> data = ScreenShotData = await TakeScreenShot( token );
        return data.IsEmpty is false;
    }
    public async ValueTask<LocalFile?> GetScreenShot( CancellationToken token = default )
    {
        ReadOnlyMemory<byte> screenShot = await TakeScreenShot( token );
        return await WriteScreenShot( screenShot, token );
    }
    public async ValueTask<LocalFile?> WriteScreenShot( CancellationToken token = default ) => await WriteScreenShot( ScreenShotData, token );
    public async ValueTask<LocalFile?> WriteScreenShot( ReadOnlyMemory<byte> memory, CancellationToken token = default )
    {
        if ( memory.IsEmpty ) { return null; }

        LocalFile file = Settings.Paths.Screenshot;
        await file.WriteAsync( memory, token );
        return file;
    }


    public static void SetDeviceID( Guid deviceID ) => DeviceID = deviceID;
    public static void SetDeviceID( long deviceID ) => DeviceIDLong = deviceID;
    public void SetSettings( TSeriloggerSettings settings )
    {
        Settings = settings.Clone();
        Settings.SetPreferences();
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
}



public abstract class Serilogger<TClass, TApp>( ActivitySource source, Logger logger, IFilePaths paths ) : Serilogger<TClass, SeriloggerSettings, TApp>( source, logger, paths )
    where TApp : IAppID
    where TClass : Serilogger<TClass, SeriloggerSettings, TApp>, ICreateSerilogger<TClass>;
