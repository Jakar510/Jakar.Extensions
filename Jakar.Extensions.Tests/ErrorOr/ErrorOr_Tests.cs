// Jakar.Extensions :: Jakar.Extensions.Tests
// Regression coverage for the 10.9.1 defects:
//   1) ErrorOrResult<TValue>.HasErrors was `Error?.IsValid is true && Value is null`, so for a
//      value-type TValue ( default(TValue) is never null ) every factory-built error reported
//      "no errors" and silently defeated validation.
//   2) Error's nullable RFC 7807 members carried Newtonsoft's [JsonRequired] ( Required.Always ),
//      which refuses to WRITE a null - so Error.Validation( description:, title: ) could not serialize.
//   3) LoginRequest could not be constructed by System.Text.Json ( Minimal API body binding ).

using Newtonsoft.Json;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(ErrorOrResult<>))]
public class ErrorOr_Tests : Assert
{
    private const string DESCRIPTION = "must be 5 characters or fewer";
    private const string FIELD       = "notes";
    private const string TITLE       = "Validation";


    private static Error CreateValidationError() => Error.Validation(description: DESCRIPTION, title: TITLE);


    // ─── Errors container ────────────────────────────────────────────────────

    [Test]
    public void FactoryError_ProducesValidErrors()
    {
        Errors errors = CreateValidationError();
        this.IsTrue(errors.IsValid);
        this.AreEqual(1, errors.Details.Length);
    }


    // ─── ErrorOrResult<TValue> : value-type TValue is the regression ──────────

    [Test]
    public void Generic_ValueType_Bool_ReportsErrors()
    {
        ErrorOrResult<bool> result = CreateValidationError();
        this.IsTrue(result.HasErrors);
        this.IsFalse(result.HasValue);
        this.IsFalse(result.TryGetValue(out bool _, out Errors? errors));
        this.NotNull(errors);
        this.IsTrue(errors!.IsValid);
    }

    [Test]
    public void Generic_ValueType_Int_ReportsErrors()
    {
        ErrorOrResult<int> result = CreateValidationError();
        this.IsTrue(result.HasErrors);
        this.IsFalse(result.HasValue);
        this.IsFalse(result.TryGetValue(out int _, out Errors? errors));
        this.NotNull(errors);
    }

    [Test]
    public void Generic_ValueType_Struct_ReportsErrors()
    {
        ErrorOrResult<Success> result = CreateValidationError();
        this.IsTrue(result.HasErrors);
        this.IsFalse(result.HasValue);
        this.IsFalse(result.TryGetValue(out Success _, out Errors? errors));
        this.NotNull(errors);
    }

    [Test]
    public void Generic_ReferenceType_ReportsErrors()
    {
        ErrorOrResult<string> result = CreateValidationError();
        this.IsTrue(result.HasErrors);
        this.IsFalse(result.HasValue);
        this.IsFalse(result.TryGetValue(out string? _, out Errors? errors));
        this.NotNull(errors);
    }

    [Test]
    public void Generic_ValuePath_ValueType()
    {
        ErrorOrResult<int> result = ErrorOrResult<int>.Create(7);
        this.IsFalse(result.HasErrors);
        this.IsTrue(result.HasValue);
        this.IsTrue(result.TryGetValue(out int value, out Errors? errors));
        this.AreEqual(7, value);
        this.IsNull(errors);
    }

    [Test]
    public void Generic_ValuePath_ValueType_DefaultValueStillCountsAsValue()
    {
        ErrorOrResult<int> result = ErrorOrResult<int>.Create(0);
        this.IsFalse(result.HasErrors);
        this.IsTrue(result.HasValue);
        this.IsTrue(result.TryGetValue(out int value, out Errors? _));
        this.AreEqual(0, value);
    }

    [Test]
    public void Generic_ValuePath_ReferenceType()
    {
        ErrorOrResult<string> result = ErrorOrResult<string>.Create("ok");
        this.IsFalse(result.HasErrors);
        this.IsTrue(result.HasValue);
        this.IsTrue(result.TryGetValue(out string? value, out Errors? errors));
        this.AreEqual("ok", value);
        this.IsNull(errors);
    }

    [Test]
    public void Generic_ErrorsOnlyOverload()
    {
        ErrorOrResult<bool> failed = CreateValidationError();
        this.IsTrue(failed.TryGetValue(out Errors? errors));
        this.NotNull(errors);

        ErrorOrResult<bool> passed = ErrorOrResult<bool>.Create(true);
        this.IsFalse(passed.TryGetValue(out Errors? none));
        this.IsFalse(none?.IsValid is true);
    }

