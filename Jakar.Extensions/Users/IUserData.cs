// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  15:2


namespace Jakar.Extensions;


public interface IUserID<out TID>
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
{
    public TID UserID { get; }
}



public interface IUserID : IUserID<Guid>;



public interface IUserData : IUserName, IUserID, IUserRights, IValidator, JsonModels.IJsonModel, INotifyPropertyChanged, INotifyPropertyChanging
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

    #if NET6_0_OR_GREATER
        if ( department.IsNullOrWhiteSpace() is false && title.IsNullOrWhiteSpace() is false && company.IsNullOrWhiteSpace() ) { return $"{department}, {title}"; }

        if ( department.IsNullOrWhiteSpace() is false && title.IsNullOrWhiteSpace() && company.IsNullOrWhiteSpace() is false ) { return $"{department} at {company}"; }

        if ( department.IsNullOrWhiteSpace() && title.IsNullOrWhiteSpace() is false && company.IsNullOrWhiteSpace() is false ) { return $"{title} at {company}"; }

        return $"{department}, {title} at {company}";
    #else
        if ( department.IsNullOrWhiteSpace() is false && title.IsNullOrWhiteSpace() is false && company.IsNullOrWhiteSpace() ) { return $"{department.ToString()}, {title.ToString()}"; }

        if ( department.IsNullOrWhiteSpace() is false && title.IsNullOrWhiteSpace() && company.IsNullOrWhiteSpace() is false ) { return $"{department.ToString()} at {company.ToString()}"; }

        if ( department.IsNullOrWhiteSpace() && title.IsNullOrWhiteSpace() is false && company.IsNullOrWhiteSpace() is false ) { return $"{title.ToString()} at {company.ToString()}"; }

        return $"{department.ToString()}, {title.ToString()} at {company.ToString()}";
    #endif
    }
}



public interface ICreatedByUser<TID>
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
{
    public TID? CreatedBy { get; }
}



public interface IEscalateToUser<TID>
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
{
    public TID? EscalateTo { get; }
}



public interface IImageID<TID>
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
{
    public TID? ImageID { get; }
}



public interface IUserData<TID> : IUserData, IImageID<TID>, IUniqueID<TID>, ICreatedByUser<TID>, IEscalateToUser<TID>
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
{
    public void   With( IUserData<TID> value );
    public string GetDescription();
}



public interface IUserData<TID, TAddress> : IUserData<TID>
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
    where TAddress : IAddress<TID>
{
    public ObservableCollection<TAddress> Addresses { get; }
}



public interface IUserData<TID, TAddress, TGroupModel, TRoleModel> : IUserData<TID, TAddress>
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
    where TGroupModel : IGroupModel<TID>
    where TRoleModel : IRoleModel<TID>
    where TAddress : IAddress<TID>
{
    public ObservableCollection<TGroupModel> Groups { get; }
    public ObservableCollection<TRoleModel>  Roles  { get; }
}



#if NET8_0_OR_GREATER
public interface ICreateUserModel<TClass, TID, TAddress, TGroupModel, TRoleModel> : IUserData<TID, TAddress, TGroupModel, TRoleModel>, ICreateUserModel<TClass, TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TGroupModel : IGroupModel<TID>
    where TRoleModel : IRoleModel<TID>
    where TAddress : IAddress<TID>
    where TClass : ICreateUserModel<TClass, TID, TAddress, TGroupModel, TRoleModel>
{
    public     TClass With( IEnumerable<TAddress>               values );
    public     TClass With( scoped in ReadOnlySpan<TAddress>    values );
    public     TClass With( IEnumerable<TGroupModel>            values );
    public     TClass With( scoped in ReadOnlySpan<TGroupModel> values );
    public     TClass With( IEnumerable<TRoleModel>             values );
    public     TClass With( scoped in ReadOnlySpan<TRoleModel>  values );
    public     TClass With( IDictionary<string, JToken?>?       data );
    public new TClass With( IUserData<TID>                      value );


    public abstract static TClass            Create( IUserData<TID>      model, IEnumerable<TAddress>            addresses, IEnumerable<TGroupModel>            groups, IEnumerable<TRoleModel>            roles );
    public abstract static TClass            Create( IUserData<TID>      model, scoped in ReadOnlySpan<TAddress> addresses, scoped in ReadOnlySpan<TGroupModel> groups, scoped in ReadOnlySpan<TRoleModel> roles );
    public abstract static ValueTask<TClass> CreateAsync( IUserData<TID> model, IAsyncEnumerable<TAddress>       addresses, IAsyncEnumerable<TGroupModel>       groups, IAsyncEnumerable<TRoleModel>       roles, CancellationToken token = default );
}



public interface ICreateUserModel<out TClass, TID> : IUserData<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : ICreateUserModel<TClass, TID>
{
    public abstract static TClass Create( IUserData<TID> model );
}
#endif



public interface IIsVendor
{
    public bool IsVendor { get; set; }
}



public interface IUserDetailsModel<TID> : IUserData<TID>
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
{
    public int?            BadLogins              { get; set; }
    public DateTimeOffset  DateCreated            { get; }
    public bool            IsDisabled             { get; set; }
    public bool            IsEmailConfirmed       { get; set; }
    public bool            IsLocked               { get; set; }
    public bool            IsPhoneNumberConfirmed { get; set; }
    public bool            IsTwoFactorEnabled     { get; set; }
    public DateTimeOffset? LastActive             { get; set; }
    public DateTimeOffset? LastBadAttempt         { get; set; }
    public DateTimeOffset? LockDate               { get; set; }
}



public interface IUserRecord<TID> : IUserDetailsModel<TID>
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
{
    /// <summary> A random value that must change whenever a user is persisted to the store </summary>
    public string ConcurrencyStamp { get; set; }

    public bool            IsActive       { get; set; }
    public DateTimeOffset? LastModified   { get; set; }
    public string          PasswordHash   { get; set; }
    public Guid?           SubscriptionID { get; set; }
}
