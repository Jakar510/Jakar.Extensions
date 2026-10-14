// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


/// <summary>
///     A collection that decides what it writes as its JSON array and how it's built from the elements read. <c>[JsonModel]</c> members of such a type use
///     <see cref="JsonCollectionConverter{TCollection,T,TConverter}"/>.
///     <para> For collections whose enumeration isn't their contents: an observable collection writes its unfiltered backing list (a filter is a view concern) and fills itself with one bulk insert (one notification). </para>
/// </summary>
public interface IJsonCollection<TSelf, T>
    where TSelf : IJsonCollection<TSelf, T>
{
    /// <summary> Writes every element with <typeparamref name="TConverter"/>, without the brackets (the converter writes those). </summary>
    void WriteJsonElements<TWriter, TConverter>( ref TWriter writer )
        where TWriter : IJsonWriter, allows ref struct
        where TConverter : IJsonConverter<T>;

    /// <summary> The collection holding <paramref name="elements"/>, in order. The span is pooled scratch space: copy it, don't keep it. </summary>
    static abstract TSelf FromJsonElements( ReadOnlySpan<T> elements );
}



/// <summary> <c>[...]</c> ↔ an <see cref="IJsonCollection{TSelf,T}"/>: written by the collection, read into pooled scratch space and handed over at once. </summary>
public readonly struct JsonCollectionConverter<TCollection, T, TConverter> : IJsonConverter<TCollection>
    where TCollection : IJsonCollection<TCollection, T>
    where TConverter : IJsonConverter<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in TCollection value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null )
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartArray();
        value.WriteJsonElements<TWriter, TConverter>(ref writer);
        writer.WriteEndArray();
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out TCollection value )
        where TReader : IJsonReader, allows ref struct
    {
        value = default;
        JsonPooledList<T> items = default;

        try
        {
            if ( !JsonSequences.TryReadElements<TReader, T, TConverter>(ref reader, ref items) ) { return false; }

            value = TCollection.FromJsonElements(items.Span);
            return true;
        }
        finally { items.Dispose(); }
    }
}
