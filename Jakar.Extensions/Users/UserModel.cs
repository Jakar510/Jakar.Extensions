// Jakar.Extensions :: Jakar.Extensions
// 11/15/2022  6:02 PM

namespace Jakar.Extensions;


[Serializable]
public abstract class UserModel<TClass, TID, TAddress, TGroupModel, TRoleModel> : ObservableClass<TClass, TID>, IUserData<TID, TAddress, TGroupModel, TRoleModel>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TGroupModel : IGroupModel<TID>, IEquatable<TGroupModel>
    where TRoleModel : IRoleModel<TID>, IEquatable<TRoleModel>
    where TAddress : IAddress<TID>, IEquatable<TAddress>
    where TClass : UserModel<TClass, TID, TAddress, TGroupModel, TRoleModel>, ICreateUserModel<TClass, TID, TAddress, TGroupModel, TRoleModel>, new()
{
    public const string                        EMPTY_PHONE_NUMBER = "(000) 000-0000";
    private      IDictionary<string, JToken?>? _additionalData;
    private      string                        _company     = string.Empty;
    private      string                        _department  = string.Empty;
    private      string                        _email       = string.Empty;
    private      string                        _ext         = string.Empty;
    private      string                        _firstName   = string.Empty;
    private      string                        _gender      = string.Empty;
    private      string                        _lastName    = string.Empty;
    private      string                        _phoneNumber = string.Empty;
    private      string                        _rights      = string.Empty;
    private      string                        _title       = string.Empty;
    private      string                        _userName    = string.Empty;
    private      string                        _website     = string.Empty;
    protected    string?                       _description;
    protected    string?                       _fullName;
    private      SupportedLanguage             _preferredLanguage = SupportedLanguage.English;
    private      TID?                          _createdBy;
    private      TID?                          _escalateTo;
    private      TID?                          _imageID;


    [JsonExtensionData] public IDictionary<string, JToken?>?  AdditionalData { get => _additionalData; set => SetProperty( ref _additionalData, value ); }
    public                     ObservableCollection<TAddress> Addresses      { get;                    init; } = [];

    [StringLength( UNICODE_CAPACITY )]
    public string Company
    {
        get => _company;
        set
        {
            if ( !SetProperty( ref _company, value ) ) { return; }

            _description = null;
            OnPropertyChanged( nameof(Description) );
        }
    }

    public TID? CreatedBy { get => _createdBy; set => SetProperty( ref _createdBy, value ); }

    [StringLength( UNICODE_CAPACITY )]
    public string Department
    {
        get => _department;
        set
        {
            if ( !SetProperty( ref _department, value ) ) { return; }

            _description = null;
            OnPropertyChanged( nameof(Description) );
        }
    }

    [StringLength(               UNICODE_CAPACITY )] public string Description { get => _description ??= GetDescription(); set => SetProperty( ref _description, value ); }
    [EmailAddress, StringLength( UNICODE_CAPACITY )] public string Email       { get => _email;                            set => SetProperty( ref _email,       value ); }
    public                                                  TID?   EscalateTo  { get => _escalateTo;                       set => SetProperty( ref _escalateTo,  value ); }
    [StringLength( UNICODE_CAPACITY )] public               string Ext         { get => _ext;                              set => SetProperty( ref _ext,         value ); }

    [Required, StringLength( 2000 )]
    public string FirstName
    {
        get => _firstName;
        set
        {
            if ( !SetProperty( ref _firstName, value ) ) { return; }

            _fullName = null;
            OnPropertyChanged( nameof(FullName) );
        }
    }

    [StringLength( UNICODE_CAPACITY )] public string                            FullName { get => _fullName ??= GetFullName(); set => SetProperty( ref _fullName, value ); }
    [StringLength( UNICODE_CAPACITY )] public string                            Gender   { get => _gender;                     set => SetProperty( ref _gender,   value ); }
    public                                    ObservableCollection<TGroupModel> Groups   { get;                                init; } = [];
    public                                    TID?                              ImageID  { get => _imageID;                    set => SetProperty( ref _imageID, value ); }


    [JsonIgnore] public virtual bool IsValid            { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => IsValidEmail                                && IsValidName && IsValidUserName; }
    [JsonIgnore] public virtual bool IsValidEmail       { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => string.IsNullOrWhiteSpace( Email ) is false && Email.IsEmailAddress(); }
    [JsonIgnore] public virtual bool IsValidName        { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => string.IsNullOrWhiteSpace( FullName ) is false; }
    [JsonIgnore] public virtual bool IsValidPhoneNumber { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => string.IsNullOrWhiteSpace( PhoneNumber ) is false; }
    [JsonIgnore] public virtual bool IsValidUserName    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => string.IsNullOrWhiteSpace( UserName ) is false; }
    [JsonIgnore] public virtual bool IsValidWebsite     { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => Uri.TryCreate( Website, UriKind.RelativeOrAbsolute, out _ ); }


    [Required, StringLength( 2000 )]
    public string LastName
    {
        get => _lastName;
        set
        {
            if ( !SetProperty( ref _lastName, value ) ) { return; }

            _fullName = null;
            OnPropertyChanged( nameof(FullName) );
        }
    }

    [Phone, StringLength( UNICODE_CAPACITY )]   public string                           PhoneNumber         { get => _phoneNumber;       set => SetProperty( ref _phoneNumber,       value ); }
    [EnumDataType( typeof(SupportedLanguage) )] public SupportedLanguage                PreferredLanguage   { get => _preferredLanguage; set => SetProperty( ref _preferredLanguage, value ); }
    [StringLength( IUserRights.MAX_SIZE )]      public string                           Rights              { get => _rights;            set => SetProperty( ref _rights,            value ); }
    public                                             ObservableCollection<TRoleModel> Roles               { get;                       init; } = [];
    public                                             DateTimeOffset?                  SubscriptionExpires { get;                       init; }

    [StringLength( UNICODE_CAPACITY )]
    public string Title
    {
        get => _title;
        set
        {
            if ( !SetProperty( ref _title, value ) ) { return; }

            _description = null;
            OnPropertyChanged( nameof(Description) );
        }
    }
    public Guid UserID { get; init; }

    [StringLength( UNICODE_CAPACITY )]
    public virtual string UserName
    {
        get => _userName;
        set
        {
            if ( SetProperty( ref _userName, value ) ) { OnPropertyChanged( nameof(IsValid) ); }
        }
    }

    [Url, StringLength( UNICODE_CAPACITY )] public string Website { get => _website; set => SetProperty( ref _website, value ); }


    protected UserModel() : base() { }
    protected UserModel( IUserData<TID> value ) : base( value.ID )
    {
        With( value );
        if ( value is IUserData<Guid> data ) { UserID = data.ID; }
    }
    protected UserModel( string firstName, string lastName )
    {
        _firstName = firstName;
        _lastName  = lastName;
    }


    public virtual string GetFullName()    => IUserData.GetFullName( this );
    public virtual string GetDescription() => IUserData.GetDescription( this );


    public virtual UserRights<TEnum> GetRights<TEnum>()
        where TEnum : struct, Enum => UserRights<TEnum>.Create( this ).With( Groups );


    public TClass With( IEnumerable<TAddress> addresses )
    {
        Addresses.Add( addresses );
        return (TClass)this;
    }
    public TClass With( scoped in ReadOnlySpan<TAddress> addresses )
    {
        Addresses.Add( addresses );
        return (TClass)this;
    }
    public TClass With( IEnumerable<TGroupModel> values )
    {
        Groups.Add( values );
        return (TClass)this;
    }
    public TClass With( scoped in ReadOnlySpan<TGroupModel> values )
    {
        Groups.Add( values );
        return (TClass)this;
    }
    public TClass With( IEnumerable<TRoleModel> values )
    {
        Roles.Add( values );
        return (TClass)this;
    }
    public TClass With( scoped in ReadOnlySpan<TRoleModel> values )
    {
        Roles.Add( values );
        return (TClass)this;
    }


    void IUserData<TID>.With( IUserData<TID> value ) => With( value );
    public TClass With<TValue>( TValue value )
        where TValue : IUserData<TID>
    {
        CreatedBy         = value.CreatedBy;
        EscalateTo        = value.EscalateTo;
        FirstName         = value.FirstName;
        LastName          = value.LastName;
        FullName          = value.FullName;
        Description       = value.Description;
        Website           = value.Website;
        Email             = value.Email;
        PhoneNumber       = value.PhoneNumber;
        Ext               = value.Ext;
        Title             = value.Title;
        Department        = value.Department;
        Company           = value.Company;
        PreferredLanguage = value.PreferredLanguage;
        Rights            = value.Rights;
        return With( value.AdditionalData );
    }
    public TClass With( IDictionary<string, JToken?>? data )
    {
        if ( data?.Count is null or 0 ) { return (TClass)this; }

        IDictionary<string, JToken?> dict = AdditionalData ??= new Dictionary<string, JToken?>();
        foreach ( (string key, JToken? jToken) in data ) { dict[key] = jToken; }

        return (TClass)this;
    }


    public override int CompareTo( TClass? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int addressComparison = string.Compare( UserName, other.UserName, StringComparison.Ordinal );
        if ( addressComparison != 0 ) { return addressComparison; }

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

        return ((int)PreferredLanguage).CompareTo( (int)other.PreferredLanguage );
    }
    public override bool Equals( TClass? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return _company == other._company && _department == other._department && _email == other._email && _ext == other._ext && _firstName == other._firstName && _gender == other._gender && _lastName == other._lastName && _phoneNumber == other._phoneNumber && _rights == other._rights && _title == other._title && _userName == other._userName && _website == other._website && _description == other._description && _fullName == other._fullName && _preferredLanguage == other._preferredLanguage && Nullable.Equals( _createdBy, other._createdBy ) && Nullable.Equals( _escalateTo, other._escalateTo ) && Nullable.Equals( _imageID, other._imageID ) && Equals( UserID, other.UserID ) && Addresses.Equals( other.Addresses ) && Groups.Equals( other.Groups ) && ID.Equals( other.ID ) && Roles.Equals( other.Roles ) && Nullable.Equals( SubscriptionExpires, other.SubscriptionExpires );
    }
    protected override int GetHashCodeInternal()
    {
        HashCode hashCode = new();
        hashCode.Add( base.GetHashCode() );
        hashCode.Add( _additionalData );
        hashCode.Add( _company );
        hashCode.Add( _department );
        hashCode.Add( _email );
        hashCode.Add( _ext );
        hashCode.Add( _firstName );
        hashCode.Add( _gender );
        hashCode.Add( _lastName );
        hashCode.Add( _phoneNumber );
        hashCode.Add( _rights );
        hashCode.Add( _title );
        hashCode.Add( _userName );
        hashCode.Add( _website );
        hashCode.Add( _description );
        hashCode.Add( _fullName );
        hashCode.Add( _preferredLanguage );
        hashCode.Add( _createdBy );
        hashCode.Add( _escalateTo );
        hashCode.Add( _imageID );
        hashCode.Add( UserID );
        hashCode.Add( Addresses );
        hashCode.Add( Groups );
        hashCode.Add( ID );
        hashCode.Add( Roles );
        hashCode.Add( SubscriptionExpires );
        return hashCode.ToHashCode();
    }
}



