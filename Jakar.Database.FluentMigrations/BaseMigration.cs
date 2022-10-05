// TrueLogic :: TrueLogic.Common.Hosting
// 08/02/2022  3:20 PM

namespace Jakar.Database.FluentMigrations;


/// <summary>
///     <para>
///         <see href="https://fluentmigrator.github.io/articles/fluent-interface.html"/>
///     </para>
/// </summary>
public abstract class Migration<TRecord> : Migration where TRecord : class
{
    public string CurrentScheme { get; }
    public string TableName     { get; } = typeof(TRecord).GetTableName();


    protected Migration( string currentScheme ) => CurrentScheme = currentScheme;


    protected ISchemaSchemaSyntax CheckSchema()
    {
        ISchemaSchemaSyntax schema = Schema.Schema(CurrentScheme);
        if ( !schema.Exists() ) { Create.Schema(CurrentScheme); }

        return schema;
    }


    protected ISchemaTableSyntax CheckTableSchema() => CheckSchema()
       .Table(TableName);


    protected ICreateTableWithColumnSyntax CreateTable() => Create.Table(TableName)
                                                                  .InSchema(CurrentScheme);


    protected void UniqueConstraint( string columnName ) => Create.UniqueConstraint()
                                                                  .OnTable(TableName)
                                                                  .WithSchema(CurrentScheme)
                                                                  .Column(columnName);
    protected void UniqueConstraint( string name, string columnName ) => Create.UniqueConstraint(name)
                                                                               .OnTable(TableName)
                                                                               .WithSchema(CurrentScheme)
                                                                               .Column(columnName);


    protected void UniqueConstraints( params string[] columnNames ) => Create.UniqueConstraint()
                                                                             .OnTable(TableName)
                                                                             .WithSchema(CurrentScheme)
                                                                             .Columns(columnNames);
    protected void UniqueConstraints( string name, params string[] columnNames ) => Create.UniqueConstraint(name)
                                                                                          .OnTable(TableName)
                                                                                          .WithSchema(CurrentScheme)
                                                                                          .Columns(columnNames);


    protected IAlterTableAddColumnOrAlterColumnOrSchemaOrDescriptionSyntax AlterTable() => Alter.Table(TableName);


    protected void DeleteTable() => Delete.Table(TableName)
                                          .InSchema(CurrentScheme);


    /// <param name="dataAsAnonymousType">The columns and values to be used set</param>
    protected IUpdateWhereSyntax UpdateTable( object dataAsAnonymousType ) => Update.Table(TableName)
                                                                                    .InSchema(CurrentScheme)
                                                                                    .Set(dataAsAnonymousType);


    protected void RenameTable( string name ) => Rename.Table(TableName)
                                                       .InSchema(CurrentScheme)
                                                       .To(name)
                                                       .InSchema(CurrentScheme);


    protected IInsertDataSyntax StartInsert() => Insert.IntoTable(TableName);
    protected IInsertDataSyntax StartIdentityInsert() => Insert.IntoTable(TableName)
                                                               .WithIdentityInsert();
}
