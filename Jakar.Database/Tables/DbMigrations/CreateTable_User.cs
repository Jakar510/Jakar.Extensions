namespace Jakar.Database.DbMigrations;


[Migration( 1 )]

// ReSharper disable once InconsistentNaming
public sealed class CreateTable_User : MigrateUserTable
{
    public CreateTable_User() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
