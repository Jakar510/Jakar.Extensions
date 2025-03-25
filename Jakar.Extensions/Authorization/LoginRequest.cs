namespace Jakar.Extensions;


public interface ILoginRequestProvider
{
    public LoginRequest GetLoginRequest();
    public LoginRequest<TValue> GetLoginRequest<TValue>(TValue value);
}



[Serializable]
public class LoginRequest( string userName, string password ) : BaseClass, ILoginRequest, ICredentials, JsonModels.IJsonModel
{
    [JsonExtensionData] public         IDictionary<string, JToken?>? AdditionalData { get; set; }
    [JsonIgnore]        public virtual bool                          IsValid        => this.IsValidRequest();
    [Required]          public         string                        Password       { get; init; } = password;
    [Required]          public         string                        UserName       { get; init; } = userName;


    public LoginRequest( ILoginRequest          request ) : this( request.UserName, request.Password ) { }
    public NetworkCredential GetCredential( Uri uri, string authType ) => this.GetNetworkCredentials();
}



[Serializable]
public class LoginRequest<TValue>( string userName, string password, TValue data ) : LoginRequest( userName, password ), ILoginRequest<TValue>
{
    [Required] public TValue Data { get; init; } = data;

    [JsonIgnore]
    public override bool IsValid => Data is IValidator validator
                                        ? this.IsValidRequest() && validator.IsValid
                                        : this.IsValidRequest();


    public LoginRequest( ILoginRequest    request, TValue data ) : this( request.UserName, request.Password, data ) { }
    public LoginRequest( ILoginRequest<TValue> request ) : this( request.UserName, request.Password, request.Data ) { }
}
