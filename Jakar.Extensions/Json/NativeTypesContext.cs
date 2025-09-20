namespace Jakar.Extensions;


[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(string?[]))]
[JsonSerializable(typeof(double[]))]
[JsonSerializable(typeof(double?[]))]
[JsonSerializable(typeof(float[]))]
[JsonSerializable(typeof(float?[]))]
[JsonSerializable(typeof(long[]))]
[JsonSerializable(typeof(long?[]))]
[JsonSerializable(typeof(int[]))]
[JsonSerializable(typeof(int?[]))]
[JsonSerializable(typeof(short[]))]
[JsonSerializable(typeof(short?[]))]
[JsonSerializable(typeof(ushort[]))]
[JsonSerializable(typeof(ushort?[]))]
[JsonSerializable(typeof(uint[]))]
[JsonSerializable(typeof(uint?[]))]
[JsonSerializable(typeof(ulong[]))]
[JsonSerializable(typeof(ulong?[]))]
[JsonSerializable(typeof(DateTime[]))]
[JsonSerializable(typeof(DateTime?[]))]
[JsonSerializable(typeof(DateOnly[]))]
[JsonSerializable(typeof(DateOnly?[]))]
[JsonSerializable(typeof(DateTimeOffset[]))]
[JsonSerializable(typeof(DateTimeOffset?[]))]
[JsonSerializable(typeof(TimeSpan[]))]
[JsonSerializable(typeof(TimeSpan?[]))]
public sealed partial class NativeTypesContext : JsonSerializerContext
{
    static NativeTypesContext()
    {
        Default.StringArray.Register();
        Default.NullableSingleArray.Register();

        Default.Int16Array.Register();
        Default.Int32Array.Register();
        Default.Int64Array.Register();
        Default.UInt64Array.Register();
        Default.SingleArray.Register();
        Default.DoubleArray.Register();

        Default.NullableInt16Array.Register();
        Default.NullableInt32Array.Register();
        Default.NullableInt64Array.Register();
        Default.NullableUInt64Array.Register();
        Default.NullableSingleArray.Register();
        Default.NullableDoubleArray.Register();

        Default.DateTime.Register();
        Default.DateTimeArray.Register();

        Default.DateTimeOffset.Register();
        Default.DateTimeOffsetArray.Register();

        Default.DateOnly.Register();
        Default.DateOnlyArray.Register();

        Default.TimeSpan.Register();
        Default.TimeSpanArray.Register();
    }
}
