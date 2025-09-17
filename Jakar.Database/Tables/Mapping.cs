namespace Jakar.Database;


public interface ICreateMapping<out TSelf, TKey, TValue>
    where TValue : class, ITableRecord<TValue>, IDbReaderMapping<TValue>
    where TKey : class, ITableRecord<TKey>, IDbReaderMapping<TKey>
    where TSelf : class, ITableRecord<TSelf>, ICreateMapping<TSelf, TKey, TValue>, IDbReaderMapping<TSelf>
{
    public abstract static TSelf Create( TKey           key, TValue           value );
    public abstract static TSelf Create( RecordID<TKey> key, RecordID<TValue> value );
}



[Serializable]
public abstract record Mapping<TSelf, TKey, TValue>( RecordID<TKey> KeyID, RecordID<TValue> ValueID, RecordID<TSelf> ID, DateTimeOffset DateCreated, DateTimeOffset? LastModified = null ) : TableRecord<TSelf>(in ID, in DateCreated, in LastModified)
    where TValue : class, ITableRecord<TValue>, IDbReaderMapping<TValue>
    where TKey : class, ITableRecord<TKey>, IDbReaderMapping<TKey>
    where TSelf : Mapping<TSelf, TKey, TValue>, ICreateMapping<TSelf, TKey, TValue>, IDbReaderMapping<TSelf>
{
    private WeakReference<TKey>?   __owner;
    private WeakReference<TValue>? __value;


    protected Mapping( RecordID<TKey> key, RecordID<TValue> value ) : this(key, value, RecordID<TSelf>.New(), DateTimeOffset.UtcNow) { }
    protected Mapping( TKey key, TValue value ) : this(key.ID, value.ID)
    {
        __owner = new WeakReference<TKey>(key);
        __value = new WeakReference<TValue>(value);
    }


    public override DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = base.ToDynamicParameters();
        parameters.Add(nameof(KeyID),   KeyID);
        parameters.Add(nameof(ValueID), ValueID);
        return parameters;
    }


    public static DynamicParameters GetDynamicParameters( TValue record ) => GetDynamicParameters(record.ID);
    public static DynamicParameters GetDynamicParameters( in RecordID<TValue> value )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(ValueID), value);
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( TKey key ) => GetDynamicParameters(key.ID);
    public static DynamicParameters GetDynamicParameters( in RecordID<TKey> key )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(KeyID), key);
        return parameters;
    }
    public static DynamicParameters GetDynamicParameters( TKey key, TValue value ) => GetDynamicParameters(key.ID, value.ID);
    public static DynamicParameters GetDynamicParameters( in RecordID<TKey> key, RecordID<TValue> value )
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(KeyID),   key);
        parameters.Add(nameof(ValueID), value);
        return parameters;
    }


    public async ValueTask<TKey?> Get( DbConnection connection, DbTransaction transaction, DbTable<TKey> selfTable, CancellationToken token )
    {
        if ( __owner is not null && __owner.TryGetTarget(out TKey? value) ) { return value; }

        TKey? record = await selfTable.Get(connection, transaction, KeyID, token);
        if ( record is not null ) { __owner = new WeakReference<TKey>(record); }

        return record;
    }
    public async ValueTask<TValue?> Get( DbConnection connection, DbTransaction transaction, DbTable<TValue> selfTable, CancellationToken token )
    {
        if ( __value is not null && __value.TryGetTarget(out TValue? value) ) { return value; }

        TValue? record = await selfTable.Get(connection, transaction, ValueID, token);
        if ( record is not null ) { __value = new WeakReference<TValue>(record); }

        return record;
    }


    public override bool Equals( TSelf? other ) => ReferenceEquals(this, other) || ( other is not null && KeyID == other.KeyID && ValueID == other.ValueID );
    public override int CompareTo( TSelf? other )
    {
        if ( other is null ) { return -1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        int ownerComparision = KeyID.CompareTo(other.KeyID);
        if ( ownerComparision == 0 ) { return ownerComparision; }

        return ValueID.CompareTo(other.ValueID);
    }
    public override int GetHashCode() => HashCode.Combine(KeyID, ValueID);


    public static async ValueTask<bool> TryAdd( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, RecordID<TKey> key, RecordID<TValue> value, CancellationToken token )
    {
        if ( await Exists(connection, transaction, selfTable, key, value, token) ) { return false; }

        TSelf record = TSelf.Create(key, value);
        TSelf self   = await selfTable.Insert(connection, transaction, record, token);
        return self.IsValidID();
    }
    public static async ValueTask TryAdd( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, DbTable<TValue> valueTable, RecordID<TKey> key, IEnumerable<RecordID<TValue>> values, CancellationToken token )
    {
        DynamicParameters parameters = GetDynamicParameters(key);
        string            ids        = string.Join(", ", values.Select(static x => x.value));

        string sql = $"""
                      SELECT * FROM {TValue.TableName}
                      LEFT JOIN {TSelf.TableName}
                      WHERE 
                          {TValue.TableName}.{nameof(ID)} != {TSelf.TableName}.{nameof(ValueID)} 
                          AND {TValue.TableName}.{nameof(ID)} IN ( {ids} )
                          AND {TSelf.TableName}.{nameof(ValueID)} NOT IN ( {ids} ) 
                          AND {TSelf.TableName}.{nameof(KeyID)} = @{nameof(KeyID)}
                      """;


        await foreach ( TValue value in valueTable.Where(connection, transaction, sql, parameters, token) )
        {
            TSelf self = TSelf.Create(key, value);
            await selfTable.Insert(connection, transaction, self, token);
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


    public static bool Exists( scoped in ReadOnlySpan<TSelf> existing, in RecordID<TValue> target )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TSelf self in existing )
        {
            if ( self.ValueID == target ) { return true; }
        }

        return false;
    }
    public static bool Exists( scoped in ReadOnlySpan<TValue> existing, in RecordID<TValue> target )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TValue value in existing )
        {
            if ( value.ID == target ) { return true; }
        }

        return false;
    }


    public static       ValueTask<bool>         Exists( DbConnection connection, DbTransaction  transaction, DbTable<TSelf> selfTable, TKey             key,   TValue                                     value, CancellationToken token ) => Exists(connection, transaction, selfTable, key.ID, value.ID, token);
    public static async ValueTask<bool>         Exists( DbConnection connection, DbTransaction  transaction, DbTable<TSelf> selfTable, RecordID<TKey>   key,   RecordID<TValue>                           value, CancellationToken token ) => await selfTable.Exists(connection, transaction, true, GetDynamicParameters(key, value), token);
    public static       IAsyncEnumerable<TSelf> Where( DbConnection  connection, DbTransaction? transaction, DbTable<TSelf> selfTable, TKey             key,   [EnumeratorCancellation] CancellationToken token ) => Where(connection, transaction, selfTable, key.ID, token);
    public static       IAsyncEnumerable<TSelf> Where( DbConnection  connection, DbTransaction? transaction, DbTable<TSelf> selfTable, RecordID<TKey>   key,   [EnumeratorCancellation] CancellationToken token ) => selfTable.Where(connection, transaction, true, GetDynamicParameters(key), token);
    public static       IAsyncEnumerable<TSelf> Where( DbConnection  connection, DbTransaction? transaction, DbTable<TSelf> selfTable, TValue           value, [EnumeratorCancellation] CancellationToken token ) => Where(connection, transaction, selfTable, value.ID, token);
    public static       IAsyncEnumerable<TSelf> Where( DbConnection  connection, DbTransaction? transaction, DbTable<TSelf> selfTable, RecordID<TValue> value, [EnumeratorCancellation] CancellationToken token ) => selfTable.Where(connection, transaction, true, GetDynamicParameters(value), token);


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static IAsyncEnumerable<TValue> Where( DbConnection connection, DbTransaction? transaction, DbTable<TValue> valueTable, RecordID<TKey> key, [EnumeratorCancellation] CancellationToken token )
    {
        string sql = $"""
                      SELECT * FROM {TValue.TableName}
                      INNER JOIN {TSelf.TableName} ON {TSelf.TableName}.{nameof(ValueID)} = {TValue.TableName}.{nameof(ID)} 
                      WHERE {TSelf.TableName}.{nameof(KeyID)} = @{nameof(KeyID)}
                      """;

        token.ThrowIfCancellationRequested();
        return valueTable.Where(connection, transaction, sql, GetDynamicParameters(key), token);
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static IAsyncEnumerable<TKey> Where( DbConnection connection, DbTransaction? transaction, DbTable<TKey> keyTable, RecordID<TValue> value, [EnumeratorCancellation] CancellationToken token )
    {
        string sql = $"""
                      SELECT * FROM {TKey.TableName}
                      INNER JOIN {TSelf.TableName} ON {TSelf.TableName}.{nameof(KeyID)} = {TKey.TableName}.{nameof(ID)} 
                      WHERE {TSelf.TableName}.{nameof(ValueID)} = @{nameof(ValueID)}
                      """;

        return keyTable.Where(connection, transaction, sql, GetDynamicParameters(value), token);
    }


    public static async ValueTask Replace( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, RecordID<TKey> key, IEnumerable<RecordID<TValue>> values, CancellationToken token )
    {
        await Delete(connection, transaction, selfTable, key, token);

        foreach ( RecordID<TValue> value in values )
        {
            TSelf record = TSelf.Create(key, value);
            await selfTable.Insert(connection, transaction, record, token);
        }
    }


    public static async ValueTask Delete( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, RecordID<TKey> key, CancellationToken token ) // TODO: OPTIMIZE THIS!!!
    {
        await foreach ( TSelf record in Where(connection, transaction, selfTable, key, token) ) { await selfTable.Delete(connection, transaction, record.ID, token); }
    }


    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static async ValueTask Delete( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, RecordID<TKey> key, IEnumerable<RecordID<TValue>> values, CancellationToken token )
    {
        string sql = $"SELECT * FROM {TSelf.TableName} WHERE {nameof(ValueID)} IN ( {string.Join(',', values.Select(static x => x.value))} ) AND {nameof(KeyID)} = @{nameof(KeyID)}";

        await foreach ( TSelf record in selfTable.Where(connection, transaction, sql, GetDynamicParameters(key), token) ) { await selfTable.Delete(connection, transaction, record.ID, token); }
    }
    public static async ValueTask Delete( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, RecordID<TKey> key, RecordID<TValue> value, CancellationToken token ) => await selfTable.Delete(connection, transaction, true, GetDynamicParameters(key, value), token);
}
