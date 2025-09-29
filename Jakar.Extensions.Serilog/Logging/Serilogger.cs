using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog.Context;
using Serilog.Debugging;
using Serilog.Enrichers.Span;
using Serilog.Extensions.Logging;



namespace Jakar.Extensions.Serilog;


public partial class Serilogger : SeriloggerConstants, ISerilogger, IAsyncDisposable, ICreateSerilogger<Serilogger>
{
    public static readonly string                            SharedName = nameof(Serilogger);
    public readonly        DebugLogEvent.Collection          Events     = [];
    public readonly        MessageEvent.Collection           Messages   = [];
    public readonly        Logger                            Logger;
    public readonly        SeriloggerOptions                 Options;
    private readonly       Synchronized<ISeriloggerSettings> __settings;
    private                LocalFile?                        __screenShotAddress;


    public Activity                      Activity    { get; protected set; }
    public bool                          CannotDebug => !Settings.EnableApi;
    Guid IDeviceID.                      DeviceID    => Options.DeviceID;
    string IDeviceName.                  DeviceName  => Options.DeviceName;
    DebugLogEvent.Collection ISerilogger.Events      => Events;
    public bool                          IsValid     => Options.DeviceID.IsValidID() || Options.DeviceIDLong != 0;
    Logger ISerilogger.                  Logger      => Logger;
    MessageEvent.Collection ISerilogger. Messages    => Messages;
    public FilePaths                     Paths       { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Options.Paths; }
    public SerilogLoggerProvider         Provider    { [Pure, MustDisposeResource] get => new(this, true); }
    public LocalFile? ScreenShotAddress
    {
        get => __screenShotAddress ??= Paths.Cache.Join( FilePaths.SCREEN_SHOT_FILE );
        set
        {
            __screenShotAddress?.Dispose();
            __screenShotAddress = value?.SetTemporary();
        }
    }
    public ReadOnlyMemory<byte>     ScreenShotData { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Options.ScreenShotData; }
    public ISeriloggerSettings      Settings       { get => __settings.Value; set => __settings.Value = value; }
    ISeriloggerSettings ISerilogger.Settings       => Settings;


    static Serilogger() => global::Serilog.Debugging.SelfLog.Enable( static message =>
                                                                     {
                                                                         // System.Diagnostics.Debug.WriteLine( message );
                                                                         // Console.WriteLine( message );
                                                                         Console.Error.WriteLine( message );
                                                                     } );
    protected Serilogger( SeriloggerOptions options )
    {
        Options = options;
        ActivitySource.AddActivityListener( GetActivityListener() );
        Activity             = GetActivity( Options.AppName );
        Log.Logger           = Logger = CreateLogger( in Options );
        __settings            = new Synchronized<ISeriloggerSettings>( Options.FromPreferences<SeriloggerSettings>() );
        ISerilogger.Instance = this;
        ClearCache();
    }


    public Logger CreateLogger( Type                           type )                                                         => (Logger)Logger.ForContext( type );
    public Logger CreateLogger( IEnumerable<ILogEventEnricher> enrichers )                                                    => (Logger)Logger.ForContext( enrichers );
    public Logger CreateLogger( ILogEventEnricher              enricher )                                                     => (Logger)Logger.ForContext( enricher );
    public Logger CreateLogger( string                         propertyName, object? value, bool destructureObjects = false ) => (Logger)Logger.ForContext( propertyName, value, destructureObjects );
    public Logger CreateLogger<TValue>() => (Logger)Logger.ForContext<TValue>();
    protected virtual Logger CreateLogger( ref readonly SeriloggerOptions options )
    {
        LoggerConfiguration builder = new();

        builder.MinimumLevel.Verbose();
        builder.MinimumLevel.Override( nameof(Microsoft), LogEventLevel.Warning );
        builder.MinimumLevel.Override( nameof(System),    LogEventLevel.Warning );
        builder.Enrich.WithProperty( Constants.SourceContextPropertyName, options.AppName );
        builder.Enrich.FromLogContext();
        builder.Enrich.With( AppContextEnricher.Create( options ) );

        builder.Enrich.WithSpan( new SpanOptions
                                 {
                                     IncludeBaggage       = true,
                                     IncludeOperationName = true,
                                     IncludeTags          = true,
                                     IncludeTraceFlags    = true,
                                     LogEventPropertiesNames = new SpanLogEventPropertiesNames
                                                               {
                                                                   OperationName = "OperationName",
                                                                   ParentId      = "ParentID",
                                                                   SpanId        = "SpanID",
                                                                   TraceId       = "TraceID"
                                                               }
                                 } );


        options.Debug?.WriteTo( builder.WriteTo );
        options.Console?.WriteTo( builder.WriteTo );
        options.AddNativeLogs?.Invoke( builder );
        options.Seq?.WriteTo( builder.WriteTo );

        return builder.CreateLogger();
    }
    public virtual void Dispose()
    {
        __screenShotAddress?.Dispose();
        __screenShotAddress = null;
        Events.Dispose();
        Options.Dispose();
        Settings.Dispose();
        Activity.Dispose();
        Logger.Dispose();

        GC.SuppressFinalize( this );
    }
    public virtual async ValueTask DisposeAsync()
    {
        __screenShotAddress?.Dispose();
        __screenShotAddress = null;
        Events.Dispose();
        Options.Dispose();
        Settings.Dispose();
        Activity.Dispose();
        Activity.Dispose();
        await Logger.DisposeAsync();

        GC.SuppressFinalize( this );
    }


