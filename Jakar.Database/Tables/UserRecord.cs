namespace Jakar.Database;


[Serializable]
[Table( TABLE_NAME )]
public sealed record UserRecord( Guid                                                                   UserID,
                                 [property: ProtectedPersonalData] string                               UserName,
                                 string                                                                 FirstName,
                                 string                                                                 LastName,
                                 string                                                                 FullName,
                                 [property: MaxLength( IRights.MAX_SIZE )] string                       Rights,
                                 [property: MaxLength( 256 )]              string                       Gender,
                                 string                                                                 Company,
                                 string                                                                 Description,
                                 string                                                                 Department,
                                 string                                                                 Title,
                                 string                                                                 Website,
                                 SupportedLanguage                                                      PreferredLanguage,
                                 string                                                                 Email,
                                 bool                                                                   IsEmailConfirmed,
                                 string                                                                 PhoneNumber,
                                 string                                                                 Ext,
                                 bool                                                                   IsPhoneNumberConfirmed,
                                 bool                                                                   IsTwoFactorEnabled,
                                 DateTimeOffset?                                                        LastBadAttempt,
                                 DateTimeOffset?                                                        LastLogin,
                                 int                                                                    BadLogins,
                                 bool                                                                   IsLocked,
                                 DateTimeOffset?                                                        LockDate,
                                 DateTimeOffset?                                                        LockoutEnd,
                                 [property: MaxLength( UserRecord.ENCRYPTED_MAX_PASSWORD_SIZE )] string PasswordHash,
                                 [property: MaxLength( UserRecord.MAX_SIZE )]                    string RefreshToken,
                                 DateTimeOffset?                                                        RefreshTokenExpiryTime,
                                 Guid?                                                                  SessionID,
                                 bool                                                                   IsActive,
                                 bool                                                                   IsDisabled,
                                 [property: MaxLength( UserRecord.MAX_SIZE )] string                    SecurityStamp,
                                 [property: MaxLength( UserRecord.MAX_SIZE )] string                    AuthenticatorKey,
                                 [property: MaxLength( UserRecord.MAX_SIZE )] string                    ConcurrencyStamp,
                                 [property: MaxLength( 256 )]                 RecordID<UserRecord>?     EscalateTo,
                                 IDictionary<string, JToken?>?                                          AdditionalData,
                                 RecordID<UserRecord>                                                   ID,
                                 RecordID<UserRecord>?                                                  CreatedBy,
                                 Guid?                                                                  OwnerUserID,
                                 DateTimeOffset                                                         DateCreated,
                                 DateTimeOffset?                                                        LastModified = default ) : OwnedTableRecord<UserRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<UserRecord>, IUserData<UserRecord>, IRefreshToken, IUserID, IUserDataRecord, IRights
{
    public const           int                           DEFAULT_BAD_LOGIN_DISABLE_THRESHOLD = 5;
    public const           int                           ENCRYPTED_MAX_PASSWORD_SIZE         = 550;
    public const           int                           MAX_PASSWORD_SIZE                   = 250;
    public const           int                           MAX_SIZE                            = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;
    public const           string                        TABLE_NAME                          = "Users";
    public static readonly TimeSpan                      DefaultLockoutTime                  = TimeSpan.FromHours( 6 );
    private                IDictionary<string, JToken?>? _additionalData                     = AdditionalData;
    public static          string                        TableName { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }


    [ProtectedPersonalData] [MaxLength( int.MaxValue )] [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData    { get => _additionalData; set => _additionalData = value; }
    [ProtectedPersonalData]                                                 public string                        Company           { get;                    set; } = Company;
    [ProtectedPersonalData]                                                 public string                        Department        { get;                    set; } = Department;
    [ProtectedPersonalData]                                                 public string                        Description       { get;                    set; } = Description;
    [ProtectedPersonalData]                                                 public string                        Email             { get;                    set; } = Email;
    [ProtectedPersonalData]                                                 public string                        Ext               { get;                    set; } = Ext;
    [ProtectedPersonalData]                                                 public string                        FirstName         { get;                    set; } = FirstName;
    [ProtectedPersonalData]                                                 public string                        FullName          { get;                    set; } = FullName;
    public                                                                         DateTimeOffset?               LastLogin         { get;                    set; } = LastLogin;
    [ProtectedPersonalData] public                                                 string                        LastName          { get;                    set; } = LastName;
    [ProtectedPersonalData] public                                                 string                        PhoneNumber       { get;                    set; } = PhoneNumber;
    public                                                                         SupportedLanguage             PreferredLanguage { get;                    set; } = PreferredLanguage;
    [ProtectedPersonalData] public                                                 string                        Title             { get;                    set; } = Title;
    Guid IUserID.                                                                                                UserID            => UserID;
    [ProtectedPersonalData] public string                                                                        Website           { get; set; } = Website;

    [Pure]
    public override DynamicParameters ToDynamicParameters()
    {
        var parameters = base.ToDynamicParameters();
        parameters.Add( nameof(UserID),                 UserID );
        parameters.Add( nameof(UserName),               UserName );
        parameters.Add( nameof(FirstName),              FirstName );
        parameters.Add( nameof(LastName),               LastName );
        parameters.Add( nameof(FullName),               FullName );
        parameters.Add( nameof(Rights),                 Rights );
        parameters.Add( nameof(Gender),                 Gender );
        parameters.Add( nameof(Company),                Company );
        parameters.Add( nameof(Department),             Department );
        parameters.Add( nameof(Description),            Description );
        parameters.Add( nameof(Title),                  Title );
        parameters.Add( nameof(Website),                Website );
        parameters.Add( nameof(PreferredLanguage),      PreferredLanguage );
        parameters.Add( nameof(Email),                  Email );
        parameters.Add( nameof(IsEmailConfirmed),       IsEmailConfirmed );
        parameters.Add( nameof(PhoneNumber),            PhoneNumber );
        parameters.Add( nameof(Ext),                    Ext );
        parameters.Add( nameof(IsPhoneNumberConfirmed), IsPhoneNumberConfirmed );
        parameters.Add( nameof(IsTwoFactorEnabled),     IsTwoFactorEnabled );
        parameters.Add( nameof(LastBadAttempt),         LastBadAttempt );
        parameters.Add( nameof(LastLogin),              LastLogin );
        parameters.Add( nameof(BadLogins),              BadLogins );
        parameters.Add( nameof(IsLocked),               IsLocked );
        parameters.Add( nameof(LockDate),               LockDate );
        parameters.Add( nameof(LockoutEnd),             LockoutEnd );
        parameters.Add( nameof(PasswordHash),           PasswordHash );
        parameters.Add( nameof(RefreshToken),           RefreshToken );
        parameters.Add( nameof(RefreshTokenExpiryTime), RefreshTokenExpiryTime );
        parameters.Add( nameof(SessionID),              SessionID );
        parameters.Add( nameof(IsActive),               IsActive );
        parameters.Add( nameof(IsDisabled),             IsDisabled );
        parameters.Add( nameof(SecurityStamp),          SecurityStamp );
        parameters.Add( nameof(AuthenticatorKey),       AuthenticatorKey );
        parameters.Add( nameof(ConcurrencyStamp),       ConcurrencyStamp );
        parameters.Add( nameof(EscalateTo),             EscalateTo?.Value );
        parameters.Add( nameof(AdditionalData),         AdditionalData );
        return parameters;
    }

    [Pure]
    public static UserRecord Create( DbDataReader reader )
    {
        var                           userID                 = reader.GetFieldValue<Guid>( nameof(UserID) );
        string                        userName               = reader.GetFieldValue<string>( nameof(UserName) );
        string                        firstName              = reader.GetFieldValue<string>( nameof(FirstName) );
        string                        lastName               = reader.GetFieldValue<string>( nameof(LastName) );
        string                        fullName               = reader.GetFieldValue<string>( nameof(FullName) );
        string                        rights                 = reader.GetFieldValue<string>( nameof(Rights) );
        string                        gender                 = reader.GetFieldValue<string>( nameof(Gender) );
        string                        company                = reader.GetFieldValue<string>( nameof(Company) );
        string                        description            = reader.GetFieldValue<string>( nameof(Description) );
        string                        department             = reader.GetFieldValue<string>( nameof(Department) );
        string                        title                  = reader.GetFieldValue<string>( nameof(Title) );
        string                        website                = reader.GetFieldValue<string>( nameof(Website) );
        SupportedLanguage             preferredLanguage      = EnumSqlHandler<SupportedLanguage>.Instance.Parse( reader.GetValue( nameof(PreferredLanguage) ) );
        string                        email                  = reader.GetFieldValue<string>( nameof(Email) );
        bool                          isEmailConfirmed       = reader.GetBoolean( nameof(IsEmailConfirmed) );
        string                        phoneNumber            = reader.GetFieldValue<string>( nameof(PhoneNumber) );
        string                        ext                    = reader.GetFieldValue<string>( nameof(Ext) );
        bool                          isPhoneNumberConfirmed = reader.GetBoolean( nameof(IsPhoneNumberConfirmed) );
        bool                          isTwoFactorEnabled     = reader.GetBoolean( nameof(IsTwoFactorEnabled) );
        var                           lastBadAttempt         = reader.GetFieldValue<DateTimeOffset?>( nameof(LastBadAttempt) );
        var                           lastLogin              = reader.GetFieldValue<DateTimeOffset?>( nameof(LastLogin) );
        int                           badLogins              = reader.GetFieldValue<int>( nameof(BadLogins) );
        bool                          isLocked               = reader.GetBoolean( nameof(IsLocked) );
        var                           lockDate               = reader.GetFieldValue<DateTimeOffset?>( nameof(LockDate) );
        var                           lockoutEnd             = reader.GetFieldValue<DateTimeOffset?>( nameof(LockoutEnd) );
        string                        passwordHash           = reader.GetFieldValue<string>( nameof(PasswordHash) );
        string                        refreshToken           = reader.GetFieldValue<string>( nameof(RefreshToken) );
        var                           refreshTokenExpiryTime = reader.GetFieldValue<DateTimeOffset?>( nameof(RefreshTokenExpiryTime) );
        var                           sessionID              = reader.GetFieldValue<Guid?>( nameof(SessionID) );
        bool                          isActive               = reader.GetBoolean( nameof(IsActive) );
        bool                          isDisabled             = reader.GetBoolean( nameof(IsDisabled) );
        string                        securityStamp          = reader.GetFieldValue<string>( nameof(SecurityStamp) );
        string                        authenticatorKey       = reader.GetFieldValue<string>( nameof(AuthenticatorKey) );
        string                        concurrencyStamp       = reader.GetFieldValue<string>( nameof(AuthenticatorKey) );
        RecordID<UserRecord>?         escalateTo             = RecordID<UserRecord>.TryCreate( reader, nameof(EscalateTo) );
        IDictionary<string, JToken?>? additionalData         = reader.GetAdditionalData();
        RecordID<UserRecord>          id                     = RecordID<UserRecord>.ID( reader );
        RecordID<UserRecord>?         createdBy              = RecordID<UserRecord>.CreatedBy( reader );
        var                           ownerUserID            = reader.GetFieldValue<Guid?>( nameof(OwnerUserID) );
        var                           dateCreated            = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var                           lastModified           = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );


        var record = new UserRecord( userID,
                                     userName,
                                     firstName,
                                     lastName,
                                     fullName,
                                     rights,
                                     gender,
                                     company,
                                     description,
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

        record.Validate();
        return record;
    }

    [Pure]
    public static async IAsyncEnumerable<UserRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    [Pure]
    public static UserRecord Create<TUser>( VerifyRequest<TUser> request, IUserRights rights, UserRecord? caller = default )
        where TUser : IUserData => Create( request, rights.ToString(), caller );

    [Pure]
    public static UserRecord Create<TUser>( VerifyRequest<TUser> request, string rights, UserRecord? caller = default )
        where TUser : IUserData
    {
        ArgumentNullException.ThrowIfNull( request.Data );
        UserRecord record = Create( request.UserName, rights, request.Data, caller );
        return record.WithPassword( request.Password ).Enable();
    }

    [Pure]
    public static UserRecord Create( string userName, string rights, IUserData data, UserRecord? caller = default ) => new(Guid.NewGuid(),
                                                                                                                           userName,
                                                                                                                           data.FirstName,
                                                                                                                           data.LastName,
                                                                                                                           data.FullName,
                                                                                                                           rights,
                                                                                                                           data.Gender,
                                                                                                                           data.Company,
                                                                                                                           data.Description,
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
                                                                                                                           (data as JsonModels.IJsonModel)?.AdditionalData,
                                                                                                                           RecordID<UserRecord>.New(),
                                                                                                                           caller?.ID,
                                                                                                                           caller?.UserID,
                                                                                                                           DateTimeOffset.UtcNow);

    [Pure] public static UserRecord Create( string userName, string password, IUserRights rights, UserRecord? caller = default ) => Create( userName, password, rights.ToString(), caller );

    [Pure]
    public static UserRecord Create( string userName, string password, string rights, UserRecord? caller = default ) =>
        new UserRecord( Guid.NewGuid(),
                        userName,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        rights,
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


    [Pure]
    public static DynamicParameters GetDynamicParameters( IUserData data )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(Email),     data.Email );
        parameters.Add( nameof(FirstName), data.FirstName );
        parameters.Add( nameof(LastName),  data.LastName );
        parameters.Add( nameof(FullName),  data.FullName );
        return parameters;
    }
    [Pure]
    public static DynamicParameters GetDynamicParameters( ILoginRequest request )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserName), request.UserName );
        return parameters;
    }
    [Pure]
    public static DynamicParameters GetDynamicParameters( string userName )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserName), userName );
        return parameters;
    }
    [Pure]
    public static DynamicParameters GetDynamicParameters( Guid userID )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserID), userID );
        return parameters;
    }


    [Pure]
    public UserRecord ClearRefreshToken( string securityStamp ) => this with
                                                                   {
                                                                       RefreshToken = string.Empty,
                                                                       RefreshTokenExpiryTime = default,
                                                                       SecurityStamp = securityStamp
                                                                   };


    public ValueTask<bool> RedeemCode( Database db, string code, CancellationToken token ) => db.TryCall( RedeemCode, db, code, token );

    public async ValueTask<bool> RedeemCode( DbConnection connection, DbTransaction transaction, Database db, string code, CancellationToken token )
    {
        await foreach ( UserRecoveryCodeRecord mapping in UserRecoveryCodeRecord.Where( connection, transaction, db.UserRecoveryCodes, this, token ) )
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


    public ValueTask<string[]> ReplaceCodes( Database db, int count = 10, CancellationToken token = default ) => db.TryCall( ReplaceCodes, db, count, token );

    public async ValueTask<string[]> ReplaceCodes( DbConnection connection, DbTransaction transaction, Database db, int count = 10, CancellationToken token = default )
    {
        IAsyncEnumerable<RecoveryCodeRecord>            old        = Codes( connection, transaction, db, token );
        IReadOnlyDictionary<string, RecoveryCodeRecord> dictionary = RecoveryCodeRecord.Create( this, count );
        string[]                                        codes      = dictionary.Keys.ToArray();


        await db.RecoveryCodes.Delete( connection, transaction, old, token );
        await UserRecoveryCodeRecord.Replace( connection, transaction, db.UserRecoveryCodes, this, dictionary.Values, token );
        return codes;
    }


    public ValueTask<string[]> ReplaceCodes( Database db, IEnumerable<string> recoveryCodes, CancellationToken token = default ) => db.TryCall( ReplaceCodes, db, recoveryCodes, token );

    public async ValueTask<string[]> ReplaceCodes( DbConnection connection, DbTransaction transaction, Database db, IEnumerable<string> recoveryCodes, CancellationToken token = default )
    {
        IAsyncEnumerable<RecoveryCodeRecord>            old        = Codes( connection, transaction, db, token );
        IReadOnlyDictionary<string, RecoveryCodeRecord> dictionary = RecoveryCodeRecord.Create( this, recoveryCodes );
        string[]                                        codes      = dictionary.Keys.ToArray();


        await db.RecoveryCodes.Delete( connection, transaction, old, token );
        await UserRecoveryCodeRecord.Replace( connection, transaction, db.UserRecoveryCodes, this, dictionary.Values, token );
        return codes;
    }


    public IAsyncEnumerable<RecoveryCodeRecord> Codes( Database db, CancellationToken token ) => db.TryCall( Codes, db, token );

    public IAsyncEnumerable<RecoveryCodeRecord> Codes( DbConnection connection, DbTransaction transaction, Database db, CancellationToken token ) =>
        UserRecoveryCodeRecord.Where( connection, transaction, db.UserRecoveryCodes, db.RecoveryCodes, this, token );


    [Pure]
    public async ValueTask<UserRights<T>> GetRights<T>( DbConnection connection, DbTransaction transaction, Database db, CancellationToken token )
        where T : struct, Enum
    {
        List<IRights> rights = new(50) { this };

        await foreach ( GroupRecord record in GetGroups( connection, transaction, db, token ) ) { rights.Add( record ); }

        await foreach ( RoleRecord record in GetRoles( connection, transaction, db, token ) ) { rights.Add( record ); }

        return UserRights<T>.Merge( rights );
    }


    public bool Equals( IUserData? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( Company,     other.Company,     StringComparison.Ordinal ) &&
               string.Equals( Department,  other.Department,  StringComparison.Ordinal ) &&
               string.Equals( Description, other.Description, StringComparison.Ordinal ) &&
               string.Equals( Email,       other.Email,       StringComparison.Ordinal ) &&
               string.Equals( Ext,         other.Ext,         StringComparison.Ordinal ) &&
               string.Equals( FirstName,   other.FirstName,   StringComparison.Ordinal ) &&
               string.Equals( FullName,    other.FullName,    StringComparison.Ordinal ) &&
               string.Equals( LastName,    other.LastName,    StringComparison.Ordinal ) &&
               string.Equals( PhoneNumber, other.PhoneNumber, StringComparison.Ordinal ) &&
               string.Equals( Title,       other.Title,       StringComparison.Ordinal ) &&
               string.Equals( Website,     other.Website,     StringComparison.Ordinal ) &&
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

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool HasPassword() => string.IsNullOrWhiteSpace( PasswordHash ) is false;

    [Pure]
    public UserRecord WithPassword( string password )
    {
        if ( password.Length > ENCRYPTED_MAX_PASSWORD_SIZE ) { throw new ArgumentException( "Encrypted password Must be less than 550 chars", nameof(password) ); }

        return this with { PasswordHash = Database.DataProtector.Encrypt( password ) };
    }
    public static bool WithPassword( ref UserRecord record, string password, PasswordValidator validator )
    {
        if ( password.Length > MAX_PASSWORD_SIZE ) { throw new ArgumentException( "Password Must be less than 550 chars", nameof(password) ); }

        if ( validator.Validate( password ) is false ) { return false; }

        record = record.WithPassword( password );
        return true;
    }
    public static bool VerifyPassword( ref UserRecord record, VerifyRequest request ) => VerifyPassword( ref record, request.Password );
    public static bool VerifyPassword( ref UserRecord record, string password )
    {
        string value = Database.DataProtector.Decrypt( record.PasswordHash );

        if ( string.Equals( value, password, StringComparison.Ordinal ) )
        {
            record = record.SetActive();
            return true;
        }

        record = record.MarkBadLogin();
        return false;
    }

    #endregion



    #region Owners

    public async ValueTask<UserRecord?> GetBoss( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) =>
        EscalateTo.HasValue
            ? await db.Users.Get( connection, transaction, EscalateTo.Value, token )
            : default;


    public bool DoesNotOwn<TRecord>( TRecord record )
        where TRecord : OwnedTableRecord<TRecord>, IDbReaderMapping<TRecord> => record.OwnerUserID != UserID;
    public bool Owns<TRecord>( TRecord record )
        where TRecord : OwnedTableRecord<TRecord>, IDbReaderMapping<TRecord> => record.OwnerUserID == UserID;

    #endregion



    #region Controls

    [Pure] public UserRecord MarkBadLogin() => MarkBadLogin( DefaultLockoutTime, DEFAULT_BAD_LOGIN_DISABLE_THRESHOLD );
    [Pure]
    public UserRecord MarkBadLogin( in TimeSpan lockoutTime, in int badLoginDisableThreshold )
    {
        int            badLogins  = BadLogins + 1;
        bool           isDisabled = badLogins > badLoginDisableThreshold;
        bool           isLocked   = isDisabled || IsActive is false;
        DateTimeOffset now        = DateTimeOffset.UtcNow;

        return this with
               {
                   BadLogins = badLogins,
                   IsDisabled = isDisabled,
                   LastBadAttempt = now,
                   IsLocked = isLocked,
                   LockDate = isLocked
                                  ? now
                                  : null,
                   LockoutEnd = isLocked
                                    ? now + lockoutTime
                                    : null
               };
    }
    [Pure] public UserRecord SetActive() => this with { LastLogin = DateTimeOffset.UtcNow };
    [Pure]
    public UserRecord SetActive( bool isActive ) => this with
                                                    {
                                                        LastLogin = DateTimeOffset.UtcNow,
                                                        IsActive = isActive
                                                    };

    [Pure]
    public UserRecord Disable() => this with
                                   {
                                       IsActive = false,
                                       IsDisabled = true
                                   };
    [Pure]
    public UserRecord Enable() => this with
                                  {
                                      IsDisabled = false,
                                      IsActive = true
                                  };


    [Pure]
    public UserRecord Reset() => this with
                                 {
                                     IsLocked = false,
                                     LockDate = default,
                                     LockoutEnd = default,
                                     BadLogins = 0,
                                     IsDisabled = false,
                                     IsActive = true
                                 };


    [Pure]
    public UserRecord Unlock() => this with
                                  {
                                      IsLocked = false,
                                      LockDate = default,
                                      LockoutEnd = default
                                  };
    [Pure] public UserRecord Lock() => Lock( DefaultLockoutTime );
    [Pure]
    public UserRecord Lock( in TimeSpan lockoutTime )
    {
        DateTimeOffset lockDate = DateTimeOffset.UtcNow;

        return this with
               {
                   IsLocked = true,
                   LockDate = lockDate,
                   LockoutEnd = lockDate + lockoutTime
               };
    }
    public static bool TryEnable( ref UserRecord record )
    {
        if ( record.LockoutEnd.HasValue && DateTimeOffset.UtcNow <= record.LockoutEnd.Value ) { record = record.Enable(); }

        return record.IsDisabled is false && record.IsActive;
    }

    #endregion



    #region Updaters

    [Pure]
    public UserRecord WithAdditionalData<T>( T? value )
        where T : IDictionary<string, JToken?>
    {
        if ( value is null || value.Count <= 0 ) { return this; }

        IDictionary<string, JToken?> data = AdditionalData ?? new Dictionary<string, JToken?>();
        foreach ( (string? key, JToken? jToken) in value ) { data[key] = jToken; }

        AdditionalData = data;
        return this;
    }
    [Pure]
    public UserRecord WithUserData( IUserData value )
    {
        UserRecord user = this with
                          {
                              FirstName = value.FirstName,
                              LastName = value.LastName,
                              FullName = value.FullName,
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

        return user.WithAdditionalData( (value as JsonModels.IJsonModel)?.AdditionalData );
    }

    #endregion



    #region Tokens

    [Pure] public static bool CheckRefreshToken( ref UserRecord record, in Tokens token, in bool hashed = true ) => CheckRefreshToken( ref record, token.RefreshToken, hashed );

    [Pure]
    public static bool CheckRefreshToken( ref UserRecord record, in string? token, in bool hashed = true )
    {
        // ReSharper disable once InvertIf
        if ( record.RefreshTokenExpiryTime.HasValue && DateTimeOffset.UtcNow > record.RefreshTokenExpiryTime.Value )
        {
            record = record.WithNoRefreshToken();
            return false;
        }

        return string.Equals( record.RefreshToken,
                              hashed
                                  ? Spans.Hash128( token ).ToString()
                                  : token,
                              StringComparison.Ordinal );
    }


    [Pure] public UserRecord WithNoRefreshToken() => WithRefreshToken( default, default );
    [Pure]
    public UserRecord WithRefreshToken( in string? token, in DateTimeOffset? date, in bool hashed = true ) =>
        this with
        {
            RefreshToken = (hashed
                                ? Spans.Hash128( token ).ToString()
                                : token) ??
                           string.Empty,
            RefreshTokenExpiryTime = date
        };
    [Pure]
    public UserRecord WithRefreshToken( in string? token, in DateTimeOffset? date, string securityStamp, in bool hashed = true ) =>
        this with
        {
            RefreshToken = (hashed
                                ? Spans.Hash128( token ).ToString()
                                : token) ??
                           string.Empty,
            RefreshTokenExpiryTime = date,
            SecurityStamp = securityStamp
        };


    [Pure] public UserRecord SetRefreshToken( Tokens token, in DateTimeOffset? date, in bool hashed                        = true ) => WithRefreshToken( token.RefreshToken, date, hashed );
    [Pure] public UserRecord SetRefreshToken( Tokens token, in DateTimeOffset? date, string  securityStamp, in bool hashed = true ) => WithRefreshToken( token.RefreshToken, date, securityStamp, hashed );

    #endregion



    #region Roles

    public async ValueTask<bool>              TryAdd( DbConnection   connection, DbTransaction  transaction, Database db, RoleRecord        value, CancellationToken token ) => await UserRoleRecord.TryAdd( connection, transaction, db.UserRoles, this, value, token );
    public       IAsyncEnumerable<RoleRecord> GetRoles( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token = default )                => UserRoleRecord.Where( connection, transaction, db.UserRoles, db.Roles, this, token );
    public async ValueTask<bool>              HasRole( DbConnection  connection, DbTransaction  transaction, Database db, RoleRecord        value, CancellationToken token ) => await UserRoleRecord.Exists( connection, transaction, db.UserRoles, this, value, token );
    public async ValueTask                    Remove( DbConnection   connection, DbTransaction  transaction, Database db, RoleRecord        value, CancellationToken token ) => await UserRoleRecord.Delete( connection, transaction, db.UserRoles, this, value, token );

    #endregion



    #region Groups

    public async ValueTask<bool>               TryAdd( DbConnection        connection, DbTransaction  transaction, Database db, GroupRecord       value, CancellationToken token ) => await UserGroupRecord.TryAdd( connection, transaction, db.UserGroups, this, value, token );
    public       IAsyncEnumerable<GroupRecord> GetGroups( DbConnection     connection, DbTransaction? transaction, Database db, CancellationToken token = default )                => UserGroupRecord.Where( connection, transaction, db.UserGroups, db.Groups, this, token );
    public async ValueTask<bool>               IsPartOfGroup( DbConnection connection, DbTransaction  transaction, Database db, GroupRecord       value, CancellationToken token ) => await UserGroupRecord.Exists( connection, transaction, db.UserGroups, this, value, token );
    public async ValueTask                     Remove( DbConnection        connection, DbTransaction  transaction, Database db, GroupRecord       value, CancellationToken token ) => await UserGroupRecord.Delete( connection, transaction, db.UserGroups, this, value, token );

    #endregion



    #region Claims

    public async ValueTask<Claim[]> GetUserClaims( DbConnection connection, DbTransaction? transaction, Database db, ClaimType types, CancellationToken token )
    {
        var groups = new List<string>( 25 );
        var roles  = new List<string>( 25 );

        if ( types.HasFlag( ClaimType.GroupSid ) )
        {
            await foreach ( GroupRecord record in GetGroups( connection, transaction, db, token ) ) { groups.Add( record.NameOfGroup ); }
        }

        if ( types.HasFlag( ClaimType.Role ) )
        {
            await foreach ( RoleRecord record in GetRoles( connection, transaction, db, token ) ) { roles.Add( record.Name ); }
        }

        var claims = new List<Claim>( 16 + groups.Count + roles.Count );

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

        if ( types.HasFlag( ClaimType.WebSite ) ) { claims.Add( new Claim( ClaimTypes.Webpage, Website, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.GroupSid ) )
        {
            claims.AddRange( from nameOfGroup in groups
                             select new Claim( ClaimTypes.GroupSid, nameOfGroup, ClaimValueTypes.String ) );
        }

        if ( types.HasFlag( ClaimType.Role ) )
        {
            claims.AddRange( from role in roles
                             select new Claim( ClaimTypes.Role, role, ClaimValueTypes.String ) );
        }

        return [.. claims];
    }
    public static ValueTask<UserRecord?> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, HttpContext context, ClaimType types, CancellationToken token ) =>
        TryFromClaims( connection, transaction, db, context.User, types, token );
    public static ValueTask<UserRecord?> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, ClaimsPrincipal principal, ClaimType types, CancellationToken token ) =>
        TryFromClaims( connection, transaction, db, principal.Claims.ToArray(), types, token );

    public static async ValueTask<UserRecord?> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, ReadOnlyMemory<Claim> claims, ClaimType types, CancellationToken token )
    {
        Debug.Assert( types.HasFlag( ClaimType.UserID ) );
        Debug.Assert( types.HasFlag( ClaimType.UserName ) );


        var parameters = new DynamicParameters();
        parameters.Add( nameof(UserName), claims.Span.Single( x => x.Type == ClaimTypes.NameIdentifier ).Value );
        parameters.Add( nameof(UserID),   Guid.Parse( claims.Span.Single( x => x.Type == ClaimTypes.Sid ).Value ) );


        if ( types.HasFlag( ClaimType.FirstName ) ) { parameters.Add( nameof(FirstName), claims.Span.Single( x => x.Type == ClaimTypes.GivenName ).Value ); }

        if ( types.HasFlag( ClaimType.LastName ) ) { parameters.Add( nameof(LastName), claims.Span.Single( x => x.Type == ClaimTypes.Surname ).Value ); }

        if ( types.HasFlag( ClaimType.FullName ) ) { parameters.Add( nameof(FullName), claims.Span.Single( x => x.Type == ClaimTypes.Name ).Value ); }

        if ( types.HasFlag( ClaimType.Email ) ) { parameters.Add( nameof(Email), claims.Span.Single( x => x.Type == ClaimTypes.Email ).Value ); }

        if ( types.HasFlag( ClaimType.MobilePhone ) ) { parameters.Add( nameof(PhoneNumber), claims.Span.Single( x => x.Type == ClaimTypes.MobilePhone ).Value ); }

        if ( types.HasFlag( ClaimType.WebSite ) ) { parameters.Add( nameof(Website), claims.Span.Single( x => x.Type == ClaimTypes.Webpage ).Value ); }

        return await db.Users.Get( connection, transaction, true, parameters, token );
    }
    public static async IAsyncEnumerable<UserRecord> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, Claim claim, [EnumeratorCancellation] CancellationToken token = default )
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

            case ClaimTypes.Webpage:
                parameters.Add( nameof(Website), claim.Value );
                break;
        }

        await foreach ( UserRecord record in db.Users.Where( connection, transaction, true, parameters, token ) ) { yield return record; }
    }

    #endregion
}
