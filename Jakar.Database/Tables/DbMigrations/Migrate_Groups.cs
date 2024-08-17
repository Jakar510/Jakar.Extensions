// Jakar.AppLogger :: Jakar.Database
// 10/26/2022  11:03 AM

namespace Jakar.Database.DbMigrations;


public abstract class Migrate_Groups : OwnedMigration<GroupRecord>
{
    protected Migrate_Groups() : base() { }


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(GroupRecord.NameOfGroup) ).AsString( 1024 ).NotNullable();

        table.WithColumn( nameof(GroupRecord.CustomerID) ).AsString( 256 ).Nullable();

        return table;
    }
}
