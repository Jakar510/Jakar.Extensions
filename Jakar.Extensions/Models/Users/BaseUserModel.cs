#nullable enable
namespace Jakar.Extensions.Models.Users;


public interface IUserRecord : IDataBaseID { }



public interface IUserModel : IUserRecord
{
    string            UserID            { get; }
    string            UserName          { get; }
    string            Address           { get; }
    string            PhoneNumber       { get; }
    string            Email             { get; }
    string            Pager             { get; }
    string            Fax               { get; }
    string            Ext               { get; }
    string            Department        { get; }
    string            Title             { get; }
    string            FirstName         { get; }
    string            LastName          { get; }
    SupportedLanguage PreferredLanguage { get; }
}



[Serializable]
public abstract class BaseUserModel<T> : BaseCollections<T>, IUserModel where T : BaseUserModel<T>
{
    private string            _address     = string.Empty;
    private string            _company     = string.Empty;
    private string            _department  = string.Empty;
    private string            _email       = string.Empty;
    private string            _ext         = string.Empty;
    private string            _fax         = string.Empty;
    private string            _firstName   = string.Empty;
    private string            _lastName    = string.Empty;
    private string            _pager       = string.Empty;
    private string            _phoneNumber = string.Empty;
    private SupportedLanguage _preferredLanguage;
    private string            _title    = string.Empty;
    private string            _userID   = string.Empty;
    private string            _userName = string.Empty;
    private string            _website  = string.Empty;


    [Key] public long ID { get; init; }


    public string Company
    {
        get => _company;
        set => SetProperty(ref _company, value);
    }


    public string Website
    {
        get => _website;
        set => SetProperty(ref _website, value);
    }
    

    [Required]
    public string UserID
    {
        get => _userID;
        set => SetProperty(ref _userID, value);
    }


    [Required]
    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }


    public string Department
    {
        get => _department;
        set => SetProperty(ref _department, value);
    }


    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }


    [Required]
    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }


    [Required]
    public string LastName
    {
        get => _lastName;
        set => SetProperty(ref _lastName, value);
    }


    [Required]
    public string Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }


    [Required]
    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }


    [Required]
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value);
    }


    public string Ext
    {
        get => _ext;
        set => SetProperty(ref _ext, value);
    }


    public string Pager
    {
        get => _pager;
        set => SetProperty(ref _pager, value);
    }


    public string Fax
    {
        get => _fax;
        set => SetProperty(ref _fax, value);
    }


    public SupportedLanguage PreferredLanguage
    {
        get => _preferredLanguage;
        set => SetProperty(ref _preferredLanguage, value);
    }



    protected BaseUserModel() { }
    protected BaseUserModel( IUserModel model )
    {
        ID                = model.ID;
        UserID            = model.UserID;
        UserName          = model.UserName;
        Address           = model.Address;
        PhoneNumber       = model.PhoneNumber;
        Email             = model.Email;
        Pager             = model.Pager;
        Fax               = model.Fax;
        Ext               = model.Ext;
        Title             = model.Title;
        Department        = model.Department;
        FirstName         = model.FirstName;
        LastName          = model.LastName;
        PreferredLanguage = model.PreferredLanguage;
    }


    public string FullName() => $"{FirstName} {LastName}";
    public string Description() => $"{Department}, {Title} at {Company}";


    public virtual void SetValue( PhoneNumber phone ) => PhoneNumber = phone.ToString();
    public virtual void SetValue( Email       email ) => Email = email.ToString();


    public override bool Equals( T? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return _userName == other._userName && _department == other._department && _title == other._title && _firstName == other._firstName && _lastName == other._lastName && _address == other._address && _phoneNumber == other._phoneNumber &&
               _email == other._email && _ext == other._ext && _pager == other._pager && _fax == other._fax && _website == other._website && _preferredLanguage == other._preferredLanguage && UserID == other.UserID;
    }
    public override int CompareTo( T? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( other is null ) { return 1; }

        int userNameComparison = string.Compare(_userName, other._userName, StringComparison.Ordinal);
        if ( userNameComparison != 0 ) { return userNameComparison; }

        int departmentComparison = string.Compare(_department, other._department, StringComparison.Ordinal);
        if ( departmentComparison != 0 ) { return departmentComparison; }

        int titleComparison = string.Compare(_title, other._title, StringComparison.Ordinal);
        if ( titleComparison != 0 ) { return titleComparison; }

        int firstNameComparison = string.Compare(_firstName, other._firstName, StringComparison.Ordinal);
        if ( firstNameComparison != 0 ) { return firstNameComparison; }

        int lastNameComparison = string.Compare(_lastName, other._lastName, StringComparison.Ordinal);
        if ( lastNameComparison != 0 ) { return lastNameComparison; }

        int addressComparison = string.Compare(Address, other._address, StringComparison.Ordinal);
        if ( addressComparison != 0 ) { return addressComparison; }

        int phoneNumberComparison = string.Compare(_phoneNumber, other._phoneNumber, StringComparison.Ordinal);
        if ( phoneNumberComparison != 0 ) { return phoneNumberComparison; }

        int emailComparison = string.Compare(_email, other._email, StringComparison.Ordinal);
        if ( emailComparison != 0 ) { return emailComparison; }

        int extComparison = string.Compare(_ext, other._ext, StringComparison.Ordinal);
        if ( extComparison != 0 ) { return extComparison; }

        int pagerComparison = string.Compare(_pager, other._pager, StringComparison.Ordinal);
        if ( pagerComparison != 0 ) { return pagerComparison; }

        int faxComparison = string.Compare(_fax, other._fax, StringComparison.Ordinal);
        if ( faxComparison != 0 ) { return faxComparison; }

        int websiteComparison = string.Compare(_website, other._website, StringComparison.Ordinal);
        if ( websiteComparison != 0 ) { return websiteComparison; }

        int user = string.Compare(UserID, other.UserID, StringComparison.Ordinal);
        if ( user != 0 ) { return user; }

        return _preferredLanguage.CompareTo(other._preferredLanguage);
    }
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(_userName);
        hashCode.Add(_department);
        hashCode.Add(_title);
        hashCode.Add(_firstName);
        hashCode.Add(_lastName);
        hashCode.Add(_address);
        hashCode.Add(_phoneNumber);
        hashCode.Add(_email);
        hashCode.Add(_ext);
        hashCode.Add(_pager);
        hashCode.Add(_fax);
        hashCode.Add(_website);
        hashCode.Add(_preferredLanguage);
        hashCode.Add(UserID);
        return hashCode.ToHashCode();
    }

}
