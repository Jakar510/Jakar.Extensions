namespace Jakar.Database;


public class EnumSqlHandler<T> : SqlConverter<EnumSqlHandler<T>, T> where T : struct, Enum
{
    private static readonly IReadOnlyDictionary<long, T> _longs = Enum.GetValues<T>()
                                                                      .ToDictionary( k => k.AsLong(), v => v );


    private static readonly IReadOnlyDictionary<ulong, T> _uLongs = Enum.GetValues<T>()
                                                                        .ToDictionary( k => k.AsULong(), v => v );
    public EnumSqlHandler() { }


    public override T Parse( object? value ) => value switch
                                                {
                                                    null        => default,
                                                    byte item   => _longs[item],
                                                    sbyte item  => _longs[item],
                                                    short item  => _longs[item],
                                                    ushort item => _uLongs[item],
                                                    int item    => _longs[item],
                                                    uint item   => _uLongs[item],
                                                    long item   => _longs[item],
                                                    ulong item  => _uLongs[item],
                                                    string item => Enum.TryParse( item, true, out T result )
                                                                       ? result
                                                                       : default,
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
            parameter.Value = value.ToString();
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
