﻿// Jakar.Extensions :: Jakar.Database
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
    public bool              TryEncrypt( ReadOnlySpan<byte>        value, Span<byte> destination, out int bytesWritten );
    public byte[]            Encrypt( scoped in ReadOnlySpan<byte> value );
    public string            Encrypt( string                       value );
    public string            Encrypt( string                       value, Encoding   encoding );
    public void              Encrypt( LocalFile                    file,  string     value );
    public void              Encrypt( LocalFile                    file,  string     value, Encoding encoding );
    public void              Encrypt( LocalFile                    file,  byte[]     value );
    public bool              TryDecrypt( ReadOnlySpan<byte>        value, Span<byte> destination, out int bytesWritten );
    public byte[]            Decrypt( scoped in ReadOnlySpan<byte> value );
    public string            Decrypt( string                       value );
    public string            Decrypt( string                       value, Encoding encoding );
    public byte[]            Decrypt( LocalFile                    file );
    public string            Decrypt( LocalFile                    file,  Encoding                                                        encoding );
    public T                 Decrypt<T>( LocalFile                 file,  Func<LocalFile.IReadHandler, IDataProtector, T>                 func );
    public ValueTask<byte[]> DecryptAsync( LocalFile               file,  CancellationToken                                               token                             = default );
    public ValueTask<string> DecryptAsync( LocalFile               file,  Encoding                                                        encoding, CancellationToken token = default );
    public ValueTask<T>      DecryptAsync<T>( LocalFile            file,  Func<LocalFile.IAsyncReadHandler, IDataProtector, ValueTask<T>> func );
    public ValueTask         DecryptAsync( LocalFile               input, LocalFile                                                       output, CancellationToken token                             = default );
    public ValueTask         DecryptAsync( LocalFile               input, LocalFile                                                       output, Encoding          encoding, CancellationToken token = default );
    public ValueTask         EncryptAsync( LocalFile               file,  string                                                          value,  CancellationToken token                             = default );
    public ValueTask         EncryptAsync( LocalFile               file,  string                                                          value,  Encoding          encoding, CancellationToken token = default );
    public ValueTask         EncryptAsync( LocalFile               file,  byte[]                                                          value,  CancellationToken token = default );
    public ValueTask<byte[]> EncryptAsync( LocalFile               value, CancellationToken                                               token                                                         = default );
    public ValueTask<string> EncryptAsync( LocalFile               value, Encoding                                                        encoding, CancellationToken token                             = default );
    public ValueTask         EncryptAsync( LocalFile               input, LocalFile                                                       output,   CancellationToken token                             = default );
    public ValueTask         EncryptAsync( LocalFile               input, LocalFile                                                       output,   Encoding          encoding, CancellationToken token = default );
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


    public DataProtector WithKey( scoped in ReadOnlySpan<char> pem )
    {
        if ( _keyIsSet ) { throw new WarningException( $"{nameof(WithKey)} or {nameof(WithKeyAsync)} has already been called" ); }

        _rsa.ImportFromPem( pem );
        _keyIsSet = true;
        return this;
    }
    public DataProtector WithKey( scoped in ReadOnlySpan<char> pem, scoped in ReadOnlySpan<char> password )
    {
        if ( _keyIsSet ) { throw new WarningException( $"{nameof(WithKey)} or {nameof(WithKeyAsync)}  has already been called" ); }

        _rsa.ImportFromEncryptedPem( pem, password );
        _keyIsSet = true;
        return this;
    }


    public       DataProtector            WithKey<T>( EmbeddedResources<T>      resources, string name )                                        => WithKey( resources.GetResourceText( name ) );
    public       DataProtector            WithKey<T>( EmbeddedResources<T>      resources, string name, SecuredString                password ) => WithKey( resources.GetResourceText( name ), password );
    public       DataProtector            WithKey<T>( EmbeddedResources<T>      resources, string name, scoped in ReadOnlySpan<char> password ) => WithKey( resources.GetResourceText( name ), password );
    public async ValueTask<DataProtector> WithKeyAsync<T>( EmbeddedResources<T> resources, string name )                                                                                               => WithKey( await resources.GetResourceTextAsync( name ) );
    public async ValueTask<DataProtector> WithKeyAsync<T>( EmbeddedResources<T> resources, string name, string                       password )                                                        => WithKey( await resources.GetResourceTextAsync( name ), password );
    public async ValueTask<DataProtector> WithKeyAsync<T>( EmbeddedResources<T> resources, string name, SecuredString                password )                                                        => WithKey( await resources.GetResourceTextAsync( name ), password );
    public async ValueTask<DataProtector> WithKeyAsync<T>( EmbeddedResources<T> resources, string name, SecuredStringResolverOptions password, IConfiguration configuration, CancellationToken token ) => await WithKeyAsync( resources, name, await password.GetSecuredStringAsync( configuration, token ) );


    public       DataProtector            WithKeyFile( LocalFile  pem )                                                                                                  => WithKey( pem.Read().AsString() );
    public       DataProtector            WithKeyFile( LocalFile  pem, scoped in ReadOnlySpan<char> password )                                                           => WithKey( pem.Read().AsString(), password );
    public       DataProtector            WithKeyFile( LocalFile  pem, SecuredString                password )                                                           => WithKey( pem.Read().AsString(), password );
    public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, CancellationToken            token                             = default )                        => WithKey( await pem.ReadAsync().AsString( token ) );
    public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, string                       password, CancellationToken token = default )                        => WithKey( await pem.ReadAsync().AsString( token ), password );
    public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, SecuredString                password, CancellationToken token = default )                        => WithKey( await pem.ReadAsync().AsString( token ), password );
    public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, SecuredStringResolverOptions password, IConfiguration    configuration, CancellationToken token ) => await WithKeyAsync( pem, await password.GetSecuredStringAsync( configuration, token ), token );


    public static byte[] GetBytes( string base64, Encoding encoding )
    {
        try { return Convert.FromBase64String( base64 ); }
        catch ( Exception ) { return encoding.GetBytes( base64 ); }
    }
    public static IMemoryOwner<byte> GetBytes( string base64, Encoding encoding, out int bytesWritten )
    {
        int                count = Encoding.Default.GetByteCount( base64 );
        IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent( count );
        Span<byte>         span  = owner.Memory.Span[..count];

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
    public bool TryDecrypt( ReadOnlySpan<byte> value, Span<byte> destination, out int bytesWritten )
    {
        ValidateState();
        return _rsa.TryDecrypt( value, destination, _padding, out bytesWritten );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public byte[] Decrypt( scoped in ReadOnlySpan<byte> encrypted )
    {
        ValidateState();
        Debug.Assert( encrypted.Length % BLOCK == 0 );
        if ( encrypted.Length <= BLOCK ) { return _rsa.Decrypt( encrypted, _padding ); }


        using MemoryStream stream    = new MemoryStream( encrypted.Length );
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


    public string Decrypt( string value ) => Decrypt( value, Encoding.Default );
    public string Decrypt( string value, Encoding encoding )
    {
        using IMemoryOwner<byte> owner = GetBytes( value, encoding, out int bytesWritten );
        return encoding.GetString( Decrypt( owner.Memory.Span[..bytesWritten] ) );
    }
    public byte[] Decrypt( LocalFile file )
    {
        byte[] raw = file.Read().AsBytes();

        byte[] result = Decrypt( raw );
        return result;
    }
    public string Decrypt( LocalFile file, Encoding encoding )
    {
        string raw = file.Read().AsString();

        string result = Decrypt( raw, encoding );
        return result;
    }
    public T Decrypt<T>( LocalFile file, Func<LocalFile.IReadHandler, IDataProtector, T> func ) => func( file.Read(), this );
    public async ValueTask<byte[]> DecryptAsync( LocalFile file, CancellationToken token = default )
    {
        byte[] raw    = await file.ReadAsync().AsBytes( token );
        byte[] result = Decrypt( raw );
        return result;
    }
    public async ValueTask<string> DecryptAsync( LocalFile file, Encoding encoding, CancellationToken token = default )
    {
        string raw    = await file.ReadAsync().AsString( token );
        string result = Decrypt( raw, encoding );
        return result;
    }
    public async ValueTask<T> DecryptAsync<T>( LocalFile file, Func<LocalFile.IAsyncReadHandler, IDataProtector, ValueTask<T>> func ) => await func( file.ReadAsync(), this );
    public async ValueTask DecryptAsync( LocalFile input, LocalFile output, CancellationToken token = default )
    {
        byte[] raw    = await input.ReadAsync().AsBytes( token );
        byte[] result = Decrypt( raw );
        await output.WriteAsync( result, token );
    }
    public async ValueTask DecryptAsync( LocalFile input, LocalFile output, Encoding encoding, CancellationToken token = default )
    {
        string raw    = await input.ReadAsync().AsString( token );
        string result = Decrypt( raw, encoding );
        await output.WriteAsync( result );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool TryEncrypt( ReadOnlySpan<byte> value, Span<byte> destination, out int bytesWritten )
    {
        ValidateState();
        return _rsa.TryEncrypt( value, destination, _padding, out bytesWritten );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public byte[] Encrypt( scoped in ReadOnlySpan<byte> value )
    {
        ValidateState();
        if ( value.Length <= DATA ) { return _rsa.Encrypt( value, _padding ); }


        using MemoryStream stream    = new MemoryStream( value.Length * 2 );
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


    public string Encrypt( string value ) => Encrypt( value, Encoding.Default );
    public string Encrypt( string value, Encoding encoding )
    {
        using IMemoryOwner<byte> owner = GetBytes( value, encoding, out int bytesWritten );
        return Convert.ToBase64String( Encrypt( owner.Memory.Span[..bytesWritten] ) );
    }
    public       void      Encrypt( LocalFile      file, string value )                    => Encrypt( file, value, Encoding.Default );
    public       void      Encrypt( LocalFile      file, string value, Encoding encoding ) => file.Write( Encrypt( value, encoding ) );
    public       void      Encrypt( LocalFile      file, byte[] value )                                                                => file.Write( Encrypt( value ) );
    public       ValueTask EncryptAsync( LocalFile file, string value, CancellationToken token                             = default ) => EncryptAsync( file, value, Encoding.Default, token );
    public async ValueTask EncryptAsync( LocalFile file, string value, Encoding          encoding, CancellationToken token = default ) => await file.WriteAsync( Encrypt( value, encoding ) );
    public async ValueTask EncryptAsync( LocalFile file, byte[] value, CancellationToken token = default ) => await file.WriteAsync( Encrypt( value ), token );
    public async ValueTask<byte[]> EncryptAsync( LocalFile value, CancellationToken token = default )
    {
        byte[] raw    = await value.ReadAsync().AsBytes( token );
        byte[] result = Encrypt( raw );
        return result;
    }
    public async ValueTask<string> EncryptAsync( LocalFile value, Encoding encoding, CancellationToken token = default )
    {
        string raw    = await value.ReadAsync().AsString( token );
        string result = Encrypt( raw, encoding );
        return result;
    }
    public async ValueTask EncryptAsync( LocalFile input, LocalFile output, CancellationToken token = default )
    {
        byte[] raw    = await input.ReadAsync().AsBytes( token );
        byte[] result = Encrypt( raw );
        await output.WriteAsync( result, token );
    }
    public async ValueTask EncryptAsync( LocalFile input, LocalFile output, Encoding encoding, CancellationToken token = default )
    {
        string raw    = await input.ReadAsync().AsString( token );
        string result = Encrypt( raw, encoding );
        await output.WriteAsync( result );
    }
}
