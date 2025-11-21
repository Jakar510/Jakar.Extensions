namespace Jakar.Extensions;


public static class UriExtensions
{
    extension( ReadOnlySpan<string> types )
    {
        public string Parameterize( IDictionary<string, object?> parameters )
        {
            StringBuilder sb = new(types.Sum(static x => x.Length) + parameters.Keys.Sum(static x => x.Length));
            sb.Parameterize(types);
            sb.Parameterize(parameters);
            return sb.ToString();
        }
        public string Parameterize()
        {
            StringBuilder sb = new(types.Sum(static x => x.Length));
            sb.Parameterize(types);
            return sb.ToString();
        }
    }



    extension( StringBuilder sb )
    {
        private void Parameterize( params ReadOnlySpan<string> values )
        {
            foreach ( string value in values ) { sb.Parameterize(value); }
        }
        private void Parameterize( string element )
        {
            if ( string.IsNullOrWhiteSpace(element) ) { return; }

            sb.Append('/');
            sb.Append(element);
        }
    }



    public static string Parameterize( this IDictionary<string, object?> parameters )
    {
        StringBuilder sb = new(parameters.Keys.Sum(static x => x.Length));
        sb.Parameterize(parameters);
        return sb.ToString();
    }
    extension( StringBuilder sb )
    {
        public void Parameterize( IDictionary<string, object?> parameters )
        {
            sb.Append('?');
            foreach ( KeyValuePair<string, object?> pair in parameters ) { sb.Parameterize(pair); }
        }
        private void Parameterize( KeyValuePair<string, object?> pair )
        {
            ( string? key, object? value ) = pair;
            string? s = value?.ToString();

            if ( string.IsNullOrWhiteSpace(key) ||
                 string.IsNullOrWhiteSpace(s)   ||
                 ( value is not null &&
                   value.GetType()
                        .Name ==
                   s ) ) { return; }

            sb.Append(key);
            sb.Append('=');
            sb.Append(s);
            sb.Append(',');
        }
    }



    public static Uri GetRoute( this string baseUri, params ReadOnlySpan<string> parameters ) => new Uri(baseUri, UriKind.Absolute).GetRoute(parameters);
    public static Uri GetRoute( this Uri baseUri, params ReadOnlySpan<string> parameters )
    {
        if ( baseUri is null ) { throw new ArgumentNullException(nameof(baseUri)); }

        return parameters.Length <= 0
                   ? baseUri
                   : new Uri(baseUri, parameters.Parameterize());
    }

    public static Uri GetRoute( this string baseUri, IDictionary<string, object?> parameters ) => new Uri(baseUri, UriKind.Absolute).GetRoute(parameters);
    public static Uri GetRoute( this Uri baseUri, IDictionary<string, object?> parameters )
    {
        if ( baseUri is null ) { throw new ArgumentNullException(nameof(baseUri)); }

        return parameters.Count <= 0
                   ? baseUri
                   : new Uri(baseUri, parameters.Parameterize());
    }


    public static Uri GetRoute( this string baseUri, IDictionary<string, object?> parameters, params ReadOnlySpan<string> paths ) => new Uri(baseUri, UriKind.Absolute).GetRoute(parameters, paths);
    public static Uri GetRoute( this Uri baseUri, IDictionary<string, object?> parameters, params ReadOnlySpan<string> paths )
    {
        if ( baseUri is null ) { throw new ArgumentNullException(nameof(baseUri)); }

        return parameters.Count <= 0
                   ? baseUri
                   : new Uri(baseUri, paths.Parameterize(parameters));
    }
}
