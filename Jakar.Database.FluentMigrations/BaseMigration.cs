// TrueLogic :: TrueLogic.Common.Hosting
// 08/02/2022  3:20 PM

namespace Jakar.Database.FluentMigrations;


/// <summary>
///     <para>
///         <see href="https://fluentmigrator.github.io/articles/fluent-interface.html"/>
///     </para>
/// </summary>
public abstract class BaseMigration : Migration
{
    private readonly Type[]? _types;

    public string CurrentScheme { get; protected set; } = "dbo";


    protected BaseMigration() { }
    protected BaseMigration( params Type[] types ) => _types = types;


    protected ISchemaSchemaSyntax CheckSchema()
    {
        ISchemaSchemaSyntax schema = Schema.Schema(CurrentScheme);
        if ( !schema.Exists() ) { Create.Schema(CurrentScheme); }

        return schema;
    }


    protected ISchemaTableSyntax CheckSchema<T>() where T : TableRecord<T> => CheckSchema()
       .Table(typeof(T).GetTableName());
    protected ISchemaTableSyntax CheckSchema( Type type ) => CheckSchema()
       .Table(type.GetTableName());
    protected ISchemaTableSyntax CheckSchema( string tableName ) => CheckSchema()
       .Table(tableName);


    protected ICreateTableWithColumnSyntax CreateTable<T>() where T : TableRecord<T> => Create.Table(typeof(T).GetTableName())
                                                                                              .InSchema(CurrentScheme);
    protected ICreateTableWithColumnSyntax CreateTable( Type type ) => Create.Table(type.GetTableName())
                                                                             .InSchema(CurrentScheme);
    protected ICreateTableWithColumnSyntax CreateTable( string tableName ) => Create.Table(tableName)
                                                                                    .InSchema(CurrentScheme);


    protected IInsertDataSyntax StartIdentityInsert<T>() where T : TableRecord<T> => Insert.IntoTable(typeof(T).GetTableName())
                                                                                           .WithIdentityInsert();
    protected IInsertDataSyntax StartIdentityInsert( Type type ) => Insert.IntoTable(type.GetTableName())
                                                                          .WithIdentityInsert();
    protected IInsertDataSyntax StartIdentityInsert( string tableName ) => Insert.IntoTable(tableName)
                                                                                 .WithIdentityInsert();


    protected IInsertDataSyntax StartInsert<T>() where T : TableRecord<T> => Insert.IntoTable(typeof(T).GetTableName());
    protected IInsertDataSyntax StartInsert( Type   type ) => Insert.IntoTable(type.GetTableName());
    protected IInsertDataSyntax StartInsert( string tableName ) => Insert.IntoTable(tableName);
}



public abstract class BaseMigration<TRecord> : BaseMigration where TRecord : TableRecord<TRecord>
{
    public static string TableName { get; } = typeof(TRecord).GetTableName();
}
