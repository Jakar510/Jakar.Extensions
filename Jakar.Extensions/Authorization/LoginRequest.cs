namespace Jakar.Extensions;


public interface IUserName
{
    public string UserName { get; }
}



public interface IUserLogin
{
    public string UserLogin { get; }
}



public interface ILoginRequest : IValidator, IUserLogin, ICredentials
{
    public string     UserPassword { get; }
    public AppVersion Version      { get; }
}



public interface ILoginRequest<out TValue> : ILoginRequest
{
    public TValue Data { get; }
}



public interface IChangePassword : ILoginRequest, INotifyPropertyChanged, INotifyPropertyChanging
{
    public     string ConfirmPassword { get; set; }
    public new string UserPassword    { get; set; }
}



public interface IChangePassword<out TValue> : ILoginRequest<TValue>, IChangePassword;



public interface ILoginRequestProvider<out TRequest>
    where TRequest : ILoginRequest, IJsonModel<TRequest>, IEqualComparable<TRequest>
{
    public TRequest GetLoginRequest();
}



public interface ILoginRequestProvider : ILoginRequestProvider<LoginRequest>;



public interface ILoginRequestProvider<out TRequest, in TValue> : ILoginRequestProvider
    where TRequest : ILoginRequest<TValue>
{
    public TRequest GetLoginRequest( TValue value );
}



public static class LoginRequestExtensions
{
    extension( IUserName self )
    {
        public bool IsValidUserName() => !string.IsNullOrWhiteSpace(self.UserName);
    }



    extension( IUserLogin self )
    {
        public bool IsValidUserName() => !string.IsNullOrWhiteSpace(self.UserLogin);
    }



    extension( ILoginRequest self )
    {
        public NetworkCredential GetNetworkCredentials()                                  => new(self.UserLogin, self.UserPassword.ToSecureString());
        public bool              IsValidPassword()                                        => self.IsValidPassword(PasswordValidator.Default);
        public bool              IsValidPassword( scoped in PasswordValidator validator ) => !string.IsNullOrWhiteSpace(self.UserPassword) && validator.Validate(self.UserPassword);
        public bool              IsValid()                                                => self.IsValidUserName()                        && self.IsValidPassword();
        public bool              IsValid( scoped in PasswordValidator validator )         => self.IsValidUserName()                        && self.IsValidPassword(in validator);
    }



    extension( IChangePassword self )
    {
        public bool IsValidRequest()                                         => self.IsValidUserName() && self.IsValidPassword();
        public bool IsValidPassword()                                        => self.IsValidPassword(PasswordValidator.Default);
        public bool IsValidPassword( scoped in PasswordValidator validator ) => !string.IsNullOrWhiteSpace(self.UserPassword) && string.Equals(self.UserPassword, self.ConfirmPassword, StringComparison.Ordinal) && validator.Validate(self.UserPassword);
    }



    extension<TValue>( ILoginRequest<TValue> self )
    {
        public bool IsValidRequest() => self.Data is IValidator validator
                                            ? self.IsValid() && validator.IsValid
                                            : self.IsValid();
    }
}



[Serializable]
[method: JsonConstructor]
public abstract class LoginRequest<TSelf>( string userLogin, string userPassword ) : BaseClass<TSelf>, ILoginRequest
    where TSelf : LoginRequest<TSelf>, IJsonModel<TSelf>, IEqualComparable<TSelf>
{
    [JsonIgnore] public virtual bool       IsValid      => this.IsValid();
    [Required]   public         string     UserPassword { get; init; } = userPassword;
    [Required]   public         string     UserLogin    { get; init; } = userLogin;
    public                      AppVersion Version      { get; init; } = AppVersion.Default;


    public virtual NetworkCredential GetCredential( Uri uri, string authType ) => this.GetNetworkCredentials();
    public override int CompareTo( TSelf? other )
    {
        if ( other is null ) { return 1; }

        if ( ReferenceEquals(this, other) ) { return 0; }

        return string.Compare(UserLogin, other.UserLogin, StringComparison.InvariantCulture);
    }
    public override bool Equals( TSelf? other ) => ReferenceEquals(this, other) || ( other is not null && string.Equals(UserLogin, other.UserLogin, StringComparison.InvariantCulture) && string.Equals(UserPassword, other.UserPassword, StringComparison.InvariantCulture) );
    public override int  GetHashCode()          => HashCode.Combine(UserLogin, UserPassword);
}



