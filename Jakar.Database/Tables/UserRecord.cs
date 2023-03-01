// ToothFairyDispatch :: ToothFairyDispatch.Cloud
// 08/29/2022  9:55 PM

namespace Jakar.Database;


/// <summary>
///     <para>
///         <see cref="IUserID"/>
///     </para>
///     <para>
///         <see cref="IUserData"/>
///     </para>
///     <para>
///         <see cref="IUserControl"/>
///     </para>
///     <para>
///         <see cref="IUserSubscription"/>
///     </para>
///     <para>
///         <see cref="IRefreshToken"/>
///     </para>
/// </summary>
[Serializable]
[Table( "Users" )]
public sealed partial record UserRecord : TableRecord<UserRecord>, JsonModels.IJsonStringModel, IRefreshToken, IUserControl, IUserID, IUserDataRecord, IUserSecurity, IUserSubscription
{
    public UserRecord() { }
    public UserRecord( IUserData data, string? rights, UserRecord? caller = default ) : this( Guid.NewGuid(), caller )
    {
        ArgumentNullException.ThrowIfNull( data );
        FirstName         = data.FirstName;
        LastName          = data.LastName;
        FullName          = data.FullName;
        Address           = data.Address;
        Line1             = data.Line1;
        Line2             = data.Line2;
        City              = data.City;
        State             = data.State;
        Country           = data.Country;
        PostalCode        = data.PostalCode;
        Description       = data.Description;
        Website           = data.Website;
        Email             = data.Email;
        PhoneNumber       = data.PhoneNumber;
        Ext               = data.Ext;
        Title             = data.Title;
        Department        = data.Department;
        Company           = data.Company;
        PreferredLanguage = data.PreferredLanguage;
        Rights            = rights ?? string.Empty;
    }
    public UserRecord( Guid id, UserRecord? caller = default ) : base( id, caller ) => UserID = Guid.NewGuid();
    public UserRecord( string userName, string password, string rights, UserRecord? caller = default ) : base( Guid.NewGuid(), caller )
    {
        ArgumentNullException.ThrowIfNull( userName );
        ArgumentNullException.ThrowIfNull( password );
        UserID   = Guid.NewGuid();
        UserName = userName;
        Rights   = rights;
        UpdatePassword( password );
    }


    private static readonly PasswordHasher<UserRecord> _hasher = new();


    public static UserRecord Create<TUser>( VerifyRequest<TUser> request, string rights, UserRecord? caller = default ) where TUser : IUserData

    {
        ArgumentNullException.ThrowIfNull( request.Data );

        UserRecord record = Create( request.Data, rights, caller );
        record.UserName = request.UserLogin;
        record.UpdatePassword( request.UserPassword );
        return record;
    }
    public static UserRecord Create( IUserData value,    string rights,   UserRecord? caller                     = default ) => new(value, rights, caller);
    public static UserRecord Create( string    userName, string password, string      rights, UserRecord? caller = default ) => new(userName, password, rights, caller);