    public static Serilogger Create( SeriloggerOptions options )  => new(options);
    public static Serilogger Create( IServiceProvider  provider ) => Create( provider.GetRequiredService<IOptions<SeriloggerOptions>>().Value );


    public Serilogger ClearCache()
    {
        foreach ( LocalFile file in Paths.Cache.GetFiles() ) { file.Delete(); }

        return this;
    }


    public Activity GetActivity( string name, Activity? parent = null, ActivityKind kind = ActivityKind.Internal ) => Options.GetActivity( name, parent, kind );


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
    protected virtual void                   ActivityStarted( Activity                                         activity ) => TrackEvent( activity, $"{nameof(ActivityStarted)}.{activity.DisplayName}" );
    protected virtual void                   ActivityStopped( Activity                                         activity ) => TrackEvent( activity, $"{nameof(ActivityStopped)}.{activity.DisplayName}" );
    protected virtual void ExceptionRecorder( Activity activity, Exception exception, ref TagList tags )
    {
        Dictionary<string, object?> dictionary = new(tags.Count);
        foreach ( (string? key, object? value) in tags ) { dictionary[key] = value; }

        Error( exception, "{ActivityOperationName} : {ActivityDisplayName} | {@Tags}", activity.OperationName, activity.DisplayName, dictionary );
    }


    public static                       Serilogger      Get( IServiceProvider         provider )                                              => provider.GetRequiredService<Serilogger>();
    [MustDisposeResource] public static ILoggerProvider GetProvider( IServiceProvider provider )                                              => Get( provider ).Provider;
    void ILogEventSink.                                 Emit( LogEvent                logEvent )                                              => ((ILogEventSink)Logger).Emit( logEvent );
    public void                                         HandleException<TValue>( TValue         _, Exception exception )                                => HandleException( _, exception, Empty );
    public void                                         HandleException<TValue>( TValue         _, Exception exception, params FileData[] attachments ) => Task.Run( async () => await HandleExceptionAsync( _, exception, attachments ) );
    public ValueTask                                    HandleExceptionAsync<TValue>( TValue    _, Exception exception ) => HandleExceptionAsync( _, exception, Empty );
    public async ValueTask HandleExceptionAsync<TValue>( TValue _, Exception exception, params FileData[] attachments )
    {
        System.Diagnostics.Debug.WriteLine( exception.ToString() );
        if ( !Settings.EnableApi ) { return; }

        if ( exception is OperationCanceledException or CredentialsException ) { return; }

        await Options.BufferScreenShot();
        TrackError( _, exception, AppState(), attachments );
    }


    public void TrackEvent<TValue>( TValue _, [CallerMemberName] string caller = BaseRecord.EMPTY )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace( caller );
        if ( !IsEnabled( LogEventLevel.Verbose ) ) { return; }

