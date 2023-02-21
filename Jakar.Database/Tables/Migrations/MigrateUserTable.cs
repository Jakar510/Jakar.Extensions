// Jakar.Extensions :: Jakar.Database.FluentMigrations
// 09/30/2022  9:55 PM

namespace Jakar.Database.Migrations;


public abstract class MigrateUserTable : Migration<UserRecord>
{
    protected MigrateUserTable() : base() { }


    protected override ICreateTableWithColumnSyntax CreateTable()
    {
        ICreateTableWithColumnSyntax table = base.CreateTable();

        table.WithColumn( nameof(UserRecord.UserName) )
             .AsString( 256 )
             .NotNullable();

        table.WithColumn( nameof(UserRecord.FirstName) )
             .AsString( 256 )
             .NotNullable();

        table.WithColumn( nameof(UserRecord.LastName) )
             .AsString( 256 )
             .NotNullable();

        table.WithColumn( nameof(UserRecord.FullName) )
             .AsString( 512 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.Description) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.SessionID) )
             .AsGuid()
             .Nullable();

        table.WithColumn( nameof(UserRecord.UserID) )
             .AsGuid()
             .NotNullable();

        table.WithColumn( nameof(UserRecord.Address) )
             .AsString( 4096 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.Line1) )
             .AsString( 512 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.Line2) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.City) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.State) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.Country) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.PostalCode) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.Website) )
             .AsString( 4096 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.Email) )
             .AsString( 1024 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.PhoneNumber) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.Ext) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.Title) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.Department) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.Company) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.PreferredLanguage) )
             .AsInt32()
             .Nullable();

        table.WithColumn( nameof(UserRecord.SubscriptionExpires) )
             .AsDateTime2()
             .Nullable();

        table.WithColumn( nameof(UserRecord.SubscriptionID) )
             .AsString( 256 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.EscalateTo) )
             .AsString( 255 )
             .Nullable();

        table.WithColumn( nameof(UserRecord.IsActive) )
             .AsBoolean()
             .NotNullable();

        table.WithColumn( nameof(UserRecord.LastActive) )
             .AsDateTime2()
             .Nullable();

        table.WithColumn( nameof(UserRecord.IsDisabled) )
             .AsBoolean()
             .NotNullable();

        table.WithColumn( nameof(UserRecord.IsLocked) )
             .AsBoolean()
             .NotNullable();

        table.WithColumn( nameof(UserRecord.BadLogins) )
             .AsInt64()
             .NotNullable();

        table.WithColumn( nameof(UserRecord.LastBadAttempt) )
             .AsDateTime2()
             .Nullable();

        table.WithColumn( nameof(UserRecord.LockDate) )
             .AsDateTime2()
             .Nullable();

        table.WithColumn( nameof(UserRecord.LockoutEnd) )
             .AsDateTime2()
             .Nullable();

        table.WithColumn( nameof(UserRecord.RefreshToken) )
             .AsString( TokenValidationParameters.DefaultMaximumTokenSizeInBytes )
             .Nullable();

        table.WithColumn( nameof(UserRecord.RefreshTokenExpiryTime) )
             .AsDateTime2()
             .Nullable();

        table.WithColumn( nameof(UserRecord.TokenExpiration) )
             .AsDateTime2()
             .Nullable();

        table.WithColumn( nameof(UserRecord.IsEmailConfirmed) )
             .AsBoolean()
             .NotNullable();

        table.WithColumn( nameof(UserRecord.IsPhoneNumberConfirmed) )
             .AsBoolean()
             .NotNullable();

        table.WithColumn( nameof(UserRecord.IsTwoFactorEnabled) )
             .AsBoolean()
             .NotNullable();

        table.WithColumn( nameof(UserRecord.SecurityStamp) )
             .AsString( TokenValidationParameters.DefaultMaximumTokenSizeInBytes )
             .Nullable();

        table.WithColumn( nameof(UserRecord.ConcurrencyStamp) )
             .AsString( TokenValidationParameters.DefaultMaximumTokenSizeInBytes )
             .Nullable();

        table.WithColumn( nameof(UserRecord.AdditionalData) )
             .AsString( int.MaxValue )
             .Nullable();

        table.WithColumn( nameof(UserRecord.PasswordHash) )
             .AsString( TokenValidationParameters.DefaultMaximumTokenSizeInBytes )
             .Nullable();

        table.WithColumn( nameof(UserRecord.Rights) )
             .AsString( TokenValidationParameters.DefaultMaximumTokenSizeInBytes )
             .NotNullable();

        return table;
    }
}
