// Jakar.Extensions :: Jakar.Extensions
// 08/25/2022  8:32 AM

namespace Jakar.Extensions;


public interface IUserName
{
    public string UserName { get; }
}



public interface ILoginRequest : IValidator, IUserName
{
    public string Password { get; }
}



public interface IChangePassword : ILoginRequest
{
    public string ConfirmPassword { get; }
}
