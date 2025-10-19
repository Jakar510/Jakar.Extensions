// Jakar.Extensions :: Jakar.Database
// 10/18/2025  00:45

namespace Jakar.Database;


public partial class DbTable<TSelf>
{
    public readonly SqlCommands SQLCache;



    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public class SqlCommands
    {
        protected const           string                                      SPACER          = ",      \n";
        protected const           string                                      CREATED_BY      = "created_by";
        protected const           string                                      DATE_CREATED    = "date_created";
        protected const           string                                      ID              = "id";
        protected static readonly FrozenDictionary<string, Descriptor<TSelf>> __sqlProperties = Descriptor<TSelf>.CreateMapping();


        protected string? __all;
        protected string? __count;
        protected string? __deleteAll;
        protected string? __first;
        protected string? __insert;
        protected string? __last;
        protected string? __random;
        protected string? __sortedIDs;
        protected string? __updateID;
        protected string? __whereColumnValue;


        public static IEnumerable<string> KeyValuePairs => __sqlProperties.Values.Select(Descriptor<TSelf>.GetKeyValuePair);


        public SqlCommand GetRandom() => __random ??= $"""
                                                       SELECT * FROM {TSelf.TableName} 
                                                       ORDER BY RANDOM()
                                                       LIMIT 1;
                                                       """;
        public SqlCommand GetRandom( int count )
        {
            string sql = $"""
                          SELECT * FROM {TSelf.TableName} 
                          ORDER BY RANDOM() 
                          LIMIT {count};
                          """;

            return new SqlCommand(sql);
        }
        public SqlCommand GetRandom( UserRecord user, int count ) => GetRandom(user.ID, count);
        public SqlCommand GetRandom( RecordID<UserRecord> id, int count )
        {
            string sql = $"""
                          SELECT * FROM {TSelf.TableName} 
                          WHERE {CREATED_BY} = '{id.Value}'
                          ORDER BY RANDOM() 
                          LIMIT {count};
                          """;

            return new SqlCommand(sql);
        }


        public SqlCommand WherePaged( bool matchAll, DynamicParameters parameters, int start, int count )
        {
            string sql = $"""
                            SELECT * FROM {TSelf.TableName}
                            {string.Join(matchAll.GetAndOr(), GetKeyValuePairs(parameters))}
                            OFFSET {start}
                            LIMIT {count}
                          """;

            return new SqlCommand(sql, parameters);
        }
        public SqlCommand WherePaged( ref readonly RecordID<UserRecord> id, int start, int count )
        {
            string sql = $"""
                          SELECT * FROM {TSelf.TableName} 
                          WHERE {CREATED_BY} = '{id.Value}'
                          OFFSET {start}
                          LIMIT {count};
                          """;

            return new SqlCommand(sql);
        }
        public SqlCommand WherePaged( int start, int count )
        {
            string sql = $"""
                          SELECT * FROM {TSelf.TableName} 
                          OFFSET {start}
                          LIMIT {count};
                          """;

            return new SqlCommand(sql);
        }
        public SqlCommand Where<TValue>( string columnName, TValue? value )
        {
            __whereColumnValue ??= $"SELECT * FROM {TSelf.TableName} WHERE {columnName} = @{nameof(value)};";

            DynamicParameters parameters = new();
            parameters.Add(nameof(value), value);

            return new SqlCommand(__whereColumnValue, parameters);
        }


        public SqlCommand Get( ref readonly RecordID<TSelf> id )
        {
            string sql = $$"""
                           SELECT * FROM {{TSelf.TableName}} 
                           WHERE {{ID}} = '{0}';
                           """;

            return new SqlCommand(string.Format(sql, id.Value.ToString()));
        }
        public SqlCommand Get( IEnumerable<RecordID<TSelf>> ids )
        {
            string sql = $"""
                          SELECT * FROM {TSelf.TableName}
                          WHERE {ID} in ({string.Join(',', ids.Select(GetValue))});
                          """;

            return new SqlCommand(sql);
        }
        public SqlCommand Get( bool matchAll, DynamicParameters parameters )
        {
            string sql = $"""
                          SELECT * FROM {TSelf.TableName}
                          WHERE {string.Join(matchAll.GetAndOr(), GetKeyValuePairs(parameters))};
                          """;

            return new SqlCommand(sql, parameters);
        }
        public SqlCommand GetAll() => __all ??= $"SELECT * FROM {TSelf.TableName};";
        public SqlCommand GetFirst() => __first ??= $"""
                                                     SELECT * FROM {TSelf.TableName} 
                                                     ORDER BY {DATE_CREATED} ASC 
                                                     LIMIT 1;
                                                     """;
        public SqlCommand GetLast() => __last ??= $"""
                                                   SELECT * FROM {TSelf.TableName} 
                                                   ORDER BY {DATE_CREATED} DESC 
                                                   LIMIT 1
                                                   """;


        public SqlCommand GetCount() => __count ??= $"SELECT COUNT(*) FROM {TSelf.TableName};";
        public SqlCommand GetSortedID() => __sortedIDs ??= $"""
                                                            SELECT {ID}, {DATE_CREATED} FROM {TSelf.TableName} 
                                                            ORDER BY {DATE_CREATED} DESC;
                                                            """;
        public SqlCommand GetExists( bool matchAll, DynamicParameters parameters )
        {
            string sql = $"""
                          EXISTS( 
                          SELECT * FROM {TSelf.TableName}
                          WHERE {string.Join(matchAll.GetAndOr(), GetKeyValuePairs(parameters))};
                          """;

            return new SqlCommand(sql, parameters);
        }


        public SqlCommand GetDelete( bool matchAll, DynamicParameters parameters )
        {
            string sql = $"""
                          DELETE FROM {TSelf.TableName} 
                          WHERE {ID} in ({string.Join(matchAll.GetAndOr(), GetKeyValuePairs(parameters))});
                          """;

            return new SqlCommand(sql, parameters);
        }
        public SqlCommand GetDeleteID( ref readonly RecordID<TSelf> id )
        {
            string sql = $"""
                          DELETE FROM {TSelf.TableName}
                          WHERE {ID} = '{id.Value}';
                          """;

            return new SqlCommand(sql);
        }
        public SqlCommand GetDelete( IEnumerable<RecordID<TSelf>> ids )
        {
            string sql = $"""
                          DELETE FROM {TSelf.TableName} 
                          WHERE {ID} in ({string.Join(',', ids.Select(GetValue))});
                          """;

            return new SqlCommand(sql);
        }
        public SqlCommand GetDeleteAll() => __deleteAll ??= $"DELETE FROM {TSelf.TableName};";


        public SqlCommand GetNext( ref readonly RecordPair<TSelf> pair )
        {
            string sql = $"""
                          SELECT * FROM {TSelf.TableName}
                          WHERE ( id = IFNULL((SELECT MIN({DATE_CREATED}) FROM {TSelf.TableName} WHERE {DATE_CREATED} > '{pair.DateCreated}' LIMIT 2, 0) );
                          """;

            return new SqlCommand(string.Format(sql));
        }
        public SqlCommand GetNextID( ref readonly RecordPair<TSelf> pair )
        {
            string sql = $"""
                          SELECT {ID} FROM {TSelf.TableName}
                          WHERE ( id = IFNULL((SELECT MIN({DATE_CREATED}) FROM {TSelf.TableName} WHERE {DATE_CREATED} > '{pair.DateCreated}' LIMIT 2), 0) );
                          """;

            return new SqlCommand(sql);
        }
        public SqlCommand GetInsert( TSelf record )
        {
            __insert ??= $"""
                              INSERT INTO {TSelf.TableName} 
                              (
                              {string.Join(SPACER, __sqlProperties.Values.Select(Descriptor<TSelf>.GetColumnName))}
                              ) 
                              values 
                              (
                                 {string.Join(SPACER, __sqlProperties.Values.Select(Descriptor<TSelf>.GetVariableName))}
                              ) 
                              RETURNING {ID};
                          """;

            return new SqlCommand(__insert, record.ToDynamicParameters());
        }
        public SqlCommand GetUpdate( TSelf record ) => new(__updateID ??= $"""
                                                                           UPDATE {TSelf.TableName} 
                                                                           SET {string.Join(',', KeyValuePairs)} 
                                                                           WHERE {ID} = @{ID};
                                                                           """,
                                                           record.ToDynamicParameters());
        public SqlCommand GetTryInsert( TSelf record, bool matchAll, DynamicParameters parameters )
        {
            DynamicParameters param = record.ToDynamicParameters();
            param.AddDynamicParams(parameters);

            string sql = $"""
                          IF NOT EXISTS(SELECT * FROM {TSelf.TableName} WHERE {string.Join(matchAll.GetAndOr(), GetKeyValuePairs(parameters))})
                          BEGIN
                          INSERT INTO {TSelf.TableName}
                          (
                          {string.Join(SPACER, __sqlProperties.Values.Select(Descriptor<TSelf>.GetColumnName))}
                          ) 
                          values 
                          (
                          {string.Join(SPACER, __sqlProperties.Values.Select(Descriptor<TSelf>.GetVariableName))}
                          ) 
                          RETURNING {ID};
                          END

                          ELSE
                          BEGIN
                          SELECT {ID} = '{Guid.Empty}';
                          END
                          """;

            return new SqlCommand(sql, param);
        }
        public SqlCommand InsertOrUpdate( TSelf record, bool matchAll, DynamicParameters parameters )
        {
            DynamicParameters param = record.ToDynamicParameters();
            param.AddDynamicParams(parameters);

            string sql = $"""
                          IF NOT EXISTS(SELECT * FROM {TSelf.TableName} WHERE {string.Join(matchAll.GetAndOr(), GetKeyValuePairs(param))})
                          BEGIN
                          INSERT INTO {TSelf.TableName}
                          (
                          {string.Join(SPACER, __sqlProperties.Values.Select(Descriptor<TSelf>.GetColumnName))}
                          ) 
                          values 
                          (
                          {string.Join(SPACER, __sqlProperties.Values.Select(Descriptor<TSelf>.GetVariableName))}
                          ) 
                          RETURNING {ID};
                          END

                          ELSE
                          BEGIN
                          UPDATE {TSelf.TableName} SET {KeyValuePairs} WHERE {ID} = @{ID};
                          SELECT @{ID};
                          END
                          """;

            return new SqlCommand(sql, parameters);
        }


        public static IEnumerable<string> GetKeyValuePairs( DynamicParameters parameters ) => parameters.ParameterNames.Select(GetKeyValuePair);
        public static string              GetKeyValuePair( string             columnName ) => __sqlProperties[columnName].KeyValuePair;
        public static Guid                GetValue( RecordID<TSelf>           id )         => id.Value;
    }
}
