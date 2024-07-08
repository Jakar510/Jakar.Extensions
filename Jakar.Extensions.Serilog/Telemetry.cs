// Jakar.Extensions :: Jakar.Extensions.Serilog
// 07/01/2024  15:07

using System.Diagnostics.Metrics;
using System.Reflection;
using Microsoft.Extensions.Logging;
using ILogger = Microsoft.Extensions.Logging.ILogger;



namespace Jakar.Extensions.Serilog;


public static class Telemetry
{
    public static readonly  Version                                  DefaultVersion    = new(1, 0, 0);
    public static readonly  KeyValuePair<string, object?>[]          Pairs             = [];
    public static readonly  ConcurrentDictionary<string, Instrument> Instruments       = [];
    public static readonly  ConcurrentDictionary<string, Meter>      Meters            = [];
    private static readonly Action<ILogger, string, Exception?>      _logEventCallback = LoggerMessage.Define<string>( LogLevel.Information, new EventId( -1329573148, nameof(LogEvent) ), "{EventName}", new LogDefineOptions { SkipEnabledCheck = true } );


    public static TelemetryTags Tags { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = new();


    public static void LogEvent( this ILogger logger, [CallerMemberName] string eventName = BaseRecord.EMPTY )
    {
        if ( logger.IsEnabled( LogLevel.Information ) ) { _logEventCallback( logger, eventName, null ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Activity       AddEvent( this Activity activity, [CallerMemberName] string eventID                                 = BaseRecord.EMPTY ) => activity.AddEvent( null, eventID );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Activity       AddEvent( this Activity activity, ActivityTagsCollection?   tags, [CallerMemberName] string eventID = BaseRecord.EMPTY ) => activity.AddEvent( new ActivityEvent( eventID, DateTimeOffset.UtcNow, tags ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource()                                           => CreateSource( GetAssembly() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource( Assembly     assembly )                    => CreateSource( assembly.GetName() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource( AssemblyName assembly )                    => CreateSource( assembly.Name ?? GetAssembly().GetName().Name ?? nameof(Telemetry), assembly );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource( string       name )                        => CreateSource( name,                                                               GetAssembly() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource( string       name, Assembly     assembly ) => CreateSource( name,                                                               assembly.GetName() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource( string       name, AssemblyName assembly ) => CreateSource( name,                                                               assembly.GetVersion() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource( string       name, AppVersion   version )  => CreateSource( name,                                                               version.ToString() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource( string       name, string       version )  => new(name, version);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter()                                                                                                                         => CreateMeter( GetAssembly() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter( Assembly     assembly )                                                                                                  => CreateMeter( assembly.GetName() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter( AssemblyName assembly )                                                                                                  => CreateMeter( assembly.Name ?? GetAssembly().GetName().Name ?? nameof(Telemetry), assembly );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter( string       name )                                                                                                      => CreateMeter( name,                                                               GetAssembly() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter( string       name, Assembly     assembly )                                                                               => CreateMeter( name,                                                               assembly.GetName() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter( string       name, AssemblyName assembly )                                                                               => CreateMeter( name,                                                               assembly.GetVersion() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter( string       name, AppVersion   version, IEnumerable<KeyValuePair<string, object?>>? tags = null, object? scope = null ) => CreateMeter( name,                                                               version.ToString(), tags, scope );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter( string       name, string?      version, IEnumerable<KeyValuePair<string, object?>>? tags = null, object? scope = null ) => new(name, version, tags, scope);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Assembly       GetAssembly()                            => Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static AppVersion     GetVersion( this Assembly     assembly ) => assembly.GetName().GetVersion();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static AppVersion     GetVersion( this AssemblyName assembly ) => assembly.Version ?? DefaultVersion;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter GetOrAddMeter( [CallerMemberName] string meterName = BaseRecord.EMPTY ) => Meters.GetOrAdd( meterName, CreateMeter );


    public static Histogram<T> GetOrAdd<T>( string unit, string description, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( description, out Instrument? value ) && value is Histogram<T> instrument ) { return instrument; }

        Instruments[description] = instrument = GetOrAddMeter( meterName ).CreateHistogram<T>( meterName, unit, description, tags ?? Pairs );
        return instrument;
    }


    public static ObservableGauge<T> GetOrAddGauge<T>( string name, Func<Measurement<T>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableGauge<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateObservableGauge( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableGauge<T> GetOrAddGauge<T>( string name, Func<IEnumerable<Measurement<T>>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableGauge<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateObservableGauge( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }


    public static Counter<T> GetOrAddCounter<T>( string name, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is Counter<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateCounter<T>( name, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableCounter<T> GetOrAddCounter<T>( string name, Func<Measurement<T>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableCounter<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateObservableCounter( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableCounter<T> GetOrAddCounter<T>( string name, Func<IEnumerable<Measurement<T>>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableCounter<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateObservableCounter( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }


    public static UpDownCounter<T> GetOrAddUpDownCounter<T>( string name, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is UpDownCounter<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateUpDownCounter<T>( name, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableUpDownCounter<T> GetOrAddUpDownCounter<T>( string name, Func<Measurement<T>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableUpDownCounter<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateObservableUpDownCounter( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableUpDownCounter<T> GetOrAddUpDownCounter<T>( string name, Func<IEnumerable<Measurement<T>>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = BaseRecord.EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableUpDownCounter<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateObservableUpDownCounter( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void AddSessionID<TID>( this Activity activity, ISessionID<TID> record )
#if NET8_0_OR_GREATER
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
        => activity.AddTag( Tags.SessionID, record.SessionID );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void AddRoleID<TID>( this Activity activity, IUniqueID<TID> record )
#if NET8_0_OR_GREATER
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
        => activity.AddTag( Tags.RoleID, record.ID );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void AddGroupID<TID>( this Activity activity, IUniqueID<TID> record )
#if NET8_0_OR_GREATER
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
        => activity.AddTag( Tags.GroupID, record.ID );


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserID( this              Activity activity, IUserID user )            => activity.AddTag( nameof(IUserID.UserID),      user.UserID );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddGroup( this               Activity activity, string? value = default ) => activity.AddTag( Tags.AddGroup,               value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddGroup( this               Activity activity, object? value = default ) => activity.AddTag( Tags.AddGroup,               value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddGroupRights( this         Activity activity, string? value = default ) => activity.AddTag( Tags.AddGroupRights,         value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddGroupRights( this         Activity activity, object? value = default ) => activity.AddTag( Tags.AddGroupRights,         value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddRole( this                Activity activity, string? value = default ) => activity.AddTag( Tags.AddRole,                value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddRole( this                Activity activity, object? value = default ) => activity.AddTag( Tags.AddRole,                value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddRoleRights( this          Activity activity, string? value = default ) => activity.AddTag( Tags.AddRoleRights,          value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddRoleRights( this          Activity activity, object? value = default ) => activity.AddTag( Tags.AddRoleRights,          value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUser( this                Activity activity, string? value = default ) => activity.AddTag( Tags.AddUser,                value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUser( this                Activity activity, object? value = default ) => activity.AddTag( Tags.AddUser,                value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserAddress( this         Activity activity, string? value = default ) => activity.AddTag( Tags.AddUserAddress,         value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserAddress( this         Activity activity, object? value = default ) => activity.AddTag( Tags.AddUserAddress,         value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserLoginInfo( this       Activity activity, string? value = default ) => activity.AddTag( Tags.AddUserLoginInfo,       value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserLoginInfo( this       Activity activity, object? value = default ) => activity.AddTag( Tags.AddUserLoginInfo,       value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserRecoveryCode( this    Activity activity, string? value = default ) => activity.AddTag( Tags.AddUserRecoveryCode,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserRecoveryCode( this    Activity activity, object? value = default ) => activity.AddTag( Tags.AddUserRecoveryCode,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserRights( this          Activity activity, string? value = default ) => activity.AddTag( Tags.AddUserRights,          value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserRights( this          Activity activity, object? value = default ) => activity.AddTag( Tags.AddUserRights,          value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserSubscription( this    Activity activity, string? value = default ) => activity.AddTag( Tags.AddUserSubscription,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserSubscription( this    Activity activity, object? value = default ) => activity.AddTag( Tags.AddUserSubscription,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserToGroup( this         Activity activity, string? value = default ) => activity.AddTag( Tags.AddUserToGroup,         value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserToGroup( this         Activity activity, object? value = default ) => activity.AddTag( Tags.AddUserToGroup,         value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserToRole( this          Activity activity, string? value = default ) => activity.AddTag( Tags.AddUserToRole,          value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserToRole( this          Activity activity, object? value = default ) => activity.AddTag( Tags.AddUserToRole,          value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void ConnectDatabase( this        Activity activity, string? value = default ) => activity.AddTag( Tags.ConnectDatabase,        value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void ConnectDatabase( this        Activity activity, object? value = default ) => activity.AddTag( Tags.ConnectDatabase,        value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void LoginUser( this              Activity activity, string? value = default ) => activity.AddTag( Tags.LoginUser,              value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void LoginUser( this              Activity activity, object? value = default ) => activity.AddTag( Tags.LoginUser,              value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveGroup( this            Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveGroup,            value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveGroup( this            Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveGroup,            value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveGroupRights( this      Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveGroupRights,      value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveGroupRights( this      Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveGroupRights,      value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveRole( this             Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveRole,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveRole( this             Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveRole,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveRoleRights( this       Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveRoleRights,       value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveRoleRights( this       Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveRoleRights,       value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUser( this             Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUser,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUser( this             Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUser,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserAddress( this      Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUserAddress,      value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserAddress( this      Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUserAddress,      value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserFromGroup( this    Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUserFromGroup,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserFromGroup( this    Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUserFromGroup,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserFromRole( this     Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUserFromRole,     value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserFromRole( this     Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUserFromRole,     value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserLoginInfo( this    Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUserLoginInfo,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserLoginInfo( this    Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUserLoginInfo,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserRecoveryCode( this Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUserRecoveryCode, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserRecoveryCode( this Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUserRecoveryCode, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserRights( this       Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUserRights,       value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserRights( this       Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUserRights,       value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserSubscription( this Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUserSubscription, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserSubscription( this Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUserSubscription, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateGroup( this            Activity activity, string? value = default ) => activity.AddTag( Tags.UpdateGroup,            value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateGroup( this            Activity activity, object? value = default ) => activity.AddTag( Tags.UpdateGroup,            value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateRole( this             Activity activity, string? value = default ) => activity.AddTag( Tags.UpdateRole,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateRole( this             Activity activity, object? value = default ) => activity.AddTag( Tags.UpdateRole,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUser( this             Activity activity, string? value = default ) => activity.AddTag( Tags.UpdateUser,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUser( this             Activity activity, object? value = default ) => activity.AddTag( Tags.UpdateUser,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUserAddress( this      Activity activity, string? value = default ) => activity.AddTag( Tags.UpdateUserAddress,      value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUserAddress( this      Activity activity, object? value = default ) => activity.AddTag( Tags.UpdateUserAddress,      value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUserLoginInfo( this    Activity activity, string? value = default ) => activity.AddTag( Tags.UpdateUserLoginInfo,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUserLoginInfo( this    Activity activity, object? value = default ) => activity.AddTag( Tags.UpdateUserLoginInfo,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUserSubscription( this Activity activity, string? value = default ) => activity.AddTag( Tags.UpdateUserSubscription, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUserSubscription( this Activity activity, object? value = default ) => activity.AddTag( Tags.UpdateUserSubscription, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void VerifyLogin( this            Activity activity, string? value = default ) => activity.AddTag( Tags.VerifyLogin,            value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void VerifyLogin( this            Activity activity, object? value = default ) => activity.AddTag( Tags.VerifyLogin,            value );
}
