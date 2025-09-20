// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  15:2


namespace Jakar.Extensions;


public interface IUserID<out TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public TID UserID { get; }
}



public interface IUserID : IUserID<Guid>;



public interface IUserData : IUserName, IUserID, IUserRights, IValidator, Json.IJsonModel
{
    public                string            Company             { get; set; }
    public                string            Department          { get; set; }
    public                string            Description         { get; set; }
    [EmailAddress] public string            Email               { get; set; }
    public                string            Ext                 { get; set; }
    [Required] public     string            FirstName           { get; set; }
    public                string            FullName            { get; set; }
    public                string            Gender              { get; set; }
    [Required] public     string            LastName            { get; set; }
    [Phone]    public     string            PhoneNumber         { get; set; }
    [Required] public     SupportedLanguage PreferredLanguage   { get; set; }
    public                DateTimeOffset?   SubscriptionExpires { get; }
    public                string            Title               { get; set; }
    [Url] public          string            Website             { get; set; }


    public static string GetFullName( IUserData    data ) => $"{data.FirstName} {data.LastName}".Trim();
    public static string GetDescription( IUserData data ) => GetDescription( data.Department, data.Title, data.Company );
    public static string GetDescription( scoped in ReadOnlySpan<char> department, scoped in ReadOnlySpan<char> title, scoped in ReadOnlySpan<char> company )
    {
        if ( department.IsNullOrWhiteSpace() && title.IsNullOrWhiteSpace() && company.IsNullOrWhiteSpace() ) { return string.Empty; }

        if ( !department.IsNullOrWhiteSpace() && !title.IsNullOrWhiteSpace() && company.IsNullOrWhiteSpace() ) { return $"{department}, {title}"; }

        if ( !department.IsNullOrWhiteSpace() && title.IsNullOrWhiteSpace() && !company.IsNullOrWhiteSpace() ) { return $"{department} at {company}"; }

        if ( department.IsNullOrWhiteSpace() && !title.IsNullOrWhiteSpace() && !company.IsNullOrWhiteSpace() ) { return $"{title} at {company}"; }

        return $"{department}, {title} at {company}";
    }
}



public interface ICreatedByUser<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public TID? CreatedBy { get; }
}



public interface IEscalateToUser<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public TID? EscalateTo { get; }
}



public interface IImageID<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public TID? ImageID { get; }
}



public interface IUserData<TID> : IUserData, IImageID<TID>, IUniqueID<TID>, ICreatedByUser<TID>, IEscalateToUser<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public void   With( IUserData<TID> value );
    public string GetDescription();
}



public interface IUserData<TID, TAddress> : IUserData<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TAddress : IAddress<TID>, IEquatable<TAddress>
{
    public ObservableCollection<TAddress> Addresses { get; }
}



public interface IUserData<TID, TAddress, TGroupModel, TRoleModel> : IUserData<TID, TAddress>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TGroupModel : IGroupModel<TID>, IEquatable<TGroupModel>
    where TRoleModel : IRoleModel<TID>, IEquatable<TRoleModel>
    where TAddress : IAddress<TID>, IEquatable<TAddress>
{
    public ObservableCollection<TGroupModel> Groups { get; }
    public ObservableCollection<TRoleModel>  Roles  { get; }
}



public interface ICreateUserModel<TClass, TID, TAddress, TGroupModel, TRoleModel> : IUserData<TID, TAddress, TGroupModel, TRoleModel>, ICreateUserModel<TClass, TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TGroupModel : IGroupModel<TID>, IEquatable<TGroupModel>
    where TRoleModel : IRoleModel<TID>, IEquatable<TRoleModel>
    where TAddress : IAddress<TID>, IEquatable<TAddress>
    where TClass : class, ICreateUserModel<TClass, TID, TAddress, TGroupModel, TRoleModel>
{
    public TClass With( IEnumerable<TAddress>            values );
    public TClass With( params ReadOnlySpan<TAddress>    values );
    public TClass With( IEnumerable<TGroupModel>         values );
    public TClass With( params ReadOnlySpan<TGroupModel> values );
    public TClass With( IEnumerable<TRoleModel>          values );
    public TClass With( params ReadOnlySpan<TRoleModel>  values );
    public TClass With( JsonObject?    data );
    public TClass With<TValue>( TValue value )
        where TValue : IUserData<TID>;


    public abstract static TClass            Create( IUserData<TID>      model, IEnumerable<TAddress>            addresses, IEnumerable<TGroupModel>            groups, IEnumerable<TRoleModel>            roles );
    public abstract static TClass            Create( IUserData<TID>      model, scoped in ReadOnlySpan<TAddress> addresses, scoped in ReadOnlySpan<TGroupModel> groups, scoped in ReadOnlySpan<TRoleModel> roles );
    public abstract static ValueTask<TClass> CreateAsync( IUserData<TID> model, IAsyncEnumerable<TAddress>       addresses, IAsyncEnumerable<TGroupModel>       groups, IAsyncEnumerable<TRoleModel>       roles, CancellationToken token = default );
}



public interface ICreateUserModel<TClass, TID> : IUserData<TID>, IEqualComparable<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : class, ICreateUserModel<TClass, TID>
{
    public abstract static TClass Create( IUserData<TID> model );
}



public interface IIsVendor
{
    public bool IsVendor { get; set; }
}



public interface IUserDetailsModel<TID> : IUserData<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public int?            BadLogins              { get; set; }
    public DateTimeOffset  DateCreated            { get; }
    public bool            IsDisabled             { get; set; }
    public bool            IsEmailConfirmed       { get; set; }
    public bool            IsLocked               { get; set; }
    public bool            IsPhoneNumberConfirmed { get; set; }
    public bool            IsTwoFactorEnabled     { get; set; }
    public DateTimeOffset? LastBadAttempt         { get; set; }
    public DateTimeOffset? LastLogin              { get; set; }
    public DateTimeOffset? LockDate               { get; set; }
}



public interface IUserRecord<TID> : IUserDetailsModel<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    /// <summary> A random value that must change whenever a user is persisted to the store </summary>
    public string ConcurrencyStamp { get; set; }

    public bool            IsActive       { get; set; }
    public DateTimeOffset? LastModified   { get; set; }
    public string          PasswordHash   { get; set; }
    public TID?            SubscriptionID { get; set; }
}



public interface ISessionID<out TID> : IDeviceID
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public TID SessionID { get; }
}
