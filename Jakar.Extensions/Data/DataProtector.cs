// Jakar.Extensions :: Jakar.Database
// 04/29/2023  10:47 PM

using Microsoft.Extensions.Configuration;



namespace Jakar.Extensions;


public interface IDataProtectorProvider
{
    public IDataProtector            GetProtector();
    public ValueTask<IDataProtector> GetProtectorAsync();
}



public interface IDataProtector : IDisposable
{
    public bool   TryEncrypt( scoped in ReadOnlySpan<byte> value, scoped ref Span<byte>    destination, out int bytesWritten );
    public byte[] Encrypt( scoped in    ReadOnlySpan<byte> value, in         TelemetrySpan parent                                                       = default );
    public string Encrypt( string                          value, in         TelemetrySpan parent                                                       = default );
    public string Encrypt( string                          value, Encoding                 encoding, in TelemetrySpan parent                            = default );
    public void   Encrypt( LocalFile                       file,  string                   value,    in TelemetrySpan parent                            = default );
    public void   Encrypt( LocalFile                       file,  string                   value,    Encoding         encoding, in TelemetrySpan parent = default );
    public void   Encrypt( LocalFile                       file,  byte[]                   value,    in TelemetrySpan parent = default );


    public ValueTask<byte[]> EncryptAsync( LocalFile value, TelemetrySpan parent = default, CancellationToken token  = default );
    public ValueTask<string> EncryptAsync( LocalFile value, Encoding      encoding,         TelemetrySpan     parent = default, CancellationToken token  = default );
    public ValueTask         DecryptAsync( LocalFile input, LocalFile     output,           Encoding          encoding,         TelemetrySpan     parent = default, CancellationToken token = default );
    public ValueTask         EncryptAsync( LocalFile file,  string        value,            TelemetrySpan     parent = default, CancellationToken token  = default );
    public ValueTask         EncryptAsync( LocalFile file,  string        value,            Encoding          encoding,         TelemetrySpan     parent = default, CancellationToken token = default );
    public ValueTask         EncryptAsync( LocalFile file,  byte[]        value,            TelemetrySpan     parent = default, CancellationToken token  = default );
    public ValueTask         EncryptAsync( LocalFile input, LocalFile     output,           TelemetrySpan     parent = default, CancellationToken token  = default );
    public ValueTask         EncryptAsync( LocalFile input, LocalFile     output,           Encoding          encoding,         TelemetrySpan     parent = default, CancellationToken token = default );


    public bool   TryDecrypt( scoped in ReadOnlySpan<byte> value, scoped ref Span<byte>    destination, out int bytesWritten );
    public byte[] Decrypt( scoped in    ReadOnlySpan<byte> value, in         TelemetrySpan parent                            = default );
    public string Decrypt( string                          value, in         TelemetrySpan parent                            = default );
    public string Decrypt( string                          value, Encoding                 encoding, in TelemetrySpan parent = default );
    public byte[] Decrypt( LocalFile                       file,  in TelemetrySpan         parent                            = default );
    public string Decrypt( LocalFile                       file,  Encoding                 encoding, in TelemetrySpan parent = default );
    public TValue Decrypt<TValue>( LocalFile               file,  Decryptor<TValue>        func,     TelemetrySpan    parent = default );


    public ValueTask<byte[]> DecryptAsync( LocalFile         file,  TelemetrySpan          parent = default, CancellationToken token  = default );
    public ValueTask<string> DecryptAsync( LocalFile         file,  Encoding               encoding,         TelemetrySpan     parent = default, CancellationToken token = default );
    public ValueTask<TValue> DecryptAsync<TValue>( LocalFile file,  DecryptorAsync<TValue> func,             TelemetrySpan     parent = default, CancellationToken token = default );
    public ValueTask         DecryptAsync( LocalFile         input, LocalFile              output,           TelemetrySpan     parent = default, CancellationToken token = default );



    public delegate TValue Decryptor<out TValue>( LocalFile.IReadHandler handler, IDataProtector dataProtector, in TelemetrySpan parent );



    public delegate ValueTask<TValue> DecryptorAsync<TValue>( LocalFile.IAsyncReadHandler handler, IDataProtector dataProtector, TelemetrySpan parent, CancellationToken token );
}



