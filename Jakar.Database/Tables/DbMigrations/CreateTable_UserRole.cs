// Jakar.AppLogger :: Jakar.AppLogger.Portal
// 10/26/2022  11:26 AM

namespace Jakar.Database.DbMigrations;


[Migration( 3 )]

// ReSharper disable once InconsistentNaming
public sealed class CreateTable_UserRole : MigrateUserRoleTable
{
    public CreateTable_UserRole() : base() { }
    public override void Up()
    {
        CheckSchema();
        CreateTable();
    }
    public override void Down() => DeleteTable();
}
