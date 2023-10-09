// Jakar.Extensions :: Jakar.Database
// 10/08/2023  10:41 PM

using System.Security;



namespace Jakar.Database;


public sealed record SecuredString( SecureString Value ) : IDisposable
{
    public static implicit operator string( SecuredString               wrapper ) => wrapper.ToString();
    public static implicit operator SecuredString( string               value )   => new(value.ToSecureString());
    public static implicit operator SecuredString( ReadOnlySpan<byte>   value )   => new(value.ToSecureString());
    public static implicit operator SecuredString( ReadOnlySpan<char>   value )   => new(value.ToSecureString());
    public static implicit operator SecuredString( ReadOnlyMemory<char> value )   => new(value.ToSecureString());
    public static implicit operator SecuredString( Memory<char>         value )   => new(value.ToSecureString());

    public override string ToString() => Value.GetValue();
    public          void   Dispose()  => Value.Dispose();
}
