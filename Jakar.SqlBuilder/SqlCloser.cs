// Jakar.Extensions :: Jakar.SqlBuilder
// 08/20/2024  20:08

namespace Jakar.SqlBuilder;


public struct SqlCloser( ref EasySqlBuilder builder, char start, char end ) : IDisposable
{
    private          EasySqlBuilder __builder = builder.Add( start );
    private readonly char           __end     = end;
    public           void           Dispose() => __builder.Add( __end );
}
