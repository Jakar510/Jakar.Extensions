// Jakar.Extensions :: Jakar.Database
// 10/01/2025  11:17

using System.Formats.Asn1;
using FluentMigrator.Model;
using Microsoft.OpenApi.Models;
using ZLinq.Linq;



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



public readonly struct SqlCommand<TSelf>( string sql, PostgresParameters<TSelf> parameters = default, CommandType? commandType = null, CommandFlags flags = CommandFlags.None )
    where TSelf : class, ITableRecord<TSelf>
{
    public readonly string                    sql         = sql;
    public readonly PostgresParameters<TSelf> parameters  = parameters;
    public readonly CommandType?              commandType = commandType;
    public readonly CommandFlags              flags       = flags;


    public static implicit operator SqlCommand<TSelf>( string sql ) => new(sql);


    public NpgsqlCommand ToNpgsqlCommand( NpgsqlConnection connection, NpgsqlTransaction? transaction = null )
    {
        ArgumentNullException.ThrowIfNull(connection);

        NpgsqlCommand command = new NpgsqlCommand
                                {
                                    Connection  = connection,
                                    CommandText = sql,
                                    CommandType = commandType ?? CommandType.Text,
                                    Transaction = transaction,
                                };

        command.Parameters.Add(parameters.Values);
        return command;
    }
}



[DefaultMember(nameof(Empty))]
public struct PostgresParameters<TSelf>( int capacity )
    where TSelf : class, ITableRecord<TSelf>
{
    public static readonly PostgresParameters<TSelf> Empty = new(0);
    private                int                       _count;
    private                NpgsqlParameter[]         _buffer = ArrayPool<NpgsqlParameter>.Shared.Rent(capacity);


    public int                           Count    => _count;
    public int                           Capacity => _buffer.Length;
    public ReadOnlySpan<NpgsqlParameter> Values   => new(_buffer, 0, _count);


    public ValueEnumerable<Select<FromSpan<NpgsqlParameter>, NpgsqlParameter, string>, string> ParameterNames
    {
        get
        {
            ValueEnumerable<FromSpan<NpgsqlParameter>, NpgsqlParameter> values = Values.AsValueEnumerable();
            return values.Select(static x => x.ParameterName);
        }
    }


    public void Dispose()
    {
        ArrayPool<NpgsqlParameter>.Shared.Return(_buffer, clearArray: true);
        _buffer = Array.Empty<NpgsqlParameter>();
        _count  = 0;
    }
    private string[] GetNames()
    {
        string[] names = new string[_count];
        for ( int i = 0; i < _count; i++ ) { names[i] = _buffer[i].ParameterName; }

        return names;
    }
    [DoesNotReturn] [MethodImpl(MethodImplOptions.NoInlining)] private static void ThrowTooManyParameters() => throw new InvalidOperationException("The number of parameters exceeds the fixed capacity.");


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public PostgresParameters<TSelf> Add( NpgsqlParameter parameter )
    {
        if ( _count >= _buffer.Length ) { ThrowTooManyParameters(); }

        _buffer[_count++] = parameter;
        return this;
    }
    public PostgresParameters<TSelf> Add<T>( T value, [CallerArgumentExpression(nameof(value))] string parameterName = "", ParameterDirection direction = ParameterDirection.Input, DataRowVersion sourceVersion = DataRowVersion.Default )
    {
        PrecisionInfo precision = PrecisionInfo.Default;
        PostgresType  pgType    = PostgresTypes.GetType<T>(out bool isNullable, out bool isEnum, ref precision);

        NpgsqlParameter parameter = new NpgsqlParameter(parameterName, value)
                                    {
                                        Direction     = direction,
                                        SourceVersion = sourceVersion,
                                        NpgsqlDbType  = pgType.ToNpgsqlDbType(),
                                        IsNullable    = isNullable
                                    };

        return Add(parameter);
    }
    public PostgresParameters<TSelf> Add<T>( string propertyName, T value, [CallerArgumentExpression(nameof(value))] string parameterName = "", ParameterDirection direction = ParameterDirection.Input, DataRowVersion sourceVersion = DataRowVersion.Default )
    {
        ColumnMetaData meta = TSelf.PropertyMetaData[propertyName];
        return Add(meta, value, parameterName, direction, sourceVersion);
    }
    public PostgresParameters<TSelf> Add<T>( ColumnMetaData meta, T value, [CallerArgumentExpression(nameof(value))] string parameterName = "", ParameterDirection direction = ParameterDirection.Input, DataRowVersion sourceVersion = DataRowVersion.Default )
    {
        NpgsqlParameter parameter = new(parameterName, meta.DbType.ToNpgsqlDbType())
                                    {
                                        SourceColumn  = meta.ColumnName,
                                        IsNullable    = meta.IsNullable,
                                        SourceVersion = sourceVersion,
                                        Direction     = direction,
                                        Value         = value,
                                    };

        return Add(parameter);
    }


    public override int GetHashCode()
    {
        HashCode code  = new();
        string[] names = ParameterNames.ToArray();
        for ( int i = 0; i < _count; i++ ) { code.Add(names[i]); }

        return code.ToHashCode();
    }
    public ulong GetHash64()
    {
        ReadOnlySpan<string> names = ParameterNames.ToArray();
        return Hashes.Hash(in names);
    }
    public UInt128 GetHash128()
    {
        ReadOnlySpan<string> names = ParameterNames.ToArray();
        return Hashes.Hash128(in names);
    }
}
