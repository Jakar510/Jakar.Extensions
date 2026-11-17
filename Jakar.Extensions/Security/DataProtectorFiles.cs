// Jakar.Extensions :: Jakar.Files
// 11/15/2026

using Microsoft.Extensions.Configuration;



namespace Jakar.Extensions;


/// <summary> <see cref="IDataProtector"/> and <see cref="DataProtector"/> members that read or write a <see cref="LocalFile"/> or load a key from <see cref="EmbeddedResources{TValue}"/>. Until 10.12.0 they were instance members in Jakar.Extensions. </summary>
public static class DataProtectorFiles
{
    public delegate TValue Decryptor<out TValue>( LocalFile.IReadHandler handler, IDataProtector dataProtector );



    public delegate ValueTask<TValue> DecryptorAsync<TValue>( LocalFile.IAsyncReadHandler handler, IDataProtector dataProtector, CancellationToken token );



    extension( IDataProtector protector )
    {
        public void Encrypt( LocalFile file, string value )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
            protector.Encrypt(file, value, Encoding.Default);
        }
        public void Encrypt( LocalFile file, string value, Encoding encoding )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
            file.Write(protector.Encrypt(value, encoding));
        }
        public void Encrypt( LocalFile file, byte[] value )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
            file.Write(protector.Encrypt(value));
        }
        public async ValueTask EncryptAsync( LocalFile file, string value, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            await protector.EncryptAsync(file, value, Encoding.Default, token).ConfigureAwait(false);
        }
        public async ValueTask EncryptAsync( LocalFile file, string value, Encoding encoding, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            await file.WriteAsync(protector.Encrypt(value, encoding), token).ConfigureAwait(false);
        }
        public async ValueTask EncryptAsync( LocalFile file, byte[] value, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            await file.WriteAsync(protector.Encrypt(value), token).ConfigureAwait(false);
        }
        public async ValueTask<byte[]> EncryptAsync( LocalFile value, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            byte[] raw = await value.ReadAsync().AsBytes(token).ConfigureAwait(false);
            return protector.Encrypt(raw);
        }
        public async ValueTask<string> EncryptAsync( LocalFile value, Encoding encoding, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            string raw = await value.ReadAsync().AsString(token).ConfigureAwait(false);
            return protector.Encrypt(raw, encoding);
        }
        public async ValueTask EncryptAsync( LocalFile input, LocalFile output, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            byte[] raw = await input.ReadAsync().AsBytes(token).ConfigureAwait(false);
            await output.WriteAsync(protector.Encrypt(raw), token).ConfigureAwait(false);
        }
        public async ValueTask EncryptAsync( LocalFile input, LocalFile output, Encoding encoding, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            string raw = await input.ReadAsync().AsString(token).ConfigureAwait(false);
            await output.WriteAsync(protector.Encrypt(raw, encoding), token).ConfigureAwait(false);
        }


        public byte[] Decrypt( LocalFile file )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            byte[] raw = file.Read().AsBytes();
            return protector.Decrypt(raw);
        }
        public string Decrypt( LocalFile file, Encoding encoding )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            string raw = file.Read().AsString();
            return protector.Decrypt(raw, encoding);
        }
        public TValue Decrypt<TValue>( LocalFile file, Decryptor<TValue> func )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
            return func(file.Read(), protector);
        }
        public async ValueTask<byte[]> DecryptAsync( LocalFile file, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            byte[] raw = await file.ReadAsync().AsBytes(token).ConfigureAwait(false);
            return protector.Decrypt(raw);
        }
        public async ValueTask<string> DecryptAsync( LocalFile file, Encoding encoding, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            string raw = await file.ReadAsync().AsString(token).ConfigureAwait(false);
            return protector.Decrypt(raw, encoding);
        }
        public async ValueTask<TValue> DecryptAsync<TValue>( LocalFile file, DecryptorAsync<TValue> func, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            return await func(file.ReadAsync(), protector, token).ConfigureAwait(false);
        }
        public async ValueTask DecryptAsync( LocalFile input, LocalFile output, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            byte[] raw = await input.ReadAsync().AsBytes(token).ConfigureAwait(false);
            await output.WriteAsync(protector.Decrypt(raw), token).ConfigureAwait(false);
        }
        public async ValueTask DecryptAsync( LocalFile input, LocalFile output, Encoding encoding, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            string raw = await input.ReadAsync().AsString(token).ConfigureAwait(false);
            await output.WriteAsync(protector.Decrypt(raw, encoding), token).ConfigureAwait(false);
        }
    }



    extension( DataProtector protector )
    {
        public DataProtector WithKey<TValue>( EmbeddedResources<TValue> resources, string name )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
            return protector.WithKey(resources.GetResourceText(name));
        }
        public DataProtector WithKey<TValue>( EmbeddedResources<TValue> resources, string name, SecuredString password )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
            return protector.WithKey(resources.GetResourceText(name), password);
        }
        public DataProtector WithKey<TValue>( EmbeddedResources<TValue> resources, string name, scoped in ReadOnlySpan<char> password )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
            return protector.WithKey(resources.GetResourceText(name), password);
        }
        public async ValueTask<DataProtector> WithKeyAsync<TValue>( EmbeddedResources<TValue> resources, string name )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            return protector.WithKey(await resources.GetResourceTextAsync(name).ConfigureAwait(false));
        }
        public async ValueTask<DataProtector> WithKeyAsync<TValue>( EmbeddedResources<TValue> resources, string name, string password )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            return protector.WithKey(await resources.GetResourceTextAsync(name).ConfigureAwait(false), password);
        }
        public async ValueTask<DataProtector> WithKeyAsync<TValue>( EmbeddedResources<TValue> resources, string name, SecuredString password )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            return protector.WithKey(await resources.GetResourceTextAsync(name).ConfigureAwait(false), password);
        }


        [RequiresUnreferencedCode("Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)")]
        public async ValueTask<DataProtector> WithKeyAsync<TValue>( EmbeddedResources<TValue> resources, string name, SecuredString.ResolverOptions password, IConfiguration configuration, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            return await protector.WithKeyAsync(resources, name, await password.GetSecuredStringAsync(configuration, token).ConfigureAwait(false)).ConfigureAwait(false);
        }


        public DataProtector WithKeyFile( LocalFile pem )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            return protector.WithKey(pem.Read().AsString());
        }
        public DataProtector WithKeyFile( LocalFile pem, scoped in ReadOnlySpan<char> password )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            return protector.WithKey(pem.Read().AsString(), password);
        }
        public DataProtector WithKeyFile( LocalFile pem, SecuredString password )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            return protector.WithKey(pem.Read().AsString(), password);
        }
        public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            return protector.WithKey(await pem.ReadAsync().AsString(token).ConfigureAwait(false));
        }
        public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, string password, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            return protector.WithKey(await pem.ReadAsync().AsString(token).ConfigureAwait(false), password);
        }
        public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, SecuredString password, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            return protector.WithKey(await pem.ReadAsync().AsString(token).ConfigureAwait(false), password);
        }


        [RequiresUnreferencedCode("Microsoft.Extensions.Configuration.ConfigurationBinder.GetValue<TValue>(String)")]
        public async ValueTask<DataProtector> WithKeyAsync( LocalFile pem, SecuredString.ResolverOptions password, IConfiguration configuration, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            return await protector.WithKeyAsync(pem, await password.GetSecuredStringAsync(configuration, token).ConfigureAwait(false), token).ConfigureAwait(false);
        }
    }



    extension( PasswordRequirements requirements )
    {
        /// <summary> Reads the blocked passwords from <paramref name="file"/>: a JSON string array, or one password per line. </summary>
        public async ValueTask SetBlockedPasswords( LocalFile file, CancellationToken token = default )
        {
            using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

            requirements.SetBlockedPasswords(await file.ReadAsync().AsString(token).ConfigureAwait(false));
        }
    }
}
