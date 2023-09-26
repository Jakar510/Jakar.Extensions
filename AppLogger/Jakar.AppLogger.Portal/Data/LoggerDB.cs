﻿using System.Collections.Immutable;
using Microsoft.AspNetCore.Http.HttpResults;
using Debug = Jakar.AppLogger.Common.Debug;



namespace Jakar.AppLogger.Portal.Data;


[ SuppressMessage( "ReSharper", "SuggestBaseTypeForParameter" ) ]
public sealed class LoggerDB : Database.Database
{
    private readonly IDataProtectorProvider                 _dataProtectorProvider;
    public           DbTable<AppRecord>                     Apps           { get; }
    public           DbTable<LoggerAttachmentRecord>        Attachments    { get; }
    public           DbTable<LoggerAttachmentMappingRecord> LogAttachments { get; }
    public           DbTable<DeviceRecord>                  Devices        { get; }
    public           DbTable<AppDeviceRecord>               AppDevices     { get; }
    public           DbTable<LogRecord>                     Logs           { get; }
    public           DbTable<LogScopeRecord>                LogScopes      { get; }
    public           DbTable<ScopeRecord>                   Scopes         { get; }
    public           DbTable<SessionRecord>                 Sessions       { get; }


    public event EventHandler<Notification>? NotificationReceived;


    static LoggerDB()
    {
        EnumSqlHandler<PlatformID>.Register();
        EnumSqlHandler<Architecture>.Register();
        EnumSqlHandler<LogLevel>.Register();
        RecordID<AppRecord>.DapperTypeHandler.Register();
        RecordID<LoggerAttachmentRecord>.DapperTypeHandler.Register();
        RecordID<DeviceRecord>.DapperTypeHandler.Register();
        RecordID<LogRecord>.DapperTypeHandler.Register();
        RecordID<ScopeRecord>.DapperTypeHandler.Register();
        RecordID<SessionRecord>.DapperTypeHandler.Register();
    }
    public LoggerDB( IConfiguration configuration, IDataProtectorProvider dataProtectorProvider, IOptions<DbOptions> options ) : base( configuration, options )
    {
        _dataProtectorProvider = dataProtectorProvider;
        Logs                   = Create<LogRecord>();
        LogScopes              = Create<LogScopeRecord>();
        Attachments            = Create<LoggerAttachmentRecord>();
        LogAttachments         = Create<LoggerAttachmentMappingRecord>();
        Devices                = Create<DeviceRecord>();
        Apps                   = Create<AppRecord>();
        AppDevices             = Create<AppDeviceRecord>();
        Sessions               = Create<SessionRecord>();
        Scopes                 = Create<ScopeRecord>();
    }


    protected override DbConnection CreateConnection() => new NpgsqlConnection( ConnectionString );


    public ValueTask<OneOf<StartSessionReply, Error>> StartSession( StartSession session, CancellationToken token ) => this.TryCall( StartSession, session, token );
    public async ValueTask<OneOf<StartSessionReply, Error>> StartSession( DbConnection connection, DbTransaction transaction, StartSession start, CancellationToken token )
    {
        if ( string.IsNullOrWhiteSpace( start.AppLoggerSecret ) ) { return new Error( Status.BadRequest, $"{nameof(start.AppLoggerSecret)} cannot be null, empty or white space." ); }

        OneOf<AppLoggerSecret, Error> check = await AppLoggerSecret.ParseAsync( _dataProtectorProvider, start.AppLoggerSecret );
        if ( check.IsT1 ) { return check.AsT1; }

        AppLoggerSecret secret = check.AsT0;

        /*
        Guid            appID     = secret.AppID;
        Guid            createdBy = secret.UserID;
        string          deviceID  = start.Device.DeviceID;

        var parameters = new DynamicParameters( start.Device );
        parameters.Add( nameof(appID),     appID );
        parameters.Add( nameof(deviceID),  deviceID );
        parameters.Add( nameof(createdBy), createdBy );


        string sql = @$"
DECLARE {nameof(deviceID)} unique_id = NULL
IF NOT EXISTS( SELECT * FROM {Devices.TableName} WHERE {nameof(deviceID)} = @{nameof(deviceID)} and {nameof(createdBy)} = @{nameof(createdBy)}  )
BEGIN
    SET NOCOUNT ON INSERT INTO {Devices.SchemaTableName} ({string.Join( ',', Devices.ColumnNames )}) values ({string.Join( ',', Devices.VariableNames )});
END;


set {nameof(deviceID)} = ( SELECT {nameof(DeviceRecord.ID)} FROM {Devices.TableName} WHERE {nameof(deviceID)} = @{nameof(deviceID)} and {nameof(createdBy)} = @{nameof(createdBy)} );

IF {nameof(deviceID)} IS NOT NULL
BEGIN
    SELECT {nameof(SessionRecord.ID)} as {nameof(StartSessionReply.SessionID)}, app.{nameof(AppRecord.ID)} as {nameof(StartSessionReply.AppID)}, {nameof(DeviceRecord.ID)} as {nameof(StartSessionReply.DeviceID)} FROM {Sessions.TableName}
    INNER JOIN {Apps.TableName} app WHERE {nameof(AppRecord.ID)} = @{appID}
    INNER JOIN {Devices.TableName} device WHERE {nameof(DeviceRecord.ID)} = {nameof(deviceID)}
END";
*/


        UserRecord? caller = await Users.Get( connection, transaction, secret.UserID, token );
        if ( caller is null ) { return new Error( Status.Unauthorized ); }

        AppRecord? app = await Apps.Get( connection, transaction, secret.AppID, token );
        if ( app is null || !app.IsActive ) { return new Error( Status.NotFound, "App not found" ); }

        OneOf<DeviceRecord, Error> device = await AddOrUpdate_Device( connection, transaction, start.Device, caller, token );
        if ( device.IsT1 ) { return device.AsT1; }

        var session = new SessionRecord( start, app, device.AsT0, caller );
        session = await Sessions.Insert( connection, transaction, session, token );

        return new StartSessionReply( session.ID.Value, app.ID.Value, device.AsT0.ID.Value );
    }


