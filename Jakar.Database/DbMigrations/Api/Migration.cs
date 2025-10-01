// Jakar.Database ::  Jakar.Database 
// 08/02/2022  3:20 PM

using FluentMigrator.Builders.Create.Constraint;



namespace Jakar.Database.DbMigrations;


/// <summary>
///     <para>
///         <see href="https://fluentmigrator.github.io/articles/fluent-interface.html"/>
///     </para>
/// </summary>
public abstract class Migration<TClass> : Migration
    where TClass : class, ITableRecord<TClass>
{
    public string TableName { get; } = typeof(TClass).GetTableName();


    protected Migration() : base() { }


    // public override void GetUpExpressions( IMigrationContext   context ) => base.GetUpExpressions( context );   // _dbContext = context.ServiceProvider.GetRequiredService<Database>();
    // public override void GetDownExpressions( IMigrationContext context ) => base.GetDownExpressions( context ); // _dbContext = context.ServiceProvider.GetRequiredService<Database>();


    protected IAlterTableAddColumnOrAlterColumnOrSchemaOrDescriptionSyntax AlterTable() => Alter.Table(TableName);


    protected virtual ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax? table = Create.Table(TableName);

        table.WithColumn(nameof(IDateCreated.ID))
             .AsGuid()
             .PrimaryKey();

        table.WithColumn(nameof(IDateCreated.DateCreated))
             .AsDateTimeOffset()
             .NotNullable()
             .WithDefaultValue(SystemMethods.CurrentUTCDateTime);

        if ( typeof(ILastModified).IsAssignableFrom(typeof(TClass)) )
        {
            table.WithColumn(nameof(ILastModified.LastModified))
                 .AsDateTimeOffset()
                 .Nullable();
        }

        return table;
    }


    protected IInsertDataSyntax StartInsert() => Insert.IntoTable(TableName)
                                                       .WithIdentityInsert();


    /// <param name="dataAsAnonymousType"> The columns and values to be used set </param>
    protected IUpdateWhereSyntax UpdateTable( object dataAsAnonymousType ) => Update.Table(TableName)
                                                                                    .Set(dataAsAnonymousType);


    protected void DeleteTable() => Delete.Table(TableName);
    protected void RenameTable( string name ) => Rename.Table(TableName)
                                                       .To(name);


    protected ICreateConstraintOptionsSyntax UniqueConstraint( string columnName ) => Create.UniqueConstraint()
                                                                                            .OnTable(TableName)
                                                                                            .Column(columnName);
    protected ICreateConstraintOptionsSyntax UniqueConstraint( string name, string columnName ) => Create.UniqueConstraint(name)
                                                                                                         .OnTable(TableName)
                                                                                                         .Column(columnName);
    protected ICreateConstraintOptionsSyntax UniqueConstraint( params string[] columnNames ) => Create.UniqueConstraint()
                                                                                                      .OnTable(TableName)
                                                                                                      .Columns(columnNames);
    protected ICreateConstraintOptionsSyntax UniqueConstraint( string name, params string[] columnNames ) => Create.UniqueConstraint(name)
                                                                                                                   .OnTable(TableName)
                                                                                                                   .Columns(columnNames);
}



[SuppressMessage("ReSharper", "StaticMemberInGenericType")]
public abstract class Migration<TClass, TKey, TValue> : Migration<TClass>
    where TClass : Mapping<TClass, TKey, TValue>, ITableRecord<TClass>, ICreateMapping<TClass, TKey, TValue>
    where TKey : class, ITableRecord<TKey>
    where TValue : class, ITableRecord<TValue>
{
    public static readonly string KeyForeignKeyName   = $"{TClass.TableName}-{TKey.TableName}.{nameof(IDateCreated.ID)}";
    public static readonly string ValueForeignKeyName = $"{TClass.TableName}-{TValue.TableName}.{nameof(IDateCreated.ID)}";
    protected Migration() : base() { }


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn(nameof(Mapping<TClass, TKey, TValue>.KeyID))
             .AsGuid()
             .ForeignKey(KeyForeignKeyName, TKey.TableName, nameof(IDateCreated.ID))
             .NotNullable();

        table.WithColumn(nameof(Mapping<TClass, TKey, TValue>.ValueID))
             .AsGuid()
             .ForeignKey(ValueForeignKeyName, TValue.TableName, nameof(IDateCreated.ID))
             .NotNullable();

        return table;
    }
}



public abstract class OwnedMigration<TClass> : Migration<TClass>
    where TClass : class, ITableRecord<TClass>, ICreatedBy
{
    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn(nameof(ICreatedBy.CreatedBy))
             .AsGuid()
             .Nullable()
             .ForeignKey($"{nameof(UserRecord.CreatedBy)}.{nameof(UserRecord.ID)}", UserRecord.TableName, nameof(UserRecord.ID));

        return table;
    }
}
