// Jakar.Extensions :: Jakar.Database
// 04/11/2023  9:48 PM

namespace Jakar.Database.DbMigrations;


public sealed class CreateUserRecoveryCodeRecord : Migrate_UserRecoveryCodes
{
    public CreateUserRecoveryCodeRecord() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
