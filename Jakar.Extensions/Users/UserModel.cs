// Jakar.Extensions :: Jakar.Extensions
// 11/15/2022  6:02 PM

namespace Jakar.Extensions;


[Serializable]
public abstract class UserModel<TSelf, TID, TAddress, TGroupModel, TRoleModel> : BaseClass<TSelf>, IUserData<TID, TAddress, TGroupModel, TRoleModel>, IUserDetails
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TGroupModel : IGroupModel<TID>, IEquatable<TGroupModel>
    where TRoleModel : IRoleModel<TID>, IEquatable<TRoleModel>
    where TAddress : IAddress<TID>, IEquatable<TAddress>
    where TSelf : UserModel<TSelf, TID, TAddress, TGroupModel, TRoleModel>, ICreateUserModel<TSelf, TID, TAddress, TGroupModel, TRoleModel>, new()
{
    protected string            _userName = EMPTY;
    protected string?           _company;
    protected string?           _department;
    protected string?           _email;
    protected string?           _ext;
    protected string?           _firstName;
    protected string?           _gender;
    protected string?           _lastName;
    protected string?           _phoneNumber;
    protected string?           _title;
    protected string?           _website;
    protected string?           _description;
    protected string?           _fullName;
    protected SupportedLanguage _preferredLanguage = SupportedLanguage.English;
    protected TID               _id;
    protected TID?              _createdBy;
    protected TID?              _escalateTo;
    protected TID?              _imageID;
    protected UserRights        _rights = new();


    public ObservableCollection<TAddress> Addresses { get; init; } = [];

    [StringLength(COMPANY)] public string? Company
    {
        get => _company;
        set
        {
            if ( !SetProperty(ref _company, value) ) { return; }

            _description = null;
            OnPropertyChanged(nameof(Description));
        }
    }

    public TID? CreatedBy { get => _createdBy; set => SetProperty(ref _createdBy, value); }

    [StringLength(DEPARTMENT)] public string? Department
    {
        get => _department;
        set
        {
            if ( !SetProperty(ref _department, value) ) { return; }

            _description = null;
            OnPropertyChanged(nameof(Description));
        }
    }

    [StringLength(               DESCRIPTION)] public string? Description { get => _description ??= GetDescription(); set => SetProperty(ref _description, value); }
    [EmailAddress] [StringLength(EMAIL)]       public string? Email       { get => _email;                            set => SetProperty(ref _email,       value); }
    public                                            TID?    EscalateTo  { get => _escalateTo;                       set => SetProperty(ref _escalateTo,  value); }
    [StringLength(PHONE_EXT)] public                  string? Ext         { get => _ext;                              set => SetProperty(ref _ext,         value); }

    [Required] [StringLength(FIRST_NAME)] public string? FirstName
    {
        get => _firstName;
        set
        {
            if ( !SetProperty(ref _firstName, value) ) { return; }

            _fullName = null;
            OnPropertyChanged(nameof(FullName));
        }
    }

    [StringLength(FULL_NAME)] public string?                           FullName           { get => _fullName ??= GetFullName(); set => SetProperty(ref _fullName, value); }
    [StringLength(GENDER)]    public string?                           Gender             { get => _gender;                     set => SetProperty(ref _gender,   value); }
    public                           ObservableCollection<TGroupModel> Groups             { get;                                init; } = [];
    public                           TID                               ID                 { get => _id;                         init => _id = value; }
    public                           TID?                              ImageID            { get => _imageID;                    set => SetProperty(ref _imageID, value); }
    [JsonIgnore] public virtual      bool                              IsValid            => IsValidEmail                      && IsValidName && IsValidUserName;
    [JsonIgnore] public virtual      bool                              IsValidEmail       => !string.IsNullOrWhiteSpace(Email) && Email.IsEmailAddress();
    [JsonIgnore] public virtual      bool                              IsValidName        => !string.IsNullOrWhiteSpace(FullName);
    [JsonIgnore] public virtual      bool                              IsValidPhoneNumber => !string.IsNullOrWhiteSpace(PhoneNumber);
    [JsonIgnore] public virtual      bool                              IsValidUserName    => !string.IsNullOrWhiteSpace(UserName);
    [JsonIgnore] public virtual      bool                              IsValidWebsite     => Uri.TryCreate(Website, UriKind.RelativeOrAbsolute, out _);

    [Required] [StringLength(LAST_NAME)] public string? LastName
    {
        get => _lastName;
        set
        {
            if ( !SetProperty(ref _lastName, value) ) { return; }

            _fullName = null;
            OnPropertyChanged(nameof(FullName));
        }
    }

    [Phone] [StringLength(PHONE)]             public string?                          PhoneNumber         { get => _phoneNumber;       set => SetProperty(ref _phoneNumber,       value); }
    [EnumDataType(typeof(SupportedLanguage))] public SupportedLanguage                PreferredLanguage   { get => _preferredLanguage; set => SetProperty(ref _preferredLanguage, value); }
    [StringLength(RIGHTS)]                    public UserRights                       Rights              { get => _rights;            set => SetProperty(ref _rights,            value); }
    public                                           ObservableCollection<TRoleModel> Roles               { get;                       init; } = [];
    public                                           DateTimeOffset?                  SubscriptionExpires { get;                       init; }

    [StringLength(TITLE)] public string? Title
    {
        get => _title;
        set
        {
            if ( !SetProperty(ref _title, value) ) { return; }

            _description = null;
            OnPropertyChanged(nameof(Description));
        }
    }
    public Guid UserID { get; init; }

    [StringLength(USER_NAME)] public virtual string UserName
    {
        get => _userName;
        set
        {
            if ( SetProperty(ref _userName, value) ) { OnPropertyChanged(nameof(IsValid)); }
        }
    }

    [Url] [StringLength(WEBSITE)] public string? Website { get => _website; set => SetProperty(ref _website, value); }


    protected UserModel() : base() { }
    protected UserModel( IUserData<TID> value ) : base()
    {
        ID     = value.ID;
        UserID = value.UserID;
        With(value);
    }
    protected UserModel( string firstName, string lastName )
    {
        _firstName = firstName;
        _lastName  = lastName;
    }


    public virtual string GetFullName()    => IUserDetails.GetFullName(this);
    public virtual string GetDescription() => IUserDetails.GetDescription(this);


    public TSelf With( IEnumerable<TAddress> addresses )
    {
        Addresses.Add(addresses);
        return (TSelf)this;
    }
    public TSelf With( params ReadOnlySpan<TAddress> addresses )
    {
        Addresses.Add(addresses);
        return (TSelf)this;
    }
    public TSelf With( IEnumerable<TGroupModel> values )
    {
        Groups.Add(values);
        return (TSelf)this;
    }
    public TSelf With( params ReadOnlySpan<TGroupModel> values )
    {
        Groups.Add(values);
        return (TSelf)this;
    }
    public TSelf With( IEnumerable<TRoleModel> values )
    {
        Roles.Add(values);
        return (TSelf)this;
    }
    public TSelf With( params ReadOnlySpan<TRoleModel> values )
    {
        Roles.Add(values);
        return (TSelf)this;
    }


    public static TSelf Create<TValue>( TValue value )
        where TValue : IUserData<TID>, IUserDetails
    {
        TSelf self = TSelf.Create(value);
        return self.With(value);
    }
    public TSelf With<TValue>( TValue value )
        where TValue : IUserData<TID>, IUserDetails
    {
        FirstName   = value.FirstName;
        LastName    = value.LastName;
        FullName    = value.FullName;
        Description = value.Description;
        Website     = value.Website;
        Email       = value.Email;
        PhoneNumber = value.PhoneNumber;
        Ext         = value.Ext;
        Title       = value.Title;
        Department  = value.Department;
        Company     = value.Company;
        return With((IUserData<TID>)value);
    }
    public TSelf With( IUserData<TID> value )
    {
        UserName          = value.UserName;
        ImageID           = value.ImageID;
        CreatedBy         = value.CreatedBy;
        EscalateTo        = value.EscalateTo;
        PreferredLanguage = value.PreferredLanguage;
        Rights            = value.Rights.Value;
        return With(value.AdditionalData);
    }
    public TSelf With( IReadOnlyDictionary<string, JsonElement>? data )
    {
        AdditionalData = Json.Merge(AdditionalData, data);
        return (TSelf)this;
    }


    public override int CompareTo( TSelf? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int addressComparison = string.Compare(UserName, other.UserName, StringComparison.Ordinal);
        if ( addressComparison != 0 ) { return addressComparison; }

        int firstNameComparison = string.Compare(_firstName, other.FirstName, StringComparison.Ordinal);
        if ( firstNameComparison != 0 ) { return firstNameComparison; }

        int lastNameComparison = string.Compare(_lastName, other.LastName, StringComparison.Ordinal);
        if ( lastNameComparison != 0 ) { return lastNameComparison; }

        int fullNameComparison = string.Compare(_fullName, other.FullName, StringComparison.Ordinal);
        if ( fullNameComparison != 0 ) { return fullNameComparison; }

        int descriptionComparison = string.Compare(_description, other.Description, StringComparison.Ordinal);
        if ( descriptionComparison != 0 ) { return descriptionComparison; }

        int companyComparison = string.Compare(_company, other.Company, StringComparison.Ordinal);
        if ( companyComparison != 0 ) { return companyComparison; }

        int departmentComparison = string.Compare(_department, other.Department, StringComparison.Ordinal);
        if ( departmentComparison != 0 ) { return departmentComparison; }

        int titleComparison = string.Compare(_title, other.Title, StringComparison.Ordinal);
        if ( titleComparison != 0 ) { return titleComparison; }

        int emailComparison = string.Compare(_email, other.Email, StringComparison.Ordinal);
        if ( emailComparison != 0 ) { return emailComparison; }

        int phoneNumberComparison = string.Compare(_phoneNumber, other.PhoneNumber, StringComparison.Ordinal);
        if ( phoneNumberComparison != 0 ) { return phoneNumberComparison; }

        int extComparison = string.Compare(_ext, other.Ext, StringComparison.Ordinal);
        if ( extComparison != 0 ) { return extComparison; }

        int websiteComparison = string.Compare(_website, other.Website, StringComparison.Ordinal);
        if ( websiteComparison != 0 ) { return websiteComparison; }

        return ( (int)PreferredLanguage ).CompareTo((int)other.PreferredLanguage);
    }
    public override bool Equals( TSelf? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return _company           == other._company            &&
               _department        == other._department         &&
               _email             == other._email              &&
               _ext               == other._ext                &&
               _firstName         == other._firstName          &&
               _gender            == other._gender             &&
               _lastName          == other._lastName           &&
               _phoneNumber       == other._phoneNumber        &&
               _rights            == other._rights             &&
               _title             == other._title              &&
               _userName          == other._userName           &&
               _website           == other._website            &&
               _description       == other._description        &&
               _fullName          == other._fullName           &&
               _preferredLanguage == other._preferredLanguage  &&
               Nullable.Equals(_createdBy,  other._createdBy)  &&
               Nullable.Equals(_escalateTo, other._escalateTo) &&
               Nullable.Equals(_imageID,    other._imageID)    &&
               Equals(UserID, other.UserID)                    &&
               Addresses.Equals(other.Addresses)               &&
               Groups.Equals(other.Groups)                     &&
               ID.Equals(other.ID)                             &&
               Roles.Equals(other.Roles)                       &&
               Nullable.Equals(SubscriptionExpires, other.SubscriptionExpires);
    }
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(_additionalData);
        hashCode.Add(_company);
        hashCode.Add(_department);
        hashCode.Add(_email);
        hashCode.Add(_ext);
        hashCode.Add(_firstName);
        hashCode.Add(_gender);
        hashCode.Add(_lastName);
        hashCode.Add(_phoneNumber);
        hashCode.Add(_rights);
        hashCode.Add(_title);
        hashCode.Add(_userName);
        hashCode.Add(_website);
        hashCode.Add(_description);
        hashCode.Add(_fullName);
        hashCode.Add(_preferredLanguage);
        hashCode.Add(_createdBy);
        hashCode.Add(_escalateTo);
        hashCode.Add(_imageID);
        hashCode.Add(UserID);
        hashCode.Add(Addresses);
        hashCode.Add(Groups);
        hashCode.Add(ID);
        hashCode.Add(Roles);
        hashCode.Add(SubscriptionExpires);
        return hashCode.ToHashCode();
    }
}
