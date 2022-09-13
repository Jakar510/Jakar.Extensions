// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:51 AM

using System.Net.Http;
using System.Net.Http.Headers;



namespace Jakar.Extensions;


public readonly struct WebResponse<T>
{
    public const string UNKNOWN_ERROR = "Unknown Error";


    public              T?         Payload           { get; init; } = default;
    public              string?    Method            { get; init; } = default;
    public              Uri?       URL               { get; init; } = default;
    public              JToken?    ErrorMessage      { get; init; } = default;
    public              Status     StatusCode        { get; init; } = Status.NotSet;
    public              string?    StatusDescription { get; init; } = default;
    public              string?    ContentEncoding   { get; init; } = default;
    public              string?    Server            { get; init; } = default;
    public              string?    ContentType       { get; init; } = default;
    [JsonIgnore] public Exception? Exception         { get; init; } = default;


    public WebResponse( HttpResponseMessage response, string    error ) : this(response, default, default, error) { }
    public WebResponse( HttpResponseMessage response, Exception e, in string error ) : this(response, default, e, error) { }
    public WebResponse( HttpResponseMessage response, in T      payload ) : this(response, payload, default) { }
    public WebResponse( HttpResponseMessage response, in T? payload, Exception? exception, string? error = default )
    {
        HttpRequestHeaders? requestHeaders = response.RequestMessage?.Headers;
        HttpContentHeaders? contentHeaders = response.RequestMessage?.Content?.Headers;


        ErrorMessage      = ResponseData.ParseError(error ?? exception?.Message);
        Payload           = payload;
        Exception         = exception;
        StatusCode        = response.StatusCode.ToStatus();
        StatusDescription = response.ReasonPhrase ?? StatusCode.ToStringFast();
        URL               = response.RequestMessage?.RequestUri;
        Method            = response.RequestMessage?.Method.Method;
        ContentType       = contentHeaders?.ContentType?.MediaType;
        ContentEncoding   = contentHeaders?.ContentEncoding.ToJson();
        Server            = requestHeaders?.From;
    }

    public override string ToString() => this.ToJson(Formatting.Indented);


    internal static WebResponse<T> None( HttpResponseMessage response ) => new(response, "NO RESPONSE");
    internal static WebResponse<T> None( HttpResponseMessage response, Exception e ) => new(response, e, "NO RESPONSE");


    public static async Task<WebResponse<T>> Create( WebHandler handler, Func<HttpResponseMessage, Task<T>> func )
    {
        using ( handler )
        {
            using HttpResponseMessage response = await handler;

            try
            {
                if ( !response.IsSuccessStatusCode ) { return await Create(handler, response); }

                T result = await func(response);
                return new WebResponse<T>(response, result);
            }
            catch ( HttpRequestException e ) { return await Create(handler, response, e); }
        }
    }
    public static async Task<WebResponse<T>> Create<TArg>( WebHandler handler, TArg arg, Func<HttpResponseMessage, TArg, Task<T>> func )
    {
        using ( handler )
        {
            using HttpResponseMessage response = await handler;

            try
            {
                if ( !response.IsSuccessStatusCode ) { return await Create(handler, response); }

                T result = await func(response, arg);
                return new WebResponse<T>(response, result);
            }
            catch ( HttpRequestException e ) { return await Create(handler, response, e); }
        }
    }
    public static async Task<WebResponse<T>> Create( WebHandler handler, HttpResponseMessage response )
    {
    #if NETSTANDARD2_1
        await using Stream? stream = await response.Content.ReadAsStreamAsync();
    #else
        await using Stream? stream = await response.Content.ReadAsStreamAsync(handler.Token);
    #endif

        string error;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is null ) { error = UNKNOWN_ERROR; }
        else
        {
            using var reader       = new StreamReader(stream);
            string    errorMessage = await reader.ReadToEndAsync();

            if ( string.IsNullOrWhiteSpace(errorMessage) ) { return None(response); }

            error = $"Error Message: '{errorMessage}'";
        }

        return new WebResponse<T>(response, error);
    }
    public static async Task<WebResponse<T>> Create( WebHandler handler, HttpResponseMessage response, Exception e )
    {
    #if NETSTANDARD2_1
        await using Stream? stream = await response.Content.ReadAsStreamAsync();
    #else
        await using Stream? stream = await response.Content.ReadAsStreamAsync(handler.Token);
    #endif

        string error;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is null ) { error = UNKNOWN_ERROR; }
        else
        {
            using var reader       = new StreamReader(stream);
            string    errorMessage = await reader.ReadToEndAsync();

            if ( string.IsNullOrWhiteSpace(errorMessage) ) { return None(response, e); }

            error = $"Error Message: '{errorMessage}'";
        }

        return new WebResponse<T>(response, e, error);
    }
}
