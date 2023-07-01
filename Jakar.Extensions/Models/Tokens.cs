// Jakar.Extensions :: Jakar.Database
// 10/10/2022  4:17 PM

namespace Jakar.Extensions;


/// <summary> The SecurityToken created by JwtSecurityTokenHandler.CreateToken </summary>
[Serializable]
public record Tokens( Guid UserID, string? FullName, AppVersion Version, string AccessToken, string? RefreshToken ) : BaseRecord, IValidator
{
    [JsonIgnore] public virtual bool IsValid => !string.IsNullOrWhiteSpace( AccessToken );


    public bool VerifyVersion( AppVersion version ) => Version.FuzzyEquals( version );
}
