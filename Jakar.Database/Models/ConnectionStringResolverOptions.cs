// Jakar.Extensions :: Jakar.Database
// 1/20/2024  21:17

namespace Jakar.Database;


[ GenerateOneOf ]
public sealed partial class ConnectionStringResolverOptions : OneOfBase<SecuredString, Func<SecuredString>, Func<CancellationToken, Task<SecuredString>>, Func<CancellationToken, ValueTask<SecuredString>>, Func<IConfiguration, SecuredString>, Func<IConfiguration, CancellationToken, SecuredString>, Func<IConfiguration, CancellationToken, ValueTask<SecuredString>>,
    Func<IConfiguration, CancellationToken, Task<SecuredString>>>
{
    public async ValueTask<SecuredString> GetConnectionStringAsync( IConfiguration configuration, string key, CancellationToken token )
    {
        if ( IsT0 ) { return AsT0; }

        if ( IsT1 ) { return AsT1(); }

        if ( IsT2 ) { return await AsT2( token ); }

        if ( IsT3 ) { return await AsT3( token ); }

        if ( IsT4 ) { return AsT4( configuration ); }

        if ( IsT5 ) { return AsT5( configuration, token ); }

        if ( IsT6 ) { return await AsT6( configuration, token ); }

        if ( IsT7 ) { return await AsT7( configuration, token ); }

        return configuration.GetConnectionString( key ) ?? throw new KeyNotFoundException( key );
    }
}
