namespace Jakar.AppLogger.Portal.Data.Tables;


[ Serializable, Table( "Apps" ) ]
public sealed record AppRecord : OwnedLoggerTable<AppRecord>, IDbReaderMapping<AppRecord>, MsJsonModels.IJsonizer<AppRecord>
{
    public static string TableName { get; } = typeof(AppRecord).GetTableName();


    public string AppName { get; init; } = string.Empty;
    public string Secret  { get; init; } = string.Empty;


    public AppRecord( string appName, string secret, UserRecord caller ) : base( caller )
    {
        AppName = appName;
        Secret  = secret;
    }


    [ Pure ]
    public static DynamicParameters GetDynamicParameters( string secret )
    {
        var parameters = new DynamicParameters();
        parameters.Add( nameof(Secret), secret );
        return parameters;
    }
    [ Pure ] public static AppRecord Create( DbDataReader reader ) => null;
    [ Pure ]
    public static async IAsyncEnumerable<AppRecord> CreateAsync( DbDataReader reader, [ EnumeratorCancellation ] CancellationToken token = default )
    {
        while ( await reader.ReadAsync( token ) ) { yield return Create( reader ); }
    }


    public override int CompareTo( AppRecord? other ) => string.CompareOrdinal( AppName, other?.AppName );
    public override int GetHashCode()                 => HashCode.Combine( AppName, base.GetHashCode() );


#if NET6_0_OR_GREATER

    [ Pure ] public static AppRecord FromJson( string json ) => json.FromJson( JsonTypeInfo() );
    [ Pure ]
    public static JsonSerializerOptions JsonOptions( bool formatted ) => new()
                                                                         {
                                                                             WriteIndented    = formatted,
                                                                             TypeInfoResolver = AppRecordContext.Default
                                                                         };
    [ Pure ] public static JsonTypeInfo<AppRecord> JsonTypeInfo() => AppRecordContext.Default.AppRecord;
#endif
}



#if NET6_0_OR_GREATER



[ JsonSerializable( typeof(AppRecord) ) ] public partial class AppRecordContext : JsonSerializerContext;
#endif
