// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/02/2026

using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;



namespace Jakar.Extensions.Tests.Serialization;


/// <summary> Library models through System.Text.Json: round trips, converters, observable collections (AOT-plan §2.13), preferences (§2.10). </summary>
[TestFixture]
public sealed class ModelJson_Tests
{
    // ─── Models ───────────────────────────────────────────────────────────────

    [Test]
    public void RoundTrip_NonDeterministicModels()
    {
        GcInfo gc = GcInfo.Create();
        Assert.That(GcInfo.FromJson(gc.ToJson()).TotalMemory, Is.EqualTo(gc.TotalMemory));

        ThreadInformation thread = ThreadInformation.Create();
        Assert.That(ThreadInformation.FromJson(thread.ToJson()).ManagedThreadID, Is.EqualTo(thread.ManagedThreadID));
    }


    [Test]
    public void ExceptionDetails_FromAThrownException_RoundTrips()
    {
        Exception exception;

        try { throw new InvalidOperationException("boom") { Data = { ["Key"] = 5, ["When"] = new DateTime(2026, 10, 2, 0, 0, 0, DateTimeKind.Utc), ["Other"] = new Point(1, 2) } }; }
        catch ( Exception e ) { exception = e; }

        ExceptionDetails details = exception.FullDetails();
        Assert.That(details.Data!["Key"]!.GetValue<int>(), Is.EqualTo(5));
        Assert.That(details.Data["Other"]!.GetValue<string>(), Is.EqualTo("Point { X = 1, Y = 2 }"), "unknown types become their ToString(): no reflection");

        ExceptionDetails back = ExceptionDetails.FromJson(details.ToJson());
        Assert.That(back.Message,                    Is.EqualTo("boom"));
        Assert.That(back.Type,                       Is.EqualTo(typeof(InvalidOperationException).FullName));
        Assert.That(back.Data!["Key"]!.GetValue<int>(), Is.EqualTo(5));
    }


    [Test]
    public void LoginRequestValue_CarriesAnyJson()
    {
        LoginRequestValue request = new("tyler", "hunter2", "{\"Token\":\"abc\",\"Ttl\":30}".FromJson<JsonElement>());
        LoginRequestValue back    = LoginRequestValue.FromJson(request.ToJson());

        Assert.That(back.UserPassword,              Is.EqualTo("hunter2"));
        Assert.That(back.Data.Get<string>("Token"), Is.EqualTo("abc"));
        Assert.That(new LoginRequestValue().ToJson(), Does.Contain("\"Data\": null"));
    }


    [Test]
    public void LoginRequest_CopyConstructors_KeepThePassword()
    {
        LoginRequest        source  = new("tyler", "hunter2");
        LoginRequestVersion version = new(new LoginRequestVersion(source, AppVersion.Default));
        Assert.That(version.UserPassword, Is.EqualTo("hunter2"));
        Assert.That(version.Equals((object)new LoginRequestVersion("tyler", "hunter2", AppVersion.Default)), Is.True);
    }


    [Test]
    public void Email_SerializesAsAString_AndTryParseValidates()
    {
        Assert.That(Json.Serialize(new Email("a@b.test")), Is.EqualTo("\"a@b.test\""));
        Assert.That("\"a@b.test\"".FromJson<Email>().Value, Is.EqualTo("a@b.test"));
        Assert.That(Email.TryParse("a@b.test", null, out _), Is.True);
        Assert.That(Email.TryParse("nope",     null, out _), Is.False);
    }


    [Test]
    public void LocalFile_And_LocalDirectory_SerializeAsTheirPath()
    {
        string        directory = Path.Combine(Path.GetTempPath(), $"jakar-{Guid.NewGuid():N}");
        LocalDirectory dir      = new(directory);

        try
        {
            LocalFile file = dir.Join("missing.txt"); // doesn't exist: the 10.x contract threw on Length
            string    json = file.ToJson();

            Assert.That(json,                                   Does.Contain("FullPath").And.Not.Contain("Length"));
            Assert.That(LocalFile.FromJson(json).FullPath,      Is.EqualTo(file.FullPath));
            Assert.That(LocalFile.FromJson($"\"{file.FullPath.Replace("\\", "\\\\")}\"").FullPath, Is.EqualTo(file.FullPath), "a plain path string reads too");
            Assert.That(LocalDirectory.FromJson(dir.ToJson()).FullPath, Is.EqualTo(dir.FullPath));
        }
        finally { Directory.Delete(directory, true); }
    }


    // ─── Observable collections: unfiltered on every path (decision 14) ───────

    [Test]
    public void ObservableCollection_WritesUnfiltered_OnEveryPath()
    {
        ObservableCollection<int> collection = new(1, 2, 3, 4) { OverrideFilter = static ( int _, ref readonly int value ) => value % 2 == 0 };

        Assert.That(collection.ToArray(),                                                                     Is.EqualTo(new[] { 1, 2, 3, 4 }));
        Assert.That(collection.Where(static _ => true).ToArray(),                                              Is.EqualTo(new[] { 2, 4 }), "enumeration is filtered");
        Assert.That(collection.ToJson(false),                                                                  Is.EqualTo("[1,2,3,4]"));
        Assert.That(JsonModel.ToJson((IEnumerable<int>)collection, TestJsonContext.Default.Int32),            Is.EqualTo("[1,2,3,4]"));
        Assert.That(JsonSerializer.Serialize(collection, TestJsonContext.Default.ObservableCollectionInt32),   Is.EqualTo("[1,2,3,4]"), "STJ itself, through the converter registered in the context");
    }


