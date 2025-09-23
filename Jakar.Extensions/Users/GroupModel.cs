// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  16:57

namespace Jakar.Extensions;


public interface IGroupModel<TID> : IUniqueID<TID>, ICreatedByUser<TID>, IUserRights, IJsonModel
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    [StringLength(UNICODE_CAPACITY)] string NameOfGroup { get; }
    TID?                                    OwnerID     { get; }
}



public interface IGroupModel<TClass, TID> : IGroupModel<TID>, IEqualComparable<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : class, IGroupModel<TClass, TID>
{
    public abstract static TClass Create( IGroupModel<TID> model );
}



[Serializable]
[method: JsonConstructor]
public class GroupModel<TClass, TID>( string nameOfGroup, TID? ownerID, TID? createdBy, TID id, string rights ) : ObservableClass<TClass, TID>(id), IGroupModel<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : GroupModel<TClass, TID>, IGroupModel<TClass, TID>, IEqualComparable<TClass>, IJsonModel<TClass>

{
    private string __nameOfGroup = nameOfGroup;
    private string __permissions = rights;
    private TID?   __createdBy   = createdBy;
    private TID?   __ownerID     = ownerID;


    [JsonExtensionData] public                  JsonObject? AdditionalData { get;                  set; }
    public                                      TID?                            CreatedBy      { get => __createdBy;   set => SetProperty(ref __createdBy,   value); }
    [StringLength(UNICODE_CAPACITY)] public     string                          NameOfGroup    { get => __nameOfGroup; set => SetProperty(ref __nameOfGroup, value); }
    public                                      TID?                            OwnerID        { get => __ownerID;     set => SetProperty(ref __ownerID,     value); }
    [StringLength(IUserRights.MAX_SIZE)] public string                          Rights         { get => __permissions; set => SetProperty(ref __permissions, value); }


    public GroupModel( IGroupModel<TID> model ) : this(model.NameOfGroup, model.OwnerID, model.CreatedBy, model.ID, model.Rights) { }


    public override int CompareTo( TClass? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int nameComparison = string.Compare(NameOfGroup, other.NameOfGroup, StringComparison.Ordinal);
        if ( nameComparison != 0 ) { return nameComparison; }

        return ID.CompareTo(other.ID);
    }
    public override bool Equals( TClass? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return string.Equals(NameOfGroup, other.NameOfGroup, StringComparison.Ordinal) && ID.Equals(other.ID);
    }
}
