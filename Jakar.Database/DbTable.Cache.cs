using System.Collections;



namespace Jakar.Database;


public abstract partial class DbTable<TRecord, TID>
{
    public class Cache : IAsyncEnumerator<TRecord>, IReadOnlyCollection<TRecord>
    {
        protected readonly DbTable<TRecord, TID> _table;
        protected readonly HashSet<TID>          _changed = new();
        protected readonly Mapping               _records = new();
        protected readonly SortedList            _hashes  = new();


        // protected readonly SortedList _keys   = new();


        public TRecord Current    => _records[_hashes.Current];
        public int     Count      => _hashes.Count;
        public bool    IsReadOnly => false;


        public TRecord this[ TID index ]
        {
            get
            {
                TRecord record = _records[index];
                _hashes[record.ID] = record.GetHashCode();
                return record;
            }
            set
            {
                if ( _hashes[value.ID] != value.GetHashCode() ) { _changed.Add(value.ID); }

                _records[index] = value;
            }
        }


        public Cache( DbTable<TRecord, TID> table )
        {
            _table                     =  table;
            _records.CollectionChanged += RecordsOnCollectionChanged;
        }
        private void RecordsOnCollectionChanged( object? sender, NotifyCollectionChangedEventArgs e )
        {
            switch ( e.Action )
            {
                case NotifyCollectionChangedAction.Add: { return; }

                case NotifyCollectionChangedAction.Remove: { return; }

                case NotifyCollectionChangedAction.Reset:
                {
                    // Changed(_records.Keys);
                    return;
                }

                case NotifyCollectionChangedAction.Replace:
                {
                    switch ( e.NewItems )
                    {
                        case IEnumerable<TID> ids:
                        {
                            Changed(ids);
                            return;
                        }

                        case IEnumerable<TRecord> records:
                        {
                            Changed(records.Select(x => x.ID));
                            return;
                        }

                        case IEnumerable<KeyValuePair<TID, TRecord>> pairs:
                        {
                            Changed(pairs.Select(x => x.Key));
                            return;
                        }
                    }

                    throw new InvalidOperationException(nameof(NotifyCollectionChangedAction.Replace));
                }

                case NotifyCollectionChangedAction.Move:
                {
                    switch ( e.NewItems )
                    {
                        case IEnumerable<TID> ids:
                        {
                            Changed(ids);
                            return;
                        }

                        case IEnumerable<TRecord> records:
                        {
                            Changed(records.Select(x => x.ID));
                            return;
                        }

                        case IEnumerable<KeyValuePair<TID, TRecord>> pairs:
                        {
                            Changed(pairs.Select(x => x.Key));
                            return;
                        }
                    }

                    throw new InvalidOperationException(nameof(NotifyCollectionChangedAction.Move));
                }


                default: throw new ArgumentOutOfRangeException();
            }
        }
        public async ValueTask DisposeAsync()
        {
            await SaveAllToDB();
            _records.CollectionChanged -= RecordsOnCollectionChanged;
            _records.Clear();
            _changed.Clear();
            _hashes.Dispose();
        }


        public async ValueTask<bool> MoveNextAsync()
        {
            if ( _changed.Contains(_hashes.Current) ) { AddOrUpdate(await _table.Get(_hashes.Current, CancellationToken.None)); }

            return _hashes.MoveNext();
        }


        protected async Task SaveAllToDB( CancellationToken token = default ) => await _table.Update(_records.Values, token);
        protected async Task SaveChangedToDB( CancellationToken token = default )
        {
            if ( !_changed.Any() ) { return; }

            IEnumerable<TRecord> records = _records.Where(x => _changed.Contains(x.Key))
                                                   .Select(x => x.Value);

            await _table.Update(records, token);
            _changed.Clear();
        }


        protected async Task RefreshFromDB( CancellationToken token = default )
        {
            List<TRecord> records = await _table.All(token);

            _records.Clear();
            foreach ( TRecord record in records ) { AddOrUpdate(record); }
        }
        protected void AddOrUpdate( TRecord record )
        {
            if ( _records.ContainsKey(record.ID) ) { _records[record.ID] = record; }
            else { _records.Add(record.ID, record); }

            _hashes.Add(record);
        }
        protected void Changed( IEnumerable<TID> ids )
        {
            foreach ( TID id in ids ) { _changed.Add(id); }
        }


        public void Add( TRecord      item ) { }
        public bool Remove( TRecord   item ) => false;
        public bool Contains( TRecord item ) => _records.ContainsValue(item);


        IEnumerator<TRecord> IEnumerable<TRecord>.GetEnumerator() => _records.Values.GetEnumerator();
        public IEnumerator GetEnumerator() => _records.GetEnumerator();
    }



    public sealed class Mapping : ObservableConcurrentDictionary<TID, TRecord> { }



    public class SortedList : SortedList<TID, int>, IEnumerator<TID>
    {
        private int        _index;
        public  TID        Current => Keys[_index];
        object IEnumerator.Current => Current;


        public SortedList() : base(Comparer<TID>.Default) { }


        public bool MoveNext() => ++_index < Count;
        public void Reset() => _index = 0;
        public void Dispose() => Clear();


        public void Add( IEnumerable<TRecord> records )
        {
            foreach ( TRecord record in records ) { Add(record); }
        }
        public void Add( TRecord record ) => this[record.ID] = record.GetHashCode();
    }
}