public sealed class DataProtector( RSA rsa, RSAEncryptionPadding padding ) : IDataProtector
{
    private const    int                  BLOCK    = 512;
    private const    int                  DATA     = 254;
    private readonly RSA                  _rsa     = rsa;
    private readonly RSAEncryptionPadding _padding = padding;
    private          bool                 _disposed;
    private          bool                 _keyIsSet;


    public DataProtector( RSAEncryptionPadding padding ) : this( RSA.Create(), padding ) { }
    public void Dispose()
    {
        _rsa.Dispose();
        _disposed = true;
    }


    public DataProtector WithKey( scoped in ReadOnlySpan<char> pem, in TelemetrySpan parent = default )
    {
        if ( _keyIsSet ) { throw new WarningException( $"{nameof(WithKey)} or {nameof(WithKeyAsync)} has already been called" ); }

        using TelemetrySpan span = parent.SubSpan();
        _rsa.ImportFromPem( pem );
        _keyIsSet = true;
        return this;
    }
    public DataProtector WithKey( scoped in ReadOnlySpan<char> pem, scoped in ReadOnlySpan<char> password, in TelemetrySpan parent = default )
    {
        if ( _keyIsSet ) { throw new WarningException( $"{nameof(WithKey)} or {nameof(WithKeyAsync)}  has already been called" ); }

        using TelemetrySpan span = parent.SubSpan();
        _rsa.ImportFromEncryptedPem( pem, password );
        _keyIsSet = true;
        return this;
    }


    public DataProtector WithKey<TValue>( EmbeddedResources<TValue> resources, string name, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return WithKey( resources.GetResourceText( name ), in span );
    }
    public DataProtector WithKey<TValue>( EmbeddedResources<TValue> resources, string name, SecuredString password, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return WithKey( resources.GetResourceText( name ), password, in span );
    }
    public DataProtector WithKey<TValue>( EmbeddedResources<TValue> resources, string name, scoped in ReadOnlySpan<char> password, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return WithKey( resources.GetResourceText( name ), password, in span );
    }
    public async ValueTask<DataProtector> WithKeyAsync<TValue>( EmbeddedResources<TValue> resources, string name, TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return WithKey( await resources.GetResourceTextAsync( name ), in span );
    }
    public async ValueTask<DataProtector> WithKeyAsync<TValue>( EmbeddedResources<TValue> resources, string name, string password, TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return WithKey( await resources.GetResourceTextAsync( name ), password, in span );
    }
    public async ValueTask<DataProtector> WithKeyAsync<TValue>( EmbeddedResources<TValue> resources, string name, SecuredString password, TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return WithKey( await resources.GetResourceTextAsync( name ), password, in span );
    }


    [RequiresUnreferencedCode( "Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)" )]
    public async ValueTask<DataProtector> WithKeyAsync<TValue>( EmbeddedResources<TValue> resources, string name, SecuredString.ResolverOptions password, IConfiguration configuration, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return await WithKeyAsync( resources, name, await password.GetSecuredStringAsync( configuration, span, token ) );
    }


