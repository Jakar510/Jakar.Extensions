// Jakar.Extensions :: Jakar.Database
// 02/19/2023  7:25 PM

using System;
using ZLinq;



namespace Jakar.Extensions;


public interface IUserRights<out TValue, TEnum> : IUserRights
    where TEnum : unmanaged, Enum
    where TValue : IUserRights<TValue, TEnum>
{
    public TValue WithRights( Permissions<TEnum> rights );
}



public interface IUserRights
{
    public UserRights Rights { get; set; }
}



public class UserRights : BaseClass, IEqualComparable<UserRights>
{
    protected                             string __rights = string.Empty;
    [StringLength(RIGHTS)] public virtual string Value { get => __rights; set => SetProperty(ref __rights, value); }

    public UserRights() { }
    public static implicit operator UserRights( string rights ) => new() { Value = rights };


    public override string ToString() => Value;
    public static UserRights Create<TEnum>( string rights )
        where TEnum : unmanaged, Enum
    {
        using Permissions<TEnum> value = Permissions<TEnum>.Create(null, rights);
        return new UserRights { Value = value.ToString() };
    }
    public static UserRights Create<TEnum>( scoped ref readonly Permissions<TEnum> rights )
        where TEnum : unmanaged, Enum
    {
        return new UserRights { Value = rights.ToString() };
    }


    public virtual void SetRights<TEnum>( scoped ref readonly Permissions<TEnum> permissions )
        where TEnum : unmanaged, Enum => Value = permissions.ToString();
    public void SetRights<TEnum>( params ReadOnlySpan<TEnum> values )
        where TEnum : unmanaged, Enum
    {
        using var permissions = Edit<TEnum>();
        permissions.Add(values);
        Value = permissions.ToString();
    }


    [MustDisposeResource] public virtual Permissions<TEnum> Edit<TEnum>()
        where TEnum : unmanaged, Enum => Permissions<TEnum>.Create(this);


    public int CompareTo( object? other ) => other is UserRights rights
                                                 ? CompareTo(rights)
                                                 : throw new ExpectedValueTypeException(other, typeof(UserRights));
    public int CompareTo( UserRights? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( other is null ) { return 1; }

        return string.Compare(__rights, other.__rights, StringComparison.Ordinal);
    }
    public bool Equals( UserRights? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return string.Equals(__rights, other.__rights, StringComparison.Ordinal);
    }
    public override bool Equals( object? obj )                              => ReferenceEquals(this, obj) || obj is UserRights other && Equals(other);
    public override int  GetHashCode()                                      => __rights.GetHashCode();
    public static   bool operator <( UserRights?  left, UserRights? right ) => Comparer<UserRights>.Default.Compare(left, right) < 0;
    public static   bool operator >( UserRights?  left, UserRights? right ) => Comparer<UserRights>.Default.Compare(left, right) > 0;
    public static   bool operator <=( UserRights? left, UserRights? right ) => Comparer<UserRights>.Default.Compare(left, right) <= 0;
    public static   bool operator >=( UserRights? left, UserRights? right ) => Comparer<UserRights>.Default.Compare(left, right) >= 0;
    public static   bool operator ==( UserRights? left, UserRights? right ) => Equals(left, right);
    public static   bool operator !=( UserRights? left, UserRights? right ) => !Equals(left, right);
}
