// Jakar.Json
// 10/07/2026

namespace Jakar.Json.Converters;


/// <summary> Shared array reading and writing. Reads collect elements in pooled scratch space; writes take a span whenever the collection has one, so nothing is boxed. </summary>
public static class JsonSequences
{
    public static void WriteSpan<TWriter, T, TConverter>( ref TWriter writer, scoped ReadOnlySpan<T> items )
        where TWriter : IJsonWriter, allows ref struct
        where TConverter : IJsonConverter<T>
    {
        writer.WriteStartArray();
        foreach ( ref readonly T item in items ) { TConverter.Write(ref writer, in item); }

        writer.WriteEndArray();
    }

    /// <summary> Arrays and lists are written from their storage; anything else is enumerated (interfaces box their enumerator: one small allocation). </summary>
    public static void WriteEnumerable<TWriter, T, TConverter>( ref TWriter writer, IEnumerable<T> items )
        where TWriter : IJsonWriter, allows ref struct
        where TConverter : IJsonConverter<T>
    {
        switch ( items )
        {
            case T[] array:
                WriteSpan<TWriter, T, TConverter>(ref writer, array);
                return;

            case List<T> list:
                WriteSpan<TWriter, T, TConverter>(ref writer, CollectionsMarshal.AsSpan(list));
                return;
        }

        writer.WriteStartArray();
        foreach ( T item in items ) { TConverter.Write(ref writer, in item); }

        writer.WriteEndArray();
    }

    /// <summary> An unordered collection: sorted first when <typeparamref name="TOrder"/> says so (SPEC.md §5.2), through a pooled copy. </summary>
    public static void WriteUnordered<TWriter, T, TConverter, TOrder>( ref TWriter writer, IEnumerable<T> items, int count )
        where TWriter : IJsonWriter, allows ref struct
        where TConverter : IJsonConverter<T>
        where TOrder : IJsonOrder<T>
    {
        if ( !TOrder.Sorted || count < 2 )
        {
            WriteEnumerable<TWriter, T, TConverter>(ref writer, items);
            return;
        }

        JsonPooledList<T> sorted = new(count);

        try
        {
            if ( items is HashSet<T> set )
            {
                foreach ( T item in set ) { sorted.Add(item); }
            }
            else
            {
                foreach ( T item in items ) { sorted.Add(item); }
            }

            sorted.Items.Sort(default(OrderComparer<T, TOrder>));
            WriteSpan<TWriter, T, TConverter>(ref writer, sorted.Span);
        }
        finally { sorted.Dispose(); }
    }


    /// <summary> Reads <c>[...]</c> into <paramref name="items"/>. </summary>
    internal static bool TryReadElements<TReader, T, TConverter>( ref TReader reader, ref JsonPooledList<T> items )
        where TReader : IJsonReader, allows ref struct
        where TConverter : IJsonConverter<T>
    {
        if ( !reader.TryReadStartArray() ) { return false; }

        while ( true )
        {
            if ( !reader.TryReadNextElement(out bool end) ) { return false; }

            if ( end ) { return true; }

            if ( !TConverter.TryRead(ref reader, out T? item) ) { return false; }

            items.Add(item!);
        }
    }

    public static List<T> ToList<T>( scoped ReadOnlySpan<T> items )
    {
        List<T> list = new(items.Length);
        CollectionsMarshal.SetCount(list, items.Length);
        items.CopyTo(CollectionsMarshal.AsSpan(list));
        return list;
    }
}



// ─── Concrete sequences ──────────────────────────────────────────────────────



public readonly struct JsonArrayConverter<T, TConverter> : IJsonConverter<T[]>
    where TConverter : IJsonConverter<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in T[] value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); }
        else { JsonSequences.WriteSpan<TWriter, T, TConverter>(ref writer, value); }
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out T[] value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        JsonPooledList<T> items = default;

        try
        {
            if ( !JsonSequences.TryReadElements<TReader, T, TConverter>(ref reader, ref items) ) { return false; }

            value = items.ToArray();
            return true;
        }
        finally { items.Dispose(); }
    }
}



