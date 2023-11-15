namespace Jakar.Database;


[ Serializable, Table( "Users" ) ]
public sealed record UserRecord( Guid                                                                                                      UserID,
                                 [ property: ProtectedPersonalData, MaxLength( 256 ) ] string                                              UserName,
                                 string                                                                                                    FirstName,
                                 string                                                                                                    LastName,
                                 string                                                                                                    FullName,
                                 [ property: MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes ) ] string                Rights,
                                 [ property: MaxLength( 256 ) ]                                                      string                Gender,
                                 string                                                                                                    Company,
                                 string                                                                                                    Description,
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
) : OwnedTableRecord<UserRecord>( ID, CreatedBy, OwnerUserID, DateCreated, LastModified ), IDbReaderMapping<UserRecord>, IUserData<UserRecord>, IRefreshToken, IUserID, IUserDataRecord, UserRights.IRights
{
    private static readonly PasswordHasher<UserRecord>    _hasher         = new();
    private                 IDictionary<string, JToken?>? _additionalData = AdditionalData;


    public static string TableName { get; } = typeof(UserRecord).GetTableName();


    [ ProtectedPersonalData, MaxLength( int.MaxValue ) ]
    public IDictionary<string, JToken?>? AdditionalData
    {
        get => _additionalData;
        set => _additionalData = value;
    }
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string            Company           { get; set; } = Company;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string            Department        { get; set; } = Department;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string            Description       { get; set; } = Description;
    [ ProtectedPersonalData, MaxLength( 1024 ) ] public string            Email             { get; set; } = Email;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string            Ext               { get; set; } = Ext;
    [ ProtectedPersonalData, MaxLength( 256 ) ]  public string            FirstName         { get; set; } = FirstName;
    [ ProtectedPersonalData, MaxLength( 512 ) ]  public string            FullName          { get; set; } = FullName;
    public                                              DateTimeOffset?   LastLogin         { get; set; } = LastLogin;
    [ ProtectedPersonalData, MaxLength( 256 ) ] public  string            LastName          { get; set; } = LastName;
    [ ProtectedPersonalData, MaxLength( 256 ) ] public  string            PhoneNumber       { get; set; } = PhoneNumber;
    public                                              SupportedLanguage PreferredLanguage { get; set; } = PreferredLanguage;
    [ ProtectedPersonalData, MaxLength( 256 ) ] public  string            Title             { get; set; } = Title;
    Guid IUserID.                                                         UserID            => UserID;
    [ ProtectedPersonalData, MaxLength( 4096 ) ] public string            Website           { get; set; } = Website;


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
        parameters.Add( nameof(EscalateTo),             EscalateTo );
        parameters.Add( nameof(AdditionalData),         AdditionalData );
        return parameters;
    }

    public static UserRecord Create( DbDataReader reader )
    {
        Guid                  userID                 = reader.GetGuid( nameof(UserID) );
        string                userName               = reader.GetString( nameof(UserName) );
        string                firstName              = reader.GetString( nameof(FirstName) );
        string                lastName               = reader.GetString( nameof(LastName) );
        string                fullName               = reader.GetString( nameof(FullName) );
        string                rights                 = reader.GetString( nameof(Rights) );
        string                gender                 = reader.GetString( nameof(Gender) );
        string                company                = reader.GetString( nameof(Company) );
        string                description            = reader.GetString( nameof(Description) );
        string                department             = reader.GetString( nameof(Department) );
        string                title                  = reader.GetString( nameof(Title) );
        string                website                = reader.GetString( nameof(Website) );
        SupportedLanguage     preferredLanguage      = EnumSqlHandler<SupportedLanguage>.Instance.Parse( reader.GetValue( nameof(PreferredLanguage) ) );
        string                email                  = reader.GetString( nameof(Email) );
        bool                  isEmailConfirmed       = reader.GetBoolean( nameof(IsEmailConfirmed) );
        string                phoneNumber            = reader.GetString( nameof(PhoneNumber) );
        string                ext                    = reader.GetString( nameof(Ext) );
        bool                  isPhoneNumberConfirmed = reader.GetBoolean( nameof(IsPhoneNumberConfirmed) );
        bool                  isTwoFactorEnabled     = reader.GetBoolean( nameof(IsTwoFactorEnabled) );
        var                   lastBadAttempt         = reader.GetFieldValue<DateTimeOffset?>( nameof(LastBadAttempt) );
        var                   lastLogin              = reader.GetFieldValue<DateTimeOffset?>( nameof(LastLogin) );
        int                   badLogins              = reader.GetFieldValue<int>( nameof(BadLogins) );
        bool                  isLocked               = reader.GetBoolean( nameof(IsLocked) );
        var                   lockDate               = reader.GetFieldValue<DateTimeOffset?>( nameof(LockDate) );
        var                   lockoutEnd             = reader.GetFieldValue<DateTimeOffset?>( nameof(LockoutEnd) );
        string                passwordHash           = reader.GetString( nameof(PasswordHash) );
        string                refreshToken           = reader.GetString( nameof(RefreshToken) );
        var                   refreshTokenExpiryTime = reader.GetFieldValue<DateTimeOffset?>( nameof(RefreshTokenExpiryTime) );
        var                   sessionID              = reader.GetFieldValue<Guid?>( nameof(SessionID) );
        bool                  isActive               = reader.GetBoolean( nameof(IsActive) );
        bool                  isDisabled             = reader.GetBoolean( nameof(IsDisabled) );
        string                securityStamp          = reader.GetString( nameof(SecurityStamp) );
        string                authenticatorKey       = reader.GetString( nameof(AuthenticatorKey) );
        string                concurrencyStamp       = reader.GetString( nameof(AuthenticatorKey) );
        var                   escalateTo             = new RecordID<UserRecord>( reader.GetFieldValue<Guid>( nameof(EscalateTo) ) );
        var                   additionalData         = JsonConvert.DeserializeObject<Dictionary<string, JToken?>>( reader.GetString( nameof(AdditionalData) ) );
        RecordID<UserRecord>  id                     = RecordID<UserRecord>.ID( reader );
        RecordID<UserRecord>? createdBy              = RecordID<UserRecord>.CreatedBy( reader );
        var                   ownerUserID            = reader.GetFieldValue<Guid?>( nameof(OwnerUserID) );
        var                   dateCreated            = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        var                   lastModified           = reader.GetFieldValue<DateTimeOffset?>( nameof(LastModified) );


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

        return record.WithPassword( request.UserPassword ).Enable();
    }
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
                                                                                                                           data.AdditionalData,
                                                                                                                           RecordID<UserRecord>.New(),
                                                                                                                           caller?.ID,
                                                                                                                           caller?.UserID,
                                                                                                                           DateTimeOffset.UtcNow);
    public static UserRecord Create( string userName, string password, UserRights rights, UserRecord? caller = default ) => Create( userName, password, rights.ToString(), caller );
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
        parameters.Add( nameof(UserName), request.UserName );
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
        string[]                                        codes      = dictionary.Keys.GetArray();


        await db.RecoveryCodes.Delete( connection, transaction, old, token );
        await UserRecoveryCodeRecord.Replace( connection, transaction, db.UserRecoveryCodes, this, dictionary.Values, token );
        return codes;
    }


     public ValueTask<string[]> ReplaceCodes( Database db, IEnumerable<string> recoveryCodes, CancellationToken token = default ) => db.TryCall( ReplaceCodes, db, recoveryCodes, token );
    
    public async ValueTask<string[]> ReplaceCodes( DbConnection connection, DbTransaction transaction, Database db, IEnumerable<string> recoveryCodes, CancellationToken token = default )
    {
        IAsyncEnumerable<RecoveryCodeRecord>            old        = Codes( connection, transaction, db, token );
        IReadOnlyDictionary<string, RecoveryCodeRecord> dictionary = RecoveryCodeRecord.Create( this, recoveryCodes );
        string[]                                        codes      = dictionary.Keys.GetArray();


        await db.RecoveryCodes.Delete( connection, transaction, old, token );
        await UserRecoveryCodeRecord.Replace( connection, transaction, db.UserRecoveryCodes, this, dictionary.Values, token );
        return codes;
    }


     public IAsyncEnumerable<RecoveryCodeRecord> Codes( Database db, CancellationToken token ) => db.TryCall( Codes, db, token );
    
    public IAsyncEnumerable<RecoveryCodeRecord> Codes( DbConnection connection, DbTransaction transaction, Database db, CancellationToken token ) =>
        UserRecoveryCodeRecord.Where( connection, transaction, db.UserRecoveryCodes, db.RecoveryCodes, this, token );


    public UserRights GetRights() => UserRights.Create( this );
    public async ValueTask<UserRights> GetRights<T>( DbConnection connection, DbTransaction transaction, Database db, CancellationToken token ) where T : struct, Enum
    {
        int totalRightCount = Enum.GetValues<T>().Length;

        return await GetRights( connection, transaction, db, totalRightCount, token );
    }
    public async ValueTask<UserRights> GetRights( DbConnection connection, DbTransaction transaction, Database db, int totalRightCount, CancellationToken token )
    {
        List<GroupRecord> groups = await GetGroups( connection, transaction, db, token ).ToList( token );

        List<RoleRecord> roles = await GetRoles( connection, transaction, db, token ).ToList( token );

        var rights = new List<UserRights.IRights>( 1 + groups.Count + roles.Count );

        rights.AddRange( groups );
        rights.AddRange( roles );
        rights.Add( this );

        return UserRights.Merge( rights, totalRightCount );
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

    public async ValueTask<UserRecord?> GetBoss( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token ) =>
        EscalateTo.HasValue
            ? await db.Users.Get( connection, transaction, EscalateTo.Value, token )
            : default;


    public bool DoesNotOwn<TRecord>( TRecord record ) where TRecord : OwnedTableRecord<TRecord>, IDbReaderMapping<TRecord> => record.OwnerUserID != UserID;
    public bool Owns<TRecord>( TRecord       record ) where TRecord : OwnedTableRecord<TRecord>, IDbReaderMapping<TRecord> => record.OwnerUserID == UserID;

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


        return string.Equals( record.RefreshToken, token?.GetHashCode().ToString(), StringComparison.Ordinal );
    }
    public static bool IsHashedRefreshToken( Tokens token, ref UserRecord record ) => IsHashedRefreshToken( token.RefreshToken, ref record );


    public UserRecord WithNoRefreshToken() => WithRefreshToken( default, default );
    public UserRecord WithRefreshToken( string? token, in DateTimeOffset? date, in bool hashed = true )
    {
        if ( hashed ) { token = token?.GetHashCode().ToString(); }

        return this with
               {
                   RefreshToken = token ?? string.Empty,
                   RefreshTokenExpiryTime = date
               };
    }
    public UserRecord WithRefreshToken( string? token, in DateTimeOffset? date, string securityStamp, in bool hashed = true )
    {
        if ( hashed ) { token = token?.GetHashCode().ToString(); }

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

    
    public async ValueTask<bool> TryAdd( DbConnection connection, DbTransaction transaction, Database db, RoleRecord value, CancellationToken token ) =>
        await UserRoleRecord.TryAdd( connection, transaction, db.UserRoles, this, value, token );
    public IAsyncEnumerable<RoleRecord> GetRoles( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token = default ) =>
        UserRoleRecord.Where( connection, transaction, db.UserRoles, db.Roles, this, token );
    public async ValueTask<bool> HasRole( DbConnection connection, DbTransaction transaction, Database db, RoleRecord value, CancellationToken token ) =>
        await UserRoleRecord.Exists( connection, transaction, db.UserRoles, this, value, token );
    public async ValueTask Remove( DbConnection connection, DbTransaction transaction, Database db, RoleRecord value, CancellationToken token ) =>
        await UserRoleRecord.Delete( connection, transaction, db.UserRoles, this, value, token );

    #endregion



    #region Groups

    
    public async ValueTask<bool> TryAdd( DbConnection connection, DbTransaction transaction, Database db, GroupRecord value, CancellationToken token ) =>
        await UserGroupRecord.TryAdd( connection, transaction, db.UserGroups, this, value, token );
    public IAsyncEnumerable<GroupRecord> GetGroups( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token = default ) =>
        UserGroupRecord.Where( connection, transaction, db.UserGroups, db.Groups, this, token );
    public async ValueTask<bool> IsPartOfGroup( DbConnection connection, DbTransaction transaction, Database db, GroupRecord value, CancellationToken token ) =>
        await UserGroupRecord.Exists( connection, transaction, db.UserGroups, this, value, token );
    public async ValueTask Remove( DbConnection connection, DbTransaction transaction, Database db, GroupRecord value, CancellationToken token ) =>
        await UserGroupRecord.Delete( connection, transaction, db.UserGroups, this, value, token );

    #endregion



    #region Claims

    public async ValueTask<Claim[]> GetUserClaims( DbConnection connection, DbTransaction? transaction, Database db, ClaimType types, CancellationToken token )
    {
        var groups = new List<string>();
        var roles  = new List<string>();

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

        if ( types.HasFlag( ClaimType.GroupSid ) ) { claims.AddRange( from nameOfGroup in groups select new Claim( ClaimTypes.GroupSid, nameOfGroup, ClaimValueTypes.String ) ); }

        if ( types.HasFlag( ClaimType.Role ) ) { claims.AddRange( from role in roles select new Claim( ClaimTypes.Role, role, ClaimValueTypes.String ) ); }

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

        parameters.Add( nameof(UserName), claims.Single( x => x.Type == ClaimTypes.NameIdentifier ).Value );

        parameters.Add( nameof(UserID), Guid.Parse( claims.Single( x => x.Type == ClaimTypes.Sid ).Value ) );


        if ( types.HasFlag( ClaimType.FirstName ) ) { parameters.Add( nameof(FirstName), claims.Single( x => x.Type == ClaimTypes.GivenName ).Value ); }

        if ( types.HasFlag( ClaimType.LastName ) ) { parameters.Add( nameof(LastName), claims.Single( x => x.Type == ClaimTypes.Surname ).Value ); }

        if ( types.HasFlag( ClaimType.FullName ) ) { parameters.Add( nameof(FullName), claims.Single( x => x.Type == ClaimTypes.Name ).Value ); }

        if ( types.HasFlag( ClaimType.Email ) ) { parameters.Add( nameof(Email), claims.Single( x => x.Type == ClaimTypes.Email ).Value ); }

        if ( types.HasFlag( ClaimType.MobilePhone ) ) { parameters.Add( nameof(PhoneNumber), claims.Single( x => x.Type == ClaimTypes.MobilePhone ).Value ); }

        if ( types.HasFlag( ClaimType.WebSite ) ) { parameters.Add( nameof(Website), claims.Single( x => x.Type == ClaimTypes.Webpage ).Value ); }

        return await db.Users.Get( connection, transaction, true, parameters, token );
    }
    public static async IAsyncEnumerable<UserRecord> TryFromClaims( DbConnection connection, DbTransaction transaction, Database db, Claim claim, [ EnumeratorCancellation ] CancellationToken token = default )
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