    public DataProtector WithKeyFile( LocalFile pem, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return WithKey( pem.Read().AsString(), in span );
    }
    public DataProtector WithKeyFile( LocalFile pem, scoped in ReadOnlySpan<char> password, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return WithKey( pem.Read().AsString(), password, in span );
    }
    public DataProtector WithKeyFile( LocalFile pem, SecuredString password, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return WithKey( pem.Read().AsString(), password, in span );
    }
    public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return WithKey( await pem.ReadAsync().AsString( span, token ), in span );
    }
    public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, string password, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return WithKey( await pem.ReadAsync().AsString( span, token ), password, in span );
    }
    public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, SecuredString password, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return WithKey( await pem.ReadAsync().AsString( span, token ), password, in span );
    }


    [RequiresUnreferencedCode( "Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)" )]
    public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, SecuredString.ResolverOptions password, IConfiguration configuration, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return await WithKeyAsync( pem, await password.GetSecuredStringAsync( configuration, span, token ), span, token );
    }


    public static byte[] GetBytes( string base64, Encoding encoding, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();

        try { return Convert.FromBase64String( base64 ); }
        catch ( Exception ) { return encoding.GetBytes( base64 ); }
    }
    public static IMemoryOwner<byte> GetBytes( string base64, Encoding encoding, out int bytesWritten, in TelemetrySpan parent = default )
    {
        using TelemetrySpan telemetrySpan = parent.SubSpan();
        int                 count         = Encoding.Default.GetByteCount( base64 );
        IMemoryOwner<byte>  owner         = MemoryPool<byte>.Shared.Rent( count );
        Span<byte>          span          = owner.Memory.Span[..count];

        try
        {
            if ( Convert.TryFromBase64String( base64, span, out bytesWritten ) ) { return owner; }

            bytesWritten = Encoding.Default.GetBytes( base64, span );
            return owner;
        }
        catch ( Exception )
        {
            byte[] array = encoding.GetBytes( base64 );
            bytesWritten = array.Length;
            array.CopyTo( span );
            return owner;
        }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private void ValidateState()
    {
        ObjectDisposedException.ThrowIf( _disposed, this );
        if ( _keyIsSet is false ) { throw new InvalidOperationException( $"Must call {nameof(WithKey)} or {nameof(WithKeyAsync)}  first" ); }
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool TryDecrypt( scoped in ReadOnlySpan<byte> value, scoped ref Span<byte> destination, out int bytesWritten )
    {
        ValidateState();
        return _rsa.TryDecrypt( value, destination, _padding, out bytesWritten );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public byte[] Decrypt( scoped in ReadOnlySpan<byte> encrypted, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        ValidateState();
        Debug.Assert( encrypted.Length % BLOCK == 0 );
        if ( encrypted.Length <= BLOCK ) { return _rsa.Decrypt( encrypted, _padding ); }


        using MemoryStream stream    = new(encrypted.Length);
        Span<byte>         partition = stackalloc byte[DATA + 1];

        for ( int i = 0; i <= encrypted.Length / BLOCK; i++ )
        {
            int                size  = Math.Min( BLOCK, encrypted.Length - i * BLOCK );
            ReadOnlySpan<byte> block = encrypted.Slice( i * BLOCK, size );
            if ( block.IsEmpty ) { continue; }

            Debug.Assert( block.Length == BLOCK );

            if ( _rsa.TryDecrypt( block, partition, _padding, out int bytesWritten ) )
            {
                stream.Write( partition[..bytesWritten] );
                Debug.Assert( bytesWritten <= DATA );
            }
        }

        return stream.ToArray();
    }


    public string Decrypt( string value, in TelemetrySpan parent = default ) { return Decrypt( value, Encoding.Default, in parent ); }
    public string Decrypt( string value, Encoding encoding, in TelemetrySpan parent = default )
    {
        using TelemetrySpan      span  = parent.SubSpan();
        using IMemoryOwner<byte> owner = GetBytes( value, encoding, out int bytesWritten );
        return encoding.GetString( Decrypt( owner.Memory.Span[..bytesWritten], in span ) );
    }
    public byte[] Decrypt( LocalFile file, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span   = parent.SubSpan();
        byte[]              raw    = file.Read().AsBytes();
        byte[]              result = Decrypt( raw, in span );
        return result;
    }
    public string Decrypt( LocalFile file, Encoding encoding, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span   = parent.SubSpan();
        string              raw    = file.Read().AsString( in span );
        string              result = Decrypt( raw, encoding );
        return result;
    }
    public TValue Decrypt<TValue>( LocalFile file, IDataProtector.Decryptor<TValue> func, TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return func( file.Read(), this, in span );
    }
    public async ValueTask<byte[]> DecryptAsync( LocalFile file, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span   = parent.SubSpan();
        byte[]              raw    = await file.ReadAsync().AsBytes( span, token );
        byte[]              result = Decrypt( raw, in span );
        return result;
    }
    public async ValueTask<string> DecryptAsync( LocalFile file, Encoding encoding, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span   = parent.SubSpan();
        string              raw    = await file.ReadAsync().AsString( span, token );
        string              result = Decrypt( raw, encoding );
        return result;
    }
    public async ValueTask<TValue> DecryptAsync<TValue>( LocalFile file, IDataProtector.DecryptorAsync<TValue> func, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        return await func( file.ReadAsync(), this, span, token );
    }
    public async ValueTask DecryptAsync( LocalFile input, LocalFile output, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span   = parent.SubSpan();
        byte[]              raw    = await input.ReadAsync().AsBytes( span, token );
        byte[]              result = Decrypt( raw, in span );
        await output.WriteAsync( result, span, token );
    }
    public async ValueTask DecryptAsync( LocalFile input, LocalFile output, Encoding encoding, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span   = parent.SubSpan();
        string              raw    = await input.ReadAsync().AsString( span, token );
        string              result = Decrypt( raw, encoding );
        await output.WriteAsync( result, span, token );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool TryEncrypt( scoped in ReadOnlySpan<byte> value, scoped ref Span<byte> destination, out int bytesWritten )
    {
        ValidateState();
        return _rsa.TryEncrypt( value, destination, _padding, out bytesWritten );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public byte[] Encrypt( scoped in ReadOnlySpan<byte> value, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        ValidateState();
        if ( value.Length <= DATA ) { return _rsa.Encrypt( value, _padding ); }


        using MemoryStream stream    = new(value.Length * 2);
        Span<byte>         partition = stackalloc byte[DATA + 1];

        for ( int i = 0; i <= value.Length / DATA; i++ )
        {
            int                size  = Math.Min( DATA, value.Length - i * DATA );
            ReadOnlySpan<byte> block = value.Slice( i * DATA, size );
            if ( block.IsEmpty ) { continue; }

            Debug.Assert( block.Length == size );
            Debug.Assert( block.Length <= DATA );

            if ( _rsa.TryEncrypt( block, partition, _padding, out int bytesWritten ) )
            {
                stream.Write( partition[..bytesWritten] );
                Debug.Assert( bytesWritten <= DATA );
            }
        }

        return stream.ToArray();
    }


    public string Encrypt( string value, in TelemetrySpan parent = default ) { return Encrypt( value, Encoding.Default, in parent ); }
    public string Encrypt( string value, Encoding encoding, in TelemetrySpan parent = default )
    {
        using TelemetrySpan      span  = parent.SubSpan();
        using IMemoryOwner<byte> owner = GetBytes( value, encoding, out int bytesWritten );
        return Convert.ToBase64String( Encrypt( owner.Memory.Span[..bytesWritten], in span ) );
    }
    public void Encrypt( LocalFile file, string value, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        Encrypt( file, value, Encoding.Default );
    }
    public void Encrypt( LocalFile file, string value, Encoding encoding, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        file.Write( Encrypt( value, encoding ) );
    }
    public void Encrypt( LocalFile file, byte[] value, in TelemetrySpan parent = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        file.Write( Encrypt( value, in span ) );
    }
    public async ValueTask EncryptAsync( LocalFile file, string value, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        await EncryptAsync( file, value, Encoding.Default, span, token );
    }
    public async ValueTask EncryptAsync( LocalFile file, string value, Encoding encoding, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        await file.WriteAsync( Encrypt( value, encoding, in span ), span, token );
    }
    public async ValueTask EncryptAsync( LocalFile file, byte[] value, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span = parent.SubSpan();
        await file.WriteAsync( Encrypt( value, in span ), span, token );
    }
    public async ValueTask<byte[]> EncryptAsync( LocalFile value, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span   = parent.SubSpan();
        byte[]              raw    = await value.ReadAsync().AsBytes( span, token );
        byte[]              result = Encrypt( raw, in span );
        return result;
    }
    public async ValueTask<string> EncryptAsync( LocalFile value, Encoding encoding, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span   = parent.SubSpan();
        string              raw    = await value.ReadAsync().AsString( span, token );
        string              result = Encrypt( raw, encoding, in span );
        return result;
    }
    public async ValueTask EncryptAsync( LocalFile input, LocalFile output, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span   = parent.SubSpan();
        byte[]              raw    = await input.ReadAsync().AsBytes( span, token );
        byte[]              result = Encrypt( raw, in span );
        await output.WriteAsync( result, span, token );
    }
    public async ValueTask EncryptAsync( LocalFile input, LocalFile output, Encoding encoding, TelemetrySpan parent = default, CancellationToken token = default )
    {
        using TelemetrySpan span   = parent.SubSpan();
        string              raw    = await input.ReadAsync().AsString( span, token );
        string              result = Encrypt( raw, encoding, in span );
        await output.WriteAsync( result, span, token );
    }
}
