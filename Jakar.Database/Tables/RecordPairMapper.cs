// Jakar.Extensions :: Jakar.Database
// 03/11/2023  11:20 PM

namespace Jakar.Database;


public readonly record struct RecordPair( Guid ID, DateTimeOffset DateCreated ) : IComparable<RecordPair>, IRecordPair
{
    public int CompareTo( RecordPair other ) => DateCreated.CompareTo( other.DateCreated );


    public static implicit operator RecordPair( (Guid ID, DateTimeOffset DateCreated) value ) => new(value.ID, value.DateCreated);
    public static implicit operator KeyValuePair<Guid, DateTimeOffset>( RecordPair    value ) => new(value.ID, value.DateCreated);
}



// public sealed class RecordPairMapper : SqlMapper.ITypeHandler
// {
//     static RecordPairMapper() => SqlMapper.AddTypeHandler( typeof(RecordPair), new RecordPairMapper() );
//
//
//     public void SetValue( IDbDataParameter parameter,       object value ) { }
//     public object Parse( Type              destinationType, object value ) => null;
// }
