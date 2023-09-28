// ToothFairyDispatch :: ToothFairyDispatch.Cloud
// 08/29/2022  9:55 PM


namespace Jakar.Database;


[ Serializable, Table( "Users" ) ]
public sealed record UserRecord( Guid                                                                                                      UserID,
                                 [ property: ProtectedPersonalData, MaxLength( 256 ) ] string                                              UserName,
                                 string                                                                                                    FirstName,
                                 string                                                                                                    LastName,
                                 string                                                                                                    FullName,
                                 [ property: MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] string                Rights,
                                 [ property: MaxLength( 256 ) ]                                                      string                Gender,
                                 string                                                                                                    Address,
                                 string                                                                                                    Company,
                                 string                                                                                                    Description,
                                 string                                                                                                    Line1,
                                 string                                                                                                    Line2,
                                 string                                                                                                    City,
                                 string                                                                                                    StateOrProvince,
                                 string                                                                                                    Country,
                                 string                                                                                                    PostalCode,
                                 string                                                                                                    Department,
                                 string                                                                                                    Title,
                                 string                                                                                                    Website,
                                 SupportedLanguage                                                                                         PreferredLanguage,
                                 string                                                                                                    Email,
                                 bool                                                                                                      IsEmailConfirmed,
                                 string                                                                                                    PhoneNumber,
                                 string                                                                                                    Ext,
                                 bool                                                                                                      IsPhoneNumberConfirmed,
                                 bool                                                                                                      IsTwoFactorEnabled,
                                 DateTimeOffset?                                                                                           LastBadAttempt,
                                 DateTimeOffset?                                                                                           LastLogin,
                                 int                                                                                                       BadLogins,
                                 bool                                                                                                      IsLocked,
                                 DateTimeOffset?                                                                                           LockDate,
                                 DateTimeOffset?                                                                                           LockoutEnd,
                                 [ property: MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] string                PasswordHash,
                                 [ property: MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] string                RefreshToken,
                                 DateTimeOffset?                                                                                           RefreshTokenExpiryTime,
                                 Guid?                                                                                                     SessionID,
                                 bool                                                                                                      IsActive,
                                 bool                                                                                                      IsDisabled,
                                 [ property: MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] string                SecurityStamp,
                                 [ property: MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] string                AuthenticatorKey,
                                 [ property: MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] string                ConcurrencyStamp,
                                 [ property: MaxLength( 256 ) ]                                                      RecordID<UserRecord>? EscalateTo,
                                 IDictionary<string, JToken?>?                                                                             AdditionalData,
                                 RecordID<UserRecord>                                                                                      ID,
                                 RecordID<UserRecord>?                                                                                     CreatedBy,
                                 Guid?                                                                                                     OwnerUserID,
                                 DateTimeOffset                                                                                            DateCreated,
                                 DateTimeOffset?                                                                                           LastModified = default
) : TableRecord<UserRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<UserRecord>, IUserData<UserRecord>, IRefreshToken, IUserID, IUserDataRecord, UserRights.IRights
{
    private static readonly PasswordHasher<UserRecord>    _hasher         = new();
    private                 IDictionary<string, JToken?>? _additionalData = AdditionalData;

    [ ProtectedPersonalData, MaxLength( int.MaxValue ) ]
    public IDictionary<string, JToken?>? AdditionalData
    {
        get => _additionalData;
        set => _additionalData = value;
    }
    [ ProtectedPersonalData, MaxLength( 4096 ) ] public string            Address           { get; set; } = Address;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string            City              { get; set; } = City;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string            Company           { get; set; } = Company;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string            Country           { get; set; } = Country;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string            Department        { get; set; } = Department;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string            Description       { get; set; } = Description;
    [ ProtectedPersonalData, MaxLength( 1024 ) ] public string            Email             { get; set; } = Email;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string            Ext               { get; set; } = Ext;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string            FirstName         { get; set; } = FirstName;
    [ ProtectedPersonalData, MaxLength( 512 ) ]  public string            FullName          { get; set; } = FullName;
    public                                              DateTimeOffset?   LastLogin         { get; set; } = LastLogin;
    [ ProtectedPersonalData, MaxLength( 256 ) ] public  string            LastName          { get; set; } = LastName;
    [ ProtectedPersonalData, MaxLength( 512 ) ] public  string            Line1             { get; set; } = Line1;
    [ ProtectedPersonalData, MaxLength( 256 ) ] public  string            Line2             { get; set; } = Line2;
    [ ProtectedPersonalData, MaxLength( 256 ) ] public  string            PhoneNumber       { get; set; } = PhoneNumber;
    [ ProtectedPersonalData, MaxLength( 256 ) ] public  string            PostalCode        { get; set; } = PostalCode;
    public                                              SupportedLanguage PreferredLanguage { get; set; } = PreferredLanguage;
    [ ProtectedPersonalData, MaxLength( 256 ) ] public  string            StateOrProvince   { get; set; } = StateOrProvince;
    [ ProtectedPersonalData, MaxLength( 256 ) ] public  string            Title             { get; set; } = Title;
    Guid IUserID.                                                         UserID            => UserID;
    [ ProtectedPersonalData, MaxLength( 4096 ) ] public string            Website           { get; set; } = Website;


    public static T TryGet<T>( DbDataReader reader, in string key ) where T : struct
    {
        int index = reader.GetOrdinal( key );
    }
    public static UserRecord Create( DbDataReader reader )
    {
        Guid              userID                 = reader.GetGuid( nameof(UserID) );
        string            userName               = reader.GetString( nameof(UserName) );
        string            firstName              = reader.GetString( nameof(FirstName) );
        string            lastName               = reader.GetString( nameof(LastName) );
        string            fullName               = reader.GetString( nameof(FullName) );
        string            rights                 = reader.GetString( nameof(Rights) );
        string            gender                 = reader.GetString( nameof(Gender) );
        string            address                = reader.GetString( nameof(Address) );
        string            company                = reader.GetString( nameof(Company) );
        string            description            = reader.GetString( nameof(Description) );
        string            line1                  = reader.GetString( nameof(Line1) );
        string            line2                  = reader.GetString( nameof(Line2) );
        string            city                   = reader.GetString( nameof(City) );
        string            stateOrProvince        = reader.GetString( nameof(StateOrProvince) );
        string            country                = reader.GetString( nameof(Country) );
        string            postalCode             = reader.GetString( nameof(PostalCode) );
        string            department             = reader.GetString( nameof(Department) );
        string            title                  = reader.GetString( nameof(Title) );
        string            website                = reader.GetString( nameof(Website) );
        SupportedLanguage preferredLanguage      = EnumSqlHandler<SupportedLanguage>.Instance.Parse( reader.GetValue( nameof(PreferredLanguage) ) );
        string            email                  = reader.GetString( nameof(Email) );
        bool              isEmailConfirmed       = reader.GetFieldValue<bool>( nameof(IsEmailConfirmed) );
        string            phoneNumber            = reader.GetString( nameof(PhoneNumber) );
        string            ext                    = reader.GetString( nameof(Ext) );
        bool              isPhoneNumberConfirmed = reader.GetFieldValue<bool>( nameof(IsPhoneNumberConfirmed) );
        bool              isTwoFactorEnabled     = reader.GetFieldValue<bool>( nameof(IsTwoFactorEnabled) );

        DateTimeOffset? lastBadAttempt         = reader.GetFieldValue<DateTimeOffset?>( nameof(LastBadAttempt) );
        DateTimeOffset? lastLogin              = reader.GetFieldValue<DateTimeOffset?>( nameof(LastLogin) );
        int             badLogins              = reader.GetFieldValue<int>( nameof(BadLogins) );
        bool            isLocked               = reader.GetFieldValue<bool>( nameof(IsLocked) );
        DateTimeOffset? lockDate               = reader.GetFieldValue<DateTimeOffset?>( nameof(LockDate) );
        DateTimeOffset? lockoutEnd             = reader.GetFieldValue<DateTimeOffset?>( nameof(LockoutEnd) );
        string          passwordHash           = reader.GetString( nameof(PasswordHash) );
        string          refreshToken           = reader.GetString( nameof(RefreshToken) );
        DateTimeOffset? refreshTokenExpiryTime = reader.GetFieldValue<DateTimeOffset?>( nameof(RefreshTokenExpiryTime) );
        Guid?           sessionID              = reader.GetFieldValue<Guid?>( nameof(SessionID) );
        bool            isActive               = reader.GetFieldValue<bool>( nameof(IsActive) );
        bool            isDisabled             = reader.GetFieldValue<bool>( nameof(IsDisabled) );
        string          securityStamp          = reader.GetString( nameof(SecurityStamp) );
        string          authenticatorKey       = reader.GetString( nameof(AuthenticatorKey) );
        string          concurrencyStamp       = reader.GetString( nameof(AuthenticatorKey) );
        var             escalateTo             = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(EscalateTo) ) );

        IDictionary<string, JToken?>? additionalData = reader.GetFieldValue<string?>( nameof(AdditionalData) )
                                                            ?.FromJson<Dictionary<string, JToken?>>();

        var id           = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(ID) ) );
        var dateCreated  = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var lastModified = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );
        var ownerUserID  = reader.GetFieldValue<Guid>( nameof(OwnerUserID) );
        var createdBy    = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(CreatedBy) ) );


        return new UserRecord( userID,
                               userName,
                               firstName,
                               lastName,
                               fullName,
                               rights,
                               gender,
                               address,
                               company,
                               description,
                               line1,
                               line2,
                               city,
                               stateOrProvince,
                               country,
                               postalCode,
                               department,
                               title,
                               website,
                               preferredLanguage,
                               email,
                               isEmailConfirmed,
                               phoneNumber,
                               ext,
                               isPhoneNumberConfirmed,
                               isTwoFactorEnabled,
                               lastBadAttempt,
                               lastLogin,
                               badLogins,
                               isLocked,
                               lockDate,
                               lockoutEnd,
                               passwordHash,
                               refreshToken,
                               refreshTokenExpiryTime,
                               sessionID,
                               isActive,
                               isDisabled,
                               securityStamp,
                               authenticatorKey,
                               concurrencyStamp,
                               escalateTo,
                               additionalData,
                               id,
                               createdBy,
                               ownerUserID,
                               dateCreated,
                               lastModified );
    }

    public static async IAsyncEnumerable<UserRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    public static UserRecord Create<TUser>( VerifyRequest<TUser> request, UserRights rights, UserRecord? caller = default ) where TUser : IUserData => Create( request, rights.ToString(), caller );
    public static UserRecord Create<TUser>( VerifyRequest<TUser> request, string rights, UserRecord? caller = default ) where TUser : IUserData
    {
        ArgumentNullException.ThrowIfNull( request.Data );
        UserRecord record = Create( request.UserLogin, rights, request.Data, caller );

        record = record with
                 {
                     UserName = request.UserLogin
                 };

        return record.WithPassword( request.UserPassword )
                     .Enable();
    }
    public static UserRecord Create( string userName, string rights, IUserData data, UserRecord? caller = default ) => new(Guid.NewGuid(),
                                                                                                                           userName,
                                                                                                                           data.FirstName,
                                                                                                                           data.LastName,
                                                                                                                           data.FullName,
                                                                                                                           rights,
                                                                                                                           data.Gender,
                                                                                                                           data.Address,
                                                                                                                           data.Company,
                                                                                                                           data.Description,
                                                                                                                           data.Line1,
                                                                                                                           data.Line2,
                                                                                                                           data.City,
                                                                                                                           data.StateOrProvince,
                                                                                                                           data.Country,
                                                                                                                           data.PostalCode,
                                                                                                                           data.Department,
                                                                                                                           data.Title,
                                                                                                                           data.Website,
                                                                                                                           data.PreferredLanguage,
                                                                                                                           data.Email,
                                                                                                                           false,
                                                                                                                           data.PhoneNumber,
                                                                                                                           data.Ext,
                                                                                                                           false,
                                                                                                                           false,
                                                                                                                           null,
                                                                                                                           null,
                                                                                                                           0,
                                                                                                                           false,
                                                                                                                           null,
                                                                                                                           null,
                                                                                                                           string.Empty,
                                                                                                                           string.Empty,
                                                                                                                           null,
                                                                                                                           null,
                                                                                                                           true,
                                                                                                                           false,
                                                                                                                           string.Empty,
                                                                                                                           string.Empty,
                                                                                                                           string.Empty,
                                                                                                                           null,
                                                                                                                           data.AdditionalData,
                                                                                                                           RecordID<UserRecord>.New(),
                                                                                                                           caller?.ID,
                                                                                                                           caller?.UserID,
                                                                                                                           DateTimeOffset.UtcNow);
    public static UserRecord Create( string userName, string password, UserRights rights, UserRecord? caller = default ) => Create( userName, password, rights.ToString(), caller );
    public static UserRecord Create( string userName, string password, string rights, UserRecord? caller = default )
    {
        return new UserRecord( Guid.NewGuid(),
                               userName,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               rights,
                               string.Empty, // TODO: data.Gender,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               SupportedLanguage.English,
                               string.Empty,
                               false,
                               string.Empty,
                               string.Empty,
                               false,
                               false,
                               null,
                               null,
                               0,
                               false,
                               null,
                               null,
                               string.Empty,
                               string.Empty,
                               null,
                               null,
                               true,
                               false,
                               string.Empty,
                               string.Empty,
                               string.Empty,
                               null,
                               null,
                               RecordID<UserRecord>.New(),
                               caller?.ID,
                               caller?.UserID,
                               DateTimeOffset.UtcNow ).WithPassword( password );
    }


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
    public static DynamicParameters GetDynamicParameters( Guid userID )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserID), userID );
        return parameters;
    }


    public UserRecord ClearRefreshToken( string securityStamp ) => this with
                                                                   {
                                                                       RefreshToken = string.Empty,
                                                                       RefreshTokenExpiryTime = default,
                                                                       SecurityStamp = securityStamp
                                                                   };


    [ RequiresPreviewFeatures ] public ValueTask<bool> RedeemCode( Database db, string code, CancellationToken token ) => db.TryCall( RedeemCode, db, code, token );
    [ RequiresPreviewFeatures ]
    public async ValueTask<bool> RedeemCode( DbConnection connection, DbTransaction transaction, Database db, string code, CancellationToken token )
    {
        IEnumerable<UserRecoveryCodeRecord> mappings = await UserRecoveryCodeRecord.Where( connection, transaction, db.UserRecoveryCodes, this, token );

        foreach ( UserRecoveryCodeRecord mapping in mappings )
        {
            RecoveryCodeRecord? record = await mapping.Get( connection, transaction, db.RecoveryCodes, token );

            if ( record is null ) { await db.UserRecoveryCodes.Delete( connection, transaction, mapping, token ); }
            else if ( RecoveryCodeRecord.IsValid( code, ref record ) )
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


    public UserRights GetRights() => new(this);
    public async ValueTask<UserRights> GetRights<T>( DbConnection connection, DbTransaction transaction, Database db, CancellationToken token ) where T : struct, Enum
    {
        int totalRightCount = Enum.GetValues<T>()
                                  .Length;

        return await GetRights( connection, transaction, db, totalRightCount, token );
    }

    public async ValueTask<UserRights> GetRights( DbConnection connection, DbTransaction transaction, Database db, int totalRightCount, CancellationToken token )
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
    public override int CompareTo( UserRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }


        int userNameComparison = string.Compare( UserName, other.UserName, StringComparison.Ordinal );
        if ( userNameComparison != 0 ) { return userNameComparison; }

        int firstNameComparison = string.Compare( FirstName, other.FirstName, StringComparison.Ordinal );
        if ( firstNameComparison != 0 ) { return firstNameComparison; }

        int lastNameComparison = string.Compare( LastName, other.LastName, StringComparison.Ordinal );
        if ( lastNameComparison != 0 ) { return lastNameComparison; }

        int fullNameComparison = string.Compare( FullName, other.FullName, StringComparison.Ordinal );
        if ( fullNameComparison != 0 ) { return fullNameComparison; }

        int descriptionComparison = string.Compare( Description, other.Description, StringComparison.Ordinal );
        if ( descriptionComparison != 0 ) { return descriptionComparison; }

        int sessionIDComparison = Nullable.Compare( SessionID, other.SessionID );
        if ( sessionIDComparison != 0 ) { return sessionIDComparison; }

        int addressComparison = string.Compare( Address, other.Address, StringComparison.Ordinal );
        if ( addressComparison != 0 ) { return addressComparison; }

        int line1Comparison = string.Compare( Line1, other.Line1, StringComparison.Ordinal );
        if ( line1Comparison != 0 ) { return line1Comparison; }

        int line2Comparison = string.Compare( Line2, other.Line2, StringComparison.Ordinal );
        if ( line2Comparison != 0 ) { return line2Comparison; }

        int cityComparison = string.Compare( City, other.City, StringComparison.Ordinal );
        if ( cityComparison != 0 ) { return cityComparison; }

        int stateComparison = string.Compare( StateOrProvince, other.StateOrProvince, StringComparison.Ordinal );
        if ( stateComparison != 0 ) { return stateComparison; }

        int countryComparison = string.Compare( Country, other.Country, StringComparison.Ordinal );
        if ( countryComparison != 0 ) { return countryComparison; }

        int postalCodeComparison = string.Compare( PostalCode, other.PostalCode, StringComparison.Ordinal );
        if ( postalCodeComparison != 0 ) { return postalCodeComparison; }

        int websiteComparison = string.Compare( Website, other.Website, StringComparison.Ordinal );
        if ( websiteComparison != 0 ) { return websiteComparison; }

        int emailComparison = string.Compare( Email, other.Email, StringComparison.Ordinal );
        if ( emailComparison != 0 ) { return emailComparison; }

        int phoneNumberComparison = string.Compare( PhoneNumber, other.PhoneNumber, StringComparison.Ordinal );
        if ( phoneNumberComparison != 0 ) { return phoneNumberComparison; }

        int extComparison = string.Compare( Ext, other.Ext, StringComparison.Ordinal );
        if ( extComparison != 0 ) { return extComparison; }

        int titleComparison = string.Compare( Title, other.Title, StringComparison.Ordinal );
        if ( titleComparison != 0 ) { return titleComparison; }

        int departmentComparison = string.Compare( Department, other.Department, StringComparison.Ordinal );
        if ( departmentComparison != 0 ) { return departmentComparison; }

        return string.Compare( Company, other.Company, StringComparison.Ordinal );
    }



    #region Passwords

    public bool HasPassword() => !string.IsNullOrWhiteSpace( PasswordHash );
    /// <summary>
    ///     <para> <see href="https://stackoverflow.com/a/63733365/9530917"/> </para>
    ///     <para> <see href="https://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file"/> </para>
    ///     <see cref="PasswordHasher{TRecord}"/>
    /// </summary>
    public UserRecord WithPassword( string password ) =>
        this with
        {
            PasswordHash = _hasher.HashPassword( this, password )
        };
    /// <summary>
    ///     <para> <see href="https://stackoverflow.com/a/63733365/9530917"/> </para>
    ///     <para> <see href="https://stackoverflow.com/questions/10520048/calculate-md5-checksum-for-a-file"/> </para>
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
                WithPassword( password );
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
        int badLogins = BadLogins + 1;

        UserRecord user = this with
                          {
                              BadLogins = badLogins,
                              IsDisabled = badLogins > 5,
                              LastBadAttempt = DateTimeOffset.UtcNow
                          };

        return user.IsDisabled
                   ? user.Lock()
                   : user.Unlock();
    }
    public UserRecord SetActive( bool? isActive = default ) => isActive.HasValue
                                                                   ? this with
                                                                     {
                                                                         IsActive = isActive.Value
                                                                     }
                                                                   : this;


    public bool TryEnable()
    {
        if ( LockoutEnd.HasValue && DateTimeOffset.UtcNow <= LockoutEnd.Value ) { Enable(); }

        return !IsDisabled && IsActive;
    }
    public UserRecord Disable() => this with
                                   {
                                       IsActive = false,
                                       IsDisabled = true
                                   };
    public UserRecord Enable() => this with
                                  {
                                      IsDisabled = false,
                                      IsActive = true
                                  };


    public UserRecord Reset() => this with
                                 {
                                     IsLocked = false,
                                     LockDate = default,
                                     LockoutEnd = default,
                                     BadLogins = 0,
                                     IsDisabled = false,
                                     IsActive = true
                                 };


    public UserRecord Unlock() => this with
                                  {
                                      IsLocked = false,
                                      LockDate = default,
                                      LockoutEnd = default
                                  };
    public UserRecord Lock() => Lock( TimeSpan.FromHours( 6 ) );
    public UserRecord Lock( in TimeSpan offset ) => this with
                                                    {
                                                        IsLocked = true,
                                                        LockDate = DateTimeOffset.UtcNow,
                                                        LockoutEnd = LockDate + offset
                                                    };

    #endregion



    #region Updaters

    public UserRecord WithAdditionalData( IDictionary<string, JToken?>? value )
    {
        if ( value is null ) { return this; }

        IDictionary<string, JToken?> data = this.GetData();
        foreach ( (string? key, JToken? jToken) in value ) { data[key] = jToken; }

        return this with
               {
                   AdditionalData = data
               };
    }
    public UserRecord WithUserData( IUserData value )
    {
        UserRecord user = this with
                          {
                              FirstName = value.FirstName,
                              LastName = value.LastName,
                              FullName = value.FullName,
                              Address = value.Address,
                              Line1 = value.Line1,
                              Line2 = value.Line2,
                              City = value.City,
                              StateOrProvince = value.StateOrProvince,
                              Country = value.Country,
                              PostalCode = value.PostalCode,
                              Description = value.Description,
                              Website = value.Website,
                              Email = value.Email,
                              PhoneNumber = value.PhoneNumber,
                              Ext = value.Ext,
                              Title = value.Title,
                              Department = value.Department,
                              Company = value.Company,
                              PreferredLanguage = value.PreferredLanguage
                          };

        return user.WithAdditionalData( value.AdditionalData );
    }

    #endregion



    #region Tokens

    public static bool IsHashedRefreshToken( string? token, ref UserRecord record )
    {
        // ReSharper disable once InvertIf
        if ( record.RefreshTokenExpiryTime.HasValue && DateTimeOffset.UtcNow > record.RefreshTokenExpiryTime.Value )
        {
            record = record.WithNoRefreshToken();
            return true;
        }


        return string.Equals( record.RefreshToken,
                              token?.GetHashCode()
                                    .ToString(),
                              StringComparison.Ordinal );
    }
    public static bool IsHashedRefreshToken( Tokens token, ref UserRecord record ) => IsHashedRefreshToken( token.RefreshToken, ref record );


    public UserRecord WithNoRefreshToken() => WithRefreshToken( default, default );
    public UserRecord WithRefreshToken( string? token, in DateTimeOffset? date, in bool hashed = true )
    {
        if ( hashed )
        {
            token = token?.GetHashCode()
                          .ToString();
        }

        return this with
               {
                   RefreshToken = token ?? string.Empty,
                   RefreshTokenExpiryTime = date
               };
    }
    public UserRecord WithRefreshToken( string? token, in DateTimeOffset? date, string securityStamp, in bool hashed = true )
    {
        if ( hashed )
        {
            token = token?.GetHashCode()
                          .ToString();
        }

        return this with
               {
                   RefreshToken = token ?? string.Empty,
                   RefreshTokenExpiryTime = date,
                   SecurityStamp = securityStamp
               };
    }

    public UserRecord SetRefreshToken( Tokens token, in DateTimeOffset? date, in bool hashed                        = true ) => WithRefreshToken( token.RefreshToken, date, hashed );
    public UserRecord SetRefreshToken( Tokens token, in DateTimeOffset? date, string  securityStamp, in bool hashed = true ) => WithRefreshToken( token.RefreshToken, date, securityStamp, hashed );

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

        if ( types.HasFlag( ClaimType.SubscriptionExpiration ) && types.HasFlag( ClaimType.Expired ) )
        {
            DateTimeOffset? expires = await db.GetSubscriptionExpiration( connection, transaction, this, token );
            claims.Add( new Claim( ClaimTypes.Expiration, expires?.ToString() ?? string.Empty,          ClaimValueTypes.DateTime ) );
            claims.Add( new Claim( ClaimTypes.Expired,    (expires > DateTimeOffset.UtcNow).ToString(), ClaimValueTypes.Boolean ) );
        }
        else if ( types.HasFlag( ClaimType.SubscriptionExpiration ) && !types.HasFlag( ClaimType.Expired ) )
        {
            DateTimeOffset? expires = await db.GetSubscriptionExpiration( connection, transaction, this, token );
            claims.Add( new Claim( ClaimTypes.Expiration, expires?.ToString() ?? string.Empty, ClaimValueTypes.DateTime ) );
        }
        else if ( types.HasFlag( ClaimType.Expired ) && !types.HasFlag( ClaimType.SubscriptionExpiration ) )
        {
            DateTimeOffset? expires = await db.GetSubscriptionExpiration( connection, transaction, this, token );
            claims.Add( new Claim( ClaimTypes.Expired, (expires > DateTimeOffset.UtcNow).ToString(), ClaimValueTypes.Boolean ) );
        }

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
