﻿namespace Jakar.Extensions;


public interface ILoginRequestProvider
{
    public LoginRequest GetLoginRequest();
    public LoginRequest<T> GetLoginRequest<T>(T value);
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
public class LoginRequest<T>( string userName, string password, T data ) : LoginRequest( userName, password ), ILoginRequest<T>
{
    [Required] public T Data { get; init; } = data;

    [JsonIgnore]
    public override bool IsValid => Data is IValidator validator
                                        ? this.IsValidRequest() && validator.IsValid
                                        : this.IsValidRequest();


    public LoginRequest( ILoginRequest    request, T data ) : this( request.UserName, request.Password, data ) { }
    public LoginRequest( ILoginRequest<T> request ) : this( request.UserName, request.Password, request.Data ) { }
}
