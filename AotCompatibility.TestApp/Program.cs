// Native AOT smoke test (AOT-plan §5.2). Every check runs in the published native executable; any failure exits non-zero.

using System.Net;
using AotCompatibility.TestApp;
using Jakar.Extensions;
using Jakar.Json;


int failures = 0;

void Check( string name, Func<bool> test )
{
    try
    {
        bool ok = test();
        Console.WriteLine($"{( ok ? "PASS" : "FAIL" )}  {name}");
        if ( !ok ) { failures++; }
    }
    catch ( Exception e )
    {
        Console.WriteLine($"FAIL  {name}: {e.GetType().Name}: {e.Message}");
        failures++;
    }
}


// ─── Models: [JsonModel] round trips ─────────────────────────────────────────
Check("AppVersion",     () => AppVersion.FromJson(new AppVersion(1, 2, 3).ToJson()).ToString()                                             == "1.2.3");
Check("AppInformation", () => AppInformation.FromJson(new AppInformation(new AppVersion(1), Guid.NewGuid(), "app", null).ToJson()).AppName == "app");
Check("StringTags",     () => StringTags.FromJson(new StringTags([new Pair("k", "v")], ["e"]).ToJson()).Tags[0].Value                      == "v");
Check("GcInfo",         () => GcInfo.FromJson(GcInfo.Create().ToJson()).TotalMemory                                                        >= 0);

Check("Extension data",
      () =>
      {
          FileMetaData file = FileMetaData.FromJson("{\"FileName\":\"t\",\"FileType\":\"text/plain\",\"MimeType\":null,\"Unknown\":{\"x\":[1,2]}}");
          return file.AdditionalData!.ContainsKey("Unknown") && file.ToJson().Contains("Unknown");
      });

// ─── App-defined models: generator + registry ───────────────────────────────
Check("App [JsonModel]",
      () => Invoice.FromJson(new Invoice
                             {
                                 Id    = "i1",
                                 Total = 9.5m
                             }.ToJson())
                   .Total ==
            9.5m);

Check("Registry lookup", () => JsonCodecRegistry.IsRegistered(typeof(Invoice)) && JsonCodecRegistry.IsRegistered(typeof(FileMetaData)));

// ─── Root arrays, collections ───────────────────────────────────────────────
Check("FromJsonList", () => JsonCodec.FromJsonList<Invoice>("[{\"Id\":\"a\"},{\"Id\":\"b\"}]").Count == 2);
Check("Array ToJson", () => Json.Serialize([new Invoice { Id = "x" }], false).StartsWith("[{"));

Check("ObservableCollection unfiltered",
      () =>
      {
          ObservableCollection<Invoice> invoices = new(new Invoice { Id = "a" }, new Invoice { Id = "b" }) { OverrideFilter = static ( int _, ref readonly Invoice? v ) => v?.Id == "b" };
          return invoices.ToJson().Contains("\"a\"") && ObservableCollection<Invoice>.FromJson("[{\"Id\":\"c\"}]").Count == 1;
      });

// ─── DOM helpers ─────────────────────────────────────────────────────────────
Check("JNode helpers",
      () =>
      {
          JObjectNode obj = "{\"A\":1}".FromJson().AsObject();
          obj.Set("B", new Invoice { Id = "two" });
          return obj.TryGetValue("A", out int a) && a == 1 && obj.Get<Invoice>("B")!.Id == "two";
      });

// ─── Exceptions (no reflection-based serialization) ─────────────────────────
Check("Exception.GetData",
      () =>
      {
          InvalidOperationException e = new("boom") { Data = { ["n"] = 5 } };
          return (int)e.GetData()["n"] == 5 && e.GetTags().Entries.Length > 0;
      });

// ─── Converters ──────────────────────────────────────────────────────────────
Check("Email as a string", () => Json.Serialize<Email, JsonParsableConverter<Email>>(new Email("a@b.test")) == "\"a@b.test\"");
Check("Encoding",          () => new LocalFile(Path.Combine(Path.GetTempPath(), "x.txt")).ToJson().Contains("\"FileEncoding\" : \"utf-8\""));

// ─── Other dependencies ──────────────────────────────────────────────────────
Check("OTP QR code (ZXing)", () => OneTimePassword.Create("Jakar").GetContent("ada").StartsWith("otpauth://"));
Check("IPAddress parse",     () => IPAddress.Parse("127.0.0.1").ToString() == "127.0.0.1");

// The Jakar.ErrorOr, Jakar.Http, Jakar.Users and Jakar.Logging types moved to their own repositories in 10.12; this app checks Jakar.Extensions only.


Console.WriteLine(failures == 0
                      ? "All checks passed."
                      : $"{failures} check(s) failed.");

return failures == 0
           ? 0
           : 1;



namespace AotCompatibility.TestApp
{
    [JsonModel]
    public sealed partial class Invoice : IEquatable<Invoice>
    {
        public string  Id    { get; init; } = "";
        public decimal Total { get; init; }


        public bool Equals( Invoice? other ) => other is not null && Id == other.Id && Total == other.Total;
        public override bool Equals( object? obj ) => obj is Invoice other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Id, Total);
    }
}
