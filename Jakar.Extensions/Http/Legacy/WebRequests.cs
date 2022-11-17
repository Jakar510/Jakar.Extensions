#nullable enable
namespace Jakar.Extensions;


[Obsolete( $"Use {nameof(WebRequester)} instead" )]
public static class WebRequests
{
    public static Exception? ConvertException( this WebException we, CancellationToken token )
    {
        Exception? exception = we.Status switch
                               {
                                   WebExceptionStatus.ConnectFailure             => new ConnectFailureException( we.Message, we, token ),
                                   WebExceptionStatus.ConnectionClosed           => new ConnectionClosedException( we.Message, we, token ),
                                   WebExceptionStatus.KeepAliveFailure           => new KeepAliveFailureException( we.Message, we, token ),
                                   WebExceptionStatus.MessageLengthLimitExceeded => new MessageLengthLimitExceededException( we.Message, we, token ),
                                   WebExceptionStatus.NameResolutionFailure      => new NameResolutionException( we.Message, we, token ),
                                   WebExceptionStatus.Pending                    => new PendingWebException( we, token ),
                                   WebExceptionStatus.PipelineFailure            => new PipelineFailureException( we, token ),
                                   WebExceptionStatus.ProtocolError              => new ProtocolErrorException( we.Message, we, token ),
                                   WebExceptionStatus.ReceiveFailure             => new ReceiveFailureException( we.Message, we, token ),
                                   WebExceptionStatus.RequestCanceled            => new RequestAbortedException( we.Message, we, token ),
                                   WebExceptionStatus.SecureChannelFailure       => new SecureChannelFailureException( we.Message, we, token ),
                                   WebExceptionStatus.SendFailure                => new SendFailureException( we.Message, we, token ),
                                   WebExceptionStatus.ServerProtocolViolation    => new ServerProtocolViolationException( we.Message, we, token ),
                                   WebExceptionStatus.Timeout                    => new TimeoutException( we.Message, we ),
                                   WebExceptionStatus.TrustFailure               => new TrustFailureException( we.Message, we, token ),
                                   WebExceptionStatus.UnknownError => we.Message.Contains( ActivelyRefusedConnectionException.REFUSED, StringComparison.OrdinalIgnoreCase )
                                                                          ? new ActivelyRefusedConnectionException( we.Message, we, token )
                                                                          : new UnknownWebErrorException( we.Message, we, token ),


                                   // WebExceptionStatus.CacheEntryNotFound => new ,
                                   // WebExceptionStatus.ProxyNameResolutionFailure => new ,
                                   // WebExceptionStatus.RequestProhibitedByCachePolicy => new ,
                                   // WebExceptionStatus.RequestProhibitedByProxy => new ,

                                   WebExceptionStatus.Success => default,
                                   _                          => default,
                               };

        return exception;
    }
    /// <summary>
    ///     <seealso href="https://stackoverflow.com/questions/19211972/getresponseasync-does-not-accept-cancellationtoken"/>
    ///     <seealso
    ///         href="https://github.com/palburtus/HttpClient.net/blob/master/Aaks.Restclient/HttpRestClient.cs"/>
    /// </summary>
    /// <param name="request"> </param>
    /// <param name="token"> </param>
    /// <param name="useSynchronizationContext"> </param>
    /// <returns> </returns>
    /// <exception cref="WebException"> </exception>
    /// <exception cref="ArgumentNullException"> </exception>
    /// <exception cref="OperationCanceledException"> </exception>
    public static async Task<WebResponse> GetResponseAsync( this WebRequest request, CancellationToken token, bool useSynchronizationContext = true )
    {
        if ( request is null ) { throw new ArgumentNullException( nameof(request) ); }


        await using ( token.Register( request.Abort, useSynchronizationContext ) )
        {
            try
            {
                return await request.GetResponseAsync()
                                    .ConfigureAwait( false );
            }
            catch ( WebException ex )
            {
                if ( token.IsCancellationRequested ) // WebException is thrown when request.Abort() is called, but there may be many other reasons, propagate the WebException to the caller correctly
                {
                    throw new OperationCanceledException( ex.Message, ex, token ); // the WebException will be available as Exception.InnerException
                }

                // cancellation hasn't been requested, rethrow the original WebException
                if ( !Debugger.IsAttached ) { throw; }


                ResponseData details = await ResponseData.Create( ex )
                                                         .ConfigureAwait( false );

                details.WriteToDebug();
                throw;
            }
        }
    }


