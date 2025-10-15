// Jakar.Extensions :: Jakar.Database
// 02/19/2023  7:25 PM

using System;
using ZLinq;



namespace Jakar.Extensions;


public interface IUserRights<out TValue, TEnum> : IUserRights
    where TEnum : struct, Enum
    where TValue : IUserRights<TValue, TEnum>
{
    public TValue WithRights( Permissions<TEnum> rights );
}



public interface IUserRights
{
    public UserRights Rights { get; set; }

    static void Main()
    {
        Permissions<FileRight> rights = Permissions<FileRight>.Default.Add(FileRight.Read)
                                                              .Add(FileRight.Delete);

        foreach ( ref readonly Right<FileRight> r in rights.Rights ) { Console.WriteLine($"{r.Index}: {( r.Value ? "✔" : "✖" )}"); }

        Console.WriteLine();
        Console.WriteLine(rights.ToString());
        Console.WriteLine();

        // Output:
        // Read: ✔
        // Write: ✖
        // Execute: ✖
        // Delete: ✔

        rights.Dispose();
    }
}



internal enum FileRight : ulong
{
    Read    = 0,
    Write   = 1,
    Execute = 2,
    Delete  = 63
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
