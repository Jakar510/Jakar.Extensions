namespace Jakar.Database;


public interface IUserSecurity
{
    public DateTimeOffset? TokenExpiration    { get; }
    public bool            IsTwoFactorEnabled { get; }


    /// <summary> A random value that must change whenever a users credentials change (password changed, login removed) </summary>
    public string? SecurityStamp { get; }


    /// <summary> A random value that must change whenever a user is persisted to the store </summary>
    public string? ConcurrencyStamp { get; }


    public void ClearRefreshToken();
    public void SetRefreshToken(string token, DateTimeOffset date);
}