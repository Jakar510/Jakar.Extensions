// Jakar.Extensions :: Jakar.Database
// 08/18/2022  11:00 PM

namespace Jakar.Database;


public interface IRefreshToken
{
    public string? RefreshToken { get; }
    public DateTimeOffset? RefreshTokenExpiryTime { get; }
}