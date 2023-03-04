// Jakar.AppLogger :: Jakar.Database
// 10/26/2022  11:03 AM

namespace Jakar.Database.DbMigrations;


public abstract class MigrateGroupTable : Migration<GroupRecord>
{
    protected MigrateGroupTable() : base() { }


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(GroupRecord.NameOfGroup) )
             .AsString( 1024 )
             .NotNullable();

        table.WithColumn( nameof(GroupRecord.CustomerID) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(GroupRecord.OwnerID) )
             .AsGuid()
             .NotNullable();

        return table;
    }
}
