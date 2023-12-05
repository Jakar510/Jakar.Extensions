// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 10/26/2022  10:30 AM

using FluentMigrator.Builders.Create.Table;



namespace Jakar.AppLogger.Portal.Data.DbMigrations;


public abstract class LoggerMigration<TRecord> : Migration<TRecord>
    where TRecord : TableRecord<TRecord>, IDbReaderMapping<TRecord>, IMsJsonContext<TRecord>
{
    protected LoggerMigration() : base() { }

    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(AppRecord.IsActive) ).AsBoolean().Nullable();

        table.WithColumn( nameof(AppRecord.AdditionalData) ).AsString( BaseRecord.MAX_STRING_SIZE ).Nullable();

        return table;
    }
}
