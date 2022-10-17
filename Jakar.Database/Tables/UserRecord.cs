// ToothFairyDispatch :: ToothFairyDispatch.Cloud
// 08/29/2022  9:55 PM

using System.Security.Claims;
using Jakar.Database.Implementations;
using Microsoft.IdentityModel.Tokens;



namespace Jakar.Database;


[Serializable]
[Table( "Users" )]
public sealed record UserRecord : TableRecord<UserRecord>, IUserRecord<UserRecord>
{
    public const            char                       RECOVERY_CODE_SEPARATOR = ';';
    private static readonly PasswordHasher<UserRecord> _hasher                 = new();
    private                 bool                       _isActive;
    private                 bool                       _isDisabled;
    private                 bool                       _isEmailConfirmed;
    private                 bool                       _isLocked;
    private                 bool                       _isPhoneNumberConfirmed;
    private                 bool                       _isTwoFactorEnabled;
    private                 DateTimeOffset?            _lastActive;
    private                 DateTimeOffset?            _lastBadAttempt;
    private                 DateTimeOffset?            _lockDate;
    private                 DateTimeOffset?            _lockEnd;
    private                 DateTimeOffset?            _refreshTokenExpiryTime;
    private                 DateTimeOffset?            _subscriptionExpires;
    private                 DateTimeOffset?            _tokenExpiration;
    private                 Guid?                      _sessionID;
    private                 int                        _badLogins;
    private                 long?                      _escalateTo;
    private                 long?                      _rights;
    private                 long?                      _subscriptionID;
    private                 string                     _firstName     = string.Empty;
    private                 string                     _lastName      = string.Empty;
    private                 string                     _passwordHash  = string.Empty;
    private                 string                     _recoveryCodes = string.Empty;
    private                 string                     _userName      = string.Empty;
    private                 string?                    _additionalData;
    private                 string?                    _address;
    private                 string?                    _city;
    private                 string?                    _company;
    private                 string?                    _concurrencyStamp;
    private                 string?                    _country;
    private                 string?                    _department;
    private                 string?                    _description;
    private                 string?                    _email;
    private                 string?                    _ext;
    private                 string?                    _fullName;
    private                 string?                    _line1;
    private                 string?                    _line2;
    private                 string?                    _loginProvider;
    private                 string?                    _phoneNumber;
    private                 string?                    _postalCode;
    private                 string?                    _providerDisplayName;
    private                 string?                    _providerKey;
    private                 string?                    _refreshToken;
    private                 string?                    _securityStamp;
    private                 string?                    _state;
    private                 string?                    _title;
    private                 string?                    _website;
    private                 SupportedLanguage          _preferredLanguage;


    public bool IsActive
    {
        get => _isActive;
        set => SetProperty( ref _isActive, value );
    }

    public bool IsDisabled
    {
        get => _isDisabled;
        set => SetProperty( ref _isDisabled, value );
    }

    public bool IsEmailConfirmed
    {
        get => _isEmailConfirmed;
        set => SetProperty( ref _isEmailConfirmed, value );
    }

    public bool IsLocked
    {
        get => _isLocked;
        set => SetProperty( ref _isLocked, value );
    }

    public bool IsPhoneNumberConfirmed
    {
        get => _isPhoneNumberConfirmed;
        set => SetProperty( ref _isPhoneNumberConfirmed, value );
    }

    public bool IsTwoFactorEnabled
    {
        get => _isTwoFactorEnabled;
        set => SetProperty( ref _isTwoFactorEnabled, value );
    }

    public DateTimeOffset? LastActive
    {
        get => _lastActive;
        set => SetProperty( ref _lastActive, value );
    }
    public DateTimeOffset? LastBadAttempt
    {
        get => _lastBadAttempt;
        set => SetProperty( ref _lastBadAttempt, value );
    }
    public DateTimeOffset? LockDate
    {
        get => _lockDate;
        set => SetProperty( ref _lockDate, value );
    }
    public DateTimeOffset? LockoutEnd
    {
        get => _lockEnd;
        set => SetProperty( ref _lockEnd, value );
    }
    public DateTimeOffset? RefreshTokenExpiryTime
    {
        get => _refreshTokenExpiryTime;
        set => SetProperty( ref _refreshTokenExpiryTime, value );
    }
    public DateTimeOffset? SubscriptionExpires
    {
        get => _subscriptionExpires;
        set => SetProperty( ref _subscriptionExpires, value );
    }
    public DateTimeOffset? TokenExpiration
    {
        get => _tokenExpiration;
        set => SetProperty( ref _tokenExpiration, value );
    }

