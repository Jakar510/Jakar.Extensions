// Jakar.Extensions :: Jakar.Extensions
// 10/02/2026

namespace Jakar.Extensions;


/// <summary> Helpers for the <see cref="IJsonModel.AdditionalData"/> / <see cref="IJsonStringModel.AdditionalData"/> extension-data bags. Keys are ordinal, in insertion order. </summary>
public static partial class Json
{
    /// <summary> Copies <paramref name="source"/> into <paramref name="destination"/> (creating it if needed), replacing existing keys. Values are cloned, so neither bag shares nodes with the other. </summary>
    public static JObjectNode? Merge( this JObjectNode? destination, JObjectNode? source )
    {
        if ( source is null || source.Count == 0 ) { return destination; }

        destination ??= [];
        foreach ( ( string key, JNode? value ) in source ) { destination[key] = value?.DeepClone(); }

        return destination;
    }



    extension( IJsonModel self )
    {
        public JObjectNode GetAdditionalData() => self.AdditionalData ??= [];

        public bool Contains( string key ) => self.AdditionalData?.ContainsKey(key) is true;


        public bool TryGet<T>( string key, [NotNullWhen(true)] out T? value )
            where T : IJsonSerializable<T> => self.AdditionalData.TryGet(key, out value);

        public T? Get<T>( string key )
            where T : IJsonSerializable<T> => self.TryGet(key, out T? value)
                                                  ? value
                                                  : default;

        public JNode? Get( string key ) => self.AdditionalData?.TryGetValue(key, out JNode? node) is true
                                               ? node
                                               : null;


        /// <summary> Adds or updates. </summary>
        public void Set<T>( string key, T value )
            where T : IJsonSerializable<T> => self.GetAdditionalData()[key] = JNode.FromModel(value);

        /// <summary> Adds or updates. A node that already has a parent is cloned. </summary>
        public void Set( string key, JNode? value ) => self.GetAdditionalData()[key] = value.Adopt();


        public bool Remove( string key ) => self.AdditionalData?.Remove(key) is true;

        public bool Remove( string key, out JNode? value )
        {
            value = null;
            return self.AdditionalData?.Remove(key, out value) is true;
        }


        public void SetAdditionalData( JObjectNode? data ) => self.AdditionalData = Merge(null, data);
    }



    extension( IJsonStringModel self )
    {
        /// <summary> Parses the stored bag; an empty one when there's nothing stored or it isn't a JSON object. </summary>
        [Pure] public JObjectNode GetAdditionalData() => self.AdditionalData.TryFromJson() as JObjectNode ?? [];


        public void SetAdditionalData( JObjectNode? data ) => self.AdditionalData = data is null || data.Count == 0
                                                                                         ? null
                                                                                         : data.ToJson(false);

        public bool Contains( string key ) => self.GetAdditionalData().ContainsKey(key);


        public bool TryGet<T>( string key, [NotNullWhen(true)] out T? value )
            where T : IJsonSerializable<T> => self.GetAdditionalData().TryGet(key, out value);

        public T? Get<T>( string key )
            where T : IJsonSerializable<T> => self.TryGet(key, out T? value)
                                                  ? value
                                                  : default;


        public void Set<T>( string key, T value )
            where T : IJsonSerializable<T>
        {
            JObjectNode data = self.GetAdditionalData();
            data[key] = JNode.FromModel(value);
            self.SetAdditionalData(data);
        }


        public bool Remove( string key )
        {
            JObjectNode data   = self.GetAdditionalData();
            bool        result = data.Remove(key);
            if ( result ) { self.SetAdditionalData(data); }

            return result;
        }
    }
}
