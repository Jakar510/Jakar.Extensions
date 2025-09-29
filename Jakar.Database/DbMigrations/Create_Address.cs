// Jakar.Extensions :: Jakar.Database
// 09/17/2024  23:09

namespace Jakar.Database.DbMigrations;


[Migration(CREATE_ADDRESS)]

// ReSharper disable once InconsistentNaming
public sealed class Create_Address : Migrate_Address
{
    public Create_Address() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
