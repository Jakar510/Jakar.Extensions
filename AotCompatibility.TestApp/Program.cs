// Native AOT smoke test (AOT-plan §5.2). Every check runs in the published native executable; any failure exits non-zero.

using System.Collections.Immutable;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Jakar.Extensions;
using Jakar.Extensions.UserGuid;
using AotCompatibility.TestApp;


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
Check("AppVersion",     () => AppVersion.FromJson(new AppVersion(1, 2, 3).ToJson()).ToString() == "1.2.3");
Check("AppInformation", () => AppInformation.FromJson(new AppInformation(new AppVersion(1), Guid.NewGuid(), "app", null).ToJson()).AppName == "app");
Check("StringTags",     () => StringTags.FromJson(new StringTags([new Pair("k", "v")], ["e"]).ToJson()).Tags[0].Value == "v");
Check("Error/Errors",   () => Errors.FromJson(Errors.Create(Error.Validation(description: "bad")).ToJson()).Details[0].Description == "bad");
Check("Error (tier 2)", () => Json.Serialize(Error.NotFound()).FromJson<Error>().StatusCode == Status.NotFound);
Check("GcInfo",         () => GcInfo.FromJson(GcInfo.Create().ToJson()).TotalMemory >= 0);
Check("UserModel",      () =>
                        {
                            UserModel user = new("Ada", "Lovelace") { UserName = "ada" };
                            user.Addresses.Add(new UserAddress("1 Main", "", "Town", "ST", "00000", "US"));
                            UserModel back = UserModel.FromJson(user.ToJson());
                            return back.UserName == "ada" && back.Addresses.Count == 1;
                        });
Check("LoginRequestValue", () => LoginRequestValue.FromJson(new LoginRequestValue("u", "p", "{\"A\":1}".FromJson<JsonElement>()).ToJson()).Data.Get<int>("A") == 1);
Check("Extension data", () =>
                        {
                            Alert alert = Alert.FromJson("{\"Title\":\"t\",\"Unknown\":{\"x\":[1,2]}}");
                            return alert.AdditionalData!.ContainsKey("Unknown") && alert.ToJson().Contains("Unknown");
                        });

// ─── App-defined models: generator + registry ───────────────────────────────
Check("App [JsonModel]",  () => Invoice.FromJson(new Invoice { Id = "i1", Total = 9.5m }.ToJson()).Total == 9.5m);
Check("Registry lookup",  () => Json.GetTypeInfo<Invoice>() == Invoice.JsonTypeInfo);
Check("Runtime-type body", () => WebRequester.CreateJsonContentOfRuntimeType(new Alert("t")).ReadAsStringAsync().Result.Contains("\"Title\":\"t\""));

// ─── Element-wise arrays, pooled buffers, collections ───────────────────────
Check("FromJsonList (no List<T> metadata)", () => JsonModel.FromJsonList("[{\"Id\":\"a\"},{\"Id\":\"b\"}]", Invoice.JsonTypeInfo).Count == 2);
Check("ImmutableArray build",               () => JsonModel.FromJsonArray("[1,2,3]", JakarExtensionsContext.Default.Int32, static ImmutableArray<int> (ReadOnlySpan<int> s) => [.. s]).Length == 3);
Check("RentedArray",                        () =>
                                            {
                                                using RentedArray<int> values = JsonModel.FromJsonArrayPooled("[1,2,3,4]"u8, JakarExtensionsContext.Default.Int32);
                                                return values.Count == 4 && values.Span[3] == 4;
                                            });
Check("Array ToJson",                       () => new[] { new Invoice { Id = "x" } }.ToJson(false).StartsWith("[{"));
Check("ObservableCollection unfiltered",    () =>
                                            {
                                                ObservableCollection<int> numbers = new(1, 2, 3, 4) { OverrideFilter = static ( int _, ref readonly int v ) => v > 2 };
                                                return numbers.ToJson(false) == "[1,2,3,4]" && ObservableCollection<int>.FromJson("[5,6]").Count == 2;
                                            });

// ─── DOM helpers ─────────────────────────────────────────────────────────────
Check("JsonNode helpers",    () =>
                             {
                                 JsonObject obj = "{\"A\":1}".FromJson().AsObject();
                                 obj.Set("B", "two");
                                 return obj.Get<int>("A") == 1 && obj.Get<string>("B") == "two";
                             });
Check("JsonElement helpers", () =>
                             {
                                 JsonElement element = "{\"A\":1}".FromJson<JsonElement>();
                                 element.Set("B", 2, JakarExtensionsContext.Default.Int32);
                                 return element.Get<int>("B") == 2;
                             });

// ─── Exceptions (no reflection-based serialization) ─────────────────────────
Check("Exception.GetData", () =>
                           {
                               InvalidOperationException e = new("boom") { Data = { ["n"] = 5 } };
                               return e.GetData()["n"]!.GetValue<int>() == 5 && e.GetTags().Entries.Length > 0;
                           });
Check("Error.Create(Exception)", () => Error.Create(new InvalidOperationException("boom")).Description == "boom");

// ─── Converters ──────────────────────────────────────────────────────────────
Check("SerializeAsString (Email)", () => Json.Serialize(new Email("a@b.test")) == "\"a@b.test\"");
Check("Encoding",                  () => Json.Serialize(new LocalFile(Path.Combine(Path.GetTempPath(), "x.txt"))).Contains("FullPath"));

// ─── Other dependencies ──────────────────────────────────────────────────────
Check("OTP QR code (ZXing)", () => OneTimePassword.Create("Jakar").GetContent("ada").StartsWith("otpauth://"));
Check("IPAddress parse",     () => IPAddress.Parse("127.0.0.1").ToString() == "127.0.0.1");


Console.WriteLine(failures == 0
                      ? "All checks passed."
                      : $"{failures} check(s) failed.");

return failures == 0
           ? 0
           : 1;



namespace AotCompatibility.TestApp
{
    [JsonSerializable(typeof(Invoice))]
    public sealed partial class AppJsonContext : JsonSerializerContext;



    [JsonModel(typeof(AppJsonContext))]
    public sealed partial class Invoice
    {
        public string  Id    { get; init; } = "";
        public decimal Total { get; init; }
    }
}