public readonly struct JsonListConverter<T, TConverter> : IJsonConverter<List<T>>
    where TConverter : IJsonConverter<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in List<T> value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); }
        else { JsonSequences.WriteSpan<TWriter, T, TConverter>(ref writer, CollectionsMarshal.AsSpan(value)); }
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out List<T> value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        JsonPooledList<T> items = default;

        try
        {
            if ( !JsonSequences.TryReadElements<TReader, T, TConverter>(ref reader, ref items) ) { return false; }

            value = JsonSequences.ToList(items.Span);
            return true;
        }
        finally { items.Dispose(); }
    }
}



/// <summary> <see cref="IEnumerable{T}"/>, <see cref="ICollection{T}"/>, <see cref="IList{T}"/>, <see cref="IReadOnlyCollection{T}"/> and <see cref="IReadOnlyList{T}"/> members: written from whatever they hold, read as a <see cref="List{T}"/>. </summary>
public readonly struct JsonEnumerableConverter<TCollection, T, TConverter> : IJsonConverter<TCollection>
    where TCollection : class, IEnumerable<T>
    where TConverter : IJsonConverter<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in TCollection value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); }
        else { JsonSequences.WriteEnumerable<TWriter, T, TConverter>(ref writer, value); }
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out TCollection value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        JsonPooledList<T> items = default;

        try
        {
            if ( !JsonSequences.TryReadElements<TReader, T, TConverter>(ref reader, ref items) ) { return false; }

            value = (TCollection)(object)JsonSequences.ToList(items.Span); // List<T> implements every interface this converter is used for
            return true;
        }
        finally { items.Dispose(); }
    }
}



public readonly struct JsonImmutableArrayConverter<T, TConverter> : IJsonConverter<ImmutableArray<T>>
    where TConverter : IJsonConverter<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in ImmutableArray<T> value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value.IsDefault ) { writer.WriteNull(); }
        else { JsonSequences.WriteSpan<TWriter, T, TConverter>(ref writer, value.AsSpan()); }
    }

    public static bool TryRead<TReader>( ref TReader reader, out ImmutableArray<T> value )
        where TReader : IJsonReader, allows ref struct
    {
        value = default;
        if ( reader.TryReadNull() ) { return true; }

        JsonPooledList<T> items = default;

        try
        {
            if ( !JsonSequences.TryReadElements<TReader, T, TConverter>(ref reader, ref items) ) { return false; }

            value = ImmutableCollectionsMarshal.AsImmutableArray(items.ToArray());
            return true;
        }
        finally { items.Dispose(); }
    }
}



public readonly struct JsonImmutableListConverter<T, TConverter> : IJsonConverter<ImmutableList<T>>
    where TConverter : IJsonConverter<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in ImmutableList<T> value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null )
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartArray();
        foreach ( T item in value ) { TConverter.Write(ref writer, in item); } // struct enumerator

        writer.WriteEndArray();
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out ImmutableList<T> value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        JsonPooledList<T> items = default;

        try
        {
            if ( !JsonSequences.TryReadElements<TReader, T, TConverter>(ref reader, ref items) ) { return false; }

            value = ImmutableList.Create(items.Span);
            return true;
        }
        finally { items.Dispose(); }
    }
}



/// <summary> <see cref="HashSet{T}"/>; also <see cref="ISet{T}"/> and <see cref="IReadOnlySet{T}"/> members, read as a <see cref="HashSet{T}"/>. </summary>
public readonly struct JsonHashSetConverter<TSet, T, TConverter, TOrder> : IJsonConverter<TSet>
    where TSet : class, IEnumerable<T>
    where TConverter : IJsonConverter<T>
    where TOrder : IJsonOrder<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in TSet value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); }
        else
        {
            JsonSequences.WriteUnordered<TWriter, T, TConverter, TOrder>(ref writer,
                                                                         value,
                                                                         value is IReadOnlyCollection<T> c
                                                                             ? c.Count
                                                                             : 16);
        }
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out TSet value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        JsonPooledList<T> items = default;

        try
        {
            if ( !JsonSequences.TryReadElements<TReader, T, TConverter>(ref reader, ref items) ) { return false; }

            HashSet<T> set = new(items.Count);
            foreach ( T item in items.Span ) { set.Add(item); }

            value = (TSet)(object)set;
            return true;
        }
        finally { items.Dispose(); }
    }
}



