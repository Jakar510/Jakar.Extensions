// Jakar.Extensions :: Jakar.Extensions
// 08/25/2022  8:32 AM

namespace Jakar.Extensions.Authorization;


public interface ILoginRequest : IValidator
{
    public string UserLogin { get; }
    public string UserPassword { get; }
}
