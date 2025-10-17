// Jakar.Extensions :: Jakar.Database
// 10/01/2025  11:17

using System.Formats.Asn1;
using FluentMigrator.Model;
using Microsoft.OpenApi.Models;



namespace Jakar.Database;


public static class PostgresParametersExtensions
{
    public static  string SqlColumnName( this string name ) => PostgresParameters.NameSnakeCaseCache.GetOrAdd(name, ToSnakeCase);
    private static string ToSnakeCase( string        x )    => x.ToSnakeCase();
}



public ref struct PostgresParameters : IDisposable
{
    public static readonly ConcurrentDictionary<string, string> NameSnakeCaseCache = new(StringComparer.InvariantCultureIgnoreCase)
                                                                                     {
                                                                                         [nameof(MimeType)]            = "mime_types",
                                                                                         [nameof(SupportedLanguage)]   = "languages",
                                                                                         [nameof(ProgrammingLanguage)] = "programming_languages",
                                                                                         [nameof(SubscriptionStatus)]  = "subscription_status",
                                                                                         [nameof(DeviceCategory)]      = "device_categories",
                                                                                         [nameof(DevicePlatform)]      = "device_platforms",
                                                                                         [nameof(DeviceTypes)]         = "device_types",
                                                                                         [nameof(DistanceUnit)]        = "distance_units",
                                                                                         [nameof(Status)]              = "statuses",
                                                                                         [nameof(NpgsqlDbType)]        = "db_types",
                                                                                     };
    private readonly DynamicParameters __parameters;
    private          string[]?         __array = null;


    public int      Length         => ParameterNames.Length;
    public string[] ParameterNames => __array ??= __parameters.ParameterNames.ToArray();


    public PostgresParameters() : this(new DynamicParameters()) { }
    public PostgresParameters( object?           template ) : this(new DynamicParameters(template)) { }
    public PostgresParameters( DynamicParameters parameters ) => __parameters = parameters;
    public void Dispose() => __array = null;


    public static implicit operator DynamicParameters( PostgresParameters parameters ) => parameters.__parameters;


    public void Add( string name, object? value = null, DbType? dbType = null, ParameterDirection? direction = null, int? size = null, byte? precision = null, byte? scale = null )
    {
        __array = null;
        __parameters.Add(name.SqlColumnName(), value, dbType, direction, size, precision, scale);
    }


    public override int GetHashCode()
    {
        HashCode             code   = new();
        ReadOnlySpan<string> values = ParameterNames;
        foreach ( string parameterName in values ) { code.Add(parameterName); }

        return code.ToHashCode();
    }
    public ulong GetHash64()
    {
        ReadOnlySpan<string> values = ParameterNames;
        return Hashes.Hash(in values);
    }
    public UInt128 GetHash128()
    {
        ReadOnlySpan<string> values = ParameterNames;
        return Hashes.Hash128(in values);
    }
}



