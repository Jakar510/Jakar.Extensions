// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  17:12

namespace Jakar.Extensions;


public interface IRoleModel<out TID> : IUniqueID<TID>, IUserRights
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
    [StringLength( BaseRecord.UNICODE_CAPACITY )] public string NameOfRole { get; }
}



#if NET8_0_OR_GREATER
public interface IRoleModel<out T, TID> : IRoleModel<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where T : IRoleModel<T, TID>
{
    public abstract static T Create( IRoleModel<TID> model );
}
#endif



[Serializable]
[method: JsonConstructor]
public record RoleModel<TRecord, TID>( string NameOfRole, string Rights, TID ID ) : ObservableRecord<TRecord, TID>( ID ), IRoleModel<TID>
#if NET8_0_OR_GREATER
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
#elif NET7_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>
#elif NET6_0
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable
#else
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
#endif

#if NET8_0_OR_GREATER
    where TRecord : RoleModel<TRecord, TID>, IRoleModel<TRecord, TID>
#else
    where TRecord : RoleModel<TRecord, TID>
#endif
{
    private string _name   = NameOfRole;
    private string _rights = Rights;


    [StringLength( UNICODE_CAPACITY )] public string NameOfRole { get => _name;   set => SetProperty( ref _name,   value ); }
    [StringLength( IUserRights.MAX_SIZE )]    public string Rights     { get => _rights; set => SetProperty( ref _rights, value ); }


    public RoleModel( IRoleModel<TID> model ) : this( model.NameOfRole, model.Rights, model.ID ) { }


    public override int CompareTo( TRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int nameComparison = string.Compare( NameOfRole, other.NameOfRole, StringComparison.Ordinal );
        if ( nameComparison != 0 ) { return nameComparison; }

        return ID.CompareTo( other.ID );
    }
    public override bool Equals( TRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( NameOfRole, other.NameOfRole ) && ID.Equals( other.ID );
    }
}



[Serializable]
[method: JsonConstructor]
public sealed record RoleModel<TID>( string NameOfRole, string Rights, TID ID ) :
#if NET8_0_OR_GREATER
    RoleModel<RoleModel<TID>, TID>( NameOfRole, Rights, ID ),
    IRoleModel<RoleModel<TID>, TID>
#else
    RoleModel<RoleModel<TID>, TID>( NameOfRole, Rights, ID )
#endif

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
    public RoleModel( IRoleModel<TID>                    model ) : this( model.NameOfRole, model.Rights, model.ID ) { }
    public static RoleModel<TID> Create( IRoleModel<TID> model ) => new(model);
}
