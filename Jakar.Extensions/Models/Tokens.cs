// Jakar.Extensions :: Jakar.Database
// 10/10/2022  4:17 PM

namespace Jakar.Extensions;


/// <summary> The SecurityToken created by JwtSecurityTokenHandler.CreateToken </summary>
[Serializable]
public sealed record Tokens : BaseRecord, IValidator
{
    public string     AccessToken  { get; init; } = string.Empty;
    public string?    FullName     { get; init; }
    public bool       IsValid      => !string.IsNullOrWhiteSpace( AccessToken );
    public string?    RefreshToken { get; init; }
    public Guid       UserID       { get; init; } = Guid.Empty;
    public AppVersion Version      { get; init; } = new();


    public Tokens() { }
    public Tokens( string accessToken, string? refreshToken, AppVersion version, Guid userID, string? fullName )
    {
        Version      = version;
        AccessToken  = accessToken;
        RefreshToken = refreshToken;
        UserID       = userID;
        FullName     = fullName;
    }


    public bool VerifyVersion( AppVersion version ) => Version.FuzzyEquals( version );
}
