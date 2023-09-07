﻿// ToothFairyDispatch :: ToothFairyDispatch.Cloud
// 08/29/2022  9:55 PM

using Microsoft.AspNetCore.Http;



namespace Jakar.Database;


[Serializable]
[Table( "Users" )]
public sealed partial record UserRecord : TableRecord<UserRecord>, IDbReaderMapping<UserRecord>, JsonModels.IJsonStringModel, IRefreshToken, IUserID, IUserDataRecord, IUserSubscription, UserRights.IRights
{
    private static readonly PasswordHasher<UserRecord> _hasher = new();


    public UserRecord() { }
    public UserRecord( IUserData data, string? rights, UserRecord? caller = default ) : base( caller )
    {
        ArgumentNullException.ThrowIfNull( data );
        FirstName         = data.FirstName;
        LastName          = data.LastName;
        FullName          = data.FullName;
        Address           = data.Address;
        Line1             = data.Line1;
        Line2             = data.Line2;
        City              = data.City;
        StateOrProvince   = data.StateOrProvince;
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
        UserID            = Guid.NewGuid();
    }
    public UserRecord( Guid id, UserRecord? caller = default ) : base( new RecordID<UserRecord>( id ), caller ) => UserID = Guid.NewGuid();
    public UserRecord( string userName, string password, string rights, UserRecord? caller = default ) : base( caller )
    {
        ArgumentNullException.ThrowIfNull( userName );
        ArgumentNullException.ThrowIfNull( password );
        UserID   = Guid.NewGuid();
        UserName = userName;
        Rights   = rights;
        UpdatePassword( password );
    }

