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


    public SqlCommand GetRandom() => Random;
    public SqlCommand GetRandom( int count )
    {
        string sql = $"""
                      SELECT * FROM {TClass.TableName} 
                      ORDER BY RANDOM() 
                      LIMIT {count};
                      """;

        return new SqlCommand(sql);
    }
    public SqlCommand GetRandom( UserRecord user, int count ) => GetRandom(user.ID, count);
    public SqlCommand GetRandom( RecordID<UserRecord> id, int count )
    {
        string sql = $"""
                      SELECT * FROM {TClass.TableName} 
                      WHERE {CreatedBy} = '{id.Value}'
                      ORDER BY RANDOM() 
                      LIMIT {count};
                      """;

        return new SqlCommand(sql);
    }


    public SqlCommand WherePaged( bool matchAll, DynamicParameters parameters, int start, int count )
    {
        string sql = $"""
                        SELECT * FROM {TClass.TableName}
                        {string.Join(matchAll.GetAndOr(), GetKeyValuePairs(parameters))}
                        OFFSET {start}
                        LIMIT {count}
                      """;

        return new SqlCommand(sql, parameters);
    }
    public SqlCommand WherePaged( ref readonly RecordID<UserRecord> id, int start, int count )
    {
        string sql = $"""
                      SELECT * FROM {TClass.TableName} 
                      WHERE {CreatedBy} = '{id.Value}'
                      OFFSET {start}
                      LIMIT {count};
                      """;

        return new SqlCommand(sql);
    }
    public SqlCommand WherePaged( int start, int count ) { return new SqlCommand(string.Format(__getPaged, start.ToString(), count.ToString())); }
    public SqlCommand Where<TValue>( string columnName, TValue? value )
    {
        __whereColumnValue ??= $"SELECT * FROM {TClass.TableName} WHERE {columnName} = @{nameof(value)};";

        DynamicParameters parameters = new();
        parameters.Add(nameof(value), value);

        return new SqlCommand(__whereColumnValue, parameters);
    }


    public SqlCommand Get( ref readonly RecordID<TClass> id )
    {
        string __getWhereID = $$"""
                                SELECT * FROM {{TClass.TableName}} 
                                WHERE {{ID}} = '{0}';
                                """;
        return new SqlCommand(string.Format(__getWhereID, id.value.ToString()));
    }
    public SqlCommand Get( IEnumerable<RecordID<TClass>> ids ) { return string.Format(__getWhereIDs, string.Join(',', ids.Select(GetValue))); }
    public SqlCommand Get( bool matchAll, DynamicParameters parameters )
    {
        using ValueStringBuilder sb = new(__where);
        sb.AppendJoin(matchAll.GetAndOr(), GetKeyValuePairs(parameters));

        return new SqlCommand(sb.ToString(), parameters);
    }
    public SqlCommand GetAll()   { return All; }
    public SqlCommand GetFirst() { return First; }
    public SqlCommand GetLast()  { return Last; }


    public SqlCommand GetCount()                                               { return Count; }
    public SqlCommand GetSortedID()                                            { return SortedIDs; }
    public SqlCommand GetExists( bool matchAll, DynamicParameters parameters ) { return new SqlCommand(string.Format(__exists, string.Join(matchAll.GetAndOr(), GetKeyValuePairs(parameters))), parameters); }


    public SqlCommand GetDelete( bool matchAll, DynamicParameters parameters )
    {
        using ValueStringBuilder sb = new(__deleteIDs);
        sb.AppendJoin(matchAll.GetAndOr(), GetKeyValuePairs(parameters));

        return new SqlCommand(sb.ToString(), parameters);
    }
    public SqlCommand GetDeleteID( ref readonly RecordID<TClass> id )  { return new SqlCommand(string.Format(__deleteID, id.value.ToString())); }
    public SqlCommand GetDelete( IEnumerable<RecordID<TClass>>   ids ) { return string.Format(__deleteIDs, string.Join(',', ids.Select(GetValue))); }
    public SqlCommand GetDeleteAll()                                   { return DeleteAll; }


    public SqlCommand GetNext( ref readonly   RecordPair<TClass> pair ) { return new SqlCommand(string.Format(__nextWhereDateCreated,    pair.DateCreated.ToString())); }
    public SqlCommand GetNextID( ref readonly RecordPair<TClass> pair ) { return new SqlCommand(string.Format(__nextID_WhereDateCreated, pair.DateCreated.ToString())); }


    public SqlCommand GetInsert( TClass record ) { return new SqlCommand(Insert,   record.ToDynamicParameters()); }
    public SqlCommand GetUpdate( TClass record ) { return new SqlCommand(UpdateID, record.ToDynamicParameters()); }
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

        using ValueStringBuilder sb = new(UpdateOrInsert);
        sb.AppendJoin(matchAll.GetAndOr(), GetKeyValuePairs(param));

        return new SqlCommand(sb.ToString(), parameters);
    }


    public static IEnumerable<string> GetKeyValuePairs( DynamicParameters parameters ) { return parameters.ParameterNames.Select(GetKeyValuePair); }
    public static string              GetKeyValuePair( string             columnName ) { return SqlProperties[columnName].KeyValuePair; }
    public static Guid                GetValue( RecordID<TClass>          id )         { return id.value; }
}
