// Jakar.Extensions :: Jakar.Database
// 09/17/2024  21:09

namespace Jakar.Database.DbMigrations;


public abstract class Migrate_UserAddress : Migration<UserAddressRecord, UserRecord, AddressRecord>
{
    protected Migrate_UserAddress() : base() { }
}