    public static UserRecord Create( DbDataReader reader )
    {
        return new UserRecord
               {
                   UserID                 = reader.GetFieldValue<Guid>( nameof(IsEmailConfirmed) ),
                   UserName               = reader.GetString( nameof(UserName) ),
                   FullName               = reader.GetString( nameof(FullName) ),
                   FirstName              = reader.GetString( nameof(FirstName) ),
                   LastName               = reader.GetString( nameof(LastName) ),
                   Line1                  = reader.GetString( nameof(Line1) ),
                   Line2                  = reader.GetString( nameof(Line2) ),
                   City                   = reader.GetString( nameof(City) ),
                   Country                = reader.GetString( nameof(Country) ),
                   PostalCode             = reader.GetString( nameof(PostalCode) ),
                   PhoneNumber            = reader.GetString( nameof(PhoneNumber) ),
                   Address                = reader.GetString( nameof(Address) ),
                   IsPhoneNumberConfirmed = reader.GetFieldValue<bool>( nameof(IsPhoneNumberConfirmed) ),
                   Email                  = reader.GetString( nameof(Email) ),
                   IsEmailConfirmed       = reader.GetFieldValue<bool>( nameof(IsEmailConfirmed) ),
                   BadLogins              = reader.GetFieldValue<int>( nameof(BadLogins) ),
                   LastBadAttempt         = reader.GetFieldValue<DateTimeOffset>( nameof(LastBadAttempt) ),
                   Company                = reader.GetString( nameof(Company) ),
                   Department             = reader.GetString( nameof(Department) ),
                   Title                  = reader.GetString( nameof(Title) ),
                   DateCreated            = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) ),
                   LastModified           = reader.GetFieldValue<DateTimeOffset>( nameof(LastModified) ),
                   OwnerUserID            = reader.GetFieldValue<Guid?>( nameof(OwnerUserID) ),
                   CreatedBy              = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) ),
                   ID                     = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) ),
               };
    }
    public static async IAsyncEnumerable<UserRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    public static UserRecord Create<TUser>( VerifyRequest<TUser> request, string rights, UserRecord? caller = default ) where TUser : IUserData

    {
        ArgumentNullException.ThrowIfNull( request.Data );

        UserRecord record = Create( request.Data, rights, caller );
        record.UserName = request.UserLogin;
        record.UpdatePassword( request.UserPassword );
        return record.Enable();
    }
    public static UserRecord Create( IUserData value,    string rights,   UserRecord? caller                     = default ) => new(value, rights, caller);
    public static UserRecord Create( string    userName, string password, string      rights, UserRecord? caller = default ) => new(userName, password, rights, caller);


    public static DynamicParameters GetDynamicParameters( IUserData data )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(Email),     data.Email );
        parameters.Add( nameof(FirstName), data.FirstName );
        parameters.Add( nameof(LastName),  data.LastName );
        parameters.Add( nameof(FullName),  data.FullName );
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
        RefreshToken           = string.Empty;
        RefreshTokenExpiryTime = default;
        SecurityStamp          = securityStamp;
        return this;
    }


    [RequiresPreviewFeatures] public ValueTask<bool> RedeemCode( Database db, string code, CancellationToken token ) => db.TryCall( RedeemCode, db, code, token );
    [RequiresPreviewFeatures]
    public async ValueTask<bool> RedeemCode( DbConnection connection, DbTransaction transaction, Database db, string code, CancellationToken token )
    {
        IEnumerable<UserRecoveryCodeRecord> mappings = await UserRecoveryCodeRecord.Where( connection, transaction, db.UserRecoveryCodes, this, token );

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
        IEnumerable<RecoveryCodeRecord>                 old        = await Codes( connection, transaction, db, token );
        IReadOnlyDictionary<string, RecoveryCodeRecord> dictionary = RecoveryCodeRecord.Create( this, count );
        string[]                                        codes      = dictionary.Keys.GetArray();


        await db.RecoveryCodes.Delete( connection, transaction, old, token );
        await UserRecoveryCodeRecord.Replace( connection, transaction, db.UserRecoveryCodes, this, dictionary.Values, token );
        return codes;
    }


    [RequiresPreviewFeatures] public ValueTask<string[]> ReplaceCodes( Database db, IEnumerable<string> recoveryCodes, CancellationToken token = default ) => db.TryCall( ReplaceCodes, db, recoveryCodes, token );
    [RequiresPreviewFeatures]
    public async ValueTask<string[]> ReplaceCodes( DbConnection connection, DbTransaction transaction, Database db, IEnumerable<string> recoveryCodes, CancellationToken token = default )
    {
        IEnumerable<RecoveryCodeRecord>                 old        = await Codes( connection, transaction, db, token );
        IReadOnlyDictionary<string, RecoveryCodeRecord> dictionary = RecoveryCodeRecord.Create( this, recoveryCodes );
        string[]                                        codes      = dictionary.Keys.GetArray();


        await db.RecoveryCodes.Delete( connection, transaction, old, token );
        await UserRecoveryCodeRecord.Replace( connection, transaction, db.UserRecoveryCodes, this, dictionary.Values, token );
        return codes;
    }


    [RequiresPreviewFeatures] public ValueTask<IEnumerable<RecoveryCodeRecord>> Codes( Database db, CancellationToken token ) => db.TryCall( Codes, db, token );
    [RequiresPreviewFeatures]
    public async ValueTask<IEnumerable<RecoveryCodeRecord>> Codes( DbConnection connection, DbTransaction transaction, Database db, CancellationToken token ) =>
        await UserRecoveryCodeRecord.Where( connection, transaction, db.UserRecoveryCodes, db.RecoveryCodes, this, token );


    public UserRecord SetSubscription<TRecord>( TRecord record ) where TRecord : TableRecord<TRecord>, IUserSubscription
    {
        SubscriptionExpires = record.SubscriptionExpires;
        SubscriptionID      = record.SubscriptionID;
        return this;
    }
    public async ValueTask<TRecord?> GetSubscription<TRecord>( DbConnection connection, DbTransaction transaction, DbTable<TRecord> table, CancellationToken token ) where TRecord : TableRecord<TRecord>, IUserSubscription =>
        await table.Get( connection, transaction, SubscriptionID, token );


    public async Task<UserRights> GetRights( DbConnection connection, DbTransaction transaction, Database db, int totalRightCount, CancellationToken token )
    {
        GroupRecord[] groups = (await GetGroups( connection, transaction, db, token )).GetArray();
        RoleRecord[]  roles  = (await GetRoles( connection, transaction, db, token )).GetArray();
        var           rights = new List<UserRights.IRights>( 1 + groups.Length + roles.Length );

        rights.AddRange( groups );
        rights.AddRange( roles );
        rights.Add( this );

        return UserRights.Merge( rights, totalRightCount );
    }


    public bool Equals( IUserData? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( _address,        other.Address,         StringComparison.Ordinal ) &&
               string.Equals( _city,           other.City,            StringComparison.Ordinal ) &&
               string.Equals( _company,        other.Company,         StringComparison.Ordinal ) &&
               string.Equals( _country,        other.Country,         StringComparison.Ordinal ) &&
               string.Equals( _department,     other.Department,      StringComparison.Ordinal ) &&
               string.Equals( _description,    other.Description,     StringComparison.Ordinal ) &&
               string.Equals( _email,          other.Email,           StringComparison.Ordinal ) &&
               string.Equals( _ext,            other.Ext,             StringComparison.Ordinal ) &&
               string.Equals( _firstName,      other.FirstName,       StringComparison.Ordinal ) &&
               string.Equals( _fullName,       other.FullName,        StringComparison.Ordinal ) &&
               string.Equals( _lastName,       other.LastName,        StringComparison.Ordinal ) &&
               string.Equals( _line1,          other.Line1,           StringComparison.Ordinal ) &&
               string.Equals( _line2,          other.Line2,           StringComparison.Ordinal ) &&
               string.Equals( _phoneNumber,    other.PhoneNumber,     StringComparison.Ordinal ) &&
               string.Equals( _postalCode,     other.PostalCode,      StringComparison.Ordinal ) &&
               string.Equals( StateOrProvince, other.StateOrProvince, StringComparison.Ordinal ) &&
               string.Equals( _title,          other.Title,           StringComparison.Ordinal ) &&
               string.Equals( _website,        other.Website,         StringComparison.Ordinal ) &&
               _preferredLanguage == other.PreferredLanguage;
    }
    public int CompareTo( IUserData? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int firstNameComparison = string.Compare( _firstName, other.FirstName, StringComparison.Ordinal );
        if ( firstNameComparison != 0 ) { return firstNameComparison; }

        int lastNameComparison = string.Compare( _lastName, other.LastName, StringComparison.Ordinal );
        if ( lastNameComparison != 0 ) { return lastNameComparison; }

        int fullNameComparison = string.Compare( _fullName, other.FullName, StringComparison.Ordinal );
        if ( fullNameComparison != 0 ) { return fullNameComparison; }

        int descriptionComparison = string.Compare( _description, other.Description, StringComparison.Ordinal );
        if ( descriptionComparison != 0 ) { return descriptionComparison; }

        int companyComparison = string.Compare( _company, other.Company, StringComparison.Ordinal );
        if ( companyComparison != 0 ) { return companyComparison; }

        int departmentComparison = string.Compare( _department, other.Department, StringComparison.Ordinal );
        if ( departmentComparison != 0 ) { return departmentComparison; }

        int titleComparison = string.Compare( _title, other.Title, StringComparison.Ordinal );
        if ( titleComparison != 0 ) { return titleComparison; }

        int emailComparison = string.Compare( _email, other.Email, StringComparison.Ordinal );
        if ( emailComparison != 0 ) { return emailComparison; }

        int phoneNumberComparison = string.Compare( _phoneNumber, other.PhoneNumber, StringComparison.Ordinal );
        if ( phoneNumberComparison != 0 ) { return phoneNumberComparison; }

        int extComparison = string.Compare( _ext, other.Ext, StringComparison.Ordinal );
        if ( extComparison != 0 ) { return extComparison; }

        int websiteComparison = string.Compare( _website, other.Website, StringComparison.Ordinal );
        if ( websiteComparison != 0 ) { return websiteComparison; }

        int cityComparison = string.Compare( _city, other.City, StringComparison.Ordinal );
        if ( cityComparison != 0 ) { return cityComparison; }

        int countryComparison = string.Compare( _country, other.Country, StringComparison.Ordinal );
        if ( countryComparison != 0 ) { return countryComparison; }

        int line1Comparison = string.Compare( _line1, other.Line1, StringComparison.Ordinal );
        if ( line1Comparison != 0 ) { return line1Comparison; }

        int line2Comparison = string.Compare( _line2, other.Line2, StringComparison.Ordinal );
        if ( line2Comparison != 0 ) { return line2Comparison; }

        int postalCodeComparison = string.Compare( _postalCode, other.PostalCode, StringComparison.Ordinal );
        if ( postalCodeComparison != 0 ) { return postalCodeComparison; }

        int stateComparison = string.Compare( StateOrProvince, other.StateOrProvince, StringComparison.Ordinal );
        if ( stateComparison != 0 ) { return stateComparison; }

        int addressComparison = string.Compare( _address, other.Address, StringComparison.Ordinal );
        if ( addressComparison != 0 ) { return addressComparison; }

        return ((int)PreferredLanguage).CompareTo( (int)other.PreferredLanguage );
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
    public bool VerifyPassword( string password )
    {
        PasswordVerificationResult result = _hasher.VerifyHashedPassword( this, PasswordHash, password );

        switch ( result )
        {
            case PasswordVerificationResult.Failed:
            {
                MarkBadLogin();
                return false;
            }

            case PasswordVerificationResult.Success:
            {
                LastLogin = DateTimeOffset.UtcNow;
                return true;
            }

            case PasswordVerificationResult.SuccessRehashNeeded:
            {
                UpdatePassword( password );
                LastLogin = DateTimeOffset.UtcNow;
                return true;
            }

            default: throw new OutOfRangeException( nameof(result), result );
        }
    }

    #endregion



    #region Owners

    public async ValueTask<UserRecord?> GetBoss( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => EscalateTo.HasValue
                                                                                                                                                    ? await db.Users.Get( connection, transaction, EscalateTo.Value, token )
                                                                                                                                                    : default;


    public bool DoesNotOwn<TRecord>( TRecord record ) where TRecord : TableRecord<TRecord> => record.OwnerUserID != UserID;
    public bool Owns<TRecord>( TRecord       record ) where TRecord : TableRecord<TRecord> => record.OwnerUserID == UserID;

    #endregion



    #region Controls

    public UserRecord MarkBadLogin()
    {
        BadLogins      += 1;
        IsDisabled     =  BadLogins > 5;
        LastBadAttempt =  DateTimeOffset.UtcNow;

        return IsDisabled
                   ? Lock()
                   : Unlock();
    }
    public UserRecord SetActive( bool? isActive = default )
    {
        if ( isActive is not null ) { IsActive = isActive.Value; }

        return this;
    }


    public bool TryEnable()
    {
        if ( LockoutEnd.HasValue && DateTimeOffset.UtcNow <= LockoutEnd.Value ) { Enable(); }

        return !IsDisabled && IsActive;
    }
    public UserRecord Disable()
    {
        IsActive   = false;
        IsDisabled = true;
        return this;
    }
    public UserRecord Enable()
    {
        IsDisabled = false;
        IsActive   = true;
        return this;
    }


    public UserRecord Reset()
    {
        BadLogins = 0;

        return Unlock()
              .Enable()
              .SetActive( true );
    }


    public UserRecord Unlock()
    {
        IsLocked   = false;
        LockDate   = default;
        LockoutEnd = default;
        return this;
    }
    public UserRecord Lock() => Lock( TimeSpan.FromHours( 6 ) );
    public UserRecord Lock( in TimeSpan offset )
    {
        IsLocked   = true;
        LockDate   = DateTimeOffset.UtcNow;
        LockoutEnd = LockDate + offset;
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
        StateOrProvince   = value.StateOrProvince;
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
    void IUserData.Update( IUserData data ) => Update( data );

    #endregion



    #region Tokens

    public UserRecord ClearRefreshToken()
    {
        RefreshToken           = string.Empty;
        RefreshTokenExpiryTime = default;
        return this;
    }
    public bool IsHashedRefreshToken( string? token )
    {
        // ReSharper disable once InvertIf
        if ( RefreshTokenExpiryTime.HasValue && DateTimeOffset.UtcNow > RefreshTokenExpiryTime.Value )
        {
            ClearRefreshToken();
            return false;
        }


        return string.Equals( RefreshToken,
                              token?.GetHashCode()
                                    .ToString(),
                              StringComparison.Ordinal );
    }
    public bool IsHashedRefreshToken( Tokens token ) => IsHashedRefreshToken( token.RefreshToken );

    public UserRecord SetRefreshToken( string? token, in DateTimeOffset date, in bool hashed = true )
    {
        if ( hashed )
        {
            token = token?.GetHashCode()
                          .ToString();
        }

        RefreshToken           = token ?? string.Empty;
        RefreshTokenExpiryTime = date;
        return this;
    }
    public UserRecord SetRefreshToken( string? token, in DateTimeOffset date, string securityStamp, in bool hashed = true )
    {
        if ( hashed )
        {
            token = token?.GetHashCode()
                          .ToString();
        }

        RefreshToken           = token ?? string.Empty;
        RefreshTokenExpiryTime = date;
        SecurityStamp          = securityStamp;
        return this;
    }

    public UserRecord SetRefreshToken( Tokens token, in DateTimeOffset date, in bool hashed                        = true ) => SetRefreshToken( token.RefreshToken, date, hashed );
    public UserRecord SetRefreshToken( Tokens token, in DateTimeOffset date, string  securityStamp, in bool hashed = true ) => SetRefreshToken( token.RefreshToken, date, securityStamp, hashed );

    #endregion



    #region Roles

    [RequiresPreviewFeatures]
    public async ValueTask<bool> TryAdd( DbConnection connection, DbTransaction transaction, Database db, RoleRecord value, CancellationToken token ) =>
        await UserRoleRecord.TryAdd( connection, transaction, db.UserRoles, this, value, token );
    public async ValueTask<IEnumerable<RoleRecord>> GetRoles( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token = default ) =>
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
    public async ValueTask<IEnumerable<GroupRecord>> GetGroups( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token = default ) =>
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

    public async ValueTask<Claim[]> GetUserClaims( DbConnection connection, DbTransaction? transaction, Database db, ClaimType types, CancellationToken token )
    {
        GroupRecord[] groups = Array.Empty<GroupRecord>();
        RoleRecord[]  roles  = Array.Empty<RoleRecord>();


        if ( types.HasFlag( ClaimType.GroupSid ) ) { groups = (await GetGroups( connection, transaction, db, token )).GetArray(); }

        if ( types.HasFlag( ClaimType.Role ) ) { roles = (await GetRoles( connection, transaction, db, token )).GetArray(); }


        var claims = new List<Claim>( 16 + groups.Length + roles.Length );

        if ( types.HasFlag( ClaimType.UserID ) ) { claims.Add( new Claim( ClaimTypes.Sid, UserID.ToString(), ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.UserName ) ) { claims.Add( new Claim( ClaimTypes.NameIdentifier, UserName, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.FirstName ) ) { claims.Add( new Claim( ClaimTypes.GivenName, FirstName, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.LastName ) ) { claims.Add( new Claim( ClaimTypes.Surname, LastName, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.FullName ) ) { claims.Add( new Claim( ClaimTypes.Name, FullName, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.Gender ) ) { claims.Add( new Claim( ClaimTypes.Gender, Gender, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.SubscriptionExpiration ) ) { claims.Add( new Claim( ClaimTypes.Expiration, SubscriptionExpires?.ToString() ?? string.Empty, ClaimValueTypes.DateTime ) ); }

        if ( types.HasFlag( ClaimType.Expired ) ) { claims.Add( new Claim( ClaimTypes.Expired, (SubscriptionExpires > DateTimeOffset.UtcNow).ToString(), ClaimValueTypes.Boolean ) ); }

        if ( types.HasFlag( ClaimType.Email ) ) { claims.Add( new Claim( ClaimTypes.Email, Email, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.MobilePhone ) ) { claims.Add( new Claim( ClaimTypes.MobilePhone, PhoneNumber, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.StreetAddressLine1 ) ) { claims.Add( new Claim( ClaimTypes.StreetAddress, Line1, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.StreetAddressLine2 ) ) { claims.Add( new Claim( ClaimTypes.Locality, Line2, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.StateOrProvince ) ) { claims.Add( new Claim( ClaimTypes.Country, StateOrProvince, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.Country ) ) { claims.Add( new Claim( ClaimTypes.Country, Country, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.PostalCode ) ) { claims.Add( new Claim( ClaimTypes.PostalCode, PostalCode, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.WebSite ) ) { claims.Add( new Claim( ClaimTypes.Webpage, Website, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.GroupSid ) ) { claims.AddRange( from record in groups select new Claim( ClaimTypes.GroupSid, record.NameOfGroup, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.Role ) ) { claims.AddRange( from record in roles select new Claim( ClaimTypes.Role, record.Name, ClaimValueTypes.String ) ); }

        return claims.GetArray();
    }
    public static ValueTask<UserRecord?> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, HttpContext context, ClaimType types, CancellationToken token ) =>
        TryFromClaims( connection, transaction, db, context.User, types, token );
    public static ValueTask<UserRecord?> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, ClaimsPrincipal principal, ClaimType types, CancellationToken token ) =>
        TryFromClaims( connection, transaction, db, principal.Claims.GetArray(), types, token );
    public static async ValueTask<UserRecord?> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, Claim[] claims, ClaimType types, CancellationToken token )
    {
        Debug.Assert( types.HasFlag( ClaimType.UserID ) );
        Debug.Assert( types.HasFlag( ClaimType.UserName ) );


        var parameters = new DynamicParameters();

        parameters.Add( nameof(UserName),
                        claims.Single( x => x.Type == ClaimTypes.NameIdentifier )
                              .Value );

        parameters.Add( nameof(UserID),
                        Guid.Parse( claims.Single( x => x.Type == ClaimTypes.Sid )
                                          .Value ) );


        if ( types.HasFlag( ClaimType.FirstName ) )
        {
            parameters.Add( nameof(FirstName),
                            claims.Single( x => x.Type == ClaimTypes.GivenName )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.LastName ) )
        {
            parameters.Add( nameof(LastName),
                            claims.Single( x => x.Type == ClaimTypes.Surname )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.FullName ) )
        {
            parameters.Add( nameof(FullName),
                            claims.Single( x => x.Type == ClaimTypes.Name )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.SubscriptionExpiration ) )
        {
            parameters.Add( nameof(SubscriptionExpires),
                            claims.Single( x => x.Type == ClaimTypes.Expiration )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.Email ) )
        {
            parameters.Add( nameof(Email),
                            claims.Single( x => x.Type == ClaimTypes.Email )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.MobilePhone ) )
        {
            parameters.Add( nameof(PhoneNumber),
                            claims.Single( x => x.Type == ClaimTypes.MobilePhone )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.StreetAddressLine1 ) )
        {
            parameters.Add( nameof(Line1),
                            claims.Single( x => x.Type == ClaimTypes.StreetAddress )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.StreetAddressLine2 ) )
        {
            parameters.Add( nameof(Line2),
                            claims.Single( x => x.Type == ClaimTypes.Locality )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.StateOrProvince ) )
        {
            parameters.Add( nameof(StateOrProvince),
                            claims.Single( x => x.Type == ClaimTypes.StateOrProvince )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.Country ) )
        {
            parameters.Add( nameof(Country),
                            claims.Single( x => x.Type == ClaimTypes.Country )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.PostalCode ) )
        {
            parameters.Add( nameof(PostalCode),
                            claims.Single( x => x.Type == ClaimTypes.PostalCode )
                                  .Value );
        }

        if ( types.HasFlag( ClaimType.WebSite ) )
        {
            parameters.Add( nameof(Website),
                            claims.Single( x => x.Type == ClaimTypes.Webpage )
                                  .Value );
        }

        return await db.Users.Get( connection, transaction, true, parameters, token );
    }
    public static async ValueTask<IEnumerable<UserRecord>> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, Claim claim, CancellationToken token )
    {
        var parameters = new DynamicParameters();

        switch ( claim.Type )
        {
            case ClaimTypes.NameIdentifier:
                parameters.Add( nameof(UserName), claim.Value );
                break;

            case ClaimTypes.Sid:
                parameters.Add( nameof(UserID), Guid.Parse( claim.Value ) );
                break;

            case ClaimTypes.GivenName:
                parameters.Add( nameof(FirstName), claim.Value );
                break;

            case ClaimTypes.Surname:
                parameters.Add( nameof(LastName), claim.Value );
                break;

            case ClaimTypes.Name:
                parameters.Add( nameof(FullName), claim.Value );
                break;

            case ClaimTypes.Expiration:
                parameters.Add( nameof(SubscriptionExpires), claim.Value );
                break;

            case ClaimTypes.Email:
                parameters.Add( nameof(Email), claim.Value );
                break;

            case ClaimTypes.MobilePhone:
                parameters.Add( nameof(PhoneNumber), claim.Value );
                break;

            case ClaimTypes.StreetAddress:
                parameters.Add( nameof(Line1), claim.Value );
                break;

            case ClaimTypes.Locality:
                parameters.Add( nameof(Line2), claim.Value );
                break;

            case ClaimTypes.StateOrProvince:
                parameters.Add( nameof(StateOrProvince), claim.Value );
                break;

            case ClaimTypes.Country:
                parameters.Add( nameof(Country), claim.Value );
                break;

            case ClaimTypes.PostalCode:
                parameters.Add( nameof(PostalCode), claim.Value );
                break;

            case ClaimTypes.Webpage:
                parameters.Add( nameof(Website), claim.Value );
                break;
        }


        return await db.Users.Where( connection, transaction, true, parameters, token );
    }

    #endregion
}
