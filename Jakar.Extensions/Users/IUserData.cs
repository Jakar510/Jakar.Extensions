// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  15:2


namespace Jakar.Extensions;


public interface IUserID
{
    public Guid UserID { get; }
}



public interface IUserData : IUserName, IUserID, IUserRights, IValidator, IJsonModel
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
    public static string GetDescription( IUserData data ) => GetDescription(data.Department, data.Title, data.Company);
    public static string GetDescription( scoped in ReadOnlySpan<char> department, scoped in ReadOnlySpan<char> title, scoped in ReadOnlySpan<char> company )
    {
        if ( department.IsNullOrWhiteSpace() && title.IsNullOrWhiteSpace() && company.IsNullOrWhiteSpace() ) { return EMPTY; }

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



public interface ICreateUserModel<TSelf, TID, TAddress, TGroupModel, TRoleModel> : IUserData<TID, TAddress, TGroupModel, TRoleModel>, ICreateUserModel<TSelf, TID>, IEqualComparable<TSelf>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TGroupModel : IGroupModel<TID>, IEquatable<TGroupModel>
    where TRoleModel : IRoleModel<TID>, IEquatable<TRoleModel>
    where TAddress : IAddress<TID>, IEquatable<TAddress>
    where TSelf : class, ICreateUserModel<TSelf, TID, TAddress, TGroupModel, TRoleModel>
{
    public TSelf With( IEnumerable<TAddress>            values );
    public TSelf With( params ReadOnlySpan<TAddress>    values );
    public TSelf With( IEnumerable<TGroupModel>         values );
    public TSelf With( params ReadOnlySpan<TGroupModel> values );
    public TSelf With( IEnumerable<TRoleModel>          values );
    public TSelf With( params ReadOnlySpan<TRoleModel>  values );
    public TSelf With( JObject?                         data );
    public TSelf With<TValue>( TValue value )
        where TValue : IUserData<TID>;


    public abstract static TSelf            Create( IUserData<TID>      model, IEnumerable<TAddress>            addresses, IEnumerable<TGroupModel>            groups, IEnumerable<TRoleModel>            roles );
    public abstract static TSelf            Create( IUserData<TID>      model, scoped in ReadOnlySpan<TAddress> addresses, scoped in ReadOnlySpan<TGroupModel> groups, scoped in ReadOnlySpan<TRoleModel> roles );
    public abstract static ValueTask<TSelf> CreateAsync( IUserData<TID> model, IAsyncEnumerable<TAddress>       addresses, IAsyncEnumerable<TGroupModel>       groups, IAsyncEnumerable<TRoleModel>       roles, CancellationToken token = default );
}



public interface ICreateUserModel<TSelf, TID> : IUserData<TID>, IJsonModel<TSelf>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TSelf : class, ICreateUserModel<TSelf, TID>
{
    public abstract static TSelf Create( IUserData<TID> model );
}



public interface IIsVendor
{
    public bool IsVendor { get; set; }
}
