// Jakar.AppLogger :: Jakar.Database
// 10/26/2022  11:35 AM

namespace Jakar.Database.DbMigrations;


[Migration( GROUPS )]

// ReSharper disable once InconsistentNaming
public sealed class CreateTable_Group : Migrate_Groups
{
    public CreateTable_Group() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
