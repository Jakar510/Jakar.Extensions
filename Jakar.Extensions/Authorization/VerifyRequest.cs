#nullable enable
namespace Jakar.Extensions;


[Serializable]
public record VerifyRequest : BaseRecord, ILoginRequest, ICredentials
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
    public override int GetHashCode() => HashCode.Combine( base.GetHashCode(), UserLogin, UserPassword );


    public virtual NetworkCredential GetCredential( Uri uri, string authType ) => new(UserLogin, UserPassword.ToSecureString());


    public virtual bool Equals( VerifyRequest? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return base.Equals( other ) && UserLogin == other.UserLogin && UserPassword == other.UserPassword;
    }
}



public record VerifyRequest<T> : BaseRecord, ICredentials, IValidator where T : notnull
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


    public override int GetHashCode() => HashCode.Combine( Request, Data );


    public NetworkCredential GetCredential( Uri uri, string authType ) => Request.GetCredential( uri, authType );


    public virtual bool Equals( VerifyRequest<T>? other )
    {
        if ( other is null ) { return false; }

        if ( ReferenceEquals( this, other ) ) { return true; }

        return Request.Equals( other.Request ) && EqualityComparer<T>.Default.Equals( Data, other.Data );
    }
}
