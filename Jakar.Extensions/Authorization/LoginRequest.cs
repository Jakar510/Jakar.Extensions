namespace Jakar.Extensions;


public interface IUserName
{
    public string UserName { get; }
}



public interface ILoginRequest : IValidator, IUserName, ICredentials
{
    public string     Password { get; }
    public AppVersion Version  { get; }
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



public interface ILoginRequestProvider
{
    public LoginRequest GetLoginRequest();
}



public interface ILoginRequestProvider<out TRequest, in TValue> : ILoginRequestProvider
    where TRequest : ILoginRequest<TValue>
{
    public TRequest GetLoginRequest( TValue value );
}



public static class LoginRequestExtensions
{
    public static NetworkCredential GetNetworkCredentials( this ILoginRequest   request )                                        => new(request.UserName, request.Password.ToSecureString());
    public static bool              IsValidPassword( this       IChangePassword request )                                        => request.IsValidPassword(PasswordValidator.Default);
    public static bool              IsValidPassword( this       ILoginRequest   request )                                        => request.IsValidPassword(PasswordValidator.Default);
    public static bool              IsValidPassword( this       IChangePassword request, scoped in PasswordValidator validator ) => !string.IsNullOrWhiteSpace(request.Password) && string.Equals(request.Password, request.ConfirmPassword, StringComparison.Ordinal) && validator.Validate(request.Password);
    public static bool              IsValidPassword( this       ILoginRequest   request, scoped in PasswordValidator validator ) => !string.IsNullOrWhiteSpace(request.Password) && validator.Validate(request.Password);
    public static bool              IsValidUserName( this       IUserName       request ) => !string.IsNullOrWhiteSpace(request.UserName);
    public static bool              IsValid( this               ILoginRequest   request ) => request.IsValidUserName() && request.IsValidPassword();
    public static bool IsValidRequest<TValue>( this ILoginRequest<TValue> request ) => request.Data is IValidator validator
                                                                                           ? request.IsValidUserName() && request.IsValidPassword() && validator.IsValid
                                                                                           : request.IsValidUserName() && request.IsValidPassword();
    public static bool IsValidRequest( this IChangePassword request ) => request.IsValidUserName() && request.IsValidPassword();
}



[Serializable]
[method: JsonConstructor]
public abstract class LoginRequest<TClass>( string userName, string password ) : BaseClass<TClass>, ILoginRequest
    where TClass : LoginRequest<TClass>, IJsonModel<TClass>
{
    [Required] public           string     Password { get; init; } = password;
    [Required] public           string     UserName { get; init; } = userName;
    public                      AppVersion Version  { get; init; } = AppVersion.Default;
    [JsonIgnore] public virtual bool       IsValid  => this.IsValid();


    public virtual NetworkCredential GetCredential( Uri uri, string authType ) => this.GetNetworkCredentials();
}



[Serializable]
[method: JsonConstructor]
public abstract class LoginRequest<TClass, TValue>( string userName, string password, TValue data ) : LoginRequest<TClass>(userName, password), ILoginRequest<TValue>
    where TClass : LoginRequest<TClass, TValue>, IJsonModel<TClass>
{
    [Required]   public          TValue Data    { get; init; } = data;
    [JsonIgnore] public override bool   IsValid => this.IsValid();


    protected LoginRequest( ILoginRequest         request, TValue data ) : this(request.UserName, request.Password, data) { }
    protected LoginRequest( ILoginRequest<TValue> request ) : this(request.UserName, request.Password, request.Data) { }
}



[Serializable]
[method: JsonConstructor]
public sealed class LoginRequestVersion( string userName, string password, AppVersion data ) : LoginRequest<LoginRequestVersion, AppVersion>(userName, password, data), IJsonModel<LoginRequestVersion>
{
    public static JsonSerializerContext               JsonContext   => JakarExtensionsContext.Default;
    public static JsonTypeInfo<LoginRequestVersion>   JsonTypeInfo  => JakarExtensionsContext.Default.LoginRequestVersion;
    public static JsonTypeInfo<LoginRequestVersion[]> JsonArrayInfo => JakarExtensionsContext.Default.LoginRequestVersionArray;
    public LoginRequestVersion( ILoginRequest             request, AppVersion data ) : this(request.UserName, request.Password, data) { }
    public LoginRequestVersion( ILoginRequest<AppVersion> request ) : this(request.UserName, request.Password, request.Data) { }


