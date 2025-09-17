// Jakar.Extensions :: Jakar.Database
// 09/29/2023  10:41 PM

namespace Jakar.Database.DbMigrations;


public abstract class Migrate_Address : OwnedMigration<AddressRecord>
{
    protected Migrate_Address() : base() { }

    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn(nameof(AddressRecord.Line1)).AsString(512).NotNullable();

        table.WithColumn(nameof(AddressRecord.Line2)).AsString(256).NotNullable();

        table.WithColumn(nameof(AddressRecord.City)).AsString(256).NotNullable();

        table.WithColumn(nameof(AddressRecord.StateOrProvince)).AsString(256).NotNullable();

        table.WithColumn(nameof(AddressRecord.Country)).AsString(256).NotNullable();

        table.WithColumn(nameof(AddressRecord.PostalCode)).AsString(256).NotNullable();

        table.WithColumn(nameof(AddressRecord.Address)).AsString(4096).NotNullable();

        return table;
    }
}