    public Guid? SessionID
    {
        get => _sessionID;
        set => SetProperty( ref _sessionID, value );
    }


    IDictionary<string, JToken?>? JsonModels.IJsonModel.AdditionalData
    {
        get => AdditionalData?.FromJson<Dictionary<string, JToken?>>();
        set => AdditionalData = value?.ToJson();
    }

    public int BadLogins
    {
        get => _badLogins;
        set => SetProperty( ref _badLogins, value );
    }
    long? IUserRecord<UserRecord>.CreatedBy => CreatedBy;

    public long? EscalateTo
    {
        get => _escalateTo;
        set => SetProperty( ref _escalateTo, value );
    }


    public long? Rights
    {
        get => _rights;
        set => SetProperty( ref _rights, value );
    }

    public long? SubscriptionID
    {
        get => _subscriptionID;
        set => SetProperty( ref _subscriptionID, value );
    }

    [MaxLength( 256 )]
    public string FirstName
    {
        get => _firstName;
        set => SetProperty( ref _firstName, value );
    }

    [MaxLength( 256 )]
    public string LastName
    {
        get => _lastName;
        set => SetProperty( ref _lastName, value );
    }

    [MaxLength( int.MaxValue )]
    public string PasswordHash
    {
        get => _passwordHash;
        set => SetProperty( ref _passwordHash, value );
    }

    [MaxLength( 10240 )]
    public string RecoveryCodes
    {
        get => _recoveryCodes;
        set => SetProperty( ref _recoveryCodes, value );
    }

    [MaxLength( 256 )]
    public string UserName
    {
        get => _userName;
        set => SetProperty( ref _userName, value );
    }

    [MaxLength( int.MaxValue )]
    public string? AdditionalData
    {
        get => _additionalData;
        set => SetProperty( ref _additionalData, value );
    }

    [MaxLength( 4096 )]
    public string? Address
    {
        get => _address;
        set => SetProperty( ref _address, value );
    }

    [MaxLength( 256 )]
    public string? City
    {
        get => _city;
        set => SetProperty( ref _city, value );
    }

    [MaxLength( 256 )]
    public string? Company
    {
        get => _company;
        set => SetProperty( ref _company, value );
    }

    [MaxLength( int.MaxValue )]
    public string? ConcurrencyStamp
    {
        get => _concurrencyStamp;
        set => SetProperty( ref _concurrencyStamp, value );
    }

    [MaxLength( 256 )]
    public string? Country
    {
        get => _country;
        set => SetProperty( ref _country, value );
    }

    [MaxLength( 256 )]
    public string? Department
    {
        get => _department;
        set => SetProperty( ref _department, value );
    }

    [MaxLength( 256 )]
    public string? Description
    {
        get => _description;
        set => SetProperty( ref _description, value );
    }

    [MaxLength( 1024 )]
    public string? Email
    {
        get => _email;
        set => SetProperty( ref _email, value );
    }

    [MaxLength( 256 )]
    public string? Ext
    {
        get => _ext;
        set => SetProperty( ref _ext, value );
    }

    [MaxLength( 512 )]
    public string? FullName
    {
        get => _fullName;
        set => SetProperty( ref _fullName, value );
    }

    [MaxLength( 512 )]
    public string? Line1
    {
        get => _line1;
        set => SetProperty( ref _line1, value );
    }

    [MaxLength( 256 )]
    public string? Line2
    {
        get => _line2;
        set => SetProperty( ref _line2, value );
    }

    [MaxLength( int.MaxValue )]
    public string? LoginProvider
    {
        get => _loginProvider;
        set => SetProperty( ref _loginProvider, value );
    }

