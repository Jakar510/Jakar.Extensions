// Jakar.AppLogger :: Jakar.Database
// 10/26/2022  11:03 AM

namespace Jakar.Database.DbMigrations;


public abstract class MigrateGroupTable : OwnedMigration<GroupRecord>
{
    protected MigrateGroupTable() : base() { }


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(GroupRecord.NameOfGroup) ).AsString( 1024 ).NotNullable();

        table.WithColumn( nameof(GroupRecord.CustomerID) ).AsString( 256 ).Nullable();

        table.WithColumn( nameof(GroupRecord.OwnerUserID) ).AsGuid().Nullable();

        return table;
    }
}
