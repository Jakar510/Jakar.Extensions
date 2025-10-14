namespace Jakar.Database;


[Serializable]
public class RecordCollection<TSelf>( int capacity = DEFAULT_CAPACITY ) : IReadOnlyList<TSelf>
    where TSelf : ITableRecord<TSelf>
{
    private readonly List<TSelf> __records = new(capacity);


    public int Count => __records.Count;
    public TSelf this[ int index ] => __records[index];


    public RecordCollection( params ReadOnlySpan<TSelf> values ) : this() => Add(values);
    public RecordCollection( IEnumerable<TSelf>         values ) : this() => Add(values);


    public RecordCollection<TSelf> Add( params ReadOnlySpan<TSelf> values )
    {
        foreach ( TSelf value in values ) { Add(value); }

        return this;
    }
    public RecordCollection<TSelf> Add( IEnumerable<TSelf> values )
    {
        foreach ( TSelf value in values ) { Add(value); }

        return this;
    }
    public RecordCollection<TSelf> Add( TSelf value )
    {
        if ( value.IsValidID() )
        {
            __records.Add(value);
            return this;
        }


        __records.Add(value.NewID(RecordID<TSelf>.New()));

        return this;
    }


    public IEnumerator<TSelf> GetEnumerator() => __records.GetEnumerator();
    IEnumerator IEnumerable.   GetEnumerator() => GetEnumerator();
}