    [Test]
    public void Generic_Match_TakesErrorBranch()
    {
        ErrorOrResult<int> result = CreateValidationError();
        this.AreEqual("error", result.Match(static ( int _ ) => "value", static ( Errors _ ) => "error"));
    }

    [Test]
    public void Generic_GetStatus_IsBadRequestForValidation()
    {
        ErrorOrResult<bool> result = CreateValidationError();
        this.AreEqual(Status.BadRequest, result.GetStatus());
    }


    // ─── ErrorOrResult ( non-generic ) ────────────────────────────────────────

    [Test]
    public void NonGeneric_ReportsErrors()
    {
        ErrorOrResult result = CreateValidationError();
        this.IsTrue(result.HasErrors);
        this.IsFalse(result.HasValue);
        this.IsFalse(result.Passed);
        this.IsFalse(result.TryGetValue(out bool? _, out Errors? errors));
        this.NotNull(errors);
        this.IsTrue(result.TryGetValue(out Errors? only));
        this.NotNull(only);
    }

    [Test]
    public void NonGeneric_ValuePath()
    {
        ErrorOrResult result = ErrorOrResult.Create(true);
        this.IsFalse(result.HasErrors);
        this.IsTrue(result.Passed);
        this.IsTrue(result.TryGetValue(out bool? value, out Errors? errors));
        this.IsTrue(value is true);
        this.IsNull(errors);
    }

    [Test]
    public void NonGeneric_Deconstruct_KeepsErrors()
    {
        ErrorOrResult result = CreateValidationError();
        result.Deconstruct(out bool passed, out Errors? errors);
        this.IsFalse(passed);
        this.NotNull(errors);
    }


    // ─── Serialization : Newtonsoft ( the path Jakar's ToJson uses ) ──────────

    [Test]
    public void Error_NewtonsoftRoundTrip()
    {
        Error  error = CreateValidationError();
        string json  = JsonConvert.SerializeObject(error);
        this.NotNull(json);

        Error? back = JsonConvert.DeserializeObject<Error>(json);
        this.NotNull(back);
        this.AreEqual(DESCRIPTION,        back!.Description);
        this.AreEqual(TITLE,              back.Title);
        this.AreEqual(Status.BadRequest,  back.StatusCode);
        this.IsNull(back.Instance);
    }

    [Test]
    public void Errors_NewtonsoftRoundTrip()
    {
        Errors errors = CreateValidationError();
        string json   = errors.ToJson();
        this.NotNull(json);

        Errors? back = JsonConvert.DeserializeObject<Errors>(json);
        this.NotNull(back);
        this.IsTrue(back!.IsValid);
        this.AreEqual(1, back.Details.Length);
    }

    [Test]
    public void Errors_WithNullAlert_NewtonsoftSerializes()
    {
        Errors errors = Errors.Create(CreateValidationError(), CreateValidationError());
        this.IsNull(errors.Alert);
        string json = errors.ToJson();
        this.NotNull(json);
    }

    [Test]
    public void Error_SystemTextJsonRoundTrip()
    {
        Error  error = CreateValidationError();
        string json  = System.Text.Json.JsonSerializer.Serialize(error);
        this.NotNull(json);

        Error? back = System.Text.Json.JsonSerializer.Deserialize<Error>(json);
        this.NotNull(back);
        this.AreEqual(DESCRIPTION,       back!.Description);
        this.AreEqual(TITLE,             back.Title);
        this.AreEqual(Status.BadRequest, back.StatusCode);
    }


    // ─── StringTags / Pair carry their payload in public fields, which System.Text.Json
    //     ignores unless it is handed an explicit contract. Without the converters these
    //     assertions fail silently: Details survives as an empty bag. ──────────────────

    private static StringTags CreateTags() => new([new Pair(FIELD, "notes"), new Pair("nullable", null)], [DESCRIPTION, "required"]);


    [Test]
    public void StringTags_SystemTextJsonRoundTrip()
    {
        StringTags tags = CreateTags();
        string     json = System.Text.Json.JsonSerializer.Serialize(tags);
        this.NotNull(json);

        StringTags back = System.Text.Json.JsonSerializer.Deserialize<StringTags>(json);
        this.AreEqual(2,           back.Tags.Length);
        this.AreEqual(2,           back.Entries.Length);
        this.AreEqual(FIELD,       back.Tags[0].Key);
        this.AreEqual("notes",     back.Tags[0].Value);
        this.IsNull(back.Tags[1].Value);
        this.AreEqual(DESCRIPTION, back.Entries[0]);
        this.IsFalse(back.IsEmpty);
    }

