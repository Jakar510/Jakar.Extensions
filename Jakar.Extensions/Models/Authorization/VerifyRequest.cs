#nullable enable
namespace Jakar.Extensions;


[Serializable]
public class VerifyRequest : BaseClass, ILoginRequest, ICredentials, IEquatable<VerifyRequest>
{
    [Required(ErrorMessage = "Email address is required.")]
    [JsonProperty(nameof(UserLogin), Required = Required.Always)]
    public string UserLogin { get; init; } = string.Empty;


    [Required(ErrorMessage = "Password is required.")]
    [JsonProperty(nameof(UserPassword), Required = Required.Always)]
    public string UserPassword { get; init; } = string.Empty;


    public virtual bool IsValid => !string.IsNullOrWhiteSpace(UserLogin) && !string.IsNullOrWhiteSpace(UserPassword);


    public VerifyRequest() { }
    public VerifyRequest( string? userName, string? userPassword )
    {
        UserLogin    = userName ?? throw new ArgumentNullException(nameof(userName));
        UserPassword = userPassword ?? throw new ArgumentNullException(nameof(userPassword));
    }


    public virtual bool Equals( VerifyRequest? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals(this, other) ) { return true; }

        return UserLogin == other.UserLogin && UserPassword == other.UserPassword;
    }
    public override bool Equals( object? obj ) => obj is VerifyRequest request && Equals(request);
    public override int GetHashCode() => HashCode.Combine(UserLogin, UserPassword);


    public virtual NetworkCredential GetCredential( Uri uri, string authType ) => new(UserLogin, UserPassword.ToSecureString());


    public static bool operator ==( VerifyRequest? left, VerifyRequest? right ) => Equals(left, right);
    public static bool operator !=( VerifyRequest? left, VerifyRequest? right ) => !Equals(left, right);
}
