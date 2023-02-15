// Jakar.Extensions :: Jakar.Extensions
// 11/15/2022  6:02 PM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "UnusedMemberInSuper.Global" )]
public interface IUserData : JsonModels.IJsonModel
{
    [Required] public string            FirstName         { get; set; }
    [Required] public string            LastName          { get; set; }
    public            string?           Address           { get; set; }
    public            string?           City              { get; set; }
    public            string?           Company           { get; set; }
    public            string?           Country           { get; set; }
    public            string?           Department        { get; set; }
    public            string?           Description       { get; set; }
    public            string?           Email             { get; set; }
    public            string?           Ext               { get; set; }
    public            string?           FullName          { get; set; }
    public            string?           Line1             { get; set; }
    public            string?           Line2             { get; set; }
    public            string?           PhoneNumber       { get; set; }
    [Required] public string?           PostalCode        { get; set; }
    public            string?           State             { get; set; }
    public            string?           Title             { get; set; }
    public            string?           Website           { get; set; }
    [Required] public SupportedLanguage PreferredLanguage { get; set; }


    public IUserData Update( IUserData model );
}



public record UserData : BaseJsonModelRecord, IUserData
{
    private string            _firstName = string.Empty;
    private string            _lastName  = string.Empty;
    private string?           _address;
    private string?           _city;
    private string?           _company;
    private string?           _country;
    private string?           _department;
    private string?           _description;
    private string?           _email;
    private string?           _ext;
    private string?           _fullName;
    private string?           _line1;
    private string?           _line2;
    private string?           _phoneNumber;
    private string?           _postalCode;
    private string?           _state;
    private string?           _title;
    private string?           _website;
    private SupportedLanguage _preferredLanguage;


    public string FirstName
    {
        get => _firstName;
        set => SetProperty( ref _firstName, value );
    }
    public string LastName
    {
        get => _lastName;
        set => SetProperty( ref _lastName, value );
    }
    public string? Address
    {
        get => _address;
        set => SetProperty( ref _address, value );
    }
    public string? City
    {
        get => _city;
        set => SetProperty( ref _city, value );
    }
    public string? Company
    {
        get => _company;
        set => SetProperty( ref _company, value );
    }
    public string? Country
    {
        get => _country;
        set => SetProperty( ref _country, value );
    }
    public string? Department
    {
        get => _department;
        set => SetProperty( ref _department, value );
    }
    public string? Description
    {
        get => _description;
        set => SetProperty( ref _description, value );
    }
    public string? Email
    {
        get => _email;
        set => SetProperty( ref _email, value );
    }
    public string? Ext
    {
        get => _ext;
        set => SetProperty( ref _ext, value );
    }
    public string? FullName
    {
        get => _fullName;
        set => SetProperty( ref _fullName, value );
    }
    public string? Line1
    {
        get => _line1;
        set => SetProperty( ref _line1, value );
    }
    public string? Line2
    {
        get => _line2;
        set => SetProperty( ref _line2, value );
    }
    public string? PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty( ref _phoneNumber, value );
    }
    public string? PostalCode
    {
        get => _postalCode;
        set => SetProperty( ref _postalCode, value );
    }
    public string? State
    {
        get => _state;
        set => SetProperty( ref _state, value );
    }
    public string? Title
    {
        get => _title;
        set => SetProperty( ref _title, value );
    }
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


        if ( value.AdditionalData is null ) { return this; }

        AdditionalData ??= new Dictionary<string, JToken?>();
        foreach ( (string? key, JToken? jToken) in value.AdditionalData ) { AdditionalData[key] = jToken; }

        return this;
    }
    IUserData IUserData.Update( IUserData value ) => Update( value );
}
