namespace Jakar.Extensions;


public static class UriExtensions
{
    public static string Parameterize( this ReadOnlySpan<string> types, IDictionary<string, object?> parameters )
    {
        StringBuilder sb = new(types.Sum( static x => x.Length ) + parameters.Keys.Sum( static x => x.Length ));
        sb.Parameterize( types );
        sb.Parameterize( parameters );
        return sb.ToString();
    }


    public static string Parameterize( this ReadOnlySpan<string> types )
    {
        StringBuilder sb = new(types.Sum( static x => x.Length ));
        sb.Parameterize( types );
        return sb.ToString();
    }
    private static void Parameterize( this StringBuilder sb, params ReadOnlySpan<string> values )
    {
        foreach ( string value in values ) { sb.Parameterize( value ); }
    }
    private static void Parameterize( this StringBuilder sb, string element )
    {
        if ( string.IsNullOrWhiteSpace( element ) ) { return; }

        sb.Append( '/' );
        sb.Append( element );
    }


    public static string Parameterize( this IDictionary<string, object?> parameters )
    {
        StringBuilder sb = new(parameters.Keys.Sum( static x => x.Length ));
        sb.Parameterize( parameters );
        return sb.ToString();
    }
    public static void Parameterize( this StringBuilder sb, IDictionary<string, object?> parameters )
    {
        sb.Append( '?' );
        foreach ( KeyValuePair<string, object?> pair in parameters ) { sb.Parameterize( pair ); }
    }
    private static void Parameterize( this StringBuilder sb, KeyValuePair<string, object?> pair )
    {
        (string? key, object? value) = pair;
        string? s = value?.ToString();

        if ( string.IsNullOrWhiteSpace( key ) || string.IsNullOrWhiteSpace( s ) || ( value is not null && value.GetType().Name == s ) ) { return; }

        sb.Append( key );
        sb.Append( '=' );
        sb.Append( s );
        sb.Append( ',' );
    }


    public static Uri GetRoute( this string baseUri, params ReadOnlySpan<string> parameters ) => new Uri( baseUri, UriKind.Absolute ).GetRoute( parameters );
    public static Uri GetRoute( this Uri baseUri, params ReadOnlySpan<string> parameters )
    {
        if ( baseUri is null ) { throw new ArgumentNullException( nameof(baseUri) ); }

        return parameters.Length <= 0
                   ? baseUri
                   : new Uri( baseUri, parameters.Parameterize() );
    }

    public static Uri GetRoute( this string baseUri, IDictionary<string, object?> parameters ) => new Uri( baseUri, UriKind.Absolute ).GetRoute( parameters );
    public static Uri GetRoute( this Uri baseUri, IDictionary<string, object?> parameters )
    {
        if ( baseUri is null ) { throw new ArgumentNullException( nameof(baseUri) ); }

        return parameters.Count <= 0
                   ? baseUri
                   : new Uri( baseUri, parameters.Parameterize() );
    }


    public static Uri GetRoute( this string baseUri, IDictionary<string, object?> parameters, params ReadOnlySpan<string> paths ) => new Uri( baseUri, UriKind.Absolute ).GetRoute( parameters, paths );
    public static Uri GetRoute( this Uri baseUri, IDictionary<string, object?> parameters, params ReadOnlySpan<string> paths )
    {
        if ( baseUri is null ) { throw new ArgumentNullException( nameof(baseUri) ); }

        return parameters.Count <= 0
                   ? baseUri
                   : new Uri( baseUri, paths.Parameterize( parameters ) );
    }
}
