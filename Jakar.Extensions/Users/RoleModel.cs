// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  17:12

namespace Jakar.Extensions;


public interface IRoleModel<out TID> : IUniqueID<TID>, IUserRights, IJsonModel
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    [StringLength(UNICODE_CAPACITY)] public string NameOfRole { get; }
}



public interface IRoleModel<TSelf, TID> : IRoleModel<TID>, IJsonModel<TSelf>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TSelf : class, IRoleModel<TSelf, TID>
{
    public abstract static TSelf Create( IRoleModel<TID> model );
}



[Serializable]
[method: JsonConstructor]
public class RoleModel<TSelf, TID>( string nameOfRole, string rights, TID id ) : BaseClass<TSelf>(), IRoleModel<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TSelf : RoleModel<TSelf, TID>, IRoleModel<TSelf, TID>, IEqualComparable<TSelf>, IJsonModel<TSelf>
{
    private   string __name   = nameOfRole;
    private   string __rights = rights;
    protected TID    _id      = id;


    public                                      TID    ID         { get => _id;      init => _id = value; }
    [StringLength(UNICODE_CAPACITY)]     public string NameOfRole { get => __name;   set => SetProperty(ref __name,   value); }
    [StringLength(IUserRights.MAX_SIZE)] public string Rights     { get => __rights; set => SetProperty(ref __rights, value); }


    public RoleModel( IRoleModel<TID> model ) : this(model.NameOfRole, model.Rights, model.ID) { }


    public override int CompareTo( TSelf? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int nameComparison = string.Compare(NameOfRole, other.NameOfRole, StringComparison.Ordinal);
        if ( nameComparison != 0 ) { return nameComparison; }

        return ID.CompareTo(other.ID);
    }
    public override bool Equals( TSelf? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return string.Equals(NameOfRole, other.NameOfRole) && ID.Equals(other.ID);
    }
}
