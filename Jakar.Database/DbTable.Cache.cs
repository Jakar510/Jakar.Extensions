using System.Collections;
using Microsoft.Extensions.Logging;



namespace Jakar.Database;


public abstract partial class DbTable<TRecord, TID>
{
    public sealed class Cache : IAsyncEnumerator<TRecord>, IReadOnlyCollection<TRecord>
    {
        private readonly DbTable<TRecord, TID> _table;
        private readonly ILogger<Cache>        _logger;
        private readonly Mapping<TRecord, TID> _records = new();
        private readonly TimeSpan              _refreshTime;


        // private readonly SortedList _keys   = new();


        public TRecord Current    => _records.Current;
        public int     Count      => _records.Count;
        public bool    IsReadOnly => false;


        public TRecord this[ TID index ]
        {
            get => _records[index];
            set => _records[index] = value;
        }


        public Cache( DbTable<TRecord, TID> table, ILogger<Cache> logger ) : this(table, logger, TimeSpan.FromSeconds(15)) { }
        public Cache( DbTable<TRecord, TID> table, ILogger<Cache> logger, TimeSpan refreshTime )
        {
            _table  = table;
            _logger = logger;

            _refreshTime = refreshTime;
        }
        public async ValueTask DisposeAsync()
        {
            await SaveAllToDB();
            _records.Dispose();
        }


        public async ValueTask<bool> MoveNextAsync()
        {
            await Refresh();


            return _records.MoveNext();
        }


        private async Task SaveAllToDB( CancellationToken token = default ) => await _table.Update(_records.Values, token);


        private async Task Refresh( CancellationToken token = default )
        {
            using var timer = new PeriodicTimer(_refreshTime);

            while ( !token.IsCancellationRequested )
            {
                try
                {
                    await timer.WaitForNextTickAsync(token);
                    await _table.Call(Refresh, token);
                }
                catch ( Exception e ) { _logger.LogCritical(e, nameof(Refresh)); }
            }
        }
        private async Task Refresh( DbConnection connection, DbTransaction transaction, CancellationToken token = default )
        {
            await _records.Refresh(connection, transaction, _table, token);

            List<TRecord> records = await _table.All(connection, transaction, token);
            _records.Clear();
            foreach ( TRecord record in records ) { _records.AddOrUpdate(record); }
        }


        public void Add( TRecord      item ) { }
        public bool Remove( TRecord   item ) => false;
        public bool Contains( TRecord item ) => _records.ContainsValue(item);


        IEnumerator<TRecord> IEnumerable<TRecord>.GetEnumerator() => _records.Values.GetEnumerator();
        public IEnumerator GetEnumerator() => _records.GetEnumerator();
    }
}
