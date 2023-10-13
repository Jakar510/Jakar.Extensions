// Jakar.Extensions :: Jakar.Database
// 10/10/2023  10:37 AM

namespace Jakar.Database;


public partial class DbTable<TRecord>
{
    [ SuppressMessage( "ReSharper", "StaticMemberInGenericType" ), SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" ), SuppressMessage( "ReSharper", "OutParameterValueIsAlwaysDiscarded.Global" ) ]
    protected class SqlCache
    {
        private readonly ConcurrentDictionary<DbInstance, SqlSet> _cache = new();
        private readonly DbTable<TRecord>                         _table;


        public IEnumerable<string> ColumnNames
        {
            [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ] get => _table.Descriptors.Select( x => x.ColumnName );
        }
        public int Count
        {
            [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _cache.Count;
        }
        public virtual string CreatedBy
        {
            [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
            get => _table.Instance switch
                   {
                       DbInstance.Postgres => $"{QUOTE}{CREATED_BY}{QUOTE}",
                       DbInstance.MsSql    => CREATED_BY,
                       _                   => throw new OutOfRangeException( nameof(Instance), _table.Instance )
                   };
        }
        public virtual string DateCreated
        {
            [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
            get => _table.Instance switch
                   {
                       DbInstance.Postgres => $"{QUOTE}{DATE_CREATED}{QUOTE}",
                       DbInstance.MsSql    => DATE_CREATED,
                       _                   => throw new OutOfRangeException( nameof(Instance), _table.Instance )
                   };
        }
        public virtual string ID_ColumnName
        {
            [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
            get => _table.Instance switch
                   {
                       DbInstance.Postgres => $"{QUOTE}{ID}{QUOTE}",
                       DbInstance.MsSql    => ID,
                       _                   => throw new OutOfRangeException( nameof(Instance), _table.Instance )
                   };
        }
        public SqlSet this[ DbInstance key ]
        {
            [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _cache[key];
        }
        public string this[ DbInstance key, SqlStatement statement ]
        {
            [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _cache[key][statement];
        }
        public IEnumerable<string> KeyValuePairs
        {
            [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ] get => _table.Descriptors.Select( x => x.KeyValuePair );
        }
        public virtual string LastModified
        {
            [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
            get => _table.Instance switch
                   {
                       DbInstance.Postgres => $"{QUOTE}{LAST_MODIFIED}{QUOTE}",
                       DbInstance.MsSql    => LAST_MODIFIED,
                       _                   => throw new OutOfRangeException( nameof(Instance), _table.Instance )
                   };
        }
        public virtual string OwnerUserID
        {
            [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
            get => _table.Instance switch
                   {
                       DbInstance.Postgres => $"{QUOTE}{OWNER_USER_ID}{QUOTE}",
                       DbInstance.MsSql    => OWNER_USER_ID,
                       _                   => throw new OutOfRangeException( nameof(Instance), _table.Instance )
                   };
        }
        public virtual string SchemaTableName
        {
            [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ] get => $"{_table.CurrentSchema}.{TableName}";
        }
        public virtual string TableName
        {
            [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
            get =>
                _table.Instance switch
                {
                    DbInstance.Postgres => $"{QUOTE}{TRecord.TableName}{QUOTE}",
                    DbInstance.MsSql    => TRecord.TableName,
                    _                   => TRecord.TableName
                };
        }
        public IEnumerable<string> VariableNames
        {
            [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ] get => _table.Descriptors.Select( x => x.VariableName );
        }
        public virtual string RandomMethod
        {
            [ MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
            get => _table.Instance switch
                   {
                       DbInstance.MsSql    => "NEWID()",
                       DbInstance.Postgres => "RANDOM()",
                       _                   => throw new OutOfRangeException( nameof(Instance), _table.Instance )
                   };
        }


        public SqlCache( DbTable<TRecord> db )
        {
            _table = db;
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
            if ( _cache.TryGetValue( key, out SqlSet? dict ) && dict.TryGetValue( statement, out sql ) ) { return true; }

            sql = null;
            return false;
        }


        protected virtual SqlSet Get_PostgresSql()
        {
            var dict = new Dictionary<SqlStatement, string>
                       {
                           [SqlStatement.All]    = $"SELECT * FROM {SchemaTableName}",
                           [SqlStatement.Update] = $"UPDATE {SchemaTableName} SET {string.Join( ',', KeyValuePairs )} WHERE {ID_ColumnName} = @{ID_ColumnName};" +
                           [SqlStatement.Single] = $"SELECT * FROM {SchemaTableName} WHERE {ID_ColumnName} = @{ID_ColumnName}",
                           [SqlStatement.Random] = $"SELECT * FROM {SchemaTableName} ORDER BY {RandomMethod} LIMIT 1",
                       };

            return new SqlSet( ImmutableDictionary.CreateRange( dict ) );
        }


        protected virtual SqlSet Get_MsSql()
        {
            var dict = new Dictionary<SqlStatement, string>
                       {
                           [SqlStatement.All]    = $"SELECT * FROM {SchemaTableName}",
                           [SqlStatement.Update] = $"UPDATE {SchemaTableName} SET {string.Join( ',', KeyValuePairs )} WHERE {ID_ColumnName} = @{ID_ColumnName};",
                           [SqlStatement.Single] = $"SELECT * FROM {SchemaTableName} WHERE {ID_ColumnName} = @{ID_ColumnName}",
                           [SqlStatement.Random] = $"SELECT TOP 1 * FROM {SchemaTableName} ORDER BY {RandomMethod}",
                       };

            return new SqlSet( ImmutableDictionary.CreateRange( dict ) );
        }



        public sealed class SqlSet
        {
            private readonly ImmutableDictionary<SqlStatement, string> _cache;

            public ConcurrentDictionary<int, string> DeleteGuids { get; } = new();
            public string this[ SqlStatement statement ]
            {
                [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] get => _cache[statement];
            }
            public ConcurrentDictionary<string, string> Where           { get; } = new();
            public ConcurrentDictionary<int, string>    WhereParameters { get; } = new();


            public SqlSet( ImmutableDictionary<SqlStatement, string> cache )
            {
                Debug.Assert( _statements.Length == cache.Count );
                _cache = cache;
            }


            [ MethodImpl( MethodImplOptions.AggressiveInlining ) ] public bool TryGetValue( SqlStatement statement, [ NotNullWhen( true ) ] out string? sql ) => _cache.TryGetValue( statement, out sql );
        }
    }



    protected internal enum SqlStatement : ulong
    {
        All,
        First,
        FirstOrDefault,
        Last,
        LastOrDefault,
        SortedIDs,
        Delete,
        InsertOrUpdate,
        Next,
        NextID,
        RandomCount,
        Random,
        RandomUserCount,
        Single,
        SingleInsert,
        TryInsert,
        Update
    }
}
