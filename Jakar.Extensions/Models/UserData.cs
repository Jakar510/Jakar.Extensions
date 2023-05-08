﻿// Jakar.Extensions :: Jakar.Extensions
// 11/15/2022  6:02 PM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "UnusedMemberInSuper.Global" )]
public interface IUserData : JsonModels.IJsonModel, IEquatable<IUserData>, IComparable<IUserData>
{
    public static Equalizer<IUserData> Equalizer => Equalizer<IUserData>.Default;
    public static Sorter<IUserData>    Sorter    => Sorter<IUserData>.Default;


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


    public void Update( IUserData value );
}



[Serializable]
public class UserData : ObservableClass, IUserData
{
    public const string                        EMPTY_PHONE_NUMBER = "(000) 000-0000";
    private      IDictionary<string, JToken?>? _additionalData;
    private      string                        _city       = string.Empty;
    private      string                        _company    = string.Empty;
    private      string                        _country    = string.Empty;
    private      string                        _department = string.Empty;
    private      string?                       _description;
    private      string                        _email       = string.Empty;
    private      string                        _ext         = string.Empty;
    private      string                        _firstName   = string.Empty;
    private      string                        _lastName    = string.Empty;
    private      string                        _line1       = string.Empty;
    private      string                        _line2       = string.Empty;
    private      string                        _phoneNumber = string.Empty;
    private      string                        _postalCode  = string.Empty;
    private      string                        _state       = string.Empty;
    private      string                        _title       = string.Empty;
    private      string                        _website     = string.Empty;
    private      string?                       _address;
    private      string?                       _fullName;
    private      SupportedLanguage             _preferredLanguage = SupportedLanguage.English;


    [JsonExtensionData]
    public IDictionary<string, JToken?>? AdditionalData
    {
        get => _additionalData;
        set => SetProperty( ref _additionalData, value );
    }


    [MaxLength( 4096 )]
    public string Address
    {
        get => _address ??= $"{Line1} {Line2} {City}, {StateOrProvince} {Country} {PostalCode}";
        set => SetProperty( ref _address, value );
    }

    [MaxLength( 256 )]
    public string City
    {
        get => _city;
        set
        {
            if ( !SetProperty( ref _city, value ) ) { return; }

            _address = default;
            OnPropertyChanged( nameof(Address) );
        }
    }

    [MaxLength( 256 )]
    public string Company
    {
        get => _company;
        set
        {
            if ( !SetProperty( ref _company, value ) ) { return; }

            _description = default;
            OnPropertyChanged( nameof(Description) );
        }
    }

    [MaxLength( 256 )]
    public string Country
    {
        get => _country;
        set
        {
            if ( !SetProperty( ref _country, value ) ) { return; }

            _address = default;
            OnPropertyChanged( nameof(Address) );
        }
    }

    [MaxLength( 256 )]
    public string Department
    {
        get => _department;
        set
        {
            if ( !SetProperty( ref _department, value ) ) { return; }

            _description = default;
            OnPropertyChanged( nameof(Description) );
        }
    }

