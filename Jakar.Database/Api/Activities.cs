// Jakar.Extensions :: Jakar.Database
// 12/02/2023  12:12 PM

namespace Jakar.Database;


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


    public static Activity Start( [ CallerMemberName ] string name = EMPTY, ActivityKind kind = ActivityKind.Internal ) =>
        Source.StartActivity( name, kind ) ?? throw new InvalidOperationException( $"{nameof(Source)} doesn't have any Listeners" );
    public static Activity Start( ActivityKind                                kind          = ActivityKind.Internal,
                                  ActivityContext                             parentContext = default,
                                  IEnumerable<KeyValuePair<string, object?>>? tags          = null,
                                  IEnumerable<ActivityLink>?                  links         = null,
                                  [ CallerMemberName ] string                 name          = EMPTY
    ) => Start( DateTimeOffset.UtcNow, kind, parentContext, tags, links, name );
    public static Activity Start( DateTimeOffset                              startTime,
                                  ActivityKind                                kind          = ActivityKind.Internal,
                                  ActivityContext                             parentContext = default,
                                  IEnumerable<KeyValuePair<string, object?>>? tags          = null,
                                  IEnumerable<ActivityLink>?                  links         = null,
                                  [ CallerMemberName ] string                 name          = EMPTY
    ) => Source.StartActivity( name, kind, parentContext, tags, links, startTime ) ?? throw new InvalidOperationException( $"{nameof(Source)} doesn't have any Listeners" );
    public static Activity Create( [ CallerMemberName ] string name = EMPTY, ActivityKind kind = ActivityKind.Internal ) =>
        Source.CreateActivity( name, kind ) ?? throw new InvalidOperationException( $"{nameof(Source)} doesn't have any Listeners" );
    public static Activity Create( ActivityKind                                kind          = ActivityKind.Internal,
                                   ActivityIdFormat                            idFormat      = ActivityIdFormat.Hierarchical,
                                   ActivityContext                             parentContext = default,
                                   IEnumerable<KeyValuePair<string, object?>>? tags          = null,
                                   IEnumerable<ActivityLink>?                  links         = null,
                                   [ CallerMemberName ] string                 name          = EMPTY
    ) => Source.CreateActivity( name, kind, parentContext, tags, links, idFormat ) ?? throw new InvalidOperationException( $"{nameof(Source)} doesn't have any Listeners" );



    [ Experimental( nameof(Tags) ) ]
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
        public static  string  Database               { get; set; } = nameof(Database);
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
            IEnumerable<PropertyInfo> properties = typeof(Tags).GetProperties( BindingFlags.Static | BindingFlags.Public ).Where( static x => x.Name != nameof(Prefix) );

            foreach ( PropertyInfo property in properties )
            {
                string line = GetLine( property.Name );
                Console.WriteLine( line );
            }

            return;
            static string GetLine( string property ) { return $"            {property} = GetPrefix( prefix, {property}, nameof({property}) );"; }
        }


        private static void SetPrefix( in ReadOnlySpan<char> prefix )
        {
            Database               = GetPrefix( prefix, Database,               nameof(Database) );
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