    [Test]
    public void StringTags_SystemTextJsonRoundTrip_WebDefaults()
    {
        // Minimal API uses JsonSerializerDefaults.Web ( camelCase ) - the converter must follow the naming policy.
        System.Text.Json.JsonSerializerOptions options = new(System.Text.Json.JsonSerializerDefaults.Web);

        StringTags tags = CreateTags();
        string     json = System.Text.Json.JsonSerializer.Serialize(tags, options);
        this.IsTrue(json.Contains("\"tags\"", StringComparison.Ordinal));

        StringTags back = System.Text.Json.JsonSerializer.Deserialize<StringTags>(json, options);
        this.AreEqual(2,       back.Tags.Length);
        this.AreEqual(FIELD,   back.Tags[0].Key);
        this.AreEqual("notes", back.Tags[0].Value);
    }

    [Test]
    public void StringTags_SystemTextJsonReadsNewtonsoftShape()
    {
        StringTags back = System.Text.Json.JsonSerializer.Deserialize<StringTags>(CreateTags().ToJson());
        this.AreEqual(2,     back.Tags.Length);
        this.AreEqual(2,     back.Entries.Length);
        this.AreEqual(FIELD, back.Tags[0].Key);
    }

    [Test]
    public void StringTags_SystemTextJsonToleratesMissingAndNullMembers()
    {
        this.IsTrue(System.Text.Json.JsonSerializer.Deserialize<StringTags>("{}").IsEmpty);
        this.IsTrue(System.Text.Json.JsonSerializer.Deserialize<StringTags>("""{"Tags":null,"Entries":null}""").IsEmpty);
        this.AreEqual(1, System.Text.Json.JsonSerializer.Deserialize<StringTags>("""{"Unknown":{"nested":[1,2]},"Entries":["x"]}""").Entries.Length);
    }

    [Test]
    public void Error_SystemTextJsonKeepsDetails()
    {
        Error  error = Error.Validation(CreateTags(), description: DESCRIPTION, title: TITLE);
        string json  = System.Text.Json.JsonSerializer.Serialize(error);

        Error? back = System.Text.Json.JsonSerializer.Deserialize<Error>(json);
        this.NotNull(back);
        this.AreEqual(2,     back!.Details.Tags.Length);
        this.AreEqual(2,     back.Details.Entries.Length);
        this.AreEqual(FIELD, back.Details.Tags[0].Key);
    }

    [Test]
    public void Errors_SystemTextJsonRoundTrip()
    {
        Errors errors = Error.Validation(CreateTags(), description: DESCRIPTION, title: TITLE);
        string json   = System.Text.Json.JsonSerializer.Serialize(errors);
        this.NotNull(json);

        Errors? back = System.Text.Json.JsonSerializer.Deserialize<Errors>(json);
        this.NotNull(back);
        this.IsTrue(back!.IsValid);
        this.AreEqual(1,     back.Details.Length);
        this.AreEqual(FIELD, back.Details[0].Details.Tags[0].Key);
    }

    [Test]
    public void Error_NewtonsoftKeepsDetails()
    {
        Error  error = Error.Validation(CreateTags(), description: DESCRIPTION, title: TITLE);
        Error? back  = JsonConvert.DeserializeObject<Error>(JsonConvert.SerializeObject(error));
        this.NotNull(back);
        this.AreEqual(2,     back!.Details.Tags.Length);
        this.AreEqual(FIELD, back.Details.Tags[0].Key);
    }


    // ─── LoginRequest must survive System.Text.Json model binding ─────────────

    [Test]
    public void LoginRequest_SystemTextJsonRoundTrip()
    {
        LoginRequest request = new("tyler", "hunter2");
        string       json    = System.Text.Json.JsonSerializer.Serialize(request);
        this.NotNull(json);

        LoginRequest? back = System.Text.Json.JsonSerializer.Deserialize<LoginRequest>(json);
        this.NotNull(back);
        this.AreEqual("tyler",   back!.UserLogin);
        this.AreEqual("hunter2", back.UserPassword);
    }

    [Test]
    public void LoginRequest_SystemTextJsonBindsRawBody()
    {
        const string BODY = """{"UserLogin":"tyler","UserPassword":"hunter2"}""";

        LoginRequest? back = System.Text.Json.JsonSerializer.Deserialize<LoginRequest>(BODY);
        this.NotNull(back);
        this.AreEqual("tyler",   back!.UserLogin);
        this.AreEqual("hunter2", back.UserPassword);
    }

    [Test]
    public void LoginRequest_NewtonsoftRoundTripStillWorks()
    {
        LoginRequest  request = new("tyler", "hunter2");
        string        json    = JsonConvert.SerializeObject(request);
        LoginRequest? back    = JsonConvert.DeserializeObject<LoginRequest>(json);
        this.NotNull(back);
        this.AreEqual("tyler",   back!.UserLogin);
        this.AreEqual("hunter2", back.UserPassword);
    }
}
