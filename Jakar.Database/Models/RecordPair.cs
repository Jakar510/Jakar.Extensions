// Jakar.Extensions :: Jakar.Database
// 03/11/2023  11:20 PM

namespace Jakar.Database;


[Serializable]
[method: JsonConstructor]
public readonly struct RecordPair<TClass>( RecordID<TClass> id, DateTimeOffset dateCreated ) : IDateCreated, IEqualityOperators<RecordPair<TClass>>, IComparisonOperators<RecordPair<TClass>>
    where TClass : ITableRecord<TClass>
{
    public readonly  RecordID<TClass> ID          = id;
    public readonly  DateTimeOffset   DateCreated = dateCreated;
    private readonly int              _hash       = HashCode.Combine(id, dateCreated);


    public static string       TableName   { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => TClass.TableName; }
    Guid IUniqueID<Guid>.      ID          => ID.Value;
    DateTimeOffset IDateCreated.DateCreated => DateCreated;


    public int CompareTo( object? other ) => other is RecordPair<TClass> pair
                                                 ? CompareTo(pair)
                                                 : throw new ExpectedValueTypeException(other, typeof(RecordPair<TClass>));
    public          int  CompareTo( RecordPair<TClass> other ) => DateCreated.CompareTo(other.DateCreated);
    public          bool Equals( RecordPair<TClass>    other ) => ID.Equals(other.ID)              && DateCreated.Equals(other.DateCreated);
    public override bool Equals( object?               other ) => other is RecordPair<TClass> pair && Equals(pair);
    public override int  GetHashCode()                         => _hash;


    public static implicit operator RecordPair<TClass>( (RecordID<TClass> id, DateTimeOffset dateCreated) value ) => new(value.id, value.dateCreated);
    public static implicit operator (RecordID<TClass> id, DateTimeOffset dateCreated)( RecordPair<TClass> value ) => ( value.ID, value.DateCreated );
    public static implicit operator KeyValuePair<RecordID<TClass>, DateTimeOffset>( RecordPair<TClass>    value ) => new(value.ID, value.DateCreated);
    public static implicit operator KeyValuePair<Guid, DateTimeOffset>( RecordPair<TClass>                value ) => new(value.ID.Value, value.DateCreated);


    [Pure]
    public static RecordPair<TClass> Create( DbDataReader reader )
    {
        DateTimeOffset   dateCreated = reader.GetFieldValue<DateTimeOffset>(nameof(DateCreated));
        RecordID<TClass> id          = RecordID<TClass>.ID(reader);
        return new RecordPair<TClass>(id, dateCreated);
    }
    [Pure]
    public static async IAsyncEnumerable<RecordPair<TClass>> CreateAsync( DbDataReader reader, [EnumeratorCancellation] CancellationToken token = default )
    {
        while ( await reader.ReadAsync(token) ) { yield return Create(reader); }
    }


    [Pure]
    public DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = new();
        parameters.Add(nameof(ID),          ID);
        parameters.Add(nameof(DateCreated), DateCreated);
        return parameters;
    }


    public static bool operator ==( RecordPair<TClass>? left, RecordPair<TClass>? right ) => Nullable.Equals(left, right);
    public static bool operator !=( RecordPair<TClass>? left, RecordPair<TClass>? right ) => !Nullable.Equals(left, right);
    public static bool operator ==( RecordPair<TClass>  left, RecordPair<TClass>  right ) => EqualityComparer<RecordPair<TClass>>.Default.Equals(left, right);
    public static bool operator !=( RecordPair<TClass>  left, RecordPair<TClass>  right ) => !EqualityComparer<RecordPair<TClass>>.Default.Equals(left, right);
    public static bool operator >( RecordPair<TClass>   left, RecordPair<TClass>  right ) => Comparer<RecordPair<TClass>>.Default.Compare(left, right) > 0;
    public static bool operator >=( RecordPair<TClass>  left, RecordPair<TClass>  right ) => Comparer<RecordPair<TClass>>.Default.Compare(left, right) >= 0;
    public static bool operator <( RecordPair<TClass>   left, RecordPair<TClass>  right ) => Comparer<RecordPair<TClass>>.Default.Compare(left, right) < 0;
    public static bool operator <=( RecordPair<TClass>  left, RecordPair<TClass>  right ) => Comparer<RecordPair<TClass>>.Default.Compare(left, right) <= 0;
}
