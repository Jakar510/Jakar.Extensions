#nullable enable
namespace Jakar.Extensions;


public class MultiDict<TKey> : Dictionary<TKey, object?> where TKey : notnull
{
    #region ctor

    public MultiDict() : this(0) { }
    public MultiDict(IEqualityComparer<TKey>? comparer) : base(0, comparer) { }
    public MultiDict(int capacity, IEqualityComparer<TKey>? comparer = null) : base(capacity, comparer) { }
    public MultiDict(IDictionary<TKey, object?> dictionary, IEqualityComparer<TKey>? comparer = null) : base(dictionary, comparer) { }
    public MultiDict(IEnumerable<KeyValuePair<TKey, object?>> collection, IEqualityComparer<TKey>? comparer = null) : base(collection, comparer) { }
    protected MultiDict(SerializationInfo info, StreamingContext context) : base(info, context) { }

    #endregion



    #region Gets

    public bool ValueAs(in TKey key, out double value) => ValueAs<double>(key, out value);
    public bool ValueAs(in TKey key, out double? value) => ValueAs<double?>(key, out value);
    public bool ValueAs(in TKey key, out float value) => ValueAs<float>(key, out value);
    public bool ValueAs(in TKey key, out float? value) => ValueAs<float?>(key, out value);
    public bool ValueAs(in TKey key, out long value) => ValueAs<long>(key, out value);
    public bool ValueAs(in TKey key, out long? value) => ValueAs<long?>(key, out value);
    public bool ValueAs(in TKey key, out ulong value) => ValueAs<ulong>(key, out value);
    public bool ValueAs(in TKey key, out ulong? value) => ValueAs<ulong?>(key, out value);
    public bool ValueAs(in TKey key, out int value) => ValueAs<int>(key, out value);
    public bool ValueAs(in TKey key, out int? value) => ValueAs<int?>(key, out value);
    public bool ValueAs(in TKey key, out uint value) => ValueAs<uint>(key, out value);
    public bool ValueAs(in TKey key, out uint? value) => ValueAs<uint?>(key, out value);
    public bool ValueAs(in TKey key, out short value) => ValueAs<short>(key, out value);
    public bool ValueAs(in TKey key, out short? value) => ValueAs<short?>(key, out value);
    public bool ValueAs(in TKey key, out ushort value) => ValueAs<ushort>(key, out value);
    public bool ValueAs(in TKey key, out ushort? value) => ValueAs<ushort?>(key, out value);
    public bool ValueAs(in TKey key, out Guid value) => ValueAs<Guid>(key, out value);
    public bool ValueAs(in TKey key, out Guid? value) => ValueAs<Guid?>(key, out value);
    public bool ValueAs(in TKey key, out DateTime value) => ValueAs<DateTime>(key, out value);
    public bool ValueAs(in TKey key, out DateTime? value) => ValueAs<DateTime?>(key, out value);
    public bool ValueAs(in TKey key, out TimeSpan value) => ValueAs<TimeSpan>(key, out value);
    public bool ValueAs(in TKey key, out TimeSpan? value) => ValueAs<TimeSpan?>(key, out value);
    public bool ValueAs(in TKey key, out bool value) => ValueAs<bool>(key, out value);
    public bool ValueAs(in TKey key, out bool? value) => ValueAs<bool?>(key, out value);


    public bool ValueAs(in TKey key, [NotNullWhen(true)] out IPAddress? value) => ValueAs<IPAddress>(key, out value);
    public bool ValueAs(in TKey key, [NotNullWhen(true)] out AppVersion? value) => ValueAs<AppVersion?>(key, out value);
    public bool ValueAs(in TKey key, [NotNullWhen(true)] out Version? value) => ValueAs<Version>(key, out value);


    public bool ValueAs<T>(in TKey key, [NotNullWhen(true)] out T? value)
    {
        object? s = this[key];

        if (s is T item)
        {
            value = item;
            return true;
        }

        value = default;
        return false;
    }


    public T ValueAs<T>(in TKey key)
    {
        if (!ContainsKey(key)) { throw new KeyNotFoundException(key.ToString()); }

        return ExpectedValueTypeException<TKey>.Verify<T>(this[key], key);
    }

    #endregion



    #region Adds

    public void Add<T>(in TKey key, in T value) => this[key] = value;

    public void Add(in TKey key, in double value) => this[key] = value;
    public void Add(in TKey key, in double? value) => this[key] = value;
    public void Add(in TKey key, in float value) => this[key] = value;
    public void Add(in TKey key, in float? value) => this[key] = value;
    public void Add(in TKey key, in long value) => this[key] = value;
    public void Add(in TKey key, in long? value) => this[key] = value;
    public void Add(in TKey key, in ulong value) => this[key] = value;
    public void Add(in TKey key, in ulong? value) => this[key] = value;
    public void Add(in TKey key, in int value) => this[key] = value;
    public void Add(in TKey key, in uint? value) => this[key] = value;
    public void Add(in TKey key, in short value) => this[key] = value;
    public void Add(in TKey key, in short? value) => this[key] = value;
    public void Add(in TKey key, in ushort value) => this[key] = value;
    public void Add(in TKey key, in ushort? value) => this[key] = value;
    public void Add(in TKey key, in Guid value) => this[key] = value;
    public void Add(in TKey key, in Guid? value) => this[key] = value;
    public void Add(in TKey key, in DateTime value) => this[key] = value;
    public void Add(in TKey key, in DateTime? value) => this[key] = value;
    public void Add(in TKey key, in TimeSpan value) => this[key] = value;
    public void Add(in TKey key, in TimeSpan? value) => this[key] = value;
    public void Add(in TKey key, in bool value) => this[key] = value;
    public void Add(in TKey key, in bool? value) => this[key] = value;
    public void Add(in TKey key, in IPAddress value) => this[key] = value;
    public void Add(in TKey key, in AppVersion value) => this[key] = value;
    public void Add(in TKey key, in Version value) => this[key] = value;

    #endregion
}



public class MultiDict : MultiDict<string>
{
    public MultiDict() : this(0) { }
    public MultiDict(IEqualityComparer<string>? comparer) : base(0, comparer) { }
    public MultiDict(int capacity, IEqualityComparer<string>? comparer = null) : base(capacity, comparer) { }
    public MultiDict(IDictionary<string, object?> dictionary, IEqualityComparer<string>? comparer = null) : base(dictionary, comparer) { }
    public MultiDict(IEnumerable<KeyValuePair<string, object?>> collection, IEqualityComparer<string>? comparer = null) : base(collection, comparer) { }
    protected MultiDict(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
