// Jakar.Extensions :: Jakar.Database
// 04/29/2023  10:47 PM

using Microsoft.Extensions.Configuration;



namespace Jakar.Extensions;


public interface IDataProtectorProvider
{
    public IDataProtector            GetProtector();
    public ValueTask<IDataProtector> GetProtectorAsync();

    // public IDataProtector            GetProtector<T>();
    // public ValueTask<IDataProtector> GetProtectorAsync<T>();
    // public IDataProtector            GetProtector( string      className );
    // public ValueTask<IDataProtector> GetProtectorAsync( string className );
}



public interface IDataProtector : IDisposable
{
    public bool   TryEncrypt( scoped in ReadOnlySpan<byte> value, scoped ref Span<byte> destination, out int bytesWritten );
    public byte[] Encrypt( scoped in    ReadOnlySpan<byte> value );
    public string Encrypt( string                          value );
    public string Encrypt( string                          value, Encoding encoding );
    public void   Encrypt( LocalFile                       file,  string   value );
    public void   Encrypt( LocalFile                       file,  string   value, Encoding encoding );
    public void   Encrypt( LocalFile                       file,  byte[]   value );


    public ValueTask<byte[]> EncryptAsync( LocalFile value, CancellationToken token                                                         = default );
    public ValueTask<string> EncryptAsync( LocalFile value, Encoding          encoding, CancellationToken token                             = default );
    public ValueTask         DecryptAsync( LocalFile input, LocalFile         output,   Encoding          encoding, CancellationToken token = default );
    public ValueTask         EncryptAsync( LocalFile file,  string            value,    CancellationToken token                             = default );
    public ValueTask         EncryptAsync( LocalFile file,  string            value,    Encoding          encoding, CancellationToken token = default );
    public ValueTask         EncryptAsync( LocalFile file,  byte[]            value,    CancellationToken token                             = default );
    public ValueTask         EncryptAsync( LocalFile input, LocalFile         output,   CancellationToken token                             = default );
    public ValueTask         EncryptAsync( LocalFile input, LocalFile         output,   Encoding          encoding, CancellationToken token = default );


    public bool   TryDecrypt( scoped in ReadOnlySpan<byte> value, scoped ref Span<byte> destination, out int bytesWritten );
    public byte[] Decrypt( scoped in    ReadOnlySpan<byte> value );
    public string Decrypt( string                          value );
    public string Decrypt( string                          value, Encoding encoding );
    public byte[] Decrypt( LocalFile                       file );
    public string Decrypt( LocalFile                       file, Encoding          encoding );
    public TValue Decrypt<TValue>( LocalFile               file, Decryptor<TValue> func );


    public ValueTask<byte[]> DecryptAsync( LocalFile         file,  CancellationToken      token                             = default );
    public ValueTask<string> DecryptAsync( LocalFile         file,  Encoding               encoding, CancellationToken token = default );
    public ValueTask<TValue> DecryptAsync<TValue>( LocalFile file,  DecryptorAsync<TValue> func,     CancellationToken token = default );
    public ValueTask         DecryptAsync( LocalFile         input, LocalFile              output,   CancellationToken token = default );



    public delegate TValue Decryptor<out TValue>( LocalFile.IReadHandler handler, IDataProtector dataProtector );



    public delegate ValueTask<TValue> DecryptorAsync<TValue>( LocalFile.IAsyncReadHandler handler, IDataProtector dataProtector, CancellationToken token );
}



public sealed class DataProtector( RSA rsa, RSAEncryptionPadding padding ) : IDataProtector
{
    private const    int                  BLOCK     = 512;
    private const    int                  DATA      = 254;
    private readonly RSA                  __rsa     = rsa;
    private readonly RSAEncryptionPadding __padding = padding;
    private          bool                 __disposed;
    private          bool                 __keyIsSet;


    public DataProtector( RSAEncryptionPadding padding ) : this(RSA.Create(), padding) { }
    public void Dispose()
    {
        __rsa.Dispose();
        __disposed = true;
    }


    public DataProtector WithKey( scoped in ReadOnlySpan<char> pem )
    {
        if ( __keyIsSet ) { throw new WarningException($"{nameof(WithKey)} or {nameof(WithKeyAsync)} has already been called"); }

        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        __rsa.ImportFromPem(pem);
        __keyIsSet = true;
        return this;
    }
    public DataProtector WithKey( scoped in ReadOnlySpan<char> pem, scoped in ReadOnlySpan<char> password )
    {
        if ( __keyIsSet ) { throw new WarningException($"{nameof(WithKey)} or {nameof(WithKeyAsync)}  has already been called"); }

        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        __rsa.ImportFromEncryptedPem(pem, password);
        __keyIsSet = true;
        return this;
    }


