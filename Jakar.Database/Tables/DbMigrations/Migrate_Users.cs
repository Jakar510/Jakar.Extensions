// Jakar.Extensions :: Jakar.Database.FluentMigrations
// 09/30/2022  9:55 PM

namespace Jakar.Database.DbMigrations;


public abstract class Migrate_Users : OwnedMigration<UserRecord>
{
    protected Migrate_Users() : base() { }


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(UserRecord.UserName) ).AsString( SQL.UNICODE_CAPACITY ).NotNullable();
        table.WithColumn( nameof(UserRecord.FirstName) ).AsString( 2000 ).NotNullable();
        table.WithColumn( nameof(UserRecord.LastName) ).AsString( 2000 ).NotNullable();
        table.WithColumn( nameof(UserRecord.FullName) ).AsString( SQL.UNICODE_CAPACITY ).NotNullable();
        table.WithColumn( nameof(UserRecord.Gender) ).AsString( SQL.UNICODE_CAPACITY ).Nullable();
        table.WithColumn( nameof(UserRecord.Description) ).AsString( SQL.UNICODE_CAPACITY ).NotNullable();
        table.WithColumn( nameof(UserRecord.Company) ).AsString( SQL.UNICODE_CAPACITY ).NotNullable();
        table.WithColumn( nameof(UserRecord.Department) ).AsString( SQL.UNICODE_CAPACITY ).NotNullable();
        table.WithColumn( nameof(UserRecord.Title) ).AsString( SQL.UNICODE_CAPACITY ).NotNullable();
        table.WithColumn( nameof(UserRecord.Website) ).AsString( SQL.UNICODE_CAPACITY ).NotNullable();
        table.WithColumn( nameof(UserRecord.PreferredLanguage) ).AsString( 512 ).NotNullable();
        table.WithColumn( nameof(UserRecord.Email) ).AsString( SQL.UNICODE_CAPACITY ).NotNullable();
        table.WithColumn( nameof(UserRecord.IsEmailConfirmed) ).AsBoolean().NotNullable();
        table.WithColumn( nameof(UserRecord.PhoneNumber) ).AsString( SQL.UNICODE_CAPACITY ).NotNullable();
        table.WithColumn( nameof(UserRecord.Ext) ).AsString( SQL.UNICODE_CAPACITY ).NotNullable();
        table.WithColumn( nameof(UserRecord.IsPhoneNumberConfirmed) ).AsBoolean().NotNullable();
        table.WithColumn( nameof(UserRecord.AuthenticatorKey) ).AsString( SQL.ANSI_CAPACITY ).Nullable();
        table.WithColumn( nameof(UserRecord.IsTwoFactorEnabled) ).AsBoolean().NotNullable();
        table.WithColumn( nameof(UserRecord.IsActive) ).AsBoolean().NotNullable();
        table.WithColumn( nameof(UserRecord.IsDisabled) ).AsBoolean().NotNullable();
        table.WithColumn( nameof(UserRecord.SubscriptionID) ).AsGuid().Nullable();
        table.WithColumn( nameof(UserRecord.SubscriptionExpires) ).AsDateTimeOffset().Nullable();
        table.WithColumn( nameof(UserRecord.LastBadAttempt) ).AsDateTimeOffset().Nullable();
        table.WithColumn( nameof(UserRecord.LastLogin) ).AsDateTimeOffset().Nullable();
        table.WithColumn( nameof(UserRecord.BadLogins) ).AsInt64().NotNullable();
        table.WithColumn( nameof(UserRecord.IsLocked) ).AsBoolean().NotNullable();
        table.WithColumn( nameof(UserRecord.LockDate) ).AsDateTimeOffset().Nullable();
        table.WithColumn( nameof(UserRecord.LockoutEnd) ).AsDateTimeOffset().Nullable();
        table.WithColumn( nameof(UserRecord.RefreshToken) ).AsString( SQL.ANSI_CAPACITY ).Nullable();
        table.WithColumn( nameof(UserRecord.RefreshTokenExpiryTime) ).AsDateTimeOffset().Nullable();
        table.WithColumn( nameof(UserRecord.SessionID) ).AsGuid().Nullable();
        table.WithColumn( nameof(UserRecord.SecurityStamp) ).AsString( SQL.ANSI_CAPACITY ).NotNullable();
        table.WithColumn( nameof(UserRecord.ConcurrencyStamp) ).AsString( SQL.ANSI_CAPACITY ).NotNullable();
        table.WithColumn( nameof(UserRecord.Rights) ).AsString( IUserRights.MAX_SIZE ).NotNullable();
        table.WithColumn( nameof(UserRecord.EscalateTo) ).AsGuid().Nullable().ForeignKey( $"{nameof(UserRecord.EscalateTo)}.{nameof(UserRecord.ID)}", UserRecord.TableName, nameof(UserRecord.ID) );
        table.WithColumn( nameof(UserRecord.AdditionalData) ).AsString( int.MaxValue ).Nullable();
        table.WithColumn( nameof(UserRecord.PasswordHash) ).AsString( UserRecord.ENCRYPTED_MAX_PASSWORD_SIZE ).NotNullable();
        table.WithColumn( nameof(UserRecord.ImageID) ).AsGuid().Nullable().ForeignKey( $"{nameof(UserRecord.ImageID)}.{nameof(FileRecord.ID)}", FileRecord.TableName, nameof(FileRecord.ID) );

        return table;
    }
}
