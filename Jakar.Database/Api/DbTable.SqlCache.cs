// Jakar.Extensions :: Jakar.Database
// 10/10/2023  10:37 AM


namespace Jakar.Database;


public abstract class SqlCache<TRecord> : ObservableClass where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    // ReSharper disable once StaticMemberInGenericType
    protected static readonly ReadOnlyMemory<DbInstance>     _instances       = Enum.GetValues<DbInstance>();
    protected static readonly TypePropertiesCache.Properties _propertiesCache = TypePropertiesCache.Current.Register<TRecord>();
    protected static readonly ReadOnlyMemory<SqlStatement>   _statements      = Enum.GetValues<SqlStatement>();
    public const              string                         COUNT            = "count";
    public const              string                         CREATED_BY       = nameof(IOwnedTableRecord.CreatedBy);
    public const              string                         DATE_CREATED     = nameof(TableRecord<TRecord>.DateCreated);
    public const              string                         ID               = nameof(TableRecord<TRecord>.ID);
    public const              string                         LAST_MODIFIED    = nameof(TableRecord<TRecord>.LastModified);
    public const              string                         OWNER_USER_ID    = nameof(IOwnedTableRecord.OwnerUserID);
    protected const           char                           QUOTE            = '"';


    protected readonly ConcurrentDictionary<DbInstance, ImmutableDictionary<SqlStatement, string>> _cache           = new();
    protected readonly ConcurrentDictionary<int, string>                                           _deleteGuids     = new();
    protected readonly ConcurrentDictionary<int, string>                                           _whereParameters = new();
    protected readonly ConcurrentDictionary<string, string>                                        _where           = new();
    protected readonly IConnectableDbRoot                                                          _database;


    public static ImmutableArray<TRecord> Empty
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => ImmutableArray<TRecord>.Empty;
    }
    public static ImmutableList<TRecord> EmptyList
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => ImmutableList<TRecord>.Empty;
    }
    public IEnumerable<string> ColumnNames
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => Descriptors.Select( x => x.ColumnName );
    }
    public int Count
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _cache.Count;
    }
    public string CreatedBy
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => GetCreatedBy( _database.Instance );
    }
    public string DateCreated
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => GetDateCreated( _database.Instance );
    }
    public virtual IEnumerable<Descriptor> Descriptors
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _propertiesCache.GetValues( _database );
    }
    public string ID_ColumnName
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => GetID_ColumnName( _database.Instance );
    }
    public ImmutableDictionary<SqlStatement, string> this[ DbInstance key ]
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _cache[key];
    }
    public string this[ DbInstance key, SqlStatement statement ]
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _cache[key][statement];
    }
    public IEnumerable<string> KeyValuePairs
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => Descriptors.Select( x => x.KeyValuePair );
    }
    public string LastModified
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => GetLastModified( _database.Instance );
    }
    public string OwnerUserID
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => GetOwnerUserID( _database.Instance );
    }
    public string RandomMethod
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => GetRandomMethod( _database.Instance );
    }
    public string SchemaTableName
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => $"{_database.CurrentSchema}.{TableName}";
    }
    public string TableName
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => GetTableName( _database.Instance );
    }
    public IEnumerable<string> VariableNames
    {
        [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => Descriptors.Select( x => x.VariableName );
    }


    protected SqlCache( IConnectableDbRoot db )
    {
        _database = db;
        if ( TRecord.TableName != typeof(TRecord).GetTableName() ) { throw new InvalidOperationException( $"{TRecord.TableName} != {typeof(TRecord).GetTableName()}" ); }

        Reset();
    }
    public void Reset()
    {
        _cache.Clear();

        foreach ( DbInstance instance in _instances.Span )
        {
            _cache[instance] = instance switch
                               {
                                   DbInstance.MsSql    => Get_MsSql(),
                                   DbInstance.Postgres => Get_PostgresSql(),
                                   _                   => throw new OutOfRangeException( nameof(instance), instance )
                               };
        }
    }


    public bool ContainsKey( DbInstance key )                         => _cache.ContainsKey( key );
    public bool ContainsKey( DbInstance key, SqlStatement statement ) => TryGetValue( key, statement, out _ );
    public bool TryGetValue( DbInstance key, SqlStatement statement, [ NotNullWhen( true ) ] out string? sql )
    {
        if ( _cache.TryGetValue( key, out ImmutableDictionary<SqlStatement, string>? dict ) && dict.TryGetValue( statement, out sql ) ) { return true; }

        sql = null;
        return false;
    }


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    protected virtual ImmutableDictionary<SqlStatement, string> Get_PostgresSql()
    {
        string schemaTableName = SchemaTableName;
        string dateCreated     = GetDateCreated( DbInstance.Postgres );
        string idColumnName    = GetID_ColumnName( DbInstance.Postgres );
        string randomMethod    = GetRandomMethod( DbInstance.Postgres );
        string ownerUserID     = GetOwnerUserID( DbInstance.MsSql );
        string keyValuePairs   = string.Join( ',', KeyValuePairs );
        string columnNames     = string.Join( ',', ColumnNames );

        var dict = new Dictionary<SqlStatement, string>
                   {
                       [SqlStatement.All]             = $"SELECT * FROM {schemaTableName}",
                       [SqlStatement.Update]          = $"UPDATE {schemaTableName} SET {keyValuePairs} WHERE {idColumnName} = @{ID};",
                       [SqlStatement.Single]          = $"SELECT * FROM {schemaTableName} WHERE {idColumnName} = @{idColumnName}",
                       [SqlStatement.Random]          = $"SELECT TOP 1 * FROM {schemaTableName} ORDER BY {randomMethod}",
                       [SqlStatement.RandomCount]     = $"SELECT TOP @{COUNT} * FROM {schemaTableName} ORDER BY {randomMethod}",
                       [SqlStatement.RandomUserCount] = $"SELECT TOP @{COUNT} * FROM {schemaTableName} WHERE {ownerUserID} = @{ownerUserID} ORDER BY {randomMethod}",
                       [SqlStatement.Next]            = @$"SELECT * FROM {schemaTableName} WHERE ( id = IFNULL((SELECT MIN({idColumnName}) FROM {schemaTableName} WHERE {idColumnName} > @{ID}), 0) )",
                       [SqlStatement.SortedIDs]       = @$"SELECT {idColumnName}, {dateCreated} FROM {schemaTableName} ORDER BY {dateCreated} DESC",
                       [SqlStatement.NextID]          = @$"SELECT {idColumnName} FROM {schemaTableName} WHERE ( id = IFNULL((SELECT MIN({idColumnName}) FROM {schemaTableName} WHERE {idColumnName} > @{ID}), 0) )",
                       [SqlStatement.Last]            = $"SELECT * FROM {schemaTableName} ORDER BY {idColumnName} DESC LIMIT 1",
                       [SqlStatement.SingleInsert]    = $"INSERT INTO {schemaTableName} ({columnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )});",
                       [SqlStatement.TryInsert] = $"""
                                                   IF NOT EXISTS(SELECT * FROM {schemaTableName} WHERE @where)
                                                   BEGIN
                                                       INSERT INTO {schemaTableName} ({columnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
                                                   END

                                                   ELSE
                                                   BEGIN
                                                       SELECT {idColumnName} = NULL
                                                   END
                                                   """,
                       [SqlStatement.InsertOrUpdate] = $"""
                                                        IF NOT EXISTS(SELECT * FROM {schemaTableName} WHERE @where)
                                                        BEGIN
                                                            INSERT INTO {schemaTableName} ({columnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
                                                        END

                                                        ELSE
                                                        BEGIN
                                                            UPDATE {schemaTableName} SET {keyValuePairs} WHERE {idColumnName} = @{ID};
                                                        
                                                            SELECT TOP 1 {idColumnName} FROM {schemaTableName} WHERE @where
                                                        END
                                                        """,
                       [SqlStatement.First]  = $"SELECT TOP 1 * FROM {schemaTableName} ORDER BY {dateCreated} ASC LIMIT 1",
                       [SqlStatement.Delete] = $"DELETE FROM {schemaTableName} WHERE {idColumnName} = @{ID};",
                       [SqlStatement.Count]  = $"SELECT COUNT({idColumnName}) FROM {schemaTableName}"
                   };

        return dict.ToImmutableDictionary();
    }


    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    protected virtual ImmutableDictionary<SqlStatement, string> Get_MsSql()
    {
        string schemaTableName = SchemaTableName;
        string dateCreated     = GetDateCreated( DbInstance.MsSql );
        string idColumnName    = GetID_ColumnName( DbInstance.MsSql );
        string randomMethod    = GetRandomMethod( DbInstance.MsSql );
        string ownerUserID     = GetOwnerUserID( DbInstance.MsSql );
        string keyValuePairs   = string.Join( ',', KeyValuePairs );
        string columnNames     = string.Join( ',', ColumnNames );

        var dict = new Dictionary<SqlStatement, string>
                   {
                       [SqlStatement.All]             = $"SELECT * FROM {schemaTableName}",
                       [SqlStatement.Update]          = $"UPDATE {schemaTableName} SET {keyValuePairs} WHERE {idColumnName} = @{idColumnName};",
                       [SqlStatement.Single]          = $"SELECT * FROM {schemaTableName} WHERE {idColumnName} = @{idColumnName}",
                       [SqlStatement.Random]          = $"SELECT * FROM {schemaTableName} ORDER BY {randomMethod} LIMIT 1",
                       [SqlStatement.RandomCount]     = $"SELECT * FROM {schemaTableName} ORDER BY {randomMethod} LIMIT @{COUNT}",
                       [SqlStatement.RandomUserCount] = $"SELECT * FROM {schemaTableName} WHERE {ownerUserID} = @{ownerUserID} ORDER BY {randomMethod} LIMIT @{COUNT}",
                       [SqlStatement.Next]            = @$"SELECT * FROM {schemaTableName} WHERE ( id = IFNULL((SELECT MIN({idColumnName}) FROM {schemaTableName} WHERE {idColumnName} > @{ID}), 0) )",
                       [SqlStatement.SortedIDs]       = @$"SELECT {idColumnName}, {dateCreated} FROM {schemaTableName} ORDER BY {dateCreated} DESC",
                       [SqlStatement.NextID]          = @$"SELECT {idColumnName} FROM {schemaTableName} WHERE ( id = IFNULL((SELECT MIN({idColumnName}) FROM {schemaTableName} WHERE {idColumnName} > @{ID}), 0) )",
                       [SqlStatement.Last]            = $"SELECT * FROM {schemaTableName} ORDER BY {idColumnName} DESC LIMIT 1",
                       [SqlStatement.SingleInsert]    = $"SET NOCOUNT ON INSERT INTO {schemaTableName} ({columnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )});",
                       [SqlStatement.TryInsert] = $"""
                                                   IF NOT EXISTS(SELECT * FROM {schemaTableName} WHERE @where)
                                                   BEGIN
                                                       SET NOCOUNT ON INSERT INTO {schemaTableName} ({columnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
                                                   END

                                                   ELSE
                                                   BEGIN
                                                       SELECT {idColumnName} = NULL
                                                   END
                                                   """,
                       [SqlStatement.InsertOrUpdate] = $"""
                                                        IF NOT EXISTS(SELECT * FROM {schemaTableName} WHERE @where)
                                                        BEGIN
                                                            SET NOCOUNT ON INSERT INTO {schemaTableName} ({columnNames}) OUTPUT INSERTED.ID values ({string.Join( ',', VariableNames )})
                                                        END

                                                        ELSE
                                                        BEGIN
                                                            UPDATE {schemaTableName} SET {keyValuePairs} WHERE {idColumnName} = @{ID};
                                                        
                                                            SELECT {idColumnName} FROM {schemaTableName} WHERE @where LIMIT 1
                                                        END
                                                        """,
                       [SqlStatement.First]  = $"SELECT * FROM {schemaTableName} ORDER BY {dateCreated} ASC LIMIT 1",
                       [SqlStatement.Delete] = $"DELETE FROM {schemaTableName} WHERE {idColumnName} = @{ID};",
                       [SqlStatement.Count]  = $"SELECT COUNT({idColumnName}) FROM {schemaTableName}"
                   };

        return dict.ToImmutableDictionary();
    }


    [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
    protected static string GetCreatedBy( in DbInstance instance ) =>
        instance switch
        {
            DbInstance.Postgres => $"{QUOTE}{CREATED_BY}{QUOTE}",
            DbInstance.MsSql    => CREATED_BY,
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };
    [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
    protected static string GetID_ColumnName( in DbInstance instance ) =>
        instance switch
        {
            DbInstance.Postgres => $"{QUOTE}{ID}{QUOTE}",
            DbInstance.MsSql    => ID,
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };
    [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
    protected static string GetLastModified( in DbInstance instance ) =>
        instance switch
        {
            DbInstance.Postgres => $"{QUOTE}{LAST_MODIFIED}{QUOTE}",
            DbInstance.MsSql    => LAST_MODIFIED,
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };
    [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
    protected static string GetOwnerUserID( in DbInstance instance ) =>
        instance switch
        {
            DbInstance.Postgres => $"{QUOTE}{OWNER_USER_ID}{QUOTE}",
            DbInstance.MsSql    => OWNER_USER_ID,
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };
    [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
    protected static string GetTableName( in DbInstance instance ) =>
        instance switch
        {
            DbInstance.Postgres => $"{QUOTE}{TRecord.TableName}{QUOTE}",
            DbInstance.MsSql    => TRecord.TableName,
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };
    [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
    protected static string GetRandomMethod( in DbInstance instance ) =>
        instance switch
        {
            DbInstance.MsSql    => "NEWID()",
            DbInstance.Postgres => "RANDOM()",
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };
    [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
    protected static string GetDateCreated( in DbInstance instance ) =>
        instance switch
        {
            DbInstance.Postgres => $"{QUOTE}{DATE_CREATED}{QUOTE}",
            DbInstance.MsSql    => DATE_CREATED,
            _                   => throw new OutOfRangeException( nameof(instance), instance )
        };



    public enum SqlStatement : ulong
    {
        All,
        First,
        Last,
        SortedIDs,
        Delete,
        InsertOrUpdate,
        Next,
        NextID,
        Count,
        RandomCount,
        Random,
        RandomUserCount,
        Single,
        SingleInsert,
        TryInsert,
        Update
    }
}
