﻿// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  16:57

namespace Jakar.Extensions;


public interface IGroupModel<TID> : IUniqueID<TID>, ICreatedByUser<TID>, IUserRights
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    [StringLength( BaseRecord.UNICODE_CAPACITY )] string NameOfGroup { get; }
    TID?                                                 OwnerID     { get; }
}



public interface IGroupModel<out T, TID> : IGroupModel<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where T : IGroupModel<T, TID>
{
    public abstract static T Create( IGroupModel<TID> model );
}



[Serializable]
[method: JsonConstructor]
public record GroupModel<TRecord, TID>( string NameOfGroup, TID? OwnerID, TID? CreatedBy, TID ID, string Rights ) : JsonModelRecord<TRecord, TID>( ID ), IGroupModel<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
    where TRecord : GroupModel<TRecord, TID>, IGroupModel<TRecord, TID>

{
    private string _nameOfGroup = NameOfGroup;
    private string _permissions = Rights;
    private TID?   _createdBy   = CreatedBy;
    private TID?   _ownerID     = OwnerID;


    public                                        TID?   CreatedBy   { get => _createdBy;   set => SetProperty( ref _createdBy,   value ); }
    [StringLength( UNICODE_CAPACITY )] public     string NameOfGroup { get => _nameOfGroup; set => SetProperty( ref _nameOfGroup, value ); }
    public                                        TID?   OwnerID     { get => _ownerID;     set => SetProperty( ref _ownerID,     value ); }
    [StringLength( IUserRights.MAX_SIZE )] public string Rights      { get => _permissions; set => SetProperty( ref _permissions, value ); }


    public GroupModel( IGroupModel<TID> model ) : this( model.NameOfGroup, model.OwnerID, model.CreatedBy, model.ID, model.Rights ) { }


    public override int CompareTo( TRecord? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int nameComparison = string.Compare( NameOfGroup, other.NameOfGroup, StringComparison.Ordinal );
        if ( nameComparison != 0 ) { return nameComparison; }

        return ID.CompareTo( other.ID );
    }
    public override bool Equals( TRecord? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return string.Equals( NameOfGroup, other.NameOfGroup, StringComparison.Ordinal ) && ID.Equals( other.ID );
    }
}



[Serializable]
[method: JsonConstructor]
public record GroupModel<TID>( string NameOfGroup, TID? OwnerID, TID? CreatedBy, TID ID, string Rights ) : GroupModel<GroupModel<TID>, TID>( NameOfGroup, OwnerID, CreatedBy, ID, Rights ), IGroupModel<GroupModel<TID>, TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public GroupModel( IGroupModel<TID>                    model ) : this( model.NameOfGroup, model.OwnerID, model.CreatedBy, model.ID, model.Rights ) { }
    public static GroupModel<TID> Create( IGroupModel<TID> model ) => new(model);
}