[Serializable]
[method: JsonConstructor]
public abstract class LoginRequest<TSelf, TValue>( string userName, string password, TValue data ) : LoginRequest<TSelf>(userName, password), ILoginRequest<TValue>
    where TSelf : LoginRequest<TSelf, TValue>, IJsonModel<TSelf>, IEqualComparable<TSelf>
{
    [Required]   public          TValue Data    { get; init; } = data;
    [JsonIgnore] public override bool   IsValid => this.IsValid();


    protected LoginRequest( ILoginRequest         request, TValue data ) : this(request.UserLogin, request.UserPassword, data) { }
    protected LoginRequest( ILoginRequest<TValue> request ) : this(request.UserLogin, request.UserPassword, request.Data) { }


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
    public LoginRequestVersion( ILoginRequest             request, AppVersion data ) : this(request.UserLogin, request.UserPassword, data) { }
    public LoginRequestVersion( ILoginRequest<AppVersion> request ) : this(request.UserLogin, request.UserLogin, request.Data) { }


    public override int  GetHashCode()                                                        => HashCode.Combine(UserLogin, UserPassword, Data);
    public override bool Equals( object?                   other )                            => ReferenceEquals(this, other) || ( other is LoginRequestValue x && Equals(x) );
    public static   bool operator ==( LoginRequestVersion? left, LoginRequestVersion? right ) => EqualityComparer<LoginRequestVersion>.Default.Equals(left, right);
    public static   bool operator !=( LoginRequestVersion? left, LoginRequestVersion? right ) => !EqualityComparer<LoginRequestVersion>.Default.Equals(left, right);
    public static   bool operator >( LoginRequestVersion   left, LoginRequestVersion  right ) => Comparer<LoginRequestVersion>.Default.Compare(left, right) > 0;
    public static   bool operator >=( LoginRequestVersion  left, LoginRequestVersion  right ) => Comparer<LoginRequestVersion>.Default.Compare(left, right) >= 0;
    public static   bool operator <( LoginRequestVersion   left, LoginRequestVersion  right ) => Comparer<LoginRequestVersion>.Default.Compare(left, right) < 0;
    public static   bool operator <=( LoginRequestVersion  left, LoginRequestVersion  right ) => Comparer<LoginRequestVersion>.Default.Compare(left, right) <= 0;
}



[Serializable]
[method: JsonConstructor]
public sealed class LoginRequestValue( string userName, string password, JToken data ) : LoginRequest<LoginRequestValue, JToken>(userName, password, data), IJsonModel<LoginRequestValue>, IEqualComparable<LoginRequestValue>
{
    public LoginRequestValue( ILoginRequest         request, JToken data ) : this(request.UserLogin, request.UserPassword, data) { }
    public LoginRequestValue( ILoginRequest<JValue> request ) : this(request.UserLogin, request.UserLogin, request.Data) { }


    public static LoginRequestValue Create( ILoginRequest request, JValue data ) => new(request.UserLogin, request.UserPassword, data);


    public override int  GetHashCode()                                                    => HashCode.Combine(UserLogin, UserPassword, Data);
    public override bool Equals( object?                 other )                          => ReferenceEquals(this, other) || ( other is LoginRequestValue x && Equals(x) );
    public static   bool operator ==( LoginRequestValue? left, LoginRequestValue? right ) => EqualityComparer<LoginRequestValue>.Default.Equals(left, right);
    public static   bool operator !=( LoginRequestValue? left, LoginRequestValue? right ) => !EqualityComparer<LoginRequestValue>.Default.Equals(left, right);
    public static   bool operator >( LoginRequestValue   left, LoginRequestValue  right ) => Comparer<LoginRequestValue>.Default.Compare(left, right) > 0;
    public static   bool operator >=( LoginRequestValue  left, LoginRequestValue  right ) => Comparer<LoginRequestValue>.Default.Compare(left, right) >= 0;
    public static   bool operator <( LoginRequestValue   left, LoginRequestValue  right ) => Comparer<LoginRequestValue>.Default.Compare(left, right) < 0;
    public static   bool operator <=( LoginRequestValue  left, LoginRequestValue  right ) => Comparer<LoginRequestValue>.Default.Compare(left, right) <= 0;
}



[Serializable]
[method: JsonConstructor]
public sealed class LoginRequest( string userName, string password ) : LoginRequest<LoginRequest>(userName, password), IJsonModel<LoginRequest>, IEqualComparable<LoginRequest>
{
    public LoginRequest( ILoginRequest request ) : this(request.UserLogin, request.UserPassword) { }


    public override int  GetHashCode()                                          => HashCode.Combine(UserLogin, UserPassword);
    public override bool Equals( object?            other )                     => ReferenceEquals(this, other) || ( other is LoginRequest x && Equals(x) );
    public static   bool operator ==( LoginRequest? left, LoginRequest? right ) => EqualityComparer<LoginRequest>.Default.Equals(left, right);
    public static   bool operator !=( LoginRequest? left, LoginRequest? right ) => !EqualityComparer<LoginRequest>.Default.Equals(left, right);
    public static   bool operator >( LoginRequest   left, LoginRequest  right ) => Comparer<LoginRequest>.Default.Compare(left, right) > 0;
    public static   bool operator >=( LoginRequest  left, LoginRequest  right ) => Comparer<LoginRequest>.Default.Compare(left, right) >= 0;
    public static   bool operator <( LoginRequest   left, LoginRequest  right ) => Comparer<LoginRequest>.Default.Compare(left, right) < 0;
    public static   bool operator <=( LoginRequest  left, LoginRequest  right ) => Comparer<LoginRequest>.Default.Compare(left, right) <= 0;
}