    public static DynamicParameters GetDynamicParameters( IUserData data )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(FirstName), data.FirstName );
        parameters.Add( nameof(LastName),  data.LastName );
        parameters.Add( nameof(FullName),  data.FullName );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters<T>( VerifyRequest<T> request ) where T : notnull
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserName), request.UserLogin );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( ILoginRequest request )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserName), request.UserLogin );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( string userName )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserName), userName );
        return parameters;
    }


    public UserRecord ClearRefreshToken( string securityStamp )
    {
        RefreshToken           = default;
        RefreshTokenExpiryTime = default;
        SecurityStamp          = securityStamp;
        return this;
    }


    [RequiresPreviewFeatures] public ValueTask<bool> RedeemCode( Database db, string code, CancellationToken token ) => db.TryCall( RedeemCode, db, code, token );
    [RequiresPreviewFeatures]
    public async ValueTask<bool> RedeemCode( DbConnection connection, DbTransaction transaction, Database db, string code, CancellationToken token )
    {
        UserRecoveryCodeRecord[] mappings = await UserRecoveryCodeRecord.Where( connection, transaction, db.UserRecoveryCodes, this, token );

        foreach ( UserRecoveryCodeRecord mapping in mappings )
        {
            RecoveryCodeRecord? record = await mapping.Get( connection, transaction, db.RecoveryCodes, token );

            if ( record is null ) { await db.UserRecoveryCodes.Delete( connection, transaction, mapping, token ); }
            else if ( record.IsValid( code ) )
            {
                await db.RecoveryCodes.Delete( connection, transaction, record, token );
                await db.UserRecoveryCodes.Delete( connection, transaction, mapping, token );
                return true;
            }
        }

        return false;
    }


    [RequiresPreviewFeatures] public ValueTask<string[]> ReplaceCodes( Database db, int count = 10, CancellationToken token = default ) => db.TryCall( ReplaceCodes, db, count, token );
    [RequiresPreviewFeatures]
    public async ValueTask<string[]> ReplaceCodes( DbConnection connection, DbTransaction transaction, Database db, int count = 10, CancellationToken token = default )
    {
        RecoveryCodeRecord[]                            old        = await Codes( connection, transaction, db, token );
        IReadOnlyDictionary<string, RecoveryCodeRecord> dictionary = RecoveryCodeRecord.Create( this, count );
        string[]                                        codes      = dictionary.Keys.ToArray();


        await db.RecoveryCodes.Delete( connection, transaction, old, token );
        await UserRecoveryCodeRecord.Replace( connection, transaction, db.UserRecoveryCodes, this, dictionary.Values, token );
        return codes;
    }


    [RequiresPreviewFeatures] public ValueTask<string[]> ReplaceCodes( Database db, IEnumerable<string> recoveryCodes, CancellationToken token = default ) => db.TryCall( ReplaceCodes, db, recoveryCodes, token );
    [RequiresPreviewFeatures]
    public async ValueTask<string[]> ReplaceCodes( DbConnection connection, DbTransaction transaction, Database db, IEnumerable<string> recoveryCodes, CancellationToken token = default )
    {
        RecoveryCodeRecord[]                            old        = await Codes( connection, transaction, db, token );
        IReadOnlyDictionary<string, RecoveryCodeRecord> dictionary = RecoveryCodeRecord.Create( this, recoveryCodes );
        string[]                                        codes      = dictionary.Keys.ToArray();


        await db.RecoveryCodes.Delete( connection, transaction, old, token );
        await UserRecoveryCodeRecord.Replace( connection, transaction, db.UserRecoveryCodes, this, dictionary.Values, token );
        return codes;
    }


    [RequiresPreviewFeatures] public ValueTask<RecoveryCodeRecord[]> Codes( Database db, CancellationToken token ) => db.TryCall( Codes, db, token );
    [RequiresPreviewFeatures]
    public async ValueTask<RecoveryCodeRecord[]> Codes( DbConnection connection, DbTransaction transaction, Database db, CancellationToken token ) =>
        await UserRecoveryCodeRecord.Where( connection, transaction, db.UserRecoveryCodes, db.RecoveryCodes, this, token );



    #region Passwords

    public bool HasPassword() => !string.IsNullOrWhiteSpace( PasswordHash );
    /// <summary>
    ///     <para>
    ///         <see href="https://stackoverflow.com/a/63733365/9530917"/>
    ///     </para>
    ///     <para>
    ///         <see href="https://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file"/>
    ///     </para>
    ///     <see cref="PasswordHasher{TRecord}"/>
    /// </summary>
    public UserRecord UpdatePassword( string password )
    {
        PasswordHash = _hasher.HashPassword( this, password );
        return this;
    }
    /// <summary>
    ///     <para>
    ///         <see href="https://stackoverflow.com/a/63733365/9530917"/>
    ///     </para>
    ///     <para>
    ///         <see href="https://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file"/>
    ///     </para>
    ///     <see cref="PasswordHasher{TRecord}"/>
    /// </summary>
    public PasswordVerificationResult VerifyPassword( string password ) => _hasher.VerifyHashedPassword( this, PasswordHash, password );

    #endregion



    #region Owners

    public async ValueTask<UserRecord?> GetBoss( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => !string.IsNullOrEmpty( EscalateTo )
                                                                                                                                                    ? await db.Users.Get( connection, transaction, EscalateTo, token )
                                                                                                                                                    : default;


    public bool DoesNotOwn<TRecord>( TRecord record ) where TRecord : TableRecord<TRecord> => record.CreatedBy != ID;
    public bool Owns<TRecord>( TRecord       record ) where TRecord : TableRecord<TRecord> => record.CreatedBy == ID;

    #endregion



    #region Controls

    public UserRecord MarkBadLogin()
    {
        LastBadAttempt =  DateTimeOffset.UtcNow;
        BadLogins      += 1;
        IsDisabled     =  BadLogins > 5;

        return IsDisabled
                   ? Lock()
                   : Unlock();
    }
    public UserRecord SetActive()
    {
        LastActive = DateTimeOffset.UtcNow;
        IsDisabled = false;
        return this;
    }
    public UserRecord Disable()
    {
        IsDisabled = true;
        return Lock();
    }
    public UserRecord Lock() => Lock( TimeSpan.FromHours( 6 ) );
    public UserRecord Lock( in TimeSpan offset )
    {
        IsDisabled = true;
        LockDate   = DateTimeOffset.UtcNow;
        LockoutEnd = LockDate + offset;
        return this;
    }
    public UserRecord Enable()
    {
        LockDate = default;
        IsActive = true;
        return Unlock();
    }
    public UserRecord Unlock()
    {
        BadLogins      = 0;
        IsDisabled     = BadLogins > 5;
        LastBadAttempt = default;
        LockDate       = default;
        LockoutEnd     = default;
        return this;
    }

    #endregion



    #region Updaters

    public UserRecord Update( IDictionary<string, JToken?>? value )
    {
        if ( value is null ) { return this; }

        JsonModels.IJsonStringModel  model = this;
        IDictionary<string, JToken?> data  = model.GetData();
        foreach ( (string? key, JToken? jToken) in value ) { data[key] = jToken; }

        model.SetAdditionalData( data );
        return this;
    }
    public UserRecord Update( IUserData value )
    {
        FirstName         = value.FirstName;
        LastName          = value.LastName;
        FullName          = value.FullName;
        Address           = value.Address;
        Line1             = value.Line1;
        Line2             = value.Line2;
        City              = value.City;
        State             = value.State;
        Country           = value.Country;
        PostalCode        = value.PostalCode;
        Description       = value.Description;
        Website           = value.Website;
        Email             = value.Email;
        PhoneNumber       = value.PhoneNumber;
        Ext               = value.Ext;
        Title             = value.Title;
        Department        = value.Department;
        Company           = value.Company;
        PreferredLanguage = value.PreferredLanguage;
        return Update( value.AdditionalData );
    }
    IUserData IUserData.Update( IUserData value ) => Update( value );

    #endregion



    #region Tokens

    public UserRecord ClearRefreshToken()
    {
        RefreshToken           = default;
        RefreshTokenExpiryTime = default;
        return this;
    }
    public UserRecord SetRefreshToken( string token, DateTimeOffset date )
    {
        RefreshToken           = token;
        RefreshTokenExpiryTime = date;
        return this;
    }
    public UserRecord SetRefreshToken( string token, DateTimeOffset date, string securityStamp )
    {
        RefreshToken           = token;
        RefreshTokenExpiryTime = date;
        SecurityStamp          = securityStamp;
        return this;
    }

    #endregion



    #region Roles

    [RequiresPreviewFeatures]
    public async ValueTask<bool> TryAdd( DbConnection connection, DbTransaction transaction, Database db, RoleRecord value, CancellationToken token ) =>
        await UserRoleRecord.TryAdd( connection, transaction, db.UserRoles, this, value, token );
    public async ValueTask<RoleRecord[]> GetRoles( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token = default ) =>
        await UserRoleRecord.Where( connection, transaction, db.UserRoles, db.Roles, this, token );
    public async ValueTask<bool> HasRole( DbConnection connection, DbTransaction transaction, Database db, RoleRecord value, CancellationToken token ) =>
        await UserRoleRecord.Exists( connection, transaction, db.UserRoles, this, value, token );
    public async ValueTask Remove( DbConnection connection, DbTransaction transaction, Database db, RoleRecord value, CancellationToken token ) =>
        await UserRoleRecord.Delete( connection, transaction, db.UserRoles, this, value, token );

    #endregion



    #region Groups

    [RequiresPreviewFeatures]
    public async ValueTask<bool> TryAdd( DbConnection connection, DbTransaction transaction, Database db, GroupRecord value, CancellationToken token ) =>
        await UserGroupRecord.TryAdd( connection, transaction, db.UserGroups, this, value, token );
    public async ValueTask<GroupRecord[]> GetGroups( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token = default ) =>
        await UserGroupRecord.Where( connection, transaction, db.UserGroups, db.Groups, this, token );
    public async ValueTask<bool> IsPartOfGroup( DbConnection connection, DbTransaction transaction, Database db, GroupRecord value, CancellationToken token ) =>
        await UserGroupRecord.Exists( connection, transaction, db.UserGroups, this, value, token );
    public async ValueTask Remove( DbConnection connection, DbTransaction transaction, Database db, GroupRecord value, CancellationToken token ) =>
        await UserGroupRecord.Delete( connection, transaction, db.UserGroups, this, value, token );

    #endregion



    /*
    #region Rights

    public void RemoveRight( int index, char right = '-' ) => SetRight( index, right );
    public bool HasRight( int    index, char right = '+' ) => Rights[index] == right;
    public void AddRight( int    index, char right = '+' ) => SetRight( index, right );
    public void SetRight( int index, char right )
    {
        Span<char> rights = stackalloc char[Rights.Length];
        Rights.CopyTo( rights );
        rights[index] = right;
        Rights        = rights.ToString();
    }
    public void InitRights( int length, char right = '-' ) => Rights = new string( right, length );
   
    #endregion 
    */



    #region Claims

    public async ValueTask<List<Claim>> GetUserClaims( DbConnection connection, DbTransaction? transaction, Database db, ClaimType types, CancellationToken token )
    {
        GroupRecord[] groups = Array.Empty<GroupRecord>();
        RoleRecord[]  roles  = Array.Empty<RoleRecord>();


        if ( types.HasFlag( ClaimType.Groups ) ) { groups = await GetGroups( connection, transaction, db, token ); }

        if ( types.HasFlag( ClaimType.Roles ) ) { roles = await GetRoles( connection, transaction, db, token ); }


        var claims = new List<Claim>( 16 + groups.Length + roles.Length );

        if ( types.HasFlag( ClaimType.UserID ) ) { claims.Add( new Claim( ClaimTypes.Sid, UserID.ToString(), ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.UserName ) ) { claims.Add( new Claim( ClaimTypes.NameIdentifier, UserName, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.FirstName ) ) { claims.Add( new Claim( ClaimTypes.GivenName, FirstName, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.LastName ) ) { claims.Add( new Claim( ClaimTypes.Surname, LastName, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.FullName ) ) { claims.Add( new Claim( ClaimTypes.Name, FullName ?? string.Empty, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.Gender ) ) { claims.Add( new Claim( ClaimTypes.Gender, Gender ?? string.Empty, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.SubscriptionExpiration ) ) { claims.Add( new Claim( ClaimTypes.Expiration, SubscriptionExpires?.ToString() ?? string.Empty, ClaimValueTypes.DateTime ) ); }

        if ( types.HasFlag( ClaimType.Expired ) ) { claims.Add( new Claim( ClaimTypes.Expired, (SubscriptionExpires > DateTimeOffset.UtcNow).ToString(), ClaimValueTypes.Boolean ) ); }

        if ( types.HasFlag( ClaimType.Email ) ) { claims.Add( new Claim( ClaimTypes.Email, Email ?? string.Empty, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.MobilePhone ) ) { claims.Add( new Claim( ClaimTypes.MobilePhone, PhoneNumber ?? string.Empty, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.StreetAddress ) ) { claims.Add( new Claim( ClaimTypes.StreetAddress, Line1 ?? string.Empty, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.Locality ) ) { claims.Add( new Claim( ClaimTypes.Locality, Line2 ?? string.Empty, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.State ) ) { claims.Add( new Claim( ClaimTypes.Country, State ?? string.Empty, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.Country ) ) { claims.Add( new Claim( ClaimTypes.Country, Country ?? string.Empty, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.PostalCode ) ) { claims.Add( new Claim( ClaimTypes.PostalCode, PostalCode ?? string.Empty, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.WebSite ) ) { claims.Add( new Claim( ClaimTypes.Webpage, Website ?? string.Empty, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.Groups ) ) { claims.AddRange( from record in groups select new Claim( ClaimTypes.GroupSid, record.NameOfGroup, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.Roles ) ) { claims.AddRange( from record in roles select new Claim( ClaimTypes.Role, record.Name, ClaimValueTypes.String ) ); }

        return claims;
    }

    #endregion
}



[Flags]
public enum ClaimType : long
{
    None                                                                         = 1 << 0,
    [Display( Description = ClaimTypes.Sid )]             UserID                 = 1 << 1,
    [Display( Description = ClaimTypes.NameIdentifier )]  UserName               = 1 << 2,
    [Display( Description = ClaimTypes.GivenName )]       FirstName              = 1 << 3,
    [Display( Description = ClaimTypes.Surname )]         LastName               = 1 << 4,
    [Display( Description = ClaimTypes.Name )]            FullName               = 1 << 5,
    [Display( Description = ClaimTypes.Gender )]          Gender                 = 1 << 7,
    [Display( Description = ClaimTypes.Expiration )]      SubscriptionExpiration = 1 << 8,
    [Display( Description = ClaimTypes.Expiration )]      Expired                = 1 << 9,
    [Display( Description = ClaimTypes.Email )]           Email                  = 1 << 10,
    [Display( Description = ClaimTypes.MobilePhone )]     MobilePhone            = 1 << 11,
    [Display( Description = ClaimTypes.StreetAddress )]   StreetAddress          = 1 << 12,
    [Display( Description = ClaimTypes.Locality )]        Locality               = 1 << 12,
    [Display( Description = ClaimTypes.StateOrProvince )] State                  = 1 << 13,
    [Display( Description = ClaimTypes.Country )]         Country                = 1 << 14,
    [Display( Description = ClaimTypes.PostalCode )]      PostalCode             = 1 << 15,
    [Display( Description = ClaimTypes.Webpage )]         WebSite                = 1 << 16,
    [Display( Description = ClaimTypes.GroupSid )]        Groups                 = 1 << 17,
    [Display( Description = ClaimTypes.Role )]            Roles                  = 1 << 18,
    All                                                                          = ~0
}
