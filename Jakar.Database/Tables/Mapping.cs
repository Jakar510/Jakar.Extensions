﻿namespace Jakar.Database;


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
    protected Mapping( UserRecord key, TValue value ) : base( key, value ) { }
}



[Serializable]
public abstract record Mapping<TSelf, TKey, TValue> : TableRecord<TSelf> where TValue : TableRecord<TValue>
                                                                         where TKey : TableRecord<TKey>
                                                                         where TSelf : Mapping<TSelf, TKey, TValue>, ICreateMapping<TSelf, TKey, TValue>
{
    private WeakReference<TKey>?   _owner;
    private WeakReference<TValue>? _value;


    [MaxLength( 256 )] public string KeyID   { get; init; } = string.Empty;
    [MaxLength( 256 )] public string ValueID { get; init; } = string.Empty;


    protected Mapping() { }
    protected Mapping( TKey key, TValue value ) : base( Guid.NewGuid() )
    {
        _owner  = new WeakReference<TKey>( key );
        _value  = new WeakReference<TValue>( value );
        KeyID   = key.ID;
        ValueID = value.ID;
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

        int ownerComparision = string.Compare( KeyID, other.KeyID, StringComparison.Ordinal );
        if ( ownerComparision == 0 ) { return ownerComparision; }

        return string.Compare( ValueID, other.ValueID, StringComparison.Ordinal );
    }
    public override bool Equals( TSelf? other )
    {
        if ( other is null ) { return false; }

        return ReferenceEquals( this, other ) || ValueID == other.ValueID && KeyID == other.ValueID;
    }
    public override int GetHashCode() => HashCode.Combine( KeyID, ValueID );


    [RequiresPreviewFeatures]
    public static async ValueTask<bool> TryAdd( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, TKey key, TValue value, CancellationToken token )
    {
        if ( await Exists( connection, transaction, selfTable, key, value, token ) ) { return false; }

        var   record = TSelf.Create( key, value );
        TSelf self   = await selfTable.Insert( connection, transaction, record, token );
        return !string.IsNullOrEmpty( self.ID );
    }
    [RequiresPreviewFeatures]
    public static async ValueTask TryAdd( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, TKey key, IEnumerable<TValue> values, CancellationToken token )
    {
        TSelf[] records = await Where( connection, transaction, selfTable, key, token );

        foreach ( TValue value in values )
        {
            if ( Exists( records, value ) ) { continue; }

            var record = TSelf.Create( key, value );
            await selfTable.Insert( connection, transaction, record, token );
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


    public static async ValueTask<bool> Exists( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, TKey key, TValue value, CancellationToken token ) =>
        await selfTable.Exists( connection, transaction, true, GetDynamicParameters( key, value ), token );


    public static async ValueTask<TSelf[]> Where( DbConnection connection, DbTransaction? transaction, DbTable<TSelf> selfTable, TKey key, CancellationToken token ) =>
        await selfTable.Where( connection, transaction, true, GetDynamicParameters( key ), token );
    public static async ValueTask<TValue[]> Where( DbConnection connection, DbTransaction? transaction, DbTable<TSelf> selfTable, DbTable<TValue> valueTable, TKey key, CancellationToken token )
    {
        string selfTableName  = selfTable.SchemaTableName;
        string valueTableName = valueTable.SchemaTableName;

        string sql = $@"SELECT * FROM {valueTableName}
INNER JOIN {selfTableName} ON {selfTableName}.{nameof(ValueID)} = {valueTableName}.{nameof(ID)} 
WHERE {selfTableName}.{nameof(KeyID)} = @{nameof(KeyID)}";

        token.ThrowIfCancellationRequested();
        return await valueTable.Where( connection, transaction, sql, GetDynamicParameters( key ), token );
    }
    public static async ValueTask<TKey[]> Where( DbConnection connection, DbTransaction? transaction, DbTable<TSelf> selfTable, DbTable<TKey> keyTable, TValue value, CancellationToken token )
    {
        string selfTableName = selfTable.SchemaTableName;
        string keyTableName  = keyTable.SchemaTableName;

        string sql = $@"SELECT * FROM {keyTableName}
INNER JOIN {selfTableName} ON {selfTableName}.{nameof(KeyID)} = {keyTableName}.{nameof(ID)} 
WHERE {selfTableName}.{nameof(ValueID)} = @{nameof(ValueID)}";

        token.ThrowIfCancellationRequested();
        return await keyTable.Where( connection, transaction, sql, GetDynamicParameters( value ), token );
    }


    [RequiresPreviewFeatures]
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
        TSelf[] records = await Where( connection, transaction, selfTable, key, token );
        foreach ( TSelf mapping in records ) { await selfTable.Delete( connection, transaction, mapping.ID, token ); }
    }
    public static async ValueTask Delete( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, TKey key, IEnumerable<TValue> values, CancellationToken token )
    {
        string sql = $"SELECT {nameof(ID)} FROM {selfTable.TableName} WHERE {nameof(ValueID)} IN ( {string.Join( ',', values.Select( x => x.ID ) )} )";

        TSelf[] records = await selfTable.Where( connection, transaction, sql, GetDynamicParameters( key ), token );
        foreach ( TSelf mapping in records ) { await selfTable.Delete( connection, transaction, mapping.ID, token ); }
    }
    public static async ValueTask Delete( DbConnection connection, DbTransaction transaction, DbTable<TSelf> selfTable, TKey key, TValue value, CancellationToken token )
    {
        string sql = $"SELECT {nameof(ID)} FROM {selfTable.TableName} WHERE {nameof(ValueID)} = @{nameof(ValueID)}";
        selfTable.Delete( connection, transaction, true, GetDynamicParameters( key, value ), token );
        TSelf[] records = await selfTable.Where( connection, transaction, sql, GetDynamicParameters( key, value ), token );
        foreach ( TSelf mapping in records ) { await selfTable.Delete( connection, transaction, mapping.ID, token ); }
    }
}