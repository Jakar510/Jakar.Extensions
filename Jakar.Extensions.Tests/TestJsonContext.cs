// Jakar.Extensions :: Jakar.Extensions.Tests
// 10/02/2026

using System.Collections.Generic;
using System.Text.Json.Serialization;



namespace Jakar.Extensions.Tests;


/// <summary> Source-generated metadata for the test models. </summary>
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true,
                             AllowTrailingCommas = true,
                             ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
                             NumberHandling = JsonNumberHandling.AllowReadingFromString,

                             // net10.0: attach the observable-collection converter per closed type (unfiltered contents, bulk insert).
                             Converters = [typeof(ObservableCollectionJsonConverter<int>)])]
[JsonSerializable(typeof(WebRequester_Tests.Item))]
[JsonSerializable(typeof(WebRequester_Tests.JsonItem))]
[JsonSerializable(typeof(List<WebRequester_Tests.Item>))]
[JsonSerializable(typeof(Base64_Tests.SampleRecord))]
[JsonSerializable(typeof(Serialization.TestPreferences))]
[JsonSerializable(typeof(Serialization.Point))]
[JsonSerializable(typeof(List<Serialization.Point>))]
[JsonSerializable(typeof(ObservableCollection<int>))]
[JsonSerializable(typeof(Dictionary<string, int>))]
internal sealed partial class TestJsonContext : JsonSerializerContext;
