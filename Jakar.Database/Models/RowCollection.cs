namespace Jakar.Database;


[Serializable]
public class RecordCollection<TClass>( int capacity = Buffers.DEFAULT_CAPACITY ) : IReadOnlyList<TClass>
    where TClass : TableRecord<TClass>, IDbReaderMapping<TClass>
{
    private readonly List<TClass> __records = new(capacity);


    public int Count => __records.Count;
    public TClass this[ int index ] => __records[index];


    public RecordCollection( params ReadOnlySpan<TClass> values ) : this() => Add( values );
    public RecordCollection( IEnumerable<TClass>         values ) : this() => Add( values );


    public RecordCollection<TClass> Add( params ReadOnlySpan<TClass> values )
    {
        foreach ( TClass value in values ) { Add( value ); }

        return this;
    }
    public RecordCollection<TClass> Add( IEnumerable<TClass> values )
    {
        foreach ( TClass value in values ) { Add( value ); }

        return this;
    }
    public RecordCollection<TClass> Add( TClass value )
    {
        if ( value.IsValidID() )
        {
            __records.Add( value );
            return this;
        }


        __records.Add( value.NewID( RecordID<TClass>.New() ) );

        return this;
    }


    public IEnumerator<TClass> GetEnumerator() => __records.GetEnumerator();
    IEnumerator IEnumerable.    GetEnumerator() => GetEnumerator();
}
