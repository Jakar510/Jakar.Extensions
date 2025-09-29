// Jakar.Extensions :: Jakar.Extensions
// 10/10/2023  10:04 AM

using Microsoft.Extensions.Configuration;



namespace Jakar.Extensions;


public sealed record SecuredString( SecureString Value ) : IDisposable
{
    public static implicit operator string( SecuredString               wrapper ) => wrapper.ToString();
    public static implicit operator ReadOnlySpan<char>( SecuredString   wrapper ) => wrapper.ToString();
    public static implicit operator SecuredString( string               value )   => new(value.ToSecureString());
    public static implicit operator SecuredString( ReadOnlySpan<byte>   value )   => new(value.ToSecureString());
    public static implicit operator SecuredString( ReadOnlySpan<char>   value )   => new(value.ToSecureString());
    public static implicit operator SecuredString( ReadOnlyMemory<char> value )   => new(value.ToSecureString());
    public static implicit operator SecuredString( Memory<char>         value )   => new(value.ToSecureString());


    public override string ToString() => Value.GetValue();
    public          void   Dispose()  => Value.Dispose();



    public readonly struct ResolverOptions
    {
        private readonly Func<CancellationToken, Task<SecuredString>>?                      __value0 = null;
        private readonly Func<CancellationToken, ValueTask<SecuredString>>?                 __value1 = null;
        private readonly Func<IConfiguration, CancellationToken, Task<SecuredString>>?      __value2 = null;
        private readonly Func<IConfiguration, CancellationToken, ValueTask<SecuredString>>? __value3 = null;
        private readonly Func<IConfiguration, SecuredString>?                               __value4 = null;
        private readonly Func<SecuredString>?                                               __value5 = null;
        private readonly SecuredString?                                                     __value6 = null;


        public ResolverOptions( SecuredString                                                     value ) => __value6 = value;
        public ResolverOptions( Func<SecuredString>                                               value ) => __value5 = value;
        public ResolverOptions( Func<CancellationToken, Task<SecuredString>>                      value ) => __value0 = value;
        public ResolverOptions( Func<CancellationToken, ValueTask<SecuredString>>                 value ) => __value1 = value;
        public ResolverOptions( Func<IConfiguration, SecuredString>                               value ) => __value4 = value;
        public ResolverOptions( Func<IConfiguration, CancellationToken, Task<SecuredString>>      value ) => __value2 = value;
        public ResolverOptions( Func<IConfiguration, CancellationToken, ValueTask<SecuredString>> value ) => __value3 = value;


        public static implicit operator ResolverOptions( string                                                            value ) => new(value);
        public static implicit operator ResolverOptions( SecuredString                                                     value ) => new(value);
        public static implicit operator ResolverOptions( Func<SecuredString>                                               value ) => new(value);
        public static implicit operator ResolverOptions( Func<CancellationToken, Task<SecuredString>>                      value ) => new(value);
        public static implicit operator ResolverOptions( Func<CancellationToken, ValueTask<SecuredString>>                 value ) => new(value);
        public static implicit operator ResolverOptions( Func<IConfiguration, SecuredString>                               value ) => new(value);
        public static implicit operator ResolverOptions( Func<IConfiguration, CancellationToken, Task<SecuredString>>      value ) => new(value);
        public static implicit operator ResolverOptions( Func<IConfiguration, CancellationToken, ValueTask<SecuredString>> value ) => new(value);


        [RequiresUnreferencedCode( "Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)" )]
        public async ValueTask<SecuredString> GetSecuredStringAsync( IConfiguration configuration, CancellationToken token = default, string key = "Default", string section = "ConnectionStrings" )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
            if ( __value0 is not null ) { return await __value0( token ); }

            if ( __value1 is not null ) { return await __value1( token ); }

            if ( __value2 is not null ) { return await __value2( configuration, token ); }

            if ( __value3 is not null ) { return await __value3( configuration, token ); }

            if ( __value4 is not null ) { return __value4( configuration ); }

            if ( __value5 is not null ) { return __value5(); }

            if ( __value6 is not null ) { return __value6; }

            return configuration.GetSection( section ).GetValue<string?>( key ) ?? throw new KeyNotFoundException( key );
        }
    }
}
