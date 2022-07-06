// Jakar.Extensions :: Jakar.AppLogger
// 07/06/2022  2:18 PM

namespace Jakar.AppLogger;


public class AppLoggerException : AggregateException
{
    public AppLoggerException() : base() { }
    public AppLoggerException( string                 message ) : base(message) { }
    public AppLoggerException( string                 message, Exception              inner ) : base(message, inner) { }
    public AppLoggerException( string                 message, IEnumerable<Exception> inner ) : base(message, inner) { }
    public AppLoggerException( string                 message, params Exception[]     inner ) : base(message, inner) { }
    public AppLoggerException( Exception              inner ) : base(inner) { }
    public AppLoggerException( IEnumerable<Exception> inner ) : base(inner) { }
    public AppLoggerException( params Exception[]     inner ) : base(inner) { }
}
