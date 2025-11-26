// Jakar.Extensions :: Jakar.Extensions
// 11/29/2023  1:49 PM

using Jakar.Extensions.UserGuid;
using Jakar.Extensions.UserLong;
using ZXing.Aztec.Internal;



namespace Jakar.Extensions;


public static class Json
{
    public const  string                 TrimWarning = "Newtonsoft.Json relies on reflection over types that may be removed when trimming.";
    public const  string                 AotWarning  = "Newtonsoft.Json relies on dynamically creating types that may not be available with Ahead of Time compilation.";
    public static JsonSerializerSettings Settings     { get; set; } = new();
    public static JsonLoadSettings       LoadSettings { get; set; } = new();



    extension( PropertyInfo self )
    {
        public bool GetJsonIsRequired() => self.GetCustomAttribute<JsonRequiredAttribute>() is not null;
        public string GetJsonKey() => self.GetCustomAttribute<JsonPropertyAttribute>()
                                         ?.PropertyName ??
                                      self.Name;
    }



    extension( string self )
    {
        public JToken FromJson()         => ThrowIfNull(JToken.Parse(self));
        public TValue FromJson<TValue>() => ThrowIfNull(JsonConvert.DeserializeObject<TValue>(self, Settings));

        public JObject? GetAdditionalData()
        {
            if ( string.IsNullOrWhiteSpace(self) ) { return null; }

            return JObject.Parse(self);
        }
    }



    public static bool Contains( this IJsonModel self, string key ) => self.AdditionalData?.ContainsKey(key) ?? false;
    public static bool Contains( this IJsonStringModel self, string key ) => self.GetAdditionalData()
                                                                                 .ContainsKey(key);



    extension( IJsonModel self )
    {
        public bool Remove( string key )
        {
            JObject dict = self.GetAdditionalData();
            return dict.Remove(key);
        }
        public bool Remove( string key, out JToken? value )
        {
            JObject dict = self.GetAdditionalData();
            dict.TryGetValue(key, out value);
            return dict.Remove(key);
        }
    }



    extension( IJsonStringModel self )
    {
        public bool Remove( string key )
        {
            JObject additionalData = self.GetAdditionalData();
            bool    result         = additionalData.Remove(key);
            self.SetAdditionalData(additionalData);
            return result;
        }
        public bool Remove( string key, out JToken? value )
        {
            JObject dict = self.GetAdditionalData();
            dict.TryGetValue(key, out value);
            bool result = dict.Remove(key);
            self.SetAdditionalData(dict);
            return result;
        }

        [Pure] public JObject GetAdditionalData() => self.AdditionalData?.GetAdditionalData() ?? new JObject();
    }



    extension( IJsonModel model )
    {
        public JObject GetAdditionalData() => model.AdditionalData ??= new();
        public JToken? Get( string key )   => model.AdditionalData?[key];
    }



    public static JToken? Get( this IJsonStringModel self, string key ) => self.GetAdditionalData()[key];



    extension( IJsonModel self )
    {
        public TValue? Get<TValue>( string key )
        {
            JToken? token = self.Get(key);
            if ( token is null ) { return default; }

            return token.ToObject<TValue>();
        }
    }



    extension( IJsonStringModel self )
    {
        public TValue? Get<TValue>( string key )
        {
            JToken? token = self.Get(key);
            if ( token is null ) { return default; }

            return token.ToObject<TValue>();
        }
    }



