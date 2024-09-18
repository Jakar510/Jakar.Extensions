// Jakar.Extensions :: Jakar.Database
// 04/11/2023  9:48 PM

namespace Jakar.Database.DbMigrations;


[Migration( USER_RECOVERY_CODE )]
public sealed class CreateUserRecoveryCode : Migrate_UserRecoveryCodes
{
    public CreateUserRecoveryCode() : base() { }
    public override void Up()   => CreateTable();
    public override void Down() => DeleteTable();
}