    // // public static void SetHeaders( this WebRequest request, IHeaderCollection headers ) { }
    // public static void SetHeaders( this WebRequest request, IDictionary<HttpRequestHeader, object> headers ) { headers.ForEach(request.SetHeader); }
    // public static void SetHeaders( this WebRequest request, IDictionary<string, object> headers ) { headers.ForEach(request.SetHeader); }
    //
    // public static void SetHeader( this WebRequest request, HttpRequestHeader key, object value )
    // {
    // 	// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
    // 	switch ( key )
    // 	{
    // 		// case HttpRequestHeader.From:
    // 		// 	break;
    // 		//
    // 		// case HttpRequestHeader.Host:
    // 		// 	break;
    //
    // 		// case HttpRequestHeader.Allow:
    // 		// 	break;
    //
    // 		// case HttpRequestHeader.Authorization:
    // 		// 	request.AuthenticationLevel = value.ToString();
    // 		// 	request.Credentials = value.ToString();
    // 		// 	break;
    //
    // 		case HttpRequestHeader.UserAgent:
    // 			throw new NotSupportedException($"{typeof(WebRequest).FullName} does not support UserAgent assignment.");
    //
    // 		case HttpRequestHeader.ContentLength:
    // 			if ( value is long contentLength ) { request.ContentLength = contentLength; }
    // 			else { throw new HeaderException(key, value, typeof(long)); }
    //
    // 			break;
    //
    // 		case HttpRequestHeader.ContentType:
    // 			request.SetContentType(value);
    // 			break;
    //
    // 		case HttpRequestHeader.ProxyAuthorization:
    // 			if ( value is IWebProxy proxy ) { request.Proxy = proxy; }
    // 			else { throw new HeaderException(key, value, typeof(IWebProxy)); }
    //
    // 			break;
    //
    // 		default:
    // 			request.Headers[key] = value.ToString();
    // 			return;
    // 	}
    // }
    //
    // public static void SetHeader( this WebRequest request, string key, object value )
    // {
    // 	if ( key == "Content-Type" ) { request.SetContentType(value); }
    // 	else { request.Headers.Add(key, value.ToJson()); }
    // }
    //
    // public static void SetHeaders( this WebRequest request, MultipartFormDataContent data ) => request.SetHeaders(data.Headers);
    //
    // public static void SetHeaders( this WebRequest request, HttpContentHeaders headers )
    // {
    // 	foreach ( ( string key, IEnumerable<string> items ) in headers )
    // 	{
    // 		string value = items.ToJson();
    //
    // 		if ( Enum.TryParse(key, true, out HttpRequestHeader result) ) { request.SetHeader(result, value); }
    // 		else { request.SetHeader(key, value); }
    // 	}
    // }


    public static void SetContentType( this WebRequest request, object value ) =>
        request.ContentType = value switch
                              {
                                  IEnumerable<string> items => items.First(),
                                  string item               => item,
                                  _                         => throw new HeaderException( HttpRequestHeader.ContentType, value, typeof(string), typeof(IEnumerable<string>) ),
                              };

