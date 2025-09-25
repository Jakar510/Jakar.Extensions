namespace Jakar.Database.Resx;


public sealed class ResxSet : ConcurrentDictionary<long, string>
{
    public ResxSet() { }
    public ResxSet( IEnumerable<KeyValuePair<long, string>> collection ) : base(collection) { }
    public ResxSet( int                                     capacity ) : base(Environment.ProcessorCount, capacity) { }
    public ResxSet( params ReadOnlySpan<KeyValuePair<long, string>> collection ) : this(collection.Length)
    {
        foreach ( ( long key, string value ) in collection ) { this[key] = value; }
    }
    public ResxSet( int concurrencyLevel, int capacity ) : base(concurrencyLevel, capacity) { }
}
