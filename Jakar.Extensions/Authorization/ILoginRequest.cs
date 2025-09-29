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



public interface ILoginRequest<out TValue> : ILoginRequest
{
    public TValue Data { get; }
}



public interface IChangePassword : ILoginRequest, INotifyPropertyChanged, INotifyPropertyChanging
{
    public     string ConfirmPassword { get; set; }
    public new string Password        { get; set; }
}



public interface IChangePassword<out TValue> : ILoginRequest<TValue>, IChangePassword;



public static class LoginRequestExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static NetworkCredential GetNetworkCredentials( this ILoginRequest   request )                                        => new(request.UserName, request.Password.ToSecureString());
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool              IsValidPassword( this       IChangePassword request )                                        => request.IsValidPassword( PasswordValidator.Default );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool              IsValidPassword( this       ILoginRequest   request )                                        => request.IsValidPassword( PasswordValidator.Default );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool              IsValidPassword( this       IChangePassword request, scoped in PasswordValidator validator ) => !string.IsNullOrWhiteSpace( request.Password ) && string.Equals( request.Password, request.ConfirmPassword, StringComparison.Ordinal ) && validator.Validate( request.Password );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool              IsValidPassword( this       ILoginRequest   request, scoped in PasswordValidator validator ) => !string.IsNullOrWhiteSpace( request.Password ) && validator.Validate( request.Password );
    [MethodImpl(MethodImplOptions.AggressiveInlining )] public static bool              IsValidUserName( this       IUserName       request ) => !string.IsNullOrWhiteSpace( request.UserName );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool              IsValidRequest( this        ILoginRequest   request ) => request.IsValidUserName() && request.IsValidPassword();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool              IsValidRequest( this        IChangePassword request ) => request.IsValidUserName() && request.IsValidPassword();
}
