// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:51 AM

using Microsoft.Extensions.Primitives;



namespace Jakar.Extensions;


[IsNotJsonSerializable]
public sealed class WebResponse<TValue>
{
    public const    string                              ERROR_MESSAGE = "Error Message: ";
    public const    string                              NO_RESPONSE   = "NO RESPONSE";
    public const    string                              UNKNOWN_ERROR = "Unknown Error";
    public readonly DateTimeOffset?                     Expires;
    public readonly DateTimeOffset?                     LastModified;
    public readonly Exception?                          Exception;
    public readonly List<string>                        Allow           = new(DEFAULT_CAPACITY);
    public readonly List<string>                        ContentEncoding = new(DEFAULT_CAPACITY);
    public readonly long?                               ContentLength;
    public readonly OneOf<JToken, string, Errors, None> Errors;
    public readonly Status                              StatusCode;
    public readonly string?                             ContentType;
    public readonly string?                             Method;
    public readonly string?                             Sender;
    public readonly string?                             Server;
    public readonly string?                             StatusDescription;
    public readonly TValue?                             Payload;
    public readonly Uri?                                Location;
    public readonly Uri?                                URL;


    public                                              string? ErrorMessage        => Errors.Match<string?>( static x => x.ToString( Formatting.Indented ), static x => x, static x => x.GetMessage(), static x => null );
    [MemberNotNullWhen( true, nameof(Payload) )] public bool    HasPayload          => Payload is not null;
    public                                              bool    IsSuccessStatusCode => StatusCode < Status.BadRequest;


    public WebResponse( HttpResponseMessage response, string    error ) : this( response, default, null, error ) { }
    public WebResponse( HttpResponseMessage response, Exception e, string error ) : this( response, default, e, error ) { }
    public WebResponse( HttpResponseMessage response, TValue? payload, Exception? exception = null, string? error = null )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        Errors            = ParseError( error ?? exception?.Message );
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
    public TValue GetPayload()
    {
        EnsureSuccessStatusCode();
        Debug.Assert( Payload is not null );
        return Payload ?? throw new NullReferenceException( nameof(Payload), Exception );
    }


    public ErrorOrResult<TValue> TryGetPayload() =>
        TryGetValue( out TValue? payload, out Errors? error )
            ? payload
            : error;


    public bool TryGetValue( [NotNullWhen( true )] out TValue? payload, [NotNullWhen( false )] out Errors? errorMessage )
    {
        if ( IsSuccessStatusCode )
        {
            payload      = Payload;
            errorMessage = null;
            return payload is not null;
        }

        payload      = default;
        errorMessage = GetError();
        return false;
    }
    private Error GetError() => Error.Create( Exception, ErrorMessage, URL?.OriginalString, StringValues.Empty, StatusCode );
    public void EnsureSuccessStatusCode()
    {
        if ( IsSuccessStatusCode ) { return; }

        throw new HttpRequestException( this.ToPrettyJson(), Exception );
    }
    public override string ToString() => this.ToJson( Formatting.Indented );


    internal static WebResponse<TValue> None( HttpResponseMessage response )              => new(response, NO_RESPONSE);
    internal static WebResponse<TValue> None( HttpResponseMessage response, Exception e ) => new(response, e, NO_RESPONSE);
    public static OneOf<JToken, string, Errors, None> ParseError( string? error )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        if ( string.IsNullOrWhiteSpace( error ) ) { return UNKNOWN_ERROR; }

