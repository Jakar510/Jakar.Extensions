// Jakar.Extensions :: Jakar.Extensions
// 04/21/2022  11:48 AM

using Jakar.Extensions.Enumerations;



namespace Jakar.Extensions.Http;


public class ResponseData
{
    public string? Method                  { get; set; }
    public Uri?    URL                     { get; init; }
    public JToken? ErrorMessage            { get; init; }
    public Status  StatusCode              { get; init; }
    public string? StatusDescription       { get; init; }
    public string? ContentEncoding         { get; init; }
    public string? Server                  { get; init; }
    public string  ContentType             { get; init; }
    public bool    IsMutuallyAuthenticated { get; init; }


    public ResponseData( in HttpWebResponse response, in string? errorMessage )
    {
        try { ErrorMessage = errorMessage?.FromJson(); }
        catch ( Exception ) { ErrorMessage = errorMessage; }

        StatusCode              = response.StatusCode.ToStatus();
        URL                     = response.ResponseUri;
        Method                  = response.Method;
        StatusDescription       = response.StatusDescription;
        ContentType             = response.ContentType;
        ContentEncoding         = response.ContentEncoding;
        IsMutuallyAuthenticated = response.IsMutuallyAuthenticated;
        Server                  = response.Server;
    }
    public ResponseData( in WebResponse response, in string? errorMessage )
    {
        try { ErrorMessage = errorMessage?.FromJson(); }
        catch ( Exception ) { ErrorMessage = errorMessage; }

        URL                     = response.ResponseUri;
        ContentType             = response.ContentType;
        IsMutuallyAuthenticated = response.IsMutuallyAuthenticated;
    }
    public override string ToString() => this.ToPrettyJson();


    public static async Task<ResponseData> Create( WebException e )
    {
        if ( e.Response is not HttpWebResponse response ) { return await Create(e.Response); }

        using ( response ) { return await Create(response); }
    }
    public static async Task<ResponseData> Create( HttpWebResponse webResponse )
    {
        await using Stream? stream = webResponse.GetResponseStream();

        string msg;

        if ( stream is not null )
        {
            using var reader = new StreamReader(stream);

            string? errorMessage = await reader.ReadToEndAsync();
            msg = $"Error Message: {errorMessage}";
        }
        else { msg = "UNKNOWN"; }

        return new ResponseData(webResponse, msg);
    }
    public static async Task<ResponseData> Create( WebResponse webResponse )
    {
        await using Stream? stream = webResponse.GetResponseStream();

        string msg;

        if ( stream is not null )
        {
            using var reader = new StreamReader(stream);

            string? errorMessage = await reader.ReadToEndAsync();
            msg = $"Error Message: {errorMessage}";
        }
        else { msg = "UNKNOWN"; }

        return new ResponseData(webResponse, msg);
    }
}



// public class ResponseData<T> : ResponseData
// {
//     public T Payload { get; init; }
//
//
//     public ResponseData( in HttpWebResponse response, in string? errorMessage, in T payload ) : base(response, errorMessage) => Payload = payload;
//     public ResponseData( in WebResponse     response, in string? errorMessage, in T payload ) : base(response, errorMessage) => Payload = payload;
//     public override string ToString() => this.ToPrettyJson();
//
//
//     public static async Task<ResponseData<T>> Create( WebException e, CancellationToken token )
//     {
//         if ( e.Response is not HttpWebResponse response ) { return await Create(e.Response); }
//
//         using ( response ) { return await Create(response); }
//     }
//     public static async Task<ResponseData<T>> Create( HttpWebResponse webResponse, CancellationToken token )
//     {
//         await using Stream? stream = webResponse.GetResponseStream();
//
//         string msg;
//
//         if ( stream is not null )
//         {
//             using var reader = new StreamReader(stream);
//
//             string? errorMessage = await reader.ReadToEndAsync();
//             msg = $"Error Message: {errorMessage}";
//         }
//         else { msg = "UNKNOWN"; }
//
//         return new ResponseData<T>(webResponse, msg, payload);
//     }
//     public static async Task<ResponseData<T>> Create( WebResponse webResponse, CancellationToken token )
//     {
//         await using Stream? stream = webResponse.GetResponseStream();
//
//         string msg;
//
//         if ( stream is not null )
//         {
//             using var reader = new StreamReader(stream);
//
//             string? errorMessage = await reader.ReadToEndAsync();
//             msg = $"Error Message: {errorMessage}";
//         }
//         else { msg = "UNKNOWN"; }
//
//         return new ResponseData<T>(webResponse, msg, payload);
//     }
// }
