// Jakar.AppLogger :: Jakar.Database
// 10/26/2022  11:35 AM

namespace Jakar.Database.DbMigrations;


[Migration( CREATE_GROUPS )]

// ReSharper disable once InconsistentNaming
public sealed class Create_Group : Migrate_Groups
{
    public Create_Group() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
