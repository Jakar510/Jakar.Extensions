// Jakar.Extensions :: Jakar.Extensions.Serilog
// 02/11/2025  16:02

using Microsoft.Extensions.Options;



namespace Jakar.Extensions.Serilog;


public sealed class SeriloggerOptions : IOptions<SeriloggerOptions>
{
    public          Func<EventDetails, EventDetails>                         UpdateEventDetails { get; set; } = UpdateEventDetailsNoOpp;
    public          Func<CancellationToken, ValueTask<ReadOnlyMemory<byte>>> TakeScreenShot     { get; set; } = TakeEmptyScreenShot;
    public          Action<LoggerConfiguration>?                             AddNativeLogs      { get; set; }
    public required IFilePaths                                               Paths              { get; set; }
    public          ActivitySource?                                          ActivitySource     { get; set; }
    SeriloggerOptions IOptions<SeriloggerOptions>.                           Value              => this;
    public Uri?                                                              RemoteLogServer    { get; set; }
    public Uri?                                                              SeqLogServer       { get; set; }
    public string?                                                           SeqApiKey          { get; set; }


    public static EventDetails                    UpdateEventDetailsNoOpp( EventDetails  details ) => details;
    public static ValueTask<ReadOnlyMemory<byte>> TakeEmptyScreenShot( CancellationToken token )   => new(ReadOnlyMemory<byte>.Empty);
}
