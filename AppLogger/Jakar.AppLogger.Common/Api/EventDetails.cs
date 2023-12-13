// Jakar.Extensions :: Jakar.AppLogger.Common
// 12/05/2023  4:41 PM

namespace Jakar.AppLogger.Common;


public class EventDetails : Dictionary<string, string?>
{
    public EventDetails() : base( 20 ) { }
    public EventDetails( IDictionary<string, string?> dictionary ) : base( dictionary ) { }
}
