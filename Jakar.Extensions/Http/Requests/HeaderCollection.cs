namespace Jakar.Extensions;


/// <summary>
///     HTTP headers to apply to a request (see <see cref="Merge(HttpRequestMessage)"/>). Keys are real header names (<c>Content-Type</c>, not <c>ContentType</c>) and are case-insensitive.
///     Values may be a <see cref="string"/>, an <see cref="IEnumerable{T}"/> of strings, or any other object (formatted with the invariant culture).
/// </summary>
public class HeaderCollection : Dictionary<string, object>
{
    public const string CONTENT_ENCODING = "Content-Encoding";
    public const string CONTENT_TYPE     = "Content-Type";


    /// <summary> <c>Content-Encoding</c>: the content coding of the body (e.g. <c>gzip</c>, <c>br</c>), not the character set — see <see cref="Encoding"/> for that. </summary>
    public string? ContentEncoding
    {
        get => TryGetValue(CONTENT_ENCODING, out object? value)
                   ? Format(value)
                   : null;
        set => this[CONTENT_ENCODING] = value ?? EMPTY;
    }
    /// <summary> <c>Content-Type</c>, including any <c>charset</c> parameter. </summary>
    public string? ContentType
    {
        get => TryGetValue(CONTENT_TYPE, out object? value)
                   ? Format(value)
                   : null;
        set => this[CONTENT_TYPE] = value ?? EMPTY;
    }
    /// <summary> The <c>charset</c> parameter of <see cref="ContentType"/> (set on <c>text/plain</c> if no content type has been set yet); <see langword="null"/> if absent or unknown. </summary>
    public Encoding? Encoding
    {
        get
        {
            if ( !MediaTypeHeaderValue.TryParse(ContentType, out MediaTypeHeaderValue? mediaType) || string.IsNullOrWhiteSpace(mediaType.CharSet) ) { return null; }

            try { return Encoding.GetEncoding(mediaType.CharSet.Trim('"')); }
            catch ( ArgumentException ) { return null; }
        }
        set
        {
            ArgumentNullException.ThrowIfNull(value);

            MediaTypeHeaderValue mediaType = MediaTypeHeaderValue.TryParse(ContentType, out MediaTypeHeaderValue? parsed)
                                                 ? parsed
                                                 : new MediaTypeHeaderValue(MimeTypeNames.Text.PLAIN);

            mediaType.CharSet = value.WebName;
            ContentType       = mediaType.ToString();
        }
    }


    public HeaderCollection() : base(StringComparer.OrdinalIgnoreCase) { }
    public HeaderCollection( MimeType contentType ) : this(contentType.ToContentType()) { }
    public HeaderCollection( string   contentType ) : this() => ContentType = contentType;
    public HeaderCollection( MimeType contentType, Encoding encoding ) : this(contentType.ToContentType(), encoding) { }
    public HeaderCollection( string contentType, Encoding encoding ) : this()
    {
        ContentType = contentType;
        Encoding    = encoding;
    }


    public HeaderCollection Add( HttpRequestHeader header, object value ) => Add(header.GetName(), value);
    public new HeaderCollection Add( string header, object value )
    {
        if ( string.IsNullOrWhiteSpace(header) ) { throw new ArgumentNullException(nameof(header)); }

        base.Add(header, value);
        return this;
    }
    public HeaderCollection Add( IDictionary<string, object> headers )
    {
        foreach ( ( string key, object value ) in headers ) { Add(key, value); }

        return this;
    }
    public HeaderCollection Add( IDictionary<HttpRequestHeader, object> headers )
    {
        foreach ( KeyValuePair<HttpRequestHeader, object> pair in headers ) { Add(pair); }

        return this;
    }
    public HeaderCollection Add( KeyValuePair<HttpRequestHeader, object> pair )
    {
        ( HttpRequestHeader key, object value ) = pair;
        Add(key, value);
        return this;
    }
    public HeaderCollection Add( KeyValuePair<string, object> pair ) => Add(pair.Key, pair.Value);


    /// <summary> Copies <paramref name="headers"/> into this collection, replacing existing keys. </summary>
    public HeaderCollection Merge( HeaderCollection headers )
    {
        foreach ( ( string key, object value ) in headers ) { this[key] = value; }

        return this;
    }
    /// <summary> Applies these headers to <paramref name="request"/>: content headers (e.g. <c>Content-Type</c>) go to <see cref="HttpRequestMessage.Content"/> when it has content, all others to <see cref="HttpRequestMessage.Headers"/>. Existing values are replaced. </summary>
    public HeaderCollection Merge( HttpRequestMessage request )
    {
        foreach ( ( string key, object value ) in this )
        {
            if ( !IsContentHeader(key) ) { Set(request.Headers, key, value); }
            else if ( request.Content is not null ) { Set(request.Content.Headers, key, value); }
        }

        return this;
    }
    /// <summary> Applies these headers to <paramref name="headers"/>, replacing existing values. Headers that don't belong to that collection (e.g. <c>Content-Type</c> on request headers) are skipped. </summary>
    public HeaderCollection Merge( HttpHeaders headers )
    {
        foreach ( ( string key, object value ) in this ) { Set(headers, key, value); }

        return this;
    }
    /// <summary> Applies these headers to <paramref name="headers"/>, replacing existing values. Non-content headers are skipped. </summary>
    public HeaderCollection Merge( HttpContentHeaders headers ) => Merge((HttpHeaders)headers);


