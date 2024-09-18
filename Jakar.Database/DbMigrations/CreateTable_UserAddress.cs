// Jakar.Extensions :: Jakar.Database
// 09/17/2024  21:09

namespace Jakar.Database.DbMigrations;


[Migration( USER_ADDRESS )]

// ReSharper disable once InconsistentNaming
public sealed class CreateTable_UserAddress : Migrate_UserAddress
{
    public CreateTable_UserAddress() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
