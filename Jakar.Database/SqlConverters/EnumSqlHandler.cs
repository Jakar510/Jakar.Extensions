namespace Jakar.Database;


public class EnumSqlHandler<TValue> : SqlConverter<EnumSqlHandler<TValue>, TValue>
    where TValue : unmanaged, Enum
{
    public static readonly FrozenDictionary<long, TValue>   Longs  = Enum.GetValues<TValue>().ToFrozenDictionary(GetLong,    SelectSelf);
    public static readonly FrozenDictionary<string, TValue> Names  = Enum.GetValues<TValue>().ToFrozenDictionary(GetString,  SelectSelf);
    public static readonly FrozenDictionary<TValue, string> Values = Enum.GetValues<TValue>().ToFrozenDictionary(SelectSelf, GetString);


    public EnumSqlHandler() { }

    private static string GetString( TValue  k ) => k.ToString();
    private static long   GetLong( TValue    k ) => k.AsLong();
    private static TValue SelectSelf( TValue v ) => v;


    public static TValue Parse( string? value ) => Names.TryGetValue(value ?? string.Empty, out TValue result)
                                                       ? result
                                                       : Enum.TryParse(value, true, out result)
                                                           ? result
                                                           : default;
    public override TValue Parse( object? value ) => value switch
                                                     {
                                                         null       => default,
                                                         string s   => Parse(s),
                                                         byte item  => Longs[item],
                                                         sbyte item => Longs[item],
                                                         short item => Longs[item],
                                                         int item   => Longs[item],
                                                         long item  => Longs[item],
                                                         _ => throw new ExpectedValueTypeException(nameof(value),
                                                                                                   value,
                                                                                                   typeof(byte),
                                                                                                   typeof(sbyte),
                                                                                                   typeof(short),
                                                                                                   typeof(ushort),
                                                                                                   typeof(int),
                                                                                                   typeof(uint),
                                                                                                   typeof(long),
                                                                                                   typeof(ulong),
                                                                                                   typeof(string))
                                                     };

    public override void SetValue( IDbDataParameter parameter, TValue value )
    {
        if ( parameter.DbType is DbType.String or DbType.StringFixedLength )
        {
            parameter.Value = Values.TryGetValue(value, out string? result)
                                  ? result
                                  : value.ToString();

            return;
        }


        Type enumType = Enum.GetUnderlyingType(typeof(TValue));

        object item = value is IConvertible convertible
                          ? convertible.ToType(enumType, CultureInfo.InvariantCulture)
                          : Enum.ToObject(enumType, value);

        parameter.Value  = item;
        parameter.DbType = DbType.Int64;
    }
}
