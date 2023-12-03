// Jakar.Extensions :: Jakar.Database
// 12/02/2023  12:12 PM

namespace Jakar.Database;


[ Experimental( nameof(OpenTelemetry) ) ]
public static class Activities
{
    public const  string         EMPTY = "";
    public static ActivitySource Source { get; set; } = new(GetSourceName(), GetAssemblyVersion().ToString());


    private static Version GetAssemblyVersion()
    {
        Version? version = Assembly.GetEntryAssembly()?.GetName().Version ?? Assembly.GetCallingAssembly().GetName().Version ?? Assembly.GetExecutingAssembly().GetName().Version;
        if ( version is null ) { return new Version( 1, 0, 0 ); }

        return version;
    }
    private static string GetSourceName() => Assembly.GetEntryAssembly()?.GetName().Name ?? Assembly.GetCallingAssembly().GetName().Name ?? Assembly.GetExecutingAssembly().GetName().Name ?? nameof(Database);


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static Activity Start( [ CallerMemberName ] string name = EMPTY, ActivityKind kind = ActivityKind.Internal ) =>
        Source.StartActivity( name, kind ) ?? throw new InvalidOperationException( $"{nameof(Source)} doesn't have any Listeners" );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static Activity Start( ActivityKind                                kind          = ActivityKind.Internal,
                                  ActivityContext                             parentContext = default,
                                  IEnumerable<KeyValuePair<string, object?>>? tags          = null,
                                  IEnumerable<ActivityLink>?                  links         = null,
                                  [ CallerMemberName ] string                 name          = EMPTY
    ) => Start( DateTimeOffset.UtcNow, kind, parentContext, tags, links, name );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static Activity Start( DateTimeOffset                              startTime,
                                  ActivityKind                                kind          = ActivityKind.Internal,
                                  ActivityContext                             parentContext = default,
                                  IEnumerable<KeyValuePair<string, object?>>? tags          = null,
                                  IEnumerable<ActivityLink>?                  links         = null,
                                  [ CallerMemberName ] string                 name          = EMPTY
    ) => Source.StartActivity( name, kind, parentContext, tags, links, startTime ) ?? throw new InvalidOperationException( $"{nameof(Source)} doesn't have any Listeners" );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static Activity Create( [ CallerMemberName ] string name = EMPTY, ActivityKind kind = ActivityKind.Internal ) =>
        Source.CreateActivity( name, kind ) ?? throw new InvalidOperationException( $"{nameof(Source)} doesn't have any Listeners" );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ]
    public static Activity Create( ActivityKind                                kind          = ActivityKind.Internal,
                                   ActivityIdFormat                            idFormat      = ActivityIdFormat.W3C,
                                   ActivityContext                             parentContext = default,
                                   IEnumerable<KeyValuePair<string, object?>>? tags          = null,
                                   IEnumerable<ActivityLink>?                  links         = null,
                                   [ CallerMemberName ] string                 name          = EMPTY
    ) => Source.CreateActivity( name, kind, parentContext, tags, links, idFormat ) ?? throw new InvalidOperationException( $"{nameof(Source)} doesn't have any Listeners" );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserID( this    Activity activity, UserRecord  record ) => activity.AddTag( nameof(record.UserID),    record.UserID );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddSessionID( this Activity activity, UserRecord  record ) => activity.AddTag( nameof(record.SessionID), record.SessionID );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddRoleID( this    Activity activity, RoleRecord  record ) => activity.AddTag( nameof(record.ID),        record.ID );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddGroupID( this   Activity activity, GroupRecord record ) => activity.AddTag( nameof(record.ID),        record.ID );


    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddGroup( this               Activity activity, string? value = default ) => activity.AddTag( Tags.AddGroup,               value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddGroup( this               Activity activity, object? value = default ) => activity.AddTag( Tags.AddGroup,               value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddGroupRights( this         Activity activity, string? value = default ) => activity.AddTag( Tags.AddGroupRights,         value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddGroupRights( this         Activity activity, object? value = default ) => activity.AddTag( Tags.AddGroupRights,         value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddRole( this                Activity activity, string? value = default ) => activity.AddTag( Tags.AddRole,                value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddRole( this                Activity activity, object? value = default ) => activity.AddTag( Tags.AddRole,                value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddRoleRights( this          Activity activity, string? value = default ) => activity.AddTag( Tags.AddRoleRights,          value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddRoleRights( this          Activity activity, object? value = default ) => activity.AddTag( Tags.AddRoleRights,          value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUser( this                Activity activity, string? value = default ) => activity.AddTag( Tags.AddUser,                value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUser( this                Activity activity, object? value = default ) => activity.AddTag( Tags.AddUser,                value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserAddress( this         Activity activity, string? value = default ) => activity.AddTag( Tags.AddUserAddress,         value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserAddress( this         Activity activity, object? value = default ) => activity.AddTag( Tags.AddUserAddress,         value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserLoginInfo( this       Activity activity, string? value = default ) => activity.AddTag( Tags.AddUserLoginInfo,       value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserLoginInfo( this       Activity activity, object? value = default ) => activity.AddTag( Tags.AddUserLoginInfo,       value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserRecoveryCode( this    Activity activity, string? value = default ) => activity.AddTag( Tags.AddUserRecoveryCode,    value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserRecoveryCode( this    Activity activity, object? value = default ) => activity.AddTag( Tags.AddUserRecoveryCode,    value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserRights( this          Activity activity, string? value = default ) => activity.AddTag( Tags.AddUserRights,          value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserRights( this          Activity activity, object? value = default ) => activity.AddTag( Tags.AddUserRights,          value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserSubscription( this    Activity activity, string? value = default ) => activity.AddTag( Tags.AddUserSubscription,    value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserSubscription( this    Activity activity, object? value = default ) => activity.AddTag( Tags.AddUserSubscription,    value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserToGroup( this         Activity activity, string? value = default ) => activity.AddTag( Tags.AddUserToGroup,         value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserToGroup( this         Activity activity, object? value = default ) => activity.AddTag( Tags.AddUserToGroup,         value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserToRole( this          Activity activity, string? value = default ) => activity.AddTag( Tags.AddUserToRole,          value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void AddUserToRole( this          Activity activity, object? value = default ) => activity.AddTag( Tags.AddUserToRole,          value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void ConnectDatabase( this        Activity activity, string? value = default ) => activity.AddTag( Tags.ConnectDatabase,        value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void ConnectDatabase( this        Activity activity, object? value = default ) => activity.AddTag( Tags.ConnectDatabase,        value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void LoginUser( this              Activity activity, string? value = default ) => activity.AddTag( Tags.LoginUser,              value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void LoginUser( this              Activity activity, object? value = default ) => activity.AddTag( Tags.LoginUser,              value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveGroup( this            Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveGroup,            value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveGroup( this            Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveGroup,            value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveGroupRights( this      Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveGroupRights,      value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveGroupRights( this      Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveGroupRights,      value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveRole( this             Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveRole,             value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveRole( this             Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveRole,             value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveRoleRights( this       Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveRoleRights,       value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveRoleRights( this       Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveRoleRights,       value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUser( this             Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUser,             value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUser( this             Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUser,             value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUserAddress( this      Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUserAddress,      value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUserAddress( this      Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUserAddress,      value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUserFromGroup( this    Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUserFromGroup,    value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUserFromGroup( this    Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUserFromGroup,    value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUserFromRole( this     Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUserFromRole,     value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUserFromRole( this     Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUserFromRole,     value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUserLoginInfo( this    Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUserLoginInfo,    value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUserLoginInfo( this    Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUserLoginInfo,    value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUserRecoveryCode( this Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUserRecoveryCode, value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUserRecoveryCode( this Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUserRecoveryCode, value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUserRights( this       Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUserRights,       value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUserRights( this       Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUserRights,       value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUserSubscription( this Activity activity, string? value = default ) => activity.AddTag( Tags.RemoveUserSubscription, value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void RemoveUserSubscription( this Activity activity, object? value = default ) => activity.AddTag( Tags.RemoveUserSubscription, value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void UpdateGroup( this            Activity activity, string? value = default ) => activity.AddTag( Tags.UpdateGroup,            value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void UpdateGroup( this            Activity activity, object? value = default ) => activity.AddTag( Tags.UpdateGroup,            value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void UpdateRole( this             Activity activity, string? value = default ) => activity.AddTag( Tags.UpdateRole,             value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void UpdateRole( this             Activity activity, object? value = default ) => activity.AddTag( Tags.UpdateRole,             value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void UpdateUser( this             Activity activity, string? value = default ) => activity.AddTag( Tags.UpdateUser,             value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void UpdateUser( this             Activity activity, object? value = default ) => activity.AddTag( Tags.UpdateUser,             value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void UpdateUserAddress( this      Activity activity, string? value = default ) => activity.AddTag( Tags.UpdateUserAddress,      value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void UpdateUserAddress( this      Activity activity, object? value = default ) => activity.AddTag( Tags.UpdateUserAddress,      value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void UpdateUserLoginInfo( this    Activity activity, string? value = default ) => activity.AddTag( Tags.UpdateUserLoginInfo,    value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void UpdateUserLoginInfo( this    Activity activity, object? value = default ) => activity.AddTag( Tags.UpdateUserLoginInfo,    value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void UpdateUserSubscription( this Activity activity, string? value = default ) => activity.AddTag( Tags.UpdateUserSubscription, value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void UpdateUserSubscription( this Activity activity, object? value = default ) => activity.AddTag( Tags.UpdateUserSubscription, value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void VerifyLogin( this            Activity activity, string? value = default ) => activity.AddTag( Tags.VerifyLogin,            value );
    [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void VerifyLogin( this            Activity activity, object? value = default ) => activity.AddTag( Tags.VerifyLogin,            value );



    [ SuppressMessage( "ReSharper", "MemberHidesStaticFromOuterClass" ) ]
    public static class Tags
    {
        private static string? _prefix;
        public static  string  AddGroup               { get; set; } = nameof(AddGroup);
        public static  string  AddGroupRights         { get; set; } = nameof(AddGroupRights);
        public static  string  AddRole                { get; set; } = nameof(AddRole);
        public static  string  AddRoleRights          { get; set; } = nameof(AddRoleRights);
        public static  string  AddUser                { get; set; } = nameof(AddUser);
        public static  string  AddUserAddress         { get; set; } = nameof(AddUserAddress);
        public static  string  AddUserLoginInfo       { get; set; } = nameof(AddUserLoginInfo);
        public static  string  AddUserRecoveryCode    { get; set; } = nameof(AddUserRecoveryCode);
        public static  string  AddUserRights          { get; set; } = nameof(AddUserRights);
        public static  string  AddUserSubscription    { get; set; } = nameof(AddUserSubscription);
        public static  string  AddUserToGroup         { get; set; } = nameof(AddUserToGroup);
        public static  string  AddUserToRole          { get; set; } = nameof(AddUserToRole);
        public static  string  ConnectDatabase        { get; set; } = nameof(ConnectDatabase);
        public static  string  LoginUser              { get; set; } = nameof(LoginUser);
        public static  string  RemoveGroup            { get; set; } = nameof(RemoveGroup);
        public static  string  RemoveGroupRights      { get; set; } = nameof(RemoveGroupRights);
        public static  string  RemoveRole             { get; set; } = nameof(RemoveRole);
        public static  string  RemoveRoleRights       { get; set; } = nameof(RemoveRoleRights);
        public static  string  RemoveUser             { get; set; } = nameof(RemoveUser);
        public static  string  RemoveUserAddress      { get; set; } = nameof(RemoveUserAddress);
        public static  string  RemoveUserFromGroup    { get; set; } = nameof(RemoveUserFromGroup);
        public static  string  RemoveUserFromRole     { get; set; } = nameof(RemoveUserFromRole);
        public static  string  RemoveUserLoginInfo    { get; set; } = nameof(RemoveUserLoginInfo);
        public static  string  RemoveUserRecoveryCode { get; set; } = nameof(RemoveUserRecoveryCode);
        public static  string  RemoveUserRights       { get; set; } = nameof(RemoveUserRights);
        public static  string  RemoveUserSubscription { get; set; } = nameof(RemoveUserSubscription);
        public static  string  UpdateGroup            { get; set; } = nameof(UpdateGroup);
        public static  string  UpdateRole             { get; set; } = nameof(UpdateRole);
        public static  string  UpdateUser             { get; set; } = nameof(UpdateUser);
        public static  string  UpdateUserAddress      { get; set; } = nameof(UpdateUserAddress);
        public static  string  UpdateUserLoginInfo    { get; set; } = nameof(UpdateUserLoginInfo);
        public static  string  UpdateUserSubscription { get; set; } = nameof(UpdateUserSubscription);
        public static  string  VerifyLogin            { get; set; } = nameof(VerifyLogin);
        public static string? Prefix
        {
            get => _prefix;
            set
            {
                _prefix = value;
                SetPrefix( value );
            }
        }


        [ Conditional( "DEBUG" ) ]
        internal static void Print()
        {
            ReadOnlySpan<PropertyInfo> properties = typeof(Tags).GetProperties( BindingFlags.Static | BindingFlags.Public ).Where( static x => x.Name != nameof(Prefix) ).ToArray();

            foreach ( PropertyInfo property in properties ) { Console.WriteLine( GetPrefixLine( property.Name ) ); }

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            foreach ( PropertyInfo property in properties ) { Console.WriteLine( GetMethodLine( property.Name ) ); }

            return;
            static string GetPrefixLine( string property ) { return $"            {property} = GetPrefix( prefix, {property}, nameof({property}) );"; }

            static string GetMethodLine( string property )
            {
                return $"""
                        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void {property}( this Activity activity, string? value = default ) => activity.AddTag( Tags.{property}, value );
                        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public static void {property}( this Activity activity, object? value = default ) => activity.AddTag( Tags.{property}, value );
                        """;
            }
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
