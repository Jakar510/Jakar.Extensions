namespace Jakar.Database.Resx;


public sealed class ResxSet : ConcurrentDictionary<string, string>
{
    public ResxSet() { }
    public ResxSet( IEnumerable<KeyValuePair<string, string>> collection ) : this( collection, StringComparer.Ordinal ) { }
    public ResxSet( IEnumerable<KeyValuePair<string, string>> collection, IEqualityComparer<string> comparer ) : base( collection, comparer ) { }
    public ResxSet( IEqualityComparer<string>                 comparer ) : base( comparer ) { }
    public ResxSet( int                                       concurrencyLevel, IEnumerable<KeyValuePair<string, string>> collection, IEqualityComparer<string> comparer ) : base( concurrencyLevel, collection, comparer ) { }
    public ResxSet( int                                       capacity ) : base( Environment.ProcessorCount, capacity, StringComparer.Ordinal ) { }
    public ResxSet( int                                       concurrencyLevel, int capacity ) : base( concurrencyLevel, capacity ) { }
    public ResxSet( int                                       concurrencyLevel, int capacity, IEqualityComparer<string> comparer ) : base( concurrencyLevel, capacity, comparer ) { }
}
