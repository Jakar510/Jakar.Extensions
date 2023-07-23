using OneOf;



namespace Jakar.AppLogger.Portal.Data;


[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" )]
public sealed class LoggerDB : Database.Database
{
    public DbTable<AppRecord>                           Apps          { get; }
    public DbTable<AttachmentRecord>                    Attachments   { get; }
    public DbTable<DeviceRecord>                        Devices       { get; }
    public DbTable<LogRecord>                           Logs          { get; }
    public ConcurrentObservableCollection<Notification> Notifications { get; } = new(Notification.Sorter);
    public DbTable<ScopeRecord>                         Scopes        { get; }
    public DbTable<SessionRecord>                       Sessions      { get; }


    static LoggerDB()
    {
        EnumSqlHandler<PlatformID>.Register();
        EnumSqlHandler<Architecture>.Register();
        EnumSqlHandler<LogLevel>.Register();
    }
    public LoggerDB( IConfiguration configuration, IOptions<DbOptions> options ) : base( configuration, options )
    {
        Logs        = Create<LogRecord>();
        Attachments = Create<AttachmentRecord>();
        Devices     = Create<DeviceRecord>();
        Apps        = Create<AppRecord>();
        Sessions    = Create<SessionRecord>();
        Scopes      = Create<ScopeRecord>();
    }


    protected override DbConnection CreateConnection() => new NpgsqlConnection( ConnectionString );


    public event EventHandler<Notification>? NotificationReceived;
    public async ValueTask<OneOf<Guid, Error>> StartSession( DbConnection connection, DbTransaction transaction, ControllerBase controller, StartSession session, CancellationToken token )
    {
        if ( string.IsNullOrWhiteSpace( session.AppLoggerSecret ) ) { return new Error( Status.BadRequest, $"{nameof(session.AppLoggerSecret)} cannot be null, empty or white space." ); }

        UserRecord? caller = await Verify( connection, transaction, session.AppLoggerSecret, token );
        if ( caller is null ) { return new Error( Status.Unauthorized ); }

        DeviceRecord? device = await AddOrUpdate_Device( connection, transaction, controller, session.Device, caller, token );
        if ( device is null ) { return new Error( Status.BadRequest, controller.ModelState ); }

        return Guid.Empty;
    }


    public ValueTask<OneOf<Guid, Error>> StartSession( ControllerBase controller, StartSession session, CancellationToken token ) => this.TryCall( StartSession, controller, session, token );
    public async ValueTask<OneOf<bool, Error>> EndSession( DbConnection connection, DbTransaction transaction, ControllerBase controller, Guid sessionID, CancellationToken token )
    {
        if ( !sessionID.IsValidID() ) { return new Error( Status.BadRequest, $"{nameof(sessionID)} cannot be empty." ); }

        SessionRecord? session = await Sessions.Get( connection, transaction, true, SessionRecord.GetDynamicParameters( sessionID ), token );
        if ( session is null || !session.IsActive ) { return new Error( Status.NotFound, session ); }


        await Sessions.Update( connection,
                               transaction,
                               session with
                               {
                                   IsActive = false,
                               },
                               token );

        return true;
    }


    public ValueTask<OneOf<bool, Error>> EndSession( ControllerBase controller, Guid sessionID, CancellationToken token ) => this.TryCall( EndSession, controller, sessionID, token );
    public async ValueTask<OneOf<bool, Error>> SendLog( DbConnection connection, DbTransaction transaction, ControllerBase controller, IEnumerable<AppLog> logs, CancellationToken token )
    {
        foreach ( AppLog log in logs )
        {
            try
            {
                OneOf<bool, Error> result = await SendLog( connection, transaction, controller, log, token );
                if ( result.IsT1 ) { return result.AsT1; }
            }
            catch ( Exception e )
            {
                Console.WriteLine( e );
                throw;
            }
        }

        return true;
    }
    public async ValueTask<OneOf<bool, Error>> SendLog( DbConnection connection, DbTransaction transaction, ControllerBase controller, AppLog log, CancellationToken token )
    {
        if ( !log.SessionID.IsValidID() )
        {
            controller.AddError( nameof(AppLog.SessionID), $"{nameof(AppLog.SessionID)} is null or empty" );
            return new Error( Status.BadRequest, controller.ModelState );
        }


        SessionRecord? session = await Sessions.Get( connection, transaction, true, SessionRecord.GetDynamicParameters( log.SessionID ), token );
        if ( session is null || !session.IsActive ) { return new Error( Status.NotFound, log.SessionID ); }

        UserRecord? caller = await session.GetUserWhoCreated( connection, transaction, this, token );
        if ( caller is null ) { return new Error( Status.Unauthorized ); }

        var record = new LogRecord( log, session, caller );
        record = await Logs.Insert( connection, transaction, record, token );


        foreach ( Attachment attachment in log.Attachments )
        {
            AttachmentRecord? attachmentRecord = await Attachments.Get( connection, transaction, true, AttachmentRecord.GetDynamicParameters( attachment ), token );

            if ( attachmentRecord is null )
            {
                attachmentRecord = new AttachmentRecord( attachment, record, caller );
                await Attachments.Insert( connection, transaction, attachmentRecord, token );
            }
            else { await Attachments.Update( connection, transaction, attachmentRecord.Update( attachment ), token ); }
        }

        return true;
    }


    public ValueTask<OneOf<bool, Error>> SendLog( ControllerBase controller, IEnumerable<AppLog> logs, CancellationToken token ) => this.TryCall( SendLog, controller, logs, token );


    public async ValueTask<DeviceRecord?> AddOrUpdate_Device( DbConnection connection, DbTransaction transaction, ControllerBase controller, DeviceDescriptor device, UserRecord caller, CancellationToken token )
    {
        DeviceRecord? record = default;

        foreach ( DeviceRecord deviceRecord in await Devices.Where( connection, transaction, true, DeviceRecord.GetDynamicParameters( device, caller ), token ) )
        {
            if ( record is null ) { record = deviceRecord; }
            else
            {
                // throw new InvalidOperationException($"Multiple records exist for '{device.DeviceID}'");
                controller.AddError( "ERROR", $"Multiple records exist for '{device.DeviceID}'" );
                return default;
            }
        }

        if ( record is null )
        {
            record = new DeviceRecord( device, caller );
            record = await Devices.Insert( connection, transaction, record, token );
        }
        else
        {
            record = record.Update( device, caller );
            await Devices.Update( connection, transaction, record, token );
        }

        return record;
    }


    public ValueTask<IEnumerable<LogRecord>> GetLogs( CancellationToken token = default ) => Logs.Where( nameof(LogRecord.IsActive), true, token );


    public async ValueTask<UserRecord?> Verify( DbConnection connection, DbTransaction transaction, string appLoggerSecret, CancellationToken token )
    {
        AppRecord? app = await Apps.Get( connection, transaction, true, AppRecord.GetDynamicParameters( appLoggerSecret ), token );

        return app is null || app.IsNotActive
                   ? default
                   : await app.GetUserWhoCreated( connection, transaction, this, token );
    }


    public void SendNotification( Notification notification )
    {
        Notifications.Add( notification );
        NotificationReceived?.Invoke( this, notification );
        notification.WriteToDebug();
    }
}
