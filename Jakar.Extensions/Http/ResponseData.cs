// Jakar.Extensions :: Jakar.Extensions
// 04/21/2022  11:48 AM

using System.Net.Http;
using Jakar.Extensions.Enumerations;
using Jakar.Extensions.SpanAndMemory;


#nullable enable
namespace Jakar.Extensions.Http;


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



#if NET6_0



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
        ErrorMessage      = ResponseData.ParseError(error ?? exception?.Message);
        Payload           = payload;
        Exception         = exception;
        StatusCode        = response.StatusCode.ToStatus();
        StatusDescription = response.ReasonPhrase ?? StatusCode.FastToString();
        URL               = response.RequestMessage?.RequestUri;
        Method            = response.RequestMessage?.Method.Method;
        ContentType       = response.RequestMessage?.Content?.Headers.ContentType?.MediaType;
        ContentEncoding   = response.RequestMessage?.Content?.Headers.ContentEncoding.ToJson();
        Server            = response.RequestMessage?.Headers.From;
    }


    internal static ResponseData<T> None( HttpResponseMessage response ) => new(response, "NO RESPONSE");


    public override string ToString() => this.ToJson(Formatting.Indented);


    public static async Task<ResponseData<T>> Create<TArg>( HttpRequestBuilder.Handler handler, TArg arg, Func<HttpResponseMessage, TArg, Task<T>> func )
    {
        using HttpResponseMessage response = await handler;

        try
        {
            T result = await func(response, arg);
            return new ResponseData<T>(response, result);
        }
        catch ( HttpRequestException e ) { return await Create(handler, response, e); }
    }
    public static async Task<ResponseData<T>> Create( HttpRequestBuilder.Handler handler, Func<HttpResponseMessage, Task<T>> func )
    {
        using HttpResponseMessage response = await handler;

        try
        {
            T result = await func(response);
            return new ResponseData<T>(response, result);
        }
        catch ( HttpRequestException e ) { return await Create(handler, response, e); }
    }
    private static async Task<ResponseData<T>> Create( HttpRequestBuilder.Handler handler, HttpResponseMessage response, HttpRequestException e )
    {
        await using Stream? stream = await response.Content.ReadAsStreamAsync(handler.token);

        string msg;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is not null )
        {
            using var reader       = new StreamReader(stream);
            string?   errorMessage = await reader.ReadToEndAsync();

            if ( string.IsNullOrWhiteSpace(errorMessage) ) { return None(response); }

            msg = $"{ERROR_MESSAGE}{errorMessage}";
        }
        else { msg = UNKNOWN_ERROR; }

        return new ResponseData<T>(response, e, msg);
    }
}



#endif
