// Jakar.Extensions :: Jakar.Database
// 12/02/2023  12:12 PM

using System.Security.Cryptography.X509Certificates;
using OpenTelemetry;
using OpenTelemetry.Resources;



namespace Jakar.Database;


public static class Telemetry
{
    public const           string                                   METER_NAME     = "Jakar.Database";
    public static readonly Version                                  DefaultVersion = new(1, 0, 0);
    public static readonly KeyValuePair<string, object?>[]          Pairs          = [];
    public static readonly Meter                                    DbMeter        = CreateMeter( METER_NAME );
    public static readonly ActivitySource                           DbSource       = CreateSource( METER_NAME );
    public static readonly ConcurrentDictionary<string, Instrument> Instruments    = [];
    public static readonly ConcurrentDictionary<string, Meter>      Meters         = [];


    public static Meter          Meter  { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = DbMeter;
    public static ActivitySource Source { [MethodImpl( MethodImplOptions.AggressiveInlining )] get; set; } = DbSource;


    public static void ConfigureExporter( this OtlpExporterOptions exporter, Uri endpoint, ExportProcessorType type, OtlpExportProtocol protocol, string? headers = null, int timeout = 10000 )
    {
        exporter.Endpoint            = endpoint;
        exporter.ExportProcessorType = type;
        exporter.Protocol            = protocol;
        exporter.TimeoutMilliseconds = timeout;
        exporter.Headers             = headers;
    }
    public static void ConfigureExporter( this OtlpExporterOptions exporter, BatchExportProcessorOptions<Activity> processor ) => exporter.BatchExportProcessorOptions = processor;
    public static void ConfigureExporter( this OtlpExporterOptions exporter, Func<HttpClient>                      factory )   => exporter.HttpClientFactory = factory;


    public static WebApplication UseTelemetry( this WebApplication application, string path = "/metrics" )
    {
        application.UseOpenTelemetryPrometheusScrapingEndpoint( path );
        return application;
    }
    public static WebApplication UseTelemetry( this WebApplication application, Func<HttpContext, bool> predicate )
    {
        application.UseOpenTelemetryPrometheusScrapingEndpoint( predicate );
        return application;
    }
    public static WebApplication UseTelemetry( this WebApplication application, MeterProvider meterProvider, Func<HttpContext, bool> predicate, Action<IApplicationBuilder> configureBranchedPipeline, string optionsName, string path = "/metrics" )
    {
        application.UseOpenTelemetryPrometheusScrapingEndpoint( meterProvider, predicate, path, configureBranchedPipeline, optionsName );
        return application;
    }


    public static WebApplicationBuilder AddTelemetry<TApp>( this WebApplicationBuilder builder, OtlpExportProtocol protocol, Uri endpoint )
        where TApp : IAppName
    {
        bool isDevelopment = builder.Environment.IsDevelopment();

        builder.Services.AddOpenTelemetry()
               .ConfigureResource( static resource => resource.AddService( TApp.AppName, null, TApp.AppVersion.ToString() ) )
               .WithMetrics( metrics =>
                             {
                                 MeterProviderBuilder provider = metrics.AddMeter( TApp.AppName ).AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddMeter( METER_NAME );
                                 if ( isDevelopment ) { provider.AddConsoleExporter(); }
                             } )
               .WithTracing( tracing =>
                             {
                                 TracerProviderBuilder provider = tracing.AddSource( TApp.AppName ).AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddSource( METER_NAME );
                                 if ( isDevelopment ) { provider.AddConsoleExporter(); }
                             } )
               .UseOtlpExporter( protocol, endpoint );


        builder.Logging.AddOpenTelemetry( options =>
                                          {
                                              OpenTelemetryLoggerOptions provider = options.SetResourceBuilder( ResourceBuilder.CreateDefault().AddService( TApp.AppName, null, TApp.AppVersion.ToString() ) );
                                              if ( isDevelopment ) { provider.AddConsoleExporter(); }
                                          } );

        return builder;
    }


    public static IMetricServer Server( int port, string url = "/metrics", CollectorRegistry? registry = null, X509Certificate2? certificate = null )
    {
        KestrelMetricServer server = new(port, url, registry, certificate);
        return server.Start();
    }
    public static IMetricServer Server( KestrelMetricServerOptions options )
    {
        KestrelMetricServer server = new(options);
        return server.Start();
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource()                                           => CreateSource( GetAssembly() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource( Assembly     assembly )                    => CreateSource( assembly.GetName() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource( AssemblyName assembly )                    => CreateSource( assembly.Name ?? nameof(Database), assembly );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource( string       name )                        => CreateSource( name,                              GetAssembly() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource( string       name, Assembly     assembly ) => CreateSource( name,                              assembly.GetName() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource( string       name, AssemblyName assembly ) => CreateSource( name,                              assembly.GetVersion() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource( string       name, AppVersion   version )  => CreateSource( name,                              version.ToString() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static ActivitySource CreateSource( string       name, string       version )  => new(name, version);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter()                                                                                                                         => CreateMeter( GetAssembly() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter( Assembly     assembly )                                                                                                  => CreateMeter( assembly.GetName() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter( AssemblyName assembly )                                                                                                  => CreateMeter( assembly.Name ?? nameof(Database), assembly );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter( string       name )                                                                                                      => CreateMeter( name,                              GetAssembly() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter( string       name, Assembly     assembly )                                                                               => CreateMeter( name,                              assembly.GetName() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter( string       name, AssemblyName assembly )                                                                               => CreateMeter( name,                              assembly.GetVersion() );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter( string       name, AppVersion   version, IEnumerable<KeyValuePair<string, object?>>? tags = null, object? scope = null ) => CreateMeter( name,                              version.ToString(), tags, scope );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter          CreateMeter( string       name, string?      version, IEnumerable<KeyValuePair<string, object?>>? tags = null, object? scope = null ) => new(name, version, tags, scope);
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Assembly       GetAssembly()                            => Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static AppVersion     GetVersion( this Assembly     assembly ) => assembly.GetName().GetVersion();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static AppVersion     GetVersion( this AssemblyName assembly ) => assembly.Version ?? DefaultVersion;


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static Meter GetOrAddMeter( [CallerMemberName] string meterName = EMPTY ) => Meters.GetOrAdd( meterName, CreateMeter );


    public static Histogram<T> GetOrAdd<T>( string unit, string description, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( description, out Instrument? value ) && value is Histogram<T> instrument ) { return instrument; }

        Instruments[description] = instrument = GetOrAddMeter( meterName ).CreateHistogram<T>( meterName, unit, description, tags ?? Pairs );
        return instrument;
    }


    public static ObservableGauge<T> GetOrAddGauge<T>( string name, Func<Measurement<T>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableGauge<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateObservableGauge( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableGauge<T> GetOrAddGauge<T>( string name, Func<IEnumerable<Measurement<T>>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableGauge<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateObservableGauge( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }


    public static Counter<T> GetOrAddCounter<T>( string name, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is Counter<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateCounter<T>( name, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableCounter<T> GetOrAddCounter<T>( string name, Func<Measurement<T>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableCounter<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateObservableCounter( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableCounter<T> GetOrAddCounter<T>( string name, Func<IEnumerable<Measurement<T>>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableCounter<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateObservableCounter( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }


    public static UpDownCounter<T> GetOrAddUpDownCounter<T>( string name, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is UpDownCounter<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateUpDownCounter<T>( name, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableUpDownCounter<T> GetOrAddUpDownCounter<T>( string name, Func<Measurement<T>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableUpDownCounter<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateObservableUpDownCounter( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }
    public static ObservableUpDownCounter<T> GetOrAddUpDownCounter<T>( string name, Func<IEnumerable<Measurement<T>>> observeValue, string? unit, string? description = null, IEnumerable<KeyValuePair<string, object?>>? tags = null, [CallerMemberName] string meterName = EMPTY )
        where T : struct
    {
        if ( Instruments.TryGetValue( name, out Instrument? value ) && value is ObservableUpDownCounter<T> instrument ) { return instrument; }

        Instruments[name] = instrument = GetOrAddMeter( meterName ).CreateObservableUpDownCounter( name, observeValue, unit, description, tags ?? Pairs );
        return instrument;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserID( this              Activity activity, UserRecord  record )          => activity.AddTag( nameof(IUserID.UserID),      record.ID );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddSessionID( this           Activity activity, UserRecord  record )          => activity.AddTag( Tags.SessionID,              record.SessionID );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddRoleID( this              Activity activity, RoleRecord  record )          => activity.AddTag( Tags.RoleID,                 record.ID );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddGroupID( this             Activity activity, GroupRecord record )          => activity.AddTag( Tags.GroupID,                record.ID );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddGroup( this               Activity activity, string?     value = null ) => activity.AddTag( Tags.AddGroup,               value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddGroup( this               Activity activity, object?     value = null ) => activity.AddTag( Tags.AddGroup,               value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddGroupRights( this         Activity activity, string?     value = null ) => activity.AddTag( Tags.AddGroupRights,         value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddGroupRights( this         Activity activity, object?     value = null ) => activity.AddTag( Tags.AddGroupRights,         value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddRole( this                Activity activity, string?     value = null ) => activity.AddTag( Tags.AddRole,                value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddRole( this                Activity activity, object?     value = null ) => activity.AddTag( Tags.AddRole,                value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddRoleRights( this          Activity activity, string?     value = null ) => activity.AddTag( Tags.AddRoleRights,          value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddRoleRights( this          Activity activity, object?     value = null ) => activity.AddTag( Tags.AddRoleRights,          value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUser( this                Activity activity, string?     value = null ) => activity.AddTag( Tags.AddUser,                value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUser( this                Activity activity, object?     value = null ) => activity.AddTag( Tags.AddUser,                value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserAddress( this         Activity activity, string?     value = null ) => activity.AddTag( Tags.AddUserAddress,         value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserAddress( this         Activity activity, object?     value = null ) => activity.AddTag( Tags.AddUserAddress,         value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserLoginInfo( this       Activity activity, string?     value = null ) => activity.AddTag( Tags.AddUserLoginInfo,       value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserLoginInfo( this       Activity activity, object?     value = null ) => activity.AddTag( Tags.AddUserLoginInfo,       value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserRecoveryCode( this    Activity activity, string?     value = null ) => activity.AddTag( Tags.AddUserRecoveryCode,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserRecoveryCode( this    Activity activity, object?     value = null ) => activity.AddTag( Tags.AddUserRecoveryCode,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserRights( this          Activity activity, string?     value = null ) => activity.AddTag( Tags.AddUserRights,          value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserRights( this          Activity activity, object?     value = null ) => activity.AddTag( Tags.AddUserRights,          value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserSubscription( this    Activity activity, string?     value = null ) => activity.AddTag( Tags.AddUserSubscription,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserSubscription( this    Activity activity, object?     value = null ) => activity.AddTag( Tags.AddUserSubscription,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserToGroup( this         Activity activity, string?     value = null ) => activity.AddTag( Tags.AddUserToGroup,         value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserToGroup( this         Activity activity, object?     value = null ) => activity.AddTag( Tags.AddUserToGroup,         value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserToRole( this          Activity activity, string?     value = null ) => activity.AddTag( Tags.AddUserToRole,          value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AddUserToRole( this          Activity activity, object?     value = null ) => activity.AddTag( Tags.AddUserToRole,          value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void ConnectDatabase( this        Activity activity, string?     value = null ) => activity.AddTag( Tags.ConnectDatabase,        value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void ConnectDatabase( this        Activity activity, object?     value = null ) => activity.AddTag( Tags.ConnectDatabase,        value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void LoginUser( this              Activity activity, string?     value = null ) => activity.AddTag( Tags.LoginUser,              value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void LoginUser( this              Activity activity, object?     value = null ) => activity.AddTag( Tags.LoginUser,              value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveGroup( this            Activity activity, string?     value = null ) => activity.AddTag( Tags.RemoveGroup,            value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveGroup( this            Activity activity, object?     value = null ) => activity.AddTag( Tags.RemoveGroup,            value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveGroupRights( this      Activity activity, string?     value = null ) => activity.AddTag( Tags.RemoveGroupRights,      value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveGroupRights( this      Activity activity, object?     value = null ) => activity.AddTag( Tags.RemoveGroupRights,      value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveRole( this             Activity activity, string?     value = null ) => activity.AddTag( Tags.RemoveRole,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveRole( this             Activity activity, object?     value = null ) => activity.AddTag( Tags.RemoveRole,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveRoleRights( this       Activity activity, string?     value = null ) => activity.AddTag( Tags.RemoveRoleRights,       value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveRoleRights( this       Activity activity, object?     value = null ) => activity.AddTag( Tags.RemoveRoleRights,       value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUser( this             Activity activity, string?     value = null ) => activity.AddTag( Tags.RemoveUser,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUser( this             Activity activity, object?     value = null ) => activity.AddTag( Tags.RemoveUser,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserAddress( this      Activity activity, string?     value = null ) => activity.AddTag( Tags.RemoveUserAddress,      value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserAddress( this      Activity activity, object?     value = null ) => activity.AddTag( Tags.RemoveUserAddress,      value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserFromGroup( this    Activity activity, string?     value = null ) => activity.AddTag( Tags.RemoveUserFromGroup,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserFromGroup( this    Activity activity, object?     value = null ) => activity.AddTag( Tags.RemoveUserFromGroup,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserFromRole( this     Activity activity, string?     value = null ) => activity.AddTag( Tags.RemoveUserFromRole,     value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserFromRole( this     Activity activity, object?     value = null ) => activity.AddTag( Tags.RemoveUserFromRole,     value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserLoginInfo( this    Activity activity, string?     value = null ) => activity.AddTag( Tags.RemoveUserLoginInfo,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserLoginInfo( this    Activity activity, object?     value = null ) => activity.AddTag( Tags.RemoveUserLoginInfo,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserRecoveryCode( this Activity activity, string?     value = null ) => activity.AddTag( Tags.RemoveUserRecoveryCode, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserRecoveryCode( this Activity activity, object?     value = null ) => activity.AddTag( Tags.RemoveUserRecoveryCode, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserRights( this       Activity activity, string?     value = null ) => activity.AddTag( Tags.RemoveUserRights,       value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserRights( this       Activity activity, object?     value = null ) => activity.AddTag( Tags.RemoveUserRights,       value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserSubscription( this Activity activity, string?     value = null ) => activity.AddTag( Tags.RemoveUserSubscription, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void RemoveUserSubscription( this Activity activity, object?     value = null ) => activity.AddTag( Tags.RemoveUserSubscription, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateGroup( this            Activity activity, string?     value = null ) => activity.AddTag( Tags.UpdateGroup,            value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateGroup( this            Activity activity, object?     value = null ) => activity.AddTag( Tags.UpdateGroup,            value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateRole( this             Activity activity, string?     value = null ) => activity.AddTag( Tags.UpdateRole,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateRole( this             Activity activity, object?     value = null ) => activity.AddTag( Tags.UpdateRole,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUser( this             Activity activity, string?     value = null ) => activity.AddTag( Tags.UpdateUser,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUser( this             Activity activity, object?     value = null ) => activity.AddTag( Tags.UpdateUser,             value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUserAddress( this      Activity activity, string?     value = null ) => activity.AddTag( Tags.UpdateUserAddress,      value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUserAddress( this      Activity activity, object?     value = null ) => activity.AddTag( Tags.UpdateUserAddress,      value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUserLoginInfo( this    Activity activity, string?     value = null ) => activity.AddTag( Tags.UpdateUserLoginInfo,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUserLoginInfo( this    Activity activity, object?     value = null ) => activity.AddTag( Tags.UpdateUserLoginInfo,    value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUserSubscription( this Activity activity, string?     value = null ) => activity.AddTag( Tags.UpdateUserSubscription, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void UpdateUserSubscription( this Activity activity, object?     value = null ) => activity.AddTag( Tags.UpdateUserSubscription, value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void VerifyLogin( this            Activity activity, string?     value = null ) => activity.AddTag( Tags.VerifyLogin,            value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void VerifyLogin( this            Activity activity, object?     value = null ) => activity.AddTag( Tags.VerifyLogin,            value );



    [SuppressMessage( "ReSharper", "MemberHidesStaticFromOuterClass" )]
    public static class Tags
    {
        private static string? _prefix;

        public static string AddGroup            { get; set; } = nameof(AddGroup);
        public static string AddGroupRights      { get; set; } = nameof(AddGroupRights);
        public static string AddRole             { get; set; } = nameof(AddRole);
        public static string AddRoleRights       { get; set; } = nameof(AddRoleRights);
        public static string AddUser             { get; set; } = nameof(AddUser);
        public static string AddUserAddress      { get; set; } = nameof(AddUserAddress);
        public static string AddUserLoginInfo    { get; set; } = nameof(AddUserLoginInfo);
        public static string AddUserRecoveryCode { get; set; } = nameof(AddUserRecoveryCode);
        public static string AddUserRights       { get; set; } = nameof(AddUserRights);
        public static string AddUserSubscription { get; set; } = nameof(AddUserSubscription);
        public static string AddUserToGroup      { get; set; } = nameof(AddUserToGroup);
        public static string AddUserToRole       { get; set; } = nameof(AddUserToRole);
        public static string ConnectDatabase     { get; set; } = nameof(ConnectDatabase);
        public static string GroupID             { get; set; } = nameof(GroupID);
        public static string LoginUser           { get; set; } = nameof(LoginUser);
        public static string? Prefix
        {
            get => _prefix;
            set
            {
                _prefix = value;
                SetPrefix( value );
            }
        }
        public static string RemoveGroup            { get; set; } = nameof(RemoveGroup);
        public static string RemoveGroupRights      { get; set; } = nameof(RemoveGroupRights);
        public static string RemoveRole             { get; set; } = nameof(RemoveRole);
        public static string RemoveRoleRights       { get; set; } = nameof(RemoveRoleRights);
        public static string RemoveUser             { get; set; } = nameof(RemoveUser);
        public static string RemoveUserAddress      { get; set; } = nameof(RemoveUserAddress);
        public static string RemoveUserFromGroup    { get; set; } = nameof(RemoveUserFromGroup);
        public static string RemoveUserFromRole     { get; set; } = nameof(RemoveUserFromRole);
        public static string RemoveUserLoginInfo    { get; set; } = nameof(RemoveUserLoginInfo);
        public static string RemoveUserRecoveryCode { get; set; } = nameof(RemoveUserRecoveryCode);
        public static string RemoveUserRights       { get; set; } = nameof(RemoveUserRights);
        public static string RemoveUserSubscription { get; set; } = nameof(RemoveUserSubscription);
        public static string RoleID                 { get; set; } = nameof(RoleID);
        public static string SessionID              { get; set; } = nameof(SessionID);
        public static string UpdateGroup            { get; set; } = nameof(UpdateGroup);
        public static string UpdateRole             { get; set; } = nameof(UpdateRole);
        public static string UpdateUser             { get; set; } = nameof(UpdateUser);
        public static string UpdateUserAddress      { get; set; } = nameof(UpdateUserAddress);
        public static string UpdateUserLoginInfo    { get; set; } = nameof(UpdateUserLoginInfo);
        public static string UpdateUserSubscription { get; set; } = nameof(UpdateUserSubscription);
        public static string VerifyLogin            { get; set; } = nameof(VerifyLogin);


        [Conditional( "DEBUG" )]
        internal static void Print()
        {
            ReadOnlySpan<PropertyInfo> properties = typeof(Tags).GetProperties( BindingFlags.Static | BindingFlags.Public ).Where( static x => x.Name != nameof(Prefix) ).ToArray();

            foreach ( PropertyInfo property in properties ) { Console.WriteLine( GetPrefixLine( property.Name ) ); }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            foreach ( PropertyInfo property in properties ) { Console.WriteLine( GetMethodLine( property.Name ) ); }

            return;
            static string GetPrefixLine( string property ) => $"            {property} = GetPrefix( prefix, {property}, nameof({property}) );";

            static string GetMethodLine( string property ) =>
                $"""
                 [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void {property}( this Activity activity,  string? value = default ) => activity.AddTag( Tags.{property}, value );
                 [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void {property}( this Activity activity,  object? value = default ) => activity.AddTag( Tags.{property}, value );
                 """;
        }


        private static void SetPrefix( in ReadOnlySpan<char> prefix )
        {
            ConnectDatabase        = GetPrefix( prefix, ConnectDatabase,        nameof(ConnectDatabase) );
            AddUser                = GetPrefix( prefix, AddUser,                nameof(AddUser) );
            UpdateUser             = GetPrefix( prefix, UpdateUser,             nameof(UpdateUser) );
            RemoveUser             = GetPrefix( prefix, RemoveUser,             nameof(RemoveUser) );
            AddUserLoginInfo       = GetPrefix( prefix, AddUserLoginInfo,       nameof(AddUserLoginInfo) );
            UpdateUserLoginInfo    = GetPrefix( prefix, UpdateUserLoginInfo,    nameof(UpdateUserLoginInfo) );
            RemoveUserLoginInfo    = GetPrefix( prefix, RemoveUserLoginInfo,    nameof(RemoveUserLoginInfo) );
            AddUserAddress         = GetPrefix( prefix, AddUserAddress,         nameof(AddUserAddress) );
            UpdateUserAddress      = GetPrefix( prefix, UpdateUserAddress,      nameof(UpdateUserAddress) );
            RemoveUserAddress      = GetPrefix( prefix, RemoveUserAddress,      nameof(RemoveUserAddress) );
            AddUserSubscription    = GetPrefix( prefix, AddUserSubscription,    nameof(AddUserSubscription) );
            UpdateUserSubscription = GetPrefix( prefix, UpdateUserSubscription, nameof(UpdateUserSubscription) );
            RemoveUserSubscription = GetPrefix( prefix, RemoveUserSubscription, nameof(RemoveUserSubscription) );
            AddUserRecoveryCode    = GetPrefix( prefix, AddUserRecoveryCode,    nameof(AddUserRecoveryCode) );
            RemoveUserRecoveryCode = GetPrefix( prefix, RemoveUserRecoveryCode, nameof(RemoveUserRecoveryCode) );
            AddUserRights          = GetPrefix( prefix, AddUserRights,          nameof(AddUserRights) );
            RemoveUserRights       = GetPrefix( prefix, RemoveUserRights,       nameof(RemoveUserRights) );
            LoginUser              = GetPrefix( prefix, LoginUser,              nameof(LoginUser) );
            VerifyLogin            = GetPrefix( prefix, VerifyLogin,            nameof(VerifyLogin) );
            AddGroup               = GetPrefix( prefix, AddGroup,               nameof(AddGroup) );
            RemoveGroup            = GetPrefix( prefix, RemoveGroup,            nameof(RemoveGroup) );
            UpdateGroup            = GetPrefix( prefix, UpdateGroup,            nameof(UpdateGroup) );
            AddGroupRights         = GetPrefix( prefix, AddGroupRights,         nameof(AddGroupRights) );
            RemoveGroupRights      = GetPrefix( prefix, RemoveGroupRights,      nameof(RemoveGroupRights) );
            AddUserToGroup         = GetPrefix( prefix, AddUserToGroup,         nameof(AddUserToGroup) );
            RemoveUserFromGroup    = GetPrefix( prefix, RemoveUserFromGroup,    nameof(RemoveUserFromGroup) );
            AddRole                = GetPrefix( prefix, AddRole,                nameof(AddRole) );
            RemoveRole             = GetPrefix( prefix, RemoveRole,             nameof(RemoveRole) );
            UpdateRole             = GetPrefix( prefix, UpdateRole,             nameof(UpdateRole) );
            AddRoleRights          = GetPrefix( prefix, AddRoleRights,          nameof(AddRoleRights) );
            RemoveRoleRights       = GetPrefix( prefix, RemoveRoleRights,       nameof(RemoveRoleRights) );
            AddUserToRole          = GetPrefix( prefix, AddUserToRole,          nameof(AddUserToRole) );
            RemoveUserFromRole     = GetPrefix( prefix, RemoveUserFromRole,     nameof(RemoveUserFromRole) );
        }
        private static string GetPrefix( in ReadOnlySpan<char> prefix, in ReadOnlySpan<char> tag, in ReadOnlySpan<char> defaultTag )
        {
            if ( prefix.IsEmpty )
            {
                return tag.IsEmpty
                           ? defaultTag.ToString()
                           : tag.ToString();
            }

            if ( tag.IsEmpty ) { return GetResult( prefix, tag ); }

            return GetResult( prefix, defaultTag );

            static string GetResult( in ReadOnlySpan<char> prefix, in ReadOnlySpan<char> tag )
            {
                Span<char> result = stackalloc char[prefix.Length + tag.Length];
                prefix.CopyTo( result );
                result[prefix.Length] = '.';

                tag.CopyTo( result[(prefix.Length + 1)..] );
                return result.ToString();
            }
        }
    }
}
