// Jakar.Extensions :: Jakar.Database
// 10/10/2022  4:17 PM

namespace Jakar.Database;


/// <summary> The SecurityToken created by JwtSecurityTokenHandler.CreateToken </summary>
[Serializable]
[SuppressMessage( "ReSharper", "InconsistentNaming" )]
[SuppressMessage( "ReSharper", "NotAccessedField.Global" )]
public readonly struct Tokens : IValidator
{
    public AppVersion Version      { get; init; } = default;
    public Guid       UserID       { get; init; } = Guid.Empty;
    public string     AccessToken  { get; init; } = string.Empty;
    public string?    RefreshToken { get; init; } = default;
    public string?    FullName     { get; init; } = default;
    public bool       IsValid      => !string.IsNullOrWhiteSpace( AccessToken );


    [JsonConstructor]
    public Tokens( string accessToken, string? refreshToken, AppVersion version, Guid userID, string? fullName )
    {
        Version      = version;
        AccessToken  = accessToken;
        RefreshToken = refreshToken;
        UserID       = userID;
        FullName     = fullName;
    }
    public static Tokens Create( string accessToken, string? refreshToken, AppVersion version, UserRecord user ) => new(accessToken, refreshToken, version, user.UserID, user.FullName);


    public bool VerifyVersion( in AppVersion version ) => Version.FuzzyEquals( version );
}
