namespace Jakar.Database;


[Serializable]
[DataBaseType(DbType.String)]
public sealed class RecordCollectionWrapper<TRecord, TOwner, TID> : CollectionWrapper<TRecord, TOwner, TID> where TRecord : BaseTableRecord<TRecord, TID>
                                                                                                            where TOwner : BaseTableRecord<TOwner, TID>
                                                                                                            where TID : IComparable<TID>, IEquatable<TID>
{
    public RecordCollectionWrapper( TOwner record ) : base(record) { }

    // public RecordCollectionWrapper( TOwner record, ReadOnlySpan<char>          span ) : base(record, span) { }
    public RecordCollectionWrapper( TOwner record, string?                     json ) : base(record, json) { }
    public RecordCollectionWrapper( TOwner record, params TRecord[]?           items ) : base(record, items) { }
    public RecordCollectionWrapper( TOwner record, ICollection<TRecord>?       collection ) : base(record, collection) { }
    public RecordCollectionWrapper( TOwner record, IDCollection<TRecord, TID>? collection ) : base(record, collection) { }


    public new static RecordCollectionWrapper<TRecord, TOwner, TID> Create( TOwner record, ICollection<TRecord>? items )
    {
        if ( items is null ) { return new RecordCollectionWrapper<TRecord, TOwner, TID>(record); }

        var collection = new RecordCollectionWrapper<TRecord, TOwner, TID>(record, items);

        return collection;
    }
    public new static RecordCollectionWrapper<TRecord, TOwner, TID> Create( TOwner record, string? jsonOrCsv )
    {
        if ( string.IsNullOrWhiteSpace(jsonOrCsv) ) { return new RecordCollectionWrapper<TRecord, TOwner, TID>(record); }

        jsonOrCsv = jsonOrCsv.Replace("\"", string.Empty);

        return new RecordCollectionWrapper<TRecord, TOwner, TID>(record, IDCollection<TRecord, TID>.Create(jsonOrCsv));
    }


    public bool Equals( RecordCollectionWrapper<TRecord, TOwner, TID>? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return Json == other.Json;
    }
    public override bool Equals( object? obj ) => ReferenceEquals(this, obj) || ( obj is RecordCollectionWrapper<TRecord, TOwner, TID> other && Equals(other) );
    public override int GetHashCode() => HashCode.Combine(Json);


    public async IAsyncEnumerable<TRecord> Iter( DbConnection connection, DbTransaction? transaction, DbTable<TRecord, TID> table, [EnumeratorCancellation] CancellationToken token )
    {
        ArgumentNullException.ThrowIfNull(connection);
        ArgumentNullException.ThrowIfNull(table);

        foreach ( TID id in Items ) { yield return await table.Get(connection, transaction, id, token); }
    }


    public static bool operator ==( RecordCollectionWrapper<TRecord, TOwner, TID>? left, RecordCollectionWrapper<TRecord, TOwner, TID>? right ) => Equals(left, right);
    public static bool operator !=( RecordCollectionWrapper<TRecord, TOwner, TID>? left, RecordCollectionWrapper<TRecord, TOwner, TID>? right ) => !Equals(left, right);
}
