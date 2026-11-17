// Jakar.Extensions :: Jakar.Files.Tests
// 11/15/2026

using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;



namespace Jakar.Extensions.Tests;


/// <summary> The LocalFile overloads of <see cref="IDataProtector"/>, <see cref="DataProtector"/> and <see cref="PasswordRequirements"/> (extension members since 10.12). </summary>
[TestFixture]
[TestOf(typeof(DataProtectorFiles))]
public sealed class DataProtectorFiles_Tests
{
    private string __directory = null!;


    [SetUp]    public void SetUp()    => Directory.CreateDirectory(__directory = Path.Combine(Path.GetTempPath(), $"jakar-{Guid.NewGuid():N}"));
    [TearDown] public void TearDown() => Directory.Delete(__directory, true);


    private async Task<DataProtector> CreateAsync()
    {
        using RSA rsa = RSA.Create(4096);
        LocalFile pem = new(Path.Combine(__directory, "key.pem"));
        await File.WriteAllTextAsync(pem.FullPath, rsa.ExportRSAPrivateKeyPem());

        return await new DataProtector(RSAEncryptionPadding.OaepSHA256).WithKeyAsync(pem);
    }


    [Test] public async Task EncryptAndDecrypt_Files_RoundTrip()
    {
        using DataProtector protector = await CreateAsync();
        IDataProtector      contract  = protector;
        LocalFile           text      = new(Path.Combine(__directory, "text.bin"));
        LocalFile           bytes     = new(Path.Combine(__directory, "bytes.bin"));

        contract.Encrypt(text, "hello", Encoding.UTF8);
        Assert.That(await File.ReadAllTextAsync(text.FullPath), Does.Not.Contain("hello"));
        Assert.That(contract.Decrypt(text, Encoding.UTF8),       Is.EqualTo("hello"));

        await contract.EncryptAsync(bytes, new byte[] { 1, 2, 3 });
        Assert.That(await contract.DecryptAsync(bytes), Is.EqualTo(new byte[] { 1, 2, 3 }));

        LocalFile plain = new(Path.Combine(__directory, "plain.bin"));
        await contract.DecryptAsync(bytes, plain);
        Assert.That(await File.ReadAllBytesAsync(plain.FullPath), Is.EqualTo(new byte[] { 1, 2, 3 }));
    }


    [Test] public void WithKeyFile_SetsTheKey()
    {
        using RSA rsa = RSA.Create(4096);
        LocalFile pem = new(Path.Combine(__directory, "key.pem"));
        File.WriteAllText(pem.FullPath, rsa.ExportRSAPrivateKeyPem());

        using DataProtector protector = new DataProtector(RSAEncryptionPadding.OaepSHA256).WithKeyFile(pem);
        Assert.That(protector.Decrypt(protector.Encrypt("secret")), Is.EqualTo("secret"));
    }


    [Test] public async Task SetBlockedPasswords_ReadsLinesOrJson()
    {
        LocalFile lines = new(Path.Combine(__directory, "blocked.txt"));
        await File.WriteAllTextAsync(lines.FullPath, "password\nletmein"); // a trailing newline adds an empty entry (PasswordRequirements.SetBlockedPasswords(string))

        PasswordRequirements requirements = new();
        await requirements.SetBlockedPasswords(lines);
        Assert.That(requirements.BlockedPasswords, Is.EquivalentTo(new[] { "password", "letmein" }));

        LocalFile json = new(Path.Combine(__directory, "blocked.json"));
        await File.WriteAllTextAsync(json.FullPath, "[\"hunter2\"]");
        await requirements.SetBlockedPasswords(json);
        Assert.That(requirements.BlockedPasswords, Is.EqualTo(new[] { "hunter2" }));
    }
}