    [MaxLength( 256 )]
    public string Description
    {
        get => _description ??= $"{Department}, {Title} at {Company}";
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
            if ( !SetProperty( ref _firstName, value ) ) { return; }

            _fullName = default;
            OnPropertyChanged( nameof(FullName) );
        }
    }

    [MaxLength( 512 )]
    public string FullName
    {
        get => _fullName ??= $"{FirstName} {LastName}";
        set => SetProperty( ref _fullName, value );
    }

    [JsonIgnore] public bool IsValidWebsite     => Uri.TryCreate( Website, UriKind.RelativeOrAbsolute, out _ );
    [JsonIgnore] public bool IsValidName        => !string.IsNullOrEmpty( FullName );
    [JsonIgnore] public bool IsValidEmail       => !string.IsNullOrEmpty( Email );
    [JsonIgnore] public bool IsValidPhoneNumber => !string.IsNullOrEmpty( PhoneNumber );

    [JsonIgnore]
    public bool IsValidAddress
    {
        get
        {
            Span<char> span = stackalloc char[Address.Length];

            Address.AsSpan()
                   .CopyTo( span );

            for ( int i = 0; i < span.Length; i++ )
            {
                if ( char.IsLetterOrDigit( span[i] ) ) { continue; }

                span[i] = ' ';
            }

            return span.IsNullOrWhiteSpace();
        }
    }

    [Required]
    [MaxLength( 256 )]
    public string LastName
    {
        get => _lastName;
        set
        {
            if ( !SetProperty( ref _lastName, value ) ) { return; }

            _fullName = default;
            OnPropertyChanged( nameof(FullName) );
        }
    }

    [MaxLength( 512 )]
    public string Line1
    {
        get => _line1;
        set
        {
            if ( !SetProperty( ref _line1, value ) ) { return; }

            _address = default;
            OnPropertyChanged( nameof(Address) );
        }
    }

    [MaxLength( 256 )]
    public string Line2
    {
        get => _line2;
        set
        {
            if ( !SetProperty( ref _line2, value ) ) { return; }

            _address = default;
            OnPropertyChanged( nameof(Address) );
        }
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
        set
        {
            if ( !SetProperty( ref _postalCode, value ) ) { return; }

            _address = default;
            OnPropertyChanged( nameof(Address) );
        }
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
        set
        {
            if ( !SetProperty( ref _state, value ) ) { return; }

            _address = default;
            OnPropertyChanged( nameof(Address) );
        }
    }

    [MaxLength( 256 )]
    public string Title
    {
        get => _title;
        set
        {
            if ( !SetProperty( ref _title, value ) ) { return; }

            _description = default;
            OnPropertyChanged( nameof(Description) );
        }
    }

    [Url]
    [MaxLength( 4096 )]
    public string Website
    {
        get => _website;
        set => SetProperty( ref _website, value );
    }


    public UserData() { }
    public UserData( IUserData value ) => Update( value );
    public UserData( string firstName, string lastName )
    {
        _firstName = firstName;
        _lastName  = lastName;
    }


    public void Update( IUserData value )
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


    public bool Equals( IUserData? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return Equals( _additionalData, other.AdditionalData ) &&
               string.Equals( _address,     other.Address,         StringComparison.Ordinal ) &&
               string.Equals( _city,        other.City,            StringComparison.Ordinal ) &&
               string.Equals( _company,     other.Company,         StringComparison.Ordinal ) &&
               string.Equals( _country,     other.Country,         StringComparison.Ordinal ) &&
               string.Equals( _department,  other.Department,      StringComparison.Ordinal ) &&
               string.Equals( _description, other.Description,     StringComparison.Ordinal ) &&
               string.Equals( _email,       other.Email,           StringComparison.Ordinal ) &&
               string.Equals( _ext,         other.Ext,             StringComparison.Ordinal ) &&
               string.Equals( _firstName,   other.FirstName,       StringComparison.Ordinal ) &&
               string.Equals( _fullName,    other.FullName,        StringComparison.Ordinal ) &&
               string.Equals( _lastName,    other.LastName,        StringComparison.Ordinal ) &&
               string.Equals( _line1,       other.Line1,           StringComparison.Ordinal ) &&
               string.Equals( _line2,       other.Line2,           StringComparison.Ordinal ) &&
               string.Equals( _phoneNumber, other.PhoneNumber,     StringComparison.Ordinal ) &&
               string.Equals( _postalCode,  other.PostalCode,      StringComparison.Ordinal ) &&
               string.Equals( _state,       other.StateOrProvince, StringComparison.Ordinal ) &&
               string.Equals( _title,       other.Title,           StringComparison.Ordinal ) &&
               string.Equals( _website,     other.Website,         StringComparison.Ordinal ) &&
               _preferredLanguage == other.PreferredLanguage;
    }
    public override bool Equals( object? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return other is IUserData data && Equals( data );
    }
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add( AdditionalData );
        hashCode.Add( Address );
        hashCode.Add( City );
        hashCode.Add( Company );
        hashCode.Add( Country );
        hashCode.Add( Department );
        hashCode.Add( Description );
        hashCode.Add( Email );
        hashCode.Add( Ext );
        hashCode.Add( FirstName );
        hashCode.Add( FullName );
        hashCode.Add( LastName );
        hashCode.Add( Line1 );
        hashCode.Add( Line2 );
        hashCode.Add( PhoneNumber );
        hashCode.Add( PostalCode );
        hashCode.Add( StateOrProvince );
        hashCode.Add( Title );
        hashCode.Add( Website );
        hashCode.Add( PreferredLanguage );
        return hashCode.ToHashCode();
    }
    public int CompareTo( IUserData? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int firstNameComparison = string.Compare( _firstName, other.FirstName, StringComparison.Ordinal );
        if ( firstNameComparison != 0 ) { return firstNameComparison; }

        int lastNameComparison = string.Compare( _lastName, other.LastName, StringComparison.Ordinal );
        if ( lastNameComparison != 0 ) { return lastNameComparison; }

        int fullNameComparison = string.Compare( _fullName, other.FullName, StringComparison.Ordinal );
        if ( fullNameComparison != 0 ) { return fullNameComparison; }

        int descriptionComparison = string.Compare( _description, other.Description, StringComparison.Ordinal );
        if ( descriptionComparison != 0 ) { return descriptionComparison; }

        int companyComparison = string.Compare( _company, other.Company, StringComparison.Ordinal );
        if ( companyComparison != 0 ) { return companyComparison; }

        int departmentComparison = string.Compare( _department, other.Department, StringComparison.Ordinal );
        if ( departmentComparison != 0 ) { return departmentComparison; }

        int titleComparison = string.Compare( _title, other.Title, StringComparison.Ordinal );
        if ( titleComparison != 0 ) { return titleComparison; }

        int emailComparison = string.Compare( _email, other.Email, StringComparison.Ordinal );
        if ( emailComparison != 0 ) { return emailComparison; }

        int phoneNumberComparison = string.Compare( _phoneNumber, other.PhoneNumber, StringComparison.Ordinal );
        if ( phoneNumberComparison != 0 ) { return phoneNumberComparison; }

        int extComparison = string.Compare( _ext, other.Ext, StringComparison.Ordinal );
        if ( extComparison != 0 ) { return extComparison; }

        int websiteComparison = string.Compare( _website, other.Website, StringComparison.Ordinal );
        if ( websiteComparison != 0 ) { return websiteComparison; }

        int cityComparison = string.Compare( _city, other.City, StringComparison.Ordinal );
        if ( cityComparison != 0 ) { return cityComparison; }

        int countryComparison = string.Compare( _country, other.Country, StringComparison.Ordinal );
        if ( countryComparison != 0 ) { return countryComparison; }

        int line1Comparison = string.Compare( _line1, other.Line1, StringComparison.Ordinal );
        if ( line1Comparison != 0 ) { return line1Comparison; }

        int line2Comparison = string.Compare( _line2, other.Line2, StringComparison.Ordinal );
        if ( line2Comparison != 0 ) { return line2Comparison; }

        int postalCodeComparison = string.Compare( _postalCode, other.PostalCode, StringComparison.Ordinal );
        if ( postalCodeComparison != 0 ) { return postalCodeComparison; }

        int stateComparison = string.Compare( _state, other.StateOrProvince, StringComparison.Ordinal );
        if ( stateComparison != 0 ) { return stateComparison; }

        int addressComparison = string.Compare( _address, other.Address, StringComparison.Ordinal );
        if ( addressComparison != 0 ) { return addressComparison; }

        return ((int)PreferredLanguage).CompareTo( (int)other.PreferredLanguage );
    }
    public int CompareTo( object? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        return other is UserData data
                   ? CompareTo( data )
                   : throw new ArgumentException( $"Object must be of type {nameof(UserData)}" );
    }


    public static bool operator ==( UserData? left, UserData? right ) => IUserData.Equalizer.Equals( left, right );
    public static bool operator !=( UserData? left, UserData? right ) => !IUserData.Equalizer.Equals( left, right );
    public static bool operator <( UserData?  left, UserData? right ) => IUserData.Sorter.Compare( left, right ) < 0;
    public static bool operator >( UserData?  left, UserData? right ) => IUserData.Sorter.Compare( left, right ) > 0;
    public static bool operator <=( UserData? left, UserData? right ) => IUserData.Sorter.Compare( left, right ) <= 0;
    public static bool operator >=( UserData? left, UserData? right ) => IUserData.Sorter.Compare( left, right ) >= 0;
}