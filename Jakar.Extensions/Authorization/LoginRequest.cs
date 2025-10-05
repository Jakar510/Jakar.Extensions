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
public abstract class LoginRequest<TSelf>( string userName, string password ) : BaseClass<TSelf>, ILoginRequest
    where TSelf : LoginRequest<TSelf>, IJsonModel<TSelf>, IEqualComparable<TSelf>
{
    [Required] public           string     Password { get; init; } = password;
    [Required] public           string     UserName { get; init; } = userName;
    public                      AppVersion Version  { get; init; } = AppVersion.Default;
    [JsonIgnore] public virtual bool       IsValid  => this.IsValid();


    public virtual NetworkCredential GetCredential( Uri uri, string authType ) => this.GetNetworkCredentials();
    public override int CompareTo( TSelf? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return string.Compare(UserName, other.UserName, StringComparison.InvariantCulture);
    }
    public override bool Equals( TSelf? other ) => ReferenceEquals(this, other) || other is not null && string.Equals(UserName, other.UserName, StringComparison.InvariantCulture) && string.Equals(Password, other.Password, StringComparison.InvariantCulture);
    public override int  GetHashCode()          => HashCode.Combine(UserName, Password);
}



[Serializable]
[method: JsonConstructor]
public abstract class LoginRequest<TSelf, TValue>( string userName, string password, TValue data ) : LoginRequest<TSelf>(userName, password), ILoginRequest<TValue>
    where TSelf : LoginRequest<TSelf, TValue>, IJsonModel<TSelf>, IEqualComparable<TSelf>
{
    [Required]   public          TValue Data    { get; init; } = data;
    [JsonIgnore] public override bool   IsValid => this.IsValid();


    protected LoginRequest( ILoginRequest         request, TValue data ) : this(request.UserName, request.Password, data) { }
    protected LoginRequest( ILoginRequest<TValue> request ) : this(request.UserName, request.Password, request.Data) { }


    public override bool Equals( TSelf? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return base.Equals(other) && EqualityComparer<TValue>.Default.Equals(Data, other.Data);
    }
    public override bool Equals( object? obj )
    {
        if ( obj is null ) { return false; }

        return ReferenceEquals(this, obj) || Equals(obj as TSelf);
    }
    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Data);
    public override int CompareTo( TSelf? other )
    {
        if ( ReferenceEquals(this, other) ) { return 0; }

        if ( other is null ) { return 1; }

        int baseCompare = base.CompareTo(other);
        if ( baseCompare != 0 ) { return baseCompare; }

        return Comparer<TValue>.Default.Compare(Data, other.Data);
    }
}



[Serializable]
[method: JsonConstructor]
public sealed class LoginRequestVersion( string userName, string password, AppVersion data ) : LoginRequest<LoginRequestVersion, AppVersion>(userName, password, data), IJsonModel<LoginRequestVersion>, IEqualComparable<LoginRequestVersion>
{
    public static JsonSerializerContext               JsonContext   => JakarExtensionsContext.Default;
    public static JsonTypeInfo<LoginRequestVersion>   JsonTypeInfo  => JakarExtensionsContext.Default.LoginRequestVersion;
    public static JsonTypeInfo<LoginRequestVersion[]> JsonArrayInfo => JakarExtensionsContext.Default.LoginRequestVersionArray;
    public LoginRequestVersion( ILoginRequest             request, AppVersion data ) : this(request.UserName, request.Password, data) { }
    public LoginRequestVersion( ILoginRequest<AppVersion> request ) : this(request.UserName, request.UserName, request.Data) { }


    public static bool operator ==( LoginRequestVersion? left, LoginRequestVersion? right ) => EqualityComparer<LoginRequestVersion>.Default.Equals(left, right);
    public static bool operator !=( LoginRequestVersion? left, LoginRequestVersion? right ) => !EqualityComparer<LoginRequestVersion>.Default.Equals(left, right);
    public static bool operator >( LoginRequestVersion   left, LoginRequestVersion  right ) => Comparer<LoginRequestVersion>.Default.Compare(left, right) > 0;
    public static bool operator >=( LoginRequestVersion  left, LoginRequestVersion  right ) => Comparer<LoginRequestVersion>.Default.Compare(left, right) >= 0;
    public static bool operator <( LoginRequestVersion   left, LoginRequestVersion  right ) => Comparer<LoginRequestVersion>.Default.Compare(left, right) < 0;
    public static bool operator <=( LoginRequestVersion  left, LoginRequestVersion  right ) => Comparer<LoginRequestVersion>.Default.Compare(left, right) <= 0;
}



