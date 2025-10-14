// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  16:57

namespace Jakar.Extensions;


public interface IGroupModel<TID> : IUniqueID<TID>, ICreatedByUser<TID>, IUserRights, IJsonModel
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    [StringLength(NAME)] string NameOfGroup { get; }
    TID?                        OwnerID     { get; }
}



public interface IGroupModel<TSelf, TID> : IGroupModel<TID>, IJsonModel<TSelf>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TSelf : class, IGroupModel<TSelf, TID>
{
    public abstract static TSelf Create( IGroupModel<TID> model );
}



[Serializable]
[method: JsonConstructor]
public class GroupModel<TSelf, TID>( string nameOfGroup, TID? ownerID, TID? createdBy, TID id, string rights ) : BaseClass<TSelf>, IGroupModel<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TSelf : GroupModel<TSelf, TID>, IGroupModel<TSelf, TID>, IEqualComparable<TSelf>, IJsonModel<TSelf>
{
    private   string __nameOfGroup = nameOfGroup;
    private   string __permissions = rights;
    private   TID?   __createdBy   = createdBy;
    private   TID?   __ownerID     = ownerID;
    protected TID    _id           = id;


    public                        TID    ID          { get => _id;           init => _id = value; }
    public                        TID?   CreatedBy   { get => __createdBy;   set => SetProperty(ref __createdBy,   value); }
    [StringLength(NAME)] public   string NameOfGroup { get => __nameOfGroup; set => SetProperty(ref __nameOfGroup, value); }
    public                        TID?   OwnerID     { get => __ownerID;     set => SetProperty(ref __ownerID,     value); }
    [StringLength(RIGHTS)] public string Rights      { get => __permissions; set => SetProperty(ref __permissions, value); }


    public GroupModel( IGroupModel<TID> model ) : this(model.NameOfGroup, model.OwnerID, model.CreatedBy, model.ID, model.Rights) { }


    public override int CompareTo( TSelf? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int nameComparison = string.Compare(NameOfGroup, other.NameOfGroup, StringComparison.Ordinal);
        if ( nameComparison != 0 ) { return nameComparison; }

        return ID.CompareTo(other.ID);
    }
    public override bool Equals( TSelf? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return string.Equals(NameOfGroup, other.NameOfGroup, StringComparison.Ordinal) && ID.Equals(other.ID);
    }
}
