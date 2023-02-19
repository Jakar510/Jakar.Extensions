namespace Jakar.Database;


public interface ICreateMapping<out TSelf, in TKey, in TValue> where TValue : TableRecord<TValue>
                                                               where TKey : TableRecord<TKey>
                                                               where TSelf : Mapping<TSelf, TKey, TValue>, ICreateMapping<TSelf, TKey, TValue>
{
    [RequiresPreviewFeatures] public abstract static TSelf Create( TKey key, TValue value );
}



public interface ICreateMapping<out TSelf, in TValue> : ICreateMapping<TSelf, UserRecord, TValue> where TValue : TableRecord<TValue>
                                                                                                  where TSelf : Mapping<TSelf, TValue>, ICreateMapping<TSelf, TValue> { }



[Serializable]
public abstract record Mapping<TSelf, TValue> : Mapping<TSelf, UserRecord, TValue> where TValue : TableRecord<TValue>
                                                                                   where TSelf : Mapping<TSelf, TValue>, ICreateMapping<TSelf, TValue>
{
    protected Mapping() { }
    protected Mapping( UserRecord key,   TValue     value ) : this( key, key, value ) { }
    protected Mapping( UserRecord owner, UserRecord key, TValue value ) : base( owner, key, value ) { }
}



[Serializable]
public abstract record Mapping<TSelf, TKey, TValue> : TableRecord<TSelf> where TValue : TableRecord<TValue>
                                                                         where TKey : TableRecord<TKey>
                                                                         where TSelf : Mapping<TSelf, TKey, TValue>, ICreateMapping<TSelf, TKey, TValue>
{
    private WeakReference<TKey>?   _owner;
    private WeakReference<TValue>? _value;


    public string OwnerID { get; init; } = string.Empty;
    public string ValueID { get; init; } = string.Empty;


    protected Mapping() { }
    protected Mapping( TKey key, TValue value )
    {
        _owner      = new WeakReference<TKey>( key );
        _value      = new WeakReference<TValue>( value );
        DateCreated = DateTimeOffset.UtcNow;
        OwnerID     = key.ID;
        ValueID     = value.ID;
    }
    protected Mapping( UserRecord user, TKey key, TValue value ) : base( user )
    {
        _owner  = new WeakReference<TKey>( key );
        _value  = new WeakReference<TValue>( value );
        OwnerID = key.ID;
        ValueID = value.ID;
    }


    public async ValueTask<TKey?> Get( DbConnection connection, DbTransaction transaction, DbTable<TKey> table, CancellationToken token )
    {
        if ( _owner is not null && _owner.TryGetTarget( out TKey? value ) ) { return value; }

        TKey? record = await table.Get( connection, transaction, OwnerID, token );
        if ( record is not null ) { _owner = new WeakReference<TKey>( record ); }

        return record;
    }
    public async ValueTask<TValue?> Get( DbConnection connection, DbTransaction transaction, DbTable<TValue> table, CancellationToken token )
    {
        if ( _value is not null && _value.TryGetTarget( out TValue? value ) ) { return value; }

        TValue? record = await table.Get( connection, transaction, ValueID, token );
        if ( record is not null ) { _value = new WeakReference<TValue>( record ); }

        return record;
    }


    public override int CompareTo( TSelf? other )
    {
        if ( other is null ) { return -1; }

        if ( ReferenceEquals( this, other ) ) { return 0; }

        int ownerComparision = string.Compare( OwnerID, other.OwnerID, StringComparison.Ordinal );
        if ( ownerComparision == 0 ) { return ownerComparision; }

        return string.Compare( ValueID, other.ValueID, StringComparison.Ordinal );
    }
    public override bool Equals( TSelf? other )
    {
        if ( other is null ) { return false; }

        return ReferenceEquals( this, other ) || ValueID == other.ValueID && OwnerID == other.ValueID;
    }
    public override int GetHashCode() => HashCode.Combine( OwnerID, ValueID );


    [RequiresPreviewFeatures]
    public static async ValueTask<bool> TryAdd( DbConnection connection, DbTransaction transaction, DbTable<TSelf> table, TKey key, TValue value, CancellationToken token )
    {
        if ( await Exists( connection, transaction, table, key, value, token ) ) { return false; }

        var   record = TSelf.Create( key, value );
        TSelf self   = await table.Insert( connection, transaction, record, token );
        return !string.IsNullOrEmpty( self.ID );
    }
    [RequiresPreviewFeatures]
    public static async ValueTask TryAdd( DbConnection connection, DbTransaction transaction, DbTable<TSelf> table, TKey key, IEnumerable<TValue> values, CancellationToken token )
    {
        TSelf[] records = await Where( connection, transaction, table, key, token );

        foreach ( TValue value in values )
        {
            if ( Exists( records, value ) ) { continue; }

            var record = TSelf.Create( key, value );
            await table.Insert( connection, transaction, record, token );
        }
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private static bool Exists( IEnumerable<TSelf> existing, TValue value )
    {
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach ( TSelf x in existing )
        {
            if ( x.ValueID == value.ID ) { return true; }
        }

        return false;
    }


    public static async ValueTask<bool> Exists( DbConnection connection, DbTransaction transaction, DbTable<TSelf> table, TKey key, TValue value, CancellationToken token )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(OwnerID), key.ID );
        parameters.Add( nameof(ValueID), value.ID );

        return await table.Exists( connection, transaction, true, parameters, token );
    }


    public static async ValueTask<TSelf[]> Where( DbConnection connection, DbTransaction transaction, DbTable<TSelf> table, TKey key, CancellationToken token )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(OwnerID), key.ID );
        return await table.Where( connection, transaction, true, parameters, token );
    }
    public static async IAsyncEnumerable<TValue?> Where( DbConnection connection, DbTransaction transaction, DbTable<TSelf> table, DbTable<TValue> valueTable, TKey key, [EnumeratorCancellation] CancellationToken token )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(OwnerID), key.ID );
        TSelf[] records = await table.Where( connection, transaction, true, parameters, token );

        foreach ( TSelf mapping in records ) { yield return await mapping.Get( connection, transaction, valueTable, token ); }
    }


    [RequiresPreviewFeatures]
    public static async ValueTask Replace( DbConnection connection, DbTransaction transaction, DbTable<TSelf> table, TKey key, IEnumerable<TValue> values, CancellationToken token )
    {
        await Delete( connection, transaction, table, key, token );

        foreach ( TValue value in values )
        {
            var record = TSelf.Create( key, value );
            await table.Insert( connection, transaction, record, token );
        }
    }


    public static async ValueTask Delete( DbConnection connection, DbTransaction transaction, DbTable<TSelf> table, TKey key, CancellationToken token )
    {
        TSelf[] records = await Where( connection, transaction, table, key, token );
        foreach ( TSelf mapping in records ) { await table.Delete( connection, transaction, mapping.ID, token ); }
    }
    public static async ValueTask Delete( DbConnection connection, DbTransaction transaction, DbTable<TSelf> table, TKey key, IEnumerable<TValue> values, CancellationToken token )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(OwnerID), key.ID );
        string sql = $"SELECT {nameof(IDataBaseID.ID)} FROM {table.TableName} WHERE {nameof(ValueID)} IN ( {string.Join( ',', values.Select( x => x.ID ) )} )";

        TSelf[] records = await table.Where( connection, transaction, sql, parameters, token );
        foreach ( TSelf mapping in records ) { await table.Delete( connection, transaction, mapping.ID, token ); }
    }
}
