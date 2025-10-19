// Jakar.Extensions :: Jakar.Database
// 10/19/2025  10:38

using ZLinq.Linq;



namespace Jakar.Database;


[DefaultMember(nameof(Empty))]
public readonly struct PostgresParameters<TSelf>( int capacity ) : IEquatable<PostgresParameters<TSelf>>
    where TSelf : ITableRecord<TSelf>
{
    public static readonly PostgresParameters<TSelf>     Empty   = new(0);
    private readonly       List<NpgsqlParameter>         _buffer = new(capacity);
    public                 int                           Count    => _buffer.Count;
    public                 int                           Capacity => _buffer.Capacity;
    public                 ReadOnlySpan<NpgsqlParameter> Values   => CollectionsMarshal.AsSpan(_buffer);


    public ValueEnumerable<Select<FromSpan<NpgsqlParameter>, NpgsqlParameter, string>, string> ParameterNames
    {
        get
        {
            ValueEnumerable<FromSpan<NpgsqlParameter>, NpgsqlParameter> values = Values.AsValueEnumerable();
            return values.Select(static x => x.ParameterName);
        }
    }


    public PostgresParameters<TSelf> With( PostgresParameters<TSelf> parameters )
    {
        _buffer.Add(parameters.Values);
        return this;
    }


    /*
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
    */
    public PostgresParameters<TSelf> Add<T>( string propertyName, T value, [CallerArgumentExpression(nameof(value))] string parameterName = "", ParameterDirection direction = ParameterDirection.Input, DataRowVersion sourceVersion = DataRowVersion.Default )
    {
        ColumnMetaData<TSelf> meta = TSelf.PropertyMetaData[propertyName];
        return Add(meta, value, parameterName, direction, sourceVersion);
    }
    public PostgresParameters<TSelf> Add<T>( ColumnMetaData<TSelf> meta, T value, [CallerArgumentExpression(nameof(value))] string parameterName = "", ParameterDirection direction = ParameterDirection.Input, DataRowVersion sourceVersion = DataRowVersion.Default )
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public PostgresParameters<TSelf> Add( NpgsqlParameter parameter )
    {
        _buffer.Add(parameter);
        return this;
    }


    public override int GetHashCode() => HashCode.Combine(_buffer);
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


    public          bool Equals( PostgresParameters<TSelf>      other )                                 => _buffer.Equals(other._buffer);
    public override bool Equals( object?                        obj )                                   => obj is PostgresParameters<TSelf> other && Equals(other);
    public static   bool operator ==( PostgresParameters<TSelf> left, PostgresParameters<TSelf> right ) => left.Equals(right);
    public static   bool operator !=( PostgresParameters<TSelf> left, PostgresParameters<TSelf> right ) => !left.Equals(right);


    public StringBuilder KeyValuePairs( bool matchAll )
    {
        string        match  = matchAll.GetAndOr();
        int           count  = Count;
        int           length = TSelf.PropertyMetaData.Values.Sum(static x => x.KeyValuePair.Length) + ( TSelf.PropertyMetaData.Count - 1 ) * match.Length;
        StringBuilder sb     = new(length);
        int           index  = 0;

        foreach ( string pair in ParameterNames.Select(GetKeyValuePair) )
        {
            if ( index++ < count - 1 )
            {
                sb.Append(pair)
                  .Append(match);
            }
            else { sb.Append(pair); }
        }

        return sb;
    }
    public StringBuilder ColumnNames()
    {
        const string  SPACER = ",\n      \n";
        int           length = TSelf.PropertyMetaData.Values.Sum(static x => x.ColumnName.Length) + ( TSelf.PropertyMetaData.Count - 1 ) * SPACER.Length;
        StringBuilder sb     = new(length);
        int           count  = Count;
        int           index  = 0;

        foreach ( string pair in ParameterNames.Select(GetColumnName) )
        {
            if ( index++ < count - 1 )
            {
                sb.Append(pair)
                  .Append(SPACER);
            }
            else { sb.Append(pair); }
        }

        return sb;
    }
    public StringBuilder VariableNames()
    {
        const string  SPACER = ",\n      ";
        int           length = TSelf.PropertyMetaData.Values.Sum(static x => x.VariableName.Length) + ( TSelf.PropertyMetaData.Count - 1 ) * SPACER.Length;
        StringBuilder sb     = new(length);
        int           count  = Count;
        int           index  = 0;

        foreach ( string pair in ParameterNames.Select(GetVariableName) )
        {
            if ( index++ < count - 1 )
            {
                sb.Append(pair)
                  .Append(SPACER);
            }
            else { sb.Append(pair); }
        }

        return sb;
    }
    private static string GetColumnName( string   propertyName ) => TSelf.PropertyMetaData[propertyName].ColumnName;
    private static string GetVariableName( string propertyName ) => TSelf.PropertyMetaData[propertyName].VariableName;
    private static string GetKeyValuePair( string propertyName ) => TSelf.PropertyMetaData[propertyName].KeyValuePair;
}



