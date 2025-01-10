// Jakar.AppLogger :: Jakar.Database
// 10/26/2022  11:35 AM

namespace Jakar.Database.DbMigrations;


[Migration( CREATE_USER_GROUP )]

// ReSharper disable once InconsistentNaming
public sealed class Create_UserGroup : Migrate_UserGroups
{
    public Create_UserGroup() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
