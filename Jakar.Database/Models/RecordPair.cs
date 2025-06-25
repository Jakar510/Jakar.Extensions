// Jakar.Extensions :: Jakar.Database
// 03/11/2023  11:20 PM

namespace Jakar.Database;


// ReSharper disable once InconsistentNaming
public readonly struct RecordPair<TClass>( RecordID<TClass> id, DateTimeOffset dateCreated ) : IRecordPair, IEqualComparableValue<RecordPair<TClass>>
    where TClass : class, ITableRecord<TClass>, IDbReaderMapping<TClass>
{
    public readonly  RecordID<TClass> id          = id;
    public readonly  DateTimeOffset   dateCreated = dateCreated;
    private readonly int              _hash       = HashCode.Combine( id, dateCreated );


    public static ValueEqualizer<RecordPair<TClass>> Equalizer { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ValueEqualizer<RecordPair<TClass>>.Default; }
    public static ValueSorter<RecordPair<TClass>>    Sorter    { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ValueSorter<RecordPair<TClass>>.Default; }


    public static string    TableName   { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => TClass.TableName; }
    Guid IUniqueID<Guid>.   ID          => id.value;
    public RecordID<TClass> ID          { get => id; }
    public DateTimeOffset   DateCreated { get => dateCreated; }


    public int CompareTo( RecordPair<TClass> other )
    {
        int idComparison = id.CompareTo( other.id );
        if ( idComparison != 0 ) { return idComparison; }

        return dateCreated.CompareTo( other.dateCreated );
    }
    public          bool Equals( RecordPair<TClass>     other ) => id.Equals( other.id ) && dateCreated.Equals( other.dateCreated );
    public          int  CompareTo( RecordPair<TClass>? other ) => 0;
    public          bool Equals( RecordPair<TClass>?    other ) => false;
    public override bool Equals( object?                obj )   => obj is RecordPair<TClass> other && Equals( other );
    public override int  GetHashCode()                          => _hash;
    public          int  CompareTo( object? obj )               => 0;


    public static implicit operator RecordPair<TClass>( (RecordID<TClass> id, DateTimeOffset dateCreated) value ) => new(value.id, value.dateCreated);
    public static implicit operator (RecordID<TClass> id, DateTimeOffset dateCreated)( RecordPair<TClass> value ) => (value.id, value.dateCreated);
    public static implicit operator KeyValuePair<RecordID<TClass>, DateTimeOffset>( RecordPair<TClass>    value ) => new(value.id, value.dateCreated);
    public static implicit operator KeyValuePair<Guid, DateTimeOffset>( RecordPair<TClass>                value ) => new(value.id.value, value.dateCreated);


    [Pure]
    public static RecordPair<TClass> Create( DbDataReader reader )
    {
        DateTimeOffset   dateCreated = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        RecordID<TClass> id          = RecordID<TClass>.ID( reader );
        return new RecordPair<TClass>( id, dateCreated );
    }
    [Pure]
    public static async IAsyncEnumerable<RecordPair<TClass>> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
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


    public static bool operator >( RecordPair<TClass>  left, RecordPair<TClass> right ) => Sorter.GreaterThan( left, right );
    public static bool operator >=( RecordPair<TClass> left, RecordPair<TClass> right ) => Sorter.GreaterThanOrEqualTo( left, right );
    public static bool operator <( RecordPair<TClass>  left, RecordPair<TClass> right ) => Sorter.LessThan( left, right );
    public static bool operator <=( RecordPair<TClass> left, RecordPair<TClass> right ) => Sorter.LessThanOrEqualTo( left, right );
    public static bool operator ==( RecordPair<TClass> left, RecordPair<TClass> right ) =>  Sorter.Equals( left, right );
    public static bool operator !=( RecordPair<TClass> left, RecordPair<TClass> right ) =>  Sorter.DoesNotEqual( left, right );
}