    public DataProtector WithKey<TValue>( EmbeddedResources<TValue> resources, string name )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        return WithKey(resources.GetResourceText(name));
    }
    public DataProtector WithKey<TValue>( EmbeddedResources<TValue> resources, string name, SecuredString password )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        return WithKey(resources.GetResourceText(name), password);
    }
    public DataProtector WithKey<TValue>( EmbeddedResources<TValue> resources, string name, scoped in ReadOnlySpan<char> password )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        return WithKey(resources.GetResourceText(name), password);
    }
    public async ValueTask<DataProtector> WithKeyAsync<TValue>( EmbeddedResources<TValue> resources, string name )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        return WithKey(await resources.GetResourceTextAsync(name));
    }
    public async ValueTask<DataProtector> WithKeyAsync<TValue>( EmbeddedResources<TValue> resources, string name, string password )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        return WithKey(await resources.GetResourceTextAsync(name), password);
    }
    public async ValueTask<DataProtector> WithKeyAsync<TValue>( EmbeddedResources<TValue> resources, string name, SecuredString password )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        return WithKey(await resources.GetResourceTextAsync(name), password);
    }


    [RequiresUnreferencedCode("Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)")]
    public async ValueTask<DataProtector> WithKeyAsync<TValue>( EmbeddedResources<TValue> resources, string name, SecuredString.ResolverOptions password, IConfiguration configuration, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        return await WithKeyAsync(resources, name, await password.GetSecuredStringAsync(configuration, token));
    }


    public DataProtector WithKeyFile( LocalFile pem )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        return WithKey(pem.Read()
                          .AsString());
    }
    public DataProtector WithKeyFile( LocalFile pem, scoped in ReadOnlySpan<char> password )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        return WithKey(pem.Read()
                          .AsString(),
                       password);
    }
    public DataProtector WithKeyFile( LocalFile pem, SecuredString password )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        return WithKey(pem.Read()
                          .AsString(),
                       password);
    }
    public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        return WithKey(await pem.ReadAsync()
                                .AsString(token));
    }
    public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, string password, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        return WithKey(await pem.ReadAsync()
                                .AsString(token),
                       password);
    }
    public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, SecuredString password, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        return WithKey(await pem.ReadAsync()
                                .AsString(token),
                       password);
    }


    [RequiresUnreferencedCode("Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)")]
    public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, SecuredString.ResolverOptions password, IConfiguration configuration, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        return await WithKeyAsync(pem, await password.GetSecuredStringAsync(configuration, token), token);
    }


    public static byte[] GetBytes( string base64, Encoding encoding )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        try { return Convert.FromBase64String(base64); }
        catch ( Exception ) { return encoding.GetBytes(base64); }
    }
    public static IMemoryOwner<byte> GetBytes( string base64, Encoding encoding, out int bytesWritten )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        int                 count         = Encoding.Default.GetByteCount(base64);
        IMemoryOwner<byte>  owner         = MemoryPool<byte>.Shared.Rent(count);
        Span<byte>          span          = owner.Memory.Span[..count];

        try
        {
            if ( Convert.TryFromBase64String(base64, span, out bytesWritten) ) { return owner; }

            bytesWritten = Encoding.Default.GetBytes(base64, span);
            return owner;
        }
        catch ( Exception )
        {
            byte[] array = encoding.GetBytes(base64);
            bytesWritten = array.Length;
            array.CopyTo(span);
            return owner;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] private void ValidateState()
    {
        ObjectDisposedException.ThrowIf(__disposed, this);
        if ( !__keyIsSet ) { throw new InvalidOperationException($"Must call {nameof(WithKey)} or {nameof(WithKeyAsync)}  first"); }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool TryDecrypt( scoped in ReadOnlySpan<byte> value, scoped ref Span<byte> destination, out int bytesWritten )
    {
        ValidateState();
        return __rsa.TryDecrypt(value, destination, __padding, out bytesWritten);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public byte[] Decrypt( scoped in ReadOnlySpan<byte> encrypted )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        ValidateState();
        Debug.Assert(encrypted.Length % BLOCK == 0);
        if ( encrypted.Length <= BLOCK ) { return __rsa.Decrypt(encrypted, __padding); }


        using MemoryStream stream    = new(encrypted.Length);
        Span<byte>         partition = stackalloc byte[DATA + 1];

        for ( int i = 0; i <= encrypted.Length / BLOCK; i++ )
        {
            int                size  = Math.Min(BLOCK, encrypted.Length - i * BLOCK);
            ReadOnlySpan<byte> block = encrypted.Slice(i * BLOCK, size);
            if ( block.IsEmpty ) { continue; }

            Debug.Assert(block.Length == BLOCK);

            if ( __rsa.TryDecrypt(block, partition, __padding, out int bytesWritten) )
            {
                stream.Write(partition[..bytesWritten]);
                Debug.Assert(bytesWritten <= DATA);
            }
        }

        return stream.ToArray();
    }


    public string Decrypt( string value ) { return Decrypt(value, Encoding.Default); }
    public string Decrypt( string value, Encoding encoding )
    {
        using TelemetrySpan      telemetrySpan = TelemetrySpan.Create();
        using IMemoryOwner<byte> owner         = GetBytes(value, encoding, out int bytesWritten);
        return encoding.GetString(Decrypt(owner.Memory.Span[..bytesWritten]));
    }
    public byte[] Decrypt( LocalFile file )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        byte[] raw = file.Read()
                         .AsBytes();

        byte[] result = Decrypt(raw);
        return result;
    }
    public string Decrypt( LocalFile file, Encoding encoding )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        string raw = file.Read()
                         .AsString();

        string result = Decrypt(raw, encoding);
        return result;
    }
    public TValue Decrypt<TValue>( LocalFile file, IDataProtector.Decryptor<TValue> func )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        return func(file.Read(), this);
    }
    public async ValueTask<byte[]> DecryptAsync( LocalFile file, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        byte[] raw = await file.ReadAsync()
                               .AsBytes(token);

        byte[] result = Decrypt(raw);
        return result;
    }
    public async ValueTask<string> DecryptAsync( LocalFile file, Encoding encoding, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        string raw = await file.ReadAsync()
                               .AsString(token);

        string result = Decrypt(raw, encoding);
        return result;
    }
    public async ValueTask<TValue> DecryptAsync<TValue>( LocalFile file, IDataProtector.DecryptorAsync<TValue> func, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        return await func(file.ReadAsync(), this, token);
    }
    public async ValueTask DecryptAsync( LocalFile input, LocalFile output, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        byte[] raw = await input.ReadAsync()
                                .AsBytes(token);

        byte[] result = Decrypt(raw);
        await output.WriteAsync(result, token);
    }
    public async ValueTask DecryptAsync( LocalFile input, LocalFile output, Encoding encoding, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        string raw = await input.ReadAsync()
                                .AsString(token);

        string result = Decrypt(raw, encoding);
        await output.WriteAsync(result, token);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool TryEncrypt( scoped in ReadOnlySpan<byte> value, scoped ref Span<byte> destination, out int bytesWritten )
    {
        ValidateState();
        return __rsa.TryEncrypt(value, destination, __padding, out bytesWritten);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public byte[] Encrypt( scoped in ReadOnlySpan<byte> value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        ValidateState();
        if ( value.Length <= DATA ) { return __rsa.Encrypt(value, __padding); }


        using MemoryStream stream    = new(value.Length * 2);
        Span<byte>         partition = stackalloc byte[DATA + 1];

        for ( int i = 0; i <= value.Length / DATA; i++ )
        {
            int                size  = Math.Min(DATA, value.Length - i * DATA);
            ReadOnlySpan<byte> block = value.Slice(i * DATA, size);
            if ( block.IsEmpty ) { continue; }

            Debug.Assert(block.Length == size);
            Debug.Assert(block.Length <= DATA);

            if ( __rsa.TryEncrypt(block, partition, __padding, out int bytesWritten) )
            {
                stream.Write(partition[..bytesWritten]);
                Debug.Assert(bytesWritten <= DATA);
            }
        }

        return stream.ToArray();
    }


    public string Encrypt( string value ) { return Encrypt(value, Encoding.Default); }
    public string Encrypt( string value, Encoding encoding )
    {
        using TelemetrySpan      telemetrySpan = TelemetrySpan.Create();
        using IMemoryOwner<byte> owner         = GetBytes(value, encoding, out int bytesWritten);
        return Convert.ToBase64String(Encrypt(owner.Memory.Span[..bytesWritten]));
    }
    public void Encrypt( LocalFile file, string value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        Encrypt(file, value, Encoding.Default);
    }
    public void Encrypt( LocalFile file, string value, Encoding encoding )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        file.Write(Encrypt(value, encoding));
    }
    public void Encrypt( LocalFile file, byte[] value )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        file.Write(Encrypt(value));
    }
    public async ValueTask EncryptAsync( LocalFile file, string value, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        await EncryptAsync(file, value, Encoding.Default, token);
    }
    public async ValueTask EncryptAsync( LocalFile file, string value, Encoding encoding, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        await file.WriteAsync(Encrypt(value, encoding), token);
    }
    public async ValueTask EncryptAsync( LocalFile file, byte[] value, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        await file.WriteAsync(Encrypt(value), token);
    }
    public async ValueTask<byte[]> EncryptAsync( LocalFile value, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        byte[] raw = await value.ReadAsync()
                                .AsBytes(token);

        byte[] result = Encrypt(raw);
        return result;
    }
    public async ValueTask<string> EncryptAsync( LocalFile value, Encoding encoding, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        string raw = await value.ReadAsync()
                                .AsString(token);

        string result = Encrypt(raw, encoding);
        return result;
    }
    public async ValueTask EncryptAsync( LocalFile input, LocalFile output, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        byte[] raw = await input.ReadAsync()
                                .AsBytes(token);

        byte[] result = Encrypt(raw);
        await output.WriteAsync(result, token);
    }
    public async ValueTask EncryptAsync( LocalFile input, LocalFile output, Encoding encoding, CancellationToken token = default )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        string raw = await input.ReadAsync()
                                .AsString(token);

        string result = Encrypt(raw, encoding);
        await output.WriteAsync(result, token);
    }
}
