// Jakar.AppLogger :: Jakar.Database
// 10/26/2022  11:35 AM

using Jakar.Database.Migrations;



namespace Jakar.Database.Tables.Migrations;


[Migration( 5 )]

// ReSharper disable once InconsistentNaming
public sealed class CreateTable_UserGroup : MigrateUserGroupRecord
{
    public CreateTable_UserGroup() : base() { }


    public override void Up()
    {
        CheckSchema();
        CreateTable();
    }
}
