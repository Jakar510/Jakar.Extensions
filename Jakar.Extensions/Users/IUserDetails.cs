// Jakar.Extensions :: Jakar.Extensions
// 02/17/2026  14:40

namespace Jakar.Extensions;


public class UserDetails : BaseClass, IUserDetails, IUserID
{
    protected string? _company;
    protected string? _department;
    protected string? _email;
    protected string? _ext;
    protected string? _firstName;
    protected string? _gender;
    protected string? _lastName;
    protected string? _phoneNumber;
    protected string? _title;
    protected string? _website;
    protected string? _description;
    protected string? _fullName;


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
    [EmailAddress] [StringLength(EMAIL)]       public string? Email       { get => _email;                           set => SetProperty(ref _email,      value); }
    [StringLength(               PHONE_EXT)]   public string? Ext         { get => _ext;                             set => SetProperty(ref _ext,        value); }

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

    [StringLength(FULL_NAME)] public         string? FullName           { get => _fullName ??= GetFullName(); set => SetProperty(ref _fullName, value); }
    [StringLength(GENDER)]    public         string? Gender             { get => _gender;                    set => SetProperty(ref _gender,  value); }
    [JsonIgnore]              public virtual bool    IsValid            => IsValidEmail                      && IsValidName;
    [JsonIgnore]              public virtual bool    IsValidEmail       => !string.IsNullOrWhiteSpace(Email) && Email.IsEmailAddress();
    [JsonIgnore]              public virtual bool    IsValidName        => !string.IsNullOrWhiteSpace(FullName);
    [JsonIgnore]              public virtual bool    IsValidPhoneNumber => !string.IsNullOrWhiteSpace(PhoneNumber);
    [JsonIgnore]              public virtual bool    IsValidWebsite     => Uri.TryCreate(Website, UriKind.RelativeOrAbsolute, out _);

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

    [Phone] [StringLength(PHONE)] public string? PhoneNumber { get => _phoneNumber; set => SetProperty(ref _phoneNumber, value); }

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


    [Url] [StringLength(WEBSITE)] public string? Website { get => _website; set => SetProperty(ref _website, value); }


    public UserDetails() { }
    public UserDetails( IUserDetails value )
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
    }


    public virtual string GetFullName()    => IUserDetails.GetFullName(this);
    public virtual string GetDescription() => IUserDetails.GetDescription(this);
}



public interface IUserDetails
{
    public                string? Company     { get; set; }
    public                string? Department  { get; set; }
    public                string? Description { get; set; }
    [EmailAddress] public string? Email       { get; set; }
    public                string? Ext         { get; set; }
    [Required] public     string? FirstName   { get; set; }
    public                string? FullName    { get; set; }
    public                string? Gender      { get; set; }
    [Required] public     string? LastName    { get; set; }
    [Phone]    public     string? PhoneNumber { get; set; }
    public                string? Title       { get; set; }
    [Url] public          string? Website     { get; set; }


    public static string GetFullName( IUserDetails data ) => $"{data.FirstName} {data.LastName}".Trim();


    public static string GetDescription( IUserDetails data ) => GetDescription(data.Department, data.Title, data.Company);
    public static string GetDescription( scoped in ReadOnlySpan<char> department, scoped in ReadOnlySpan<char> title, scoped in ReadOnlySpan<char> company )
    {
        if ( department.IsNullOrWhiteSpace() && title.IsNullOrWhiteSpace() && company.IsNullOrWhiteSpace() ) { return EMPTY; }

        return department.IsNullOrWhiteSpace() switch
               {
                   false when !title.IsNullOrWhiteSpace() && company.IsNullOrWhiteSpace()  => $"{department}, {title}",
                   false when title.IsNullOrWhiteSpace()  && !company.IsNullOrWhiteSpace() => $"{department} at {company}",
                   false when !title.IsNullOrWhiteSpace() && !company.IsNullOrWhiteSpace() => $"{title} at {company}",
                   _                                                                       => $"{department}, {title} at {company}"
               };
    }
}