    public static void SetHeader( this HttpWebRequest request, HttpRequestHeader key, object value )
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch ( key )
        {
            // case HttpRequestHeader.From:
            // 	break;
            //
            // case HttpRequestHeader.Host:
            // 	break;

            // case HttpRequestHeader.Allow:
            // 	break;

            // case HttpRequestHeader.Authorization:
            // 	request.AuthenticationLevel = value.ToString();
            // 	request.Credentials = value.ToString();
            // 	break;

            case HttpRequestHeader.Accept:
                request.Accept = value.ToString();
                break;

            case HttpRequestHeader.Connection:
                request.Connection = value.ToString();
                break;

            case HttpRequestHeader.ContentLength:
                if ( value is long contentLength ) { request.ContentLength = contentLength; }
                else { throw new HeaderException( key, value, typeof(long) ); }

                break;

            case HttpRequestHeader.ContentType:
                request.SetContentType( value );
                break;

            case HttpRequestHeader.Cookie:
                request.CookieContainer ??= new CookieContainer();

                switch ( value )
                {
                    case Cookie cookie:
                        request.CookieContainer.Add( cookie );
                        break;

                    case IEnumerable<Cookie> cookies:
                        foreach ( Cookie item in cookies ) { request.CookieContainer.Add( item ); }

                        break;

                    default: throw new HeaderException( key, value, typeof(Cookie), typeof(IEnumerable<Cookie>) );
                }

                break;

            case HttpRequestHeader.Date:
                if ( value is DateTime dateTime ) { request.Date = dateTime; }
                else { throw new HeaderException( key, value, typeof(DateTime) ); }

                break;

            case HttpRequestHeader.Expect:
                request.Expect = value.ToString();
                break;

            case HttpRequestHeader.KeepAlive:
                if ( value is bool keepAlive ) { request.KeepAlive = keepAlive; }
                else { throw new HeaderException( key, value, typeof(bool) ); }

                break;

            case HttpRequestHeader.MaxForwards:
                if ( value is int redirects ) { request.MaximumAutomaticRedirections = redirects; }
                else { throw new HeaderException( key, value, typeof(int) ); }

                break;

            case HttpRequestHeader.ProxyAuthorization:
                if ( value is IWebProxy proxy ) { request.Proxy = proxy; }
                else { throw new HeaderException( key, value, typeof(IWebProxy) ); }

                break;

            case HttpRequestHeader.TransferEncoding:
                request.TransferEncoding = value.ToString();
                break;

            case HttpRequestHeader.UserAgent:
                request.UserAgent = value.ToString();
                break;

            default:
                if ( value is string s ) { request.Headers.Add( key, s ); }
                else { request.Headers.Add( key,                     value.ToJson() ); }

                return;
        }
    }

    public static void SetHeader( this HttpWebRequest request, string key, object value )
    {
        switch ( key )
        {
            case "Content-Type":
                request.SetContentType( value );
                break;

            case "Content-Length":
                if ( value is long contentLength ) { request.ContentLength = contentLength; }
                else { throw new HeaderException( HttpRequestHeader.ContentLength, value, typeof(long) ); }

                break;

            case "Keep-Alive":
                if ( value is bool keepAlive ) { request.KeepAlive = keepAlive; }
                else { throw new HeaderException( HttpRequestHeader.KeepAlive, value, typeof(bool) ); }

                break;

            case "Max-Forwards":
                if ( value is int redirects ) { request.MaximumAutomaticRedirections = redirects; }
                else { throw new HeaderException( HttpRequestHeader.MaxForwards, value, typeof(int) ); }

                break;

            case "Proxy-Authorization":
                if ( value is IWebProxy proxy ) { request.Proxy = proxy; }
                else { throw new HeaderException( HttpRequestHeader.ProxyAuthorization, value, typeof(IWebProxy) ); }

                break;

            case "User-Agent":
                request.UserAgent = value.ToString();
                break;

            case "Transfer-Encoding":
                request.TransferEncoding = value.ToString();
                break;

            default:
                if ( value is string s ) { request.Headers.Add( key, s ); }
                else { request.Headers.Add( key,                     value.ToJson() ); }

                break;
        }
    }


    public static void SetHeader( this HttpContent content, string key, object value )
    {
        switch ( key )
        {
            case "Content-Type":
                content.Headers.ContentType = value switch
                                              {
                                                  MediaTypeHeaderValue v => v,
                                                  string v               => new MediaTypeHeaderValue( v ),
                                                  _                      => throw new HeaderException( HttpRequestHeader.ContentType, value, typeof(MediaTypeHeaderValue), typeof(string) ),
                                              };

                break;

            case "Content-Length":
                if ( value is long contentLength ) { content.Headers.ContentLength = contentLength; }
                else { throw new HeaderException( HttpRequestHeader.ContentLength, value, typeof(long) ); }

                break;

            case "Keep-Alive":
                if ( value is bool keepAlive ) { content.Headers.Add( key, keepAlive.ToString() ); }
                else { throw new HeaderException( HttpRequestHeader.KeepAlive, value, typeof(bool) ); }

                break;

            // case "Max-Forwards":
            //     if ( value is int redirects ) { headers.MaximumAutomaticRedirections = redirects; }
            //     else { throw new HeaderException(HttpRequestHeader.MaxForwards, value, typeof(int)); }
            //
            //     break;
            //
            // case "Proxy-Authorization":
            //     if ( value is IWebProxy proxy ) { content.Proxy = proxy; }
            //     else { throw new HeaderException(HttpRequestHeader.ProxyAuthorization, value, typeof(IWebProxy)); }
            //
            //     break;
            //
            // case "User-Agent":
            //     content.Headers.UserAgent = value.ToString();
            //     break;
            //
            // case "Transfer-Encoding":
            //     content.Headers.TransferEncoding = value.ToString();
            //     break;

            default:
                if ( value is string s ) { content.Headers.Add( key, s ); }
                else { content.Headers.Add( key,                     value.ToJson() ); }

                break;
        }
    }

    public static void SetHeaders( this HttpWebRequest request, MultipartFormDataContent data ) => request.SetHeaders( data.Headers );
    public static void SetHeaders( this HttpWebRequest request, HttpContentHeaders? headers )
    {
        if ( headers is null ) { return; }

        foreach ( (string key, IEnumerable<string> items) in headers )
        {
            if ( Enum.TryParse( key, true, out HttpRequestHeader httpRequestHeader ) ) { request.SetHeader( httpRequestHeader, items ); }
            else { request.SetHeader( key,                                                                                     items ); }
        }
    }


    public static void SetHeaders( this HttpWebRequest request, HeaderCollection? headers )
    {
        if ( headers is null ) { return; }

        foreach ( (string key, object value) in headers )
        {
            if ( Enum.TryParse( key, true, out HttpRequestHeader header ) ) { request.SetHeader( header, value ); }
            else { request.SetHeader( key,                                                               value ); }
        }
    }
}
