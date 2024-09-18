// Jakar.Extensions :: Jakar.Database
// 04/11/2023  9:44 PM

namespace Jakar.Database.DbMigrations;


public abstract class Migrate_UserRecoveryCodes : Migration<UserRecoveryCodeRecord, UserRecord, RecoveryCodeRecord>
{
    protected Migrate_UserRecoveryCodes() : base() { }
}