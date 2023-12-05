// Jakar.Extensions :: Jakar.Extensions
// 11/15/2022  6:02 PM

using System.Text.Json.Serialization.Metadata;



namespace Jakar.Extensions;


public interface IUserData : MsJsonModels.IJsonModel, IEquatable<IUserData>, IComparable<IUserData>
{
    public static Equalizer<IUserData> Equalizer => Equalizer<IUserData>.Default;
    public static Sorter<IUserData>    Sorter    => Sorter<IUserData>.Default;


    [ MaxLength( 256 ) ]                public string            Company           { get; }
    [ MaxLength( 256 ) ]                public string            Department        { get; }
    [ MaxLength( 256 ) ]                public string            Description       { get; }
    [ MaxLength( 1024 ), EmailAddress ] public string            Email             { get; }
    [ MaxLength( 256 ) ]                public string            Ext               { get; }
    [ MaxLength( 256 ), Required ]      public string            FirstName         { get; }
    [ MaxLength( 512 ) ]                public string            FullName          { get; }
    [ MaxLength( 256 ) ]                public string            Gender            { get; }
    [ MaxLength( 256 ), Required ]      public string            LastName          { get; }
    [ MaxLength( 256 ), Phone ]         public string            PhoneNumber       { get; }
    [ Required ]                        public SupportedLanguage PreferredLanguage { get; }
    [ MaxLength( 256 ) ]                public string            Title             { get; }
    [ MaxLength( 4096 ), Url ]          public string            Website           { get; }
}



public interface IUserData<out T> : IUserData
    where T : IUserData<T>
{
    public T WithUserData( IUserData value );
}



[ Serializable ]
public class UserData : ObservableClass, IUserData<UserData>, MsJsonModels.IJsonizer<UserData, UserDataContext>
{
    public const string                            EMPTY_PHONE_NUMBER = "(000) 000-0000";
    private      IDictionary<string, JsonElement>? _additionalData;
    private      string                            _company     = string.Empty;
    private      string                            _department  = string.Empty;
    private      string                            _email       = string.Empty;
    private      string                            _ext         = string.Empty;
    private      string                            _firstName   = string.Empty;
    private      string                            _gender      = string.Empty;
    private      string                            _lastName    = string.Empty;
    private      string                            _phoneNumber = string.Empty;
    private      string                            _title       = string.Empty;
    private      string                            _website     = string.Empty;
    private      string?                           _description;
    private      string?                           _fullName;
    private      SupportedLanguage                 _preferredLanguage = SupportedLanguage.English;


    public static UserDataContext        JsonSerializerContext => UserDataContext.Default;
    public static JsonTypeInfo<UserData> JsonTypeInfo          => JsonSerializerContext.UserData;


    [ JsonExtensionData ]
    public IDictionary<string, JsonElement>? AdditionalData
    {
        get => _additionalData;
        set => SetProperty( ref _additionalData, value );
    }

    public ObservableCollection<UserAddress> Addresses { get; init; } = new();

    [ MaxLength( 256 ) ]
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

    [ MaxLength( 256 ) ]
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

    [ MaxLength( 256 ) ]
    public string Description
    {
        get => _description ??= $"{Department}, {Title} at {Company}";
        set => SetProperty( ref _description, value );
    }

    [ EmailAddress, MaxLength( 1024 ) ]
    public string Email
    {
        get => _email;
        set => SetProperty( ref _email, value );
    }

    [ MaxLength( 256 ) ]
    public string Ext
    {
        get => _ext;
        set => SetProperty( ref _ext, value );
    }

    [ Required, MaxLength( 256 ) ]
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

    [ MaxLength( 512 ) ]
    public string FullName
    {
        get => _fullName ??= $"{FirstName} {LastName}";
        set => SetProperty( ref _fullName, value );
    }

    [ MaxLength( 256 ) ]
    public string Gender
    {
        get => _gender;
        set => SetProperty( ref _gender, value );
    }

    [ JsonIgnore ] public bool IsValidEmail       => !string.IsNullOrEmpty( Email );
    [ JsonIgnore ] public bool IsValidName        => !string.IsNullOrEmpty( FullName );
    [ JsonIgnore ] public bool IsValidPhoneNumber => !string.IsNullOrEmpty( PhoneNumber );
    [ JsonIgnore ] public bool IsValidWebsite     => Uri.TryCreate( Website, UriKind.RelativeOrAbsolute, out _ );

    [ Required, MaxLength( 256 ) ]
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

    [ Phone, MaxLength( 256 ) ]
    public string PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty( ref _phoneNumber, value );
    }

    [ Required ]
    public SupportedLanguage PreferredLanguage
    {
        get => _preferredLanguage;
        set => SetProperty( ref _preferredLanguage, value );
    }

    [ MaxLength( 256 ) ]
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

    [ Url, MaxLength( 4096 ) ]
    public string Website
    {
        get => _website;
        set => SetProperty( ref _website, value );
    }


    public UserData() { }
    public UserData( IUserData value ) => WithUserData( value );
    public UserData( IUserData value, IEnumerable<IAddress>    addresses ) : this( value ) => WithAddresses( addresses );
    public UserData( IUserData value, IEnumerable<UserAddress> addresses ) : this( value ) => WithAddresses( addresses );
    public UserData( string firstName, string lastName )
    {
        _firstName = firstName;
        _lastName  = lastName;
    }

    public static UserData FromJson( string json ) => json.FromJson<UserData, UserDataContext>();


    public UserData WithAddresses( IEnumerable<IAddress> addresses ) => WithAddresses( addresses.Select( x => new UserAddress( x ) ) );
    public UserData WithAddresses( IEnumerable<UserAddress> addresses )
    {
        Addresses.Add( addresses );
        return this;
    }
    public UserData WithUserData( IUserData value )
    {
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

        if ( value.AdditionalData is null ) { return this; }

        AdditionalData ??= new Dictionary<string, JsonElement>();
        foreach ( (string key, JsonElement jToken) in value.AdditionalData ) { AdditionalData[key] = jToken; }

        return this;
    }


    public bool Equals( IUserData? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return Equals( _additionalData, other.AdditionalData )                            &&
               string.Equals( _company,     other.Company,     StringComparison.Ordinal ) &&
               string.Equals( _department,  other.Department,  StringComparison.Ordinal ) &&
               string.Equals( _description, other.Description, StringComparison.Ordinal ) &&
               string.Equals( _email,       other.Email,       StringComparison.Ordinal ) &&
               string.Equals( _ext,         other.Ext,         StringComparison.Ordinal ) &&
               string.Equals( _firstName,   other.FirstName,   StringComparison.Ordinal ) &&
               string.Equals( _fullName,    other.FullName,    StringComparison.Ordinal ) &&
               string.Equals( _lastName,    other.LastName,    StringComparison.Ordinal ) &&
               string.Equals( _phoneNumber, other.PhoneNumber, StringComparison.Ordinal ) &&
               string.Equals( _title,       other.Title,       StringComparison.Ordinal ) &&
               string.Equals( _website,     other.Website,     StringComparison.Ordinal ) &&
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
        hashCode.Add( Company );
        hashCode.Add( Department );
        hashCode.Add( Description );
        hashCode.Add( Email );
        hashCode.Add( Ext );
        hashCode.Add( FirstName );
        hashCode.Add( FullName );
        hashCode.Add( LastName );
        hashCode.Add( PhoneNumber );
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



[ JsonSerializable( typeof(UserData) ) ] public partial class UserDataContext : JsonSerializerContext { }
