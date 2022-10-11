#nullable enable
namespace Jakar.Extensions;


[Serializable]
public class VerifyRequest : BaseClass, ILoginRequest, ICredentials, IEquatable<VerifyRequest>
{
    public virtual bool IsValid => !string.IsNullOrWhiteSpace( UserLogin ) && !string.IsNullOrWhiteSpace( UserPassword );

    [Required( ErrorMessage = "Email address is required." )]
    [JsonProperty( nameof(UserLogin), Required = Required.Always )]
    public string UserLogin { get; init; } = string.Empty;


    [Required( ErrorMessage = "Password is required." )]
    [JsonProperty( nameof(UserPassword), Required = Required.Always )]
    public string UserPassword { get; init; } = string.Empty;

    public VerifyRequest() { }
    public VerifyRequest( string? userName, string? userPassword )
    {
        UserLogin    = userName ?? throw new ArgumentNullException( nameof(userName) );
        UserPassword = userPassword ?? throw new ArgumentNullException( nameof(userPassword) );
    }


    public static bool operator ==( VerifyRequest? left, VerifyRequest? right ) => Equals( left, right );
    public static bool operator !=( VerifyRequest? left, VerifyRequest? right ) => !Equals( left, right );
    public override bool Equals( object?           obj ) => obj is VerifyRequest request && Equals( request );
    public override int GetHashCode() => HashCode.Combine( UserLogin, UserPassword );


    public virtual NetworkCredential GetCredential( Uri uri, string authType ) => new(UserLogin, UserPassword.ToSecureString());


    public virtual bool Equals( VerifyRequest? other )
    {
        if (other is null) { return false; }

        if (ReferenceEquals( this, other )) { return true; }

        return UserLogin == other.UserLogin && UserPassword == other.UserPassword;
    }
}



public class VerifyRequest<T> : BaseClass, ICredentials, IValidator, IEquatable<VerifyRequest<T>> where T : notnull
{
    public bool IsValid => Data is IValidator validator
                               ? Request.IsValid && validator.IsValid
                               : Request.IsValid;

    public T             Data    { get; init; } = default!;
    public VerifyRequest Request { get; init; } = new();


    public VerifyRequest() { }
    public VerifyRequest( VerifyRequest request, T data )
    {
        Request = request;
        Data    = data;
    }


    public static bool operator ==( VerifyRequest<T>? left, VerifyRequest<T>? right ) => Equals( left, right );
    public static bool operator !=( VerifyRequest<T>? left, VerifyRequest<T>? right ) => !Equals( left, right );
    public override bool Equals( object? other )
    {
        if (other is null) { return false; }

        if (ReferenceEquals( this, other )) { return true; }

        return other is VerifyRequest<T> request && Equals( request );
    }
    public override int GetHashCode() => HashCode.Combine( Request, Data );


    public NetworkCredential GetCredential( Uri uri, string authType ) => Request.GetCredential( uri, authType );


    public bool Equals( VerifyRequest<T>? other )
    {
        if (other is null) { return false; }

        if (ReferenceEquals( this, other )) { return true; }

        return Request.Equals( other.Request ) && EqualityComparer<T>.Default.Equals( Data, other.Data );
    }
}