        const string ESCAPED_QUOTE = @"\""";
        const string QUOTE         = @"""";
        if ( error.Contains( ESCAPED_QUOTE ) ) { error = error.Replace( ESCAPED_QUOTE, QUOTE ); }

        try { return error.FromJson<Errors>(); }
        catch ( Exception e )
        {
            telemetrySpan.AddException( e );

            try { return error.FromJson(); }
            catch ( Exception ) { return error; }
        }
    }


    /*
    public static JToken? ParseError( ReadOnlySpan<char> error )
    {
        if ( error.IsNullOrWhiteSpace() ) { return default; }

        try
        {
            try
            {
                Span<char> oldChars = stackalloc char[2];
                oldChars[0] = '\\';
                oldChars[1] = '"';
                Span<char> newChars = stackalloc char[2];
                newChars[0] = '"';

                Span<char> buffer = stackalloc char[error.Length + 1];
                Spans.Replace( error, oldChars, newChars, buffer, out int charWritten );
                buffer = buffer[..charWritten];
                if ( buffer.StartsWith( ERROR_MESSAGE ) ) { buffer = buffer[ERROR_MESSAGE.Length..]; }

                return buffer.ToString()
                             .FromJson();
            }
            catch ( Exception ) { return error.ToString(); }
        }
        catch ( Exception ) { return error.ToString(); }
    }
    */


    public static async ValueTask<WebResponse<TValue>> Create( WebHandler handler, Func<HttpResponseMessage, ValueTask<TValue>> func )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( handler )
        {
            using HttpResponseMessage response = await handler;

            return handler.RetryPolicy?.AllowRetries is true
                       ? await Create( response, func, handler.RetryPolicy.Value, handler.Token )
                       : await Create( response, func, handler.Token );
        }
    }
    public static async ValueTask<WebResponse<TValue>> Create<TArg>( WebHandler handler, TArg arg, Func<HttpResponseMessage, TArg, ValueTask<TValue>> func )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        using ( handler )
        {
            using HttpResponseMessage response = await handler;

            try
            {
                if ( response.IsSuccessStatusCode is false ) { return await Create( response, handler.Token ); }

                TValue result = await func( response, arg );
                return new WebResponse<TValue>( response, result );
            }
            catch ( HttpRequestException e )
            {
                telemetrySpan.AddException( e );
                return await Create( response, e, handler.Token );
            }
        }
    }


    public static async ValueTask<WebResponse<TValue>> Create( HttpResponseMessage response, Func<HttpResponseMessage, ValueTask<TValue>> func, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        try
        {
            if ( response.IsSuccessStatusCode is false ) { return await Create( response, token ); }

            TValue result = await func( response );
            return new WebResponse<TValue>( response, result );
        }
        catch ( HttpRequestException e )
        {
            telemetrySpan.AddException( e );
            return await Create( response, e, token );
        }
    }
    public static async ValueTask<WebResponse<TValue>> Create<TArg>( HttpResponseMessage response, TArg arg, Func<HttpResponseMessage, TArg, ValueTask<TValue>> func, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        try
        {
            if ( response.IsSuccessStatusCode is false ) { return await Create( response, token ); }

            TValue result = await func( response, arg );
            return new WebResponse<TValue>( response, result );
        }
        catch ( HttpRequestException e )
        {
            telemetrySpan.AddException( e );
            return await Create( response, e, token );
        }
    }


    public static async ValueTask<WebResponse<TValue>> Create( HttpResponseMessage response, Func<HttpResponseMessage, ValueTask<TValue>> func, RetryPolicy policy, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        ushort              count         = 0;
        List<Exception>     exceptions    = new(policy.MaxRetires);

        while ( count < policy.MaxRetires )
        {
            try
            {
                if ( response.IsSuccessStatusCode is false ) { return await Create( response, token ); }

                TValue result = await func( response );
                return new WebResponse<TValue>( response, result );
            }
            catch ( HttpRequestException e ) { exceptions.Add( e ); }

            using ( telemetrySpan.SubSpan( nameof(policy.IncrementAndWait) ) ) { await policy.IncrementAndWait( ref count, token ); }
        }

        try { throw new AggregateException( exceptions.ToArray() ); }
        catch ( AggregateException e )
        {
            telemetrySpan.AddException( e );
            return await Create( response, e, token );
        }
    }
    public static async ValueTask<WebResponse<TValue>> Create<TArg>( HttpResponseMessage response, TArg arg, Func<HttpResponseMessage, TArg, ValueTask<TValue>> func, RetryPolicy policy, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        ushort              count         = 0;
        List<Exception>     exceptions    = new(policy.MaxRetires);

        while ( count < policy.MaxRetires )
        {
            try
            {
                if ( response.IsSuccessStatusCode is false ) { return await Create( response, token ); }

                TValue result = await func( response, arg );
                return new WebResponse<TValue>( response, result );
            }
            catch ( HttpRequestException e ) { exceptions.Add( e ); }

            using ( telemetrySpan.SubSpan( nameof(policy.IncrementAndWait) ) ) { await policy.IncrementAndWait( ref count, token ); }
        }

        try { throw new AggregateException( exceptions.ToArray() ); }
        catch ( AggregateException e )
        {
            telemetrySpan.AddException( e );
            return await Create( response, e, token );
        }
    }


    public static async ValueTask<WebResponse<TValue>> Create( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        await using Stream? stream        = await response.Content.ReadAsStreamAsync( token );
        string              error;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is null ) { error = UNKNOWN_ERROR; }
        else
        {
            using StreamReader reader       = new(stream);
            string             errorMessage = await reader.ReadToEndAsync( token );

            if ( string.IsNullOrWhiteSpace( errorMessage ) ) { return new WebResponse<TValue>( response, errorMessage ); }

            error = errorMessage;
        }

        return new WebResponse<TValue>( response, error );
    }
    public static async ValueTask<WebResponse<TValue>> Create( HttpResponseMessage response, Exception e, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        await using Stream? stream        = await response.Content.ReadAsStreamAsync( token );
        string              error;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is null ) { error = UNKNOWN_ERROR; }
        else
        {
            using StreamReader reader       = new(stream);
            string             errorMessage = await reader.ReadToEndAsync( token );

            if ( string.IsNullOrWhiteSpace( errorMessage ) ) { return new WebResponse<TValue>( response, errorMessage ); }

            error = errorMessage;
        }

        return new WebResponse<TValue>( response, e, error );
    }
}
