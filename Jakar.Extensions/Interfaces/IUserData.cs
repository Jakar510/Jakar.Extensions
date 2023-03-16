// Jakar.Extensions :: Jakar.Extensions
// 11/15/2022  6:02 PM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "UnusedMemberInSuper.Global" )]
public interface IUserData : JsonModels.IJsonModel
{
    [MaxLength( 4096 )]                public string            Address           { get; set; }
    [MaxLength( 256 )]                 public string            City              { get; set; }
    [MaxLength( 256 )]                 public string            Company           { get; set; }
    [MaxLength( 256 )]                 public string            Country           { get; set; }
    [MaxLength( 256 )]                 public string            Department        { get; set; }
    [MaxLength( 256 )]                 public string            Description       { get; set; }
    [MaxLength( 1024 )] [EmailAddress] public string            Email             { get; set; }
    [MaxLength( 256 )]                 public string            Ext               { get; set; }
    [MaxLength( 256 )] [Required]      public string            FirstName         { get; set; }
    [MaxLength( 512 )]                 public string            FullName          { get; set; }
    [MaxLength( 256 )] [Required]      public string            LastName          { get; set; }
    [MaxLength( 512 )]                 public string            Line1             { get; set; }
    [MaxLength( 256 )]                 public string            Line2             { get; set; }
    [MaxLength( 256 )] [Phone]         public string            PhoneNumber       { get; set; }
    [MaxLength( 256 )] [Required]      public string            PostalCode        { get; set; }
    [Required]                         public SupportedLanguage PreferredLanguage { get; set; }
    [MaxLength( 256 )]                 public string            StateOrProvince   { get; set; }
    [MaxLength( 256 )]                 public string            Title             { get; set; }
    [MaxLength( 4096 )] [Url]          public string            Website           { get; set; }
}



public sealed class UserData : ObservableClass, IUserData, IEquatable<UserData>, IComparable<UserData>, IComparable
{
    private IDictionary<string, JToken?>? _additionalData;
    private string                        _address           = string.Empty;
    private string                        _city              = string.Empty;
    private string                        _company           = string.Empty;
    private string                        _country           = string.Empty;
    private string                        _department        = string.Empty;
    private string                        _description       = string.Empty;
    private string                        _email             = string.Empty;
    private string                        _ext               = string.Empty;
    private string                        _firstName         = string.Empty;
    private string                        _fullName          = string.Empty;
    private string                        _lastName          = string.Empty;
    private string                        _line1             = string.Empty;
    private string                        _line2             = string.Empty;
    private string                        _phoneNumber       = string.Empty;
    private string                        _postalCode        = string.Empty;
    private string                        _state             = string.Empty;
    private string                        _title             = string.Empty;
    private string                        _website           = string.Empty;
    private SupportedLanguage             _preferredLanguage = SupportedLanguage.English;


    [JsonExtensionData]
    public IDictionary<string, JToken?>? AdditionalData
    {
        get => _additionalData;
        set => SetProperty( ref _additionalData, value );
    }


    [MaxLength( 4096 )]
    public string Address
    {
        get => _address;
        set => SetProperty( ref _address, value );
    }

    [MaxLength( 256 )]
    public string City
    {
        get => _city;
        set => SetProperty( ref _city, value );
    }

    [MaxLength( 256 )]
    public string Company
    {
        get => _company;
        set => SetProperty( ref _company, value );
    }

    [MaxLength( 256 )]
    public string Country
    {
        get => _country;
        set => SetProperty( ref _country, value );
    }

    [MaxLength( 256 )]
    public string Department
    {
        get => _department;
        set => SetProperty( ref _department, value );
    }

    [MaxLength( 256 )]
    public string Description
    {
        get => _description;
        set => SetProperty( ref _description, value );
    }

    [EmailAddress]
    [MaxLength( 1024 )]
    public string Email
    {
        get => _email;
        set => SetProperty( ref _email, value );
    }

    [MaxLength( 256 )]
    public string Ext
    {
        get => _ext;
        set => SetProperty( ref _ext, value );
    }

