// Jakar.Extensions :: Jakar.Extensions
// 10/10/2023  10:04 AM

namespace Jakar.Extensions;


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
