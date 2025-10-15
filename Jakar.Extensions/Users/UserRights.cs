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



public class UserRights : BaseClass
{
    protected                             string __rights = string.Empty;
    [StringLength(RIGHTS)] public virtual string Value { get => __rights; set => SetProperty(ref __rights, value); }

    public UserRights() { }
    public override string ToString() => Value;


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
        where TEnum : unmanaged, Enum => new(this);
}
