// Jakar.Extensions :: Jakar.Database
// 1/8/2024  21:46

namespace Jakar.Database;


public sealed class ProtectedDataProvider : IDataProtectionProvider
{
    private readonly ConcurrentDictionary<string, ProtectedData> _data = new();


    public static void           Register( IServiceCollection builder ) => builder.TryAddSingleton<IDataProtectionProvider, ProtectedDataProvider>();
    public        IProtectedData CreateProtector( string      purpose ) => _data.GetOrAdd(purpose, static ( _, protector ) => new ProtectedData(protector), Database.DataProtector);
}



public sealed class ProtectedData( IDataProtector protector ) : IProtectedData
{
    private readonly IDataProtector __protector = protector;
    public           IProtectedData CreateProtector( string purpose )       => this;
    public           byte[]         Protect( byte[]         plaintext )     => __protector.Encrypt(plaintext);
    public           byte[]         Unprotect( byte[]       protectedData ) => __protector.Decrypt(protectedData);
}
