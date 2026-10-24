// Jakar.Extensions :: Jakar.Extensions
// 10/02/2026

namespace Jakar.Extensions;


/// <summary>
///     Typed get / add / update / delete helpers for <see cref="JNode"/> trees.
///     <para> <c> TryGet </c> / <c> Get </c> / <c> GetOrDefault </c> / <c> As </c> read <c> [JsonModel] </c> types (<see cref="IJsonSerializable{TSelf}"/>) straight from the tree, with no intermediate text; <c> TryGetValue </c> reads scalars (<see cref="ISpanParsable{TSelf}"/>: numbers, dates, <see cref="Guid"/>, ...). </para>
///     <list type="table">
///         <listheader> <term> Case </term> <description> TryGet / Get / GetOrDefault </description> </listheader>
///         <item> <term> key/index missing, or not an object/array </term> <description> <see langword="false"/> / throws <see cref="KeyNotFoundException"/> / <c> defaultValue </c> </description> </item>
///         <item> <term> present, JSON <c> null </c> </term> <description> <see langword="false"/> / <c> default </c> / <c> defaultValue </c> </description> </item>
///         <item> <term> present, wrong shape </term> <description> <see langword="false"/> / throws <see cref="JsonReadException"/> / <c> defaultValue </c> </description> </item>
///     </list>
///     <para> Writing a node that already has a parent clones it. Not thread-safe. </para>
/// </summary>
public static partial class Json
{
    // ─── Reading ──────────────────────────────────────────────────────────────



    extension( JNode? self )
    {
        public bool Contains( string key ) => self is JObjectNode obj && obj.ContainsKey(key);


        public bool TryGet<T>( string key, [NotNullWhen(true)] out T? value )
            where T : IJsonSerializable<T>
        {
            if ( self is JObjectNode obj && obj.TryGetValue(key, out JNode? node) ) { return TryConvert(node, out value); }

            value = default;
            return false;
        }

        public bool TryGet<T>( int index, [NotNullWhen(true)] out T? value )
            where T : IJsonSerializable<T>
        {
            if ( self is JArrayNode array && (uint)index < (uint)array.Count ) { return TryConvert(array[index], out value); }

            value = default;
            return false;
        }


        /// <exception cref="KeyNotFoundException"> <paramref name="key"/> is missing. </exception>
        /// <exception cref="JsonReadException"> The value has the wrong shape for <typeparamref name="T"/>. </exception>
        public T? Get<T>( string key )
            where T : IJsonSerializable<T> => self is JObjectNode obj && obj.TryGetValue(key, out JNode? node)
                                                  ? node is null
                                                        ? default
                                                        : node.ToModel<T>()
                                                  : throw new KeyNotFoundException(key);


        public T? GetOrDefault<T>( string key, T? defaultValue = default )
            where T : IJsonSerializable<T> => self.TryGet(key, out T? value)
                                                  ? value
                                                  : defaultValue;


        /// <summary> The node itself as <typeparamref name="T"/>. </summary>
        /// <exception cref="JsonReadException"> The node has the wrong shape for <typeparamref name="T"/>. </exception>
        public T? As<T>()
            where T : IJsonSerializable<T> => self is null
                                                  ? default
                                                  : self.ToModel<T>();


        /// <summary> A scalar member (string, number or boolean) parsed as <typeparamref name="T"/>. </summary>
        public bool TryGetValue<T>( string key, [NotNullWhen(true)] out T? value )
            where T : ISpanParsable<T>
        {
            value = default;
            if ( self is not JObjectNode obj || !obj.TryGetValue(key, out JNode? node) || node is not JValueNode scalar ) { return false; }

            try
            {
                value = scalar.GetValue<T>()!;
                return true;
            }
            catch ( Exception e ) when ( e is FormatException or OverflowException or InvalidOperationException )
            {
                value = default;
                return false;
            }
        }
    }



    // ─── Writing (in place) ───────────────────────────────────────────────────



    extension( JObjectNode self )
    {
        /// <summary> <see langword="false"/> if <paramref name="key"/> already exists. </summary>
        public bool TryAdd<T>( string key, T value )
            where T : IJsonSerializable<T> => self.TryAdd(key, JNode.FromModel(value));


        /// <summary> <see langword="false"/> if <paramref name="key"/> is missing. </summary>
        public bool TryUpdate( string key, JNode? value )
        {
            if ( !self.ContainsKey(key) ) { return false; }

            self[key] = value.Adopt();
            return true;
        }

        /// <inheritdoc cref="TryUpdate(JObjectNode, string, JNode?)"/>
        public bool TryUpdate<T>( string key, T value )
            where T : IJsonSerializable<T> => self.TryUpdate(key, JNode.FromModel(value));


        /// <summary> Adds or updates. </summary>
        public void Set<T>( string key, T value )
            where T : IJsonSerializable<T> => self[key] = JNode.FromModel(value);
    }



    extension( JArrayNode self )
    {
        public void Add<T>( T value )
            where T : IJsonSerializable<T> => self.Add(JNode.FromModel(value));
        public void Insert<T>( int index, T value )
            where T : IJsonSerializable<T> => self.Insert(index, JNode.FromModel(value));
        public void Set<T>( int index, T value )
            where T : IJsonSerializable<T> => self[index] = JNode.FromModel(value);
    }



    // ─── Internals ────────────────────────────────────────────────────────────


    /// <summary> Writing a node that already has a parent would throw; clone it instead. </summary>
    private static JNode? Adopt( this JNode? node ) => node is { Parent: not null }
                                                          ? node.DeepClone()
                                                          : node;


    private static bool TryConvert<T>( JNode? node, [NotNullWhen(true)] out T? value )
        where T : IJsonSerializable<T>
    {
        if ( node is null )
        {
            value = default;
            return false;
        }

        try
        {
            value = node.ToModel<T>()!;
            return true;
        }
        catch ( JsonReadException )
        {
            value = default;
            return false;
        }
    }
}
