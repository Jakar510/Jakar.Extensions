// Jakar.AppLogger :: Jakar.Database
// 10/26/2022  11:35 AM

namespace Jakar.Database.DbMigrations;


[Migration( USER_GROUP )]

// ReSharper disable once InconsistentNaming
public sealed class CreateTable_UserGroup : Migrate_UserGroups
{
    public CreateTable_UserGroup() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
