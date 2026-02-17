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
    private   string?           __company     = EMPTY;
    private   string?           __department  = EMPTY;
    private   string?           __email       = EMPTY;
    private   string?           __ext         = EMPTY;
    private   string?           __firstName   = EMPTY;
    private   string?           __gender      = EMPTY;
    private   string?           __lastName    = EMPTY;
    private   string?           __phoneNumber = EMPTY;
    private   string?           __title       = EMPTY;
    private   string            __userName    = EMPTY;
    private   string?           __website     = EMPTY;
    protected string?           _description;
    protected string?           _fullName;
    private   SupportedLanguage __preferredLanguage = SupportedLanguage.English;
    protected TID               _id;
    private   TID?              __createdBy;
    private   TID?              __escalateTo;
    private   TID?              __imageID;
    private   UserRights        __rights = new();


    public ObservableCollection<TAddress> Addresses { get; init; } = [];

    [StringLength(COMPANY)] public string? Company
    {
        get => __company;
        set
        {
            if ( !SetProperty(ref __company, value) ) { return; }

            _description = null;
            OnPropertyChanged(nameof(Description));
        }
    }

    public TID? CreatedBy { get => __createdBy; set => SetProperty(ref __createdBy, value); }

    [StringLength(DEPARTMENT)] public string? Department
    {
        get => __department;
        set
        {
            if ( !SetProperty(ref __department, value) ) { return; }

            _description = null;
            OnPropertyChanged(nameof(Description));
        }
    }

    [StringLength(               DESCRIPTION)] public string? Description { get => _description ??= GetDescription(); set => SetProperty(ref _description, value); }
    [EmailAddress] [StringLength(EMAIL)]       public string? Email       { get => __email;                           set => SetProperty(ref __email,      value); }
    public                                            TID?    EscalateTo  { get => __escalateTo;                      set => SetProperty(ref __escalateTo, value); }
    [StringLength(PHONE_EXT)] public                  string? Ext         { get => __ext;                             set => SetProperty(ref __ext,        value); }

    [Required] [StringLength(2000)] public string? FirstName
    {
        get => __firstName;
        set
        {
            if ( !SetProperty(ref __firstName, value) ) { return; }

            _fullName = null;
            OnPropertyChanged(nameof(FullName));
        }
    }

    [StringLength(FULL_NAME)] public string?                           FullName { get => _fullName ??= GetFullName(); set => SetProperty(ref _fullName, value); }
    [StringLength(GENDER)]    public string?                           Gender   { get => __gender;                    set => SetProperty(ref __gender,  value); }
    public                           ObservableCollection<TGroupModel> Groups   { get;                                init; } = [];


    public                      TID  ID                 { get => _id;       init => _id = value; }
    public                      TID? ImageID            { get => __imageID; set => SetProperty(ref __imageID, value); }
    [JsonIgnore] public virtual bool IsValid            => IsValidEmail                      && IsValidName && IsValidUserName;
    [JsonIgnore] public virtual bool IsValidEmail       => !string.IsNullOrWhiteSpace(Email) && Email.IsEmailAddress();
    [JsonIgnore] public virtual bool IsValidName        => !string.IsNullOrWhiteSpace(FullName);
    [JsonIgnore] public virtual bool IsValidPhoneNumber => !string.IsNullOrWhiteSpace(PhoneNumber);
    [JsonIgnore] public virtual bool IsValidUserName    => !string.IsNullOrWhiteSpace(UserName);
    [JsonIgnore] public virtual bool IsValidWebsite     => Uri.TryCreate(Website, UriKind.RelativeOrAbsolute, out _);

    [Required] [StringLength(2000)] public string? LastName
    {
        get => __lastName;
        set
        {
            if ( !SetProperty(ref __lastName, value) ) { return; }

            _fullName = null;
            OnPropertyChanged(nameof(FullName));
        }
    }

    [Phone] [StringLength(PHONE)]             public string?                          PhoneNumber         { get => __phoneNumber;       set => SetProperty(ref __phoneNumber,       value); }
    [EnumDataType(typeof(SupportedLanguage))] public SupportedLanguage                PreferredLanguage   { get => __preferredLanguage; set => SetProperty(ref __preferredLanguage, value); }
    [StringLength(RIGHTS)]                    public UserRights                       Rights              { get => __rights;            set => SetProperty(ref __rights,            value); }
    public                                           ObservableCollection<TRoleModel> Roles               { get;                        init; } = [];
    public                                           DateTimeOffset?                  SubscriptionExpires { get;                        init; }

    [StringLength(TITLE)] public string? Title
    {
        get => __title;
        set
        {
            if ( !SetProperty(ref __title, value) ) { return; }

            _description = null;
            OnPropertyChanged(nameof(Description));
        }
    }
    public Guid UserID { get; init; }

    [StringLength(USER_NAME)] public virtual string UserName
    {
        get => __userName;
        set
        {
            if ( SetProperty(ref __userName, value) ) { OnPropertyChanged(nameof(IsValid)); }
        }
    }

    [Url] [StringLength(WEBSITE)] public string? Website { get => __website; set => SetProperty(ref __website, value); }


    protected UserModel() : base() { }
    protected UserModel( IUserData<TID> value ) : base()
    {
        ID     = value.ID;
        UserID = value.UserID;
        With(value);
    }
    protected UserModel( string firstName, string lastName )
    {
        __firstName = firstName;
        __lastName  = lastName;
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
        Rights            = value.Rights;
        CreatedBy         = value.CreatedBy;
        EscalateTo        = value.EscalateTo;
        ImageID           = value.ImageID;
        CreatedBy         = value.CreatedBy;
        EscalateTo        = value.EscalateTo;
        PreferredLanguage = value.PreferredLanguage;
        Rights            = value.Rights;
        return With(value.AdditionalData);
    }
    public TSelf With( JObject? data )
    {
        if ( data?.Count is null or 0 ) { return (TSelf)this; }

        JObject dict = AdditionalData ??= new JObject();
        foreach ( ( string key, JToken? jToken ) in data ) { dict[key] = jToken; }

        return (TSelf)this;
    }


    public override int CompareTo( TSelf? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int addressComparison = string.Compare(UserName, other.UserName, StringComparison.Ordinal);
        if ( addressComparison != 0 ) { return addressComparison; }

        int firstNameComparison = string.Compare(__firstName, other.FirstName, StringComparison.Ordinal);
        if ( firstNameComparison != 0 ) { return firstNameComparison; }

        int lastNameComparison = string.Compare(__lastName, other.LastName, StringComparison.Ordinal);
        if ( lastNameComparison != 0 ) { return lastNameComparison; }

        int fullNameComparison = string.Compare(_fullName, other.FullName, StringComparison.Ordinal);
        if ( fullNameComparison != 0 ) { return fullNameComparison; }

        int descriptionComparison = string.Compare(_description, other.Description, StringComparison.Ordinal);
        if ( descriptionComparison != 0 ) { return descriptionComparison; }

        int companyComparison = string.Compare(__company, other.Company, StringComparison.Ordinal);
        if ( companyComparison != 0 ) { return companyComparison; }

        int departmentComparison = string.Compare(__department, other.Department, StringComparison.Ordinal);
        if ( departmentComparison != 0 ) { return departmentComparison; }

        int titleComparison = string.Compare(__title, other.Title, StringComparison.Ordinal);
        if ( titleComparison != 0 ) { return titleComparison; }

        int emailComparison = string.Compare(__email, other.Email, StringComparison.Ordinal);
        if ( emailComparison != 0 ) { return emailComparison; }

        int phoneNumberComparison = string.Compare(__phoneNumber, other.PhoneNumber, StringComparison.Ordinal);
        if ( phoneNumberComparison != 0 ) { return phoneNumberComparison; }

        int extComparison = string.Compare(__ext, other.Ext, StringComparison.Ordinal);
        if ( extComparison != 0 ) { return extComparison; }

        int websiteComparison = string.Compare(__website, other.Website, StringComparison.Ordinal);
        if ( websiteComparison != 0 ) { return websiteComparison; }

        return ( (int)PreferredLanguage ).CompareTo((int)other.PreferredLanguage);
    }
    public override bool Equals( TSelf? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return __company           == other.__company            &&
               __department        == other.__department         &&
               __email             == other.__email              &&
               __ext               == other.__ext                &&
               __firstName         == other.__firstName          &&
               __gender            == other.__gender             &&
               __lastName          == other.__lastName           &&
               __phoneNumber       == other.__phoneNumber        &&
               __rights            == other.__rights             &&
               __title             == other.__title              &&
               __userName          == other.__userName           &&
               __website           == other.__website            &&
               _description        == other._description         &&
               _fullName           == other._fullName            &&
               __preferredLanguage == other.__preferredLanguage  &&
               Nullable.Equals(__createdBy,  other.__createdBy)  &&
               Nullable.Equals(__escalateTo, other.__escalateTo) &&
               Nullable.Equals(__imageID,    other.__imageID)    &&
               Equals(UserID, other.UserID)                      &&
               Addresses.Equals(other.Addresses)                 &&
               Groups.Equals(other.Groups)                       &&
               ID.Equals(other.ID)                               &&
               Roles.Equals(other.Roles)                         &&
               Nullable.Equals(SubscriptionExpires, other.SubscriptionExpires);
    }
    public override int GetHashCode()
    {
        HashCode hashCode = new();
        hashCode.Add(base.GetHashCode());
        hashCode.Add(_additionalData);
        hashCode.Add(__company);
        hashCode.Add(__department);
        hashCode.Add(__email);
        hashCode.Add(__ext);
        hashCode.Add(__firstName);
        hashCode.Add(__gender);
        hashCode.Add(__lastName);
        hashCode.Add(__phoneNumber);
        hashCode.Add(__rights);
        hashCode.Add(__title);
        hashCode.Add(__userName);
        hashCode.Add(__website);
        hashCode.Add(_description);
        hashCode.Add(_fullName);
        hashCode.Add(__preferredLanguage);
        hashCode.Add(__createdBy);
        hashCode.Add(__escalateTo);
        hashCode.Add(__imageID);
        hashCode.Add(UserID);
        hashCode.Add(Addresses);
        hashCode.Add(Groups);
        hashCode.Add(ID);
        hashCode.Add(Roles);
        hashCode.Add(SubscriptionExpires);
        return hashCode.ToHashCode();
    }
}
