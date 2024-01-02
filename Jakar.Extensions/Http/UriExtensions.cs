namespace Jakar.Extensions;


public static class UriExtensions // TODO: use spans -- Parameterize
{
    public static string Parameterize( this IEnumerable<string>          types, IDictionary<string, object?> parameters ) => types.Aggregate( "", Parameterize ) + parameters.Parameterize();
    public static string Parameterize( this IEnumerable<string>          types )      => types.Aggregate( "", Parameterize );
    public static string Parameterize( this IDictionary<string, object?> parameters ) => parameters.Aggregate( "?", Parameterize );
    private static string Parameterize( this string previous, KeyValuePair<string, object?> pair )
    {
        (string? key, object? value) = pair;

        string? s = value?.ToString();

        if ( string.IsNullOrWhiteSpace( key ) || string.IsNullOrWhiteSpace( s ) || value is not null && value.GetType().Name == s ) { return previous; }

        return previous + $"{key}={s},";
    }
    private static string Parameterize( this string current, string element ) => string.IsNullOrWhiteSpace( element )
                                                                                     ? current
                                                                                     : $"{current}/{element}";
    public static Uri GetRoute( this string baseUri, params string[] parameters ) => new Uri( baseUri, UriKind.Absolute ).GetRoute( parameters );
    public static Uri GetRoute( this Uri baseUri, params string[] parameters )
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


    public static Uri GetRoute( this string baseUri, IDictionary<string, object?> parameters, params string[] paths ) => new Uri( baseUri, UriKind.Absolute ).GetRoute( parameters, paths );
    public static Uri GetRoute( this Uri baseUri, IDictionary<string, object?> parameters, params string[] paths )
    {
        if ( baseUri is null ) { throw new ArgumentNullException( nameof(baseUri) ); }

        return parameters.Count <= 0
                   ? baseUri
                   : new Uri( baseUri, paths.Parameterize( parameters ) );
    }
}
