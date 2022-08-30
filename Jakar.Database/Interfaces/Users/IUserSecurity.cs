namespace Jakar.Database;


/// <summary>
/// <see cref="UserLoginInfo"/>
/// </summary>
public interface IUserSecurity
{
    public DateTimeOffset? TokenExpiration        { get; }
    public bool            IsTwoFactorEnabled     { get; }
    public bool            IsEmailConfirmed       { get; }
    public bool            IsPhoneNumberConfirmed { get; }
    public string?         LoginProvider          { get; }
    public string?         ProviderKey            { get; }
    public string?         ProviderDisplayName    { get; }


    /// <summary> A random value that must change whenever a users credentials change (password changed, login removed) </summary>
    public string? SecurityStamp { get; }


    /// <summary> A random value that must change whenever a user is persisted to the store </summary>
    public string? ConcurrencyStamp { get; }


    public void ClearRefreshToken();
    public void SetRefreshToken( string token, DateTimeOffset date );
}
