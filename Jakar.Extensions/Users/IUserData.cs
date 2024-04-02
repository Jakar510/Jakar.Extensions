// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  15:2


namespace Jakar.Extensions;


public interface ICreatedByUser<TID>
#if NET8_0
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



public interface IUserID<out TID>
#if NET8_0
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



public interface IImageID<TID>
#if NET8_0
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



public interface IUserData : IUserName, IUserID
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
    public                DateTimeOffset?   SubscriptionExpires { get; init; }
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



public interface IUserData<TID> : IUserData, IImageID<TID>, IUniqueID<TID>
#if NET8_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif
{
    public TID? CreatedBy  { get; }
    public TID? EscalateTo { get; }
}



public interface IUserData<TID, TAddress> : IUserData<TID>
#if NET8_0
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



public interface IUserData<TID, TAddress, TGroupModel, TRoleModel> : IUserData<TID, TAddress>, JsonModels.IJsonModel, IUserRights
#if NET8_0
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



public interface ICreateUserModel<TID, TAddress, TGroupModel, TRoleModel> : IUserData<TID, TAddress, TGroupModel, TRoleModel>, IChangePassword
#if NET8_0
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
    where TAddress : IAddress<TID>;
