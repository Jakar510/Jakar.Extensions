namespace Jakar.Extensions.Models.Authorization;


public interface ILoginRequest : IEquatable<ILoginRequest>
{
    public string UserLogin    { get; }
    public string UserPassword { get; }
}



[Serializable]
public class VerifyRequest : ILoginRequest
{
    [Required(ErrorMessage = "Email address is required.")]
    [JsonProperty(nameof(UserLogin), Required = Required.Always)]
    public string UserLogin { get; init; } = string.Empty;


    [Required(ErrorMessage = "Password is required.")]
    [JsonProperty(nameof(UserPassword), Required = Required.Always)]
    public string UserPassword { get; init; } = string.Empty;


    [JsonConstructor]
    public VerifyRequest() { }

    public VerifyRequest( string? userName, string? userPassword )
    {
        UserLogin    = userName ?? throw new ArgumentNullException(nameof(userName));
        UserPassword = userPassword ?? throw new ArgumentNullException(nameof(userPassword));
    }


    public override bool Equals( object? obj ) => obj is ILoginRequest request && Equals(request);


    public virtual bool Equals( ILoginRequest? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return UserLogin == other.UserLogin && UserPassword == other.UserPassword;
    }


    public override int GetHashCode() => HashCode.Combine(UserLogin, UserPassword);


    public static bool operator ==( VerifyRequest? left, VerifyRequest? right ) => Equals(left, right);


    public static bool operator !=( VerifyRequest? left, VerifyRequest? right ) => !Equals(left, right);
}
