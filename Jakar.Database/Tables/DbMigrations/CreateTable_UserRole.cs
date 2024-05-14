// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 10/26/2022  11:26 AM

namespace Jakar.Database.DbMigrations;


[Migration( USER_ROLE )]

// ReSharper disable once InconsistentNaming
public sealed class CreateTable_UserRole : Migrate_UserRoles
{
    public CreateTable_UserRole() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
