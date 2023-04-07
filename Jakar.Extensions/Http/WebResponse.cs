﻿// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:51 AM

using static Jakar.Extensions.WebRequester;



namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "UnusedParameter.Global" )]
[SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
public readonly record struct WebResponse<T>
{
    public const        string          ERROR_MESSAGE = "Error Message: ";
    public const        string          UNKNOWN_ERROR = "Unknown Error";
    public              List<string>    Allow             { get; init; } = new();
    public              List<string>    ContentEncoding   { get; init; } = new();
    public              long?           ContentLength     { get; init; } = default;
    public              string?         ContentType       { get; init; } = default;
    public              JToken?         ErrorMessage      { get; init; } = default;
    [JsonIgnore] public Exception?      Exception         { get; init; } = default;
    public              DateTimeOffset? Expires           { get; init; } = default;
    public              DateTimeOffset? LastModified      { get; init; } = default;
    public              Uri?            Location          { get; init; } = default;
    public              string?         Method            { get; init; } = default;
    public              T?              Payload           { get; init; } = default;
    public              string?         Sender            { get; init; } = default;
    public              string?         Server            { get; init; } = default;
    public              Status          StatusCode        { get; init; } = Status.NotSet;
    public              string?         StatusDescription { get; init; } = default;
    public              Uri?            URL               { get; init; } = default;


    public WebResponse( HttpResponseMessage response, string    error ) : this( response, default, default, error ) { }
    public WebResponse( HttpResponseMessage response, Exception e, in string error ) : this( response, default, e, error ) { }
    public WebResponse( HttpResponseMessage response, T? payload, Exception? exception = default, string? error = default )
    {
        ErrorMessage      = ParseError( error ?? exception?.Message );
        Payload           = payload;
        Exception         = exception;
        StatusCode        = response.StatusCode.ToStatus();
        StatusDescription = response.ReasonPhrase ?? StatusCode.ToStringFast();
        URL               = response.RequestMessage?.RequestUri;
        Method            = response.RequestMessage?.Method.Method;
        Sender            = response.RequestMessage?.Headers.From;
        Location          = response.Headers.Location;
        Server            = response.Headers.Server.ToString();

        HttpContentHeaders contentHeaders = response.Content.Headers;
        ContentLength = contentHeaders.ContentLength;
        Expires       = contentHeaders.Expires;
        LastModified  = contentHeaders.LastModified;
        ContentType   = contentHeaders.ContentType?.ToString();

        ContentEncoding.AddRange( contentHeaders.ContentEncoding );
        Allow.AddRange( contentHeaders.Allow );
    }


    /// <summary> Gets the payload if available; otherwise throws. </summary>
    /// <exception cref="HttpRequestException"> </exception>
    public T GetPayload()
    {
        EnsureSuccessStatusCode();
        Debug.Assert( Payload is not null );
        return Payload ?? throw new NullReferenceException( nameof(Payload), Exception );
    }
    public bool IsSuccessStatusCode() => StatusCode < Status.BadRequest;
    public void EnsureSuccessStatusCode()
    {
        if ( IsSuccessStatusCode() ) { return; }

        throw new HttpRequestException( this.ToPrettyJson(), Exception );
    }
    public override string ToString() => this.ToJson( Formatting.Indented );


    internal static WebResponse<T> None( HttpResponseMessage response ) => new(response, "NO RESPONSE");
    internal static WebResponse<T> None( HttpResponseMessage response, Exception e ) => new(response, e, "NO RESPONSE");
    public static JToken? ParseError( ReadOnlySpan<char> error )
    {
        if ( error.IsNullOrWhiteSpace() ) { return default; }

        if ( error.StartsWith( ERROR_MESSAGE, StringComparison.OrdinalIgnoreCase ) ) { error = error[ERROR_MESSAGE.Length..]; }

        try { return error.FromJson(); }
        catch ( Exception ) { return error.ToString(); }
    }


    public static async ValueTask<WebResponse<T>> Create( WebHandler handler, Func<HttpResponseMessage, ValueTask<T>> func )
    {
        using ( handler )
        {
            using HttpResponseMessage response = await handler;

            return handler.RetryPolicy?.AllowRetries is true
                       ? await Create( response, func, handler.RetryPolicy.Value, handler.Token )
                       : await Create( response, func, handler.Token );
        }
    }
    public static async ValueTask<WebResponse<T>> Create<TArg>( WebHandler handler, TArg arg, Func<HttpResponseMessage, TArg, ValueTask<T>> func )
    {
        using ( handler )
        {
            using HttpResponseMessage response = await handler;

            try
            {
                if ( !response.IsSuccessStatusCode ) { return await Create( response, handler.Token ); }

                T result = await func( response, arg );
                return new WebResponse<T>( response, result );
            }
            catch ( HttpRequestException e ) { return await Create( response, e, handler.Token ); }
        }
    }


    public static async ValueTask<WebResponse<T>> Create( HttpResponseMessage response, Func<HttpResponseMessage, ValueTask<T>> func, CancellationToken token )
    {
        try
        {
            if ( !response.IsSuccessStatusCode ) { return await Create( response, token ); }

            T result = await func( response );
            return new WebResponse<T>( response, result );
        }
        catch ( HttpRequestException e ) { return await Create( response, e, token ); }
    }
    public static async ValueTask<WebResponse<T>> Create<TArg>( HttpResponseMessage response, TArg arg, Func<HttpResponseMessage, TArg, ValueTask<T>> func, CancellationToken token )
    {
        try
        {
            if ( !response.IsSuccessStatusCode ) { return await Create( response, token ); }

            T result = await func( response, arg );
            return new WebResponse<T>( response, result );
        }
        catch ( HttpRequestException e ) { return await Create( response, e, token ); }
    }


    public static async ValueTask<WebResponse<T>> Create( HttpResponseMessage response, Func<HttpResponseMessage, ValueTask<T>> func, RetryPolicy policy, CancellationToken token )
    {
        int count      = 0;
        var exceptions = new Exception[policy.MaxRetires];

        while ( count < policy.MaxRetires )
        {
            try
            {
                if ( !response.IsSuccessStatusCode ) { return await Create( response, token ); }

                T result = await func( response );
                return new WebResponse<T>( response, result );
            }
            catch ( HttpRequestException e ) { exceptions[count] = e; }


            await policy.Wait( ref count, token );
        }

        try { throw new AggregateException( exceptions ); }
        catch ( AggregateException e ) { return await Create( response, e, token ); }
    }
    public static async ValueTask<WebResponse<T>> Create<TArg>( HttpResponseMessage response, TArg arg, Func<HttpResponseMessage, TArg, ValueTask<T>> func, RetryPolicy policy, CancellationToken token )
    {
        int count      = 0;
        var exceptions = new Exception[policy.MaxRetires];

        while ( count < policy.MaxRetires )
        {
            try
            {
                if ( !response.IsSuccessStatusCode ) { return await Create( response, token ); }

                T result = await func( response, arg );
                return new WebResponse<T>( response, result );
            }
            catch ( HttpRequestException e ) { exceptions[count] = e; }


            await policy.Wait( ref count, token );
        }

        try { throw new AggregateException( exceptions ); }
        catch ( AggregateException e ) { return await Create( response, e, token ); }
    }


    public static async ValueTask<WebResponse<T>> Create( HttpResponseMessage response, CancellationToken token )
    {
    #if NETSTANDARD2_1
        await using Stream? stream = await response.Content.ReadAsStreamAsync();
    #else
        await using Stream? stream = await response.Content.ReadAsStreamAsync( token );
    #endif

        string error;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is null ) { error = UNKNOWN_ERROR; }
        else
        {
            using var reader       = new StreamReader( stream );
            string    errorMessage = await reader.ReadToEndAsync();

            if ( string.IsNullOrWhiteSpace( errorMessage ) ) { return None( response ); }

            error = $"Error Message: '{errorMessage}'";
        }

        return new WebResponse<T>( response, error );
    }
    public static async ValueTask<WebResponse<T>> Create( HttpResponseMessage response, Exception e, CancellationToken token )
    {
    #if NETSTANDARD2_1
        await using Stream? stream = await response.Content.ReadAsStreamAsync();
    #else
        await using Stream? stream = await response.Content.ReadAsStreamAsync( token );
    #endif

        string error;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is null ) { error = UNKNOWN_ERROR; }
        else
        {
            using var reader       = new StreamReader( stream );
            string    errorMessage = await reader.ReadToEndAsync();

            if ( string.IsNullOrWhiteSpace( errorMessage ) ) { return None( response, e ); }

            error = $"Error Message: '{errorMessage}'";
        }

        return new WebResponse<T>( response, e, error );
    }
}
