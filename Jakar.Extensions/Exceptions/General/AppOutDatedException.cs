// Jakar.Extensions :: Jakar.Extensions
// 09/22/2022  5:36 PM

namespace Jakar.Extensions;


public sealed class AppOutDatedException : Exception
{
    public AppOutDatedException() { }
    public AppOutDatedException( AppVersion version ) : base( version.ToString() ) { }
    public AppOutDatedException( string     message ) : base( message ) { }
    public AppOutDatedException( string     message, Exception e ) : base( message, e ) { }
}
