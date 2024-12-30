namespace Jakar.Extensions;


public class HeaderCollection : Dictionary<string, object>
{
    public string? ContentEncoding
    {
        get => TryGetValue( nameof(HttpRequestHeader.ContentEncoding), out object? value )
                   ? value.ToString()
                   : null;
        set => this[nameof(HttpRequestHeader.ContentEncoding)] = value ?? string.Empty;
    }
    public string? ContentType
    {
        get => TryGetValue( nameof(HttpRequestHeader.ContentType), out object? value )
                   ? value.ToString()
                   : null;
        set => this[nameof(HttpRequestHeader.ContentType)] = value ?? string.Empty;
    }
    public Encoding Encoding
    {
        set
        {
            ContentEncoding        = value.EncodingName;
            this[value.HeaderName] = value.WebName;
        }
    }


    public HeaderCollection() : base() { }
    public HeaderCollection( MimeType contentType ) : this( contentType.ToContentType() ) { }
    public HeaderCollection( string   contentType ) : this() => ContentType = contentType;
    public HeaderCollection( MimeType contentType, Encoding encoding ) : this( contentType.ToContentType(), encoding ) { }
    public HeaderCollection( string contentType, Encoding encoding ) : this()
    {
        ContentType = contentType;
        Encoding    = encoding;
    }


    public HeaderCollection Add( HttpRequestHeader header, object value ) => Add( header.ToString(), value );
    public new HeaderCollection Add( string header, object value )
    {
        if ( string.IsNullOrWhiteSpace( header ) ) { throw new ArgumentNullException( nameof(header) ); }

        base.Add( header, value );
        return this;
    }
    public HeaderCollection Add( IDictionary<string, object> headers )
    {
        foreach ( (string key, object value) in headers ) { Add( key, value ); }

        return this;
    }
    public HeaderCollection Add( IDictionary<HttpRequestHeader, object> headers )
    {
        foreach ( KeyValuePair<HttpRequestHeader, object> pair in headers ) { Add( pair ); }

        return this;
    }
    public HeaderCollection Add( KeyValuePair<HttpRequestHeader, object> pair )
    {
        (HttpRequestHeader key, object value) = pair;
        Add( key, value );
        return this;
    }
    public HeaderCollection Add( KeyValuePair<string, object> pair ) => Add( pair.Key, pair.Value );


    public HeaderCollection Merge( HeaderCollection headers )
    {
        foreach ( (string key, object value) in headers ) { this[key] = value; }

        return this;
    }
    public HeaderCollection Merge( HttpRequestMessage request ) => Merge( request.Headers );
    public HeaderCollection Merge( HttpHeaders headers )
    {
        foreach ( (string key, object value) in this )
        {
            switch ( value )
            {
                case string s:
                    headers.Add( key, s );
                    break;

                case IEnumerable<string> items:
                    headers.Add( key, items );
                    break;
            }
        }

        return this;
    }
    public HeaderCollection Merge( HttpContentHeaders headers )
    {
        foreach ( (string key, object value) in this )
        {
            switch ( value )
            {
                case string s:
                    headers.Add( key, s );
                    break;

                case IEnumerable<string> items:
                    headers.Add( key, items );
                    break;
            }
        }

        return this;
    }
}
