namespace Jakar.Database;


[Serializable, Table(TABLE_NAME), SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed record UserRecord : OwnedTableRecord<UserRecord>, ITableRecord<UserRecord>, IRefreshToken, IUserDataRecord
{
    public const           int      DEFAULT_BAD_LOGIN_DISABLE_THRESHOLD = 5;
    public const           int      ENCRYPTED_MAX_PASSWORD_SIZE         = 550;
    public const           int      MAX_PASSWORD_SIZE                   = 250;
    public const           string   TABLE_NAME                          = "users";
    public static readonly TimeSpan DefaultLockoutTime                  = TimeSpan.FromHours(6);


    public static                                                                                             JsonTypeInfo<UserRecord[]> JsonArrayInfo          => JakarDatabaseContext.Default.UserRecordArray;
    public static                                                                                             JsonSerializerContext      JsonContext            => JakarDatabaseContext.Default;
    public static                                                                                             JsonTypeInfo<UserRecord>   JsonTypeInfo           => JakarDatabaseContext.Default.UserRecord;
    public static                                                                                             string                     TableName              { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => TABLE_NAME; }
    [ProtectedPersonalData, StringLength(Constants.UNICODE_TEXT_CAPACITY), JsonExtensionData] public override JsonObject?                AdditionalData         { get => _additionalData; set => _additionalData = value; }
    [StringLength(                       Constants.ANSI_CAPACITY)]                            public          string                     AuthenticatorKey       { get;                    set; }
    public                                                                                                    int?                       BadLogins              { get;                    set; }
    [ProtectedPersonalData, StringLength(Constants.UNICODE_CAPACITY)] public                                  string                     Company                { get;                    set; }
    [StringLength(                       Constants.ANSI_CAPACITY)]    public                                  string                     ConcurrencyStamp       { get;                    set; }
    Guid? ICreatedByUser<Guid>.                                                                                                          CreatedBy              => CreatedBy?.Value;
    [ProtectedPersonalData, StringLength(Constants.UNICODE_CAPACITY)] public string                                                      Department             { get; set; }
    [ProtectedPersonalData, StringLength(Constants.UNICODE_CAPACITY)] public string                                                      Description            { get; set; }
    [ProtectedPersonalData, StringLength(Constants.UNICODE_CAPACITY)] public string                                                      Email                  { get; set; }
    public                                                                   RecordID<UserRecord>?                                       EscalateTo             { get; set; }
    Guid? IEscalateToUser<Guid>.                                                                                                         EscalateTo             => EscalateTo?.Value;
    [ProtectedPersonalData, StringLength(Constants.UNICODE_CAPACITY)] public string                                                      Ext                    { get; set; }
    [ProtectedPersonalData, StringLength(2000)]                       public string                                                      FirstName              { get; set; }
    [ProtectedPersonalData, StringLength(Constants.UNICODE_CAPACITY)] public string                                                      FullName               { get; set; }
    [ProtectedPersonalData, StringLength(Constants.UNICODE_CAPACITY)] public string                                                      Gender                 { get; set; }
    Guid? IImageID<Guid>.                                                                                                                ImageID                => ImageID?.Value;
    public   RecordID<FileRecord>?                                                                                                       ImageID                { get; set; }
    public   bool                                                                                                                        IsActive               { get; set; }
    public   bool                                                                                                                        IsDisabled             { get; set; }
    public   bool                                                                                                                        IsEmailConfirmed       { get; set; }
    public   bool                                                                                                                        IsLocked               { get; set; }
    public   bool                                                                                                                        IsPhoneNumberConfirmed { get; set; }
    public   bool                                                                                                                        IsTwoFactorEnabled     { get; set; }
    internal bool                                                                                                                        IsValid                { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => !string.IsNullOrWhiteSpace(UserName) && ID.IsValid(); }
    bool IValidator.                                                                                                                     IsValid                => IsValid;
    public DateTimeOffset?                                                                                                               LastBadAttempt         { get;                  set; }
    public DateTimeOffset?                                                                                                               LastLogin              { get;                  set; }
    DateTimeOffset? IUserRecord<Guid>.                                                                                                   LastModified           { get => _lastModified; set => _lastModified = value; }
    [ProtectedPersonalData, StringLength(2000)] public                        string                                                     LastName               { get;                  set; }
    public                                                                    DateTimeOffset?                                            LockDate               { get;                  set; }
    public                                                                    DateTimeOffset?                                            LockoutEnd             { get;                  set; }
    [StringLength(                       ENCRYPTED_MAX_PASSWORD_SIZE)] public string                                                     PasswordHash           { get;                  set; }
    [ProtectedPersonalData, StringLength(Constants.UNICODE_CAPACITY)]  public string                                                     PhoneNumber            { get;                  set; }
    [StringLength(                       512)]                         public SupportedLanguage                                          PreferredLanguage      { get;                  set; }
    [StringLength(                       Constants.ANSI_CAPACITY)]     public string                                                     RefreshToken           { get;                  set; }
    public                                                                    DateTimeOffset?                                            RefreshTokenExpiryTime { get;                  set; }
    [StringLength(IUserRights.MAX_SIZE)]    public                            string                                                     Rights                 { get;                  set; }
    [StringLength(Constants.ANSI_CAPACITY)] public                            string                                                     SecurityStamp          { get;                  set; }
    public                                                                    Guid?                                                      SessionID              { get;                  set; }
    public                                                                    DateTimeOffset?                                            SubscriptionExpires    { get;                  set; }
    public                                                                    Guid?                                                      SubscriptionID         { get;                  set; }
    [ProtectedPersonalData, StringLength(Constants.UNICODE_CAPACITY)] public  string                                                     Title                  { get;                  set; }
    Guid IUserID<Guid>.                                                                                                                  UserID                 => ID.Value;
    public                                                                   string                                                      UserName               { get; init; }
    [ProtectedPersonalData, StringLength(Constants.UNICODE_CAPACITY)] public string                                                      Website                { get; set; }


    public UserRecord( string                UserName,
                       string                FirstName,
                       string                LastName,
                       string                FullName,
                       string                Gender,
                       string                Description,
                       string                Company,
                       string                Department,
                       string                Title,
                       string                Website,
                       SupportedLanguage     PreferredLanguage,
                       string                Email,
                       bool                  IsEmailConfirmed,
                       string                PhoneNumber,
                       string                Ext,
                       bool                  IsPhoneNumberConfirmed,
                       string                AuthenticatorKey,
                       bool                  IsTwoFactorEnabled,
                       bool                  IsActive,
                       bool                  IsDisabled,
                       Guid?                 SubscriptionID,
                       DateTimeOffset?       SubscriptionExpires,
                       DateTimeOffset?       LastBadAttempt,
                       DateTimeOffset?       LastLogin,
                       int?                  BadLogins,
                       bool                  IsLocked,
                       DateTimeOffset?       LockDate,
                       DateTimeOffset?       LockoutEnd,
                       string                RefreshToken,
                       DateTimeOffset?       RefreshTokenExpiryTime,
                       Guid?                 SessionID,
                       string                SecurityStamp,
                       string                ConcurrencyStamp,
                       string                Rights,
                       RecordID<UserRecord>? EscalateTo,
                       JsonObject?           AdditionalData,
                       string                PasswordHash,
                       RecordID<FileRecord>? ImageID,
                       RecordID<UserRecord>  ID,
                       RecordID<UserRecord>? CreatedBy,
                       DateTimeOffset        DateCreated,
                       DateTimeOffset?       LastModified = null
    ) : base(in CreatedBy, in ID, in DateCreated, in LastModified)
    {
        this.UserName               = UserName;
        this.AdditionalData         = AdditionalData;
        this.AuthenticatorKey       = AuthenticatorKey;
        this.BadLogins              = BadLogins;
        this.Company                = Company;
        this.ConcurrencyStamp       = ConcurrencyStamp;
        this.Department             = Department;
        this.Description            = Description;
        this.Email                  = Email;
        this.EscalateTo             = EscalateTo;
        this.Ext                    = Ext;
        this.FirstName              = FirstName;
        this.FullName               = FullName;
        this.Gender                 = Gender;
        this.ImageID                = ImageID;
        this.IsActive               = IsActive;
        this.IsDisabled             = IsDisabled;
        this.IsEmailConfirmed       = IsEmailConfirmed;
        this.IsLocked               = IsLocked;
        this.IsPhoneNumberConfirmed = IsPhoneNumberConfirmed;
        this.IsTwoFactorEnabled     = IsTwoFactorEnabled;
        this.LastBadAttempt         = LastBadAttempt;
        this.LastLogin              = LastLogin;
        this.LastName               = LastName;
        this.LockDate               = LockDate;
        this.LockoutEnd             = LockoutEnd;
        this.PasswordHash           = PasswordHash;
        this.PhoneNumber            = PhoneNumber;
        this.PreferredLanguage      = PreferredLanguage;
        this.RefreshToken           = RefreshToken;
        this.RefreshTokenExpiryTime = RefreshTokenExpiryTime;
        this.Rights                 = Rights;
        this.SecurityStamp          = SecurityStamp;
        this.SessionID              = SessionID;
        this.SubscriptionExpires    = SubscriptionExpires;
        this.SubscriptionID         = SubscriptionID;
        this.Title                  = Title;
        this.Website                = Website;
    }


    public override DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = base.ToDynamicParameters();
        parameters.Add(nameof(UserName),               UserName);
        parameters.Add(nameof(FirstName),              FirstName);
        parameters.Add(nameof(LastName),               LastName);
        parameters.Add(nameof(FullName),               FullName);
        parameters.Add(nameof(Rights),                 Rights);
        parameters.Add(nameof(Gender),                 Gender);
        parameters.Add(nameof(Company),                Company);
        parameters.Add(nameof(Department),             Department);
        parameters.Add(nameof(Description),            Description);
        parameters.Add(nameof(Title),                  Title);
        parameters.Add(nameof(Website),                Website);
        parameters.Add(nameof(PreferredLanguage),      PreferredLanguage);
        parameters.Add(nameof(Email),                  Email);
        parameters.Add(nameof(IsEmailConfirmed),       IsEmailConfirmed);
        parameters.Add(nameof(PhoneNumber),            PhoneNumber);
        parameters.Add(nameof(Ext),                    Ext);
        parameters.Add(nameof(IsPhoneNumberConfirmed), IsPhoneNumberConfirmed);
        parameters.Add(nameof(IsTwoFactorEnabled),     IsTwoFactorEnabled);
        parameters.Add(nameof(LastBadAttempt),         LastBadAttempt);
        parameters.Add(nameof(LastLogin),              LastLogin);
        parameters.Add(nameof(BadLogins),              BadLogins);
        parameters.Add(nameof(IsLocked),               IsLocked);
        parameters.Add(nameof(LockDate),               LockDate);
        parameters.Add(nameof(LockoutEnd),             LockoutEnd);
        parameters.Add(nameof(PasswordHash),           PasswordHash);
        parameters.Add(nameof(RefreshToken),           RefreshToken);
        parameters.Add(nameof(RefreshTokenExpiryTime), RefreshTokenExpiryTime);
        parameters.Add(nameof(SessionID),              SessionID);
        parameters.Add(nameof(IsActive),               IsActive);
        parameters.Add(nameof(IsDisabled),             IsDisabled);
        parameters.Add(nameof(SecurityStamp),          SecurityStamp);
        parameters.Add(nameof(ConcurrencyStamp),       ConcurrencyStamp);
        parameters.Add(nameof(EscalateTo),             EscalateTo?.Value);
        parameters.Add(nameof(AuthenticatorKey),       AuthenticatorKey);
        parameters.Add(nameof(AdditionalData),         AdditionalData);
        return parameters;
    }


    public static UserRecord Create( DbDataReader reader )
    {
        string                userName               = reader.GetFieldValue<string>(nameof(UserName));
        string                firstName              = reader.GetFieldValue<string>(nameof(FirstName));
        string                lastName               = reader.GetFieldValue<string>(nameof(LastName));
        string                fullName               = reader.GetFieldValue<string>(nameof(FullName));
        string                rights                 = reader.GetFieldValue<string>(nameof(Rights));
        string                gender                 = reader.GetFieldValue<string>(nameof(Gender));
        string                company                = reader.GetFieldValue<string>(nameof(Company));
        string                description            = reader.GetFieldValue<string>(nameof(Description));
        string                department             = reader.GetFieldValue<string>(nameof(Department));
        string                title                  = reader.GetFieldValue<string>(nameof(Title));
        string                website                = reader.GetFieldValue<string>(nameof(Website));
        SupportedLanguage     preferredLanguage      = EnumSqlHandler<SupportedLanguage>.Instance.Parse(reader.GetValue(nameof(PreferredLanguage)));
        string                email                  = reader.GetFieldValue<string>(nameof(Email));
        bool                  isEmailConfirmed       = reader.GetBoolean(nameof(IsEmailConfirmed));
        string                phoneNumber            = reader.GetFieldValue<string>(nameof(PhoneNumber));
        string                ext                    = reader.GetFieldValue<string>(nameof(Ext));
        bool                  isPhoneNumberConfirmed = reader.GetBoolean(nameof(IsPhoneNumberConfirmed));
        bool                  isTwoFactorEnabled     = reader.GetBoolean(nameof(IsTwoFactorEnabled));
        Guid?                 subscriptionID         = reader.GetFieldValue<Guid?>(nameof(SubscriptionID));
        DateTimeOffset?       subscriptionExpires    = reader.GetFieldValue<DateTimeOffset?>(nameof(SubscriptionExpires));
        DateTimeOffset?       lastBadAttempt         = reader.GetFieldValue<DateTimeOffset?>(nameof(LastBadAttempt));
        DateTimeOffset?       lastLogin              = reader.GetFieldValue<DateTimeOffset?>(nameof(LastLogin));
        int                   badLogins              = reader.GetFieldValue<int>(nameof(BadLogins));
        bool                  isLocked               = reader.GetBoolean(nameof(IsLocked));
        DateTimeOffset?       lockDate               = reader.GetFieldValue<DateTimeOffset?>(nameof(LockDate));
        DateTimeOffset?       lockoutEnd             = reader.GetFieldValue<DateTimeOffset?>(nameof(LockoutEnd));
        string                passwordHash           = reader.GetFieldValue<string>(nameof(PasswordHash));
        string                authenticatorKey       = reader.GetFieldValue<string>(nameof(AuthenticatorKey));
        string                refreshToken           = reader.GetFieldValue<string>(nameof(RefreshToken));
        DateTimeOffset?       refreshTokenExpiryTime = reader.GetFieldValue<DateTimeOffset?>(nameof(RefreshTokenExpiryTime));
        Guid?                 sessionID              = reader.GetFieldValue<Guid?>(nameof(SessionID));
        bool                  isActive               = reader.GetBoolean(nameof(IsActive));
        bool                  isDisabled             = reader.GetBoolean(nameof(IsDisabled));
        string                securityStamp          = reader.GetFieldValue<string>(nameof(SecurityStamp));
        string                concurrencyStamp       = reader.GetFieldValue<string>(nameof(ConcurrencyStamp));
        RecordID<UserRecord>? escalateTo             = RecordID<UserRecord>.TryCreate(reader, nameof(EscalateTo));
        JsonObject?           additionalData         = reader.GetAdditionalData();
        RecordID<FileRecord>? imageID                = RecordID<FileRecord>.TryCreate(reader, nameof(ImageID));
        RecordID<UserRecord>  id                     = RecordID<UserRecord>.ID(reader);
        RecordID<UserRecord>? createdBy              = RecordID<UserRecord>.CreatedBy(reader);
        DateTimeOffset        dateCreated            = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        DateTimeOffset?       lastModified           = reader.GetFieldValue<DateTimeOffset?>(nameof(LastModified));


        UserRecord record = new(userName,
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
                                lastModified);

        return record.Validate();
    }


    public static UserRecord Create<TUser, TEnum>( ILoginRequest<TUser> request, UserRecord? caller = null )
        where TUser : class, IUserData<Guid>
        where TEnum : struct, Enum => Create(request, UserRights<TEnum>.Create(request.Data), caller);
    public static UserRecord Create<TUser, TEnum>( ILoginRequest<TUser> request, scoped in UserRights<TEnum> rights, UserRecord? caller = null )
        where TUser : class, IUserData<Guid>
        where TEnum : struct, Enum => Create(request, rights.ToString(), caller);
    public static UserRecord Create<TUser>( ILoginRequest<TUser> request, string rights, UserRecord? caller = null )
        where TUser : class, IUserData<Guid>
    {
        ArgumentNullException.ThrowIfNull(request.Data);
        return Create(request.UserName, rights, request.Data, caller).WithPassword(request.Password).Enable();
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
                                                    RecordID<UserRecord>.TryCreate(data.EscalateTo),
                                                    data.AdditionalData,
                                                    string.Empty,
                                                    RecordID<FileRecord>.TryCreate(data.ImageID),
                                                    RecordID<UserRecord>.New(),
                                                    caller?.ID,
                                                    DateTimeOffset.UtcNow);

    public static UserRecord Create<TEnum>( string userName, string password, scoped in UserRights<TEnum> rights, UserRecord? caller = null )
        where TEnum : struct, Enum => Create(userName, password, rights.ToString(), caller);
    public static UserRecord Create( string userName, string password, string rights, UserRecord? caller = null ) =>
        new UserRecord(userName,
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
                       DateTimeOffset.UtcNow).WithPassword(password);


    public static DynamicParameters GetDynamicParameters( IUserData data )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(Email),     data.Email);
        parameters.Add(nameof(FirstName), data.FirstName);
        parameters.Add(nameof(LastName),  data.LastName);
        parameters.Add(nameof(FullName),  data.FullName);
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( ILoginRequest request )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(UserName), request.UserName);
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( string userName )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(UserName), userName);
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( RecordID<UserRecord> userID )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(ID), userID);
        return parameters;
    }


    public UserRecord ClearRefreshToken( string securityStamp )
    {
        RefreshToken           = string.Empty;
        RefreshTokenExpiryTime = null;
        SecurityStamp          = securityStamp;
        return this;
    }


    public string        GetDescription()              => IUserData.GetDescription(this);
    void IUserData<Guid>.With( IUserData<Guid> value ) => With(value);
    public UserRecord With( IUserData<Guid> value )
    {
        CreatedBy         = RecordID<UserRecord>.TryCreate(value.CreatedBy);
        EscalateTo        = RecordID<UserRecord>.TryCreate(value.EscalateTo);
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

        return WithAdditionalData(value);
    }


    public ValueTask<bool> RedeemCode( Database db, string code, CancellationToken token ) => db.TryCall(RedeemCode, db, code, token);
    public async ValueTask<bool> RedeemCode( NpgsqlConnection connection, DbTransaction transaction, Database db, string code, CancellationToken token )
    {
        await foreach ( UserRecoveryCodeRecord mapping in UserRecoveryCodeRecord.Where(connection, transaction, db.UserRecoveryCodes, this, token) )
        {
            RecoveryCodeRecord? record = await mapping.Get(connection, transaction, db.RecoveryCodes, token);

            if ( record is null ) { await db.UserRecoveryCodes.Delete(connection, transaction, mapping, token); }
            else if ( RecoveryCodeRecord.IsValid(code, ref record) )
            {
                await db.RecoveryCodes.Delete(connection, transaction, record, token);
                await db.UserRecoveryCodes.Delete(connection, transaction, mapping, token);
                return true;
            }
        }

        return false;
    }


    public ValueTask<string[]> ReplaceCodes( Database db, int count = 10, CancellationToken token = default ) => db.TryCall(ReplaceCodes, db, count, token);
    public async ValueTask<string[]> ReplaceCodes( NpgsqlConnection connection, DbTransaction transaction, Database db, int count = 10, CancellationToken token = default )
    {
        IAsyncEnumerable<RecoveryCodeRecord>            old        = Codes(connection, transaction, db, token);
        IReadOnlyDictionary<string, RecoveryCodeRecord> dictionary = RecoveryCodeRecord.Create(this, count);
        string[]                                        codes      = dictionary.Keys.ToArray();


        await db.RecoveryCodes.Delete(connection, transaction, old, token);
        await UserRecoveryCodeRecord.Replace(connection, transaction, db.UserRecoveryCodes, this, RecordID<RecoveryCodeRecord>.Create(dictionary.Values), token);
        return codes;
    }


    public ValueTask<string[]> ReplaceCodes( Database db, IEnumerable<string> recoveryCodes, CancellationToken token = default ) => db.TryCall(ReplaceCodes, db, recoveryCodes, token);
    public async ValueTask<string[]> ReplaceCodes( NpgsqlConnection connection, DbTransaction transaction, Database db, IEnumerable<string> recoveryCodes, CancellationToken token = default )
    {
        IAsyncEnumerable<RecoveryCodeRecord> old        = Codes(connection, transaction, db, token);
        RecoveryCodeRecord.Codes             dictionary = RecoveryCodeRecord.Create(this, recoveryCodes);
        string[]                             codes      = [.. dictionary.Keys];


        await db.RecoveryCodes.Delete(connection, transaction, old, token);
        await UserRecoveryCodeRecord.Replace(connection, transaction, db.UserRecoveryCodes, this, RecordID<RecoveryCodeRecord>.Create(dictionary.Values), token);
        return codes;
    }


    public IAsyncEnumerable<RecoveryCodeRecord> Codes( Database         db,         CancellationToken token )                                             => db.TryCall(Codes, db, token);
    public IAsyncEnumerable<RecoveryCodeRecord> Codes( NpgsqlConnection connection, DbTransaction     transaction, Database db, CancellationToken token ) => UserRecoveryCodeRecord.Where(connection, transaction, db.RecoveryCodes, this, token);


    public UserRecord WithRights<TEnum>( scoped in UserRights<TEnum> rights )
        where TEnum : struct, Enum
    {
        Rights = rights.ToString();
        return this;
    }
    public async ValueTask<UserModel> GetRights( NpgsqlConnection connection, DbTransaction transaction, Database db, CancellationToken token )
    {
        UserModel model = new(this);
        await foreach ( GroupRecord record in GetGroups(connection, transaction, db, token) ) { model.Groups.Add(record.ToGroupModel()); }

        await foreach ( RoleRecord record in GetRoles(connection, transaction, db, token) ) { model.Roles.Add(record.ToRoleModel()); }

        return model;
    }


    public override bool Equals( UserRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return base.Equals(other) && string.Equals(UserName, other.UserName, StringComparison.InvariantCultureIgnoreCase) && string.Equals(FullName, other.FullName, StringComparison.InvariantCultureIgnoreCase) && string.Equals(FirstName, other.FirstName, StringComparison.InvariantCultureIgnoreCase) && string.Equals(LastName, other.LastName, StringComparison.InvariantCultureIgnoreCase);
    }
    public override int CompareTo( UserRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }


        int userNameComparison = string.Compare(UserName, other.UserName, StringComparison.Ordinal);
        if ( userNameComparison != 0 ) { return userNameComparison; }

        int firstNameComparison = string.Compare(FirstName, other.FirstName, StringComparison.Ordinal);
        if ( firstNameComparison != 0 ) { return firstNameComparison; }

        int lastNameComparison = string.Compare(LastName, other.LastName, StringComparison.Ordinal);
        if ( lastNameComparison != 0 ) { return lastNameComparison; }

        int fullNameComparison = string.Compare(FullName, other.FullName, StringComparison.Ordinal);
        if ( fullNameComparison != 0 ) { return fullNameComparison; }

        int descriptionComparison = string.Compare(Description, other.Description, StringComparison.Ordinal);
        if ( descriptionComparison != 0 ) { return descriptionComparison; }

        int sessionIDComparison = Nullable.Compare(SessionID, other.SessionID);
        if ( sessionIDComparison != 0 ) { return sessionIDComparison; }

        int websiteComparison = string.Compare(Website, other.Website, StringComparison.Ordinal);
        if ( websiteComparison != 0 ) { return websiteComparison; }

        int emailComparison = string.Compare(Email, other.Email, StringComparison.Ordinal);
        if ( emailComparison != 0 ) { return emailComparison; }

        int phoneNumberComparison = string.Compare(PhoneNumber, other.PhoneNumber, StringComparison.Ordinal);
        if ( phoneNumberComparison != 0 ) { return phoneNumberComparison; }

        int extComparison = string.Compare(Ext, other.Ext, StringComparison.Ordinal);
        if ( extComparison != 0 ) { return extComparison; }

        int titleComparison = string.Compare(Title, other.Title, StringComparison.Ordinal);
        if ( titleComparison != 0 ) { return titleComparison; }

        int departmentComparison = string.Compare(Department, other.Department, StringComparison.Ordinal);
        if ( departmentComparison != 0 ) { return departmentComparison; }

        return string.Compare(Company, other.Company, StringComparison.Ordinal);
    }
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), UserName, FullName, FirstName, LastName);


    public static bool operator >( UserRecord  left, UserRecord right ) => left.CompareTo(right) > 0;
    public static bool operator >=( UserRecord left, UserRecord right ) => left.CompareTo(right) >= 0;
    public static bool operator <( UserRecord  left, UserRecord right ) => left.CompareTo(right) < 0;
    public static bool operator <=( UserRecord left, UserRecord right ) => left.CompareTo(right) <= 0;


    public UserModel ToUserModel() => ToUserModel<UserModel>();
    public TClass ToUserModel<TClass>()
        where TClass : UserModel<TClass, Guid, UserAddress, GroupModel, RoleModel>, ICreateUserModel<TClass, Guid, UserAddress, GroupModel, RoleModel>, IJsonModel<TClass>, new() => ToUserModel<TClass, UserAddress, GroupModel, RoleModel>();
    public TClass ToUserModel<TClass, TAddress, TGroupModel, TRoleModel>()
        where TClass : class, IUserData<Guid, TAddress, TGroupModel, TRoleModel>, ICreateUserModel<TClass, Guid, TAddress, TGroupModel, TRoleModel>, new()
        where TGroupModel : class, IGroupModel<TGroupModel, Guid>, IEquatable<TGroupModel>
        where TRoleModel : class, IRoleModel<TRoleModel, Guid>, IEquatable<TRoleModel>
        where TAddress : class, IAddress<TAddress, Guid>, IEquatable<TAddress> => TClass.Create(this);


    public ValueTask<UserModel> ToUserModel( NpgsqlConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) => ToUserModel<UserModel>(connection, transaction, db, token);
    public ValueTask<TClass> ToUserModel<TClass>( NpgsqlConnection connection, DbTransaction? transaction, Database db, CancellationToken token )
        where TClass : UserModel<TClass, Guid, UserAddress, GroupModel, RoleModel>, ICreateUserModel<TClass, Guid, UserAddress, GroupModel, RoleModel>, IJsonModel<TClass>, new() => ToUserModel<TClass, UserAddress, GroupModel, RoleModel>(connection, transaction, db, token);
    public async ValueTask<TClass> ToUserModel<TClass, TAddress, TGroupModel, TRoleModel>( NpgsqlConnection connection, DbTransaction? transaction, Database db, CancellationToken token )
        where TClass : class, IUserData<Guid, TAddress, TGroupModel, TRoleModel>, ICreateUserModel<TClass, Guid, TAddress, TGroupModel, TRoleModel>, new()
        where TGroupModel : class, IGroupModel<TGroupModel, Guid>, IEquatable<TGroupModel>
        where TRoleModel : class, IRoleModel<TRoleModel, Guid>, IEquatable<TRoleModel>
        where TAddress : class, IAddress<TAddress, Guid>, IEquatable<TAddress>
    {
        TClass model = TClass.Create(this);

        await foreach ( AddressRecord record in GetAddresses(connection, transaction, db, token) ) { model.Addresses.Add(record.ToAddressModel<TAddress>()); }

        await foreach ( GroupRecord record in GetGroups(connection, transaction, db, token) ) { model.Groups.Add(record.ToGroupModel<TGroupModel>()); }

        await foreach ( RoleRecord record in GetRoles(connection, transaction, db, token) ) { model.Roles.Add(record.ToRoleModel<TRoleModel>()); }

        return model;
    }


    public void Deconstruct( out string                UserName,
                             out string                FirstName,
                             out string                LastName,
                             out string                FullName,
                             out string                Gender,
                             out string                Description,
                             out string                Company,
                             out string                Department,
                             out string                Title,
                             out string                Website,
                             out SupportedLanguage     PreferredLanguage,
                             out string                Email,
                             out bool                  IsEmailConfirmed,
                             out string                PhoneNumber,
                             out string                Ext,
                             out bool                  IsPhoneNumberConfirmed,
                             out string                AuthenticatorKey,
                             out bool                  IsTwoFactorEnabled,
                             out bool                  IsActive,
                             out bool                  IsDisabled,
                             out Guid?                 SubscriptionID,
                             out DateTimeOffset?       SubscriptionExpires,
                             out DateTimeOffset?       LastBadAttempt,
                             out DateTimeOffset?       LastLogin,
                             out int?                  BadLogins,
                             out bool                  IsLocked,
                             out DateTimeOffset?       LockDate,
                             out DateTimeOffset?       LockoutEnd,
                             out string                RefreshToken,
                             out DateTimeOffset?       RefreshTokenExpiryTime,
                             out Guid?                 SessionID,
                             out string                SecurityStamp,
                             out string                ConcurrencyStamp,
                             out string                Rights,
                             out RecordID<UserRecord>? EscalateTo,
                             out JsonObject?           AdditionalData,
                             out string                PasswordHash,
                             out RecordID<FileRecord>? ImageID,
                             out RecordID<UserRecord>  ID,
                             out RecordID<UserRecord>? CreatedBy,
                             out DateTimeOffset        DateCreated,
                             out DateTimeOffset?       LastModified
    )
    {
        UserName               = this.UserName;
        FirstName              = this.FirstName;
        LastName               = this.LastName;
        FullName               = this.FullName;
        Gender                 = this.Gender;
        Description            = this.Description;
        Company                = this.Company;
        Department             = this.Department;
        Title                  = this.Title;
        Website                = this.Website;
        PreferredLanguage      = this.PreferredLanguage;
        Email                  = this.Email;
        IsEmailConfirmed       = this.IsEmailConfirmed;
        PhoneNumber            = this.PhoneNumber;
        Ext                    = this.Ext;
        IsPhoneNumberConfirmed = this.IsPhoneNumberConfirmed;
        AuthenticatorKey       = this.AuthenticatorKey;
        IsTwoFactorEnabled     = this.IsTwoFactorEnabled;
        IsActive               = this.IsActive;
        IsDisabled             = this.IsDisabled;
        SubscriptionID         = this.SubscriptionID;
        SubscriptionExpires    = this.SubscriptionExpires;
        LastBadAttempt         = this.LastBadAttempt;
        LastLogin              = this.LastLogin;
        BadLogins              = this.BadLogins;
        IsLocked               = this.IsLocked;
        LockDate               = this.LockDate;
        LockoutEnd             = this.LockoutEnd;
        RefreshToken           = this.RefreshToken;
        RefreshTokenExpiryTime = this.RefreshTokenExpiryTime;
        SessionID              = this.SessionID;
        SecurityStamp          = this.SecurityStamp;
        ConcurrencyStamp       = this.ConcurrencyStamp;
        Rights                 = this.Rights;
        EscalateTo             = this.EscalateTo;
        AdditionalData         = this.AdditionalData;
        PasswordHash           = this.PasswordHash;
        ImageID                = this.ImageID;
        ID                     = this.ID;
        CreatedBy              = this.CreatedBy;
        DateCreated            = this.DateCreated;
        LastModified           = this.LastModified;
    }



    #region Passwords

    [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool HasPassword() => !string.IsNullOrWhiteSpace(PasswordHash);


    public UserRecord WithPassword( string password )
    {
        if ( password.Length > MAX_PASSWORD_SIZE ) { throw new ArgumentException($"Password Must be less than {MAX_PASSWORD_SIZE} chars", nameof(password)); }

        PasswordHash = Database.DataProtector.Encrypt(password);
        return this;
    }
    public UserRecord WithPassword( in string password, scoped in Requirements requirements ) => WithPassword(password, requirements, out _);
    public UserRecord WithPassword( in string password, scoped in Requirements requirements, out PasswordValidator.Results results )
    {
        if ( requirements.maxLength > MAX_PASSWORD_SIZE ) { throw new ArgumentException($"Password Must be less than {MAX_PASSWORD_SIZE} chars", nameof(password)); }

        PasswordValidator validator = new(requirements);
        if ( !validator.Validate(password, out results) ) { return this; }

        return WithPassword(password);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool VerifyPassword( scoped ref UserRecord record, ILoginRequest request ) => VerifyPassword(ref record, request.Password);
    public static bool VerifyPassword( scoped ref UserRecord record, in string password )
    {
        string value = Database.DataProtector.Decrypt(record.PasswordHash);

        if ( string.Equals(value, password, StringComparison.Ordinal) )
        {
            record = record.SetActive();
            return true;
        }

        record = record.MarkBadLogin();
        return false;
    }

    #endregion



    #region Owners

    public async ValueTask<ErrorOrResult<UserRecord>> GetBoss( NpgsqlConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) =>
        EscalateTo.HasValue
            ? await db.Users.Get(connection, transaction, EscalateTo.Value, token)
            : Error.Create(Status.Gone, StringTags.Empty);


    public bool DoesNotOwn<TClass>( TClass record )
        where TClass : OwnedTableRecord<TClass>, ITableRecord<TClass> => record.CreatedBy != ID;
    public bool Owns<TClass>( TClass record )
        where TClass : OwnedTableRecord<TClass>, ITableRecord<TClass> => record.CreatedBy == ID;

    #endregion



    #region Controls

    public UserRecord MarkBadLogin() => MarkBadLogin(DefaultLockoutTime);
    public UserRecord MarkBadLogin( scoped in TimeSpan lockoutTime, in int badLoginDisableThreshold = DEFAULT_BAD_LOGIN_DISABLE_THRESHOLD )
    {
        int badLogins = BadLogins ?? 0;
        badLogins++;
        bool           isDisabled = badLogins > badLoginDisableThreshold;
        bool           isLocked   = isDisabled || !IsActive;
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
    public UserRecord Lock() => Lock(DefaultLockoutTime);
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

        return !record.IsDisabled && record.IsActive;
    }

    #endregion



    #region Updaters

    public override UserRecord WithAdditionalData( JsonObject? value )
    {
        if ( value is null || value.Count <= 0 ) { return this; }

        base.WithAdditionalData(value);
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
        EscalateTo        = RecordID<UserRecord>.TryCreate(value.EscalateTo);
        ImageID           = RecordID<FileRecord>.TryCreate(value.ImageID);

        // CreatedBy = value.CreatedBy;

        return WithAdditionalData(value.AdditionalData);
    }

    #endregion



    #region Tokens

    public static bool CheckRefreshToken( ref UserRecord record, Tokens token, bool hashed = true ) => CheckRefreshToken(ref record, token.RefreshToken, hashed);
    public static bool CheckRefreshToken( ref UserRecord record, string? refreshToken, bool hashed = true )
    {
        // ReSharper disable once InvertIf
        if ( record.RefreshTokenExpiryTime.HasValue && DateTimeOffset.UtcNow > record.RefreshTokenExpiryTime.Value )
        {
            record = record.WithNoRefreshToken();
            return false;
        }

        return hashed
                   ? string.Equals(record.RefreshToken, refreshToken?.Hash_SHA512(), StringComparison.Ordinal)
                   : string.Equals(record.RefreshToken, refreshToken,                StringComparison.Ordinal);
    }


    public UserRecord WithNoRefreshToken()                                                                 => WithRefreshToken(string.Empty);
    public UserRecord WithRefreshToken( Tokens token, DateTimeOffset? date, string? securityStamp = null ) => WithRefreshToken(token.RefreshToken, date, securityStamp);
    public UserRecord WithRefreshToken( string? refreshToken, DateTimeOffset? date = null, string? securityStamp = null, bool hashed = true )
    {
        if ( string.IsNullOrEmpty(refreshToken) )
        {
            RefreshToken           = string.Empty;
            RefreshTokenExpiryTime = null;
            SecurityStamp          = securityStamp ?? string.Empty;
            return Modified();
        }

        date ??= DateTimeOffset.UtcNow;
        string hash = refreshToken.Hash_SHA512();
        SecurityStamp = securityStamp ?? hash;

        RefreshToken = hashed
                           ? hash
                           : refreshToken;

        RefreshTokenExpiryTime = date;
        return Modified();
    }

    #endregion



    #region Roles

    public async ValueTask<bool>                 TryAdd( NpgsqlConnection       connection, DbTransaction  transaction, Database db, AddressRecord                              value, CancellationToken token ) => await UserAddressRecord.TryAdd(connection, transaction, db.UserAddresses, this, value, token);
    public       IAsyncEnumerable<AddressRecord> GetAddresses( NpgsqlConnection connection, DbTransaction? transaction, Database db, [EnumeratorCancellation] CancellationToken token = default )                => UserAddressRecord.Where(connection, transaction, db.Addresses, this, token);
    public async ValueTask<bool>                 HasAddress( NpgsqlConnection   connection, DbTransaction  transaction, Database db, AddressRecord                              value, CancellationToken token ) => await UserAddressRecord.Exists(connection, transaction, db.UserAddresses, this, value, token);
    public async ValueTask                       Remove( NpgsqlConnection       connection, DbTransaction  transaction, Database db, AddressRecord                              value, CancellationToken token ) => await UserAddressRecord.Delete(connection, transaction, db.UserAddresses, this, value, token);

    #endregion



    #region Roles

    public async ValueTask<bool>              TryAdd( NpgsqlConnection   connection, DbTransaction  transaction, Database db, RoleRecord        value, CancellationToken token ) => await UserRoleRecord.TryAdd(connection, transaction, db.UserRoles, this, value, token);
    public       IAsyncEnumerable<RoleRecord> GetRoles( NpgsqlConnection connection, DbTransaction? transaction, Database db, CancellationToken token = default )                => UserRoleRecord.Where(connection, transaction, db.Roles, this, token);
    public async ValueTask<bool>              HasRole( NpgsqlConnection  connection, DbTransaction  transaction, Database db, RoleRecord        value, CancellationToken token ) => await UserRoleRecord.Exists(connection, transaction, db.UserRoles, this, value, token);
    public async ValueTask                    Remove( NpgsqlConnection   connection, DbTransaction  transaction, Database db, RoleRecord        value, CancellationToken token ) => await UserRoleRecord.Delete(connection, transaction, db.UserRoles, this, value, token);

    #endregion



    #region Groups

    public async ValueTask<bool>               TryAdd( NpgsqlConnection        connection, DbTransaction  transaction, Database db, GroupRecord       value, CancellationToken token ) => await UserGroupRecord.TryAdd(connection, transaction, db.UserGroups, this, value, token);
    public       IAsyncEnumerable<GroupRecord> GetGroups( NpgsqlConnection     connection, DbTransaction? transaction, Database db, CancellationToken token = default )                => UserGroupRecord.Where(connection, transaction, db.Groups, this, token);
    public async ValueTask<bool>               IsPartOfGroup( NpgsqlConnection connection, DbTransaction  transaction, Database db, GroupRecord       value, CancellationToken token ) => await UserGroupRecord.Exists(connection, transaction, db.UserGroups, this, value, token);
    public async ValueTask                     Remove( NpgsqlConnection        connection, DbTransaction  transaction, Database db, GroupRecord       value, CancellationToken token ) => await UserGroupRecord.Delete(connection, transaction, db.UserGroups, this, value, token);

    #endregion



    #region Claims

    public async  ValueTask<Claim[]>                   GetUserClaims( NpgsqlConnection connection, DbTransaction? transaction, Database db, ClaimType       types,     CancellationToken token )                          => ( await ToUserModel(connection, transaction, db, token) ).GetClaims(types);
    public static ValueTask<ErrorOrResult<UserRecord>> TryFromClaims( NpgsqlConnection connection, DbTransaction  transaction, Database db, ClaimsPrincipal principal, ClaimType         types, CancellationToken token ) => TryFromClaims(connection, transaction, db, principal.Claims.ToArray(), types, token);
    public static ValueTask<ErrorOrResult<UserRecord>> TryFromClaims( NpgsqlConnection connection, DbTransaction transaction, Database db, scoped in ReadOnlySpan<Claim> claims, in ClaimType types, CancellationToken token )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(ID), Guid.Parse(claims.Single(static ( ref readonly Claim x ) => x.IsUserID()).Value));

        if ( types.HasFlag(ClaimType.UserName) ) { parameters.Add(nameof(UserName), claims.Single(static ( ref readonly Claim x ) => x.IsUserName()).Value); }

        if ( types.HasFlag(ClaimType.FirstName) ) { parameters.Add(nameof(FirstName), claims.Single(static ( ref readonly Claim x ) => x.IsFirstName()).Value); }

        if ( types.HasFlag(ClaimType.LastName) ) { parameters.Add(nameof(LastName), claims.Single(static ( ref readonly Claim x ) => x.IsLastName()).Value); }

        if ( types.HasFlag(ClaimType.FullName) ) { parameters.Add(nameof(FullName), claims.Single(static ( ref readonly Claim x ) => x.IsFullName()).Value); }

        if ( types.HasFlag(ClaimType.Email) ) { parameters.Add(nameof(Email), claims.Single(static ( ref readonly Claim x ) => x.IsEmail()).Value); }

        if ( types.HasFlag(ClaimType.MobilePhone) ) { parameters.Add(nameof(PhoneNumber), claims.Single(static ( ref readonly Claim x ) => x.IsMobilePhone()).Value); }

        if ( types.HasFlag(ClaimType.WebSite) ) { parameters.Add(nameof(Website), claims.Single(static ( ref readonly Claim x ) => x.IsWebSite()).Value); }

        return db.Users.Get(connection, transaction, true, parameters, token);
    }
    public static async IAsyncEnumerable<UserRecord> TryFromClaims( NpgsqlConnection connection, DbTransaction transaction, Database db, Claim claim, [EnumeratorCancellation] CancellationToken token = default )
    {
        DynamicParameters parameters = new();

        switch ( claim.Type )
        {
            case ClaimTypes.NameIdentifier:
                parameters.Add(nameof(UserName), claim.Value);
                break;

            case ClaimTypes.Sid:
                parameters.Add(nameof(ID), Guid.Parse(claim.Value));
                break;

            case ClaimTypes.GivenName:
                parameters.Add(nameof(FirstName), claim.Value);
                break;

            case ClaimTypes.Surname:
                parameters.Add(nameof(LastName), claim.Value);
                break;

            case ClaimTypes.Name:
                parameters.Add(nameof(FullName), claim.Value);
                break;

            case ClaimTypes.Email:
                parameters.Add(nameof(Email), claim.Value);
                break;

            case ClaimTypes.MobilePhone:
                parameters.Add(nameof(PhoneNumber), claim.Value);
                break;

            case ClaimTypes.Webpage:
                parameters.Add(nameof(Website), claim.Value);
                break;
        }

        await foreach ( UserRecord record in db.Users.Where(connection, transaction, true, parameters, token) ) { yield return record; }
    }

    #endregion
}
