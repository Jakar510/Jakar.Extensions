using FluentMigrator;
using FluentMigrator.Builders.Create.Table;



namespace Jakar.AppLogger.Portal.Data.DbMigrations;


[Migration( 108 )]
// ReSharper disable once InconsistentNaming
public sealed class Create_SessionRecord : LoggerMigration<SessionRecord>
{
    public Create_SessionRecord() : base() { }


    public override void Down() => DeleteTable();
    public override void Up()
    {
        CheckSchema();
        ICreateTableWithColumnSyntax table = CreateTable();


        table.WithColumn( nameof(SessionRecord.SessionID) )
             .AsGuid()
             .NotNullable();

        table.WithColumn( nameof(SessionRecord.AppID) )
             .AsInt64()
             .NotNullable();

        table.WithColumn( nameof(SessionRecord.DeviceID) )
             .AsInt64()
             .NotNullable();
    }
}
