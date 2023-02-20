﻿// ToothFairyDispatch :: ToothFairyDispatch.Cloud
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
    public UserRecord( IUserData value, UserRights? rights = default ) : this( Guid.NewGuid(), rights )
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
        UserID            = Guid.NewGuid();
    }
    public UserRecord( IUserData value, UserRecord caller, UserRights? rights = default ) : base( caller )
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
        Rights            = rights?.ToString() ?? string.Empty;
    }
    public UserRecord( string id, UserRights? rights = default ) : base( id )
    {
        UserID = Guid.NewGuid();
        Rights = rights?.ToString() ?? string.Empty;
    }
    public UserRecord( Guid id, UserRights? rights = default ) : this( id.ToBase64(), rights ) { }
    public UserRecord( UserRecord caller, UserRights? rights = default ) : base( Guid.NewGuid(), caller )
    {
        UserID = Guid.NewGuid();
        Rights = rights?.ToString() ?? string.Empty;
    }
    public UserRecord( UserRecord caller, string id, UserRights? rights = default ) : base( id, caller )
    {
        UserID = Guid.NewGuid();
        Rights = rights?.ToString() ?? string.Empty;
    }


    private static readonly PasswordHasher<UserRecord> _hasher = new();


    public static UserRecord Create<TUser>( VerifyRequest<TUser> request, UserRights? rights = default ) where TUser : IUserData
    {
        ArgumentNullException.ThrowIfNull( request.Data );

        var record = new UserRecord( request.Data, rights )
                     {
                         UserName = request.UserLogin
                     };

        record.UpdatePassword( request.UserPassword );
        return record;
    }
    public static UserRecord Create<TUser>( VerifyRequest<TUser> request, UserRecord caller, UserRights? rights = default ) where TUser : IUserData

    {
        ArgumentNullException.ThrowIfNull( request.Data );

        var record = new UserRecord( request.Data, caller, rights )
                     {
                         UserName = request.UserLogin
                     };

        record.UpdatePassword( request.UserPassword );
        return record;
    }
    public static UserRecord Create( IUserData value, UserRights? rights ) => new(value, rights);
    public static UserRecord Create( IUserData value, UserRecord  caller, UserRights? rights ) => new(value, caller, rights);


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

    public async ValueTask<RoleRecord[]> GetRoles( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token = default ) =>
        await UserRoleRecord.Where( connection, transaction, db.UserRoles, db.Roles, this, token );
    public async ValueTask<bool> HasRole( DbConnection connection, DbTransaction transaction, Database db, RoleRecord value, CancellationToken token ) =>
        await UserRoleRecord.Exists( connection, transaction, db.UserRoles, this, value, token );
    public async ValueTask Remove( DbConnection connection, DbTransaction transaction, Database db, RoleRecord value, CancellationToken token ) =>
        await UserRoleRecord.Delete( connection, transaction, db.UserRoles, this, value, token );

    #endregion



    #region Groups

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

    public List<Claim> GetUserClaims( bool includePersonalData = false )
    {
        var claims = new List<Claim>( 25 )
                     {
                         new(ClaimTypes.Sid, UserID.ToString()),
                         new(ClaimTypes.Dsa, UserID.ToString()),
                         new(ClaimTypes.NameIdentifier, UserName),
                         new(ClaimTypes.Name, FullName ?? string.Empty),
                         new(ClaimTypes.Expiration, SubscriptionExpires?.ToString() ?? string.Empty)
                     };

        if ( !includePersonalData ) { return claims; }

        claims.Add( new Claim( ClaimTypes.MobilePhone,   PhoneNumber ?? string.Empty ) );
        claims.Add( new Claim( ClaimTypes.Email,         Email ?? string.Empty ) );
        claims.Add( new Claim( ClaimTypes.StreetAddress, Address ?? string.Empty ) );
        claims.Add( new Claim( ClaimTypes.Country,       Country ?? string.Empty ) );
        claims.Add( new Claim( ClaimTypes.PostalCode,    PostalCode ?? string.Empty ) );
        claims.Add( new Claim( ClaimTypes.Webpage,       Website ?? string.Empty ) );
        return claims;
    }
    public async ValueTask<List<Claim>> GetUserClaims( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token, bool includeRoles = true, bool includeGroups = true, bool includePersonalData = false )
    {
        List<Claim> claims = GetUserClaims( includePersonalData );

        if ( includeRoles ) { claims.AddRange( from record in await GetRoles( connection, transaction, db, token ) select new Claim( ClaimTypes.Role, record.Name ) ); }

        if ( includeGroups ) { claims.AddRange( from record in await GetGroups( connection, transaction, db, token ) select new Claim( ClaimTypes.GroupSid, record.NameOfGroup ) ); }

        return claims;
    }

    #endregion
}
