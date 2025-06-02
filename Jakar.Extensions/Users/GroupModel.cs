// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  16:57

namespace Jakar.Extensions;


public interface IGroupModel<TID> : IUniqueID<TID>, ICreatedByUser<TID>, IUserRights, JsonModels.IJsonModel
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    [StringLength( UNICODE_CAPACITY )] string NameOfGroup { get; }
    TID?                                      OwnerID     { get; }
}



public interface IGroupModel<TClass, TID> : IGroupModel<TID>, IEqualComparable<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : class, IGroupModel<TClass, TID>
{
    public abstract static TClass Create( IGroupModel<TID> model );
}



[Serializable]
[method: JsonConstructor]
public class GroupModel<TClass, TID>( string nameOfGroup, TID? ownerID, TID? createdBy, TID id, string rights ) : ObservableClass<TClass, TID>( id ), IGroupModel<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : GroupModel<TClass, TID>, IGroupModel<TClass, TID>, IEqualComparable<TClass>

{
    private string _nameOfGroup = nameOfGroup;
    private string _permissions = rights;
    private TID?   _createdBy   = createdBy;
    private TID?   _ownerID     = ownerID;


    [JsonExtensionData] public                    IDictionary<string, JToken?>? AdditionalData { get;                 set; }
    public                                        TID?                          CreatedBy      { get => _createdBy;   set => SetProperty( ref _createdBy,   value ); }
    [StringLength( UNICODE_CAPACITY )] public     string                        NameOfGroup    { get => _nameOfGroup; set => SetProperty( ref _nameOfGroup, value ); }
    public                                        TID?                          OwnerID        { get => _ownerID;     set => SetProperty( ref _ownerID,     value ); }
    [StringLength( IUserRights.MAX_SIZE )] public string                        Rights         { get => _permissions; set => SetProperty( ref _permissions, value ); }


    public GroupModel( IGroupModel<TID> model ) : this( model.NameOfGroup, model.OwnerID, model.CreatedBy, model.ID, model.Rights ) { }


    public override int CompareTo( TClass? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int nameComparison = string.Compare( NameOfGroup, other.NameOfGroup, StringComparison.Ordinal );
        if ( nameComparison != 0 ) { return nameComparison; }

        return ID.CompareTo( other.ID );
    }
    public override bool Equals( TClass? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( NameOfGroup, other.NameOfGroup, StringComparison.Ordinal ) && ID.Equals( other.ID );
    }
}



[Serializable]
[method: JsonConstructor]
public class GroupModel<TID>( string nameOfGroup, TID? ownerID, TID? createdBy, TID id, string rights ) : GroupModel<GroupModel<TID>, TID>( nameOfGroup, ownerID, createdBy, id, rights ), IGroupModel<GroupModel<TID>, TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public GroupModel( IGroupModel<TID>                    model ) : this( model.NameOfGroup, model.OwnerID, model.CreatedBy, model.ID, model.Rights ) { }
    public static GroupModel<TID> Create( IGroupModel<TID> model ) => new(model);


    public override bool Equals( object? other )                                      => other is GroupModel<TID> x && Equals( x );
    public override int  GetHashCode()                                                => HashCode.Combine( NameOfGroup, ID, Rights );
    public static   bool operator ==( GroupModel<TID>? left, GroupModel<TID>? right ) => Equalizer.Equals( left, right );
    public static   bool operator !=( GroupModel<TID>? left, GroupModel<TID>? right ) => Equalizer.Equals( left, right ) is false;
    public static   bool operator >( GroupModel<TID>   left, GroupModel<TID>  right ) => Sorter.GreaterThan( left, right );
    public static   bool operator >=( GroupModel<TID>  left, GroupModel<TID>  right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static   bool operator <( GroupModel<TID>   left, GroupModel<TID>  right ) => Sorter.LessThan( left, right );
    public static   bool operator <=( GroupModel<TID>  left, GroupModel<TID>  right ) => Sorter.LessThanOrEqualTo( left, right );
}
