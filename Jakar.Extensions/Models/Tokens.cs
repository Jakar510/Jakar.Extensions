// Jakar.Extensions :: Jakar.Database
// 10/10/2022  4:17 PM

namespace Jakar.Extensions;


/// <summary> The SecurityToken created by JwtSecurityTokenHandler.CreateToken </summary>
[Serializable]
public record Tokens( Guid UserID, string? FullName, AppVersion Version, string AccessToken, string? RefreshToken ) : BaseRecord, IValidator
{
    public                      string AccessToken { get; set; } = AccessToken;
    [JsonIgnore] public virtual bool   IsValid     => !string.IsNullOrWhiteSpace( AccessToken );


    public bool VerifyVersion( AppVersion version ) => Version.FuzzyEquals( version );
}



/// <summary> The SecurityToken created by JwtSecurityTokenHandler.CreateToken </summary>
[Serializable]
public record Tokens<TID>( Guid UserID, string? FullName, AppVersion Version, string AccessToken, string? RefreshToken, Guid DeviceID, TID SessionID ) : BaseRecord, IValidator, ISessionID<TID>
    where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable
{
    public                      string AccessToken { get; set; } = AccessToken;
    [JsonIgnore] public virtual bool   IsValid     => !string.IsNullOrWhiteSpace( AccessToken );


    public bool VerifyVersion( AppVersion version ) => Version.FuzzyEquals( version );
}
