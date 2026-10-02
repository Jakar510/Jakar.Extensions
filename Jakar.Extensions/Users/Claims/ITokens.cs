// Jakar.Extensions :: Jakar.Database
// 10/10/2022  4:17 PM

namespace Jakar.Extensions;


public interface ITokens : IValidator
{
    string     AccessToken  { get; set; }
    string?    FullName     { get; set; }
    string?    RefreshToken { get; set; }
    Guid       UserID       { get; set; }
    AppVersion Version      { get; set; }
}
