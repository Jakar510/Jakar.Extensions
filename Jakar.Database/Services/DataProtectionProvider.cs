// Jakar.Extensions :: Jakar.Database
// 1/8/2024  21:46

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using IDataProtector = Jakar.Extensions.IDataProtector;
using IProtectedData = Microsoft.AspNetCore.DataProtection.IDataProtector;



namespace Jakar.Database;


public sealed class ProtectedDataProvider : IDataProtectionProvider
{
    public static void Register( IServiceCollection builder ) => builder.TryAddSingleton<IDataProtectionProvider, ProtectedDataProvider>();
    public IProtectedData CreateProtector( string purpose )
    {
        var data = new ProtectedData( Database.DataProtector );
        return data;
    }
}



public sealed class ProtectedData( IDataProtector protector ) : IProtectedData
{
    private readonly IDataProtector _protector = protector;
    public           IProtectedData CreateProtector( string purpose )       => this;
    public           byte[]         Protect( byte[]         plaintext )     => _protector.Encrypt( plaintext );
    public           byte[]         Unprotect( byte[]       protectedData ) => _protector.Decrypt( protectedData );
}
