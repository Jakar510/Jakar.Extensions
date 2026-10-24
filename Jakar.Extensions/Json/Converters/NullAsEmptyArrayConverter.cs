// Jakar.Extensions :: Jakar.Extensions
// 10/23/2026

namespace Jakar.Extensions;


/// <summary> <c>[...]</c> ↔ <typeparamref name="T"/>[], where a <see langword="null"/> array is written as <c>[]</c> and a JSON <c>null</c> reads as an empty array (e.g. a <see langword="default"/> struct's array fields). </summary>
public readonly struct NullAsEmptyArrayConverter<T, TConverter> : IJsonConverter<T[]>
    where TConverter : IJsonConverter<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in T[] value )
        where TWriter : IJsonWriter, allows ref struct => JsonArrayConverter<T, TConverter>.Write(ref writer, value ?? []);


    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out T[] value )
        where TReader : IJsonReader, allows ref struct
    {
        if ( !reader.TryReadNull() ) { return JsonArrayConverter<T, TConverter>.TryRead(ref reader, out value); }

        value = [];
        return true;
    }
}
