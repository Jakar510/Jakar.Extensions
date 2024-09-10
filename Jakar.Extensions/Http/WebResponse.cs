// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:51 AM

namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "UnusedParameter.Global" ), SuppressMessage( "ReSharper", "CollectionNeverQueried.Global" )]
public readonly record struct WebResponse<T>
{
    public const string ERROR_MESSAGE = "Error Message: ";
    public const string NO_RESPONSE   = "NO RESPONSE";
    public const string UNKNOWN_ERROR = "Unknown Error";


    public              List<string>                Allow             { get; init; } = [];
    public              List<string>                ContentEncoding   { get; init; } = [];
    public              long?                       ContentLength     { get; init; } = null;
    public              string?                     ContentType       { get; init; } = null;
    [JsonIgnore] public OneOf<JToken, string, None> Error             { get; init; } = new None();
    public              string?                     ErrorMessage      => Error.Match<string?>( x => x.ToString( Formatting.Indented ), x => x, x => null );
    [JsonIgnore] public Exception?                  Exception         { get; init; } = null;
    public              DateTimeOffset?             Expires           { get; init; } = null;
    public              DateTimeOffset?             LastModified      { get; init; } = null;
    public              Uri?                        Location          { get; init; } = null;
    public              string?                     Method            { get; init; } = null;
    public              T?                          Payload           { get; init; } = default;
    public              string?                     Sender            { get; init; } = null;
    public              string?                     Server            { get; init; } = null;
    public              Status                      StatusCode        { get; init; } = Status.NotSet;
    public              string?                     StatusDescription { get; init; } = null;
    public              Uri?                        URL               { get; init; } = null;


    public WebResponse( HttpResponseMessage response, in string error ) : this( response, default, null, error ) { }
    public WebResponse( HttpResponseMessage response, Exception e, in string error ) : this( response, default, e, error ) { }
    public WebResponse( HttpResponseMessage response, T? payload, Exception? exception = default, in string? error = null )
    {
        Error             = ParseError( error ?? exception?.Message );
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


    [MemberNotNullWhen( true, nameof(Payload) )] public bool IsSuccessStatusCode() => StatusCode < Status.BadRequest && Payload is not null;
    public void EnsureSuccessStatusCode()
    {
        if ( IsSuccessStatusCode() ) { return; }

        throw new HttpRequestException( this.ToPrettyJson(), Exception );
    }
    public override string ToString() => this.ToJson( Formatting.Indented );


    internal static WebResponse<T> None( HttpResponseMessage response )              => new(response, NO_RESPONSE);
    internal static WebResponse<T> None( HttpResponseMessage response, Exception e ) => new(response, e, NO_RESPONSE);
    public static OneOf<JToken, string, None> ParseError( in string? error )
    {
        if ( string.IsNullOrWhiteSpace( error ) ) { return new None(); }

        try { return error.Replace( @"\""", @"""" ).FromJson(); }
        catch ( Exception ) { return error; }
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
                if ( response.IsSuccessStatusCode is false ) { return await Create( response, handler.Token ); }

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
            if ( response.IsSuccessStatusCode is false ) { return await Create( response, token ); }

            T result = await func( response );
            return new WebResponse<T>( response, result );
        }
        catch ( HttpRequestException e ) { return await Create( response, e, token ); }
    }
    public static async ValueTask<WebResponse<T>> Create<TArg>( HttpResponseMessage response, TArg arg, Func<HttpResponseMessage, TArg, ValueTask<T>> func, CancellationToken token )
    {
        try
        {
            if ( response.IsSuccessStatusCode is false ) { return await Create( response, token ); }

            T result = await func( response, arg );
            return new WebResponse<T>( response, result );
        }
        catch ( HttpRequestException e ) { return await Create( response, e, token ); }
    }


    public static async ValueTask<WebResponse<T>> Create( HttpResponseMessage response, Func<HttpResponseMessage, ValueTask<T>> func, RetryPolicy policy, CancellationToken token )
    {
        ushort      count      = 0;
        Exception[] exceptions = new Exception[policy.MaxRetires];

        while ( count < policy.MaxRetires )
        {
            try
            {
                if ( response.IsSuccessStatusCode is false ) { return await Create( response, token ); }

                T result = await func( response );
                return new WebResponse<T>( response, result );
            }
            catch ( HttpRequestException e ) { exceptions[count] = e; }


            await policy.Wait( ref count, token );
        }

        try { throw new AggregateException( exceptions[..count] ); }
        catch ( AggregateException e ) { return await Create( response, e, token ); }
    }
    public static async ValueTask<WebResponse<T>> Create<TArg>( HttpResponseMessage response, TArg arg, Func<HttpResponseMessage, TArg, ValueTask<T>> func, RetryPolicy policy, CancellationToken token )
    {
        ushort      count      = 0;
        Exception[] exceptions = new Exception[policy.MaxRetires];

        while ( count < policy.MaxRetires )
        {
            try
            {
                if ( response.IsSuccessStatusCode is false ) { return await Create( response, token ); }

                T result = await func( response, arg );
                return new WebResponse<T>( response, result );
            }
            catch ( HttpRequestException e ) { exceptions[count] = e; }


            await policy.Wait( ref count, token );
        }

        try { throw new AggregateException( exceptions[..count] ); }
        catch ( AggregateException e ) { return await Create( response, e, token ); }
    }


    public static async ValueTask<WebResponse<T>> Create( HttpResponseMessage response, CancellationToken token )
    {
        await using Stream? stream = await response.Content.ReadAsStreamAsync( token );
        string              error;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is null ) { error = UNKNOWN_ERROR; }
        else
        {
            using StreamReader reader       = new StreamReader( stream );
            string             errorMessage = await reader.ReadToEndAsync( token );

            if ( string.IsNullOrWhiteSpace( errorMessage ) ) { return None( response ); }

            error = errorMessage;
        }

        return new WebResponse<T>( response, error );
    }
    public static async ValueTask<WebResponse<T>> Create( HttpResponseMessage response, Exception e, CancellationToken token )
    {
        await using Stream? stream = await response.Content.ReadAsStreamAsync( token );
        string              error;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is null ) { error = UNKNOWN_ERROR; }
        else
        {
            using StreamReader reader       = new StreamReader( stream );
            string             errorMessage = await reader.ReadToEndAsync( token );

            if ( string.IsNullOrWhiteSpace( errorMessage ) ) { return None( response, e ); }

            error = errorMessage;
        }

        return new WebResponse<T>( response, e, error );
    }
}
