using Jakar.Database.Migrations;



namespace Jakar.Database.Tables.Migrations;


[Migration( 2 )]
// ReSharper disable once InconsistentNaming
public sealed class CreateTable_Role : MigrateRoleTable
{
    public CreateTable_Role() : base() { }


    public override void Down() => DeleteTable();
    public override void Up()
    {
        CheckSchema();
        CreateTable();
    }
}