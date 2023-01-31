namespace Jakar.Database;


/// <summary>
///     <see cref="UserLoginInfo"/>
/// </summary>
public interface IUserSecurity
{
    public bool            IsEmailConfirmed       { get; }
    public bool            IsPhoneNumberConfirmed { get; }
    public bool            IsTwoFactorEnabled     { get; }
    public DateTimeOffset? TokenExpiration        { get; }


    /// <summary> A random value that must change whenever a user is persisted to the store </summary>
    public string? ConcurrencyStamp { get; }


    /// <summary> A random value that must change whenever a users credentials change (password changed, login removed) </summary>
    public string? SecurityStamp { get; }


    public UserRecord ClearRefreshToken();
    public UserRecord SetRefreshToken( string token, DateTimeOffset date );
}
