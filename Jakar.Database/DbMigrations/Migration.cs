// Jakar.Database ::  Jakar.Database 
// 08/02/2022  3:20 PM

namespace Jakar.Database.DbMigrations;


/// <summary>
///     <para>
///         <see href="https://fluentmigrator.github.io/articles/fluent-interface.html"/>
///     </para>
/// </summary>
public abstract class Migration<TRecord> : Migration where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    private        Database? _dbContext;
    public virtual string    CurrentScheme => _dbContext?.CurrentSchema ?? throw new NullReferenceException( nameof(_dbContext) );
    public         string    TableName     { get; } = typeof(TRecord).GetTableName();


    protected Migration() : base() { }


    public override void GetUpExpressions( IMigrationContext context )
    {
        _dbContext = context.ServiceProvider.GetRequiredService<Database>();
        base.GetUpExpressions( context );
    }
    public override void GetDownExpressions( IMigrationContext context )
    {
        _dbContext = context.ServiceProvider.GetRequiredService<Database>();
        base.GetDownExpressions( context );
    }


    protected IAlterTableAddColumnOrAlterColumnOrSchemaOrDescriptionSyntax AlterTable() => Alter.Table( TableName );


    protected virtual ICreateTableWithColumnSyntax CreateTable()
    {
        if ( Schema.Schema( CurrentScheme )
                   .Exists() is false ) { Create.Schema( CurrentScheme ); }

        ICreateTableWithColumnSyntax? table = Create.Table( TableName )
                                                    .InSchema( CurrentScheme );


        table.WithColumn( nameof(TableRecord<TRecord>.ID) )
             .AsGuid()
             .PrimaryKey();

        table.WithColumn( nameof(TableRecord<TRecord>.DateCreated) )
             .AsDateTimeOffset()
             .NotNullable()
             .WithDefaultValue( SystemMethods.CurrentUTCDateTime );

        table.WithColumn( nameof(TableRecord<TRecord>.LastModified) )
             .AsDateTimeOffset()
             .Nullable();

        return table;
    }


    protected IInsertDataSyntax StartInsert() => Insert.IntoTable( TableName );


    protected ISchemaSchemaSyntax _CheckSchema
    {
        get
        {
            ISchemaSchemaSyntax schema = Schema.Schema( CurrentScheme );
            if ( !schema.Exists() ) { _ = Create.Schema( CurrentScheme ); }

            return schema;
        }
    }
    protected ISchemaTableSyntax CheckTableSchema() => _CheckSchema.Table( TableName );


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



public abstract class OwnedMigration<TRecord> : Migration<TRecord> where TRecord : OwnedTableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        var table = base.CreateTable();

        table.WithColumn( nameof(OwnedTableRecord<TRecord>.OwnerUserID) )
             .AsGuid()
             .Nullable();

        table.WithColumn( nameof(OwnedTableRecord<TRecord>.CreatedBy) )
             .AsGuid()
             .Nullable();

        return table;
    }
}
