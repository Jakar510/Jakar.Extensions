namespace Jakar.Database.DbMigrations;


[Migration( CREATE_USERS )]

// ReSharper disable once InconsistentNaming
public sealed class Create_User : Migrate_Users
{
    public Create_User() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
