﻿// Jakar.Extensions :: Jakar.Database
// 04/29/2023  10:47 PM

using Microsoft.Extensions.Configuration;



namespace Jakar.Extensions;


#if NET6_0_OR_GREATER



public interface IDataProtectorProvider
{
    public IDataProtector            GetProtector();
    public ValueTask<IDataProtector> GetProtectorAsync();
}



public interface IDataProtector : IDisposable
{
    public bool              TryEncrypt( ReadOnlySpan<byte> value, Span<byte> destination, out int bytesWritten );
    public byte[]            Encrypt( byte[]                value );
    public string            Encrypt( string                value );
    public string            Encrypt( string                value, Encoding   encoding );
    public void              Encrypt( LocalFile             file,  string     value );
    public void              Encrypt( LocalFile             file,  string     value, Encoding encoding );
    public void              Encrypt( LocalFile             file,  byte[]     value );
    public bool              TryDecrypt( ReadOnlySpan<byte> value, Span<byte> destination, out int bytesWritten );
    public byte[]            Decrypt( byte[]                value );
    public string            Decrypt( string                value );
    public string            Decrypt( string                value, Encoding encoding );
    public byte[]            Decrypt( LocalFile             file );
    public string            Decrypt( LocalFile             file,  Encoding                                                        encoding );
    public T                 Decrypt<T>( LocalFile          file,  Func<LocalFile.IReadHandler, IDataProtector, T>                 func );
    public ValueTask<byte[]> DecryptAsync( LocalFile        file,  CancellationToken                                               token                             = default );
    public ValueTask<string> DecryptAsync( LocalFile        file,  Encoding                                                        encoding, CancellationToken token = default );
    public ValueTask<T>      DecryptAsync<T>( LocalFile     file,  Func<LocalFile.IAsyncReadHandler, IDataProtector, ValueTask<T>> func );
    public ValueTask         DecryptAsync( LocalFile        input, LocalFile                                                       output, CancellationToken token                             = default );
    public ValueTask         DecryptAsync( LocalFile        input, LocalFile                                                       output, Encoding          encoding, CancellationToken token = default );
    public ValueTask         EncryptAsync( LocalFile        file,  string                                                          value,  CancellationToken token                             = default );
    public ValueTask         EncryptAsync( LocalFile        file,  string                                                          value,  Encoding          encoding, CancellationToken token = default );
    public ValueTask         EncryptAsync( LocalFile        file,  byte[]                                                          value,  CancellationToken token = default );
    public ValueTask<byte[]> EncryptAsync( LocalFile        value, CancellationToken                                               token                                                         = default );
    public ValueTask<string> EncryptAsync( LocalFile        value, Encoding                                                        encoding, CancellationToken token                             = default );
    public ValueTask         EncryptAsync( LocalFile        input, LocalFile                                                       output,   CancellationToken token                             = default );
    public ValueTask         EncryptAsync( LocalFile        input, LocalFile                                                       output,   Encoding          encoding, CancellationToken token = default );
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


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public bool TryDecrypt( ReadOnlySpan<byte> value, Span<byte> destination, out int bytesWritten )
    {
        if ( _disposed ) { throw new ObjectDisposedException( nameof(DataProtector) ); }

        if ( !_keyIsSet ) { throw new InvalidOperationException( $"Must call {nameof(WithKey)} or {nameof(WithKeyAsync)}  first" ); }

        return _rsa.TryDecrypt( value, destination, _padding, out bytesWritten );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public byte[] Decrypt( byte[] encrypted )
    {
        if ( _disposed ) { throw new ObjectDisposedException( nameof(DataProtector) ); }

        if ( !_keyIsSet ) { throw new InvalidOperationException( $"Must call {nameof(WithKey)} or {nameof(WithKeyAsync)}  first" ); }

        Debug.Assert( encrypted.Length % BLOCK == 0 );
        if ( encrypted.Length <= BLOCK ) { return _rsa.Decrypt( encrypted, _padding ); }


        using MemoryStream stream = new MemoryStream( encrypted.Length );

        for ( int i = 0; i <= encrypted.Length / BLOCK; i++ )
        {
            int                size  = Math.Min( BLOCK, encrypted.Length - i * BLOCK );
            ReadOnlySpan<byte> block = encrypted.AsSpan( i * BLOCK, size );
            if ( block.IsEmpty ) { continue; }

            Debug.Assert( block.Length == BLOCK );
            byte[] partition = _rsa.Decrypt( block.ToArray(), _padding );
            Debug.Assert( partition.Length <= DATA );
            stream.Write( partition );
        }

        return stream.ToArray();
    }


    public string Decrypt( string value )                    => Decrypt( value, Encoding.Default );
    public string Decrypt( string value, Encoding encoding ) => encoding.GetString( Decrypt( GetBytes( value, encoding ) ) );
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
        if ( _disposed ) { throw new ObjectDisposedException( nameof(DataProtector) ); }

        if ( !_keyIsSet ) { throw new InvalidOperationException( $"Must call {nameof(WithKey)} or {nameof(WithKeyAsync)}  first" ); }

        return _rsa.TryEncrypt( value, destination, _padding, out bytesWritten );
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public byte[] Encrypt( byte[] value )
    {
        if ( _disposed ) { throw new ObjectDisposedException( nameof(DataProtector) ); }

        if ( !_keyIsSet ) { throw new InvalidOperationException( $"Must call {nameof(WithKey)} or {nameof(WithKeyAsync)}  first" ); }

        if ( value.Length <= DATA ) { return _rsa.Encrypt( value, _padding ); }


        using MemoryStream stream = new MemoryStream( value.Length * 2 );

        for ( int i = 0; i <= value.Length / DATA; i++ )
        {
            int                size  = Math.Min( DATA, value.Length - i * DATA );
            ReadOnlySpan<byte> block = value.AsSpan( i * DATA, size );
            if ( block.IsEmpty ) { continue; }

            Debug.Assert( block.Length == size );
            Debug.Assert( block.Length <= DATA );
            byte[] partition = _rsa.Encrypt( block.ToArray(), _padding );
            Debug.Assert( partition.Length == BLOCK );
            stream.Write( partition );
        }

        return stream.ToArray();
    }


    public       string    Encrypt( string         value )                                    => Encrypt( value, Encoding.Default );
    public       string    Encrypt( string         value, Encoding encoding )                 => Convert.ToBase64String( Encrypt( GetBytes( value, encoding ) ) );
    public       void      Encrypt( LocalFile      file,  string   value )                    => Encrypt( file, value, Encoding.Default );
    public       void      Encrypt( LocalFile      file,  string   value, Encoding encoding ) => file.Write( Encrypt( value, encoding ) );
    public       void      Encrypt( LocalFile      file,  byte[]   value )                                                                => file.Write( Encrypt( value ) );
    public       ValueTask EncryptAsync( LocalFile file,  string   value, CancellationToken token                             = default ) => EncryptAsync( file, value, Encoding.Default, token );
    public async ValueTask EncryptAsync( LocalFile file,  string   value, Encoding          encoding, CancellationToken token = default ) => await file.WriteAsync( Encrypt( value, encoding ) );
    public async ValueTask EncryptAsync( LocalFile file,  byte[]   value, CancellationToken token = default ) => await file.WriteAsync( Encrypt( value ), token );
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
#endif
