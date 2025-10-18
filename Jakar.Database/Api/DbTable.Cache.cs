// Jakar.Extensions :: Jakar.Database
// 10/18/2025  00:45

namespace Jakar.Database;


public partial class DbTable<TSelf>
{
    public static readonly SqlCache SQLCache = new();



    [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
    [SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public sealed class SqlCache
    {
        private const string SPACER = ",      \n";
        private static readonly string __createdBy = nameof(ICreatedBy.CreatedBy)
           .ToSnakeCase();
        private static readonly string __dateCreated = nameof(IDateCreated.DateCreated)
           .ToSnakeCase();
        private static readonly string __id = nameof(IDateCreated.ID)
           .ToSnakeCase();
        private static readonly FrozenDictionary<string, Descriptor> __sqlProperties = Descriptor.CreateMapping<TSelf>();


        private string? __all;
        private string? __count;
        private string? __deleteAll;
        private string? __first;
        private string? __insert;
        private string? __last;
        private string? __random;
        private string? __sortedIDs;
        private string? __updateID;
        private string? __whereColumnValue;


        public static IEnumerable<string> KeyValuePairs => __sqlProperties.Values.Select(Descriptor.GetKeyValuePair);


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
                          WHERE {__createdBy} = '{id.Value}'
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
                          WHERE {__createdBy} = '{id.Value}'
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
                           WHERE {{__id}} = '{0}';
                           """;

            return new SqlCommand(string.Format(sql, id.Value.ToString()));
        }
        public SqlCommand Get( IEnumerable<RecordID<TSelf>> ids )
        {
            string sql = $"""
                          SELECT * FROM {TSelf.TableName}
                          WHERE {__id} in ({string.Join(',', ids.Select(GetValue))});
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
                                                     ORDER BY {__dateCreated} ASC 
                                                     LIMIT 1;
                                                     """;
        public SqlCommand GetLast() => __last ??= $"""
                                                   SELECT * FROM {TSelf.TableName} 
                                                   ORDER BY {__dateCreated} DESC 
                                                   LIMIT 1
                                                   """;


        public SqlCommand GetCount() => __count ??= $"SELECT COUNT(*) FROM {TSelf.TableName};";
        public SqlCommand GetSortedID() => __sortedIDs ??= $"""
                                                            SELECT {__id}, {__dateCreated} FROM {TSelf.TableName} 
                                                            ORDER BY {__dateCreated} DESC;
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
                          WHERE {__id} in ({string.Join(matchAll.GetAndOr(), GetKeyValuePairs(parameters))});
                          """;

            return new SqlCommand(sql, parameters);
        }
        public SqlCommand GetDeleteID( ref readonly RecordID<TSelf> id )
        {
            string sql = $"""
                          DELETE FROM {TSelf.TableName}
                          WHERE {__id} = '{id.Value}';
                          """;

            return new SqlCommand(sql);
        }
        public SqlCommand GetDelete( IEnumerable<RecordID<TSelf>> ids )
        {
            string sql = $"""
                          DELETE FROM {TSelf.TableName} 
                          WHERE {__id} in ({string.Join(',', ids.Select(GetValue))});
                          """;

            return new SqlCommand(sql);
        }
        public SqlCommand GetDeleteAll() => __deleteAll ??= $"DELETE FROM {TSelf.TableName};";


        public SqlCommand GetNext( ref readonly RecordPair<TSelf> pair )
        {
            string sql = $"""
                          SELECT * FROM {TSelf.TableName}
                          WHERE ( id = IFNULL((SELECT MIN({__dateCreated}) FROM {TSelf.TableName} WHERE {__dateCreated} > '{pair.DateCreated}' LIMIT 2, 0) );
                          """;

            return new SqlCommand(string.Format(sql));
        }
        public SqlCommand GetNextID( ref readonly RecordPair<TSelf> pair )
        {
            string sql = $"""
                          SELECT {__id} FROM {TSelf.TableName}
                          WHERE ( id = IFNULL((SELECT MIN({__dateCreated}) FROM {TSelf.TableName} WHERE {__dateCreated} > '{pair.DateCreated}' LIMIT 2), 0) );
                          """;

            return new SqlCommand(sql);
        }
        public SqlCommand GetInsert( TSelf record )
        {
            __insert ??= $"""
                              INSERT INTO {TSelf.TableName} 
                              (
                              {string.Join(SPACER, __sqlProperties.Values.Select(Descriptor.GetColumnName))}
                              ) 
                              values 
                              (
                                 {string.Join(SPACER, __sqlProperties.Values.Select(Descriptor.GetVariableName))}
                              ) 
                              RETURNING {__id};
                          """;

            return new SqlCommand(__insert, record.ToDynamicParameters());
        }
        public SqlCommand GetUpdate( TSelf record ) => new(__updateID ??= $"""
                                                                           UPDATE {TSelf.TableName} 
                                                                           SET {string.Join(',', KeyValuePairs)} 
                                                                           WHERE {__id} = @{__id};
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
                          {string.Join(SPACER, __sqlProperties.Values.Select(Descriptor.GetColumnName))}
                          ) 
                          values 
                          (
                          {string.Join(SPACER, __sqlProperties.Values.Select(Descriptor.GetVariableName))}
                          ) 
                          RETURNING {__id};
                          END

                          ELSE
                          BEGIN
                          SELECT {__id} = '{Guid.Empty}';
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
                          {string.Join(SPACER, __sqlProperties.Values.Select(Descriptor.GetColumnName))}
                          ) 
                          values 
                          (
                          {string.Join(SPACER, __sqlProperties.Values.Select(Descriptor.GetVariableName))}
                          ) 
                          RETURNING {__id};
                          END

                          ELSE
                          BEGIN
                          UPDATE {TSelf.TableName} SET {KeyValuePairs} WHERE {__id} = @{__id};
                          SELECT @{__id};
                          END
                          """;

            return new SqlCommand(sql, parameters);
        }


        public static IEnumerable<string> GetKeyValuePairs( DynamicParameters parameters ) => parameters.ParameterNames.Select(GetKeyValuePair);
        public static string              GetKeyValuePair( string             columnName ) => __sqlProperties[columnName].KeyValuePair;
        public static Guid                GetValue( RecordID<TSelf>           id )         => id.Value;
    }
}