    [MaxLength( 256 )]
    public string? PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty( ref _phoneNumber, value );
    }

    [MaxLength( 256 )]
    public string? PostalCode
    {
        get => _postalCode;
        set => SetProperty( ref _postalCode, value );
    }

    [MaxLength( int.MaxValue )]
    public string? ProviderDisplayName
    {
        get => _providerDisplayName;
        set => SetProperty( ref _providerDisplayName, value );
    }

    [MaxLength( int.MaxValue )]
    public string? ProviderKey
    {
        get => _providerKey;
        set => SetProperty( ref _providerKey, value );
    }

    [MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes )]
    public string? RefreshToken
    {
        get => _refreshToken;
        set => SetProperty( ref _refreshToken, value );
    }

    [MaxLength( int.MaxValue )]
    public string? SecurityStamp
    {
        get => _securityStamp;
        set => SetProperty( ref _securityStamp, value );
    }

    [MaxLength( 256 )]
    public string? State
    {
        get => _state;
        set => SetProperty( ref _state, value );
    }

    [MaxLength( 256 )]
    public string? Title
    {
        get => _title;
        set => SetProperty( ref _title, value );
    }

    [MaxLength( 4096 )]
    public string? Website
    {
        get => _website;
        set => SetProperty( ref _website, value );
    }

    public SupportedLanguage PreferredLanguage
    {
        get => _preferredLanguage;
        set => SetProperty( ref _preferredLanguage, value );
    }


    public UserRecord() { }
    public UserRecord( IUserData value, long? rights = default )
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
        UserID            = Guid.NewGuid();
    }
    public UserRecord( IUserData value, UserRecord caller, long? rights = default ) : base( caller )
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
        UserID            = Guid.NewGuid();
    }
    public void AddRight<TRights>( TRights right ) where TRights : struct, Enum => Rights = (Rights ?? 0) | right.AsLong();


    public async ValueTask AddRole( DbConnection connection, DbTransaction transaction, Database db, RoleRecord role, CancellationToken token )
    {
        UserRoleRecord? record = await db.UserRoles.Get( connection, transaction, true, UserRoleRecord.GetDynamicParameters( this, role ), token );

        if (record is null)
        {
            record = new UserRoleRecord( this, role );
            await db.UserRoles.Insert( connection, transaction, record, token );
        }
    }


    public UserRecord AddUserLoginInfo( UserLoginInfo info )
    {
        LoginProvider       = info.LoginProvider;
        ProviderKey         = info.ProviderKey;
        ProviderDisplayName = info.ProviderDisplayName;
        return this;
    }
    public string[] Codes() => RecoveryCodes.Split( RECOVERY_CODE_SEPARATOR );
    public int CountCodes() => Codes()
       .Length;
    public static UserRecord Create<TRights>( IUserData value, TRights    rights ) where TRights : struct, Enum => new(value, rights.AsLong());
    public static UserRecord Create<TRights>( IUserData value, UserRecord caller, TRights rights ) where TRights : struct, Enum => new(value, caller, rights.AsLong());
    public bool DoesNotOwn<TRecord>( TRecord            record ) where TRecord : TableRecord<TRecord> => record.CreatedBy != ID;


    [SuppressMessage( "ReSharper", "NonReadonlyMemberInGetHashCode" )]
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add( base.GetHashCode() );
        hashCode.Add( UserName );
        hashCode.Add( FirstName );
        hashCode.Add( LastName );
        hashCode.Add( FullName );
        hashCode.Add( Description );
        hashCode.Add( Address );
        hashCode.Add( Line1 );
        hashCode.Add( Line2 );
        hashCode.Add( City );
        hashCode.Add( State );
        hashCode.Add( Country );
        hashCode.Add( PostalCode );
        hashCode.Add( Website );
        hashCode.Add( Email );
        hashCode.Add( PhoneNumber );
        hashCode.Add( Ext );
        hashCode.Add( Title );
        hashCode.Add( Department );
        hashCode.Add( Company );
        return hashCode.ToHashCode();
    }


    public async IAsyncEnumerable<RoleRecord> GetRoles( DbConnection connection, DbTransaction? transaction, Database db, [EnumeratorCancellation] CancellationToken token = default )
    {
        await foreach (UserRoleRecord record in db.UserRoles.Where( connection, transaction, true, UserRoleRecord.GetDynamicParameters( this ), token ))
        {
            RoleRecord? role = await db.Roles.Get( connection, transaction, record.RoleID, token );
            if (role is not null) { yield return role; }
        }
    }
    public async ValueTask<List<Claim>> GetUserClaims( DbConnection connection, DbTransaction? transaction, Database db, CancellationToken token )
    {
        List<Claim> claims = GetUserClaims();
        await foreach (RoleRecord role in GetRoles( connection, transaction, db, token )) { claims.Add( new Claim( ClaimTypes.Role, role.Name ) ); }

        return claims;
    }
    public IList<UserLoginInfo> GetUserLoginInfo() =>
        new List<UserLoginInfo>
        {
            new(LoginProvider, ProviderKey, ProviderDisplayName)
        };


    public bool HasPassword() => !string.IsNullOrWhiteSpace( PasswordHash );


    public bool HasRight<TRights>( TRights right ) where TRights : struct, Enum => Rights.HasValue && (Rights.Value & right.AsLong()) > 0;
    public async ValueTask<bool> HasRole( DbConnection connection, DbTransaction transaction, Database db, RoleRecord role, CancellationToken token )
    {
        UserRoleRecord? record = await db.UserRoles.Get( connection, transaction, true, UserRoleRecord.GetDynamicParameters( this, role ), token );
        return record is not null;
    }
    public UserRecord Lock( in TimeSpan offset )
    {
        IsDisabled = true;
        LockDate   = DateTimeOffset.UtcNow;
        LockoutEnd = LockDate + offset;
        return this;
    }


    public bool Owns<TRecord>( TRecord record ) where TRecord : TableRecord<TRecord> => record.CreatedBy == ID;
    public bool RedeemCode( string code )
    {
        string[] codes = Codes();
        return codes.Contains( code ) && ReplaceCode( code );
    }
    public void RemoveRight<TRights>( TRights right ) where TRights : struct, Enum => Rights = (Rights ?? 0) & right.AsLong();
    public async ValueTask RemoveRole( DbConnection connection, DbTransaction transaction, Database db, RoleRecord role, CancellationToken token )
    {
        UserRoleRecord? record = await db.UserRoles.Get( connection, transaction, true, UserRoleRecord.GetDynamicParameters( this, role ), token );

        if (record is not null) { await db.UserRoles.Delete( connection, transaction, record, token ); }
    }
    public UserRecord RemoveUserLoginInfo()
    {
        LoginProvider       = default;
        ProviderKey         = default;
        ProviderDisplayName = default;
        return this;
    }
    public bool ReplaceCode( string code )
    {
        var updatedCodes = new List<string>( Codes()
                                                .Where( s => s != code ) );

        string.Join( RECOVERY_CODE_SEPARATOR, updatedCodes );
        return true;
    }
    public bool ReplaceCode( params string[] codes ) => ReplaceCode( codes.AsEnumerable() );
    public bool ReplaceCode( IEnumerable<string> codes )
    {
        var updatedCodes = new List<string>( Codes()
                                                .Where( codes.Contains ) );

        string.Join( RECOVERY_CODE_SEPARATOR, updatedCodes );
        return true;
    }


    public UserRecord UpdatePassword( string password )
    {
        PasswordHash = _hasher.HashPassword( this, password );
        return this;
    }
    public PasswordVerificationResult VerifyPassword( string password ) => _hasher.VerifyHashedPassword( this, PasswordHash, password );


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


    public async ValueTask<UserRecord?> GetBoss( DbConnection connection, DbTransaction? transaction, DbTableBase<UserRecord> table, CancellationToken token ) => EscalateTo.HasValue
                                                                                                                                                                      ? await table.Get( connection, transaction, EscalateTo.Value, token )
                                                                                                                                                                      : default;


    public UserRecord Update( IUserData value )
    {
        IUserData data = this;
        data.Update( value );
        return this;
    }
    void IUserData.Update( IUserData value )
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
    }


    public override int CompareTo( UserRecord? other )
    {
        if (other is null) { return 1; }

        if (ReferenceEquals( this, other )) { return 0; }


        int userNameComparison = string.Compare( UserName, other.UserName, StringComparison.Ordinal );
        if (userNameComparison != 0) { return userNameComparison; }

        int firstNameComparison = string.Compare( FirstName, other.FirstName, StringComparison.Ordinal );
        if (firstNameComparison != 0) { return firstNameComparison; }

        int lastNameComparison = string.Compare( LastName, other.LastName, StringComparison.Ordinal );
        if (lastNameComparison != 0) { return lastNameComparison; }

        int fullNameComparison = string.Compare( FullName, other.FullName, StringComparison.Ordinal );
        if (fullNameComparison != 0) { return fullNameComparison; }

        int descriptionComparison = string.Compare( Description, other.Description, StringComparison.Ordinal );
        if (descriptionComparison != 0) { return descriptionComparison; }

        int sessionIDComparison = Nullable.Compare( SessionID, other.SessionID );
        if (sessionIDComparison != 0) { return sessionIDComparison; }

        int addressComparison = string.Compare( Address, other.Address, StringComparison.Ordinal );
        if (addressComparison != 0) { return addressComparison; }

        int line1Comparison = string.Compare( Line1, other.Line1, StringComparison.Ordinal );
        if (line1Comparison != 0) { return line1Comparison; }

        int line2Comparison = string.Compare( Line2, other.Line2, StringComparison.Ordinal );
        if (line2Comparison != 0) { return line2Comparison; }

        int cityComparison = string.Compare( City, other.City, StringComparison.Ordinal );
        if (cityComparison != 0) { return cityComparison; }

        int stateComparison = string.Compare( State, other.State, StringComparison.Ordinal );
        if (stateComparison != 0) { return stateComparison; }

        int countryComparison = string.Compare( Country, other.Country, StringComparison.Ordinal );
        if (countryComparison != 0) { return countryComparison; }

        int postalCodeComparison = string.Compare( PostalCode, other.PostalCode, StringComparison.Ordinal );
        if (postalCodeComparison != 0) { return postalCodeComparison; }

        int websiteComparison = string.Compare( Website, other.Website, StringComparison.Ordinal );
        if (websiteComparison != 0) { return websiteComparison; }

        int emailComparison = string.Compare( Email, other.Email, StringComparison.Ordinal );
        if (emailComparison != 0) { return emailComparison; }

        int phoneNumberComparison = string.Compare( PhoneNumber, other.PhoneNumber, StringComparison.Ordinal );
        if (phoneNumberComparison != 0) { return phoneNumberComparison; }

        int extComparison = string.Compare( Ext, other.Ext, StringComparison.Ordinal );
        if (extComparison != 0) { return extComparison; }

        int titleComparison = string.Compare( Title, other.Title, StringComparison.Ordinal );
        if (titleComparison != 0) { return titleComparison; }

        int departmentComparison = string.Compare( Department, other.Department, StringComparison.Ordinal );
        if (departmentComparison != 0) { return departmentComparison; }

        return string.Compare( Company, other.Company, StringComparison.Ordinal );
    }
    public override bool Equals( UserRecord? other )
    {
        if (other is null) { return false; }

        if (ReferenceEquals( this, other )) { return true; }

        return base.Equals( other ) && UserName == other.UserName && FirstName == other.FirstName && LastName == other.LastName && FullName == other.FullName && Description == other.Description && Address == other.Address && Line1 == other.Line1 &&
               Line2 == other.Line2 && City == other.City && State == other.State && Country == other.Country && PostalCode == other.PostalCode && Website == other.Website && Email == other.Email && PhoneNumber == other.PhoneNumber &&
               Ext == other.Ext && Title == other.Title && Department == other.Department && Company == other.Company;
    }
}