    public ValueTask<OneOf<bool, Error>> EndSession( Guid sessionID, CancellationToken token ) => this.TryCall( EndSession, sessionID, token );
    public async ValueTask<OneOf<bool, Error>> EndSession( DbConnection connection, DbTransaction transaction, Guid sessionID, CancellationToken token )
    {
        if ( !sessionID.IsValidID() ) { return new Error( Status.BadRequest, $"{nameof(sessionID)} cannot be empty." ); }

        SessionRecord? session = await Sessions.Get( connection, transaction, true, SessionRecord.GetDynamicParameters( sessionID ), token );
        if ( session is null || !session.IsActive ) { return new Error( Status.NotFound, session ); }

        session.IsActive = true;
        await Sessions.Update( connection, transaction, session, token );
        return true;
    }


    public async ValueTask<OneOf<bool, Error>> SendLog( DbConnection connection, DbTransaction transaction, IEnumerable<AppLog> logs, CancellationToken token )
    {
        foreach ( AppLog log in logs )
        {
            OneOf<LogRecord, Error> result = await SendLog( connection, transaction, log, token );
            if ( result.IsT1 ) { return result.AsT1; }
        }

        return true;
    }
    public ValueTask<OneOf<bool, Error>> SendLog( IEnumerable<AppLog> logs, CancellationToken token ) => this.TryCall( SendLog, logs, token );
    public async ValueTask<OneOf<LogRecord, Error>> SendLog( DbConnection connection, DbTransaction transaction, AppLog log, CancellationToken token )
    {
        SessionRecord? session = await Sessions.Get( connection, transaction, true, SessionRecord.GetDynamicParameters( log.Session.SessionID ), token );
        if ( session is null || !session.IsActive ) { return new Error( Status.NotFound, log.Session.SessionID.ToString() ); }

        UserRecord? caller = await session.GetUserWhoCreated( connection, transaction, this, token );
        if ( caller is null ) { return new Error( Status.Unauthorized ); }

        AppRecord? app = await Apps.Get( connection, transaction, log.Session.AppID, token );
        if ( app is null || !app.IsActive ) { return new Error( Status.NotFound, log.Session.AppID.ToString() ); }

        OneOf<DeviceRecord, Error> device = await AddOrUpdate_Device( connection, transaction, app, log.Device, caller, token );
        if ( device.IsT1 ) { return device.AsT1; }

        var record = new LogRecord( log, session, caller );
        record = await Logs.Insert( connection, transaction, record, token );

        IEnumerable<ScopeRecord> scopes = ScopeRecord.Create( log, app, device.AsT0, session, caller );
        await LogScopeRecord.TryAdd( connection, transaction, LogScopes, record, scopes, caller, token );

        IEnumerable<LoggerAttachmentRecord> attachments = LoggerAttachmentRecord.Create( log, app, device.AsT0, record, session, caller );
        await LoggerAttachmentMappingRecord.TryAdd( connection, transaction, LogAttachments, record, attachments, caller, token );

        return record;
    }
    public async ValueTask<OneOf<DeviceRecord, Error>> AddOrUpdate_Device( DbConnection connection, DbTransaction transaction, AppRecord app, DeviceDescriptor device, UserRecord caller, CancellationToken token )
    {
        OneOf<DeviceRecord, Error> check = await AddOrUpdate_Device( connection, transaction, device, caller, token );
        if ( check.IsT1 ) { return check.AsT1; }

        DeviceRecord record = check.AsT0;
        await AppDeviceRecord.TryAdd( connection, transaction, AppDevices, app, record, token );
        return record;
    }
    public async ValueTask<OneOf<DeviceRecord, Error>> AddOrUpdate_Device( DbConnection connection, DbTransaction transaction, DeviceDescriptor device, UserRecord caller, CancellationToken token )
    {
        DeviceRecord? record = default;

        foreach ( DeviceRecord deviceRecord in await Devices.Where( connection, transaction, true, DeviceRecord.GetDynamicParameters( device, caller ), token ) )
        {
            if ( record is null ) { record = deviceRecord; }
            else
            {
                // throw new InvalidOperationException($"Multiple records exist for '{device.DeviceID}'");
                return new Error( Status.Conflict, $"Multiple records exist for '{device.DeviceID}'" );
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


    public ValueTask<IEnumerable<LogRecord>> GetLogs( CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(LogRecord.IsActive), true );
        return Logs.Where( true, parameters, token );
    }
    public ValueTask<IEnumerable<LogRecord>> GetLogs( AppRecord app, CancellationToken token = default )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(LogRecord.IsActive), true );
        parameters.Add( nameof(LogRecord.AppID),    app.ID );
        return Logs.Where( true, parameters, token );
    }


    public async ValueTask<UserRecord?> Verify( DbConnection connection, DbTransaction transaction, string appLoggerSecret, CancellationToken token )
    {
        AppRecord? app = await Apps.Get( connection, transaction, true, AppRecord.GetDynamicParameters( appLoggerSecret ), token );

        return app is null || app.IsNotActive
                   ? default
                   : await app.GetUserWhoCreated( connection, transaction, this, token );
    }


    public void SendNotification( Notification notification )
    {
        NotificationReceived?.Invoke( this, notification );
        notification.WriteToDebug();
    }
}
