// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  17:12

namespace Jakar.Extensions;


public interface IRoleModel<out TID> : IUniqueID<TID>, IUserRights, IJsonModel
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    [StringLength(UNICODE_CAPACITY)] public string NameOfRole { get; }
}



public interface IRoleModel<TClass, TID> : IRoleModel<TID>, IEqualComparable<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : class, IRoleModel<TClass, TID>
{
    public abstract static TClass Create( IRoleModel<TID> model );
}



[Serializable]
[method: JsonConstructor]
public class RoleModel<TClass, TID>( string nameOfRole, string rights, TID id ) : BaseClass<TClass, TID>(id), IRoleModel<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : RoleModel<TClass, TID>, IRoleModel<TClass, TID>, IEqualComparable<TClass>, IJsonModel<TClass>
{
    private string __name   = nameOfRole;
    private string __rights = rights;


    [StringLength(UNICODE_CAPACITY)]     public string NameOfRole { get => __name;   set => SetProperty(ref __name,   value); }
    [StringLength(IUserRights.MAX_SIZE)] public string Rights     { get => __rights; set => SetProperty(ref __rights, value); }


    public RoleModel( IRoleModel<TID> model ) : this(model.NameOfRole, model.Rights, model.ID) { }


    public override int CompareTo( TClass? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int nameComparison = string.Compare(NameOfRole, other.NameOfRole, StringComparison.Ordinal);
        if ( nameComparison != 0 ) { return nameComparison; }

        return ID.CompareTo(other.ID);
    }
    public override bool Equals( TClass? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return string.Equals(NameOfRole, other.NameOfRole) && ID.Equals(other.ID);
    }
}
