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


    public virtual NetworkCredential GetCredential( Uri uri, string authType ) => new(UserLogin, UserPassword.ToSecureString());
}



[SuppressMessage( "ReSharper", "NullableWarningSuppressionIsUsed" )]
public record VerifyRequest<T> : VerifyRequest
{
    private VerifyRequest? _request;

    public override bool IsValid => Data is IValidator validator
                                        ? base.IsValid && validator.IsValid
                                        : base.IsValid;


    [JsonProperty( nameof(Data), Required = Required.AllowNull )] public T? Data { get; init; }


    [JsonIgnore]
    [Obsolete( "Will be removed in a future version" )]
    public VerifyRequest Request
    {
        get => _request ??= this;
        init => _request = value;
    }


    public VerifyRequest() { }
    public VerifyRequest( string? userLogin, string? userPassword, T? data ) : base( userLogin, userPassword ) => Data = data;
    
    [Obsolete( "Will be removed in a future version" )] public VerifyRequest( VerifyRequest request, T? data ) : this( request.UserLogin, request.UserPassword, data ) { }
}
