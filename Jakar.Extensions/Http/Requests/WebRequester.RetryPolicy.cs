namespace Jakar.Extensions;


public partial class WebRequester
{
    /// <summary>
    ///     How <see cref="WebHandler"/> retries a request. A retry re-sends a fresh copy of the request (an <see cref="HttpRequestMessage"/> can only be sent once) when:
    ///     <list type="bullet">
    ///         <item> sending fails with a network error (<see cref="HttpRequestException"/>, <see cref="IOException"/>) or times out; </item>
    ///         <item> reading the response body fails the same way; </item>
    ///         <item> the server answers 408, 429, 500, 502, 503 or 504. </item>
    ///     </list>
    ///     Only requests whose body can be sent again are retried (no body, bytes, string, JSON, form, or multipart made of those); a <see cref="StreamContent"/> body is sent once.
    ///     The wait before retry <c>n</c> (1-based) is <see cref="Delay"/> + <see cref="Scale"/> × n, or the server's <c>Retry-After</c> when that is longer; a <c>Retry-After</c> above
    ///     <see cref="MaxRetryAfter"/> is not waited for and the response is returned as is.
    /// </summary>
    /// <param name="delay"> The base wait before each retry. </param>
    /// <param name="scale"> Added once per retry number (linear back-off). </param>
    /// <param name="maxRetires"> Retries after the first attempt (total attempts = 1 + <paramref name="maxRetires"/>). </param>
    [DefaultValue(nameof(Default))]
    public readonly struct RetryPolicy( TimeSpan delay, TimeSpan scale, ushort maxRetires )
    {
        public static readonly TimeSpan    Time          = TimeSpan.FromSeconds(2);
        public static readonly TimeSpan    MaxRetryAfter = TimeSpan.FromMinutes(1);
        public static readonly RetryPolicy Default       = new(Time, Time, 3);
        public static readonly RetryPolicy None          = new(TimeSpan.Zero, TimeSpan.Zero, 0);
        public static readonly RetryPolicy Single        = new(TimeSpan.Zero, TimeSpan.Zero, 1);
        public readonly        bool        AllowRetries  = maxRetires > 0;
        public readonly        ushort      MaxRetires    = maxRetires;
        public readonly        TimeSpan    Delay         = delay;
        public readonly        TimeSpan    Scale         = scale;


        public static RetryPolicy Create( ushort   maxRetries )                               => Create(Time,  maxRetries);
        public static RetryPolicy Create( TimeSpan delay, ushort   maxRetries )               => Create(delay, delay, maxRetries);
        public static RetryPolicy Create( TimeSpan delay, TimeSpan scale, ushort maxRetries ) => new(delay, scale, maxRetries);


        /// <summary> The wait before retry number <paramref name="retry"/> (1-based): <see cref="Delay"/> + <see cref="Scale"/> × <paramref name="retry"/>. </summary>
        public TimeSpan GetDelay( int retry ) => Delay + Scale * retry;


        public Task IncrementAndWait( ref ushort count, CancellationToken token )
        {
            if ( !AllowRetries || count > MaxRetires ) { return Task.CompletedTask; }

            count++;
            return Task.Delay(GetDelay(count), token);
        }


        /// <summary> Status codes worth retrying: 408, 429, 500, 502, 503, 504. </summary>
        public static bool IsTransient( HttpStatusCode status ) => status is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests or HttpStatusCode.InternalServerError or HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout;


        /// <summary> Network failures and timeouts. Cancellation requested through <paramref name="token"/> is not a failure. </summary>
        public static bool IsTransient( Exception exception, CancellationToken token ) => exception switch
                                                                                          {
                                                                                              HttpRequestException       => true,
                                                                                              IOException                => true,
                                                                                              OperationCanceledException => !token.IsCancellationRequested, // HttpClient.Timeout
                                                                                              _                          => false
                                                                                          };
    }
}
