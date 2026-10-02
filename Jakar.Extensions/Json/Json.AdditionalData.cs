// Jakar.Extensions :: Jakar.Extensions
// 10/02/2026

namespace Jakar.Extensions;


/// <summary> Helpers for the <see cref="IJsonModel.AdditionalData"/> / <see cref="IJsonStringModel.AdditionalData"/> extension-data bags. Keys are case-insensitive. </summary>
public static partial class Json
{
    /// <summary> A new, case-insensitive extension-data bag. </summary>
    public static Dictionary<string, JsonElement> CreateAdditionalData() => new(StringComparer.OrdinalIgnoreCase);


    /// <summary> Copies <paramref name="source"/> into <paramref name="destination"/> (creating it if needed), replacing existing keys. <see cref="JsonElement"/>s are immutable, so nothing is cloned. </summary>
    public static Dictionary<string, JsonElement>? Merge( Dictionary<string, JsonElement>? destination, IReadOnlyDictionary<string, JsonElement>? source )
    {
        if ( source is null || source.Count == 0 ) { return destination; }

        destination ??= CreateAdditionalData();
        foreach ( ( string key, JsonElement value ) in source ) { destination[key] = value; }

        return destination;
    }



    extension( IJsonModel self )
    {
        public Dictionary<string, JsonElement> GetAdditionalData() => self.AdditionalData ??= CreateAdditionalData();

        public bool Contains( string key ) => self.AdditionalData?.ContainsKey(key) is true;


        public bool TryGet<T>( string key, JsonTypeInfo<T> info, out T? value )
        {
            value = default;
            return self.AdditionalData?.TryGetValue(key, out JsonElement element) is true && TryConvert(element, info, out value);
        }

        public bool TryGet<T>( string key, out T? value ) => self.TryGet(key, GetTypeInfo<T>(), out value);

        public T? Get<T>( string key, JsonTypeInfo<T> info ) => self.TryGet(key, info, out T? value)
                                                                    ? value
                                                                    : default;

        public T? Get<T>( string key ) => self.Get(key, GetTypeInfo<T>());

        public JsonElement? Get( string key ) => self.AdditionalData?.TryGetValue(key, out JsonElement element) is true
                                                     ? element
                                                     : null;


        /// <summary> Adds or updates. </summary>
        public void Set<T>( string key, T value, JsonTypeInfo<T> info ) => self.GetAdditionalData()[key] = JsonSerializer.SerializeToElement(value, info);

        public void Set<T>( string key, T value ) => self.Set(key, value, GetTypeInfo<T>());

        public void Set( string key, JsonElement value ) => self.GetAdditionalData()[key] = value;


        public bool Remove( string key ) => self.AdditionalData?.Remove(key) is true;

        public bool Remove( string key, out JsonElement value )
        {
            value = default;
            return self.AdditionalData?.Remove(key, out value) is true;
        }


        public void SetAdditionalData( IReadOnlyDictionary<string, JsonElement>? data ) => self.AdditionalData = Merge(null, data);
    }



    extension( IJsonStringModel self )
    {
        /// <summary> Parses the stored bag; an empty one when there's nothing stored or it isn't a JSON object. </summary>
        [Pure]
        public Dictionary<string, JsonElement> GetAdditionalData()
        {
            Dictionary<string, JsonElement> result = CreateAdditionalData();
            if ( string.IsNullOrWhiteSpace(self.AdditionalData) ) { return result; }

            try
            {
                using JsonDocument document = JsonDocument.Parse(self.AdditionalData, DocumentOptions);
                if ( document.RootElement.ValueKind != JsonValueKind.Object ) { return result; }

                foreach ( JsonProperty property in document.RootElement.EnumerateObject() ) { result[property.Name] = property.Value.Clone(); }
            }
            catch ( JsonException e ) { SelfLogger.WriteLine("Json parsing error: {Error}", e); }

            return result;
        }


        public void SetAdditionalData( IReadOnlyDictionary<string, JsonElement>? data ) => self.AdditionalData = data is null || data.Count == 0
                                                                                                                      ? null
                                                                                                                      : JsonSerializer.Serialize(Merge(null, data), JakarExtensionsContext.Default.DictionaryStringJsonElement);

        public bool Contains( string key ) => self.GetAdditionalData().ContainsKey(key);


        public bool TryGet<T>( string key, JsonTypeInfo<T> info, out T? value )
        {
            value = default;
            return self.GetAdditionalData().TryGetValue(key, out JsonElement element) && TryConvert(element, info, out value);
        }

        public T? Get<T>( string key, JsonTypeInfo<T> info ) => self.TryGet(key, info, out T? value)
                                                                    ? value
                                                                    : default;

        public T? Get<T>( string key ) => self.Get(key, GetTypeInfo<T>());


        public void Set<T>( string key, T value, JsonTypeInfo<T> info )
        {
            Dictionary<string, JsonElement> data = self.GetAdditionalData();
            data[key] = JsonSerializer.SerializeToElement(value, info);
            self.SetAdditionalData(data);
        }

        public void Set<T>( string key, T value ) => self.Set(key, value, GetTypeInfo<T>());


        public bool Remove( string key )
        {
            Dictionary<string, JsonElement> data   = self.GetAdditionalData();
            bool                            result = data.Remove(key);
            if ( result ) { self.SetAdditionalData(data); }

            return result;
        }
    }
}
