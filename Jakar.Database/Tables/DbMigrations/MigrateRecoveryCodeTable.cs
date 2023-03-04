// Jakar.Extensions :: Jakar.Database
// 01/29/2023  1:49 PM

namespace Jakar.Database.DbMigrations;


public abstract class MigrateRecoveryCodeTable : Migration<RecoveryCodeRecord>
{
    protected MigrateRecoveryCodeTable() : base() { }


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(RecoveryCodeRecord.Code) )
             .AsString( 1024 )
             .NotNullable();

        return table;
    }
}
