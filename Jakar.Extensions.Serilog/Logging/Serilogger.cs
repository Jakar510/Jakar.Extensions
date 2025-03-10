using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Context;
using Serilog.Debugging;
using Serilog.Enrichers.Span;
using Serilog.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;



namespace Jakar.Extensions.Serilog;


[SuppressMessage( "ReSharper", "AsyncVoidLambda" ), SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" ), SuppressMessage( "ReSharper", "StaticMemberInGenericType" ), SuppressMessage( "ReSharper", "CollectionNeverQueried.Local" )]
public abstract partial class Serilogger<TSerilogger, TSeriloggerSettings, TApp> : SeriloggerConstants, ISerilogger, IAsyncDisposable
    where TSerilogger : Serilogger<TSerilogger, TSeriloggerSettings, TApp>, ICreateSerilogger<TSerilogger, TApp>
    where TSeriloggerSettings : class, ICreateSeriloggerSettings<TSeriloggerSettings, TApp>
    where TApp : IApp
{
    public readonly        SeriloggerOptions<TApp>           Options;
    public static readonly string                            SharedName = typeof(TSerilogger).Name;
    private readonly       SerilogLoggerProvider             _provider;
    private readonly       Synchronized<TSeriloggerSettings> _settings;
    private                LocalFile?                        _screenShotAddress;


    public ActivitySource           ActivitySource    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public Activity                 Activity          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; protected set; }
    public bool                     CannotDebug       { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Settings.EnableApi is false; }
    Guid IDeviceID.                 DeviceID          => Options.DeviceID;
    string IDeviceName.             DeviceName        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Options.DeviceName; }
    public bool                     Disabled          { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Enabled is false; }
    public bool                     Enabled           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Settings.EnableApi; }
    public bool                     EnableDebugEvents { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; }
    public DebugLogEvent.Collection Events            { get; } = [];
    public bool                     IsValid           => Options.DeviceID.IsValidID() || Options.DeviceIDLong != 0;
    public Logger                   Logger            { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; init; }
    public MessageEvent.Collection  Messages          { get; } = [];
    public LocalFile? ScreenShotAddress
    {
        get => _screenShotAddress ??= Settings.Paths.Cache.Join( IFilePaths.SCREEN_SHOT_FILE );
        set
        {
            _screenShotAddress?.Dispose();
            _screenShotAddress = value?.SetTemporary();
        }
    }
    public ReadOnlyMemory<byte>                                     ScreenShotData     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get;                    protected internal set; }
    public TSeriloggerSettings                                      Settings           { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => _settings.Value; set => _settings.Value = value; }
    public Func<CancellationToken, ValueTask<ReadOnlyMemory<byte>>> TakeScreenShot     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Options.TakeScreenShot; }
    public Func<EventDetails, EventDetails>                         UpdateEventDetails { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Options.UpdateEventDetails; }
    ISeriloggerSettings ISerilogger.                                Settings           => Settings;


    static Serilogger() => SelfLog.Enable( static message =>
                                           {
                                               // System.Diagnostics.Debug.WriteLine( message );
                                               // Console.WriteLine( message );
                                               Console.Error.WriteLine( message );
                                           } );
    protected Serilogger( SeriloggerOptions<TApp> options )
    {
        Options = options;
        ActivitySource source = options.ActivitySource ??= new ActivitySource( Options.AppName, options.AppVersion.ToString() );

        ActivitySource = source;
        ActivitySource.AddActivityListener( GetActivityListener() );
        Activity             = GetActivity( Options.AppName );
        Log.Logger           = Logger = CreateLogger( in Options );
        _settings            = new Synchronized<TSeriloggerSettings>( Options.FromPreferences<TSeriloggerSettings, TApp>() );
        _provider            = new SerilogLoggerProvider( this, true );
        ISerilogger.Instance = this;
        ClearCache();
    }
    protected virtual Logger CreateLogger( ref readonly SeriloggerOptions<TApp> options )
    {
        LoggerConfiguration builder = new();

        builder.MinimumLevel.Verbose();
        builder.MinimumLevel.Override( nameof(Microsoft), LogEventLevel.Warning );
        builder.MinimumLevel.Override( nameof(System),    LogEventLevel.Warning );

        builder.Enrich.WithProperty( Constants.SourceContextPropertyName, options.AppName );
        builder.Enrich.WithProperty( nameof(IAppID.AppID),                options.AppID.ToString() );
        builder.Enrich.WithProperty( nameof(AppVersion),                  options.AppVersion.ToString() );
        builder.Enrich.FromLogContext();
        builder.Enrich.With<AppContextEnricher<TApp>>();

        builder.Enrich.WithSpan( new SpanOptions
                                 {
                                     IncludeBaggage       = true,
                                     IncludeOperationName = true,
                                     IncludeTags          = true,
                                     IncludeTraceFlags    = true
                                 } );


        options.Debug?.WriteTo( builder.WriteTo );
        options.Console?.WriteTo( builder.WriteTo );
        options.AddNativeLogs?.Invoke( builder );
        options.Seq?.WriteTo( builder.WriteTo );

        return builder.CreateLogger();
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


    public static TSerilogger Create( IServiceProvider provider ) => TSerilogger.Create( provider.GetRequiredService<IOptions<SeriloggerOptions<TApp>>>().Value );


    public TSerilogger ClearCache()
    {
        foreach ( LocalFile file in Settings.Paths.Cache.GetFiles() ) { file.Delete(); }

        return (TSerilogger)this;
    }


    public        Activity GetActivity( string         name,   Activity?               parent = null, ActivityKind kind   = ActivityKind.Internal )                           => GetActivity( ActivitySource, name,         Options, parent, kind );
    public static Activity GetActivity( ActivitySource source, SeriloggerOptions<TApp> options,       Activity?    parent = null, ActivityKind kind = ActivityKind.Internal ) => GetActivity( source,         TApp.AppName, options, parent, kind );
    public static Activity GetActivity( ActivitySource source, string name, SeriloggerOptions<TApp> options, Activity? parent = null, ActivityKind kind = ActivityKind.Internal )
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
        activity.SetTag( nameof(IAppID.AppName),     options.AppName );
        activity.SetTag( nameof(IAppID.AppID),       options.AppID.ToString() );
        activity.SetTag( nameof(IAppID.AppVersion),  options.AppVersion.ToString() );
        activity.SetTag( nameof(IDeviceID.DeviceID), options.DeviceID.ToString() );
        activity.SetTag( nameof(IApp.DebugID),       options.DebugID.ToString() );
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


    protected virtual bool                   ShouldListenTo( ActivitySource                                    source )   => string.Equals( source.Name, Options.AppName, StringComparison.Ordinal );
    protected virtual ActivitySamplingResult SampleUsingParentId( ref ActivityCreationOptions<string>          options )  => ActivitySamplingResult.AllData;
    protected virtual ActivitySamplingResult Sample( ref              ActivityCreationOptions<ActivityContext> options )  => ActivitySamplingResult.AllData;
    protected virtual void                   ActivityStarted( Activity                                         activity ) { TrackEvent( activity, $"{nameof(ActivityStarted)}.{activity.DisplayName}" ); }
    protected virtual void                   ActivityStopped( Activity                                         activity ) { TrackEvent( activity, $"{nameof(ActivityStopped)}.{activity.DisplayName}" ); }
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
        if ( Settings.IncludeAppStateOnError ) { EventDetails.AddAppState( in result, Options.AppName ); }

        await Settings.Paths.FeedbackFile.WriteAsync( result.ToPrettyJson() );
    }


    public EventDetails AppState() => UpdateEventDetails( EventDetails.AppState( Options.AppName ) );


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


    public void SetDeviceID( Guid deviceID ) => Options.DeviceID = deviceID;
    public void SetDeviceID( long deviceID ) => Options.DeviceIDLong = deviceID;
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
