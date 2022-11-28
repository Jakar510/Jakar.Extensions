using FluentMigrator;
using FluentMigrator.Builders.Create.Table;



namespace Jakar.AppLogger.Portal.Data.DbMigrations;


[Migration( 103 )]
[SuppressMessage( "ReSharper", "InconsistentNaming" )]
public sealed class Create_AppRecord : LoggerMigration<AppRecord>
{
    public Create_AppRecord() : base() { }


    public override void Down() => DeleteTable();
    public override void Up()
    {
        CheckSchema();
        ICreateTableWithColumnSyntax table = CreateTable();


        table.WithColumn( nameof(AppRecord.AppName) )
             .AsString( 1024 )
             .NotNullable();

        table.WithColumn( nameof(AppRecord.Secret) )
             .AsString( 10240 )
             .NotNullable();
    }
}
