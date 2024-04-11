// Jakar.Extensions :: Jakar.Database
// 01/30/2023  9:42 PM

namespace Jakar.Database.DbMigrations;


public abstract class Migrate_UserLoginInfos : OwnedMigration<UserLoginInfoRecord>
{
    protected Migrate_UserLoginInfos() : base() { }


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(UserLoginInfoRecord.LoginProvider) ).AsString( int.MaxValue ).NotNullable();

        table.WithColumn( nameof(UserLoginInfoRecord.ProviderKey) ).AsString( int.MaxValue ).NotNullable();

        table.WithColumn( nameof(UserLoginInfoRecord.ProviderDisplayName) ).AsString( int.MaxValue ).Nullable();

        table.WithColumn( nameof(UserLoginInfoRecord.Value) ).AsString( int.MaxValue ).Nullable();

        return table;
    }
}
