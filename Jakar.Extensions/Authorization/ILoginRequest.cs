// Jakar.Extensions :: Jakar.Extensions
// 08/25/2022  8:32 AM

namespace Jakar.Extensions;


public interface ILoginRequest : IValidator
{
    public string UserName    { get; }
    public string Password { get; }
}
