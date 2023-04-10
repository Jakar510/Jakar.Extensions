// Jakar.Extensions :: Jakar.Database
// 01/30/2023  9:44 PM

namespace Jakar.Database.DbMigrations;


[Migration( 7 )]

// ReSharper disable once InconsistentNaming
public sealed class CreateTable_UserLoginInfo : MigrateUserLoginInfoTable
{
    public CreateTable_UserLoginInfo() : base() { }
    public override void Up()
    {
        CheckSchema();
        CreateTable();
    }
    public override void Down() => DeleteTable();
}
