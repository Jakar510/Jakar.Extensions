// ToothFairyDispatch :: ToothFairyDispatch.Cloud
// 08/29/2022  9:55 PM

using Microsoft.AspNetCore.Http;



namespace Jakar.Database;


[ Serializable, Table( "Users" ) ]
public sealed partial record UserRecord( [ property: ProtectedPersonalData, MaxLength( int.MaxValue ) ] string?                                                           AdditionalData,
                                         [ property: ProtectedPersonalData, MaxLength( 4096 ) ]         string                                                            Address,
                                         int                                                                                                                              BadLogins,
                                         [ property: ProtectedPersonalData, MaxLength( 256 ) ]                                                      string                City,
                                         [ property: ProtectedPersonalData, MaxLength( 256 ) ]                                                      string                Company,
                                         [ property: MaxLength(                        TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] string                ConcurrencyStamp,
                                         [ property: ProtectedPersonalData, MaxLength( 256 ) ]                                                      string                Country,
                                         [ property: ProtectedPersonalData, MaxLength( 256 ) ]                                                      string                Department,
                                         [ property: ProtectedPersonalData, MaxLength( 256 ) ]                                                      string                Description,
                                         [ property: ProtectedPersonalData, MaxLength( 1024 ) ]                                                     string                Email,
                                         [ property: MaxLength(                        256 ) ]                                                      RecordID<UserRecord>? EscalateTo,
                                         [ property: ProtectedPersonalData, MaxLength( 256 ) ]                                                      string                Ext,
                                         [ property: ProtectedPersonalData, MaxLength( 256 ) ]                                                      string                FirstName,
                                         [ property: ProtectedPersonalData, MaxLength( 512 ) ]                                                      string                FullName,
                                         [ property: MaxLength(                        256 ) ]                                                      string                Gender,
                                         bool                                                                                                                             IsActive,
                                         bool                                                                                                                             IsDisabled,
                                         bool                                                                                                                             IsEmailConfirmed,
                                         bool                                                                                                                             IsLocked,
                                         bool                                                                                                                             IsPhoneNumberConfirmed,
                                         bool                                                                                                                             IsTwoFactorEnabled,
                                         DateTimeOffset?                                                                                                                  LastBadAttempt,
                                         DateTimeOffset?                                                                                                                  LastLogin,
                                         [ property: ProtectedPersonalData, MaxLength( 256 ) ] string                                                                     LastName,
                                         [ property: ProtectedPersonalData, MaxLength( 512 ) ] string                                                                     Line1,
                                         [ property: ProtectedPersonalData, MaxLength( 256 ) ] string                                                                     Line2,
                                         DateTimeOffset?                                                                                                                  LockDate,
                                         DateTimeOffset?                                                                                                                  LockoutEnd,
                                         [ property: MaxLength(                        TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] string                PasswordHash,
                                         [ property: ProtectedPersonalData, MaxLength( 256 ) ]                                                      string                PhoneNumber,
                                         [ property: ProtectedPersonalData, MaxLength( 256 ) ]                                                      string                PostalCode,
                                         SupportedLanguage                                                                                                                PreferredLanguage,
                                         [ property: MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] string                                       RefreshToken,
                                         DateTimeOffset?                                                                                                                  RefreshTokenExpiryTime,
                                         [ property: MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] string                                       Rights,
                                         [ property: MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] string                                       SecurityStamp,
                                         Guid?                                                                                                                            SessionID,
                                         [ property: ProtectedPersonalData, MaxLength( 256 ) ] string                                                                     StateOrProvince,
                                         DateTimeOffset?                                                                                                                  SubscriptionExpires,
                                         [ property: MaxLength(                        256 ) ] Guid?                                                                      SubscriptionID,
                                         [ property: ProtectedPersonalData, MaxLength( 256 ) ] string                                                                     Title,
                                         DateTimeOffset?                                                                                                                  TokenExpiration,
                                         Guid                                                                                                                             UserID,
                                         [ property: ProtectedPersonalData, MaxLength( 256 ) ]  string                                                                    UserName,
                                         [ property: ProtectedPersonalData, MaxLength( 4096 ) ] string                                                                    Website,
                                         RecordID<UserRecord>                                                                                                             ID,
                                         RecordID<UserRecord>?                                                                                                            CreatedBy,
                                         Guid?                                                                                                                            OwnerUserID,
                                         DateTimeOffset                                                                                                                   DateCreated,
                                         DateTimeOffset?                                                                                                                  LastModified = default
) : TableRecord<UserRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<UserRecord>, JsonModels.IJsonStringModel, IRefreshToken, IUserID, IUserDataRecord, IUserSubscription, UserRights.IRights
{
    private static readonly PasswordHasher<UserRecord> _hasher = new();


    public UserRecord( IUserData data, string? rights, UserRecord? caller = default ) : this( caller )
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
    public UserRecord( Guid id, UserRecord? caller = default ) : this( new RecordID<UserRecord>( id ), caller ) => UserID = Guid.NewGuid();
    public UserRecord( string userName, string password, string rights, UserRecord? caller = default ) : this( caller )
    {
        ArgumentNullException.ThrowIfNull( userName );
        ArgumentNullException.ThrowIfNull( password );
        UserID   = Guid.NewGuid();
        UserName = userName;
        Rights   = rights;
        UpdatePassword( password );
    }


    // [DbReaderMapping]
    public static UserRecord Create( DbDataReader reader )
    {
        var userID                 = reader.GetFieldValue<Guid>( nameof(IsEmailConfirmed) );
        var userName               = reader.GetString( nameof(UserName) );
        var fullName               = reader.GetString( nameof(FullName) );
        var firstName              = reader.GetString( nameof(FirstName) );
        var lastName               = reader.GetString( nameof(LastName) );
        var line1                  = reader.GetString( nameof(Line1) );
        var line2                  = reader.GetString( nameof(Line2) );
        var city                   = reader.GetString( nameof(City) );
        var country                = reader.GetString( nameof(Country) );
        var postalCode             = reader.GetString( nameof(PostalCode) );
        var phoneNumber            = reader.GetString( nameof(PhoneNumber) );
        var address                = reader.GetString( nameof(Address) );
        var isPhoneNumberConfirmed = reader.GetFieldValue<bool>( nameof(IsPhoneNumberConfirmed) );
        var email                  = reader.GetString( nameof(Email) );
        var isEmailConfirmed       = reader.GetFieldValue<bool>( nameof(IsEmailConfirmed) );
        var badLogins              = reader.GetFieldValue<int>( nameof(BadLogins) );
        var lastBadAttempt         = reader.GetFieldValue<DateTimeOffset>( nameof(LastBadAttempt) );
        var company                = reader.GetString( nameof(Company) );
        var department             = reader.GetString( nameof(Department) );
        var title                  = reader.GetString( nameof(Title) );
        var dateCreated            = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified           = reader.GetFieldValue<DateTimeOffset>( nameof(LastModified) );
        var ownerUserID            = reader.GetFieldValue<Guid?>( nameof(OwnerUserID) );
        var createdBy              = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );
        var id                     = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        return new UserRecord( id, createdBy, ownerUserID, dateCreated, lastModified );
    }
    public static async IAsyncEnumerable<UserRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
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


    [ RequiresPreviewFeatures ] public ValueTask<bool> RedeemCode( Database db, string code, CancellationToken token ) => db.TryCall( RedeemCode, db, code, token );
    [ RequiresPreviewFeatures ]
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


    [ RequiresPreviewFeatures ] public ValueTask<string[]> ReplaceCodes( Database db, int count = 10, CancellationToken token = default ) => db.TryCall( ReplaceCodes, db, count, token );
    [ RequiresPreviewFeatures ]
    public async ValueTask<string[]> ReplaceCodes( DbConnection connection, DbTransaction transaction, Database db, int count = 10, CancellationToken token = default )
    {
        IEnumerable<RecoveryCodeRecord>                 old        = await Codes( connection, transaction, db, token );
        IReadOnlyDictionary<string, RecoveryCodeRecord> dictionary = RecoveryCodeRecord.Create( this, count );
        string[]                                        codes      = dictionary.Keys.GetArray();


        await db.RecoveryCodes.Delete( connection, transaction, old, token );
        await UserRecoveryCodeRecord.Replace( connection, transaction, db.UserRecoveryCodes, this, dictionary.Values, token );
        return codes;
    }


    [ RequiresPreviewFeatures ] public ValueTask<string[]> ReplaceCodes( Database db, IEnumerable<string> recoveryCodes, CancellationToken token = default ) => db.TryCall( ReplaceCodes, db, recoveryCodes, token );
    [ RequiresPreviewFeatures ]
    public async ValueTask<string[]> ReplaceCodes( DbConnection connection, DbTransaction transaction, Database db, IEnumerable<string> recoveryCodes, CancellationToken token = default )
    {
        IEnumerable<RecoveryCodeRecord>                 old        = await Codes( connection, transaction, db, token );
        IReadOnlyDictionary<string, RecoveryCodeRecord> dictionary = RecoveryCodeRecord.Create( this, recoveryCodes );
        string[]                                        codes      = dictionary.Keys.GetArray();


        await db.RecoveryCodes.Delete( connection, transaction, old, token );
        await UserRecoveryCodeRecord.Replace( connection, transaction, db.UserRecoveryCodes, this, dictionary.Values, token );
        return codes;
    }


    [ RequiresPreviewFeatures ] public ValueTask<IEnumerable<RecoveryCodeRecord>> Codes( Database db, CancellationToken token ) => db.TryCall( Codes, db, token );
    [ RequiresPreviewFeatures ]
    public async ValueTask<IEnumerable<RecoveryCodeRecord>> Codes( DbConnection connection, DbTransaction transaction, Database db, CancellationToken token ) =>
        await UserRecoveryCodeRecord.Where( connection, transaction, db.UserRecoveryCodes, db.RecoveryCodes, this, token );


    public UserRecord SetSubscription<TRecord>( TRecord record ) where TRecord : TableRecord<TRecord>, IUserSubscription
    {
        SubscriptionExpires = record.SubscriptionExpires;
        SubscriptionID      = record.SubscriptionID;
        return this;
    }
    public async ValueTask<TRecord?> GetSubscription<TRecord>( DbConnection connection, DbTransaction transaction, DbTable<TRecord> table, CancellationToken token ) where TRecord : TableRecord<TRecord>, IUserSubscription, IDbReaderMapping<TRecord> =>
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

        return string.Equals( Address,         other.Address,         StringComparison.Ordinal ) &&
               string.Equals( City,            other.City,            StringComparison.Ordinal ) &&
               string.Equals( Company,         other.Company,         StringComparison.Ordinal ) &&
               string.Equals( Country,         other.Country,         StringComparison.Ordinal ) &&
               string.Equals( Department,      other.Department,      StringComparison.Ordinal ) &&
               string.Equals( Description,     other.Description,     StringComparison.Ordinal ) &&
               string.Equals( Email,           other.Email,           StringComparison.Ordinal ) &&
               string.Equals( Ext,             other.Ext,             StringComparison.Ordinal ) &&
               string.Equals( FirstName,       other.FirstName,       StringComparison.Ordinal ) &&
               string.Equals( FullName,        other.FullName,        StringComparison.Ordinal ) &&
               string.Equals( LastName,        other.LastName,        StringComparison.Ordinal ) &&
               string.Equals( Line1,           other.Line1,           StringComparison.Ordinal ) &&
               string.Equals( Line2,           other.Line2,           StringComparison.Ordinal ) &&
               string.Equals( PhoneNumber,     other.PhoneNumber,     StringComparison.Ordinal ) &&
               string.Equals( PostalCode,      other.PostalCode,      StringComparison.Ordinal ) &&
               string.Equals( StateOrProvince, other.StateOrProvince, StringComparison.Ordinal ) &&
               string.Equals( Title,           other.Title,           StringComparison.Ordinal ) &&
               string.Equals( Website,         other.Website,         StringComparison.Ordinal ) &&
               PreferredLanguage == other.PreferredLanguage;
    }
    public int CompareTo( IUserData? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int firstNameComparison = string.Compare( FirstName, other.FirstName, StringComparison.Ordinal );
        if ( firstNameComparison != 0 ) { return firstNameComparison; }

        int lastNameComparison = string.Compare( LastName, other.LastName, StringComparison.Ordinal );
        if ( lastNameComparison != 0 ) { return lastNameComparison; }

        int fullNameComparison = string.Compare( FullName, other.FullName, StringComparison.Ordinal );
        if ( fullNameComparison != 0 ) { return fullNameComparison; }

        int descriptionComparison = string.Compare( Description, other.Description, StringComparison.Ordinal );
        if ( descriptionComparison != 0 ) { return descriptionComparison; }

        int companyComparison = string.Compare( Company, other.Company, StringComparison.Ordinal );
        if ( companyComparison != 0 ) { return companyComparison; }

        int departmentComparison = string.Compare( Department, other.Department, StringComparison.Ordinal );
        if ( departmentComparison != 0 ) { return departmentComparison; }

        int titleComparison = string.Compare( Title, other.Title, StringComparison.Ordinal );
        if ( titleComparison != 0 ) { return titleComparison; }

        int emailComparison = string.Compare( Email, other.Email, StringComparison.Ordinal );
        if ( emailComparison != 0 ) { return emailComparison; }

        int phoneNumberComparison = string.Compare( PhoneNumber, other.PhoneNumber, StringComparison.Ordinal );
        if ( phoneNumberComparison != 0 ) { return phoneNumberComparison; }

        int extComparison = string.Compare( Ext, other.Ext, StringComparison.Ordinal );
        if ( extComparison != 0 ) { return extComparison; }

        int websiteComparison = string.Compare( Website, other.Website, StringComparison.Ordinal );
        if ( websiteComparison != 0 ) { return websiteComparison; }

        int cityComparison = string.Compare( City, other.City, StringComparison.Ordinal );
        if ( cityComparison != 0 ) { return cityComparison; }

        int countryComparison = string.Compare( Country, other.Country, StringComparison.Ordinal );
        if ( countryComparison != 0 ) { return countryComparison; }

        int line1Comparison = string.Compare( Line1, other.Line1, StringComparison.Ordinal );
        if ( line1Comparison != 0 ) { return line1Comparison; }

        int line2Comparison = string.Compare( Line2, other.Line2, StringComparison.Ordinal );
        if ( line2Comparison != 0 ) { return line2Comparison; }

        int postalCodeComparison = string.Compare( PostalCode, other.PostalCode, StringComparison.Ordinal );
        if ( postalCodeComparison != 0 ) { return postalCodeComparison; }

        int stateComparison = string.Compare( StateOrProvince, other.StateOrProvince, StringComparison.Ordinal );
        if ( stateComparison != 0 ) { return stateComparison; }

        int addressComparison = string.Compare( Address, other.Address, StringComparison.Ordinal );
        if ( addressComparison != 0 ) { return addressComparison; }

        return PreferredLanguage.CompareTo( other.PreferredLanguage );
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

    [ RequiresPreviewFeatures ]
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

    [ RequiresPreviewFeatures ]
    public async ValueTask<bool> TryAdd( DbConnection connection, DbTransaction transaction, Database db, GroupRecord value, CancellationToken token ) =>
        await UserGroupRecord.TryAdd( connection, transaction, db.UserGroups, this, value, token );
    public async ValueTask<IEnumerable<GroupRecord>> GetGroups( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token = default ) =>
        await UserGroupRecord.Where( connection, transaction, db.UserGroups, db.Groups, this, token );
    public async ValueTask<bool> IsPartOfGroup( DbConnection connection, DbTransaction transaction, Database db, GroupRecord value, CancellationToken token ) =>
        await UserGroupRecord.Exists( connection, transaction, db.UserGroups, this, value, token );
    public async ValueTask Remove( DbConnection connection, DbTransaction transaction, Database db, GroupRecord value, CancellationToken token ) =>
        await UserGroupRecord.Delete( connection, transaction, db.UserGroups, this, value, token );

    #endregion



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
