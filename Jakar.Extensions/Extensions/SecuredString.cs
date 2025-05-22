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
        private readonly Func<CancellationToken, Task<SecuredString>>?                      _value0 = null;
        private readonly Func<CancellationToken, ValueTask<SecuredString>>?                 _value1 = null;
        private readonly Func<IConfiguration, CancellationToken, Task<SecuredString>>?      _value2 = null;
        private readonly Func<IConfiguration, CancellationToken, ValueTask<SecuredString>>? _value3 = null;
        private readonly Func<IConfiguration, SecuredString>?                               _value4 = null;
        private readonly Func<SecuredString>?                                               _value5 = null;
        private readonly SecuredString?                                                     _value6 = null;


        public ResolverOptions( SecuredString                                                     value ) => _value6 = value;
        public ResolverOptions( Func<SecuredString>                                               value ) => _value5 = value;
        public ResolverOptions( Func<CancellationToken, Task<SecuredString>>                      value ) => _value0 = value;
        public ResolverOptions( Func<CancellationToken, ValueTask<SecuredString>>                 value ) => _value1 = value;
        public ResolverOptions( Func<IConfiguration, SecuredString>                               value ) => _value4 = value;
        public ResolverOptions( Func<IConfiguration, CancellationToken, Task<SecuredString>>      value ) => _value2 = value;
        public ResolverOptions( Func<IConfiguration, CancellationToken, ValueTask<SecuredString>> value ) => _value3 = value;


        public static implicit operator ResolverOptions( string                                                            value ) => new(value);
        public static implicit operator ResolverOptions( SecuredString                                                     value ) => new(value);
        public static implicit operator ResolverOptions( Func<SecuredString>                                               value ) => new(value);
        public static implicit operator ResolverOptions( Func<CancellationToken, Task<SecuredString>>                      value ) => new(value);
        public static implicit operator ResolverOptions( Func<CancellationToken, ValueTask<SecuredString>>                 value ) => new(value);
        public static implicit operator ResolverOptions( Func<IConfiguration, SecuredString>                               value ) => new(value);
        public static implicit operator ResolverOptions( Func<IConfiguration, CancellationToken, Task<SecuredString>>      value ) => new(value);
        public static implicit operator ResolverOptions( Func<IConfiguration, CancellationToken, ValueTask<SecuredString>> value ) => new(value);


        [RequiresUnreferencedCode( "Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)" )]
        public async ValueTask<SecuredString> GetSecuredStringAsync( IConfiguration configuration, TelemetrySpan parent = default, CancellationToken token = default, string key = "Default", string section = "ConnectionStrings" )
        {
            using TelemetrySpan span = parent.SubSpan();
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
}
