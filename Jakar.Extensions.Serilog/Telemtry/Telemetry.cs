// Jakar.Extensions :: Jakar.Extensions.Serilog
// 07/01/2024  15:07

using System.Diagnostics.Metrics;
using System.Reflection;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;



namespace Jakar.Extensions.Serilog;


public static class Telemetry
{
    public static readonly Version                                  DefaultVersion = new(1, 0, 0);
    public static readonly KeyValuePair<string, object?>[]          Pairs          = [];
    public static readonly ConcurrentDictionary<string, Instrument> Instruments    = [];
    public static readonly ConcurrentDictionary<string, Meter>      Meters         = [];
    public static readonly TelemetryTags                            Tags           = new();


    private static readonly Action<ILogger, string, Exception?> _logEventCallback = LoggerMessage.Define<string>( LogLevel.Trace, new EventId( -1329573148, nameof(LogEvent) ), "{EventName}", new LogDefineOptions { SkipEnabledCheck = true } );

    public static void LogEvent( this ILogger logger, [CallerMemberName] string eventName = BaseRecord.EMPTY )
    {
        if ( logger.IsEnabled( LogLevel.Trace ) ) { _logEventCallback( logger, eventName, null ); }
    }


    public static Activity       AddEvent( this Activity      activity, [CallerMemberName] string eventID                                 = BaseRecord.EMPTY ) => activity.AddEvent( null, eventID );
    public static Activity       AddEvent( this Activity      activity, ActivityTagsCollection?   tags, [CallerMemberName] string eventID = BaseRecord.EMPTY ) => activity.AddEvent( new ActivityEvent( eventID, DateTimeOffset.UtcNow, tags ) );
    public static ActivitySource CreateActivitySource( string name,     AppVersion                version )                                                                                => CreateActivitySource( name, version.ToString() );
    public static ActivitySource CreateActivitySource( string name,     string                    version )                                                                                => new(name, version);
    public static Meter          CreateMeter( string          name,     AppVersion                version, IEnumerable<KeyValuePair<string, object?>>? tags = null, object? scope = null ) => CreateMeter( name, version.ToString(), tags, scope );
    public static Meter          CreateMeter( string          name,     string?                   version, IEnumerable<KeyValuePair<string, object?>>? tags = null, object? scope = null ) => new(name, version, tags, scope);
    public static Meter          CreateMeter( string          name )      => new(name);
    public static Assembly       GetAssembly()                            => Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
    public static AppVersion     GetVersion( this Assembly     assembly ) => assembly.GetName().GetVersion();
    public static AppVersion     GetVersion( this AssemblyName assembly ) => assembly.Version ?? DefaultVersion;


    public static Meter GetOrAdd_Meter( [CallerMemberName] string meterName = BaseRecord.EMPTY ) => Meters.GetOrAdd( meterName, CreateMeter );


