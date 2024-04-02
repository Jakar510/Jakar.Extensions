﻿// Jakar.Extensions :: Jakar.Extensions
// 4/1/2024  17:12

using System;
using Org.BouncyCastle.Tls;



namespace Jakar.Extensions;


public interface IRoleModel<out TID> : IUniqueID<TID>, IRights
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
{
    [StringLength( BaseRecord.UNICODE_STRING_CAPACITY )] public string NameOfRole { get; }
}



[Serializable]
[method: JsonConstructor]
public record RoleModel<TRecord, TID>( string NameOfRole, string Rights, TID ID ) : ObservableRecord<TRecord, TID>( ID ), IRoleModel<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
    where TRecord : RoleModel<TRecord, TID>
{
    private string _name   = NameOfRole;
    private string _rights = Rights;


    [StringLength( UNICODE_STRING_CAPACITY )] public string NameOfRole { get => _name;   set => SetProperty( ref _name,   value ); }
    [StringLength( IRights.MAX_SIZE )]        public string Rights     { get => _rights; set => SetProperty( ref _rights, value ); }


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
public sealed record RoleModel<TID>( string NameOfRole, string Rights, TID ID ) : RoleModel<RoleModel<TID>, TID>( NameOfRole, Rights, ID )
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable
{
    public RoleModel( IRoleModel<TID> model ) : this( model.NameOfRole, model.Rights, model.ID ) { }
}
