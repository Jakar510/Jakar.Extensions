namespace Jakar.Database.Resx;


public sealed class ResxSet : ConcurrentDictionary<long, string>
{
    public ResxSet() { }
    public ResxSet( IEnumerable<KeyValuePair<long, string>> collection ) : this( collection, ValueEqualizer<long>.Default ) { }
    public ResxSet( IEnumerable<KeyValuePair<long, string>> collection, IEqualityComparer<long> comparer ) : base( collection, comparer ) { }
    public ResxSet( IEqualityComparer<long>                 comparer ) : base( comparer ) { }
    public ResxSet( int                                     concurrencyLevel, IEnumerable<KeyValuePair<long, string>> collection, IEqualityComparer<long> comparer ) : base( concurrencyLevel, collection, comparer ) { }
    public ResxSet( int                                     capacity ) : base( Environment.ProcessorCount, capacity, ValueEqualizer<long>.Default ) { }
    public ResxSet( int                                     concurrencyLevel, int capacity ) : base( concurrencyLevel, capacity ) { }
    public ResxSet( int                                     concurrencyLevel, int capacity, IEqualityComparer<long> comparer ) : base( concurrencyLevel, capacity, comparer ) { }
}