[Serializable]
[method: JsonConstructor]
public sealed class LoginRequestValue( string userName, string password, JsonValue data ) : LoginRequest<LoginRequestValue, JsonValue>(userName, password, data), IJsonModel<LoginRequestValue>, IEqualComparable<LoginRequestValue>
{
    public static JsonSerializerContext             JsonContext   => JakarExtensionsContext.Default;
    public static JsonTypeInfo<LoginRequestValue>   JsonTypeInfo  => JakarExtensionsContext.Default.LoginRequestValue;
    public static JsonTypeInfo<LoginRequestValue[]> JsonArrayInfo => JakarExtensionsContext.Default.LoginRequestValueArray;
    public LoginRequestValue( ILoginRequest            request, JsonValue data ) : this(request.UserName, request.Password, data) { }
    public LoginRequestValue( ILoginRequest<JsonValue> request ) : this(request.UserName, request.UserName, request.Data) { }


    public static LoginRequestValue Create( ILoginRequest request, JsonValue data ) => new(request.UserName, request.Password, data);


    public static bool operator ==( LoginRequestValue? left, LoginRequestValue? right ) => EqualityComparer<LoginRequestValue>.Default.Equals(left, right);
    public static bool operator !=( LoginRequestValue? left, LoginRequestValue? right ) => !EqualityComparer<LoginRequestValue>.Default.Equals(left, right);
    public static bool operator >( LoginRequestValue   left, LoginRequestValue  right ) => Comparer<LoginRequestValue>.Default.Compare(left, right) > 0;
    public static bool operator >=( LoginRequestValue  left, LoginRequestValue  right ) => Comparer<LoginRequestValue>.Default.Compare(left, right) >= 0;
    public static bool operator <( LoginRequestValue   left, LoginRequestValue  right ) => Comparer<LoginRequestValue>.Default.Compare(left, right) < 0;
    public static bool operator <=( LoginRequestValue  left, LoginRequestValue  right ) => Comparer<LoginRequestValue>.Default.Compare(left, right) <= 0;
}



[Serializable]
[method: JsonConstructor]
public sealed class LoginRequest( string userName, string password ) : LoginRequest<LoginRequest>(userName, password), IJsonModel<LoginRequest>, IEqualComparable<LoginRequest>
{
    public static JsonSerializerContext        JsonContext   => JakarExtensionsContext.Default;
    public static JsonTypeInfo<LoginRequest>   JsonTypeInfo  => JakarExtensionsContext.Default.LoginRequest;
    public static JsonTypeInfo<LoginRequest[]> JsonArrayInfo => JakarExtensionsContext.Default.LoginRequestArray;


    public LoginRequest( ILoginRequest request ) : this(request.UserName, request.Password) { }


    public static bool operator ==( LoginRequest? left, LoginRequest? right ) => EqualityComparer<LoginRequest>.Default.Equals(left, right);
    public static bool operator !=( LoginRequest? left, LoginRequest? right ) => !EqualityComparer<LoginRequest>.Default.Equals(left, right);
    public static bool operator >( LoginRequest   left, LoginRequest  right ) => Comparer<LoginRequest>.Default.Compare(left, right) > 0;
    public static bool operator >=( LoginRequest  left, LoginRequest  right ) => Comparer<LoginRequest>.Default.Compare(left, right) >= 0;
    public static bool operator <( LoginRequest   left, LoginRequest  right ) => Comparer<LoginRequest>.Default.Compare(left, right) < 0;
    public static bool operator <=( LoginRequest  left, LoginRequest  right ) => Comparer<LoginRequest>.Default.Compare(left, right) <= 0;
}