    public static Histogram<TValue> GetOrAdd_Histogram<TValue>( string unit, string description, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where TValue : struct
    {
        if ( Instruments.TryGetValue( description, out Instrument? value ) && value is Histogram<TValue> instrument ) { return instrument; }

        Instruments[description] = instrument = GetOrAdd_Meter( meterName ).CreateHistogram<TValue>( meterName, unit, description, tags ?? Pairs );
        return instrument;
    }


    public static ObservableGauge<TValue> GetOrAdd_Gauge<TValue>( string name, Func<Measurement<TValue>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where TValue : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableGauge<TValue> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAdd_Meter( meterName ).CreateObservableGauge( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableGauge<TValue> GetOrAdd_Gauge<TValue>( string name, Func<IEnumerable<Measurement<TValue>>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where TValue : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableGauge<TValue> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAdd_Meter( meterName ).CreateObservableGauge( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }


    public static Counter<TValue> GetOrAdd_Counter<TValue>( string name, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where TValue : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is Counter<TValue> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAdd_Meter( meterName ).CreateCounter<TValue>( name, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableCounter<TValue> GetOrAdd_Counter<TValue>( string name, Func<Measurement<TValue>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where TValue : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableCounter<TValue> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAdd_Meter( meterName ).CreateObservableCounter( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableCounter<TValue> GetOrAdd_Counter<TValue>( string name, Func<IEnumerable<Measurement<TValue>>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where TValue : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableCounter<TValue> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAdd_Meter( meterName ).CreateObservableCounter( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }


    public static UpDownCounter<TValue> GetOrAdd_UpDownCounter<TValue>( string name, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where TValue : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is UpDownCounter<TValue> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAdd_Meter( meterName ).CreateUpDownCounter<TValue>( name, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableUpDownCounter<TValue> GetOrAdd_UpDownCounter<TValue>( string name, Func<Measurement<TValue>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where TValue : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableUpDownCounter<TValue> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAdd_Meter( meterName ).CreateObservableUpDownCounter( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableUpDownCounter<TValue> GetOrAdd_UpDownCounter<TValue>( string name, Func<IEnumerable<Measurement<TValue>>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where TValue : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableUpDownCounter<TValue> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAdd_Meter( meterName ).CreateObservableUpDownCounter( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }


    public static void AddSessionID<TID>( this Activity activity, ISessionID<TID> record )
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable => activity.AddTag( Tags.SessionID, record.SessionID );


    public static void AddRoleID<TID>( this Activity activity, IUniqueID<TID> record )
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable => activity.AddTag( Tags.RoleID, record.ID );


    public static void AddGroupID<TID>( this Activity activity, IUniqueID<TID> record )
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable => activity.AddTag( Tags.GroupID, record.ID );


    public static void AddUserID( this              Activity activity, IUserID user )         => activity.AddTag( nameof(IUserID.UserID),      user.UserID );
    public static void AddGroup( this               Activity activity, string? value = null ) => activity.AddTag( Tags.AddGroup,               value );
    public static void AddGroup( this               Activity activity, object? value = null ) => activity.AddTag( Tags.AddGroup,               value );
    public static void AddGroupRights( this         Activity activity, string? value = null ) => activity.AddTag( Tags.AddGroupRights,         value );
    public static void AddGroupRights( this         Activity activity, object? value = null ) => activity.AddTag( Tags.AddGroupRights,         value );
    public static void AddRole( this                Activity activity, string? value = null ) => activity.AddTag( Tags.AddRole,                value );
    public static void AddRole( this                Activity activity, object? value = null ) => activity.AddTag( Tags.AddRole,                value );
    public static void AddRoleRights( this          Activity activity, string? value = null ) => activity.AddTag( Tags.AddRoleRights,          value );
    public static void AddRoleRights( this          Activity activity, object? value = null ) => activity.AddTag( Tags.AddRoleRights,          value );
    public static void AddUser( this                Activity activity, string? value = null ) => activity.AddTag( Tags.AddUser,                value );
    public static void AddUser( this                Activity activity, object? value = null ) => activity.AddTag( Tags.AddUser,                value );
    public static void AddUserAddress( this         Activity activity, string? value = null ) => activity.AddTag( Tags.AddUserAddress,         value );
    public static void AddUserAddress( this         Activity activity, object? value = null ) => activity.AddTag( Tags.AddUserAddress,         value );
    public static void AddUserLoginInfo( this       Activity activity, string? value = null ) => activity.AddTag( Tags.AddUserLoginInfo,       value );
    public static void AddUserLoginInfo( this       Activity activity, object? value = null ) => activity.AddTag( Tags.AddUserLoginInfo,       value );
    public static void AddUserRecoveryCode( this    Activity activity, string? value = null ) => activity.AddTag( Tags.AddUserRecoveryCode,    value );
    public static void AddUserRecoveryCode( this    Activity activity, object? value = null ) => activity.AddTag( Tags.AddUserRecoveryCode,    value );
    public static void AddUserRights( this          Activity activity, string? value = null ) => activity.AddTag( Tags.AddUserRights,          value );
    public static void AddUserRights( this          Activity activity, object? value = null ) => activity.AddTag( Tags.AddUserRights,          value );
    public static void AddUserSubscription( this    Activity activity, string? value = null ) => activity.AddTag( Tags.AddUserSubscription,    value );
    public static void AddUserSubscription( this    Activity activity, object? value = null ) => activity.AddTag( Tags.AddUserSubscription,    value );
    public static void AddUserToGroup( this         Activity activity, string? value = null ) => activity.AddTag( Tags.AddUserToGroup,         value );
    public static void AddUserToGroup( this         Activity activity, object? value = null ) => activity.AddTag( Tags.AddUserToGroup,         value );
    public static void AddUserToRole( this          Activity activity, string? value = null ) => activity.AddTag( Tags.AddUserToRole,          value );
    public static void AddUserToRole( this          Activity activity, object? value = null ) => activity.AddTag( Tags.AddUserToRole,          value );
    public static void ConnectDatabase( this        Activity activity, string? value = null ) => activity.AddTag( Tags.ConnectDatabase,        value );
    public static void ConnectDatabase( this        Activity activity, object? value = null ) => activity.AddTag( Tags.ConnectDatabase,        value );
    public static void LoginUser( this              Activity activity, string? value = null ) => activity.AddTag( Tags.LoginUser,              value );
    public static void LoginUser( this              Activity activity, object? value = null ) => activity.AddTag( Tags.LoginUser,              value );
    public static void RemoveGroup( this            Activity activity, string? value = null ) => activity.AddTag( Tags.RemoveGroup,            value );
    public static void RemoveGroup( this            Activity activity, object? value = null ) => activity.AddTag( Tags.RemoveGroup,            value );
    public static void RemoveGroupRights( this      Activity activity, string? value = null ) => activity.AddTag( Tags.RemoveGroupRights,      value );
    public static void RemoveGroupRights( this      Activity activity, object? value = null ) => activity.AddTag( Tags.RemoveGroupRights,      value );
    public static void RemoveRole( this             Activity activity, string? value = null ) => activity.AddTag( Tags.RemoveRole,             value );
    public static void RemoveRole( this             Activity activity, object? value = null ) => activity.AddTag( Tags.RemoveRole,             value );
    public static void RemoveRoleRights( this       Activity activity, string? value = null ) => activity.AddTag( Tags.RemoveRoleRights,       value );
    public static void RemoveRoleRights( this       Activity activity, object? value = null ) => activity.AddTag( Tags.RemoveRoleRights,       value );
    public static void RemoveUser( this             Activity activity, string? value = null ) => activity.AddTag( Tags.RemoveUser,             value );
    public static void RemoveUser( this             Activity activity, object? value = null ) => activity.AddTag( Tags.RemoveUser,             value );
    public static void RemoveUserAddress( this      Activity activity, string? value = null ) => activity.AddTag( Tags.RemoveUserAddress,      value );
    public static void RemoveUserAddress( this      Activity activity, object? value = null ) => activity.AddTag( Tags.RemoveUserAddress,      value );
    public static void RemoveUserFromGroup( this    Activity activity, string? value = null ) => activity.AddTag( Tags.RemoveUserFromGroup,    value );
    public static void RemoveUserFromGroup( this    Activity activity, object? value = null ) => activity.AddTag( Tags.RemoveUserFromGroup,    value );
    public static void RemoveUserFromRole( this     Activity activity, string? value = null ) => activity.AddTag( Tags.RemoveUserFromRole,     value );
    public static void RemoveUserFromRole( this     Activity activity, object? value = null ) => activity.AddTag( Tags.RemoveUserFromRole,     value );
    public static void RemoveUserLoginInfo( this    Activity activity, string? value = null ) => activity.AddTag( Tags.RemoveUserLoginInfo,    value );
    public static void RemoveUserLoginInfo( this    Activity activity, object? value = null ) => activity.AddTag( Tags.RemoveUserLoginInfo,    value );
    public static void RemoveUserRecoveryCode( this Activity activity, string? value = null ) => activity.AddTag( Tags.RemoveUserRecoveryCode, value );
    public static void RemoveUserRecoveryCode( this Activity activity, object? value = null ) => activity.AddTag( Tags.RemoveUserRecoveryCode, value );
    public static void RemoveUserRights( this       Activity activity, string? value = null ) => activity.AddTag( Tags.RemoveUserRights,       value );
    public static void RemoveUserRights( this       Activity activity, object? value = null ) => activity.AddTag( Tags.RemoveUserRights,       value );
    public static void RemoveUserSubscription( this Activity activity, string? value = null ) => activity.AddTag( Tags.RemoveUserSubscription, value );
    public static void RemoveUserSubscription( this Activity activity, object? value = null ) => activity.AddTag( Tags.RemoveUserSubscription, value );
    public static void UpdateGroup( this            Activity activity, string? value = null ) => activity.AddTag( Tags.UpdateGroup,            value );
    public static void UpdateGroup( this            Activity activity, object? value = null ) => activity.AddTag( Tags.UpdateGroup,            value );
    public static void UpdateRole( this             Activity activity, string? value = null ) => activity.AddTag( Tags.UpdateRole,             value );
    public static void UpdateRole( this             Activity activity, object? value = null ) => activity.AddTag( Tags.UpdateRole,             value );
    public static void UpdateUser( this             Activity activity, string? value = null ) => activity.AddTag( Tags.UpdateUser,             value );
    public static void UpdateUser( this             Activity activity, object? value = null ) => activity.AddTag( Tags.UpdateUser,             value );
    public static void UpdateUserAddress( this      Activity activity, string? value = null ) => activity.AddTag( Tags.UpdateUserAddress,      value );
    public static void UpdateUserAddress( this      Activity activity, object? value = null ) => activity.AddTag( Tags.UpdateUserAddress,      value );
    public static void UpdateUserLoginInfo( this    Activity activity, string? value = null ) => activity.AddTag( Tags.UpdateUserLoginInfo,    value );
    public static void UpdateUserLoginInfo( this    Activity activity, object? value = null ) => activity.AddTag( Tags.UpdateUserLoginInfo,    value );
    public static void UpdateUserSubscription( this Activity activity, string? value = null ) => activity.AddTag( Tags.UpdateUserSubscription, value );
    public static void UpdateUserSubscription( this Activity activity, object? value = null ) => activity.AddTag( Tags.UpdateUserSubscription, value );
    public static void VerifyLogin( this            Activity activity, string? value = null ) => activity.AddTag( Tags.VerifyLogin,            value );
    public static void VerifyLogin( this            Activity activity, object? value = null ) => activity.AddTag( Tags.VerifyLogin,            value );
}
