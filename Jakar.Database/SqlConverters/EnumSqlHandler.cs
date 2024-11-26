namespace Jakar.Database;


public class EnumSqlHandler<T> : SqlConverter<EnumSqlHandler<T>, T>
    where T : struct, Enum
{
    public static readonly FrozenDictionary<long, T>   Longs  = Enum.GetValues<T>().ToFrozenDictionary( GetLong,    SelectSelf );
    public static readonly FrozenDictionary<string, T> Names  = Enum.GetValues<T>().ToFrozenDictionary( GetString,  SelectSelf );
    public static readonly FrozenDictionary<T, string> Values = Enum.GetValues<T>().ToFrozenDictionary( SelectSelf, GetString );


    public EnumSqlHandler() { }

    private static string GetString( T  k ) => k.ToString();
    private static long   GetLong( T    k ) => k.AsLong();
    private static T      SelectSelf( T v ) => v;


    public static T Parse( string? value ) => Names.TryGetValue( value ?? string.Empty, out T result )
                                                  ? result
                                                  : Enum.TryParse( value, true, out result )
                                                      ? result
                                                      : default;
    public override T Parse( object? value ) => value switch
                                                {
                                                    null       => default,
                                                    string s   => Parse( s ),
                                                    byte item  => Longs[item],
                                                    sbyte item => Longs[item],
                                                    short item => Longs[item],
                                                    int item   => Longs[item],
                                                    long item  => Longs[item],
                                                    _ => throw new ExpectedValueTypeException( nameof(value),
                                                                                               value,
                                                                                               [
                                                                                                   typeof(byte),
                                                                                                   typeof(sbyte),
                                                                                                   typeof(short),
                                                                                                   typeof(ushort),
                                                                                                   typeof(int),
                                                                                                   typeof(uint),
                                                                                                   typeof(long),
                                                                                                   typeof(ulong),
                                                                                                   typeof(string)
                                                                                               ] )
                                                };

    public override void SetValue( IDbDataParameter parameter, T value )
    {
        if ( parameter.DbType is DbType.String or DbType.StringFixedLength )
        {
            parameter.Value = Values.TryGetValue( value, out string? result )
                                  ? result
                                  : value.ToString();

            return;
        }


        Type enumType = Enum.GetUnderlyingType( typeof(T) );

        object item = value is IConvertible convertible
                          ? convertible.ToType( enumType, CultureInfo.InvariantCulture )
                          : Enum.ToObject( enumType, value );

        parameter.Value  = item;
        parameter.DbType = DbType.Int64;
    }
}
