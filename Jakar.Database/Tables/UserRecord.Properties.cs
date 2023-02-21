// Jakar.Extensions :: Jakar.Database
// 01/30/2023  9:29 PM

namespace Jakar.Database;


public sealed partial record UserRecord
{
    private bool              _isActive;
    private bool              _isDisabled;
    private bool              _isEmailConfirmed;
    private bool              _isLocked;
    private bool              _isPhoneNumberConfirmed;
    private bool              _isTwoFactorEnabled;
    private DateTimeOffset?   _lastActive;
    private DateTimeOffset?   _lastBadAttempt;
    private DateTimeOffset?   _lockDate;
    private DateTimeOffset?   _lockEnd;
    private DateTimeOffset?   _refreshTokenExpiryTime;
    private DateTimeOffset?   _subscriptionExpires;
    private DateTimeOffset?   _tokenExpiration;
    private Guid?             _sessionID;
    private int               _badLogins;
    private string            _firstName    = string.Empty;
    private string            _lastName     = string.Empty;
    private string            _passwordHash = string.Empty;
    private string            _userName     = string.Empty;
    private string?           _additionalData;
    private string?           _address;
    private string?           _city;
    private string?           _company;
    private string?           _concurrencyStamp;
    private string?           _country;
    private string?           _department;
    private string?           _description;
    private string?           _email;
    private string?           _escalateTo;
    private string?           _ext;
    private string?           _fullName;
    private string?           _line1;
    private string?           _line2;
    private string?           _phoneNumber;
    private string?           _postalCode;
    private string?           _refreshToken;
    private string            _rights = string.Empty;
    private string?           _securityStamp;
    private string?           _state;
    private string?           _subscriptionID;
    private string?           _title;
    private string?           _website;
    private SupportedLanguage _preferredLanguage;


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
    public Guid UserID { get; init; }

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


    [ProtectedPersonalData]
    [MaxLength( 256 )]
    public string FirstName
    {
        get => _firstName;
        set => SetProperty( ref _firstName, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 256 )]
    public string LastName
    {
        get => _lastName;
        set => SetProperty( ref _lastName, value );
    }


    [MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes )]
    public string PasswordHash
    {
        get => _passwordHash;
        set => SetProperty( ref _passwordHash, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 256 )]
    public string UserName
    {
        get => _userName;
        set => SetProperty( ref _userName, value );
    }


    [ProtectedPersonalData]
    [MaxLength( int.MaxValue )]
    public string? AdditionalData
    {
        get => _additionalData;
        set => SetProperty( ref _additionalData, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 4096 )]
    public string? Address
    {
        get => _address;
        set => SetProperty( ref _address, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 256 )]
    public string? City
    {
        get => _city;
        set => SetProperty( ref _city, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 256 )]
    public string? Company
    {
        get => _company;
        set => SetProperty( ref _company, value );
    }


    [MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes )]
    public string? ConcurrencyStamp
    {
        get => _concurrencyStamp;
        set => SetProperty( ref _concurrencyStamp, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 256 )]
    public string? Country
    {
        get => _country;
        set => SetProperty( ref _country, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 256 )]
    public string? Department
    {
        get => _department;
        set => SetProperty( ref _department, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 256 )]
    public string? Description
    {
        get => _description;
        set => SetProperty( ref _description, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 1024 )]
    public string? Email
    {
        get => _email;
        set => SetProperty( ref _email, value );
    }
    [MaxLength( 256 )]
    public string? EscalateTo
    {
        get => _escalateTo;
        set => SetProperty( ref _escalateTo, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 256 )]
    public string? Ext
    {
        get => _ext;
        set => SetProperty( ref _ext, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 512 )]
    public string? FullName
    {
        get => _fullName;
        set => SetProperty( ref _fullName, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 512 )]
    public string? Line1
    {
        get => _line1;
        set => SetProperty( ref _line1, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 256 )]
    public string? Line2
    {
        get => _line2;
        set => SetProperty( ref _line2, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 256 )]
    public string? PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty( ref _phoneNumber, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 256 )]
    public string? PostalCode
    {
        get => _postalCode;
        set => SetProperty( ref _postalCode, value );
    }


    [MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes )]
    public string? RefreshToken
    {
        get => _refreshToken;
        set => SetProperty( ref _refreshToken, value );
    }


    [MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes )]
    public string Rights
    {
        get => _rights;
        set => SetProperty( ref _rights, value );
    }


    [MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes )]
    public string? SecurityStamp
    {
        get => _securityStamp;
        set => SetProperty( ref _securityStamp, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 256 )]
    public string? State
    {
        get => _state;
        set => SetProperty( ref _state, value );
    }
    [MaxLength( 256 )]
    public string? SubscriptionID
    {
        get => _subscriptionID;
        set => SetProperty( ref _subscriptionID, value );
    }


    [ProtectedPersonalData]
    [MaxLength( 256 )]
    public string? Title
    {
        get => _title;
        set => SetProperty( ref _title, value );
    }


    [ProtectedPersonalData]
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

        int stateComparison = string.Compare( State, other.State, StringComparison.Ordinal );
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
    public override bool Equals( UserRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return base.Equals( other ) && UserName == other.UserName && FirstName == other.FirstName && LastName == other.LastName && FullName == other.FullName && Description == other.Description && Address == other.Address && Line1 == other.Line1 &&
               Line2 == other.Line2 && City == other.City && State == other.State && Country == other.Country && PostalCode == other.PostalCode && Website == other.Website && Email == other.Email && PhoneNumber == other.PhoneNumber &&
               Ext == other.Ext && Title == other.Title && Department == other.Department && Company == other.Company;
    }
}
