// Jakar.Extensions :: Jakar.Database
// 04/11/2023  9:44 PM

namespace Jakar.Database.DbMigrations;


public abstract class Migrate_UserRecoveryCodes : Migration<UserRecoveryCodeRecord>
{
    protected Migrate_UserRecoveryCodes() : base() { }
    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(UserRecoveryCodeRecord.KeyID) ).AsGuid().NotNullable().ForeignKey( $"{nameof(UserRecoveryCodeRecord.KeyID)}.{nameof(UserRecord.ID)}", UserRecord.TableName, nameof(UserRecord.ID) );

        table.WithColumn( nameof(UserRecoveryCodeRecord.ValueID) ).AsGuid().NotNullable().ForeignKey( $"{nameof(UserRecoveryCodeRecord.ValueID)}.{nameof(RecoveryCodeRecord.ID)}", RecoveryCodeRecord.TableName, nameof(RecoveryCodeRecord.ID) );

        return table;
    }
}