    /// <summary> Headers that belong to <see cref="HttpContentHeaders"/> rather than request/response headers. </summary>
    private static readonly FrozenSet<string> __contentHeaders = new[]
                                                                 {
                                                                     "Allow", "Content-Disposition", CONTENT_ENCODING, "Content-Language", "Content-Length", "Content-Location", "Content-MD5", "Content-Range", CONTENT_TYPE, "Expires",
                                                                     "Last-Modified"
                                                                 }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);

    public static bool IsContentHeader( string name ) => __contentHeaders.Contains(name);


    /// <summary> Replaces <paramref name="key"/> in <paramref name="headers"/>; returns <see langword="false"/> (without throwing) when the header doesn't belong to that kind of collection. </summary>
    private static bool Set( HttpHeaders headers, string key, object value )
    {
        if ( IsContentHeader(key) != headers is HttpContentHeaders ) { return false; }

        headers.Remove(key);

        return value switch
               {
                   string text                  => headers.TryAddWithoutValidation(key, text),
                   IEnumerable<string> values   => headers.TryAddWithoutValidation(key, values),
                   _                            => headers.TryAddWithoutValidation(key, Format(value))
               };
    }
    private static string? Format( object? value ) => value switch
                                                      {
                                                          null                       => null,
                                                          string text                => text,
                                                          IEnumerable<string> values => string.Join(", ", values),
                                                          IFormattable formattable   => formattable.ToString(null, CultureInfo.InvariantCulture),
                                                          _                          => value.ToString()
                                                      };
}



public static class HttpRequestHeaderNames
{
    /// <summary> The wire name of <paramref name="header"/> (e.g. <see cref="HttpRequestHeader.ContentType"/> → <c>Content-Type</c>). </summary>
    public static string GetName( this HttpRequestHeader header ) => header switch
                                                                     {
                                                                         HttpRequestHeader.CacheControl       => "Cache-Control",
                                                                         HttpRequestHeader.Connection         => "Connection",
                                                                         HttpRequestHeader.Date               => "Date",
                                                                         HttpRequestHeader.KeepAlive          => "Keep-Alive",
                                                                         HttpRequestHeader.Pragma             => "Pragma",
                                                                         HttpRequestHeader.Trailer            => "Trailer",
                                                                         HttpRequestHeader.TransferEncoding   => "Transfer-Encoding",
                                                                         HttpRequestHeader.Upgrade            => "Upgrade",
                                                                         HttpRequestHeader.Via                => "Via",
                                                                         HttpRequestHeader.Warning            => "Warning",
                                                                         HttpRequestHeader.Allow              => "Allow",
                                                                         HttpRequestHeader.ContentLength      => "Content-Length",
                                                                         HttpRequestHeader.ContentType        => HeaderCollection.CONTENT_TYPE,
                                                                         HttpRequestHeader.ContentEncoding    => HeaderCollection.CONTENT_ENCODING,
                                                                         HttpRequestHeader.ContentLanguage    => "Content-Language",
                                                                         HttpRequestHeader.ContentLocation    => "Content-Location",
                                                                         HttpRequestHeader.ContentMd5         => "Content-MD5",
                                                                         HttpRequestHeader.ContentRange       => "Content-Range",
                                                                         HttpRequestHeader.Expires            => "Expires",
                                                                         HttpRequestHeader.LastModified       => "Last-Modified",
                                                                         HttpRequestHeader.Accept             => "Accept",
                                                                         HttpRequestHeader.AcceptCharset      => "Accept-Charset",
                                                                         HttpRequestHeader.AcceptEncoding     => "Accept-Encoding",
                                                                         HttpRequestHeader.AcceptLanguage     => "Accept-Language",
                                                                         HttpRequestHeader.Authorization      => "Authorization",
                                                                         HttpRequestHeader.Cookie             => "Cookie",
                                                                         HttpRequestHeader.Expect             => "Expect",
                                                                         HttpRequestHeader.From               => "From",
                                                                         HttpRequestHeader.Host               => "Host",
                                                                         HttpRequestHeader.IfMatch            => "If-Match",
                                                                         HttpRequestHeader.IfModifiedSince    => "If-Modified-Since",
                                                                         HttpRequestHeader.IfNoneMatch        => "If-None-Match",
                                                                         HttpRequestHeader.IfRange            => "If-Range",
                                                                         HttpRequestHeader.IfUnmodifiedSince  => "If-Unmodified-Since",
                                                                         HttpRequestHeader.MaxForwards        => "Max-Forwards",
                                                                         HttpRequestHeader.ProxyAuthorization => "Proxy-Authorization",
                                                                         HttpRequestHeader.Referer            => "Referer",
                                                                         HttpRequestHeader.Range              => "Range",
                                                                         HttpRequestHeader.Te                 => "TE",
                                                                         HttpRequestHeader.Translate          => "Translate",
                                                                         HttpRequestHeader.UserAgent          => "User-Agent",
                                                                         _                                    => header.ToString()
                                                                     };
}
