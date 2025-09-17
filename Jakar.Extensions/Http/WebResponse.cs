// Jakar.Extensions :: Jakar.Extensions
// 08/15/2022  11:51 AM

using Microsoft.Extensions.Primitives;



namespace Jakar.Extensions;


[IsNotJsonSerializable, SuppressMessage("ReSharper", "NotAccessedField.Global")]
public sealed class WebResponse<TValue>
{
    public readonly DateTimeOffset? Expires;
    public readonly DateTimeOffset? LastModified;
    public readonly Exception?      Exception;
    public readonly List<string>    Allow;
    public readonly List<string>    ContentEncoding;
    public readonly long?           ContentLength;
    public readonly OneOfErrors     Errors;
    public readonly Status          StatusCode;
    public readonly string?         ContentType;
    public readonly string?         Method;
    public readonly string?         Sender;
    public readonly string?         Server;
    public readonly string?         StatusDescription;
    public readonly TValue?         Payload;
    public readonly Uri?            Location;
    public readonly Uri?            URL;


    [JsonIgnore, MemberNotNullWhen(true, nameof(Payload))] public bool HasPayload          => Payload is not null;
    public                                                        bool IsSuccessStatusCode => StatusCode < Status.BadRequest;


    public WebResponse( HttpResponseMessage response, string    error ) : this(response, default, null, error) { }
    public WebResponse( HttpResponseMessage response, Exception e, string error ) : this(response, default, e, error) { }
    public WebResponse( HttpResponseMessage response, TValue? payload, Exception? exception = null, string? error = null )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        Errors            = OneOfErrors.Parse(error ?? exception?.Message);
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
        ContentLength   = contentHeaders.ContentLength;
        Expires         = contentHeaders.Expires;
        LastModified    = contentHeaders.LastModified;
        ContentType     = contentHeaders.ContentType?.ToString();
        ContentEncoding = [.. contentHeaders.ContentEncoding];
        Allow           = [.. contentHeaders.Allow];
    }


    /// <summary> Gets the payload if available; otherwise throws. </summary>
    /// <exception cref="HttpRequestException"> </exception>
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)]
    public TValue GetPayload()
    {
        EnsureSuccessStatusCode();
        Debug.Assert(Payload is not null);
        return Payload ?? throw new NullReferenceException(nameof(Payload), Exception);
    }


    public ErrorOrResult<TValue> TryGetPayload() =>
        TryGetValue(out TValue? payload, out Errors? error)
            ? payload
            : error;


    public bool TryGetValue( [NotNullWhen(true)] out TValue? payload, [NotNullWhen(false)] out Errors? errorMessage )
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
    public bool TryGetValue( [NotNullWhen(true)] out TValue? payload, [NotNullWhen(false)] out OneOfErrors errorMessage )
    {
        if ( IsSuccessStatusCode )
        {
            payload      = Payload;
            errorMessage = OneOfErrors.Empty;
            return payload is not null;
        }

        payload      = default;
        errorMessage = Errors;
        return false;
    }
    public  Errors GetError()                => Errors.Match<Errors>(x => GetError(x.ToString(Formatting.Indented)), GetError, static x => x);
    private Errors GetError( string detail ) => Error.Create(Exception, ErrorMessage(), URL?.OriginalString, StringValues.Empty, StatusCode);
    public  string ErrorMessage()            => Errors.Match<string>(static x => x.ToString(Formatting.Indented), static x => x, static x => x.GetMessage());

    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)] public override string ToString() => this.ToJson(Formatting.Indented);
    [RequiresUnreferencedCode(JsonModels.TRIM_WARNING), RequiresDynamicCode(JsonModels.AOT_WARNING)]
    public void EnsureSuccessStatusCode()
    {
        if ( IsSuccessStatusCode ) { return; }

        throw new HttpRequestException(this.ToPrettyJson(), Exception);
    }


    internal static WebResponse<TValue> None( HttpResponseMessage response )              => new(response, WebHandler.NO_RESPONSE);
    internal static WebResponse<TValue> None( HttpResponseMessage response, Exception e ) => new(response, e, WebHandler.NO_RESPONSE);


    public static async ValueTask<WebResponse<TValue>> Create( HttpResponseMessage response, Func<HttpResponseMessage, CancellationToken, ValueTask<TValue>> func, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        try
        {
            if ( !response.IsSuccessStatusCode ) { return await Create(response, token); }

            TValue result = await func(response, token);
            return new WebResponse<TValue>(response, result);
        }
        catch ( HttpRequestException e )
        {
            telemetrySpan.AddException(e);
            return await Create(response, e, token);
        }
    }
    public static async ValueTask<WebResponse<TValue>> Create<TArg>( HttpResponseMessage response, TArg arg, Func<HttpResponseMessage, TArg, CancellationToken, ValueTask<TValue>> func, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();

        try
        {
            if ( !response.IsSuccessStatusCode ) { return await Create(response, token); }

            TValue result = await func(response, arg, token);
            return new WebResponse<TValue>(response, result);
        }
        catch ( HttpRequestException e )
        {
            telemetrySpan.AddException(e);
            return await Create(response, e, token);
        }
    }


    public static async ValueTask<WebResponse<TValue>> Create( HttpResponseMessage response, Func<HttpResponseMessage, CancellationToken, ValueTask<TValue>> func, RetryPolicy policy, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        ushort              count         = 0;
        List<Exception>     exceptions    = new(policy.MaxRetires);

        while ( count < policy.MaxRetires )
        {
            try
            {
                if ( !response.IsSuccessStatusCode ) { return await Create(response, token); }

                TValue result = await func(response, token);
                return new WebResponse<TValue>(response, result);
            }
            catch ( HttpRequestException e ) { exceptions.Add(e); }

            using ( telemetrySpan.SubSpan(nameof(policy.IncrementAndWait)) ) { await policy.IncrementAndWait(ref count, token); }
        }

        try { throw new AggregateException(exceptions.ToArray()); }
        catch ( AggregateException e )
        {
            telemetrySpan.AddException(e);
            return await Create(response, e, token);
        }
    }
    public static async ValueTask<WebResponse<TValue>> Create<TArg>( HttpResponseMessage response, TArg arg, Func<HttpResponseMessage, TArg, CancellationToken, ValueTask<TValue>> func, RetryPolicy policy, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        ushort              count         = 0;
        List<Exception>     exceptions    = new(policy.MaxRetires);

        while ( count < policy.MaxRetires )
        {
            try
            {
                if ( !response.IsSuccessStatusCode ) { return await Create(response, token); }

                TValue result = await func(response, arg, token);
                return new WebResponse<TValue>(response, result);
            }
            catch ( HttpRequestException e ) { exceptions.Add(e); }

            using ( telemetrySpan.SubSpan(nameof(policy.IncrementAndWait)) ) { await policy.IncrementAndWait(ref count, token); }
        }

        try { throw new AggregateException(exceptions.ToArray()); }
        catch ( AggregateException e )
        {
            telemetrySpan.AddException(e);
            return await Create(response, e, token);
        }
    }


    public static async ValueTask<WebResponse<TValue>> Create( HttpResponseMessage response, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        await using Stream? stream        = await response.Content.ReadAsStreamAsync(token);
        string              error;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is null ) { error = OneOfErrors.UNKNOWN_ERROR; }
        else
        {
            using StreamReader reader       = new(stream);
            string             errorMessage = await reader.ReadToEndAsync(token);
            if ( string.IsNullOrWhiteSpace(errorMessage) ) { return new WebResponse<TValue>(response, errorMessage); }

            error = errorMessage;
        }

        return new WebResponse<TValue>(response, error);
    }
    public static async ValueTask<WebResponse<TValue>> Create( HttpResponseMessage response, Exception e, CancellationToken token )
    {
        using TelemetrySpan telemetrySpan = TelemetrySpan.Create();
        await using Stream? stream        = await response.Content.ReadAsStreamAsync(token);
        string              error;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if ( stream is null ) { error = OneOfErrors.UNKNOWN_ERROR; }
        else
        {
            using StreamReader reader       = new(stream);
            string             errorMessage = await reader.ReadToEndAsync(token);
            if ( string.IsNullOrWhiteSpace(errorMessage) ) { return new WebResponse<TValue>(response, e, errorMessage); }

            error = errorMessage;
        }

        return new WebResponse<TValue>(response, e, error);
    }
}
