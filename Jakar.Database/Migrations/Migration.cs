// TrueLogic :: TrueLogic.Common.Hosting
// 08/02/2022  3:20 PM

namespace Jakar.Database.Migrations;


/// <summary>
///     <para>
///         <see href="https://fluentmigrator.github.io/articles/fluent-interface.html"/>
///     </para>
/// </summary>
public abstract class Migration<TRecord> : Migration where TRecord : TableRecord<TRecord>
{
    public virtual string CurrentScheme => DatabaseDefaults.Schema;
    public         string TableName     { get; } = typeof(TRecord).GetTableName();


    protected Migration() : base() { }


    protected IAlterTableAddColumnOrAlterColumnOrSchemaOrDescriptionSyntax AlterTable() => Alter.Table( TableName );


    protected virtual ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax? table = Create.Table( TableName )
                                                    .InSchema( CurrentScheme );


        table.WithColumn( nameof(TableRecord<TRecord>.ID) )
             .AsInt64()
             .NotNullable()
             .PrimaryKey()
             .Identity();

        table.WithColumn( nameof(TableRecord<TRecord>.UserID) )
             .AsGuid()
             .NotNullable();

        table.WithColumn( nameof(TableRecord<TRecord>.CreatedBy) )
             .AsInt64()
             .Nullable();

        table.WithColumn( nameof(TableRecord<TRecord>.DateCreated) )
             .AsDateTime2()
             .NotNullable();

        table.WithColumn( nameof(TableRecord<TRecord>.LastModified) )
             .AsDateTime2()
             .Nullable();

        return table;
    }
    protected IInsertDataSyntax StartIdentityInsert() => Insert.IntoTable( TableName )
                                                               .WithIdentityInsert();


    protected IInsertDataSyntax StartInsert() => Insert.IntoTable( TableName );


    protected ISchemaSchemaSyntax CheckSchema()
    {
        ISchemaSchemaSyntax schema = Schema.Schema( CurrentScheme );
        if ( !schema.Exists() ) { Create.Schema( CurrentScheme ); }

        return schema;
    }


    protected ISchemaTableSyntax CheckTableSchema() => CheckSchema()
       .Table( TableName );


    /// <param name="dataAsAnonymousType"> The columns and values to be used set </param>
    protected IUpdateWhereSyntax UpdateTable( object dataAsAnonymousType ) => Update.Table( TableName )
                                                                                    .InSchema( CurrentScheme )
                                                                                    .Set( dataAsAnonymousType );


    protected void DeleteTable() => Delete.Table( TableName )
                                          .InSchema( CurrentScheme );


    protected void RenameTable( string name ) => Rename.Table( TableName )
                                                       .InSchema( CurrentScheme )
                                                       .To( name )
                                                       .InSchema( CurrentScheme );


    protected void UniqueConstraint( string columnName ) => Create.UniqueConstraint()
                                                                  .OnTable( TableName )
                                                                  .WithSchema( CurrentScheme )
                                                                  .Column( columnName );
    protected void UniqueConstraint( string name, string columnName ) => Create.UniqueConstraint( name )
                                                                               .OnTable( TableName )
                                                                               .WithSchema( CurrentScheme )
                                                                               .Column( columnName );


    protected void UniqueConstraints( params string[] columnNames ) => Create.UniqueConstraint()
                                                                             .OnTable( TableName )
                                                                             .WithSchema( CurrentScheme )
                                                                             .Columns( columnNames );
    protected void UniqueConstraints( string name, params string[] columnNames ) => Create.UniqueConstraint( name )
                                                                                          .OnTable( TableName )
                                                                                          .WithSchema( CurrentScheme )
                                                                                          .Columns( columnNames );
}