public readonly struct JsonFrozenSetConverter<T, TConverter, TOrder> : IJsonConverter<FrozenSet<T>>
    where TConverter : IJsonConverter<T>
    where TOrder : IJsonOrder<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in FrozenSet<T> value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null ) { writer.WriteNull(); }
        else if ( TOrder.Sorted ) { JsonSequences.WriteUnordered<TWriter, T, TConverter, TOrder>(ref writer, value, value.Count); }
        else { JsonSequences.WriteSpan<TWriter, T, TConverter>(ref writer, value.Items.AsSpan()); }
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out FrozenSet<T> value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        JsonPooledList<T> items = default;

        try
        {
            if ( !JsonSequences.TryReadElements<TReader, T, TConverter>(ref reader, ref items) ) { return false; }

            value = FrozenSet.Create(items.Span);
            return true;
        }
        finally { items.Dispose(); }
    }
}



/// <summary> Written in its own order. </summary>
public readonly struct JsonSortedSetConverter<T, TConverter> : IJsonConverter<SortedSet<T>>
    where TConverter : IJsonConverter<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in SortedSet<T> value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null )
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartArray();
        foreach ( T item in value ) { TConverter.Write(ref writer, in item); }

        writer.WriteEndArray();
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out SortedSet<T> value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        JsonPooledList<T> items = default;

        try
        {
            if ( !JsonSequences.TryReadElements<TReader, T, TConverter>(ref reader, ref items) ) { return false; }

            value = [];
            foreach ( T item in items.Span ) { value.Add(item); }

            return true;
        }
        finally { items.Dispose(); }
    }
}



public readonly struct JsonQueueConverter<T, TConverter> : IJsonConverter<Queue<T>>
    where TConverter : IJsonConverter<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in Queue<T> value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null )
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartArray();
        foreach ( T item in value ) { TConverter.Write(ref writer, in item); }

        writer.WriteEndArray();
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out Queue<T> value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        JsonPooledList<T> items = default;

        try
        {
            if ( !JsonSequences.TryReadElements<TReader, T, TConverter>(ref reader, ref items) ) { return false; }

            value = new Queue<T>(items.Count);
            foreach ( T item in items.Span ) { value.Enqueue(item); }

            return true;
        }
        finally { items.Dispose(); }
    }
}



/// <summary> Written top first (enumeration order); read back by pushing in reverse, so the stack round-trips. </summary>
public readonly struct JsonStackConverter<T, TConverter> : IJsonConverter<Stack<T>>
    where TConverter : IJsonConverter<T>
{
    public static void Write<TWriter>( ref TWriter writer, scoped in Stack<T> value )
        where TWriter : IJsonWriter, allows ref struct
    {
        if ( value is null )
        {
            writer.WriteNull();
            return;
        }

        writer.WriteStartArray();
        foreach ( T item in value ) { TConverter.Write(ref writer, in item); }

        writer.WriteEndArray();
    }

    public static bool TryRead<TReader>( ref TReader reader, [MaybeNullWhen(false)] out Stack<T> value )
        where TReader : IJsonReader, allows ref struct
    {
        value = null;
        JsonPooledList<T> items = default;

        try
        {
            if ( !JsonSequences.TryReadElements<TReader, T, TConverter>(ref reader, ref items) ) { return false; }

            ReadOnlySpan<T> span = items.Span;
            value = new Stack<T>(span.Length);
            for ( int i = span.Length - 1; i >= 0; i-- ) { value.Push(span[i]); }

            return true;
        }
        finally { items.Dispose(); }
    }
}
