// Jakar.Extensions :: Jakar.Database
// 1/20/2024  21:17

namespace Jakar.Database;


public readonly struct SecuredStringResolverOptions
{
    public const     string                                                             DEFAULT_SQL_CONNECTION_STRING_KEY         = "DEFAULT";
    public const     string                                                             DEFAULT_SQL_CONNECTION_STRING_SECTION_KEY = "ConnectionStrings";
    private readonly Func<CancellationToken, Task<SecuredString>>?                      __value0                                  = null;
    private readonly Func<CancellationToken, ValueTask<SecuredString>>?                 __value1                                  = null;
    private readonly Func<IConfiguration, CancellationToken, Task<SecuredString>>?      __value2                                  = null;
    private readonly Func<IConfiguration, CancellationToken, ValueTask<SecuredString>>? __value3                                  = null;
    private readonly Func<IConfiguration, SecuredString>?                               __value4                                  = null;
    private readonly Func<SecuredString>?                                               __value5                                  = null;
    private readonly SecuredString?                                                     __value6                                  = null;


    public SecuredStringResolverOptions( SecuredString                                                     value ) => __value6 = value;
    public SecuredStringResolverOptions( Func<SecuredString>                                               value ) => __value5 = value;
    public SecuredStringResolverOptions( Func<CancellationToken, Task<SecuredString>>                      value ) => __value0 = value;
    public SecuredStringResolverOptions( Func<CancellationToken, ValueTask<SecuredString>>                 value ) => __value1 = value;
    public SecuredStringResolverOptions( Func<IConfiguration, SecuredString>                               value ) => __value4 = value;
    public SecuredStringResolverOptions( Func<IConfiguration, CancellationToken, Task<SecuredString>>      value ) => __value2 = value;
    public SecuredStringResolverOptions( Func<IConfiguration, CancellationToken, ValueTask<SecuredString>> value ) => __value3 = value;


    public static implicit operator SecuredStringResolverOptions( string                                                            value ) => new(value);
    public static implicit operator SecuredStringResolverOptions( SecuredString                                                     value ) => new(value);
    public static implicit operator SecuredStringResolverOptions( Func<SecuredString>                                               value ) => new(value);
    public static implicit operator SecuredStringResolverOptions( Func<CancellationToken, Task<SecuredString>>                      value ) => new(value);
    public static implicit operator SecuredStringResolverOptions( Func<CancellationToken, ValueTask<SecuredString>>                 value ) => new(value);
    public static implicit operator SecuredStringResolverOptions( Func<IConfiguration, SecuredString>                               value ) => new(value);
    public static implicit operator SecuredStringResolverOptions( Func<IConfiguration, CancellationToken, Task<SecuredString>>      value ) => new(value);
    public static implicit operator SecuredStringResolverOptions( Func<IConfiguration, CancellationToken, ValueTask<SecuredString>> value ) => new(value);


    public static SecuredString GetSecuredString( IConfiguration configuration, string key = DEFAULT_SQL_CONNECTION_STRING_KEY, string section = DEFAULT_SQL_CONNECTION_STRING_SECTION_KEY ) => configuration.GetSection(section)
                                                                                                                                                                                                             .GetValue<string?>(key) ??
                                                                                                                                                                                                throw new KeyNotFoundException(key);
    public async ValueTask<SecuredString> GetSecuredStringAsync( IConfiguration configuration, CancellationToken token, string key = DEFAULT_SQL_CONNECTION_STRING_KEY, string section = DEFAULT_SQL_CONNECTION_STRING_SECTION_KEY )
    {
        if ( __value0 is not null ) { return await __value0(token); }

        if ( __value1 is not null ) { return await __value1(token); }

        if ( __value2 is not null ) { return await __value2(configuration, token); }

        if ( __value3 is not null ) { return await __value3(configuration, token); }

        if ( __value4 is not null ) { return __value4(configuration); }

        if ( __value5 is not null ) { return __value5(); }

        if ( __value6 is not null ) { return __value6; }

        return configuration.GetSection(section)
                            .GetValue<string?>(key) ??
               throw new KeyNotFoundException(key);
    }
}
