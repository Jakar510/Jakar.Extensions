// Jakar.Extensions :: Jakar.Database
// 03/11/2023  11:20 PM

namespace Jakar.Database;


// ReSharper disable once InconsistentNaming
public readonly record struct RecordPair<TRecord>( RecordID<TRecord> ID, DateTimeOffset DateCreated ) : IComparable<RecordPair<TRecord>>, IRecordPair, IDbReaderMapping<RecordPair<TRecord>>, MsJsonModels.IJsonizer<RecordPair<TRecord>>
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public static string                              TableName => TRecord.TableName;
    public static ValueEqualizer<RecordPair<TRecord>> Equalizer => ValueEqualizer<RecordPair<TRecord>>.Default;
    public static ValueSorter<RecordPair<TRecord>>    Sorter    => ValueSorter<RecordPair<TRecord>>.Default;
    Guid IUniqueID<Guid>.                             ID        => ID.Value;


    public                          int CompareTo( RecordPair<TRecord>                                      other ) => DateCreated.CompareTo( other.DateCreated );
    public static implicit operator RecordPair<TRecord>( (RecordID<TRecord> ID, DateTimeOffset DateCreated) value ) => new(value.ID, value.DateCreated);
    public static implicit operator KeyValuePair<Guid, DateTimeOffset>( RecordPair<TRecord>                 value ) => new(value.ID.Value, value.DateCreated);


    [ Pure ]
    public static RecordPair<TRecord> Create( DbDataReader reader )
    {
        var               dateCreated = reader.GetFieldValue<DateTimeOffset>( nameof(DateCreated) );
        RecordID<TRecord> id          = RecordID<TRecord>.ID( reader );
        return new RecordPair<TRecord>( id, dateCreated );
    }
    [ Pure ]
    public static async IAsyncEnumerable<RecordPair<TRecord>> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    [ Pure ]
    public DynamicParameters ToDynamicParameters()
    {
        DynamicParameters parameters = new();
        parameters.Add( nameof(ID),          ID );
        parameters.Add( nameof(DateCreated), DateCreated );
        return parameters;
    }


    [ Pure ] public static RecordPair<TRecord> FromJson( string json ) => json.FromJson( JsonTypeInfo() );
    [ Pure ]
    public static JsonSerializerOptions JsonOptions( bool formatted ) => new()
                                                                         {
                                                                             WriteIndented    = formatted,
                                                                             TypeInfoResolver = JsonContext.Default
                                                                         };
    [ Pure ] public static JsonTypeInfo<RecordPair<TRecord>> JsonTypeInfo() => JsonContext.Default.RecordPair;



    public sealed class JsonContext( JsonSerializerOptions? options ) : JsonSerializerContext( options )
    {
        public static      JsonContext                       Default                    { get; } = new(JsonSerializerOptions.Default);
        protected override JsonSerializerOptions?            GeneratedSerializerOptions => JsonOptions( false );
        public             JsonTypeInfo<RecordPair<TRecord>> RecordPair                 { get; } = MsJsonTypeInfo.CreateJsonTypeInfo<RecordPair<TRecord>>( JsonSerializerOptions.Default );
        public override    MsJsonTypeInfo                    GetTypeInfo( Type type )   => RecordPair;
    }
}