    extension( IJsonModel self )
    {
        public void Add( string key, bool value )
        {
            JToken element = value;
            self.Add(key, element);
        }
        public void Add( string key, byte value )
        {
            JToken element = value;
            self.Add(key, element);
        }
        public void Add( string key, short value )
        {
            JToken element = value;
            self.Add(key, element);
        }
        public void Add( string key, int value )
        {
            JToken element = value;
            self.Add(key, element);
        }
        public void Add( string key, long value )
        {
            JToken element = value;
            self.Add(key, element);
        }
        public void Add( string key, float value )
        {
            JToken element = value;
            self.Add(key, element);
        }
        public void Add( string key, double value )
        {
            JToken element = value;
            self.Add(key, element);
        }
        public void Add( string key, decimal value )
        {
            JToken element = value;
            self.Add(key, element);
        }
        public void Add( string key, string value )
        {
            JToken element = value;
            self.Add(key, element);
        }
        public void Add( string key, DateTime value )
        {
            JToken element = value;
            self.Add(key, element);
        }
        public void Add( string key, DateTimeOffset value )
        {
            JToken element = value;
            self.Add(key, element);
        }
        public void Add( string key, TimeSpan value )
        {
            JToken? element = value;
            self.Add(key, element);
        }
        public void Add<TValue>( string key, TValue value )
        {
            JsonSerializer     jsonSerializer = JsonSerializer.Create(Settings);
            using JTokenWriter jsonWriter     = new();

            jsonSerializer.Serialize(jsonWriter, value);
            JToken element = jsonWriter.Token!;
            self.Add(key, element);
        }
        public void Add( string                 key, JToken? element ) => self.GetAdditionalData()[key] = element;
        public void SetAdditionalData( JObject? data ) => self.AdditionalData = data;
    }



    public static JToken? ToToken<TValue>( [NotNullIfNotNull(nameof(value))] this TValue? value )
    {
        JsonSerializer     jsonSerializer = JsonSerializer.Create(Settings);
        using JTokenWriter jsonWriter     = new();

        jsonSerializer.Serialize(jsonWriter, value);
        JToken element = jsonWriter.Token!;
        return element;
    }


    public static void SetAdditionalData( this IJsonStringModel model, JObject? data ) => model.AdditionalData = data?.ToJson();


    public static string ToJson( this         JToken value ) => value.ToString(Formatting.Indented);
    public static string ToJson<TValue>( this TValue value ) => JsonConvert.SerializeObject(value, Formatting.Indented);



    extension( Stream self )
    {
        public async ValueTask<JToken> FromJson( CancellationToken token = default )
        {
            JsonLoadSettings loadSettings = LoadSettings;

            // StreamReader and JsonTextReader do not implement IAsyncDisposable so let the caller dispose the stream.
            using StreamReader textReader = new(self, leaveOpen: true);

            await using JsonTextReader reader = new(textReader) { CloseInput = false };

            JToken jToken = await JToken.LoadAsync(reader, loadSettings, token)
                                        .ConfigureAwait(false);

            return jToken;
        }

        public async ValueTask<T> FromJson<T>( CancellationToken token = default )
        {
            JsonLoadSettings loadSettings = LoadSettings;
            JsonSerializer   serializer   = JsonSerializer.Create(Settings);

            // StreamReader and JsonTextReader do not implement IAsyncDisposable so let the caller dispose the stream.
            using StreamReader         textReader = new(self, leaveOpen: true);
            await using JsonTextReader reader     = new(textReader) { CloseInput = false };

            JToken jToken = await JToken.LoadAsync(reader, loadSettings, token)
                                        .ConfigureAwait(false);

            return ThrowIfNull(jToken.ToObject<T>(serializer));
        }


        /// <summary> Asynchronously load and synchronously deserialize values from a stream containing a JSON array.  The root object of the JSON stream must in fact be an array, or an exception is thrown </summary>
        public async IAsyncEnumerable<T?> FromJsonAsync<T>( [EnumeratorCancellation] CancellationToken token = default )
        {
            JsonLoadSettings loadSettings = LoadSettings;
            JsonSerializer   serializer   = JsonSerializer.Create(Settings);

            // StreamReader and JsonTextReader do not implement IAsyncDisposable so let the caller dispose the stream.
            using StreamReader textReader = new(self, leaveOpen: true);

            await using ( JsonTextReader reader = new(textReader) { CloseInput = false } )
            {
                await foreach ( JToken jToken in reader.LoadAsyncEnumerable(loadSettings, token)
                                                       .ConfigureAwait(false) ) { yield return jToken.ToObject<T>(serializer); }
            }
        }

