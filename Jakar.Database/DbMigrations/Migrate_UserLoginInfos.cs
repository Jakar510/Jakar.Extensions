// Jakar.Extensions :: Jakar.Database
// 01/30/2023  9:42 PM

namespace Jakar.Database.DbMigrations;


public abstract class Migrate_UserLoginInfos : OwnedMigration<UserLoginProviderRecord>
{
    protected Migrate_UserLoginInfos() : base() { }


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn(nameof(UserLoginProviderRecord.LoginProvider)).AsString(int.MaxValue).NotNullable();

        table.WithColumn(nameof(UserLoginProviderRecord.ProviderKey)).AsString(int.MaxValue).NotNullable();

        table.WithColumn(nameof(UserLoginProviderRecord.ProviderDisplayName)).AsString(int.MaxValue).Nullable();

        table.WithColumn(nameof(UserLoginProviderRecord.Value)).AsString(int.MaxValue).Nullable();

        return table;
    }
}
