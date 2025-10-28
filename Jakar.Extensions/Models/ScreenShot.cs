// Jakar.Extensions :: Jakar.Extensions
// 08/13/2025  11:45

using Serilog.Core;
using Serilog.Events;



namespace Jakar.Extensions;


public sealed class ScreenShot( ReadOnlyMemory<byte> data ) : ILogEventEnricher
{
    public const    int                  MAX_SIZE    = 1024 * 1024 * 5;
    public readonly ReadOnlyMemory<byte> Data        = data;
    public readonly string               ReferenceID = CreateID(data.Span);
    public readonly string               Value       = Convert.ToBase64String(data.Span);
    private         LogEventProperty?    __property;


    public static bool      EnableLogging { get; set; }
    public static LocalFile File          => AppLoggerOptions.Current.Paths.Screenshot;
    public        bool      IsEmpty       => Data.IsEmpty;


    public static implicit operator ScreenShot( ReadOnlyMemory<byte> data ) => new(data);
    public static implicit operator ScreenShot( byte[]               data ) => new(data);


    public LogEventProperty ToProperty() => __property ??= new LogEventProperty(nameof(ReferenceID), new ScalarValue(ReferenceID));
    public void Enrich( LogEvent log, ILogEventPropertyFactory propertyFactory )
    {
        if ( EnableLogging ) { log.AddPropertyIfAbsent(ToProperty()); }
    }


    public static string CreateID( params ReadOnlySpan<byte> data ) => data.Hash()
                                                                           .ToString();
    public static string CreateID()
    {
        Span<byte> span = stackalloc byte[16];
        RandomNumberGenerator.Fill(span);
        return Convert.ToHexString(span);
    }


    public static ValueTask<ScreenShot?> Empty( CancellationToken token ) => new(default(ScreenShot));
}



public delegate ValueTask<ScreenShot?> TakeScreenShotAsync( CancellationToken token );
