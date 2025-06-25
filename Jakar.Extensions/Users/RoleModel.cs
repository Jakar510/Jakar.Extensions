// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  17:12

namespace Jakar.Extensions;


public interface IRoleModel<out TID> : IUniqueID<TID>, IUserRights, JsonModels.IJsonModel
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    [StringLength( UNICODE_CAPACITY )] public string NameOfRole { get; }
}



public interface IRoleModel<TClass, TID> : IRoleModel<TID>, IEqualComparable<TClass>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : class, IRoleModel<TClass, TID>
{
    public abstract static TClass Create( IRoleModel<TID> model );
}



[Serializable]
[method: JsonConstructor]
public class RoleModel<TClass, TID>( string nameOfRole, string rights, TID id ) : ObservableClass<TClass, TID>( id ), IRoleModel<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TClass : RoleModel<TClass, TID>, IRoleModel<TClass, TID>, IEqualComparable<TClass>
{
    private string _name   = nameOfRole;
    private string _rights = rights;


    [JsonExtensionData]                    public IDictionary<string, JToken?>? AdditionalData { get;            set; }
    [StringLength( UNICODE_CAPACITY )]     public string                        NameOfRole     { get => _name;   set => SetProperty( ref _name,   value ); }
    [StringLength( IUserRights.MAX_SIZE )] public string                        Rights         { get => _rights; set => SetProperty( ref _rights, value ); }


    public RoleModel( IRoleModel<TID> model ) : this( model.NameOfRole, model.Rights, model.ID ) { }


    public override int CompareTo( TClass? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int nameComparison = string.Compare( NameOfRole, other.NameOfRole, StringComparison.Ordinal );
        if ( nameComparison != 0 ) { return nameComparison; }

        return ID.CompareTo( other.ID );
    }
    public override bool Equals( TClass? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( NameOfRole, other.NameOfRole ) && ID.Equals( other.ID );
    }
}



[Serializable]
[method: JsonConstructor]
public sealed class RoleModel<TID>( string nameOfRole, string rights, TID id ) : RoleModel<RoleModel<TID>, TID>( nameOfRole, rights, id ), IRoleModel<RoleModel<TID>, TID> 
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public RoleModel( IRoleModel<TID>                    model ) : this( model.NameOfRole, model.Rights, model.ID ) { }
    public static RoleModel<TID> Create( IRoleModel<TID> model ) => new(model);

    
    public override bool Equals( object?          other )                 => other is RoleModel<TID> x && Equals( x );
    public override int  GetHashCode()                                    => HashCode.Combine( NameOfRole, ID, Rights );
    public static   bool operator ==( RoleModel<TID>? left, RoleModel<TID>? right ) =>  Sorter.Equals( left, right );
    public static   bool operator !=( RoleModel<TID>? left, RoleModel<TID>? right ) =>  Sorter.DoesNotEqual( left, right );
    public static   bool operator >( RoleModel<TID>   left, RoleModel<TID>  right ) => Sorter.GreaterThan( left, right );
    public static   bool operator >=( RoleModel<TID>  left, RoleModel<TID>  right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static   bool operator <( RoleModel<TID>   left, RoleModel<TID>  right ) => Sorter.LessThan( left, right );
    public static   bool operator <=( RoleModel<TID>  left, RoleModel<TID>  right ) => Sorter.LessThanOrEqualTo( left, right );
}