        Debug( "[{ClassName}.{Caller}.{EventId}]", typeof(TValue).Name, caller, caller.GetHashCode() );
        if ( Events.IsEnabled ) { Events.Add( DebugLogEvent.Create<TValue>( caller, LogEventLevel.Debug, null, caller ) ); }
    }
    public void TrackEvent<TValue>( TValue _, EventDetails properties, [CallerMemberName] string caller = BaseRecord.EMPTY )
    {
        if ( !Settings.EnableApi || !Settings.EnableCrashes ) { return; }

        ArgumentException.ThrowIfNullOrWhiteSpace( caller );
        if ( !IsEnabled( LogEventLevel.Verbose ) ) { return; }

        Debug( "[{ClassName}.{Caller}.{EventId}]", typeof(TValue).Name, caller, caller.GetHashCode() );
        if ( Events.IsEnabled ) { Events.Add( DebugLogEvent.Create<TValue>( caller, LogEventLevel.Debug, properties, caller ) ); }
    }
    public void TrackEvent<TValue>( TValue _, string eventType, EventDetails? properties, [CallerMemberName] string caller = BaseRecord.EMPTY )
    {
        if ( !Settings.EnableApi || !Settings.EnableCrashes ) { return; }

        ArgumentException.ThrowIfNullOrWhiteSpace( caller );
        ArgumentException.ThrowIfNullOrWhiteSpace( eventType );
        if ( !IsEnabled( LogEventLevel.Verbose ) ) { return; }

        using ( Disposables disposables = new() )
        {
            if ( properties is not null )
            {
                foreach ( (string key, string? value) in properties ) { disposables.Add( LogContext.PushProperty( key, value ) ); }
            }

            Debug( "[{ClassName}.{Caller}.{EventId}] {EventType}", typeof(TValue).Name, caller, eventType.GetHashCode(), eventType );
        }

        if ( Events.IsEnabled ) { Events.Add( DebugLogEvent.Create( typeof(TValue).Name, eventType, LogEventLevel.Debug, properties, caller ) ); }
    }


    public void TrackError<TValue>( TValue _, Exception exception, EventDetails? details, IEnumerable<FileData>? attachments, [CallerMemberName] string caller = BaseRecord.EMPTY )
    {
        if ( !Settings.EnableApi || !Settings.EnableCrashes ) { return; }

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
    public void TrackError<TValue>( TValue _, Exception exception, [CallerMemberName] string caller = BaseRecord.EMPTY )
    {
        if ( !Settings.EnableApi || !Settings.EnableCrashes ) { return; }

        ArgumentException.ThrowIfNullOrWhiteSpace( caller );
        if ( !IsEnabled( LogEventLevel.Error ) ) { return; }

        Error( exception, "[{ClassName}.{Caller}.{EventId}] {Message}", typeof(TValue).Name, caller, exception.Message.GetHashCode(), exception.Message );
        if ( Events.IsEnabled ) { Messages.Enqueue( MessageEvent.Create( exception ) ); }
    }


    public async ValueTask SaveFeedBackAppState( Dictionary<string, string> feedback, string key = "feedback" )
    {
        Dictionary<string, object?> result = new() { [key] = feedback };
        if ( Settings.IncludeAppStateOnError ) { EventDetails.AddAppState( in result, Options.AppName ); }

        await Paths.FeedbackFile.WriteAsync( result.ToPrettyJson() );
    }


    public EventDetails          AppState()                                                                        => Options.UpdateEventDetails( EventDetails.AppState( Options.AppName ) );
    public ValueTask<bool>       BufferScreenShot( CancellationToken   token                           = default ) => Options.BufferScreenShot( token );
    public ValueTask<LocalFile?> GetScreenShot( CancellationToken      token                           = default ) => Options.GetScreenShot( token );
    public ValueTask<LocalFile?> WriteScreenShot( CancellationToken    token                           = default ) => Options.WriteScreenShot( token );
    public ValueTask<LocalFile?> WriteScreenShot( ReadOnlyMemory<byte> memory, CancellationToken token = default ) => Options.WriteScreenShot( memory, token );


    public void SetDeviceID( Guid deviceID ) => Options.DeviceID = deviceID;
    public void SetDeviceID( long deviceID ) => Options.DeviceIDLong = deviceID;
    public void SetSettings( SeriloggerSettings settings )
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
