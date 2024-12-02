// Jakar.Extensions :: Jakar.Database
// 03/11/2023  11:20 PM

namespace Jakar.Database;


// ReSharper disable once InconsistentNaming
public readonly struct RecordPair<TRecord>( RecordID<TRecord> id, DateTimeOffset dateCreated ) : IEquatable<RecordPair<TRecord>>, IComparable<RecordPair<TRecord>>, IRecordPair, IDbReaderMapping<RecordPair<TRecord>>
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public readonly  RecordID<TRecord> id          = id;
    public readonly  DateTimeOffset    dateCreated = dateCreated;
    private readonly int               _hash       = HashCode.Combine( id, dateCreated );


    public static ValueEqualizer<RecordPair<TRecord>> Equalizer   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ValueEqualizer<RecordPair<TRecord>>.Default; }
    public static ValueSorter<RecordPair<TRecord>>    Sorter      { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ValueSorter<RecordPair<TRecord>>.Default; }
    public static string                              TableName   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TRecord.TableName; }
    Guid IUniqueID<Guid>.                             ID          => id.value;
    public RecordID<TRecord>                          ID          { get => id; }
    public DateTimeOffset                             DateCreated { get => dateCreated; }


    public int CompareTo( RecordPair<TRecord> other )
    {
        int idComparison = id.CompareTo( other.id );
        if ( idComparison != 0 ) { return idComparison; }

        return dateCreated.CompareTo( other.dateCreated );
    }
    public          bool Equals( RecordPair<TRecord> other ) => id.Equals( other.id )            && dateCreated.Equals( other.dateCreated );
    public override bool Equals( object?             obj )   => obj is RecordPair<TRecord> other && Equals( other );
    public override int  GetHashCode()                       => _hash;


    public static implicit operator RecordPair<TRecord>( (RecordID<TRecord> id, DateTimeOffset dateCreated) value ) => new(value.id, value.dateCreated);
    public static implicit operator (RecordID<TRecord> id, DateTimeOffset dateCreated)( RecordPair<TRecord> value ) => (value.id, value.dateCreated);
    public static implicit operator KeyValuePair<RecordID<TRecord>, DateTimeOffset>( RecordPair<TRecord>    value ) => new(value.id, value.dateCreated);
    public static implicit operator KeyValuePair<Guid, DateTimeOffset>( RecordPair<TRecord>                 value ) => new(value.id.value, value.dateCreated);


    [Pure]
    public static RecordPair<TRecord> Create( DbDataReader reader )
    {
        DateTimeOffset    dateCreated = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        RecordID<TRecord> id          = RecordID<TRecord>.ID( reader );
        return new RecordPair<TRecord>( id, dateCreated );
    }
    [Pure]
    public static async IAsyncEnumerable<RecordPair<TRecord>> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    [Pure]
    public DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(ID),          id );
        parameters.Add( nameof(DateCreated), dateCreated );
        return parameters;
    }
}
