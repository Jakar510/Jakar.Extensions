// Jakar.Extensions :: Jakar.Database
// 09/15/2025  22:59

using Serilog.Configuration;
using Serilog.Formatting;



namespace Jakar.Database;


/// <summary> Write log events to a <a href="https://datalust.co/seq"> Seq </a> server. </summary>
/// <param name="ServerUrl"> The base URL of the Seq server that log events will be written to. </param>
/// <param name="RestrictedToMinimumLevel"> The minimum log event level required in order to write an event to the sink. </param>
/// <param name="BatchPostingLimit"> The maximum number of events to post in a single batch. </param>
/// <param name="Period"> The time to wait between checking for event batches. </param>
/// <param name="BufferBaseFilename">
///     Path for a set of files that will be used to buffer events until they can be successfully transmitted across the network. Individual files will be created using the pattern
///     <paramref
///         name="BufferBaseFilename"/>
///     *.json, which should not clash with any other filenames in the same directory.
/// </param>
/// <param name="APIKey"> A Seq <i> API key </i> that authenticates the client to the Seq server. </param>
/// <param name="BufferSizeLimitBytes"> The maximum amount of data, in bytes, to which the buffer log file for a specific date will be allowed to grow. By default no limit will be applied. </param>
/// <param name="EventBodyLimitBytes"> The maximum size, in bytes, that the JSON representation of an event may take before it is dropped rather than being sent to the Seq server. Specify null for no limit. The default is 265 KB. </param>
/// <param name="ControlLevelSwitch">
///     If provided, the switch will be updated based on the Seq server's level setting for the corresponding API key. Passing the same key to MinimumLevel.ControlledBy() will make the whole pipeline dynamically controlled. Do not specify
///     <paramref
///         name="RestrictedToMinimumLevel"/>
///     with this setting.
/// </param>
/// <param name="MessageHandler"> Used to construct the HttpClient that will send the log messages to Seq. </param>
/// <param name="RetainedInvalidPayloadsLimitBytes"> A soft limit for the number of bytes to use for storing failed requests. The limit is soft in that it can be exceeded by any single error payload, but in that case only that single error payload will be retained. </param>
/// <param name="QueueSizeLimit"> The maximum number of events that will be held in-memory while waiting to ship them to Seq. Beyond this limit, events will be dropped. The default is 100,000. Has no effect on durable log shipping. </param>
/// <param name="PayloadFormatter"> An <see cref="ITextFormatter"/> that will be used to format (newline-delimited CLEF/JSON) payloads. Experimental. </param>
/// <param name="FormatProvider">
///     An <see cref="IFormatProvider"/> that will be used to render log event tokens. Does not apply if <paramref name="PayloadFormatter"/> is provided. If <paramref name="FormatProvider"/> is provided then event messages will be rendered and included in the payload.
/// </param>
/// <returns> Logger configuration, allowing configuration to continue. </returns>
/// <exception cref="ArgumentNullException"> A required parameter is null. </exception>
public sealed record SeqConfig( string              ServerUrl,
                                LogEventLevel       RestrictedToMinimumLevel          = LevelAlias.Minimum,
                                int                 BatchPostingLimit                 = 1000,
                                TimeSpan?           Period                            = null,
                                string?             APIKey                            = null,
                                string?             BufferBaseFilename                = null,
                                long?               BufferSizeLimitBytes              = null,
                                long?               EventBodyLimitBytes               = 256 * 1024,
                                LoggingLevelSwitch? ControlLevelSwitch                = null,
                                HttpMessageHandler? MessageHandler                    = null,
                                long?               RetainedInvalidPayloadsLimitBytes = null,
                                int                 QueueSizeLimit                    = 100000,
                                ITextFormatter?     PayloadFormatter                  = null,
                                IFormatProvider?    FormatProvider                    = null )
{
    public void Configure( LoggerSinkConfiguration sinkTo )
    {
        sinkTo.Seq(ServerUrl,
                   RestrictedToMinimumLevel,
                   BatchPostingLimit,
                   Period,
                   APIKey,
                   BufferBaseFilename,
                   BufferSizeLimitBytes,
                   EventBodyLimitBytes,
                   ControlLevelSwitch,
                   MessageHandler,
                   RetainedInvalidPayloadsLimitBytes,
                   QueueSizeLimit,
                   PayloadFormatter,
                   FormatProvider);
    }
}
