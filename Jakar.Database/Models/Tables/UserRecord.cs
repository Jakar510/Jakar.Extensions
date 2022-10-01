// ToothFairyDispatch :: ToothFairyDispatch.Cloud
// 08/29/2022  9:55 PM

using System.Security.Claims;
using Newtonsoft.Json.Linq;



namespace Jakar.Database;


public sealed record UserRecord : TableRecord<UserRecord>, IUserRecord<UserRecord>
{
    private static readonly PasswordHasher<UserRecord> _hasher = new();


    private string            _userName  = string.Empty;
    private string            _firstName = string.Empty;
    private string            _lastName  = string.Empty;
    private string?           _fullName;
    private string?           _description;
    private string?           _address;
    private string?           _line1;
    private string?           _line2;
    private string?           _city;
    private string?           _state;
    private string?           _country;
    private string?           _postalCode;
    private string?           _website;
    private string?           _email;
    private string?           _phoneNumber;
    private string?           _ext;
    private string?           _title;
    private string?           _department;
    private string?           _company;
    private SupportedLanguage _preferredLanguage;
    private Guid?             _sessionID;
    private DateTimeOffset?   _subscriptionExpires;
    private long?             _subscriptionID;
    private long?             _escalateTo;
    private bool              _isActive;
    private bool              _isDisabled;
    private bool              _isLocked;
    private int?              _badLogins;
    private DateTimeOffset?   _lastActive;
    private DateTimeOffset?   _lastBadAttempt;
    private DateTimeOffset?   _lockDate;
    private string?           _refreshToken;
    private DateTimeOffset?   _refreshTokenExpiryTime;
    private DateTimeOffset?   _tokenExpiration;
    private bool              _isEmailConfirmed;
    private bool              _isPhoneNumberConfirmed;
    private bool              _isTwoFactorEnabled;
    private string?           _loginProvider;
    private string?           _providerKey;
    private string?           _providerDisplayName;
    private string?           _securityStamp;
    private string?           _concurrencyStamp;
    private string?           _additionalData;
    private string?           _passwordHash;
    private long?             _rights;


    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }

    public string? FullName
    {
        get => _fullName;
        set => SetProperty(ref _fullName, value);
    }

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public Guid? SessionID
    {
        get => _sessionID;
        set => SetProperty(ref _sessionID, value);
    }

    public string? Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }
    
    public string? Line1
    {
        get => _line1;
        set => SetProperty(ref _line1, value);
    }
    
    public string? Line2
    {
        get => _line2;
        set => SetProperty(ref _line2, value);
    }
    
    public string? City
    {
        get => _city;
        set => SetProperty(ref _city, value);
    }
    
    public string? State
    {
        get => _state;
        set => SetProperty(ref _state, value);
    }
    
    public string? Country
    {
        get => _country;
        set => SetProperty(ref _country, value);
    }
    
    public string? PostalCode
    {
        get => _postalCode;
        set => SetProperty(ref _postalCode, value);
    }
    
    public string? Website
    {
        get => _website;
        set => SetProperty(ref _website, value);
    }
    
    public string? Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }
    
    public string? PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value);
    }
    
    public string? Ext
    {
        get => _ext;
        set => SetProperty(ref _ext, value);
    }
    
    public string? Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    
    public string? Department
    {
        get => _department;
        set => SetProperty(ref _department, value);
    }
    
    public string? Company
    {
        get => _company;
        set => SetProperty(ref _company, value);
    }
    
    public SupportedLanguage PreferredLanguage
    {
        get => _preferredLanguage;
        set => SetProperty(ref _preferredLanguage, value);
    }
    
    public DateTimeOffset? SubscriptionExpires
    {
        get => _subscriptionExpires;
        set => SetProperty(ref _subscriptionExpires, value);
    }
    
    public long? SubscriptionID
    {
        get => _subscriptionID;
        set => SetProperty(ref _subscriptionID, value);
    }
    
    public long? EscalateTo
    {
        get => _escalateTo;
        set => SetProperty(ref _escalateTo, value);
    }
    
    public bool IsActive
    {
        get => _isActive;
        set => SetProperty(ref _isActive, value);
    }

    public bool IsDisabled
    {
        get => _isDisabled;
        set => SetProperty(ref _isDisabled, value);
    }

    public bool IsLocked
    {
        get => _isLocked;
        set => SetProperty(ref _isLocked, value);
    }

    public int? BadLogins
    {
        get => _badLogins;
        set => SetProperty(ref _badLogins, value);
    }

    public DateTimeOffset? LastActive
    {
        get => _lastActive;
        set => SetProperty(ref _lastActive, value);
    }

    public DateTimeOffset? LastBadAttempt
    {
        get => _lastBadAttempt;
        set => SetProperty(ref _lastBadAttempt, value);
    }

    public DateTimeOffset? LockDate
    {
        get => _lockDate;
        set => SetProperty(ref _lockDate, value);
    }

    public string? RefreshToken
    {
        get => _refreshToken;
        set => SetProperty(ref _refreshToken, value);
    }

    public DateTimeOffset? RefreshTokenExpiryTime
    {
        get => _refreshTokenExpiryTime;
        set => SetProperty(ref _refreshTokenExpiryTime, value);
    }

    public DateTimeOffset? TokenExpiration
    {
        get => _tokenExpiration;
        set => SetProperty(ref _tokenExpiration, value);
    }

    public bool IsEmailConfirmed
    {
        get => _isEmailConfirmed;
        set => SetProperty(ref _isEmailConfirmed, value);
    }

    public bool IsPhoneNumberConfirmed
    {
        get => _isPhoneNumberConfirmed;
        set => SetProperty(ref _isPhoneNumberConfirmed, value);
    }

    public bool IsTwoFactorEnabled
    {
        get => _isTwoFactorEnabled;
        set => SetProperty(ref _isTwoFactorEnabled, value);
    }

    public string? LoginProvider
    {
        get => _loginProvider;
        set => SetProperty(ref _loginProvider, value);
    }

    public string? ProviderKey
    {
        get => _providerKey;
        set => SetProperty(ref _providerKey, value);
    }

    public string? ProviderDisplayName
    {
        get => _providerDisplayName;
        set => SetProperty(ref _providerDisplayName, value);
    }

    public string? SecurityStamp
    {
        get => _securityStamp;
        set => SetProperty(ref _securityStamp, value);
    }

    public string? ConcurrencyStamp
    {
        get => _concurrencyStamp;
        set => SetProperty(ref _concurrencyStamp, value);
    }

    public string? AdditionalData
    {
        get => _additionalData;
        set => SetProperty(ref _additionalData, value);
    }

    public string? PasswordHash
    {
        get => _passwordHash;
        set => SetProperty(ref _passwordHash, value);
    }

    public long? Rights
    {
        get => _rights;
        set => SetProperty(ref _rights, value);
    }


    IDictionary<string, JToken?>? JsonModels.IJsonModel.AdditionalData
    {
        get => AdditionalData?.FromJson<Dictionary<string, JToken?>>();
        set => AdditionalData = value?.ToJson();
    }
    long? IUserRecord<UserRecord>.CreatedBy => CreatedBy;


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
    public UserRecord( IUserData value, UserRecord caller, long? rights = default ) : base(caller)
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
    public static UserRecord Create<TRights>( IUserData value, TRights    rights ) where TRights : struct, Enum => new(value, rights.AsLong());
    public static UserRecord Create<TRights>( IUserData value, UserRecord caller, TRights rights ) where TRights : struct, Enum => new(value, caller, rights.AsLong());


    public bool HasRight<TRights>( TRights    right ) where TRights : struct, Enum => Rights.HasValue && ( Rights.Value & right.AsLong() ) > 0;
    public void AddRight<TRights>( TRights    right ) where TRights : struct, Enum => Rights = ( Rights ?? 0 ) | right.AsLong();
    public void RemoveRight<TRights>( TRights right ) where TRights : struct, Enum => Rights = ( Rights ?? 0 ) & right.AsLong();


    public bool Owns<TRecord>( TRecord       record ) where TRecord : TableRecord<TRecord> => record.CreatedBy == ID;
    public bool DoesNotOwn<TRecord>( TRecord record ) where TRecord : TableRecord<TRecord> => record.CreatedBy != ID;


    public void UpdatePassword( string password )
    {
        VerifyAccess();
        PasswordHash = _hasher.HashPassword(this, password);
    }
    public PasswordVerificationResult VerifyPassword( string password ) => _hasher.VerifyHashedPassword(this, PasswordHash, password);


    public UserRecord MarkBadLogin()
    {
        VerifyAccess();
        LastBadAttempt = DateTimeOffset.Now;
        BadLogins      = 0;
        IsDisabled     = BadLogins > 5;

        return IsDisabled
                   ? Lock()
                   : Unlock();
    }
    public UserRecord SetActive()
    {
        VerifyAccess();
        LastActive = DateTimeOffset.Now;
        return this;
    }
    public UserRecord Disable()
    {
        VerifyAccess();
        IsDisabled = true;
        return Lock();
    }
    public UserRecord Lock()
    {
        VerifyAccess();
        IsDisabled = true;
        LockDate   = DateTimeOffset.Now;
        return this;
    }
    public UserRecord Enable()
    {
        VerifyAccess();
        LockDate = default;
        IsActive = true;
        return Unlock();
    }
    public UserRecord Unlock()
    {
        VerifyAccess();
        BadLogins      = 0;
        IsDisabled     = BadLogins > 5;
        LastBadAttempt = default;
        LockDate       = default;
        return this;
    }


    public void ClearRefreshToken()
    {
        VerifyAccess();
        RefreshToken           = default;
        RefreshTokenExpiryTime = default;
    }
    public void SetRefreshToken( string token, DateTimeOffset date )
    {
        VerifyAccess();
        RefreshToken           = token;
        RefreshTokenExpiryTime = date;
    }


    public List<Claim> GetUserClaims() => new()
                                          {
                                              new Claim(ClaimTypes.Sid,             UserID.ToString()),
                                              new Claim(ClaimTypes.Dsa,             UserID.ToString()),
                                              new Claim(ClaimTypes.NameIdentifier,  UserName),
                                              new Claim(ClaimTypes.Name,            FullName ?? string.Empty),
                                              new Claim(ClaimTypes.Country,         Country ?? string.Empty),
                                              new Claim(ClaimTypes.MobilePhone,     PhoneNumber ?? string.Empty),
                                              new Claim(ClaimTypes.Email,           Email ?? string.Empty),
                                              new Claim(ClaimTypes.PostalCode,      PostalCode ?? string.Empty),
                                              new Claim(ClaimTypes.StateOrProvince, State ?? string.Empty),
                                              new Claim(ClaimTypes.StreetAddress,   Line1 ?? string.Empty),
                                              new Claim(ClaimTypes.Webpage,         Website ?? string.Empty),
                                              new Claim(ClaimTypes.Expiration,      SubscriptionExpires?.ToString() ?? string.Empty),
                                          };


    public async Task<UserRecord?> GetBoss( DbConnection connection, DbTransaction? transaction, DbTable<UserRecord> table, CancellationToken token ) => EscalateTo.HasValue
                                                                                                                                                             ? await table.Get(connection, transaction, EscalateTo.Value, token)
                                                                                                                                                             : default;


    public void Update( IUserData value )
    {
        VerifyAccess();
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

        int addressComparison = string.Compare(Address, other.Address, StringComparison.Ordinal);
        if ( addressComparison != 0 ) { return addressComparison; }

        int line1Comparison = string.Compare(Line1, other.Line1, StringComparison.Ordinal);
        if ( line1Comparison != 0 ) { return line1Comparison; }

        int line2Comparison = string.Compare(Line2, other.Line2, StringComparison.Ordinal);
        if ( line2Comparison != 0 ) { return line2Comparison; }

        int cityComparison = string.Compare(City, other.City, StringComparison.Ordinal);
        if ( cityComparison != 0 ) { return cityComparison; }

        int stateComparison = string.Compare(State, other.State, StringComparison.Ordinal);
        if ( stateComparison != 0 ) { return stateComparison; }

        int countryComparison = string.Compare(Country, other.Country, StringComparison.Ordinal);
        if ( countryComparison != 0 ) { return countryComparison; }

        int postalCodeComparison = string.Compare(PostalCode, other.PostalCode, StringComparison.Ordinal);
        if ( postalCodeComparison != 0 ) { return postalCodeComparison; }

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
    public override bool Equals( UserRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return base.Equals(other) && UserName == other.UserName && FirstName == other.FirstName && LastName == other.LastName && FullName == other.FullName && Description == other.Description && Address == other.Address && Line1 == other.Line1 &&
               Line2 == other.Line2 && City == other.City && State == other.State && Country == other.Country && PostalCode == other.PostalCode && Website == other.Website && Email == other.Email && PhoneNumber == other.PhoneNumber &&
               Ext == other.Ext && Title == other.Title && Department == other.Department && Company == other.Company;
    }


    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(UserName);
        hashCode.Add(FirstName);
        hashCode.Add(LastName);
        hashCode.Add(FullName);
        hashCode.Add(Description);
        hashCode.Add(Address);
        hashCode.Add(Line1);
        hashCode.Add(Line2);
        hashCode.Add(City);
        hashCode.Add(State);
        hashCode.Add(Country);
        hashCode.Add(PostalCode);
        hashCode.Add(Website);
        hashCode.Add(Email);
        hashCode.Add(PhoneNumber);
        hashCode.Add(Ext);
        hashCode.Add(Title);
        hashCode.Add(Department);
        hashCode.Add(Company);
        return hashCode.ToHashCode();
    }
}
