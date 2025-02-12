// Jakar.Extensions :: Jakar.Database
// 01/01/2025  16:01

namespace Jakar.Database;


[SuppressMessage( "ReSharper", "StaticMemberInGenericType" )]
public sealed class SqlCache<TRecord>
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public const           string                               SPACER        = ",      \n";
    public static readonly string                               CreatedBy     = nameof(IOwnedTableRecord.CreatedBy).ToSnakeCase();
    public static readonly string                               DateCreated   = nameof(IRecordPair.DateCreated).ToSnakeCase();
    public static readonly string                               ID            = nameof(IRecordPair.ID).ToSnakeCase();
    public static readonly string                               Ids           = "ids";
    public static readonly string                               LastModified  = nameof(ITableRecord.LastModified).ToSnakeCase();
    public static readonly FrozenDictionary<string, Descriptor> SqlProperties = Descriptor.CreateMapping<TRecord>();


    public readonly string _all       = $"SELECT * FROM {TRecord.TableName};";
    public readonly string _count     = $"SELECT COUNT(*) FROM {TRecord.TableName};";
    public readonly string _deleteAll = $"DELETE FROM {TRecord.TableName};";
    public readonly string _deleteID = $$"""
                                         DELETE FROM {{TRecord.TableName}}
                                         WHERE {{ID}} = '{0}';
                                         """;
    public readonly string _deleteIDs = $$"""
                                          DELETE FROM {{TRecord.TableName}} 
                                          WHERE {{ID}} in ({0});
                                          """;
    public readonly string _exists = $$"""
                                       EXISTS( 
                                       SELECT * FROM {{TRecord.TableName}} 
                                       WHERE {0} 
                                       );
                                       """;
    public readonly string _first = $"""
                                     SELECT * FROM {TRecord.TableName} 
                                     ORDER BY {DateCreated} ASC 
                                     LIMIT 1;
                                     """;
    public readonly string _getWhereIDs = $$"""
                                            SELECT * FROM {{TRecord.TableName}} 
                                            WHERE {{ID}} in ({0});
                                            """;
    public readonly string _getPaged = $$"""
                                         SELECT * FROM {{TRecord.TableName}} 
                                         OFFSET {0}
                                         LIMIT {1};
                                         """;
    public readonly string _getPagedWhere = $$"""
                                              SELECT * FROM {{TRecord.TableName}} 
                                              {0} 
                                              OFFSET {1}
                                              LIMIT {2};
                                              """;
    public readonly string _getPagedWhereCreatedBy = $$"""
                                                       SELECT * FROM {{TRecord.TableName}} 
                                                       WHERE {{CreatedBy}} = '{0}'
                                                       OFFSET {1}
                                                       LIMIT {2};
                                                       """;
    public readonly string _getWhereID = $$"""
                                           SELECT * FROM {{TRecord.TableName}} 
                                           WHERE {{ID}} = '{0}';
                                           """;
    public readonly string _insert = $"""
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
    public readonly string _last = $"""
                                    SELECT * FROM {TRecord.TableName} 
                                    ORDER BY {DateCreated} DESC 
                                    LIMIT 1
                                    """;
    public readonly string _nextWhereDateCreated = $$"""
                                                     SELECT * FROM {{TRecord.TableName}}
                                                     WHERE ( id = IFNULL((SELECT MIN({{ID}}) FROM {{TRecord.TableName}} WHERE {DateCreated} > '{0}', 0) );
                                                     """;
    public readonly string _nextID_WhereDateCreated = $$"""
                                                        SELECT {{ID}} FROM {{TRecord.TableName}}
                                                        WHERE ( id = IFNULL((SELECT MIN({{ID}}) FROM {{TRecord.TableName}} WHERE {{DateCreated}} > '{0}'), 0) );
                                                        """;
    public readonly string _random = $"""
                                      SELECT * FROM {TRecord.TableName} 
                                      ORDER BY RANDOM()
                                      LIMIT 1;
                                      """;
    public readonly string _randomCount = $$"""
                                            SELECT * FROM {{TRecord.TableName}} 
                                            ORDER BY RANDOM() 
                                            LIMIT {0};
                                            """;
    public readonly string _randomCountWhereCreatedBy = $$"""
                                                          SELECT * FROM {{TRecord.TableName}} 
                                                          WHERE {{CreatedBy}} = '{0}'
                                                          ORDER BY RANDOM() 
                                                          LIMIT {1};
                                                          """;
    public readonly string _sortedIDs = $"""
                                         SELECT {ID}, {DateCreated} FROM {TRecord.TableName} 
                                         ORDER BY {DateCreated} DESC;
                                         """;
    public readonly string _tryInsert = $$"""
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
                                              SELECT {{ID}} = NULL;
                                          END
                                          """;
    public readonly string _updateID = $"""
                                        UPDATE {TRecord.TableName} 
                                        SET {string.Join( ',', KeyValuePairs )} 
                                        WHERE {ID} = @{ID};
                                        """;
    public readonly string _updateOrInsert = $$"""
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
    public readonly string _where = $$"""
                                      SELECT * FROM {{TRecord.TableName}} 
                                      WHERE {0};
                                      """;


    public static IEnumerable<string> KeyValuePairs => SqlProperties.Values.Select( Descriptor.GetKeyValuePair );


    public SqlCommand Random()                                       => _random;
    public SqlCommand Random( int                  count )           => new(string.Format( _randomCount, count.ToString() ));
    public SqlCommand Random( UserRecord           user, int count ) => Random( user.ID, count );
    public SqlCommand Random( RecordID<UserRecord> id,   int count ) => new(string.Format( _randomCountWhereCreatedBy, id.Value.ToString(), count.ToString() ));


    public SqlCommand WherePaged( bool                              matchAll, DynamicParameters parameters, int start, int count ) => new(string.Format( _getPagedWhere, string.Join( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) ), start.ToString(), count.ToString() ));
    public SqlCommand WherePaged( ref readonly RecordID<UserRecord> id,       int               start,      int count ) => new(string.Format( _getPagedWhereCreatedBy, id.Value.ToString(), start.ToString(), count.ToString() ));
    public SqlCommand WherePaged( int                               start,    int               count ) => new(string.Format( _getPaged, start.ToString(), count.ToString() ));


    public SqlCommand Get( ref readonly RecordID<TRecord> id )                                     => new(string.Format( _getWhereID, id.value.ToString() ));
    public string     Get( IEnumerable<RecordID<TRecord>> ids )                                    => string.Format( _getWhereIDs, string.Join( ',', ids.Select( GetValue ) ) );
    public SqlCommand Get( bool                           matchAll, DynamicParameters parameters ) => new(string.Format( _where, string.Join( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) ) ), parameters);
    public SqlCommand GetAll()   => _all;
    public SqlCommand GetFirst() => _first;
    public SqlCommand GetLast()  => _last;


    public SqlCommand Count()                                               => _count;
    public SqlCommand SortedID()                                            => _sortedIDs;
    public SqlCommand Exists( bool matchAll, DynamicParameters parameters ) => new(string.Format( _exists, string.Join( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) ) ), parameters);


    public SqlCommand Delete( bool                             matchAll, DynamicParameters parameters ) => new(string.Format( _deleteIDs, string.Join( matchAll.GetAndOr(), GetKeyValuePairs( parameters ) ) ), parameters);
    public SqlCommand DeleteID( ref readonly RecordID<TRecord> id )  => new(string.Format( _deleteID, id.value.ToString() ));
    public string     Delete( IEnumerable<RecordID<TRecord>>   ids ) => string.Format( _deleteIDs, string.Join( ',', ids.Select( GetValue ) ) );
    public SqlCommand DeleteAll()                                    => _deleteAll;


    public SqlCommand Next( ref readonly   RecordPair<TRecord> pair ) => new(string.Format( _nextWhereDateCreated,    pair.DateCreated.ToString() ));
    public SqlCommand NextID( ref readonly RecordPair<TRecord> pair ) => new(string.Format( _nextID_WhereDateCreated, pair.DateCreated.ToString() ));


    public SqlCommand Insert( TRecord record ) => new(_insert, record.ToDynamicParameters());
    public SqlCommand Update( TRecord record ) => new(_updateID, record.ToDynamicParameters());
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


    public static IEnumerable<string> GetKeyValuePairs( DynamicParameters parameters ) => parameters.ParameterNames.Select( GetKeyValuePair );
    public static string              GetKeyValuePair( string             columnName ) => SqlProperties[columnName].KeyValuePair;
    public static Guid                GetValue( RecordID<TRecord>         id )         => id.value;
}
