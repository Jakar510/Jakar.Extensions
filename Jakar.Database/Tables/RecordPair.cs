// Jakar.Extensions :: Jakar.Database
// 03/11/2023  11:20 PM

namespace Jakar.Database;


public readonly record struct RecordPair( Guid ID, DateTimeOffset DateCreated ) : IComparable<RecordPair>, IRecordPair
{
    public int CompareTo( RecordPair other ) => DateCreated.CompareTo( other.DateCreated );


    public static implicit operator RecordPair( (Guid ID, DateTimeOffset DateCreated) value ) => new(value.ID, value.DateCreated);
    public static implicit operator KeyValuePair<Guid, DateTimeOffset>( RecordPair    value ) => new(value.ID, value.DateCreated);
}



public readonly record struct RecordPair<TRecord>( RecordID<TRecord> ID, DateTimeOffset DateCreated ) : IComparable<RecordPair<TRecord>>, IRecordPair where TRecord : TableRecord<TRecord>
{
    Guid IUniqueID<Guid>.ID => ID.Value;

    public int CompareTo( RecordPair<TRecord> other ) => DateCreated.CompareTo( other.DateCreated );


    public static implicit operator RecordPair<TRecord>( (RecordID<TRecord> ID, DateTimeOffset DateCreated) value ) => new(value.ID, value.DateCreated);
    public static implicit operator KeyValuePair<Guid, DateTimeOffset>( RecordPair<TRecord>                 value ) => new(value.ID.Value, value.DateCreated);
}
