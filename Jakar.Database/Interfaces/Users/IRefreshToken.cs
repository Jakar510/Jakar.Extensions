// Jakar.Extensions :: Jakar.Database
// 08/18/2022  11:00 PM

namespace Jakar.Database;


public interface IRefreshToken
{
    public DateTimeOffset? RefreshTokenExpiryTime { get; }
    public string?         RefreshToken           { get; }
}