        /// <summary> Asynchronously load and return JToken values from a stream containing a JSON array.  The root object of the JSON stream must in fact be an array, or an exception is thrown </summary>
        public async IAsyncEnumerable<JToken> FromJsonAsync( [EnumeratorCancellation] CancellationToken token = default )
        {
            JsonLoadSettings loadSettings = LoadSettings;

            // StreamReader and JsonTextReader do not implement IAsyncDisposable so let the caller dispose the stream.
            using StreamReader textReader = new(self, leaveOpen: true);

            await using ( JsonTextReader reader = new(textReader) { CloseInput = false } )
            {
                await foreach ( JToken jToken in reader.LoadAsyncEnumerable(loadSettings, token)
                                                       .ConfigureAwait(false) ) { yield return jToken; }
            }
        }
    }



    /// <summary> Asynchronously load and return JToken values from a stream containing a JSON array. The root object of the JSON stream must in fact be an array, or an exception is thrown </summary>
    private static async IAsyncEnumerable<JToken> LoadAsyncEnumerable( this JsonTextReader reader, JsonLoadSettings loadSettings, [EnumeratorCancellation] CancellationToken token = default )
    {
        ( await reader.MoveToContentAndAssertAsync(token)
                      .ConfigureAwait(false) ).AssertTokenType(JsonToken.StartArray);

        token.ThrowIfCancellationRequested();

        while ( ( await reader.ReadToContentAndAssert(token)
                              .ConfigureAwait(false) ).TokenType !=
                JsonToken.EndArray )
        {
            token.ThrowIfCancellationRequested();

            yield return await JToken.LoadAsync(reader, loadSettings, token)
                                     .ConfigureAwait(false);
        }

        token.ThrowIfCancellationRequested();
    }



    extension( JsonReader self )
    {
        public JsonReader AssertTokenType( JsonToken tokenType ) => self.TokenType == tokenType
                                                                        ? self
                                                                        : throw new JsonSerializationException($"Unexpected token {self.TokenType}, expected {tokenType}");

        public async ValueTask<JsonReader> ReadToContentAndAssert( CancellationToken token = default ) =>
            await ( await self.ReadAndAssertAsync(token)
                              .ConfigureAwait(false) ).MoveToContentAndAssertAsync(token)
                                                      .ConfigureAwait(false);


        public async ValueTask<JsonReader> MoveToContentAndAssertAsync( CancellationToken token = default )
        {
            if ( self.TokenType is JsonToken.None ) // Skip past beginning of stream.
                await self.ReadAndAssertAsync(token)
                          .ConfigureAwait(false);

            while ( self.TokenType is JsonToken.Comment ) // Skip past comments.
                await self.ReadAndAssertAsync(token)
                          .ConfigureAwait(false);

            return self;
        }
        public async ValueTask<JsonReader> ReadAndAssertAsync( CancellationToken token = default )
        {
            if ( !await self.ReadAsync(token)
                            .ConfigureAwait(false) ) { throw new JsonReaderException("Unexpected end of JSON stream."); }

            return self;
        }
    }



    public static string ToJson<TValue>( this scoped in ReadOnlySpan<TValue> values )
        
    {
        TValue[] array = ArrayPool<TValue>.Shared.Rent(values.Length);

        try
        {
            values.CopyTo(array);
            return array.ToJson();
        }
        finally { ArrayPool<TValue>.Shared.Return(array); }
    }
}



public interface IJsonModel
{
    [Newtonsoft.Json.JsonExtensionData] public JObject? AdditionalData { get; set; }
}



public interface IJsonStringModel
{
    public string? AdditionalData { get; set; }
}
