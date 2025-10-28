namespace Jakar.Database.Resx;


public sealed class ResxSet : ConcurrentDictionary<long, string>
{
    public ResxSet() : base() { }
    public ResxSet( IEnumerable<KeyValuePair<long, string>> collection ) : base(collection) { }
    public ResxSet( int                                     concurrencyLevel, int capacity ) : base(concurrencyLevel, capacity) { }
    public ResxSet( params ReadOnlySpan<KeyValuePair<long, string>> collection ) : base()
    {
        foreach ( ( long key, string value ) in collection ) { this[key] = value; }
    }
}
