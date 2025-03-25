// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:51 AM

using Microsoft.Extensions.Primitives;



namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "UnusedParameter.Global" ), SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
public readonly record struct WebResponse<TValue>
{
    public const string ERROR_MESSAGE = "Error Message: ";
    public const string NO_RESPONSE   = "NO RESPONSE";
    public const string UNKNOWN_ERROR = "Unknown Error";


    public              List<string>                         Allow             { get; init; } = [];
    public              List<string>                         ContentEncoding   { get; init; } = [];
    public              long?                                ContentLength     { get; init; } = null;
    public              string?                              ContentType       { get; init; } = null;
    [JsonIgnore] public OneOf<JToken, string, Error[], None> Errors            { get; init; } = new None();
    public              string?                              ErrorMessage      => Errors.Match<string?>( static x => x.ToString( Formatting.Indented ), static x => x, static x => x.GetMessage(), static x => null );
    [JsonIgnore] public Exception?                           Exception         { get; init; } = null;
    public              DateTimeOffset?                      Expires           { get; init; } = null;
    public              DateTimeOffset?                      LastModified      { get; init; } = null;
    public              Uri?                                 Location          { get; init; } = null;
    public              string?                              Method            { get; init; } = null;
    public              TValue?                                   Payload           { get; init; } = default;
    public              string?                              Sender            { get; init; } = null;
    public              string?                              Server            { get; init; } = null;
    public              Status                               StatusCode        { get; init; } = Status.NotSet;
    public              string?                              StatusDescription { get; init; } = null;
    public              Uri?                                 URL               { get; init; } = null;


    public WebResponse( HttpResponseMessage response, string    error ) : this( response, default, null, error ) { }
    public WebResponse( HttpResponseMessage response, Exception e, string error ) : this( response, default, e, error ) { }
    public WebResponse( HttpResponseMessage response, TValue? payload, Exception? exception = null, string? error = null )
    {
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


    public ErrorOrResult<TValue> TryGetPayload()
    {
        return TryGetValue( out TValue? payload, out Error? error )
                   ? payload
                   : error.Value;
    }


    public bool TryGetValue( [NotNullWhen( true )] out TValue? payload, [NotNullWhen( false )] out Error? errorMessage )
    {
        if ( IsSuccessStatusCode() )
        {
            payload      = Payload;
            errorMessage = null;
            return payload is not null;
        }

        payload      = default;
        errorMessage = GetError();
        return false;
    }
    public  bool  HasPayload()          => Payload is not null;
    private Error GetError()            => Error.Create( Exception, ErrorMessage, URL?.OriginalString, StringValues.Empty, status: StatusCode );
    public  bool  IsSuccessStatusCode() => StatusCode < Status.BadRequest;
    public void EnsureSuccessStatusCode()
    {
        if ( IsSuccessStatusCode() ) { return; }

        throw new HttpRequestException( this.ToPrettyJson(), Exception );
    }
    public override string ToString() => this.ToJson( Formatting.Indented );


    internal static WebResponse<TValue> None( HttpResponseMessage response )              => new(response, NO_RESPONSE);
    internal static WebResponse<TValue> None( HttpResponseMessage response, Exception e ) => new(response, e, NO_RESPONSE);
    public static OneOf<JToken, string, Error[], None> ParseError( string? error )
    {
        if ( string.IsNullOrWhiteSpace( error ) ) { return UNKNOWN_ERROR; }

        const string ESCAPED_QUOTE                   = @"\""";
        const string QUOTE                           = @"""";
        if ( error.Contains( ESCAPED_QUOTE ) ) error = error.Replace( ESCAPED_QUOTE, QUOTE );

        try { return error.FromJson<Error[]>(); }
        catch ( Exception )
        {
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
        using ( handler )
        {
            using HttpResponseMessage response = await handler;

            try
            {
                if ( response.IsSuccessStatusCode is false ) { return await Create( response, handler.Token ); }

                TValue result = await func( response, arg );
                return new WebResponse<TValue>( response, result );
            }
            catch ( HttpRequestException e ) { return await Create( response, e, handler.Token ); }
        }
    }


    public static async ValueTask<WebResponse<TValue>> Create( HttpResponseMessage response, Func<HttpResponseMessage, ValueTask<TValue>> func, CancellationToken token )
    {
        try
        {
            if ( response.IsSuccessStatusCode is false ) { return await Create( response, token ); }

            TValue result = await func( response );
            return new WebResponse<TValue>( response, result );
        }
        catch ( HttpRequestException e ) { return await Create( response, e, token ); }
    }
    public static async ValueTask<WebResponse<TValue>> Create<TArg>( HttpResponseMessage response, TArg arg, Func<HttpResponseMessage, TArg, ValueTask<TValue>> func, CancellationToken token )
    {
        try
        {
            if ( response.IsSuccessStatusCode is false ) { return await Create( response, token ); }

            TValue result = await func( response, arg );
            return new WebResponse<TValue>( response, result );
        }
        catch ( HttpRequestException e ) { return await Create( response, e, token ); }
    }


    public static async ValueTask<WebResponse<TValue>> Create( HttpResponseMessage response, Func<HttpResponseMessage, ValueTask<TValue>> func, RetryPolicy policy, CancellationToken token )
    {
        ushort          count      = 0;
        List<Exception> exceptions = new(policy.MaxRetires);

        while ( count < policy.MaxRetires )
        {
            try
            {
                if ( response.IsSuccessStatusCode is false ) { return await Create( response, token ); }

                TValue result = await func( response );
                return new WebResponse<TValue>( response, result );
            }
            catch ( HttpRequestException e ) { exceptions.Add( e ); }

            await policy.IncrementAndWait( ref count, token );
        }

        try { throw new AggregateException( exceptions.ToArray() ); }
        catch ( AggregateException e ) { return await Create( response, e, token ); }
    }
    public static async ValueTask<WebResponse<TValue>> Create<TArg>( HttpResponseMessage response, TArg arg, Func<HttpResponseMessage, TArg, ValueTask<TValue>> func, RetryPolicy policy, CancellationToken token )
    {
        ushort          count      = 0;
        List<Exception> exceptions = new(policy.MaxRetires);

        while ( count < policy.MaxRetires )
        {
            try
            {
                if ( response.IsSuccessStatusCode is false ) { return await Create( response, token ); }

                TValue result = await func( response, arg );
                return new WebResponse<TValue>( response, result );
            }
            catch ( HttpRequestException e ) { exceptions.Add( e ); }

            await policy.IncrementAndWait( ref count, token );
        }

        try { throw new AggregateException( exceptions.ToArray() ); }
        catch ( AggregateException e ) { return await Create( response, e, token ); }
    }


    public static async ValueTask<WebResponse<TValue>> Create( HttpResponseMessage response, CancellationToken token )
    {
        await using Stream? stream = await response.Content.ReadAsStreamAsync( token );
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
        await using Stream? stream = await response.Content.ReadAsStreamAsync( token );
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