    public override bool Equals( LoginRequestVersion?      other )                            => ReferenceEquals(this, other) || other is not null && string.Equals(UserName, other.UserName, StringComparison.InvariantCulture) && string.Equals(Password, other.Password, StringComparison.InvariantCulture);
    public override int  CompareTo( LoginRequestVersion?   other )                            => string.Compare(UserName, other?.UserName, StringComparison.InvariantCulture);
    public static   bool operator ==( LoginRequestVersion? left, LoginRequestVersion? right ) => EqualityComparer<LoginRequestVersion>.Default.Equals(left, right);
    public static   bool operator !=( LoginRequestVersion? left, LoginRequestVersion? right ) => !EqualityComparer<LoginRequestVersion>.Default.Equals(left, right);
    public static   bool operator >( LoginRequestVersion   left, LoginRequestVersion  right ) => Comparer<LoginRequestVersion>.Default.Compare(left, right) > 0;
    public static   bool operator >=( LoginRequestVersion  left, LoginRequestVersion  right ) => Comparer<LoginRequestVersion>.Default.Compare(left, right) >= 0;
    public static   bool operator <( LoginRequestVersion   left, LoginRequestVersion  right ) => Comparer<LoginRequestVersion>.Default.Compare(left, right) < 0;
    public static   bool operator <=( LoginRequestVersion  left, LoginRequestVersion  right ) => Comparer<LoginRequestVersion>.Default.Compare(left, right) <= 0;
}



[Serializable]
[method: JsonConstructor]
public sealed class LoginRequestValue( string userName, string password, JsonNode data ) : LoginRequest<LoginRequestValue, JsonNode>(userName, password, data), IJsonModel<LoginRequestValue>
{
    public static JsonSerializerContext             JsonContext   => JakarExtensionsContext.Default;
    public static JsonTypeInfo<LoginRequestValue>   JsonTypeInfo  => JakarExtensionsContext.Default.LoginRequestValue;
    public static JsonTypeInfo<LoginRequestValue[]> JsonArrayInfo => JakarExtensionsContext.Default.LoginRequestValueArray;
    public LoginRequestValue( ILoginRequest           request, string   data ) : this(request.UserName, request.Password, data) { }
    public LoginRequestValue( ILoginRequest           request, JsonNode data ) : this(request.UserName, request.Password, data) { }
    public LoginRequestValue( ILoginRequest<JsonNode> request ) : this(request.UserName, request.Password, request.Data) { }


    public static LoginRequestValue Create<T>( ILoginRequest request, T data )
        where T : IJsonModel<T> => new(request.UserName, request.Password, data.ToJsonNode());


    public override bool Equals( LoginRequestValue?      other )                          => ReferenceEquals(this, other) || other is not null && string.Equals(UserName, other.UserName, StringComparison.InvariantCulture) && string.Equals(Password, other.Password, StringComparison.InvariantCulture);
    public override int  CompareTo( LoginRequestValue?   other )                          => string.Compare(UserName, other?.UserName, StringComparison.InvariantCulture);
    public static   bool operator ==( LoginRequestValue? left, LoginRequestValue? right ) => EqualityComparer<LoginRequestValue>.Default.Equals(left, right);
    public static   bool operator !=( LoginRequestValue? left, LoginRequestValue? right ) => !EqualityComparer<LoginRequestValue>.Default.Equals(left, right);
    public static   bool operator >( LoginRequestValue   left, LoginRequestValue  right ) => Comparer<LoginRequestValue>.Default.Compare(left, right) > 0;
    public static   bool operator >=( LoginRequestValue  left, LoginRequestValue  right ) => Comparer<LoginRequestValue>.Default.Compare(left, right) >= 0;
    public static   bool operator <( LoginRequestValue   left, LoginRequestValue  right ) => Comparer<LoginRequestValue>.Default.Compare(left, right) < 0;
    public static   bool operator <=( LoginRequestValue  left, LoginRequestValue  right ) => Comparer<LoginRequestValue>.Default.Compare(left, right) <= 0;
}



[Serializable]
[method: JsonConstructor]
public sealed class LoginRequest( string userName, string password ) : LoginRequest<LoginRequest>(userName, password), IJsonModel<LoginRequest>
{
    public static JsonSerializerContext        JsonContext   => JakarExtensionsContext.Default;
    public static JsonTypeInfo<LoginRequest>   JsonTypeInfo  => JakarExtensionsContext.Default.LoginRequest;
    public static JsonTypeInfo<LoginRequest[]> JsonArrayInfo => JakarExtensionsContext.Default.LoginRequestArray;


    public LoginRequest( ILoginRequest            request ) : this(request.UserName, request.Password) { }
    public override bool Equals( LoginRequest?    other )                       => ReferenceEquals(this, other) || other is not null && string.Equals(UserName, other.UserName, StringComparison.InvariantCulture) && string.Equals(Password, other.Password, StringComparison.InvariantCulture);
    public override int  CompareTo( LoginRequest? other )                       => string.Compare(UserName, other?.Password, StringComparison.CurrentCultureIgnoreCase);
    public override int  GetHashCode()                                          => HashCode.Combine(UserName, Password);
    public static   bool operator ==( LoginRequest? left, LoginRequest? right ) => EqualityComparer<LoginRequest>.Default.Equals(left, right);
    public static   bool operator !=( LoginRequest? left, LoginRequest? right ) => !EqualityComparer<LoginRequest>.Default.Equals(left, right);
    public static   bool operator >( LoginRequest   left, LoginRequest  right ) => Comparer<LoginRequest>.Default.Compare(left, right) > 0;
    public static   bool operator >=( LoginRequest  left, LoginRequest  right ) => Comparer<LoginRequest>.Default.Compare(left, right) >= 0;
    public static   bool operator <( LoginRequest   left, LoginRequest  right ) => Comparer<LoginRequest>.Default.Compare(left, right) < 0;
    public static   bool operator <=( LoginRequest  left, LoginRequest  right ) => Comparer<LoginRequest>.Default.Compare(left, right) <= 0;
}
