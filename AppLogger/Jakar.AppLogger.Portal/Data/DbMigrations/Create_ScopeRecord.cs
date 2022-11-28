using FluentMigrator;
using FluentMigrator.Builders.Create.Table;



namespace Jakar.AppLogger.Portal.Data.DbMigrations;


[Migration( 107 )]
// ReSharper disable once InconsistentNaming
public sealed class Create_ScopeRecord : LoggerMigration<ScopeRecord>
{
    public Create_ScopeRecord() : base() { }


    public override void Down() => DeleteTable();
    public override void Up()
    {
        CheckSchema();
        ICreateTableWithColumnSyntax table = CreateTable();
        

        table.WithColumn( nameof(ScopeRecord.ScopeID) )
             .AsGuid()
             .NotNullable();

        table.WithColumn( nameof(ScopeRecord.SessionID) )
             .AsGuid()
             .NotNullable();

        table.WithColumn( nameof(ScopeRecord.AppID) )
             .AsInt64()
             .NotNullable();

        table.WithColumn( nameof(ScopeRecord.DeviceID) )
             .AsInt64()
             .NotNullable();
    }
}