[Serializable]
public abstract class UserModel<TClass, TID> : UserModel<TClass, TID, UserAddress<TID>, GroupModel<TID>, RoleModel<TID>>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : UserModel<TClass, TID>, ICreateUserModel<TClass, TID, UserAddress<TID>, GroupModel<TID>, RoleModel<TID>>, new()

{
    protected UserModel() : base() { }
    protected UserModel( IUserData<TID> value ) : base( value ) { }
    protected UserModel( string         firstName, string lastName ) : base( firstName, lastName ) { }
}



[Serializable]
public sealed class UserModel<TID> : UserModel<UserModel<TID>, TID, UserAddress<TID>, GroupModel<TID>, RoleModel<TID>>, ICreateUserModel<UserModel<TID>, TID, UserAddress<TID>, GroupModel<TID>, RoleModel<TID>>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public UserModel() : base() { }
    public UserModel( IUserData<TID> value ) : base( value ) { }
    public UserModel( string         firstName, string lastName ) : base( firstName, lastName ) { }


    public static UserModel<TID> Create( IUserData<TID> model )                                                                                                                                                   => new(model);
    public static UserModel<TID> Create( IUserData<TID> model, IEnumerable<UserAddress<TID>>            addresses, IEnumerable<GroupModel<TID>>            groups, IEnumerable<RoleModel<TID>>            roles ) => Create( model ).With( addresses ).With( groups ).With( roles );
    public static UserModel<TID> Create( IUserData<TID> model, scoped in ReadOnlySpan<UserAddress<TID>> addresses, scoped in ReadOnlySpan<GroupModel<TID>> groups, scoped in ReadOnlySpan<RoleModel<TID>> roles ) => Create( model ).With( addresses ).With( groups ).With( roles );
    public static async ValueTask<UserModel<TID>> CreateAsync( IUserData<TID> model, IAsyncEnumerable<UserAddress<TID>> addresses, IAsyncEnumerable<GroupModel<TID>> groups, IAsyncEnumerable<RoleModel<TID>> roles, CancellationToken token = default )
    {
        UserModel<TID> user = Create( model );
        await user.Addresses.Add( addresses, token );
        await user.Groups.Add( groups, token );
        await user.Roles.Add( roles, token );
        return user;
    }
}
