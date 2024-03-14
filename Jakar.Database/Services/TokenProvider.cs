// Jakar.Database ::  Jakar.Database 
// 04/11/2023  11:56 PM

namespace Jakar.Database;


public sealed class TokenProvider( Database database ) : IUserTwoFactorTokenProvider<UserRecord>
{
    private readonly IUserTwoFactorTokenProvider<UserRecord> _database = database;


    public Task<string> GenerateAsync( string purpose, UserManager<UserRecord> manager, UserRecord user ) =>
        _database.GenerateAsync( purpose, manager, user );


    public Task<bool> ValidateAsync( string purpose, string token, UserManager<UserRecord> manager, UserRecord user ) =>
        _database.ValidateAsync( purpose, token, manager, user );


    public Task<bool> CanGenerateTwoFactorTokenAsync( UserManager<UserRecord> manager, UserRecord user ) =>
        _database.CanGenerateTwoFactorTokenAsync( manager, user );
}