public readonly struct SqlCommand<TSelf>( string sql, in PostgresParameters<TSelf> parameters = default, CommandType? commandType = null, CommandFlags flags = CommandFlags.None ) : IEquatable<SqlCommand<TSelf>>
    where TSelf : ITableRecord<TSelf>
{
    public readonly                 string                    sql         = sql;
    public readonly                 PostgresParameters<TSelf> parameters  = parameters;
    public readonly                 CommandType?              commandType = commandType;
    public readonly                 CommandFlags              flags       = flags;
    public static implicit operator SqlCommand<TSelf>( string sql ) => new(sql, in PostgresParameters<TSelf>.Empty);


    public NpgsqlCommand ToNpgsqlCommand( NpgsqlConnection connection, NpgsqlTransaction? transaction = null )
    {
        ArgumentNullException.ThrowIfNull(connection);

        NpgsqlCommand command = new NpgsqlCommand
                                {
                                    Connection     = connection,
                                    CommandText    = sql,
                                    CommandType    = commandType ?? CommandType.Text,
                                    Transaction    = transaction,
                                    CommandTimeout = 30
                                };

        command.Parameters.Add(parameters.Values);
        return command;
    }
    public NpgsqlDataReader Execute( NpgsqlConnection connection, NpgsqlTransaction? transaction = null )
    {
        NpgsqlCommand command = ToNpgsqlCommand(connection, transaction);
        return command.ExecuteReader();
    }
    public Task<NpgsqlDataReader> ExecuteAsync( NpgsqlConnection connection, NpgsqlTransaction? transaction = null, CancellationToken token = default )
    {
        NpgsqlCommand command = ToNpgsqlCommand(connection, transaction);
        return command.ExecuteReaderAsync(token);
    }


    public          bool Equals( SqlCommand<TSelf> other ) => string.Equals(sql, other.sql, StringComparison.InvariantCultureIgnoreCase) && parameters.Equals(other.parameters) && commandType == other.commandType && flags == other.flags;
    public override bool Equals( object?           obj )   => obj is SqlCommand<TSelf> other                                             && Equals(other);
    public override int GetHashCode()
    {
        HashCode hashCode = new HashCode();
        hashCode.Add(sql, StringComparer.InvariantCultureIgnoreCase);
        hashCode.Add(parameters);
        hashCode.Add(commandType);
        hashCode.Add((int)flags);
        return hashCode.ToHashCode();
    }
    public static bool operator ==( SqlCommand<TSelf> left, SqlCommand<TSelf> right ) => left.Equals(right);
    public static bool operator !=( SqlCommand<TSelf> left, SqlCommand<TSelf> right ) => !left.Equals(right);


    public const string SPACER       = ",      \n";
    public const string CREATED_BY   = "created_by";
    public const string DATE_CREATED = "date_created";
    public const string ID           = "id";


    public static IEnumerable<string> KeyValuePairs => TSelf.PropertyMetaData.Values.Select(ColumnMetaData<TSelf>.GetKeyValuePair);


    public static SqlCommand<TSelf> GetRandom() => $"""
                                                    SELECT * FROM {TSelf.TableName} 
                                                    ORDER BY RANDOM()
                                                    LIMIT 1;
                                                    """;
    public static SqlCommand<TSelf> GetRandom( int count )
    {
        string sql = $"""
                      SELECT * FROM {TSelf.TableName} 
                      ORDER BY RANDOM() 
                      LIMIT {count};
                      """;

        return new SqlCommand<TSelf>(sql);
    }
    public static SqlCommand<TSelf> GetRandom( UserRecord user, int count ) => GetRandom(user.ID, count);
    public static SqlCommand<TSelf> GetRandom( RecordID<UserRecord> id, int count )
    {
        string sql = $"""
                      SELECT * FROM {TSelf.TableName} 
                      WHERE {CREATED_BY} = '{id.Value}'
                      ORDER BY RANDOM() 
                      LIMIT {count};
                      """;

        return new SqlCommand<TSelf>(sql);
    }


    public static SqlCommand<TSelf> WherePaged( bool matchAll, PostgresParameters<TSelf> parameters, int start, int count )
    {
        string sql = $"""
                        SELECT * FROM {TSelf.TableName}
                        {parameters.KeyValuePairs(matchAll)}
                        OFFSET {start}
                        LIMIT {count}
                      """;

        return new SqlCommand<TSelf>(sql, parameters);
    }
    public static SqlCommand<TSelf> WherePaged( ref readonly RecordID<UserRecord> id, int start, int count )
    {
        string sql = $"""
                      SELECT * FROM {TSelf.TableName} 
                      WHERE {CREATED_BY} = '{id.Value}'
                      OFFSET {start}
                      LIMIT {count};
                      """;

        return new SqlCommand<TSelf>(sql);
    }
    public static SqlCommand<TSelf> WherePaged( int start, int count )
    {
        string sql = $"""
                      SELECT * FROM {TSelf.TableName} 
                      OFFSET {start}
                      LIMIT {count};
                      """;

        return new SqlCommand<TSelf>(sql);
    }
    public static SqlCommand<TSelf> Where<TValue>( string columnName, TValue? value )
    {
        string sql = $"SELECT * FROM {TSelf.TableName} WHERE {columnName} = @{nameof(value)};";

        PostgresParameters<TSelf> parameters = new();
        parameters.Add(nameof(value), value);

        return new SqlCommand<TSelf>(sql, parameters);
    }


    public static SqlCommand<TSelf> Get( ref readonly RecordID<TSelf> id )
    {
        string sql = $$"""
                       SELECT * FROM {{TSelf.TableName}} 
                       WHERE {{ID}} = '{0}';
                       """;

        return new SqlCommand<TSelf>(string.Format(sql, id.Value.ToString()));
    }
    public static SqlCommand<TSelf> Get( IEnumerable<RecordID<TSelf>> ids )
    {
        string sql = $"""
                      SELECT * FROM {TSelf.TableName}
                      WHERE {ID} in ({string.Join(',', ids.Select(GetValue))});
                      """;

        return new SqlCommand<TSelf>(sql);
    }
    public static SqlCommand<TSelf> Get( bool matchAll, PostgresParameters<TSelf> parameters )
    {
        string sql = $"""
                      SELECT * FROM {TSelf.TableName}
                      WHERE {parameters.KeyValuePairs(matchAll)};
                      """;

        return new SqlCommand<TSelf>(sql, parameters);
    }
    public static SqlCommand<TSelf> GetAll() => $"SELECT * FROM {TSelf.TableName};";
    public static SqlCommand<TSelf> GetFirst() => $"""
                                                   SELECT * FROM {TSelf.TableName} 
                                                   ORDER BY {DATE_CREATED} ASC 
                                                   LIMIT 1;
                                                   """;
    public static SqlCommand<TSelf> GetLast() => $"""
                                                  SELECT * FROM {TSelf.TableName} 
                                                  ORDER BY {DATE_CREATED} DESC 
                                                  LIMIT 1
                                                  """;


    public static SqlCommand<TSelf> GetCount() => $"SELECT COUNT(*) FROM {TSelf.TableName};";
    public static SqlCommand<TSelf> GetSortedID() => $"""
                                                      SELECT {ID}, {DATE_CREATED} FROM {TSelf.TableName} 
                                                      ORDER BY {DATE_CREATED} DESC;
                                                      """;
    public static SqlCommand<TSelf> GetExists( bool matchAll, PostgresParameters<TSelf> parameters )
    {
        string sql = $"""
                      EXISTS( 
                      SELECT * FROM {TSelf.TableName}
                      WHERE {parameters.KeyValuePairs(matchAll)};
                      """;

        return new SqlCommand<TSelf>(sql, parameters);
    }


    public static SqlCommand<TSelf> GetDelete( bool matchAll, PostgresParameters<TSelf> parameters )
    {
        string sql = $"""
                      DELETE FROM {TSelf.TableName} 
                      WHERE {ID} in ({parameters.KeyValuePairs(matchAll)});
                      """;

        return new SqlCommand<TSelf>(sql, parameters);
    }
    public static SqlCommand<TSelf> GetDeleteID( ref readonly RecordID<TSelf> id )
    {
        string sql = $"""
                      DELETE FROM {TSelf.TableName}
                      WHERE {ID} = '{id.Value}';
                      """;

        return new SqlCommand<TSelf>(sql);
    }
    public static SqlCommand<TSelf> GetDelete( IEnumerable<RecordID<TSelf>> ids )
    {
        string sql = $"""
                      DELETE FROM {TSelf.TableName} 
                      WHERE {ID} in ({string.Join(',', ids.Select(GetValue))});
                      """;

        return new SqlCommand<TSelf>(sql);
    }
    public static SqlCommand<TSelf> GetDeleteAll() => $"DELETE FROM {TSelf.TableName};";


    public static SqlCommand<TSelf> GetNext( ref readonly RecordPair<TSelf> pair )
    {
        string sql = $"""
                      SELECT * FROM {TSelf.TableName}
                      WHERE ( id = IFNULL((SELECT MIN({DATE_CREATED}) FROM {TSelf.TableName} WHERE {DATE_CREATED} > '{pair.DateCreated}' LIMIT 2, 0) );
                      """;

        return new SqlCommand<TSelf>(string.Format(sql));
    }
    public static SqlCommand<TSelf> GetNextID( ref readonly RecordPair<TSelf> pair )
    {
        string sql = $"""
                      SELECT {ID} FROM {TSelf.TableName}
                      WHERE ( id = IFNULL((SELECT MIN({DATE_CREATED}) FROM {TSelf.TableName} WHERE {DATE_CREATED} > '{pair.DateCreated}' LIMIT 2), 0) );
                      """;

        return new SqlCommand<TSelf>(sql);
    }
    public static SqlCommand<TSelf> GetInsert( TSelf record )
    {
        PostgresParameters<TSelf> parameters = record.ToDynamicParameters();

        string sql = $"""
                      INSERT INTO {TSelf.TableName} 
                      (
                        {parameters.ColumnNames()}
                      )
                      values
                      (
                        {parameters.VariableNames()}
                      ) 
                      RETURNING {ID};
                      """;

        return new SqlCommand<TSelf>(sql, record.ToDynamicParameters());
    }
    public static SqlCommand<TSelf> GetUpdate( TSelf record ) => new($"""
                                                                      UPDATE {TSelf.TableName} 
                                                                      SET {string.Join(',', KeyValuePairs)} 
                                                                      WHERE {ID} = @{ID};
                                                                      """,
                                                                     record.ToDynamicParameters());
    public static SqlCommand<TSelf> GetTryInsert( TSelf record, bool matchAll, PostgresParameters<TSelf> parameters )
    {
        PostgresParameters<TSelf> param = record.ToDynamicParameters()
                                                .With(parameters);

        string sql = $"""
                      IF NOT EXISTS(SELECT * FROM {TSelf.TableName} WHERE {parameters.KeyValuePairs(matchAll)})
                      BEGIN
                      INSERT INTO {TSelf.TableName}
                      (
                        {parameters.ColumnNames()}
                      )
                      values
                      (
                        {parameters.VariableNames()}
                      ) 
                      RETURNING {ID};
                      END

                      ELSE
                      BEGIN
                      SELECT {ID} = '{Guid.Empty}';
                      END
                      """;

        return new SqlCommand<TSelf>(sql, param);
    }
    public static SqlCommand<TSelf> InsertOrUpdate( TSelf record, bool matchAll, PostgresParameters<TSelf> parameters )
    {
        PostgresParameters<TSelf> param = record.ToDynamicParameters()
                                                .With(parameters);

        string sql = $"""
                      IF NOT EXISTS(SELECT * FROM {TSelf.TableName} WHERE {parameters.KeyValuePairs(matchAll)})
                      BEGIN
                      INSERT INTO {TSelf.TableName}
                      (
                      {param.ColumnNames()}
                      ) 
                      values 
                      (
                      {param.VariableNames()}
                      ) 
                      RETURNING {ID};
                      END

                      ELSE
                      BEGIN
                      UPDATE {TSelf.TableName} SET {KeyValuePairs} WHERE {ID} = @{ID};
                      SELECT @{ID};
                      END
                      """;

        return new SqlCommand<TSelf>(sql, parameters);
    }


    private static Guid GetValue( RecordID<TSelf> id ) => id.Value;
}
