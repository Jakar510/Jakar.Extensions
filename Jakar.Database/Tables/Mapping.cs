namespace Jakar.Database;


public interface ICreateMapping<out TSelf, in TKey, in TValue> where TValue : TableRecord<TValue>, IDbReaderMapping<TValue>
                                                               where TKey : TableRecord<TKey>, IDbReaderMapping<TKey>
                                                               where TSelf : Mapping<TSelf, TKey, TValue>, ICreateMapping<TSelf, TKey, TValue>, IDbReaderMapping<TSelf>
{
    public abstract static TSelf Create( TKey key, TValue value );
}



[ Serializable ]
public abstract record Mapping<TSelf, TKey, TValue>( RecordID<TKey> KeyID, RecordID<TValue> ValueID, RecordID<TSelf> ID, DateTimeOffset DateCreated, DateTimeOffset? LastModified = default ) : TableRecord<TSelf>( ID, DateCreated, LastModified )
    where TValue : TableRecord<TValue>, IDbReaderMapping<TValue>
    where TKey : TableRecord<TKey>, IDbReaderMapping<TKey>
    where TSelf : Mapping<TSelf, TKey, TValue>, ICreateMapping<TSelf, TKey, TValue>, IDbReaderMapping<TSelf>
{
    private WeakReference<TKey>?   _owner;
    private WeakReference<TValue>? _value;


    protected Mapping( TKey key, TValue value ) : this( key.ID, value.ID, RecordID<TSelf>.New(), DateTimeOffset.UtcNow )
    {
        _owner = new WeakReference<TKey>( key );
        _value = new WeakReference<TValue>( value );
    }

    public override DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = base.ToDynamicParameters();
        parameters.Add( nameof(KeyID),   KeyID );
        parameters.Add( nameof(ValueID), ValueID );
        return parameters;
    }

    public static DynamicParameters GetDynamicParameters( TValue record )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(KeyID), record.ID );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( TKey key )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(KeyID), key.ID );
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( TKey key, TValue value )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(KeyID),   key.ID );
        parameters.Add( nameof(ValueID), value.ID );
        return parameters;
    }


    public async ValueTask<TKey?> Get( DbConnection connection, DbTransaction transaction, DbTable<TKey> selfTable, CancellationToken token )
    {
        if ( _owner is not null && _owner.TryGetTarget( out TKey? value ) ) { return value; }

        TKey? record = await selfTable.Get( connection, transaction, KeyID, token );
        if ( record is not null ) { _owner = new WeakReference<TKey>( record ); }

        return record;
    }
    public async ValueTask<TValue?> Get( DbConnection connection, DbTransaction transaction, DbTable<TValue> selfTable, CancellationToken token )
    {
        if ( _value is not null && _value.TryGetTarget( out TValue? value ) ) { return value; }

        TValue? record = await selfTable.Get( connection, transaction, ValueID, token );
        if ( record is not null ) { _value = new WeakReference<TValue>( record ); }

        return record;
    }


    public override int CompareTo( TSelf? other )
    {
        if ( other is null ) { return -1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int ownerComparision = KeyID.CompareTo( other.KeyID );
        if ( ownerComparision == 0 ) { return ownerComparision; }

        return ValueID.CompareTo( other.ValueID );
    }
    public override int GetHashCode() => HashCode.Combine( KeyID, ValueID );


    public static async ValueTask<bool> TryAdd( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, TKey key, TValue value, CancellationToken token )
    {
        if ( await Exists( connection, transaction, selfTable, key, value, token ) ) { return false; }

        var   record = TSelf.Create( key, value );
        TSelf self   = await selfTable.Insert( connection, transaction, record, token );
        return self.IsValidID();
    }
    public static async ValueTask TryAdd( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, DbTable<TValue> valueTable, TKey key, ImmutableArray<TValue> values, CancellationToken token )
    {
        // TODO: finish implementation

        var    parameters = GetDynamicParameters( key );
        string ids        = string.Join( ", ", values.Select( x => x.ID.Value ) );

        string sql = @$"
SELECT * FROM {TValue.TableName}
LEFT JOIN {TSelf.TableName}
WHERE 
    {TValue.TableName}.{nameof(ID)} != {TSelf.TableName}.{nameof(ValueID)} 
    AND {TValue.TableName}.{nameof(ID)} IN ( {ids} )
    AND {TSelf.TableName}.{nameof(ValueID)} NOT IN ( {ids} ) 
    AND {TSelf.TableName}.{nameof(KeyID)} = @{nameof(KeyID)}";


        await foreach ( TValue value in valueTable.Where( connection, transaction, sql, parameters, token ) )
        {
            var self = TSelf.Create( key, value );
            await selfTable.Insert( connection, transaction, self, token );
        }

        // foreach ( TSelf self in FilterExisting( records.GetArray(), key, values, caller ) ) { }
    }


    /*
    [ RequiresPreviewFeatures, MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
    private static IEnumerable<TSelf> FilterExisting( TSelf[] records, TKey key, IEnumerable<TValue> values )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TValue value in values )
        {
            if ( Exists( records, value ) ) { continue; }

            yield return TSelf.Create( key, value );
        }
    }

    [ RequiresPreviewFeatures, MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization ) ]
    private static TSelf? FilterExisting( in ReadOnlyMemory<TSelf> records, TKey key, IEnumerable<TValue> values )
    {
        foreach ( TValue value in values )
        {
            if ( Exists( records, value ) ) { continue; }

            return TSelf.Create( key, value );
        }
    }
    */

    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    private static bool Exists( ImmutableArray<TSelf> existing, TValue value )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TSelf self in existing )
        {
            if ( self.ValueID == value.ID ) { return true; }
        }

        return false;
    }

    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    private static bool Exists( ImmutableArray<TValue> existing, TSelf self )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TValue value in existing )
        {
            if ( value.ID == self.ValueID ) { return true; }
        }

        return false;
    }


    public static async ValueTask<bool> Exists( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, TKey key, TValue value, CancellationToken token ) =>
        await selfTable.Exists( connection, transaction, true, GetDynamicParameters( key, value ), token );


    public static IAsyncEnumerable<TSelf> Where( DbConnection connection, DbTransaction? transaction, DbTable<TSelf> selfTable, TKey key, CancellationToken token ) =>
        selfTable.Where( connection, transaction, true, GetDynamicParameters( key ), token );
    public static IAsyncEnumerable<TSelf> Where( DbConnection connection, DbTransaction? transaction, DbTable<TSelf> selfTable, TValue value, CancellationToken token ) =>
        selfTable.Where( connection, transaction, true, GetDynamicParameters( value ), token );
    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public static IAsyncEnumerable<TValue> Where( DbConnection connection, DbTransaction? transaction, DbTable<TSelf> selfTable, DbTable<TValue> valueTable, TKey key, CancellationToken token )
    {
        string sql = $@"SELECT * FROM {TValue.TableName}
INNER JOIN {TSelf.TableName} ON {TSelf.TableName}.{nameof(ValueID)} = {TValue.TableName}.{nameof(ID)} 
WHERE {TSelf.TableName}.{nameof(KeyID)} = @{nameof(KeyID)}";

        token.ThrowIfCancellationRequested();
        return valueTable.Where( connection, transaction, sql, GetDynamicParameters( key ), token );
    }
    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public static IAsyncEnumerable<TKey> Where( DbConnection connection, DbTransaction? transaction, DbTable<TSelf> selfTable, DbTable<TKey> keyTable, TValue value, CancellationToken token )
    {
        string sql = $@"SELECT * FROM {TKey.TableName}
INNER JOIN {TSelf.TableName} ON {TSelf.TableName}.{nameof(KeyID)} = {TKey.TableName}.{nameof(ID)} 
WHERE {TSelf.TableName}.{nameof(ValueID)} = @{nameof(ValueID)}";

        return keyTable.Where( connection, transaction, sql, GetDynamicParameters( value ), token );
    }


    public static async ValueTask Replace( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, TKey key, IEnumerable<TValue> values, CancellationToken token )
    {
        await Delete( connection, transaction, selfTable, key, token );

        foreach ( TValue value in values )
        {
            var record = TSelf.Create( key, value );
            await selfTable.Insert( connection, transaction, record, token );
        }
    }


    public static async ValueTask Delete( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, TKey key, CancellationToken token )
    {
        await foreach ( TSelf record in Where( connection, transaction, selfTable, key, token ) ) { await selfTable.Delete( connection, transaction, record.ID, token ); }
    }
    [ MethodImpl( MethodImplOptions.AggressiveOptimization ) ]
    public static async ValueTask Delete( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, TKey key, IEnumerable<TValue> values, CancellationToken token )
    {
        string sql = $"SELECT * FROM {TSelf.TableName} WHERE {nameof(ValueID)} IN ( {string.Join( ',', values.Select( x => x.ID ) )} ) AND {nameof(KeyID)} = @{nameof(KeyID)}";

        await foreach ( TSelf record in selfTable.Where( connection, transaction, sql, GetDynamicParameters( key ), token ) ) { await selfTable.Delete( connection, transaction, record.ID, token ); }
    }
    public static async ValueTask Delete( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, TKey key, TValue value, CancellationToken token ) =>
        await selfTable.Delete( connection, transaction, true, GetDynamicParameters( key, value ), token );
}
