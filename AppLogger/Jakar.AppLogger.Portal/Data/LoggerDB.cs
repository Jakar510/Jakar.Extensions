namespace Jakar.AppLogger.Portal.Data;


[SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" )]
public sealed class LoggerDB : Database.Database
{
    public override    AppVersion                                   Version       { get; } = new(1, 0, 0);
    public             ConcurrentObservableCollection<Notification> Notifications { get; } = new();
    public             DbTable<AppRecord>                       Apps          { get; }
    public             DbTable<AttachmentRecord>                Attachments   { get; }
    public             DbTable<DeviceRecord>                    Devices       { get; }
    public             DbTable<LogRecord>                       Logs          { get; }
    public             DbTable<ScopeRecord>                     Scopes        { get; }
    public             DbTable<SessionRecord>                   Sessions      { get; }
    protected override PasswordRequirements                         _Requirements { get; } = new();


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
    public async ValueTask<ActionResult<Guid>> StartSession( DbConnection connection, DbTransaction transaction, ControllerBase controller, StartSession session, CancellationToken token )
    {
        if ( string.IsNullOrWhiteSpace( session.AppLoggerSecret ) ) { return controller.BadRequest( $"{nameof(session.AppLoggerSecret)} cannot be null, empty or white space." ); }

        UserRecord? caller = await Verify( connection, transaction, session.AppLoggerSecret, token );
        if ( caller is null ) { return controller.Unauthorized(); }

        DeviceRecord? device = await AddOrUpdate_Device( connection, transaction, controller, session.Device, caller, token );
        if ( device is null ) { return controller.BadRequest( controller.ModelState ); }

        return Guid.Empty;
    }


    public ValueTask<ActionResult<Guid>> StartSession( ControllerBase controller, StartSession session, CancellationToken token ) => this.TryCall( StartSession, controller, session, token );
    public async ValueTask<ActionResult> EndSession( DbConnection connection, DbTransaction transaction, ControllerBase controller, Guid sessionID, CancellationToken token )
    {
        if ( !sessionID.IsValidID() ) { return controller.BadRequest( $"{nameof(sessionID)} cannot be empty." ); }

        SessionRecord? session = await Sessions.Get( connection, transaction, true, SessionRecord.GetDynamicParameters( sessionID ), token );
        if ( session is null || !session.IsActive ) { return controller.NotFound( session ); }


        await Sessions.Update( connection,
                               transaction,
                               session with
                               {
                                   IsActive = false,
                               },
                               token );

        return controller.Ok();
    }


    public ValueTask<ActionResult> EndSession( ControllerBase controller, Guid sessionID, CancellationToken token ) => this.TryCall( EndSession, controller, sessionID, token );
    public async ValueTask<ActionResult> Log( DbConnection connection, DbTransaction transaction, ControllerBase controller, Log log, CancellationToken token )
    {
        if ( !log.SessionID.IsValidID() )
        {
            controller.AddError( nameof(log.SessionID), $"{nameof(log.SessionID)} is null or empty" );
            return controller.BadRequest( controller.ModelState );
        }


        SessionRecord? session = await Sessions.Get( connection, transaction, true, SessionRecord.GetDynamicParameters( log.SessionID ), token );
        if ( session is null || !session.IsActive ) { return controller.NotFound( log.SessionID ); }

        UserRecord? caller = await session.GetUserWhoCreated( connection, transaction, this, token );
        if ( caller is null ) { return controller.Unauthorized(); }

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

        return controller.Ok();
    }


    public ValueTask<ActionResult> Log( ControllerBase controller, Log log, CancellationToken token ) => this.TryCall( Log, controller, log, token );


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


    public ValueTask<LogRecord[]> GetLogs( CancellationToken token = default ) => Logs.Where( nameof(LogRecord.IsActive), true, token );


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
