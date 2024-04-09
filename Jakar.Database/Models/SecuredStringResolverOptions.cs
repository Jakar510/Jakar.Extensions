// Jakar.Extensions :: Jakar.Database
// 1/20/2024  21:17

namespace Jakar.Database;


public readonly struct SecuredStringResolverOptions
{
    private readonly Func<CancellationToken, Task<SecuredString>>?                      _value0 = null;
    private readonly Func<CancellationToken, ValueTask<SecuredString>>?                 _value1 = null;
    private readonly Func<IConfiguration, CancellationToken, Task<SecuredString>>?      _value2 = null;
    private readonly Func<IConfiguration, CancellationToken, ValueTask<SecuredString>>? _value3 = null;
    private readonly Func<IConfiguration, SecuredString>?                               _value4 = null;
    private readonly Func<SecuredString>?                                               _value5 = null;
    private readonly SecuredString?                                                     _value6 = null;


    public SecuredStringResolverOptions( SecuredString                                                     value ) => _value6 = value;
    public SecuredStringResolverOptions( Func<SecuredString>                                               value ) => _value5 = value;
    public SecuredStringResolverOptions( Func<CancellationToken, Task<SecuredString>>                      value ) => _value0 = value;
    public SecuredStringResolverOptions( Func<CancellationToken, ValueTask<SecuredString>>                 value ) => _value1 = value;
    public SecuredStringResolverOptions( Func<IConfiguration, SecuredString>                               value ) => _value4 = value;
    public SecuredStringResolverOptions( Func<IConfiguration, CancellationToken, Task<SecuredString>>      value ) => _value2 = value;
    public SecuredStringResolverOptions( Func<IConfiguration, CancellationToken, ValueTask<SecuredString>> value ) => _value3 = value;


    public static implicit operator SecuredStringResolverOptions( string                                                            value ) => new(value);
    public static implicit operator SecuredStringResolverOptions( SecuredString                                                     value ) => new(value);
    public static implicit operator SecuredStringResolverOptions( Func<SecuredString>                                               value ) => new(value);
    public static implicit operator SecuredStringResolverOptions( Func<CancellationToken, Task<SecuredString>>                      value ) => new(value);
    public static implicit operator SecuredStringResolverOptions( Func<CancellationToken, ValueTask<SecuredString>>                 value ) => new(value);
    public static implicit operator SecuredStringResolverOptions( Func<IConfiguration, SecuredString>                               value ) => new(value);
    public static implicit operator SecuredStringResolverOptions( Func<IConfiguration, CancellationToken, Task<SecuredString>>      value ) => new(value);
    public static implicit operator SecuredStringResolverOptions( Func<IConfiguration, CancellationToken, ValueTask<SecuredString>> value ) => new(value);


    public async ValueTask<SecuredString> GetSecuredStringAsync( IConfiguration configuration, CancellationToken token, string key = "Default", string section = "ConnectionStrings" )
    {
        if ( _value0 is not null ) { return await _value0( token ); }

        if ( _value1 is not null ) { return await _value1( token ); }

        if ( _value2 is not null ) { return await _value2( configuration, token ); }

        if ( _value3 is not null ) { return await _value3( configuration, token ); }

        if ( _value4 is not null ) { return _value4( configuration ); }

        if ( _value5 is not null ) { return _value5(); }

        if ( _value6 is not null ) { return _value6; }

        return configuration.GetSection( section ).GetValue<string?>( key ) ?? throw new KeyNotFoundException( key );
    }
}
