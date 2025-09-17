// Jakar.Extensions :: Jakar.Database
// 01/01/2025  16:01

using FluentMigrator.Runner.Generators.Base;



namespace Jakar.Database;


[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
public sealed class SqlCache<TClass>
    where TClass : class, ITableRecord<TClass>, IDbReaderMapping<TClass>
{
    public const           string                               SPACER        = ",      \n";
    public static readonly string                               CreatedBy     = nameof(IOwnedTableRecord.CreatedBy).ToSnakeCase();
    public static readonly string                               DateCreated   = nameof(IRecordPair.DateCreated).ToSnakeCase();
    public static readonly string                               ID            = nameof(IRecordPair.ID).ToSnakeCase();
    public static readonly string                               Ids           = "ids";
    public static readonly string                               LastModified  = nameof(ITableRecord.LastModified).ToSnakeCase();
    public static readonly FrozenDictionary<string, Descriptor> SqlProperties = Descriptor.CreateMapping<TClass>();


    public readonly string All       = $"SELECT * FROM {TClass.TableName};";
    public readonly string Count     = $"SELECT COUNT(*) FROM {TClass.TableName};";
    public readonly string DeleteAll = $"DELETE FROM {TClass.TableName};";
    private readonly string __deleteID = $$"""
                                           DELETE FROM {{TClass.TableName}}
                                           WHERE {{ID}} = '{0}';
                                           """;
    private readonly string __deleteIDs = $$"""
                                            DELETE FROM {{TClass.TableName}} 
                                            WHERE {{ID}} in ({0});
                                            """;
    private readonly string __exists = $$"""
                                         EXISTS( 
                                         SELECT * FROM {{TClass.TableName}} 
                                         WHERE {0} 
                                         );
                                         """;
    public readonly string First = $"""
                                    SELECT * FROM {TClass.TableName} 
                                    ORDER BY {DateCreated} ASC 
                                    LIMIT 1;
                                    """;
    private readonly string __getPaged = $$"""
                                           SELECT * FROM {{TClass.TableName}} 
                                           OFFSET {0}
                                           LIMIT {1};
                                           """;
    private readonly string __getPagedWhere = $$"""
                                                SELECT * FROM {{TClass.TableName}} 
                                                {0} 
                                                OFFSET {1}
                                                LIMIT {2};
                                                """;
    public readonly string GetPagedWhereCreatedBy = $$"""
                                                      SELECT * FROM {{TClass.TableName}} 
                                                      WHERE {{CreatedBy}} = '{0}'
                                                      OFFSET {1}
                                                      LIMIT {2};
                                                      """;
    private readonly string __getWhereID = $$"""
                                             SELECT * FROM {{TClass.TableName}} 
                                             WHERE {{ID}} = '{0}';
                                             """;
    private readonly string __getWhereIDs = $$"""
                                              SELECT * FROM {{TClass.TableName}} 
                                              WHERE {{ID}} in ({0});
                                              """;
    public readonly string Insert = $"""
                                         INSERT INTO {TClass.TableName} 
                                         (
                                         {string.Join(SPACER, SqlProperties.Values.Select(Descriptor.GetColumnName))}
                                         ) 
                                         values 
                                         (
                                            {string.Join(SPACER, SqlProperties.Values.Select(Descriptor.GetVariableName))}
                                         ) 
                                         RETURNING {ID};
                                     """;
    public readonly string Last = $"""
                                   SELECT * FROM {TClass.TableName} 
                                   ORDER BY {DateCreated} DESC 
                                   LIMIT 1
                                   """;
    private readonly string __nextID_WhereDateCreated = $$"""
                                                          SELECT {{ID}} FROM {{TClass.TableName}}
                                                          WHERE ( id = IFNULL((SELECT MIN({{ID}}) FROM {{TClass.TableName}} WHERE {{DateCreated}} > '{0}'), 0) );
                                                          """;
    private readonly string __nextWhereDateCreated = $$"""
                                                       SELECT * FROM {{TClass.TableName}}
                                                       WHERE ( id = IFNULL((SELECT MIN({{ID}}) FROM {{TClass.TableName}} WHERE {DateCreated} > '{0}', 0) );
                                                       """;
    public readonly string Random = $"""
                                     SELECT * FROM {TClass.TableName} 
                                     ORDER BY RANDOM()
                                     LIMIT 1;
                                     """;
    private readonly string __randomCount = $$"""
                                              SELECT * FROM {{TClass.TableName}} 
                                              ORDER BY RANDOM() 
                                              LIMIT {0};
                                              """;
    private readonly string __randomCountWhereCreatedBy = $$"""
                                                            SELECT * FROM {{TClass.TableName}} 
                                                            WHERE {{CreatedBy}} = '{0}'
                                                            ORDER BY RANDOM() 
                                                            LIMIT {1};
                                                            """;
    public readonly string SortedIDs = $"""
                                        SELECT {ID}, {DateCreated} FROM {TClass.TableName} 
                                        ORDER BY {DateCreated} DESC;
                                        """;
    public readonly string TryInsert = $$"""
                                         IF NOT EXISTS(SELECT * FROM {TClass.TableName} WHERE {0})
                                         BEGIN
                                             INSERT INTO {{TClass.TableName}}
                                             (
                                             {{string.Join(SPACER, SqlProperties.Values.Select(Descriptor.GetColumnName))}}
                                             ) 
                                             values 
                                             (
                                                {{string.Join(SPACER, SqlProperties.Values.Select(Descriptor.GetVariableName))}}
                                             ) 
                                             RETURNING {{ID}};
                                         END

                                         ELSE
                                         BEGIN
                                             SELECT {{ID}} = NULL;
                                         END
                                         """;
    public readonly string UpdateID = $"""
                                       UPDATE {TClass.TableName} 
                                       SET {string.Join(',', KeyValuePairs)} 
                                       WHERE {ID} = @{ID};
                                       """;
    public readonly string UpdateOrInsert = $$"""
                                              IF NOT EXISTS(SELECT * FROM {TClass.TableName} WHERE {0})
                                              BEGIN
                                                  INSERT INTO {{TClass.TableName}}
                                                  (
                                                    {{string.Join(SPACER, SqlProperties.Values.Select(Descriptor.GetColumnName))}}
                                                  ) 
                                                  values 
                                                  (
                                                    {{string.Join(SPACER, SqlProperties.Values.Select(Descriptor.GetVariableName))}}
                                                  ) 
                                                  RETURNING {{ID}};
                                              END

                                              ELSE
                                              BEGIN
                                                  UPDATE {{TClass.TableName}} SET {{KeyValuePairs}} WHERE {{ID}} = @{{ID}};
                                                  SELECT @{{ID}};
                                              END
                                              """;
    private readonly string __where = $$"""
                                        SELECT * FROM {{TClass.TableName}} 
                                        WHERE {0};
                                        """;
    private string? __whereColumnValue;


    public static IEnumerable<string> KeyValuePairs => SqlProperties.Values.Select(Descriptor.GetKeyValuePair);


    public SqlCommand GetRandom()                                       => Random;
    public SqlCommand GetRandom( int                  count )           => new(string.Format(__randomCount, count.ToString()));
    public SqlCommand GetRandom( UserRecord           user, int count ) => GetRandom(user.ID, count);
    public SqlCommand GetRandom( RecordID<UserRecord> id,   int count ) => new(string.Format(__randomCountWhereCreatedBy, id.Value.ToString(), count.ToString()));


    public SqlCommand WherePaged( bool                              matchAll, DynamicParameters parameters, int start, int count ) => new(string.Format(__getPagedWhere, string.Join(matchAll.GetAndOr(), GetKeyValuePairs(parameters)), start.ToString(), count.ToString()));
    public SqlCommand WherePaged( ref readonly RecordID<UserRecord> id,       int               start,      int count ) => new(string.Format(GetPagedWhereCreatedBy, id.Value.ToString(), start.ToString(), count.ToString()));
    public SqlCommand WherePaged( int                               start,    int               count ) => new(string.Format(__getPaged, start.ToString(), count.ToString()));
    public SqlCommand Where<TValue>( string columnName, TValue? value )
    {
        __whereColumnValue ??= $"SELECT * FROM {TClass.TableName} WHERE {columnName} = @{nameof(value)};";
        
        DynamicParameters parameters = new();
        parameters.Add(nameof(value), value);

        return new SqlCommand(__whereColumnValue, parameters);
    }


    public SqlCommand Get( ref readonly RecordID<TClass> id )  => new(string.Format(__getWhereID, id.value.ToString()));
    public SqlCommand Get( IEnumerable<RecordID<TClass>> ids ) => string.Format(__getWhereIDs, string.Join(',', ids.Select(GetValue)));
    public SqlCommand Get( bool matchAll, DynamicParameters parameters )
    {
        using ValueStringBuilder sb = new(__where);
        sb.AppendJoin(matchAll.GetAndOr(), GetKeyValuePairs(parameters));

        return new SqlCommand(sb.ToString(), parameters);
    }
    public SqlCommand GetAll()   => All;
    public SqlCommand GetFirst() => First;
    public SqlCommand GetLast()  => Last;


    public SqlCommand GetCount()                                               => Count;
    public SqlCommand GetSortedID()                                            => SortedIDs;
    public SqlCommand GetExists( bool matchAll, DynamicParameters parameters ) => new(string.Format(__exists, string.Join(matchAll.GetAndOr(), GetKeyValuePairs(parameters))), parameters);


    public SqlCommand GetDelete( bool                            matchAll, DynamicParameters parameters ) => new(string.Format(__deleteIDs, string.Join(matchAll.GetAndOr(), GetKeyValuePairs(parameters))), parameters);
    public SqlCommand GetDeleteID( ref readonly RecordID<TClass> id )  => new(string.Format(__deleteID, id.value.ToString()));
    public SqlCommand GetDelete( IEnumerable<RecordID<TClass>>   ids ) => string.Format(__deleteIDs, string.Join(',', ids.Select(GetValue)));
    public SqlCommand GetDeleteAll()                                   => DeleteAll;


    public SqlCommand GetNext( ref readonly   RecordPair<TClass> pair ) => new(string.Format(__nextWhereDateCreated,    pair.DateCreated.ToString()));
    public SqlCommand GetNextID( ref readonly RecordPair<TClass> pair ) => new(string.Format(__nextID_WhereDateCreated, pair.DateCreated.ToString()));


    public SqlCommand GetInsert( TClass record ) => new(Insert, record.ToDynamicParameters());
    public SqlCommand GetUpdate( TClass record ) => new(UpdateID, record.ToDynamicParameters());
    public SqlCommand GetTryInsert( TClass record, bool matchAll, DynamicParameters parameters )
    {
        DynamicParameters param = record.ToDynamicParameters();
        param.AddDynamicParams(parameters);

        return new SqlCommand(string.Format(TryInsert, string.Join(matchAll.GetAndOr(), GetKeyValuePairs(parameters))), param);
    }
    public SqlCommand InsertOrUpdate( TClass record, bool matchAll, DynamicParameters parameters )
    {
        DynamicParameters param = record.ToDynamicParameters();
        param.AddDynamicParams(parameters);

        return new SqlCommand(string.Format(UpdateOrInsert, string.Join(matchAll.GetAndOr(), GetKeyValuePairs(parameters)), param));
    }


    public static IEnumerable<string> GetKeyValuePairs( DynamicParameters parameters ) => parameters.ParameterNames.Select(GetKeyValuePair);
    public static string              GetKeyValuePair( string             columnName ) => SqlProperties[columnName].KeyValuePair;
    public static Guid                GetValue( RecordID<TClass>          id )         => id.value;
}
