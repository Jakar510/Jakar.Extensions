// ToothFairyDispatch :: ToothFairyDispatch.Cloud
// 08/29/2022  9:55 PM

namespace Jakar.Database;


[Serializable]
[Table( "Users" )]
public sealed partial record UserRecord : TableRecord<UserRecord>, IUserRecord<UserRecord>
{
    public UserRecord() { }
    public UserRecord( IUserData value, long rights = default )
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
        Rights            = rights;
        DateCreated       = DateTimeOffset.UtcNow;
        UserID            = Guid.NewGuid();
    }
    public UserRecord( IUserData value, UserRecord caller, long rights = default ) : base( caller )
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
        DateCreated       = DateTimeOffset.UtcNow;
        Rights            = rights;
    }
    public UserRecord( long id, long rights = default ) : base( id )
    {
        UserID      = Guid.NewGuid();
        DateCreated = DateTimeOffset.UtcNow;
        Rights      = rights;
    }


    private static readonly PasswordHasher<UserRecord> _hasher = new();


    public async ValueTask Add( DbConnection connection, DbTransaction transaction, Database db, RoleRecord value, CancellationToken token )
    {
        UserRoleRecord? record = await db.UserRoles.Get( connection, transaction, true, UserRoleRecord.GetDynamicParameters( this, value ), token );

        if ( record is null )
        {
            record = new UserRoleRecord( this, value );
            await db.UserRoles.Insert( connection, transaction, record, token );
        }
    }
    public async ValueTask Add( DbConnection connection, DbTransaction transaction, Database db, GroupRecord value, CancellationToken token )
    {
        UserGroupRecord? record = await db.UserGroups.Get( connection, transaction, true, UserGroupRecord.GetDynamicParameters( this, value ), token );

        if ( record is null )
        {
            record = new UserGroupRecord( this, value );
            await db.UserGroups.Insert( connection, transaction, record, token );
        }
    }
    public UserRecord ClearRefreshToken( string securityStamp )
    {
        RefreshToken           = default;
        RefreshTokenExpiryTime = default;
        SecurityStamp          = securityStamp;
        return this;
    }


    public static UserRecord Create<TUser>( VerifyRequest<TUser> request, long rights = default ) where TUser : IUserData
    {
        ArgumentNullException.ThrowIfNull( request.Data );

        var record = new UserRecord( request.Data, rights )
                     {
                         UserName = request.UserLogin
                     };

        record.UpdatePassword( request.UserPassword );
        return record;
    }
    public static UserRecord Create<TUser>( VerifyRequest<TUser> request, UserRecord caller, long rights = default ) where TUser : IUserData
    {
        ArgumentNullException.ThrowIfNull( request.Data );

        var record = new UserRecord( request.Data, caller, rights )
                     {
                         UserName = request.UserLogin
                     };

        record.UpdatePassword( request.UserPassword );
        return record;
    }
    public static UserRecord Create<TUser, TRights>( VerifyRequest<TUser> request, TRights rights = default ) where TUser : IUserData
                                                                                                              where TRights : struct, Enum
    {
        ArgumentNullException.ThrowIfNull( request.Data );

        var record = new UserRecord( request.Data, rights.AsLong() )
                     {
                         UserName = request.UserLogin
                     };

        record.UpdatePassword( request.UserPassword );
        return record;
    }
    public static UserRecord Create<TUser, TRights>( VerifyRequest<TUser> request, UserRecord caller, TRights rights = default ) where TUser : IUserData
                                                                                                                                 where TRights : struct, Enum
    {
        ArgumentNullException.ThrowIfNull( request.Data );

        var record = new UserRecord( request.Data, caller, rights.AsLong() )
                     {
                         UserName = request.UserLogin
                     };

        record.UpdatePassword( request.UserPassword );
        return record;
    }
    public static UserRecord Create<TRights>( IUserData value, TRights    rights ) where TRights : struct, Enum => new(value, rights.AsLong());
    public static UserRecord Create<TRights>( IUserData value, UserRecord caller, TRights rights ) where TRights : struct, Enum => new(value, caller, rights.AsLong());


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



    #region Passwords

    public bool HasPassword() => !string.IsNullOrWhiteSpace( PasswordHash );
    public UserRecord UpdatePassword( string password )
    {
        PasswordHash = _hasher.HashPassword( this, password );
        return this;
    }
    public PasswordVerificationResult VerifyPassword( string password ) => _hasher.VerifyHashedPassword( this, PasswordHash, password );

    #endregion



    #region Owners

    public async ValueTask<UserRecord?> GetBoss( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => EscalateTo.HasValue
                                                                                                                                                    ? await db.Users.Get( connection, transaction, EscalateTo.Value, token )
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

    public async IAsyncEnumerable<RoleRecord> GetRoles( DbConnection connection, DbTransaction? transaction, Database db, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( UserRoleRecord record in await db.UserRoles.Where( connection, transaction, true, UserRoleRecord.GetDynamicParameters( this ), token ) )
        {
            RoleRecord? role = await db.Roles.Get( connection, transaction, record.RoleID, token );
            if ( role is not null ) { yield return role; }
        }
    }
    public async ValueTask<bool> HasRole( DbConnection connection, DbTransaction transaction, Database db, RoleRecord value, CancellationToken token )
    {
        UserRoleRecord? record = await db.UserRoles.Get( connection, transaction, true, UserRoleRecord.GetDynamicParameters( this, value ), token );
        return record is not null;
    }
    public async ValueTask Remove( DbConnection connection, DbTransaction transaction, Database db, RoleRecord role, CancellationToken token )
    {
        UserRoleRecord? record = await db.UserRoles.Get( connection, transaction, true, UserRoleRecord.GetDynamicParameters( this, role ), token );

        if ( record is not null ) { await db.UserRoles.Delete( connection, transaction, record, token ); }
    }

    #endregion



    #region Groups

    public async IAsyncEnumerable<GroupRecord> GetGroups( DbConnection connection, DbTransaction? transaction, Database db, [EnumeratorCancellation] CancellationToken token = default )
    {
        foreach ( UserGroupRecord record in await db.UserGroups.Where( connection, transaction, true, UserGroupRecord.GetDynamicParameters( this ), token ) )
        {
            GroupRecord? role = await db.Groups.Get( connection, transaction, record.GroupID, token );
            if ( role is not null ) { yield return role; }
        }
    }
    public async ValueTask<bool> IsPartOfGroup( DbConnection connection, DbTransaction transaction, Database db, GroupRecord value, CancellationToken token )
    {
        UserGroupRecord? record = await db.UserGroups.Get( connection, transaction, true, UserGroupRecord.GetDynamicParameters( this, value ), token );
        return record is not null;
    }
    public async ValueTask Remove( DbConnection connection, DbTransaction transaction, Database db, GroupRecord role, CancellationToken token )
    {
        UserGroupRecord? record = await db.UserGroups.Get( connection, transaction, true, UserGroupRecord.GetDynamicParameters( this, role ), token );

        if ( record is not null ) { await db.UserGroups.Delete( connection, transaction, record, token ); }
    }

    #endregion



    #region Rights

    public void RemoveRight<TRights>( TRights right ) where TRights : struct, Enum => Rights &= right.AsLong();
    public bool HasRight<TRights>( TRights    right ) where TRights : struct, Enum => (Rights & right.AsLong()) > 0;
    public void AddRight<TRights>( TRights    right ) where TRights : struct, Enum => Rights |= right.AsLong();
    public void SetRights<TRights>( TRights   rights ) where TRights : struct, Enum => Rights = rights.AsLong();

    #endregion



    #region Claims

    public List<Claim> GetUserClaims() => new()
                                          {
                                              new Claim( ClaimTypes.Sid,            UserID.ToString() ),
                                              new Claim( ClaimTypes.Dsa,            UserID.ToString() ),
                                              new Claim( ClaimTypes.NameIdentifier, UserName ),
                                              new Claim( ClaimTypes.Name,           FullName ?? string.Empty ),
                                              new Claim( ClaimTypes.MobilePhone,    PhoneNumber ?? string.Empty ),
                                              new Claim( ClaimTypes.Email,          Email ?? string.Empty ),
                                              new Claim( ClaimTypes.StreetAddress,  Address ?? string.Empty ),
                                              new Claim( ClaimTypes.Country,        Country ?? string.Empty ),
                                              new Claim( ClaimTypes.PostalCode,     PostalCode ?? string.Empty ),
                                              new Claim( ClaimTypes.Webpage,        Website ?? string.Empty ),
                                              new Claim( ClaimTypes.Expiration,     SubscriptionExpires?.ToString() ?? string.Empty )
                                          };
    public async ValueTask<List<Claim>> GetUserClaims( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token, bool includeRoles = true, bool includeGroups = false )
    {
        List<Claim> claims = GetUserClaims();

        if ( includeRoles )
        {
            await foreach ( RoleRecord record in GetRoles( connection, transaction, db, token ) ) { claims.Add( new Claim( ClaimTypes.Role, record.Name ) ); }
        }

        if ( !includeGroups ) { return claims; }

        await foreach ( GroupRecord record in GetGroups( connection, transaction, db, token ) ) { claims.Add( new Claim( ClaimTypes.GroupSid, record.NameOfGroup ) ); }

        return claims;
    }

    #endregion
}
