// Jakar.AppLogger :: Jakar.Database
// 10/26/2022  11:03 AM

namespace Jakar.Database.Migrations;


public abstract class MigrateGroupTable : Migration<GroupRecord>
{
    protected MigrateGroupTable() : base() { }


    public override void Down() => DeleteTable();


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        var table = base.CreateTable();

        table.WithColumn( nameof( GroupRecord.NameOfGroup ) )
             .AsString( 1024 )
             .NotNullable();

        table.WithColumn( nameof( GroupRecord.CustomerID ) )
             .AsString( 1024 )
             .NotNullable();

        table.WithColumn( nameof( GroupRecord.OwnerID ) )
             .AsString( 4096 )
             .NotNullable();

        return table;
    }
}
