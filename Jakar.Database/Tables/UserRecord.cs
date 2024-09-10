namespace Jakar.Database;


[Serializable, Table( TABLE_NAME )]
public sealed record UserRecord( string                        UserName,
                                 string                        FirstName,
                                 string                        LastName,
                                 string                        FullName,
                                 string                        Gender,
                                 string                        Description,
                                 string                        Company,
                                 string                        Department,
                                 string                        Title,
                                 string                        Website,
                                 SupportedLanguage             PreferredLanguage,
                                 string                        Email,
                                 bool                          IsEmailConfirmed,
                                 string                        PhoneNumber,
                                 string                        Ext,
                                 bool                          IsPhoneNumberConfirmed,
                                 string                        AuthenticatorKey,
                                 bool                          IsTwoFactorEnabled,
                                 bool                          IsActive,
                                 bool                          IsDisabled,
                                 Guid?                         SubscriptionID,
                                 DateTimeOffset?               SubscriptionExpires,
                                 DateTimeOffset?               LastBadAttempt,
                                 DateTimeOffset?               LastLogin,
                                 int?                          BadLogins,
                                 bool                          IsLocked,
                                 DateTimeOffset?               LockDate,
                                 DateTimeOffset?               LockoutEnd,
                                 string                        RefreshToken,
                                 DateTimeOffset?               RefreshTokenExpiryTime,
                                 Guid?                         SessionID,
                                 string                        SecurityStamp,
                                 string                        ConcurrencyStamp,
                                 string                        Rights,
                                 RecordID<UserRecord>?         EscalateTo,
                                 IDictionary<string, JToken?>? AdditionalData,
                                 string                        PasswordHash,
                                 RecordID<FileRecord>?         ImageID,
                                 RecordID<UserRecord>          ID,
                                 RecordID<UserRecord>?         CreatedBy,
                                 DateTimeOffset                DateCreated,
                                 DateTimeOffset?               LastModified = null ) : OwnedTableRecord<UserRecord>( CreatedBy, ID, DateCreated, LastModified ), IDbReaderMapping<UserRecord>, IRefreshToken, IUserDataRecord
{
    public const                                                                                 int                           DEFAULT_BAD_LOGIN_DISABLE_THRESHOLD = 5;
    public const                                                                                 int                           ENCRYPTED_MAX_PASSWORD_SIZE         = 550;
    public const                                                                                 int                           MAX_PASSWORD_SIZE                   = 250;
    public const                                                                                 string                        TABLE_NAME                          = "Users";
    public static readonly                                                                       TimeSpan                      DefaultLockoutTime                  = TimeSpan.FromHours( 6 );
    private                                                                                      IDictionary<string, JToken?>? _additionalData                     = AdditionalData;
    public static                                                                                string                        TableName              { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TABLE_NAME; }
    [ProtectedPersonalData, StringLength( SQL.UNICODE_TEXT_CAPACITY ), JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData         { get => _additionalData; set => _additionalData = value; }
    [StringLength(                        SQL.ANSI_CAPACITY )]                            public string                        AuthenticatorKey       { get;                    set; } = AuthenticatorKey;
    public                                                                                       int?                          BadLogins              { get;                    set; } = BadLogins;
    [ProtectedPersonalData, StringLength( SQL.UNICODE_CAPACITY )] public                         string                        Company                { get;                    set; } = Company;
    [StringLength(                        SQL.ANSI_CAPACITY )]    public                         string                        ConcurrencyStamp       { get;                    set; } = ConcurrencyStamp;
    Guid? ICreatedByUser<Guid>.                                                                                                CreatedBy              => CreatedBy?.Value;
    [ProtectedPersonalData, StringLength( SQL.UNICODE_CAPACITY )] public string                                                Department             { get; set; } = Department;
    [ProtectedPersonalData, StringLength( SQL.UNICODE_CAPACITY )] public string                                                Description            { get; set; } = Description;
    [ProtectedPersonalData, StringLength( SQL.UNICODE_CAPACITY )] public string                                                Email                  { get; set; } = Email;
    public                                                               RecordID<UserRecord>?                                 EscalateTo             { get; set; } = EscalateTo;
    Guid? IEscalateToUser<Guid>.                                                                                               EscalateTo             => EscalateTo?.Value;
    [ProtectedPersonalData, StringLength( SQL.UNICODE_CAPACITY )] public string                                                Ext                    { get; set; } = Ext;
    [ProtectedPersonalData, StringLength( 2000 )]                 public string                                                FirstName              { get; set; } = FirstName;
    [ProtectedPersonalData, StringLength( SQL.UNICODE_CAPACITY )] public string                                                FullName               { get; set; } = FullName;
    [ProtectedPersonalData, StringLength( SQL.UNICODE_CAPACITY )] public string                                                Gender                 { get; set; } = Gender;
    Guid? IImageID<Guid>.                                                                                                      ImageID                => ImageID?.Value;
    public   RecordID<FileRecord>?                                                                                             ImageID                { get; set; } = ImageID;
    public   bool                                                                                                              IsActive               { get; set; } = IsActive;
    public   bool                                                                                                              IsDisabled             { get; set; } = IsDisabled;
    public   bool                                                                                                              IsEmailConfirmed       { get; set; } = IsEmailConfirmed;
    public   bool                                                                                                              IsLocked               { get; set; } = IsLocked;
    public   bool                                                                                                              IsPhoneNumberConfirmed { get; set; } = IsPhoneNumberConfirmed;
    public   bool                                                                                                              IsTwoFactorEnabled     { get; set; } = IsTwoFactorEnabled;
    internal bool                                                                                                              IsValid                { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => string.IsNullOrWhiteSpace( UserName ) is false && ID.IsValid(); }
    bool IValidator.                                                                                                           IsValid                => IsValid;
    public                                                                      DateTimeOffset?                                LastBadAttempt         { get; set; } = LastBadAttempt;
    public                                                                      DateTimeOffset?                                LastLogin              { get; set; } = LastLogin;
    [ProtectedPersonalData, StringLength( 2000 )] public                        string                                         LastName               { get; set; } = LastName;
    public                                                                      DateTimeOffset?                                LockDate               { get; set; } = LockDate;
    public                                                                      DateTimeOffset?                                LockoutEnd             { get; set; } = LockoutEnd;
    [StringLength(                        ENCRYPTED_MAX_PASSWORD_SIZE )] public string                                         PasswordHash           { get; set; } = PasswordHash;
    [ProtectedPersonalData, StringLength( SQL.UNICODE_CAPACITY )]        public string                                         PhoneNumber            { get; set; } = PhoneNumber;
    [StringLength(                        512 )]                         public SupportedLanguage                              PreferredLanguage      { get; set; } = PreferredLanguage;
    [StringLength(                        SQL.ANSI_CAPACITY )]           public string                                         RefreshToken           { get; set; } = RefreshToken;
    public                                                                      DateTimeOffset?                                RefreshTokenExpiryTime { get; set; } = RefreshTokenExpiryTime;
    [StringLength( IUserRights.MAX_SIZE )] public                               string                                         Rights                 { get; set; } = Rights;
    [StringLength( SQL.ANSI_CAPACITY )]    public                               string                                         SecurityStamp          { get; set; } = SecurityStamp;
    public                                                                      Guid?                                          SessionID              { get; set; } = SessionID;
    public                                                                      DateTimeOffset?                                SubscriptionExpires    { get; set; } = SubscriptionExpires;
    public                                                                      Guid?                                          SubscriptionID         { get; set; } = SubscriptionID;
    [ProtectedPersonalData, StringLength( SQL.UNICODE_CAPACITY )] public        string                                         Title                  { get; set; } = Title;
    Guid IUserID<Guid>.                                                                                                        UserID                 => ID.Value;
    [ProtectedPersonalData, StringLength( SQL.UNICODE_CAPACITY )] public string                                                Website                { get;                  set; } = Website;
    DateTimeOffset? IUserRecord<Guid>.                                                                                         LastModified           { get => _lastModified; set => _lastModified = value; }


    public override DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = base.ToDynamicParameters();
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
        parameters.Add( nameof(ConcurrencyStamp),       ConcurrencyStamp );
        parameters.Add( nameof(EscalateTo),             EscalateTo?.Value );
        parameters.Add( nameof(AuthenticatorKey),       AuthenticatorKey );
        parameters.Add( nameof(AdditionalData),         AdditionalData );
        return parameters;
    }


    public static UserRecord Create( DbDataReader reader )
    {
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
        Guid?                         subscriptionID         = reader.GetFieldValue<Guid?>( nameof(SubscriptionID) );
        DateTimeOffset?               subscriptionExpires    = reader.GetFieldValue<DateTimeOffset?>( nameof(SubscriptionExpires) );
        DateTimeOffset?               lastBadAttempt         = reader.GetFieldValue<DateTimeOffset?>( nameof(LastBadAttempt) );
        DateTimeOffset?               lastLogin              = reader.GetFieldValue<DateTimeOffset?>( nameof(LastLogin) );
        int                           badLogins              = reader.GetFieldValue<int>( nameof(BadLogins) );
        bool                          isLocked               = reader.GetBoolean( nameof(IsLocked) );
        DateTimeOffset?               lockDate               = reader.GetFieldValue<DateTimeOffset?>( nameof(LockDate) );
        DateTimeOffset?               lockoutEnd             = reader.GetFieldValue<DateTimeOffset?>( nameof(LockoutEnd) );
        string                        passwordHash           = reader.GetFieldValue<string>( nameof(PasswordHash) );
        string                        authenticatorKey       = reader.GetFieldValue<string>( nameof(AuthenticatorKey) );
        string                        refreshToken           = reader.GetFieldValue<string>( nameof(RefreshToken) );
        DateTimeOffset?               refreshTokenExpiryTime = reader.GetFieldValue<DateTimeOffset?>( nameof(RefreshTokenExpiryTime) );
        Guid?                         sessionID              = reader.GetFieldValue<Guid?>( nameof(SessionID) );
        bool                          isActive               = reader.GetBoolean( nameof(IsActive) );
        bool                          isDisabled             = reader.GetBoolean( nameof(IsDisabled) );
        string                        securityStamp          = reader.GetFieldValue<string>( nameof(SecurityStamp) );
        string                        concurrencyStamp       = reader.GetFieldValue<string>( nameof(ConcurrencyStamp) );
        RecordID<UserRecord>?         escalateTo             = RecordID<UserRecord>.TryCreate( reader, nameof(EscalateTo) );
        IDictionary<string, JToken?>? additionalData         = reader.GetAdditionalData();
        RecordID<FileRecord>?         imageID                = RecordID<FileRecord>.TryCreate( reader, nameof(ImageID) );
        RecordID<UserRecord>          id                     = RecordID<UserRecord>.ID( reader );
        RecordID<UserRecord>?         createdBy              = RecordID<UserRecord>.CreatedBy( reader );
        DateTimeOffset                dateCreated            = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        DateTimeOffset?               lastModified           = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );


        UserRecord record = new UserRecord( userName,
                                            firstName,
                                            lastName,
                                            fullName,
                                            gender,
                                            description,
                                            company,
                                            department,
                                            title,
                                            website,
                                            preferredLanguage,
                                            email,
                                            isEmailConfirmed,
                                            phoneNumber,
                                            ext,
                                            isPhoneNumberConfirmed,
                                            authenticatorKey,
                                            isTwoFactorEnabled,
                                            isActive,
                                            isDisabled,
                                            subscriptionID,
                                            subscriptionExpires,
                                            lastBadAttempt,
                                            lastLogin,
                                            badLogins,
                                            isLocked,
                                            lockDate,
                                            lockoutEnd,
                                            refreshToken,
                                            refreshTokenExpiryTime,
                                            sessionID,
                                            securityStamp,
                                            concurrencyStamp,
                                            rights,
                                            escalateTo,
                                            additionalData,
                                            passwordHash,
                                            imageID,
                                            id,
                                            createdBy,
                                            dateCreated,
                                            lastModified );

        record.Validate();
        return record;
    }


    public static async IAsyncEnumerable<UserRecord> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    public static UserRecord Create<TUser, TEnum>( ILoginRequest<TUser> request, UserRecord? caller = null )
        where TUser : class, IUserData<Guid>
        where TEnum : struct, Enum => Create( request, UserRights<TEnum>.Create( request.Data ), caller );
    public static UserRecord Create<TUser, TEnum>( ILoginRequest<TUser> request, scoped in UserRights<TEnum> rights, UserRecord? caller = null )
        where TUser : class, IUserData<Guid>
        where TEnum : struct, Enum => Create( request, rights.ToString(), caller );
    public static UserRecord Create<TUser>( ILoginRequest<TUser> request, string rights, UserRecord? caller = null )
        where TUser : class, IUserData<Guid>
    {
        ArgumentNullException.ThrowIfNull( request.Data );
        return Create( request.UserName, rights, request.Data, caller ).WithPassword( request.Password ).Enable();
    }

    public static UserRecord Create<TUser>( string userName, string rights, TUser data, UserRecord? caller = null )
        where TUser : class, IUserData<Guid> => new(userName,
                                                    data.FirstName,
                                                    data.LastName,
                                                    data.FullName,
                                                    data.Gender,
                                                    data.Description,
                                                    data.Company,
                                                    data.Department,
                                                    data.Title,
                                                    data.Website,
                                                    data.PreferredLanguage,
                                                    data.Email,
                                                    false,
                                                    data.PhoneNumber,
                                                    data.Ext,
                                                    false,
                                                    string.Empty,
                                                    true,
                                                    false,
                                                    false,
                                                    null,
                                                    null,
                                                    null,
                                                    null,
                                                    0,
                                                    false,
                                                    null,
                                                    null,
                                                    string.Empty,
                                                    null,
                                                    null,
                                                    string.Empty,
                                                    string.Empty,
                                                    rights,
                                                    RecordID<UserRecord>.TryCreate( data.EscalateTo ),
                                                    data.AdditionalData,
                                                    string.Empty,
                                                    RecordID<FileRecord>.TryCreate( data.ImageID ),
                                                    RecordID<UserRecord>.New(),
                                                    caller?.ID,
                                                    DateTimeOffset.UtcNow);

    public static UserRecord Create<TEnum>( string userName, string password, scoped in UserRights<TEnum> rights, UserRecord? caller = null )
        where TEnum : struct, Enum => Create( userName, password, rights.ToString(), caller );
    public static UserRecord Create( string userName, string password, string rights, UserRecord? caller = null ) =>
        new UserRecord( userName,
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
                        string.Empty,
                        true,
                        false,
                        false,
                        null,
                        null,
                        null,
                        null,
                        0,
                        false,
                        null,
                        null,
                        string.Empty,
                        null,
                        null,
                        string.Empty,
                        string.Empty,
                        rights,
                        RecordID<UserRecord>.Empty,
                        null,
                        string.Empty,
                        RecordID<FileRecord>.Empty,
                        RecordID<UserRecord>.New(),
                        caller?.ID,
                        DateTimeOffset.UtcNow ).WithPassword( password );


    public static DynamicParameters GetDynamicParameters( IUserData data )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(Email),     data.Email );
        parameters.Add( nameof(FirstName), data.FirstName );
        parameters.Add( nameof(LastName),  data.LastName );
        parameters.Add( nameof(FullName),  data.FullName );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( ILoginRequest request )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(UserName), request.UserName );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( string userName )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(UserName), userName );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( RecordID<UserRecord> userID )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(ID), userID );
        return parameters;
    }


    public UserRecord ClearRefreshToken( string securityStamp )
    {
        RefreshToken           = string.Empty;
        RefreshTokenExpiryTime = null;
        SecurityStamp          = securityStamp;
        return this;
    }


    public string        GetDescription()              => IUserData.GetDescription( this );
    void IUserData<Guid>.With( IUserData<Guid> value ) => With( value );
    public UserRecord With( IUserData<Guid> value )
    {
        CreatedBy         = RecordID<UserRecord>.TryCreate( value.CreatedBy );
        EscalateTo        = RecordID<UserRecord>.TryCreate( value.EscalateTo );
        FirstName         = value.FirstName;
        LastName          = value.LastName;
        FullName          = value.FullName;
        Description       = value.Description;
        Website           = value.Website;
        Email             = value.Email;
        PhoneNumber       = value.PhoneNumber;
        Ext               = value.Ext;
        Title             = value.Title;
        Department        = value.Department;
        Company           = value.Company;
        PreferredLanguage = value.PreferredLanguage;

        IDictionary<string, JToken?>? data = value.AdditionalData;
        if ( data is null ) { return this; }

        AdditionalData ??= new Dictionary<string, JToken?>();
        foreach ( (string key, JToken? jToken) in data ) { AdditionalData[key] = jToken; }

        return this;
    }


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
        await UserRecoveryCodeRecord.Replace( connection, transaction, db.UserRecoveryCodes, this, RecordID<RecoveryCodeRecord>.Create( dictionary.Values ), token );
        return codes;
    }


    public ValueTask<string[]> ReplaceCodes( Database db, IEnumerable<string> recoveryCodes, CancellationToken token = default ) => db.TryCall( ReplaceCodes, db, recoveryCodes, token );
    public async ValueTask<string[]> ReplaceCodes( DbConnection connection, DbTransaction transaction, Database db, IEnumerable<string> recoveryCodes, CancellationToken token = default )
    {
        IAsyncEnumerable<RecoveryCodeRecord>           old        = Codes( connection, transaction, db, token );
        ReadOnlyDictionary<string, RecoveryCodeRecord> dictionary = RecoveryCodeRecord.Create( this, recoveryCodes );
        string[]                                       codes      = [.. dictionary.Keys];


        await db.RecoveryCodes.Delete( connection, transaction, old, token );
        await UserRecoveryCodeRecord.Replace( connection, transaction, db.UserRecoveryCodes, this, RecordID<RecoveryCodeRecord>.Create( dictionary.Values ), token );
        return codes;
    }


    public IAsyncEnumerable<RecoveryCodeRecord> Codes( Database     db,         CancellationToken token )                                             => db.TryCall( Codes, db, token );
    public IAsyncEnumerable<RecoveryCodeRecord> Codes( DbConnection connection, DbTransaction     transaction, Database db, CancellationToken token ) => UserRecoveryCodeRecord.Where( connection, transaction, db.RecoveryCodes, this, token );


    public UserRecord WithRights<TEnum>( scoped in UserRights<TEnum> rights )
        where TEnum : struct, Enum
    {
        Rights = rights.ToString();
        return this;
    }
    public async ValueTask<UserModel<Guid>> GetRights( DbConnection connection, DbTransaction transaction, Database db, CancellationToken token )
    {
        UserModel<Guid> model = new(this);
        await foreach ( GroupRecord record in GetGroups( connection, transaction, db, token ) ) { model.Groups.Add( record.ToGroupModel() ); }

        await foreach ( RoleRecord record in GetRoles( connection, transaction, db, token ) ) { model.Roles.Add( record.ToRoleModel() ); }

        return model;
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


    public UserModel ToUserModel() => ToUserModel<UserModel>();
    public TClass ToUserModel<TClass>()
        where TClass : UserModel<TClass, Guid, UserAddress, GroupModel, RoleModel>, ICreateUserModel<TClass, Guid, UserAddress, GroupModel, RoleModel>, new() => ToUserModel<TClass, UserAddress, GroupModel, RoleModel>();
    public TClass ToUserModel<TClass, TAddress, TGroupModel, TRoleModel>()
        where TClass : IUserData<Guid, TAddress, TGroupModel, TRoleModel>, ICreateUserModel<TClass, Guid, TAddress, TGroupModel, TRoleModel>, new()
        where TGroupModel : IGroupModel<TGroupModel, Guid>
        where TRoleModel : IRoleModel<TRoleModel, Guid>
        where TAddress : IAddress<TAddress, Guid> => TClass.Create( this );


    public ValueTask<UserModel> ToUserModel( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => ToUserModel<UserModel>( connection, transaction, db, token );
    public ValueTask<TClass> ToUserModel<TClass>( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token )
        where TClass : UserModel<TClass, Guid, UserAddress, GroupModel, RoleModel>, ICreateUserModel<TClass, Guid, UserAddress, GroupModel, RoleModel>, new() => ToUserModel<TClass, UserAddress, GroupModel, RoleModel>( connection, transaction, db, token );
    public async ValueTask<TClass> ToUserModel<TClass, TAddress, TGroupModel, TRoleModel>( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token )
        where TClass : IUserData<Guid, TAddress, TGroupModel, TRoleModel>, ICreateUserModel<TClass, Guid, TAddress, TGroupModel, TRoleModel>, new()
        where TGroupModel : IGroupModel<TGroupModel, Guid>
        where TRoleModel : IRoleModel<TRoleModel, Guid>
        where TAddress : IAddress<TAddress, Guid>
    {
        TClass model = TClass.Create( this );

        await foreach ( AddressRecord record in GetAddresses( connection, transaction, db, token ) ) { model.Addresses.Add( record.ToAddressModel<TAddress>() ); }

        await foreach ( GroupRecord record in GetGroups( connection, transaction, db, token ) ) { model.Groups.Add( record.ToGroupModel<TGroupModel>() ); }

        await foreach ( RoleRecord record in GetRoles( connection, transaction, db, token ) ) { model.Roles.Add( record.ToRoleModel<TRoleModel>() ); }

        return model;
    }



    #region Passwords

    [MethodImpl( MethodImplOptions.AggressiveInlining )] public bool HasPassword() => string.IsNullOrWhiteSpace( PasswordHash ) is false;


    public UserRecord WithPassword( string password )
    {
        if ( password.Length > MAX_PASSWORD_SIZE ) { throw new ArgumentException( $"Password Must be less than {MAX_PASSWORD_SIZE} chars", nameof(password) ); }

        PasswordHash = Database.DataProtector.Encrypt( password );
        return this;
    }
    public UserRecord WithPassword( in string password, scoped in Requirements requirements ) => WithPassword( password, requirements, out _ );
    public UserRecord WithPassword( in string password, scoped in Requirements requirements, out PasswordValidator.Results results )
    {
        if ( requirements.maxLength > MAX_PASSWORD_SIZE ) { throw new ArgumentException( $"Password Must be less than {MAX_PASSWORD_SIZE} chars", nameof(password) ); }

        PasswordValidator validator = new(requirements);
        if ( validator.Validate( password, out results ) is false ) { return this; }

        return WithPassword( password );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool VerifyPassword( scoped ref UserRecord record, ILoginRequest request ) => VerifyPassword( ref record, request.Password );
    public static bool VerifyPassword( scoped ref UserRecord record, in string password )
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

    public async ValueTask<UserRecord?> GetBoss( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => EscalateTo.HasValue
                                                                                                                                                    ? await db.Users.Get( connection, transaction, EscalateTo.Value, token )
                                                                                                                                                    : null;


    public bool DoesNotOwn<TRecord>( TRecord record )
        where TRecord : OwnedTableRecord<TRecord>, IDbReaderMapping<TRecord> => record.CreatedBy != ID;
    public bool Owns<TRecord>( TRecord record )
        where TRecord : OwnedTableRecord<TRecord>, IDbReaderMapping<TRecord> => record.CreatedBy == ID;

    #endregion



    #region Controls

    public UserRecord MarkBadLogin() => MarkBadLogin( DefaultLockoutTime );
    public UserRecord MarkBadLogin( scoped in TimeSpan lockoutTime, in int badLoginDisableThreshold = DEFAULT_BAD_LOGIN_DISABLE_THRESHOLD )
    {
        int badLogins = BadLogins ?? 0;
        badLogins++;
        bool           isDisabled = badLogins > badLoginDisableThreshold;
        bool           isLocked   = isDisabled || IsActive is false;
        DateTimeOffset now        = DateTimeOffset.UtcNow;

        BadLogins      = badLogins;
        IsDisabled     = isDisabled;
        LastBadAttempt = now;
        IsLocked       = isLocked;

        LockDate = isLocked
                       ? now
                       : null;

        LockoutEnd = isLocked
                         ? now + lockoutTime
                         : null;

        return Modified();
    }

    public UserRecord SetActive()
    {
        LastLogin = DateTimeOffset.UtcNow;
        return Modified();
    }

    public UserRecord SetActive( bool isActive )
    {
        LastLogin = DateTimeOffset.UtcNow;
        IsActive  = isActive;
        return Modified();
    }


    public UserRecord Disable()
    {
        IsActive   = false;
        IsDisabled = true;
        return Modified();
    }

    public UserRecord Enable()
    {
        IsDisabled = false;
        IsActive   = true;
        return Modified();
    }


    public UserRecord Reset()
    {
        IsLocked   = false;
        LockDate   = null;
        LockoutEnd = null;
        BadLogins  = 0;
        IsDisabled = false;
        IsActive   = true;
        return Modified();
    }


    public UserRecord Unlock()
    {
        IsLocked   = false;
        LockDate   = null;
        LockoutEnd = null;
        return Modified();
    }
    public UserRecord Lock() => Lock( DefaultLockoutTime );
    public UserRecord Lock( scoped in TimeSpan lockoutTime )
    {
        DateTimeOffset lockDate = DateTimeOffset.UtcNow;

        IsLocked   = true;
        LockDate   = lockDate;
        LockoutEnd = lockDate + lockoutTime;
        return Modified();
    }
    public static bool TryEnable( scoped ref UserRecord record )
    {
        if ( record.LockoutEnd.HasValue && DateTimeOffset.UtcNow <= record.LockoutEnd.Value ) { record = record.Enable(); }

        return record.IsDisabled is false && record.IsActive;
    }

    #endregion



    #region Updaters

    public UserRecord WithAdditionalData<T>( T? value )
        where T : IDictionary<string, JToken?>
    {
        if ( value is null || value.Count <= 0 ) { return Modified(); }

        IDictionary<string, JToken?> data = AdditionalData ?? new Dictionary<string, JToken?>();
        foreach ( (string? key, JToken? jToken) in value ) { data[key] = jToken; }

        AdditionalData = data;
        return Modified();
    }


    public UserRecord WithUserData( IUserData<Guid> value )
    {
        FirstName         = value.FirstName;
        LastName          = value.LastName;
        FullName          = value.FullName;
        Description       = value.Description;
        Website           = value.Website;
        Email             = value.Email;
        PhoneNumber       = value.PhoneNumber;
        Ext               = value.Ext;
        Title             = value.Title;
        Department        = value.Department;
        Company           = value.Company;
        PreferredLanguage = value.PreferredLanguage;
        EscalateTo        = RecordID<UserRecord>.TryCreate( value.EscalateTo );
        ImageID           = RecordID<FileRecord>.TryCreate( value.ImageID );

        // CreatedBy = value.CreatedBy;

        return WithAdditionalData( value.AdditionalData );
    }

    #endregion



    #region Tokens

    public static bool CheckRefreshToken( scoped ref UserRecord record, in Tokens token, in bool hashed = true ) => CheckRefreshToken( ref record, token.RefreshToken, hashed );
    public static bool CheckRefreshToken( scoped ref UserRecord record, in string? token, in bool hashed = true )
    {
        // ReSharper disable once InvertIf
        if ( record.RefreshTokenExpiryTime.HasValue && DateTimeOffset.UtcNow > record.RefreshTokenExpiryTime.Value )
        {
            record = record.WithNoRefreshToken();
            return false;
        }

        return string.Equals( record.RefreshToken,
                              hashed
                                  ? Hashes.Hash128( token ).ToString()
                                  : token,
                              StringComparison.Ordinal );
    }


    public UserRecord WithNoRefreshToken()                                                                 => WithRefreshToken( string.Empty );
    public UserRecord WithRefreshToken( Tokens token, DateTimeOffset? date, string? securityStamp = null ) => WithRefreshToken( token.RefreshToken, date, securityStamp );
    public UserRecord WithRefreshToken( in string? token, DateTimeOffset? date = null, string? securityStamp = null, bool hashed = true )
    {
        if ( string.IsNullOrEmpty( token ) )
        {
            RefreshToken           = string.Empty;
            RefreshTokenExpiryTime = null;
            SecurityStamp          = securityStamp ?? string.Empty;
            return Modified();
        }

        date ??= DateTimeOffset.UtcNow;
        string hash = Hashes.Hash128( token ).ToString();
        SecurityStamp = securityStamp ?? hash;

        RefreshToken = hashed
                           ? hash
                           : token;

        RefreshTokenExpiryTime = date;
        return Modified();
    }

    #endregion



    #region Roles

    public async ValueTask<bool>                 TryAdd( DbConnection       connection, DbTransaction  transaction, Database db, AddressRecord                              value, CancellationToken token ) => await UserAddressRecord.TryAdd( connection, transaction, db.UserAddresses, this, value, token );
    public       IAsyncEnumerable<AddressRecord> GetAddresses( DbConnection connection, DbTransaction? transaction, Database db, [EnumeratorCancellation] CancellationToken token = default )                => UserAddressRecord.Where( connection, transaction, db.Addresses, this, token );
    public async ValueTask<bool>                 HasAddress( DbConnection   connection, DbTransaction  transaction, Database db, AddressRecord                              value, CancellationToken token ) => await UserAddressRecord.Exists( connection, transaction, db.UserAddresses, this, value, token );
    public async ValueTask                       Remove( DbConnection       connection, DbTransaction  transaction, Database db, AddressRecord                              value, CancellationToken token ) => await UserAddressRecord.Delete( connection, transaction, db.UserAddresses, this, value, token );

    #endregion



    #region Roles

    public async ValueTask<bool>              TryAdd( DbConnection   connection, DbTransaction  transaction, Database db, RoleRecord        value, CancellationToken token ) => await UserRoleRecord.TryAdd( connection, transaction, db.UserRoles, this, value, token );
    public       IAsyncEnumerable<RoleRecord> GetRoles( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token = default )                => UserRoleRecord.Where( connection, transaction, db.Roles, this, token );
    public async ValueTask<bool>              HasRole( DbConnection  connection, DbTransaction  transaction, Database db, RoleRecord        value, CancellationToken token ) => await UserRoleRecord.Exists( connection, transaction, db.UserRoles, this, value, token );
    public async ValueTask                    Remove( DbConnection   connection, DbTransaction  transaction, Database db, RoleRecord        value, CancellationToken token ) => await UserRoleRecord.Delete( connection, transaction, db.UserRoles, this, value, token );

    #endregion



    #region Groups

    public async ValueTask<bool>               TryAdd( DbConnection        connection, DbTransaction  transaction, Database db, GroupRecord       value, CancellationToken token ) => await UserGroupRecord.TryAdd( connection, transaction, db.UserGroups, this, value, token );
    public       IAsyncEnumerable<GroupRecord> GetGroups( DbConnection     connection, DbTransaction? transaction, Database db, CancellationToken token = default )                => UserGroupRecord.Where( connection, transaction, db.Groups, this, token );
    public async ValueTask<bool>               IsPartOfGroup( DbConnection connection, DbTransaction  transaction, Database db, GroupRecord       value, CancellationToken token ) => await UserGroupRecord.Exists( connection, transaction, db.UserGroups, this, value, token );
    public async ValueTask                     Remove( DbConnection        connection, DbTransaction  transaction, Database db, GroupRecord       value, CancellationToken token ) => await UserGroupRecord.Delete( connection, transaction, db.UserGroups, this, value, token );

    #endregion



    #region Claims

    public async  ValueTask<Claim[]>     GetUserClaims( DbConnection connection, DbTransaction? transaction, Database db, ClaimType       types,     CancellationToken token )                          => (await ToUserModel( connection, transaction, db, token )).GetClaims( types );
    public static ValueTask<UserRecord?> TryFromClaims( DbConnection connection, DbTransaction  transaction, Database db, ClaimsPrincipal principal, ClaimType         types, CancellationToken token ) => TryFromClaims( connection, transaction, db, principal.Claims.ToArray(), types, token );
    public static ValueTask<UserRecord?> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, scoped in ReadOnlySpan<Claim> claims, in ClaimType types, CancellationToken token )
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(ID), Guid.Parse( claims.Single( Claims.IsUserID ).Value ) );

        if ( types.HasFlag( ClaimType.UserName ) ) { parameters.Add( nameof(UserName), claims.Single( Claims.IsUserName ).Value ); }

        if ( types.HasFlag( ClaimType.FirstName ) ) { parameters.Add( nameof(FirstName), claims.Single( Claims.IsFirstName ).Value ); }

        if ( types.HasFlag( ClaimType.LastName ) ) { parameters.Add( nameof(LastName), claims.Single( Claims.IsLastName ).Value ); }

        if ( types.HasFlag( ClaimType.FullName ) ) { parameters.Add( nameof(FullName), claims.Single( Claims.IsFullName ).Value ); }

        if ( types.HasFlag( ClaimType.Email ) ) { parameters.Add( nameof(Email), claims.Single( Claims.IsEmail ).Value ); }

        if ( types.HasFlag( ClaimType.MobilePhone ) ) { parameters.Add( nameof(PhoneNumber), claims.Single( Claims.IsMobilePhone ).Value ); }

        if ( types.HasFlag( ClaimType.WebSite ) ) { parameters.Add( nameof(Website), claims.Single( Claims.IsWebSite ).Value ); }

        return db.Users.Get( connection, transaction, true, parameters, token );
    }
    public static async IAsyncEnumerable<UserRecord> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, Claim claim, [EnumeratorCancellation] CancellationToken token = default )
    {
        DynamicParameters parameters = new();

        switch ( claim.Type )
        {
            case ClaimTypes.NameIdentifier:
                parameters.Add( nameof(UserName), claim.Value );
                break;

            case ClaimTypes.Sid:
                parameters.Add( nameof(ID), Guid.Parse( claim.Value ) );
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
