namespace Jakar.Extensions;


/// <summary>
///     Builds routes and query strings. Path segments are appended to the base URI's path (a segment may contain '/' to add several levels); keys, values and segments are percent-encoded;
///     pairs are joined with '&amp;'; values are formatted with the invariant culture. Pairs whose key or value is null/blank, or whose value has no meaningful <see cref="object.ToString"/>, are skipped.
///     The base URI's existing query and fragment are kept.
/// </summary>
public static class UriExtensions
{
    /// <summary> The query string for <paramref name="parameters"/> (<c>?a=1&amp;b=2</c>), or <see cref="string.Empty"/> when no pair is written. </summary>
    public static string Parameterize( this IDictionary<string, object?> parameters )
    {
        ValueStringBuilder sb = new(stackalloc char[256]);
        AppendQuery(ref sb, parameters, false);
        return sb.ToString();
    }


    public static Uri GetRoute( this string baseUri, params ReadOnlySpan<string> parameters ) => new Uri(baseUri, UriKind.Absolute).GetRoute(parameters);
    public static Uri GetRoute( this Uri    baseUri, params ReadOnlySpan<string> parameters ) => Combine(baseUri, parameters, null);

    public static Uri GetRoute( this string baseUri, IDictionary<string, object?> parameters ) => new Uri(baseUri, UriKind.Absolute).GetRoute(parameters);
    public static Uri GetRoute( this Uri    baseUri, IDictionary<string, object?> parameters ) => Combine(baseUri, default, parameters);

    public static Uri GetRoute( this string baseUri, IDictionary<string, object?> parameters, params ReadOnlySpan<string> paths ) => new Uri(baseUri, UriKind.Absolute).GetRoute(parameters, paths);
    public static Uri GetRoute( this Uri    baseUri, IDictionary<string, object?> parameters, params ReadOnlySpan<string> paths ) => Combine(baseUri, paths, parameters);



    extension( ReadOnlySpan<string> types )
    {
        /// <summary> <c>/segment/segment?a=1&amp;b=2</c> </summary>
        public string Parameterize( IDictionary<string, object?> parameters )
        {
            ValueStringBuilder sb = new(stackalloc char[256]);
            AppendSegments(ref sb, types);
            AppendQuery(ref sb, parameters, false);
            return sb.ToString();
        }
        /// <summary> <c>/segment/segment</c> </summary>
        public string Parameterize()
        {
            ValueStringBuilder sb = new(stackalloc char[256]);
            AppendSegments(ref sb, types);
            return sb.ToString();
        }
    }



    extension( StringBuilder sb )
    {
        /// <summary> Appends the query string for <paramref name="parameters"/> (<c>?a=1&amp;b=2</c>); nothing when no pair is written. </summary>
        public void Parameterize( IDictionary<string, object?> parameters )
        {
            ValueStringBuilder query = new(stackalloc char[256]);
            AppendQuery(ref query, parameters, false);
            sb.Append(query.Values);
            query.Dispose();
        }
    }



    private static Uri Combine( Uri baseUri, ReadOnlySpan<string> paths, IDictionary<string, object?>? parameters )
    {
        ArgumentNullException.ThrowIfNull(baseUri);
        if ( paths.IsEmpty && ( parameters is null || parameters.Count == 0 ) ) { return baseUri; }

        ValueStringBuilder sb   = new(stackalloc char[512]);
        string             path = baseUri.GetLeftPart(UriPartial.Path);

        sb.Append(paths.IsEmpty
                      ? path.AsSpan()
                      : MemoryExtensions.TrimEnd(path.AsSpan(), '/'));

        AppendSegments(ref sb, paths);

        string existingQuery = baseUri.Query;
        sb.Append(existingQuery);
        if ( parameters is not null ) { AppendQuery(ref sb, parameters, existingQuery.Length > 1); }

        sb.Append(baseUri.Fragment);
        return new Uri(sb.ToString(), UriKind.Absolute);
    }


    /// <summary> Appends each non-blank segment as <c>/escaped</c>; a segment containing '/' adds one level per part, each part escaped. </summary>
    private static void AppendSegments( ref ValueStringBuilder sb, ReadOnlySpan<string> segments )
    {
        foreach ( string? segment in segments )
        {
            if ( string.IsNullOrWhiteSpace(segment) ) { continue; }

            ReadOnlySpan<char> trimmed = MemoryExtensions.Trim(segment.AsSpan(), '/');

            foreach ( Range range in trimmed.Split('/') )
            {
                ReadOnlySpan<char> part = trimmed[range];
                if ( part.IsEmpty ) { continue; }

                sb.Append('/');
                AppendEscaped(ref sb, part);
            }
        }
    }


    /// <summary> Appends <c>key=value</c> pairs separated by '&amp;', starting with '?' (or '&amp;' when <paramref name="hasQuery"/>). </summary>
    private static void AppendQuery( ref ValueStringBuilder sb, IDictionary<string, object?> parameters, bool hasQuery )
    {
        bool first = true;

        foreach ( ( string? key, object? value ) in parameters )
        {
            if ( string.IsNullOrWhiteSpace(key) || !TryFormat(value, out string? text) ) { continue; }

            sb.Append(first && !hasQuery
                          ? '?'
                          : '&');

            AppendEscaped(ref sb, key);
            sb.Append('=');
            AppendEscaped(ref sb, text);
            first = false;
        }
    }


    private static bool TryFormat( object? value, [NotNullWhen(true)] out string? text )
    {
        text = value switch
               {
                   null              => null,
                   string s          => s,
                   IFormattable f    => f.ToString(null, CultureInfo.InvariantCulture),
                   _                 => value.ToString()
               };

        // Skip blanks and objects without a meaningful ToString (which returns the type name).
        if ( string.IsNullOrWhiteSpace(text) ) { return false; }

        Type type = value!.GetType();
        return text != type.Name && text != type.FullName;
    }


    private static void AppendEscaped( ref ValueStringBuilder sb, scoped ReadOnlySpan<char> value )
    {
        // Escaping expands each char to at most 3 ("%XX") for ASCII and 9 for a 3-byte UTF-8 char; 12 per char is a safe bound for surrogate pairs.
        int max = value.Length * 12;
        sb.EnsureCapacity(sb.Length + max);

        if ( Uri.TryEscapeDataString(value, sb.Next, out int written) ) { sb.Length += written; }
        else { sb.Append(Uri.EscapeDataString(value.ToString())); }
    }
}
