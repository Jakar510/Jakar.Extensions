// Jakar.Extensions :: Jakar.Database
// 09/17/2024  21:09

namespace Jakar.Database.DbMigrations;


[Migration(CREATE_USER_ADDRESS)]

// ReSharper disable once InconsistentNaming
public sealed class Create_UserAddress : Migrate_UserAddress
{
    public Create_UserAddress() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
