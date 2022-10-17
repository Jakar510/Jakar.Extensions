namespace Jakar.Extensions.Hosting;


public static class CommonUrl
{
    public static readonly Uri Listen_443 = new("http://0.0.0.0:443");
    public static readonly Uri Listen_80  = new("http://0.0.0.0:80");


    public static readonly Uri Listen_443S = new("https://0.0.0.0:443");
    public static readonly Uri Listen_80S  = new("https://0.0.0.0:80");


    public static string GetListen( int port ) => port.IsValidPort()
                                                      ? $"http://0.0.0.0:{port}"
                                                      : throw new ArgumentException( $"{nameof(port)} is not a valid port" );
    public static string GetListenSecure( int port ) => port.IsValidPort()
                                                            ? $"https://0.0.0.0:{port}"
                                                            : throw new ArgumentException( $"{nameof(port)} is not a valid port" );
    public static Uri GetListenSecureUri( int port ) => new(GetListenSecure( port ));


    public static Uri GetListenUri( int port ) => new(GetListen( port ));
    public static string GetLocalHost( int port ) => port.IsValidPort()
                                                         ? $"http://localhost:{port}"
                                                         : throw new ArgumentException( $"{nameof(port)} is not a valid port" );
    public static string GetLocalHostSecure( int port ) => port.IsValidPort()
                                                               ? $"https://localhost:{port}"
                                                               : throw new ArgumentException( $"{nameof(port)} is not a valid port" );
    public static Uri GetLocalHostSecureUri( int port ) => new(GetLocalHostSecure( port ));
    public static Uri GetLocalHostUri( int       port ) => new(GetLocalHost( port ));
}
