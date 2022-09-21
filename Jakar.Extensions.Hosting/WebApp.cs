namespace Jakar.Extensions.Hosting;


#nullable enable
#if NET6_0



[SuppressMessage("ReSharper", "RedundantLambdaParameterType")]
[SuppressMessage("ReSharper", "ParameterTypeCanBeEnumerable.Global")]
public static class WebApp
{
    public static void ParsePort( this string[] args, in ReadOnlySpan<char> key, in int defaultPort, out int port )
    {
        port = defaultPort;

        foreach ( ReadOnlySpan<char> arg in args )
        {
            if ( arg.StartsWith(key) )
            {
                port = int.Parse(arg[key.Length..]);
                return;
            }
        }
    }


    public static bool CheckArgs<TEnum>( this string[] args, in string version, string message, params string[] enumTriggers ) where TEnum : struct, Enum
    {
        if ( args.Any(enumTriggers.Contains) )
        {
            Console.WriteLine(message);
            foreach ( string name in Enum.GetNames<TEnum>() ) { Console.WriteLine(name); }

            return true;
        }


        if ( args.Any(x => x is "--version" or "-V") )
        {
            Console.WriteLine(version);

            return true;
        }

        return false;
    }
    public static void CreateUrls( this string[] args, in int http, in int https, out bool useSLL, out string[] urls, string key = "--ssl" )
    {
        if ( args.Contains(key) )
        {
            urls = new[]
                   {
                       $"http://0.0.0.0:{http}",
                       $"http://localhost:{http}",
                       $"https://0.0.0.0:{https}",
                       $"https://localhost:{https}"
                   };

            useSLL = true;
        }
        else
        {
            urls = new[]
                   {
                       $"http://0.0.0.0:{http}",
                       $"http://localhost:{http}",
                   };

            useSLL = false;
        }
    }


    public static WebApplication UseUrls( this WebApplication app, params string[] urls )
    {
        foreach ( string url in urls ) { app.Urls.Add(url); }

        return app;
    }
}



#endif
