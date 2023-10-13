// Jakar.Extensions :: Jakar.Database
// 10/10/2023  10:37 AM

namespace Jakar.Database;


public partial class DbTable<TRecord>
{
    protected sealed class SqlCache
    {
        private readonly ConcurrentDictionary<DbInstance, ConcurrentDictionary<SqlStatement, string?>> _cache = new();


        public int Count => _cache.Count;
        public string? this[ DbInstance key, SqlStatement statement ]
        {
            get => _cache[key][statement];
            set => _cache[key][statement] = value;
        }

        public SqlCache( IConnectableDb db )
        {
            foreach ( DbInstance instance in Enum.GetValues<DbInstance>() ) { _cache[instance] = new ConcurrentDictionary<SqlStatement, string?>(); }
        }

        public void Clear()
        {
            _cache.Values.ForEach( static x => x.Clear() );
            _cache.Clear();
        }


        public bool ContainsKey( DbInstance key )                         => _cache.ContainsKey( key );
        public bool ContainsKey( DbInstance key, SqlStatement statement ) => TryGetValue( key, statement, out _ );


        public bool TryGetValue( DbInstance key, SqlStatement statement, [ NotNullWhen( true ) ] out string? value )
        {
            if ( _cache.TryGetValue( key, out ConcurrentDictionary<SqlStatement, string?>? dict ) && dict.TryGetValue( statement, out string? result ) )
            {
                value = result;
                return !string.IsNullOrEmpty( value );
            }

            value = null;
            return false;
        }
    }



    protected enum SqlStatement
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
