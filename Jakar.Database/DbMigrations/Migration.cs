// Jakar.Database ::  Jakar.Database 
// 08/02/2022  3:20 PM

using FluentMigrator.Builders.Create.Constraint;



namespace Jakar.Database.DbMigrations;


/// <summary>
///     <para>
///         <see href="https://fluentmigrator.github.io/articles/fluent-interface.html"/>
///     </para>
/// </summary>
public abstract class Migration<TRecord> : Migration
    where TRecord : ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    public string TableName { get; } = typeof(TRecord).GetTableName();


    protected Migration() : base() { }


    // public override void GetUpExpressions( IMigrationContext   context ) => base.GetUpExpressions( context );   // _dbContext = context.ServiceProvider.GetRequiredService<Database>();
    // public override void GetDownExpressions( IMigrationContext context ) => base.GetDownExpressions( context ); // _dbContext = context.ServiceProvider.GetRequiredService<Database>();


    protected IAlterTableAddColumnOrAlterColumnOrSchemaOrDescriptionSyntax AlterTable() => Alter.Table( TableName );


    protected virtual ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax? table = Create.Table( TableName );

        table.WithColumn( nameof(ITableRecord<TRecord>.ID) ).AsGuid().PrimaryKey();
        table.WithColumn( nameof(ITableRecord<TRecord>.DateCreated) ).AsDateTimeOffset().NotNullable().WithDefaultValue( SystemMethods.CurrentUTCDateTime );
        table.WithColumn( nameof(ITableRecord<TRecord>.LastModified) ).AsDateTimeOffset().Nullable();

        return table;
    }


    protected IInsertDataSyntax StartInsert() => Insert.IntoTable( TableName ).WithIdentityInsert();


    /// <param name="dataAsAnonymousType"> The columns and values to be used set </param>
    protected IUpdateWhereSyntax UpdateTable( object dataAsAnonymousType ) => Update.Table( TableName ).Set( dataAsAnonymousType );


    protected void DeleteTable()              => Delete.Table( TableName );
    protected void RenameTable( string name ) => Rename.Table( TableName ).To( name );


    protected ICreateConstraintOptionsSyntax UniqueConstraint( string          columnName )              => Create.UniqueConstraint().OnTable( TableName ).Column( columnName );
    protected ICreateConstraintOptionsSyntax UniqueConstraint( string          name, string columnName ) => Create.UniqueConstraint( name ).OnTable( TableName ).Column( columnName );
    protected ICreateConstraintOptionsSyntax UniqueConstraint( params string[] columnNames )                       => Create.UniqueConstraint().OnTable( TableName ).Columns( columnNames );
    protected ICreateConstraintOptionsSyntax UniqueConstraint( string          name, params string[] columnNames ) => Create.UniqueConstraint( name ).OnTable( TableName ).Columns( columnNames );
}



public abstract class OwnedMigration<TRecord> : Migration<TRecord>
    where TRecord : IOwnedTableRecord, ITableRecord<TRecord>, IDbReaderMapping<TRecord>
{
    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(IOwnedTableRecord.OwnerUserID) ).AsGuid().Nullable();
        table.WithColumn( nameof(IOwnedTableRecord.CreatedBy) ).AsGuid().Nullable();

        return table;
    }
}