    [Test]
    public void ObservableCollection_FromJson_InsertsInBulk()
    {
        ObservableCollection<int> collection = ObservableCollection<int>.FromJson("[1,2,3]");
        Assert.That(collection.ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));

        Assert.That(ObservableCollection<int>.TryFromJson("[1,", out _), Is.False);
        Assert.That(ConcurrentObservableCollection<int>.FromJson("[4,5]").ToArray(), Is.EqualTo(new[] { 4, 5 }));
        Assert.That(ObservableHashSet<int>.FromJson("[6,6,7]").Count,                Is.EqualTo(2));
    }


    [Test]
    public void UserModel_ObservableProperties_RoundTrip()
    {
        UserGuid.UserModel user = Golden.GoldenSamples.User();
        UserGuid.UserModel back = UserGuid.UserModel.FromJson(user.ToJson());

        Assert.That(back.Addresses.Single().City, Is.EqualTo("Springfield"));
        Assert.That(back.Groups.Single().NameOfGroup, Is.EqualTo("Admins"));
        Assert.That(back.Roles.Single().NameOfRole, Is.EqualTo("Admin"));
    }


    [Test]
    public void ConcurrentObservableCollection_ToJson_IsAConsistentSnapshot()
    {
        ConcurrentObservableCollection<int> collection = new(Enumerable.Range(0, 1000).ToArray());
        using CancellationTokenSource       stop       = new();

        Task writer = Task.Run(() =>
                               {
                                   while ( !stop.IsCancellationRequested )
                                   {
                                       collection.Add(-1);
                                       collection.Remove(-1);
                                   }
                               });

        try
        {
            for ( int i = 0; i < 50; i++ )
            {
                int[] values = JsonModel.FromJsonArray(collection.ToJson(false), TestJsonContext.Default.Int32);
                Assert.That(values.Count(static x => x >= 0), Is.EqualTo(1000));
            }
        }
        finally
        {
            stop.Cancel();
            writer.Wait();
        }
    }


    // ─── HTTP bodies: runtime type ────────────────────────────────────────────

    [Test]
    public async Task JsonBody_UsesTheRuntimeType()
    {
        BaseClass              model   = Golden.GoldenSamples.Alert();
        using ByteArrayContent content = WebRequester.CreateJsonContentOfRuntimeType(model);
        string                 json    = await content.ReadAsStringAsync();

        Assert.That(json, Does.Contain("\"Title\":\"Heads up\""), "serialized as Alert, not as BaseClass");
        Assert.That(json, Does.Not.Contain("\n"),                  "compact");
    }


    // ─── PreferenceFile (§2.10) ───────────────────────────────────────────────

    [Test]
    public async Task Preferences_SaveLoad_KeepUnknownKeys()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"jakar-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        try
        {
            LocalFile file = new(Path.Combine(directory, "prefs.json"));
            await File.WriteAllTextAsync(file.FullPath, "{\"General\":{\"Theme\":\"Light\",\"Enabled\":true},\"Server\":{\"Host\":\"h\",\"Port\":\"8080\"},\"FromNewerVersion\":{\"A\":1}}");

            await using ( TestPreferences prefs = await TestPreferences.CreateAsync(file) )
            {
                Assert.That(prefs.General.Theme,   Is.EqualTo("Light"));
                Assert.That(prefs.Server.Port,     Is.EqualTo(8080));
                Assert.That(prefs.Contains("fromnewerversion"), Is.True, "unknown keys are kept, case-insensitively");
                prefs.General.Theme = "Blue";
            }

            string saved = await File.ReadAllTextAsync(file.FullPath);
            Assert.That(saved, Does.Contain("\"Blue\"").And.Contain("FromNewerVersion"));
        }
        finally { Directory.Delete(directory, true); }
    }


    [Test]
    public async Task Preferences_ImportLegacyIni()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"jakar-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        try
        {
            LocalFile file = new(Path.Combine(directory, "prefs.json"));
            await File.WriteAllTextAsync(Path.Combine(directory, "prefs.ini"), "[General]\nTheme=Green\nEnabled=true\n\n[Server]\nHost=example\nPort=9000\n");

            TestPreferences prefs = await TestPreferences.CreateAsync(file);
            Assert.That(prefs.General.Theme,   Is.EqualTo("Green"));
            Assert.That(prefs.General.Enabled, Is.True);
            Assert.That(prefs.Server.Port,     Is.EqualTo(9000));
            Assert.That(file.Exists,            Is.True, "imported preferences are saved as JSON");
            Assert.That(File.Exists(Path.Combine(directory, "prefs.ini")), Is.True, "the .ini is kept as a backup");
        }
        finally { Directory.Delete(directory, true); }
    }


    [Test]
    public void IniConfig_JsonBridge_RoundTrips()
    {
        IniConfig config = IniConfig.Parse("[A]\nx=1\ny=true\n[B]\nz=text\n");
        System.Text.Json.Nodes.JsonObject json = config.ToJsonObject();

        Assert.That(json["A"]!["y"]!.GetValue<bool>(), Is.True);
        Assert.That(json["A"]!["x"]!.GetValue<string>(), Is.EqualTo("1"));

        IniConfig back = IniConfig.FromJsonObject(json);
        Assert.That(back["B"]["z"], Is.EqualTo("text"));
        Assert.That(back["A"]["y"], Is.EqualTo("true"));
    }
}