public ref struct PostgresParameters<TSelf>( int capacity = DEFAULT_CAPACITY ) : IDisposable
    where TSelf : class, ITableRecord<TSelf>
{
    private string[]?                     __array      = null;
    private FilterBuffer<NpgsqlParameter> __parameters = new(capacity);


    public string[]                      ParameterNames => __array ??= GetNames();
    public ReadOnlySpan<NpgsqlParameter> Values         => __parameters.Values;
    public int                           Length         => __parameters.Length;
    public int                           Capacity       => __parameters.Capacity;


    public void Dispose()
    {
        __array = null;
        __parameters.Dispose();
    }
    private string[] GetNames() => __parameters.Values.AsValueEnumerable()
                                              .Select(x => x.ParameterName)
                                              .ToArray();


    public PostgresParameters<TSelf> Add( NpgsqlParameter parameter )
    {
        __parameters.Add(parameter);
        return this;
    }
    public PostgresParameters<TSelf> Add<TClass, T>( string propertyName, T value, [CallerArgumentExpression(nameof(value))] string parameterName = EMPTY, ParameterDirection direction = ParameterDirection.Input, DataRowVersion sourceVersion = DataRowVersion.Default )
        where TClass : ITableRecord<TClass> => Add(TClass.PropertyMetaData[propertyName], value, parameterName, direction, sourceVersion);
    public PostgresParameters<TSelf> Add<T>( T value, [CallerArgumentExpression(nameof(value))] string parameterName = EMPTY, ParameterDirection direction = ParameterDirection.Input, DataRowVersion sourceVersion = DataRowVersion.Default )
    {
        PrecisionInfo length = PrecisionInfo.Default;
        PostgresType  type   = PostgresTypes.GetType<T>(out bool isNullable, out bool isEnum, ref length);

        NpgsqlParameter parameter = new(parameterName, value)
                                    {
                                        Direction     = direction,
                                        SourceVersion = sourceVersion,
                                        NpgsqlDbType  = type.ToNpgsqlDbType(),
                                        IsNullable    = isNullable,
                                        Value         = value,
                                    };

        return Add(parameter);
    }
    public PostgresParameters<TSelf> Add<T>( ColumnMetaData data, T value, [CallerArgumentExpression(nameof(value))] string parameterName = EMPTY, ParameterDirection direction = ParameterDirection.Input, DataRowVersion sourceVersion = DataRowVersion.Default )
    {
        NpgsqlParameter parameter = data.ToParameter(value, parameterName, direction, sourceVersion);
        return Add(parameter);
    }


    /*
    public Parameters Add<T>( string parameterName, T value, NpgsqlDbType? parameterType = null )
    {
        NpgsqlParameter parameter = new(parameterName.SqlColumnName(), parameterType)
                                    {
                                        Value = value,
                                    };

        return Add(parameter);
    }
    public Parameters Add<T>( string parameterName, T value, int size, NpgsqlDbType? parameterType = null )
    {
        NpgsqlParameter parameter = new(parameterName.SqlColumnName(), parameterType, size)
                                    {
                                        Value = value,
                                    };

        return Add(parameter);
    }
    public Parameters Add<T>( string parameterName, T value, int size, string? sourceColumn, NpgsqlDbType? parameterType = null )
    {
        NpgsqlParameter parameter = new(parameterName.SqlColumnName(), parameterType, size)
                                    {
                                        SourceColumn = sourceColumn?.SqlColumnName() ?? string.Empty,
                                        Value        = value,
                                    };

        return Add(parameter);
    }
    public Parameters Add<T>( string parameterName, T value, int size, string? sourceColumn, ParameterDirection direction, bool isNullable, byte precision, byte scale, DataRowVersion sourceVersion, NpgsqlDbType? parameterType = null )
    {
        NpgsqlParameter parameter = new(parameterName.SqlColumnName(), parameterType, size)
                                    {
                                        SourceColumn  = sourceColumn?.SqlColumnName() ?? string.Empty,
                                        SourceVersion = sourceVersion,
                                        Direction     = direction,
                                        IsNullable    = isNullable,
                                        Precision     = precision,
                                        Scale         = scale,
                                        Value         = value,
                                    };

        return Add(parameter);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Parameters Add{T}"/>.
    /// </summary>
    /// <param name="parameterName">The name of the parameter to map.</param>
    /// <param name="parameterType">One of the <see cref="System.Data.DbType"/> values.</param>
    /// <param name="size">The length of the parameter.</param>
    /// <param name="sourceColumn">The name of the source column.</param>
    /// <param name="direction">One of the <see cref="System.Data.ParameterDirection"/> values.</param>
    /// <param name="isNullable">
    /// <see langword="true"/> if the value of the field can be <see langword="null"/>, otherwise <see langword="false"/>.
    /// </param>
    /// <param name="precision">
    /// The total number of digits to the left and right of the decimal point to which <see cref="Value"/> is resolved.
    /// </param>
    /// <param name="scale">The total number of decimal places to which <see cref="Value"/> is resolved.</param>
    /// <param name="sourceVersion">One of the <see cref="System.Data.DataRowVersion"/> values.</param>
    /// <param name="value">An <see cref="object"/> that is the value of the <see cref="Parameters Add{T}>"/>.</param>
    public Parameters Add<T>( string parameterName, T value, int size, string? sourceColumn, bool isNullable, byte precision, byte scale, ParameterDirection direction = ParameterDirection.Input, DataRowVersion sourceVersion = DataRowVersion.Default, NpgsqlDbType? parameterType = null )
    {
        NpgsqlParameter parameter = new(parameterName.SqlColumnName(), parameterType, size)
                                    {
                                        SourceColumn  = sourceColumn?.SqlColumnName() ?? string.Empty,
                                        SourceVersion = sourceVersion,
                                        Direction     = direction,
                                        IsNullable    = isNullable,
                                        Precision     = precision,
                                        Scale         = scale,
                                        Value         = value,
                                    };

        return Add(parameter);
    }
    */


    public override int GetHashCode()
    {
        HashCode             code   = new();
        ReadOnlySpan<string> values = ParameterNames;
        foreach ( string parameterName in values ) { code.Add(parameterName); }

        return code.ToHashCode();
    }
    public ulong GetHash64()
    {
        ReadOnlySpan<string> values = ParameterNames;
        return Hashes.Hash(in values);
    }
    public UInt128 GetHash128()
    {
        ReadOnlySpan<string> values = ParameterNames;
        return Hashes.Hash128(in values);
    }
}
