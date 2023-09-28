namespace Jakar.Database;


public class EnumSqlHandler<T> : SqlConverter<EnumSqlHandler<T>, T> where T : struct, Enum
{
    private static readonly IReadOnlyDictionary<string, T> _names = Enum.GetValues<T>()
                                                                        .ToDictionary( k => k.ToString(), v => v );
    private static readonly IReadOnlyDictionary<T, string> _values = Enum.GetValues<T>()
                                                                         .ToDictionary( k => k, v => v.ToString() );
    private static readonly IReadOnlyDictionary<long, T> _longs = Enum.GetValues<T>()
                                                                      .ToDictionary( k => k.AsLong(), v => v );


    public EnumSqlHandler() { }


    public static T Parse( string? value ) => _names.TryGetValue( value ?? string.Empty, out T result )
                                                  ? result
                                                  : Enum.TryParse( value, true, out result )
                                                      ? result
                                                      : default;
    public override T Parse( object? value ) => value switch
                                                {
                                                    null       => default,
                                                    string s   => Parse( s ),
                                                    byte item  => _longs[item],
                                                    sbyte item => _longs[item],
                                                    short item => _longs[item],
                                                    int item   => _longs[item],
                                                    long item  => _longs[item],
                                                    _ => throw new ExpectedValueTypeException( nameof(value),
                                                                                               value,
                                                                                               typeof(byte),
                                                                                               typeof(sbyte),
                                                                                               typeof(short),
                                                                                               typeof(ushort),
                                                                                               typeof(int),
                                                                                               typeof(uint),
                                                                                               typeof(long),
                                                                                               typeof(ulong),
                                                                                               typeof(string) ),
                                                };

    public override void SetValue( IDbDataParameter parameter, T value )
    {
        if ( parameter.DbType is DbType.String or DbType.StringFixedLength )
        {
            parameter.Value = _values.TryGetValue( value, out string? result )
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
