// Jakar.Extensions :: Jakar.Extensions
// 11/15/2022  6:02 PM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "UnusedMemberInSuper.Global" )]
public interface IUserData : JsonModels.IJsonModel
{
    [MaxLength( 4096 )]           public string            Address           { get; set; }
    [MaxLength( 256 )]            public string            City              { get; set; }
    [MaxLength( 256 )]            public string            Company           { get; set; }
    [MaxLength( 256 )]            public string            Country           { get; set; }
    [MaxLength( 256 )]            public string            Department        { get; set; }
    [MaxLength( 256 )]            public string            Description       { get; set; }
    [MaxLength( 1024 )]           public string            Email             { get; set; }
    [MaxLength( 256 )]            public string            Ext               { get; set; }
    [MaxLength( 256 )] [Required] public string            FirstName         { get; set; }
    [MaxLength( 512 )]            public string            FullName          { get; set; }
    [MaxLength( 256 )] [Required] public string            LastName          { get; set; }
    [MaxLength( 512 )]            public string            Line1             { get; set; }
    [MaxLength( 256 )]            public string            Line2             { get; set; }
    [MaxLength( 256 )]            public string            PhoneNumber       { get; set; }
    [MaxLength( 256 )] [Required] public string            PostalCode        { get; set; }
    [Required]                    public SupportedLanguage PreferredLanguage { get; set; }
    [MaxLength( 256 )]            public string            StateOrProvince   { get; set; }
    [MaxLength( 256 )]            public string            Title             { get; set; }
    [MaxLength( 4096 )]           public string            Website           { get; set; }


    public IUserData Update( IUserData model );
}



public class UserData : ObservableClass, IUserData
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
        set => SetProperty( ref _firstName, value );
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
        set => SetProperty( ref _lastName, value );
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

    [MaxLength( 256 )]
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty( ref _phoneNumber, value );
    }

    [Required]
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

    [MaxLength( 4096 )]
    public string Website
    {
        get => _website;
        set => SetProperty( ref _website, value );
    }


    public UserData() { }
    public UserData( IUserData model ) => Update( model );
    public UserData( string firstName, string lastName )
    {
        _firstName = firstName;
        _lastName  = lastName;
    }


    public UserData Update( IUserData value )
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


        if ( value.AdditionalData is null ) { return this; }

        AdditionalData ??= new Dictionary<string, JToken?>();
        foreach ( (string key, JToken? jToken) in value.AdditionalData ) { AdditionalData[key] = jToken; }

        return this;
    }
    IUserData IUserData.Update( IUserData value ) => Update( value );
}
