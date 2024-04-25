namespace Jakar.Extensions;


public interface IVerifyRequestProvider
{
    public VerifyRequest GetVerifyRequest();
}



public interface IVerifyRequestProvider<T> : IVerifyRequestProvider
{
    public new VerifyRequest<T> GetVerifyRequest();
}



[Serializable]
public record VerifyRequest( [property: Required] string UserName, [property: Required] string Password ) : BaseRecord, ILoginRequest, ICredentials, JsonModels.IJsonModel
{
    [JsonExtensionData] public         IDictionary<string, JToken?>? AdditionalData { get; set; }
    [JsonIgnore]        public virtual bool                          IsValid        => this.IsValidRequest();

    public VerifyRequest( ILoginRequest         request ) : this( request.UserName, request.Password ) { }
    public NetworkCredential GetCredential( Uri uri, string authType ) => this.GetNetworkCredentials();
}



[SuppressMessage( "ReSharper", "NullableWarningSuppressionIsUsed" )]
public record VerifyRequest<T>( [property: Required] string UserName, [property: Required] string Password, [property: Required] T Data ) : BaseRecord, ILoginRequest<T>, ICredentials, JsonModels.IJsonModel
{
    [JsonExtensionData] public IDictionary<string, JToken?>? AdditionalData { get; set; }

    [JsonIgnore]
    public bool IsValid => Data is IValidator validator
                               ? this.IsValidRequest() && validator.IsValid
                               : this.IsValidRequest();

    public VerifyRequest( ILoginRequest         request, T data ) : this( request.UserName, request.Password, data ) { }
    public VerifyRequest( ILoginRequest<T>      request ) : this( request.UserName, request.Password, request.Data ) { }
    public NetworkCredential GetCredential( Uri uri, string authType ) => this.GetNetworkCredentials();
}