    [Required]
    [MaxLength( 256 )]
    public string FirstName
    {
        get => _firstName;
        set
        {
            if ( SetProperty( ref _firstName, value ) ) { FullName = $"{value} {LastName}"; }
        }
    }

    [MaxLength( 512 )]
    public string FullName
    {
        get => _fullName;
        set => SetProperty( ref _fullName, value );
    }

    [Required]
    [MaxLength( 256 )]
    public string LastName
    {
        get => _lastName;
        set
        {
            if ( SetProperty( ref _lastName, value ) ) { FullName = $"{FirstName} {value}"; }
        }
    }

    [MaxLength( 512 )]
    public string Line1
    {
        get => _line1;
        set => SetProperty( ref _line1, value );
    }

    [MaxLength( 256 )]
    public string Line2
    {
        get => _line2;
        set => SetProperty( ref _line2, value );
    }

    [Phone]
    [MaxLength( 256 )]
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty( ref _phoneNumber, value );
    }

    [MaxLength( 256 )]
    public string PostalCode
    {
        get => _postalCode;
        set => SetProperty( ref _postalCode, value );
    }

    [Required]
    public SupportedLanguage PreferredLanguage
    {
        get => _preferredLanguage;
        set => SetProperty( ref _preferredLanguage, value );
    }

    [MaxLength( 256 )]
    public string StateOrProvince
    {
        get => _state;
        set => SetProperty( ref _state, value );
    }

    [MaxLength( 256 )]
    public string Title
    {
        get => _title;
        set => SetProperty( ref _title, value );
    }

    [Url]
    [MaxLength( 4096 )]
    public string Website
    {
        get => _website;
        set => SetProperty( ref _website, value );
    }


    public UserData() { }
    public UserData( IUserData value )
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

        if ( value.AdditionalData is null ) { return; }

        AdditionalData ??= new Dictionary<string, JToken?>();
        foreach ( (string key, JToken? jToken) in value.AdditionalData ) { AdditionalData[key] = jToken; }
    }
    public UserData( string firstName, string lastName )
    {
        _firstName = firstName;
        _lastName  = lastName;
    }


    public bool Equals( UserData? other )
    {
        if ( ReferenceEquals( null, other ) ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return Equals( _additionalData, other._additionalData ) && _address == other._address && _city == other._city && _company == other._company && _country == other._country && _department == other._department &&
               _description == other._description && _email == other._email && _ext == other._ext && _firstName == other._firstName && _fullName == other._fullName && _lastName == other._lastName && _line1 == other._line1 && _line2 == other._line2 &&
               _phoneNumber == other._phoneNumber && _postalCode == other._postalCode && _state == other._state && _title == other._title && _website == other._website && _preferredLanguage == other._preferredLanguage;
    }
    public override bool Equals( object? obj )
    {
        if ( ReferenceEquals( null, obj ) ) { return false; }

        if ( ReferenceEquals( this, obj ) ) { return true; }

        if ( obj.GetType() != this.GetType() ) { return false; }

        return Equals( (UserData)obj );
    }
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add( _additionalData );
        hashCode.Add( _address );
        hashCode.Add( _city );
        hashCode.Add( _company );
        hashCode.Add( _country );
        hashCode.Add( _department );
        hashCode.Add( _description );
        hashCode.Add( _email );
        hashCode.Add( _ext );
        hashCode.Add( _firstName );
        hashCode.Add( _fullName );
        hashCode.Add( _lastName );
        hashCode.Add( _line1 );
        hashCode.Add( _line2 );
        hashCode.Add( _phoneNumber );
        hashCode.Add( _postalCode );
        hashCode.Add( _state );
        hashCode.Add( _title );
        hashCode.Add( _website );
        hashCode.Add( (int)_preferredLanguage );
        return hashCode.ToHashCode();
    }
    public int CompareTo( UserData? other )
    {
        if ( ReferenceEquals( this, other ) ) { return 0; }

        if ( ReferenceEquals( null, other ) ) { return 1; }

        int addressComparison = string.Compare( _address, other._address, StringComparison.Ordinal );
        if ( addressComparison != 0 ) { return addressComparison; }

        int cityComparison = string.Compare( _city, other._city, StringComparison.Ordinal );
        if ( cityComparison != 0 ) { return cityComparison; }

        int companyComparison = string.Compare( _company, other._company, StringComparison.Ordinal );
        if ( companyComparison != 0 ) { return companyComparison; }

        int countryComparison = string.Compare( _country, other._country, StringComparison.Ordinal );
        if ( countryComparison != 0 ) { return countryComparison; }

        int departmentComparison = string.Compare( _department, other._department, StringComparison.Ordinal );
        if ( departmentComparison != 0 ) { return departmentComparison; }

        int descriptionComparison = string.Compare( _description, other._description, StringComparison.Ordinal );
        if ( descriptionComparison != 0 ) { return descriptionComparison; }

        int emailComparison = string.Compare( _email, other._email, StringComparison.Ordinal );
        if ( emailComparison != 0 ) { return emailComparison; }

        int extComparison = string.Compare( _ext, other._ext, StringComparison.Ordinal );
        if ( extComparison != 0 ) { return extComparison; }

        int firstNameComparison = string.Compare( _firstName, other._firstName, StringComparison.Ordinal );
        if ( firstNameComparison != 0 ) { return firstNameComparison; }

        int fullNameComparison = string.Compare( _fullName, other._fullName, StringComparison.Ordinal );
        if ( fullNameComparison != 0 ) { return fullNameComparison; }

        int lastNameComparison = string.Compare( _lastName, other._lastName, StringComparison.Ordinal );
        if ( lastNameComparison != 0 ) { return lastNameComparison; }

        int line1Comparison = string.Compare( _line1, other._line1, StringComparison.Ordinal );
        if ( line1Comparison != 0 ) { return line1Comparison; }

        int line2Comparison = string.Compare( _line2, other._line2, StringComparison.Ordinal );
        if ( line2Comparison != 0 ) { return line2Comparison; }

        int phoneNumberComparison = string.Compare( _phoneNumber, other._phoneNumber, StringComparison.Ordinal );
        if ( phoneNumberComparison != 0 ) { return phoneNumberComparison; }

        int postalCodeComparison = string.Compare( _postalCode, other._postalCode, StringComparison.Ordinal );
        if ( postalCodeComparison != 0 ) { return postalCodeComparison; }

        int stateComparison = string.Compare( _state, other._state, StringComparison.Ordinal );
        if ( stateComparison != 0 ) { return stateComparison; }

        int titleComparison = string.Compare( _title, other._title, StringComparison.Ordinal );
        if ( titleComparison != 0 ) { return titleComparison; }

        int websiteComparison = string.Compare( _website, other._website, StringComparison.Ordinal );
        if ( websiteComparison != 0 ) { return websiteComparison; }

        return _preferredLanguage.CompareTo( other._preferredLanguage );
    }
    public int CompareTo( object? obj )
    {
        if ( ReferenceEquals( null, obj ) ) { return 1; }

        if ( ReferenceEquals( this, obj ) ) { return 0; }

        return obj is UserData other
                   ? CompareTo( other )
                   : throw new ArgumentException( $"Object must be of type {nameof(UserData)}" );
    }


    public static bool operator ==( UserData? left, UserData? right ) => Equalizer<UserData>.Default.Equals( left, right );
    public static bool operator !=( UserData? left, UserData? right ) => !Equalizer<UserData>.Default.Equals( left, right );
    public static bool operator <( UserData?  left, UserData? right ) => Sorter<UserData>.Default.Compare( left, right ) < 0;
    public static bool operator >( UserData?  left, UserData? right ) => Sorter<UserData>.Default.Compare( left, right ) > 0;
    public static bool operator <=( UserData? left, UserData? right ) => Sorter<UserData>.Default.Compare( left, right ) <= 0;
    public static bool operator >=( UserData? left, UserData? right ) => Sorter<UserData>.Default.Compare( left, right ) >= 0;
}
