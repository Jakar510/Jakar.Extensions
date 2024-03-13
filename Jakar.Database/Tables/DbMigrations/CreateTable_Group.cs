// Jakar.AppLogger :: Jakar.Database
// 10/26/2022  11:35 AM

namespace Jakar.Database.DbMigrations;


[Migration( 4 )]

// ReSharper disable once InconsistentNaming
public sealed class CreateTable_Group : MigrateGroupTable
{
    public CreateTable_Group() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
