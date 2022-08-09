// Jakar.Extensions :: Jakar.Extensions
// 04/21/2022  11:48 AM

#nullable enable
#nullable enable
using System.Net.Http;
using System.Net.Http.Headers;



namespace Jakar.Extensions;


public readonly struct ResponseData
{
    private static readonly ResponseData _none         = new("NO RESPONSE");
    public const            string       ERROR_MESSAGE = "Error Message: ";
    public const            string       UNKNOWN_ERROR = "Unknown Error";


    public string? Method            { get; init; } = default;
    public Uri?    URL               { get; init; } = default;
    public JToken? ErrorMessage      { get; init; } = default;
    public Status? StatusCode        { get; init; } = default;
    public string? StatusDescription { get; init; } = default;
    public string? ContentEncoding   { get; init; } = default;
    public string? Server            { get; init; } = default;
    public string? ContentType       { get; init; } = default;


    public ResponseData( in string? error ) => ErrorMessage = ParseError(error);
    public ResponseData( in HttpWebResponse response, in string? error ) : this(error)
    {
        StatusCode        = response.StatusCode.ToStatus();
        URL               = response.ResponseUri;
        Method            = response.Method;
        StatusDescription = response.StatusDescription;
        ContentType       = response.ContentType;
        ContentEncoding   = response.ContentEncoding;
        Server            = response.Server;
    }
    public ResponseData( in WebResponse response, in string? error ) : this(error)
    {
        URL         = response.ResponseUri;
        ContentType = response.ContentType;
    }


    public override string ToString() => this.ToPrettyJson();


    internal static JToken? ParseError( ReadOnlySpan<char> error )
    {
        if ( error.IsNullOrWhiteSpace() ) { return default; }

        if ( error.StartsWith(ERROR_MESSAGE, StringComparison.OrdinalIgnoreCase) ) { error = error[ERROR_MESSAGE.Length..]; }

        try { return error.FromJson(); }
        catch ( Exception ) { return error.ToString(); }
    }


    public static async Task<ResponseData> Create( WebException e )
    {
        if ( e.Response is null ) { return _none; }

        return e.Response is HttpWebResponse response
                   ? await Create(response)
                   : await Create(e.Response);
    }
    public static async Task<ResponseData> Create( HttpWebResponse response )
    {
        await using Stream? stream = response.GetResponseStream();

        string msg;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is not null )
        {
            using var reader = new StreamReader(stream);

            string? errorMessage = await reader.ReadToEndAsync();
            msg = $"Error Message: {errorMessage}";
        }
        else { msg = "UNKNOWN"; }

        return new ResponseData(response, msg);
    }
    public static async Task<ResponseData> Create( WebResponse response )
    {
        await using Stream? stream = response.GetResponseStream();

        string msg;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is not null )
        {
            using var reader = new StreamReader(stream);

            string? errorMessage = await reader.ReadToEndAsync();
            msg = $"Error Message: {errorMessage}";
        }
        else { msg = "UNKNOWN"; }

        return new ResponseData(response, msg);
    }
}



public readonly struct ResponseData<T>
{
    public const string ERROR_MESSAGE = "Error Message: ";
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


    public ResponseData( HttpResponseMessage response, in string    error ) : this(response, default, default, error) { }
    public ResponseData( HttpResponseMessage response, in Exception e, in string error ) : this(response, default, e, error) { }
    public ResponseData( HttpResponseMessage response, in T         payload ) : this(response, payload, default) { }
    public ResponseData( HttpResponseMessage response, in T? payload, Exception? exception, in string? error = default )
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


    internal static ResponseData<T> None( HttpResponseMessage response ) => new(response, "NO RESPONSE");
    internal static ResponseData<T> None( HttpResponseMessage response, Exception e ) => new(response, e, "NO RESPONSE");


    public override string ToString() => this.ToJson(Formatting.Indented);


    public static async Task<ResponseData<T>> Create<TArg>( HttpRequestBuilder.Handler handler, TArg arg, Func<HttpResponseMessage, TArg, Task<T>> func )
    {
        using ( handler )
        {
            using HttpResponseMessage response = await handler;

            try
            {
                if ( !response.IsSuccessStatusCode ) { return await Create(handler, response); }

                T result = await func(response, arg);
                return new ResponseData<T>(response, result);
            }
            catch ( HttpRequestException e ) { return await Create(handler, response, e); }
        }
    }
    public static async Task<ResponseData<T>> Create( HttpRequestBuilder.Handler handler, Func<HttpResponseMessage, Task<T>> func )
    {
        using ( handler )
        {
            using HttpResponseMessage response = await handler;

            try
            {
                if ( !response.IsSuccessStatusCode ) { return await Create(handler, response); }

                T result = await func(response);
                return new ResponseData<T>(response, result);
            }
            catch ( HttpRequestException e ) { return await Create(handler, response, e); }
        }
    }
    private static async Task<ResponseData<T>> Create( HttpRequestBuilder.Handler handler, HttpResponseMessage response )
    {
    #if NET6_0
        await using Stream? stream = await response.Content.ReadAsStreamAsync(handler.token);
    #else
        await using Stream? stream = await response.Content.ReadAsStreamAsync();
    #endif

        string error;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is null ) { error = UNKNOWN_ERROR; }
        else
        {
            using var reader       = new StreamReader(stream);
            string    errorMessage = await reader.ReadToEndAsync();

            if ( string.IsNullOrWhiteSpace(errorMessage) ) { return None(response); }

            error = $"{ERROR_MESSAGE}{errorMessage}";
        }

        return new ResponseData<T>(response, error);
    }
    private static async Task<ResponseData<T>> Create( HttpRequestBuilder.Handler handler, HttpResponseMessage response, Exception e )
    {
    #if NET6_0
        await using Stream? stream = await response.Content.ReadAsStreamAsync(handler.token);
    #else
        await using Stream? stream = await response.Content.ReadAsStreamAsync();
    #endif

        string error;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is null ) { error = UNKNOWN_ERROR; }
        else
        {
            using var reader       = new StreamReader(stream);
            string    errorMessage = await reader.ReadToEndAsync();

            if ( string.IsNullOrWhiteSpace(errorMessage) ) { return None(response, e); }

            error = $"{ERROR_MESSAGE}{errorMessage}";
        }

        return new ResponseData<T>(response, e, error);
    }
}
