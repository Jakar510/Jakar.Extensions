// Jakar.Extensions :: Jakar.Database
// 01/01/2025  16:01

using NoAlloq;



namespace Jakar.Database;


[SuppressMessage( "ReSharper", "StaticMemberInGenericType" )]
public sealed class SqlCache<TRecord>
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    private const          string                               SPACER        = ",      \n";
    public static readonly string                               CreatedBy     = nameof(IOwnedTableRecord.CreatedBy).ToSnakeCase();
    public static readonly string                               DateCreated   = nameof(IRecordPair.DateCreated).ToSnakeCase();
    public static readonly string                               ID            = nameof(IRecordPair.ID).ToSnakeCase();
    public static readonly string                               Ids           = "ids";
    public static readonly string                               LastModified  = nameof(ITableRecord.LastModified).ToSnakeCase();
    public static readonly FrozenDictionary<string, Descriptor> SqlProperties = Descriptor.CreateMapping<TRecord>();
    private readonly       string                               _deleteIDs    = $"DELETE FROM {TRecord.TableName} WHERE {ID} in ({{0}});";
    private readonly       string                               _getID        = $"SELECT * FROM {TRecord.TableName} WHERE {ID} = @{ID};";
    private readonly       string                               _getIDs       = $"SELECT * FROM {TRecord.TableName} WHERE {ID} in ({{0}});";
    private readonly       string                               _where        = $"SELECT * FROM {TRecord.TableName} WHERE {{0}};";
    private readonly       string                               _exists       = $"EXISTS( SELECT * FROM {TRecord.TableName} WHERE {{0}} );";
    public readonly        string                               count         = @$"SELECT COUNT(*) FROM {TRecord.TableName};";
    private readonly string _tryInsert = $$"""
                                           IF NOT EXISTS(SELECT * FROM {TRecord.TableName} WHERE {0})
                                           BEGIN
                                               INSERT INTO {{TRecord.TableName}}
                                               (
                                               {{string.Join( SPACER, SqlProperties.Values.Select( Descriptor.GetColumnName ) )}}
                                               ) 
                                               values 
                                               (
                                                  {{string.Join( SPACER, SqlProperties.Values.Select( Descriptor.GetVariableName ) )}}
                                               ) 
                                               RETURNING {{ID}};
                                           END

                                           ELSE
                                           BEGIN
                                               SELECT {{ID}} = NULL
                                           END
                                           """;
    private readonly string _updateOrInsert = $$"""
                                                IF NOT EXISTS(SELECT * FROM {TRecord.TableName} WHERE {0})
                                                BEGIN
                                                    INSERT INTO {{TRecord.TableName}}
                                                    (
                                                      {{string.Join( SPACER, SqlProperties.Values.Select( Descriptor.GetColumnName ) )}}
                                                    ) 
                                                    values 
                                                    (
                                                      {{string.Join( SPACER, SqlProperties.Values.Select( Descriptor.GetVariableName ) )}}
                                                    ) 
                                                    RETURNING {{ID}};
                                                END

                                                ELSE
                                                BEGIN
                                                    UPDATE {{TRecord.TableName}} SET {{KeyValuePairs}} WHERE {{ID}} = @{{ID}};
                                                    SELECT @{{ID}};
                                                END
                                                """;
    public readonly  string all       = $"SELECT * FROM {TRecord.TableName};";
    public readonly  string deleteAll = $"DELETE FROM {TRecord.TableName}";
    private readonly string deleteID  = $"DELETE FROM  {TRecord.TableName} WHERE {ID} = @{ID};";
    public readonly  string first     = @$"SELECT * FROM {TRecord.TableName} ORDER BY {DateCreated} ASC LIMIT 1";
    private readonly string insert = $"""
                                          INSERT INTO {TRecord.TableName} 
                                          (
                                          {string.Join( SPACER, SqlProperties.Values.Select( Descriptor.GetColumnName ) )}
                                          ) 
                                          values 
                                          (
                                             {string.Join( SPACER, SqlProperties.Values.Select( Descriptor.GetVariableName ) )}
                                          ) 
                                          RETURNING {ID};
                                      """;
    public readonly  string last      = @$"SELECT * FROM {TRecord.TableName} ORDER BY {DateCreated} DESC LIMIT 1";
    private readonly string next      = @$"SELECT * FROM {TRecord.TableName} WHERE ( id = IFNULL((SELECT MIN({ID}) FROM {TRecord.TableName} WHERE {DateCreated} > @{DateCreated}), 0) )";
    private readonly string nextID    = @$"SELECT {ID} FROM {TRecord.TableName} WHERE ( id = IFNULL((SELECT MIN({ID}) FROM {TRecord.TableName} WHERE {DateCreated} > @{DateCreated}), 0) )";
    public readonly  string sortedIDs = @$"SELECT {ID}, {DateCreated} FROM {TRecord.TableName} ORDER BY {DateCreated} DESC";
    private readonly string updateID  = $"UPDATE {TRecord.TableName} SET {string.Join( ',', KeyValuePairs )} WHERE {ID} = @{ID};";
    public readonly  string whereID   = $"SELECT * FROM {TRecord.TableName} WHERE {ID} = @{ID}";


    public static IEnumerable<string> KeyValuePairs => SqlProperties.Values.Select( Descriptor.GetKeyValuePair );

    public SqlCommand Get( RecordID<TRecord>              ids )                                    => new(_getID, ids.ToDynamicParameters());
    public string     Get( IEnumerable<RecordID<TRecord>> ids )                                    => string.Format( _getIDs, string.Join( ',', ids.Select( GetValue ) ) );
    public SqlCommand Get( bool                           matchAll, DynamicParameters parameters ) => new(string.Format( _where, string.Join( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) ) ), parameters);


    public SqlCommand Exists( bool matchAll, DynamicParameters parameters ) => new(string.Format( _exists, string.Join( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) ) ), parameters);


    public SqlCommand Delete( bool matchAll, DynamicParameters parameters ) =>
        new(string.Format( _deleteIDs, string.Join( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) ) ), parameters);
    public SqlCommand DeleteID( RecordID<TRecord>            ids ) => new(deleteID, ids.ToDynamicParameters());
    public string     Delete( IEnumerable<RecordID<TRecord>> ids ) => string.Format( _deleteIDs, string.Join( ',', ids.Select( GetValue ) ) );


    public SqlCommand Next( RecordPair<TRecord>   pair ) => new(next, pair.ToDynamicParameters());
    public SqlCommand NextID( RecordPair<TRecord> pair ) => new(nextID, pair.ToDynamicParameters());


    public SqlCommand Insert( TRecord record ) => new(insert, record.ToDynamicParameters());
    public SqlCommand Update( TRecord record ) => new(updateID, record.ToDynamicParameters());
    public SqlCommand TryInsert( TRecord record, bool matchAll, DynamicParameters parameters )
    {
        var param = record.ToDynamicParameters();
        param.AddDynamicParams( parameters );

        return new SqlCommand( string.Format( _tryInsert, string.Join( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) ) ), param );
    }
    public SqlCommand InsertOrUpdate( TRecord record, bool matchAll, DynamicParameters parameters )
    {
        var param = record.ToDynamicParameters();
        param.AddDynamicParams( parameters );

        return new SqlCommand( string.Format( _updateOrInsert, string.Join( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) ), param ) );
    }


    public static  IEnumerable<string> GetKeyValuePairs( DynamicParameters parameters ) => parameters.ParameterNames.Select( GetKeyValuePair );
    public static  string              GetKeyValuePair( string             columnName ) => SqlProperties[columnName].KeyValuePair;
    private static Guid                GetValue( RecordID<TRecord>         x )          => x.value;
}
