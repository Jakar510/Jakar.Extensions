// Jakar.Extensions :: Jakar.Database
// 04/29/2023  10:47 PM

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


    public bool   TryDecrypt( scoped in ReadOnlySpan<byte> value, scoped ref Span<byte> destination, out int bytesWritten );
    public byte[] Decrypt( scoped in    ReadOnlySpan<byte> value );
    public string Decrypt( string                          value );
    public string Decrypt( string                          value, Encoding encoding );
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
        if ( __keyIsSet ) { throw new WarningException($"{nameof(WithKey)} or WithKeyAsync has already been called"); }

        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        __rsa.ImportFromPem(pem);
        __keyIsSet = true;
        return this;
    }
    public DataProtector WithKey( scoped in ReadOnlySpan<char> pem, scoped in ReadOnlySpan<char> password )
    {
        if ( __keyIsSet ) { throw new WarningException($"{nameof(WithKey)} or WithKeyAsync  has already been called"); }

        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        __rsa.ImportFromEncryptedPem(pem, password);
        __keyIsSet = true;
        return this;
    }



    public static byte[] GetBytes( string base64, Encoding encoding )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        try { return Convert.FromBase64String(base64); }
        catch ( Exception ) { return encoding.GetBytes(base64); }
    }
    [MustDisposeResource] public static ArrayBuffer<byte> GetBytes( string base64OrRawString, Encoding encoding, out int bytesWritten )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        int                 count         = Encoding.Default.GetByteCount(base64OrRawString);
        ArrayBuffer<byte>   owner         = new(count);
        Span<byte>          span          = owner.Span[..count];

        try
        {
            if ( Convert.TryFromBase64String(base64OrRawString, span, out bytesWritten) ) { return owner; }

            bytesWritten = Encoding.Default.GetBytes(base64OrRawString, span);
            return owner;
        }
        catch ( Exception )
        {
            byte[] array = encoding.GetBytes(base64OrRawString);
            bytesWritten = array.Length;
            array.CopyTo(span);
            return owner;
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] private void ValidateState()
    {
        ObjectDisposedException.ThrowIf(__disposed, this);
        if ( !__keyIsSet ) { throw new InvalidOperationException($"Must call {nameof(WithKey)} or WithKeyAsync  first"); }
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


    public string Decrypt( string value ) => Decrypt(value, Encoding.Default);
    public string Decrypt( string value, Encoding encoding )
    {
        using TelemetrySpan     telemetrySpan = TelemetrySpan.Create();
        using ArrayBuffer<byte> owner         = GetBytes(value, encoding, out int bytesWritten);
        return encoding.GetString(Decrypt(owner.Span[..bytesWritten]));
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


    public string Encrypt( string value ) => Encrypt(value, Encoding.Default);
    public string Encrypt( string value, Encoding encoding )
    {
        using TelemetrySpan     telemetrySpan = TelemetrySpan.Create();
        using ArrayBuffer<byte> owner         = GetBytes(value, encoding, out int bytesWritten);
        return Convert.ToBase64String(Encrypt(owner.Span[..bytesWritten]));
    }
}
