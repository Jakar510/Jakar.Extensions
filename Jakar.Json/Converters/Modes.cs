// Jakar.Json
// 10/07/2026

namespace Jakar.Json.Converters;


// Settings that change how a value is read or written travel as type arguments (marker structs with a static property), so the
// JIT specializes each combination and the check folds to a constant: JsonIntegerConverter<long, JsonNumberModes.Flags2>.



[Flags]
public enum JsonNumberMode : byte
{
    Strict                 = 0,
    /// <summary> Read numbers given as JSON strings too (<c>NumbersFromStrings = Allow</c>). </summary>
    AllowReadingFromString = 1,
    /// <summary> Write integers beyond ±(2^53 − 1) as strings (<c>LargeIntegers = String</c>); they read back from strings too. </summary>
    WriteLargeAsString     = 2,
    /// <summary> Write and read NaN and ±Infinity as <c>"NaN"</c>, <c>"Infinity"</c>, <c>"-Infinity"</c> (<c>NonFiniteFloats = AsString</c>). </summary>
    NonFiniteAsString      = 4
}



public interface IJsonNumberMode
{
    static abstract JsonNumberMode Mode { get; }
}



/// <summary> <c>FlagsN</c> carries <c>(JsonNumberMode)N</c>. </summary>
public static class JsonNumberModes
{
    public readonly struct Flags0 : IJsonNumberMode
    {
        public static JsonNumberMode Mode => (JsonNumberMode)0;
    }



    public readonly struct Flags1 : IJsonNumberMode
    {
        public static JsonNumberMode Mode => (JsonNumberMode)1;
    }



    public readonly struct Flags2 : IJsonNumberMode
    {
        public static JsonNumberMode Mode => (JsonNumberMode)2;
    }



    public readonly struct Flags3 : IJsonNumberMode
    {
        public static JsonNumberMode Mode => (JsonNumberMode)3;
    }



    public readonly struct Flags4 : IJsonNumberMode
    {
        public static JsonNumberMode Mode => (JsonNumberMode)4;
    }



    public readonly struct Flags5 : IJsonNumberMode
    {
        public static JsonNumberMode Mode => (JsonNumberMode)5;
    }



    public readonly struct Flags6 : IJsonNumberMode
    {
        public static JsonNumberMode Mode => (JsonNumberMode)6;
    }



    public readonly struct Flags7 : IJsonNumberMode
    {
        public static JsonNumberMode Mode => (JsonNumberMode)7;
    }
}



public interface IJsonDateTimeMode
{
    static abstract JsonLocalDateTimes Mode { get; }
}



public static class JsonDateTimeModes
{
    public readonly struct ConvertToUtc : IJsonDateTimeMode
    {
        public static JsonLocalDateTimes Mode => JsonLocalDateTimes.ConvertToUtc;
    }



    public readonly struct WriteOffset : IJsonDateTimeMode
    {
        public static JsonLocalDateTimes Mode => JsonLocalDateTimes.WriteOffset;
    }



    public readonly struct Error : IJsonDateTimeMode
    {
        public static JsonLocalDateTimes Mode => JsonLocalDateTimes.Error;
    }
}



/// <summary>
///     How an unordered collection's elements (or a map's keys) are ordered when written (SPEC.md §5.2): sorted, so equal collections write the same bytes, or in
///     enumeration order. The generator picks the comparison at compile time (ordinal for strings, <see cref="IComparable{T}"/>, an enum's value), so nothing is
///     resolved at runtime.
/// </summary>
public interface IJsonOrder<in T>
{
    static abstract bool Sorted { get; }
    static abstract int  Compare( T x, T y );
}



public static class JsonOrders
{
    /// <summary> Enumeration order: no sorting (<c>UnorderedCollections = Enumeration</c>, or elements that can't be compared). </summary>
    public readonly struct Enumeration<T> : IJsonOrder<T>
    {
        public static bool Sorted              => false;
        public static int  Compare( T x, T y ) => 0;
    }



    /// <summary> Strings, ordinally: culture never affects the output. </summary>
    public readonly struct Ordinal : IJsonOrder<string?>
    {
        public static bool Sorted                          => true;
        public static int  Compare( string? x, string? y ) => string.CompareOrdinal(x, y);
    }



    public readonly struct Comparable<T> : IJsonOrder<T>
        where T : IComparable<T>
    {
        public static bool Sorted => true;

        public static int Compare( T x, T y ) => x is null
                                                     ? y is null
                                                           ? 0
                                                           : -1
                                                     : x.CompareTo(y);
    }



    /// <summary> Enums by their numeric value. </summary>
    public readonly struct Enum<TEnum, TUnderlying> : IJsonOrder<TEnum>
        where TEnum : struct, System.Enum
        where TUnderlying : struct, IBinaryInteger<TUnderlying>
    {
        public static bool Sorted                      => true;
        public static int  Compare( TEnum x, TEnum y ) => Unsafe.BitCast<TEnum, TUnderlying>(x).CompareTo(Unsafe.BitCast<TEnum, TUnderlying>(y));
    }



    /// <summary> <c>T?</c> for a value type: <see langword="null"/> first, then <typeparamref name="TOrder"/>. </summary>
    public readonly struct Nullable<T, TOrder> : IJsonOrder<T?>
        where T : struct
        where TOrder : IJsonOrder<T>
    {
        public static bool Sorted => TOrder.Sorted;

        public static int Compare( T? x, T? y ) => x.HasValue
                                                       ? y.HasValue
                                                             ? TOrder.Compare(x.GetValueOrDefault(), y.GetValueOrDefault())
                                                             : 1
                                                       : y.HasValue
                                                           ? -1
                                                           : 0;
    }
}



internal readonly struct OrderComparer<T, TOrder> : IComparer<T>
    where TOrder : IJsonOrder<T>
{
    public int Compare( T? x, T? y ) => TOrder.Compare(x!, y!);
}



internal readonly struct KeyOrderComparer<TKey, TValue, TOrder> : IComparer<KeyValuePair<TKey, TValue>>
    where TOrder : IJsonOrder<TKey>
{
    public int Compare( KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y ) => TOrder.Compare(x.Key, y.Key);
}
