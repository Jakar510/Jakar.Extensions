// Jakar.Extensions :: Jakar.Database
// 09/01/2022  6:40 PM

using System.Collections;



namespace Jakar.Database;


public sealed class Mapping<TRecord, TID> : IDictionary<TID, TRecord>, IEnumerator<TRecord> where TRecord : BaseTableRecord<TRecord, TID>
                                                                                            where TID : IComparable<TID>, IEquatable<TID>
{
    private readonly SortedIDs                                    _hashes  = new();
    private readonly ObservableConcurrentDictionary<TID, TRecord> _records = new();
    private readonly HashSet<TID>                                 _changed = new();


    public bool                 HasChanged     => _changed.Count > 0;
    public IEnumerable<TID>     Changed        => _changed;
    public IEnumerable<TRecord> RecordsChanged => _changed.Select(x => _records[x]);


    public TRecord this[ TID key ]
    {
        get
        {
            TRecord record = _records[_hashes.Current];
            _hashes[record.ID] = record.GetHashCode();
            return record;
        }
        set
        {
            _records[key] = value;

            if ( _hashes[value.ID] != value.GetHashCode() ) { _changed.Add(value.ID); }
        }
    }
    public ICollection<TID>     Keys       => _records.Keys;
    public ICollection<TRecord> Values     => _records.Values;
    public TRecord              Current    => this[_hashes.Current];
    object IEnumerator.         Current    => Current;
    public int                  Count      => _records.Count;
    public bool                 IsReadOnly => _records.IsReadOnly;


    public Mapping() : base() => _records.CollectionChanged += OnCollectionChanged;
    public void Dispose()
    {
        _records.CollectionChanged -= OnCollectionChanged;
        _hashes.Dispose();
        _changed.Clear();
    }
    public bool MoveNext() => _hashes.MoveNext();
    public void Reset() => _hashes.Reset();


    // private void ValuesChanged( IEnumerable<KeyValuePair<TID, TRecord>> ids ) => _changed.Add(ids.Select(x => x.Key));
    // private void ValuesChanged( IEnumerable<TRecord>                    ids ) => _changed.Add(ids.Select(x => x.ID));
    private void ValuesChanged( IEnumerable<TRecord> ids ) => _changed.Add(ids.Select(x => x.ID));
    private void Added( IEnumerable<TRecord>         ids ) => _hashes.Add(ids);
    private void Removed( IEnumerable<TRecord>       ids ) => _hashes.Remove(ids);
    private void OnCollectionChanged( object? sender, NotifyCollectionChangedEventArgs e )
    {
        IEnumerable<TRecord> ids = e.NewItems switch
                                   {
                                       IEnumerable<TRecord> values                    => values,
                                       IEnumerable<KeyValuePair<TID, TRecord>> values => values.Select(x => x.Value),
                                       _                                              => throw new ExpectedValueTypeException(nameof(e.NewItems), e.NewItems, typeof(IEnumerable<TID>), typeof(IEnumerable<TRecord>), typeof(KeyValuePair<TID, TRecord>))
                                   };


        switch ( e.Action )
        {
            case NotifyCollectionChangedAction.Add:
            {
                Added(ids);
                return;
            }

            case NotifyCollectionChangedAction.Remove:
            {
                Removed(ids);
                return;
            }

            case NotifyCollectionChangedAction.Reset:
            {
                _hashes.Clear();
                _changed.Clear();
                Added(Values);
                return;
            }


            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
            {
                ValuesChanged(ids);

                return;
            }

            default: throw new OutOfRangeException(nameof(e.Action), e.Action);
        }
    }


    public async Task Refresh( DbConnection connection, DbTransaction transaction, DbTable<TRecord, TID> table, CancellationToken token = default )
    {
        if ( !HasChanged ) { return; }

        await table.Update(connection, transaction, RecordsChanged, token);
        await AddOrUpdate(table.Get(connection, transaction, Changed, token), token);
    }


    public async Task AddOrUpdate( IAsyncEnumerable<TRecord> records, CancellationToken token = default )
    {
        await foreach ( TRecord record in records.WithCancellation(token) ) { AddOrUpdate(record); }
    }
    public void AddOrUpdate( IEnumerable<TRecord> records )
    {
        foreach ( TRecord record in records ) { AddOrUpdate(record); }
    }
    public void AddOrUpdate( TRecord record )
    {
        if ( _records.ContainsKey(record.ID) ) { _records[record.ID] = record; }
        else { _records.Add(record.ID, record); }

        _hashes.Add(record);
    }


    public void Add( TID                        key, TRecord value ) => _records.Add(key, value);
    public void Add( KeyValuePair<TID, TRecord> pair ) => Add(pair.Key, pair.Value);
    public void Clear() => _records.Clear();


    public bool Contains( KeyValuePair<TID, TRecord> pair ) => _records.Contains(pair);
    public bool ContainsKey( TID                     key ) => _records.ContainsKey(key);
    public bool ContainsValue( TRecord               pair ) => _records.ContainsValue(pair);


    public bool Remove( KeyValuePair<TID, TRecord> pair ) => _records.Remove(pair);
    public bool Remove( TID                        key ) => _records.Remove(key);


    public bool TryGetValue( TID                     key,   out TRecord value ) => _records.TryGetValue(key, out value);
    public void CopyTo( KeyValuePair<TID, TRecord>[] array, int         arrayIndex ) => _records.CopyTo(array, arrayIndex);


    public IEnumerator<KeyValuePair<TID, TRecord>> GetEnumerator() => _records.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ( (IEnumerable)_records ).GetEnumerator();



    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    private class SortedIDs : SortedList<TID, int>, IEnumerator<TID>
    {
        private int        _index;
        public  TID        Current => Keys[_index];
        object IEnumerator.Current => Current;


        public SortedIDs() : base(Comparer<TID>.Default) { }


        public void Add( IEnumerable<TRecord> records )
        {
            foreach ( TRecord record in records ) { Add(record); }
        }
        public void Add( TRecord record ) => this[record.ID] = record.GetHashCode();

        public void Remove( IEnumerable<TRecord> records )
        {
            foreach ( TRecord record in records ) { Remove(record); }
        }
        public void Remove( TRecord record ) => Remove(record.ID);


        public bool MoveNext() => ++_index < Count;
        public void Reset() => _index = 0;
        public void Dispose() => Clear();
    }
}
