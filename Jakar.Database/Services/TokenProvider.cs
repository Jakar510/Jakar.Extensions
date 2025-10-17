// Jakar.Database ::  Jakar.Database 
// 04/11/2023  11:56 PM

namespace Jakar.Database;


public class TokenProvider( Database database ) : IUserTwoFactorTokenProvider<UserRecord>
{
    protected readonly Database _database = database;


    public virtual Task<string> GenerateAsync( string purpose, UserManager<UserRecord> manager, UserRecord user ) =>
        _database.GenerateAsync(purpose, manager, user);


    public virtual Task<bool> ValidateAsync( string purpose, string token, UserManager<UserRecord> manager, UserRecord user ) =>
        _database.ValidateAsync(purpose, token, manager, user);


    public virtual Task<bool> CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord user ) =>
        _database.CanGenerateTwoFactorTokenAsync(manager, user);
}



public class DataProtectorTokenProvider( IDataProtectionProvider dataProtectionProvider, IOptions<DataProtectionTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<UserRecord>> logger ) : DataProtectorTokenProvider<UserRecord>(dataProtectionProvider, options, logger)
{
    public override Task<bool> CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord user ) => base.CanGenerateTwoFactorTokenAsync(manager, user);

    public override Task<string> GenerateAsync( string purpose, UserManager<UserRecord> manager, UserRecord user ) => base.GenerateAsync(purpose, manager, user);

    public override Task<bool> ValidateAsync( string purpose, string token, UserManager<UserRecord> manager, UserRecord user ) => base.ValidateAsync(purpose, token, manager, user);
}



public class PhoneNumberTokenProvider : PhoneNumberTokenProvider<UserRecord>
{
    public override Task<bool> CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord user ) => base.CanGenerateTwoFactorTokenAsync(manager, user);

    public override Task<string> GenerateAsync( string purpose, UserManager<UserRecord> manager, UserRecord user ) => base.GenerateAsync(purpose, manager, user);

    public override Task<bool> ValidateAsync( string purpose, string token, UserManager<UserRecord> manager, UserRecord user ) => base.ValidateAsync(purpose, token, manager, user);

    public override Task<string> GetUserModifierAsync( string purpose, UserManager<UserRecord> manager, UserRecord user ) => base.GetUserModifierAsync(purpose, manager, user);
}



public class EmailTokenProvider : EmailTokenProvider<UserRecord>
{
    public override Task<bool> CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord user ) => base.CanGenerateTwoFactorTokenAsync(manager, user);

    public override Task<string> GenerateAsync( string purpose, UserManager<UserRecord> manager, UserRecord user ) => base.GenerateAsync(purpose, manager, user);

    public override Task<bool> ValidateAsync( string purpose, string token, UserManager<UserRecord> manager, UserRecord user ) => base.ValidateAsync(purpose, token, manager, user);

    public override Task<string> GetUserModifierAsync( string purpose, UserManager<UserRecord> manager, UserRecord user ) => base.GetUserModifierAsync(purpose, manager, user);
}



public class OtpAuthenticatorTokenProvider : AuthenticatorTokenProvider<UserRecord>
{
    public readonly OneTimePassword OTP;
    public readonly string          SecretKey;


    public OtpAuthenticatorTokenProvider( TelemetrySource source )
    {
        AppInformation app = source.Info;
        SecretKey = app.AppID.ToString();
        OTP       = OneTimePassword.Create(SecretKey, app.AppName);
    }


    public override Task<bool>   CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord              user )                                                      => Task.FromResult(!string.IsNullOrWhiteSpace(SecretKey));
    public override Task<string> GenerateAsync( string                                   purpose, UserManager<UserRecord> manager, UserRecord              user )                     => Task.FromResult(OTP.GetContent(user));
    public override Task<bool>   ValidateAsync( string                                   purpose, string                  token,   UserManager<UserRecord> manager, UserRecord user ) => Task.FromResult(OTP.ValidateToken(token));
}
