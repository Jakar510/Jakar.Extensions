#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public static class ShellExtensions
{
    public static string GetPath( bool root, params Type[] types ) => types.Parameterize( root );
    public static string GetPath( this object type, bool root, IDictionary<string, object>? parameters = null ) => type.GetType()
                                                                                                                       .GetPath( root, parameters );
    public static string GetPath( this Type type, bool root, IDictionary<string, object>? parameters = null ) => type.Name.GetPath( root, parameters );

    public static string GetPath( this string type, bool root, IDictionary<string, object>? parameters = null )
    {
        if (parameters is null) { return type; }

        string result = type + parameters.Parameterize();

        return root
                   ? $"//{result}"
                   : result;
    }

    public static string GetPath( this IEnumerable<Type> types, bool root, IDictionary<string, object> parameters ) =>
        types.Aggregate( root
                             ? "//"
                             : "",
                         Parameterize ) + parameters.Parameterize();
    public static async Task GoToAsync( this Shell shell, bool root, params Type[] types ) =>
        await shell.GoToAsync( GetPath( root, types ) )
                   .ConfigureAwait( false );

    public static async Task GoToAsync( this Shell shell, bool root, IDictionary<string, object> parameters, params Type[] types ) =>
        await shell.GoToAsync( types.GetPath( root, parameters ) )
                   .ConfigureAwait( false );

    public static string Parameterize( this IEnumerable<Type> types, bool root ) =>
        types.Aggregate( root
                             ? "//"
                             : "",
                         Parameterize );


    private static string Parameterize( string previous, Type type ) => previous + $"/{type.Name}";

    public static string Parameterize( this IDictionary<string, object> parameters ) => parameters.Aggregate( "?", Parameterize );

    private static string Parameterize( string previous, KeyValuePair<string, object> pair )
    {
        (string? key, object? value) = pair;

        return previous + $"{key}={value},";
    }
}
