// Jakar.Extensions :: Jakar.Extensions
// 01/05/2025  13:01

using System.Text;
using Microsoft.Extensions.Logging;
using Serilog.Context;



namespace Jakar.Extensions.Serilog;


[SuppressMessage( "ReSharper", "CollectionNeverQueried.Local" )]
public static class AppLoggers
{
    public static void TrackError<T>( this T _, Exception                 exception, IEnumerable<FileData>?    attachments, [CallerMemberName] string caller                                        = BaseRecord.EMPTY ) => TrackError( _, exception, ISerilogger.Instance?.AppState(), attachments, caller );
    public static void TrackError<T>( this T _, Exception                 exception, EventDetails?             details,     IEnumerable<FileData>?    attachments, [CallerMemberName] string caller = BaseRecord.EMPTY ) => ISerilogger.Instance?.TrackError( _, exception, details, attachments, caller );
    public static void TrackError<T>( this T _, Exception                 exception, [CallerMemberName] string caller = BaseRecord.EMPTY ) => ISerilogger.Instance?.TrackError( _, exception, caller );
    public static void TrackEvent<T>( this T _, [CallerMemberName] string eventType                                                                          = BaseRecord.EMPTY ) => ISerilogger.Instance?.TrackEvent( _, eventType );
    public static void TrackEvent<T>( this T _, EventDetails              properties, [CallerMemberName] string eventType                                    = BaseRecord.EMPTY ) => ISerilogger.Instance?.TrackEvent( _, properties, eventType );
    public static void TrackEvent<T>( this T _, string                    eventType,  EventDetails?             properties, [CallerMemberName] string caller = BaseRecord.EMPTY ) => ISerilogger.Instance?.TrackEvent( _, eventType,  properties, caller );


    [RequiresUnreferencedCode( "Metadata for the method might be incomplete or removed" )]
    public static void Details<T>( Exception e, T dict )
        where T : class, IDictionary<string, string?>
    {
        dict[nameof(Type)]                                = e.GetType().FullName;
        dict[nameof(e.Source)]                            = e.Source;
        dict[nameof(e.Message)]                           = e.Message;
        dict[nameof(e.StackTrace)]                        = e.StackTrace;
        dict[nameof(Exception.HelpLink)]                  = e.HelpLink;
        dict[nameof(ExceptionExtensions.MethodSignature)] = e.MethodSignature();
        dict[nameof(e.ToString)]                          = e.ToString();
    }


    public static string ToStringFast( this LogLevel level ) => level switch
                                                                {
                                                                    LogLevel.Trace       => nameof(LogLevel.Trace),
                                                                    LogLevel.Debug       => nameof(LogLevel.Debug),
                                                                    LogLevel.Information => nameof(LogLevel.Information),
                                                                    LogLevel.Warning     => nameof(LogLevel.Warning),
                                                                    LogLevel.Error       => nameof(LogLevel.Error),
                                                                    LogLevel.Critical    => nameof(LogLevel.Critical),
                                                                    LogLevel.None        => nameof(LogLevel.None),
                                                                    _                    => throw new OutOfRangeException( level )
                                                                };
    public static LogLevel ToLogLevel( this LogEventLevel level ) => level switch
                                                                     {
                                                                         LogEventLevel.Verbose     => (LogLevel.Trace),
                                                                         LogEventLevel.Debug       => (LogLevel.Debug),
                                                                         LogEventLevel.Information => (LogLevel.Information),
                                                                         LogEventLevel.Warning     => (LogLevel.Warning),
                                                                         LogEventLevel.Error       => (LogLevel.Error),
                                                                         LogEventLevel.Fatal       => (LogLevel.Critical),
                                                                         _                         => throw new OutOfRangeException( level )
                                                                     };
    public static LogEventLevel ToLogEventLevel( this LogLevel level ) => level switch
                                                                          {
                                                                              LogLevel.Trace       => LogEventLevel.Verbose,
                                                                              LogLevel.Debug       => LogEventLevel.Debug,
                                                                              LogLevel.Information => LogEventLevel.Information,
                                                                              LogLevel.Warning     => LogEventLevel.Warning,
                                                                              LogLevel.Error       => LogEventLevel.Error,
                                                                              LogLevel.Critical    => LogEventLevel.Fatal,
                                                                              LogLevel.None        => LogEventLevel.Verbose,
                                                                              _                    => throw new OutOfRangeException( level )
                                                                          };
    public static string ToStringFast( this LogEventLevel level ) => level switch
                                                                     {
                                                                         LogEventLevel.Verbose     => nameof(LogEventLevel.Verbose),
                                                                         LogEventLevel.Debug       => nameof(LogEventLevel.Debug),
                                                                         LogEventLevel.Information => nameof(LogEventLevel.Information),
                                                                         LogEventLevel.Warning     => nameof(LogEventLevel.Warning),
                                                                         LogEventLevel.Error       => nameof(LogEventLevel.Error),
                                                                         LogEventLevel.Fatal       => nameof(LogEventLevel.Fatal),
                                                                         _                         => throw new OutOfRangeException( level )
                                                                     };


    public static IDisposable AddFileToLogContext( this FileData file )
    {
        ArgumentNullException.ThrowIfNull( file.MetaData );
        ArgumentException.ThrowIfNullOrWhiteSpace( file.MetaData.FileName );

        return file.FileSize > 0
                   ? LogContext.PushProperty( file.MetaData.FileName, file.ToPrettyJson() )
                   : NullScope.Instance;
    }
    public static FileData GetAttachment<T>( this T value, string fileName )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace( fileName );
        FileMetaData data = FileMetaData.Create( fileName, MimeType.Json );
        return FileData.Create( data, value?.ToPrettyJson() ?? string.Empty, Encoding.Default );
    }
    public static FileData GetAttachment( this EventDetails value, string fileName )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace( fileName );
        FileMetaData data = FileMetaData.Create( fileName, MimeType.Json );
        return FileData.Create( data, value.ToPrettyJson(), Encoding.Default );
    }
    public static FileData GetAttachment( this string text, string fileName )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace( fileName );
        FileMetaData data = FileMetaData.Create( fileName, MimeType.PlainText );
        return FileData.Create( data, text, Encoding.Default );
    }
    public static FileData GetAttachment( this byte[] bytes, string fileName, string contentType )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace( fileName );
        FileMetaData data = new(fileName, contentType, contentType.ToMimeType());
        return FileData.Create( data, bytes );
    }
    public static FileData GetAttachment( this ref readonly ReadOnlyMemory<byte> bytes, string fileName, string contentType )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace( fileName );
        FileMetaData data = new(fileName, contentType, contentType.ToMimeType());
        return FileData.Create( data, bytes.Span );
    }
    public static FileData GetAttachment( this byte[] bytes, string fileName, MimeType mime )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace( fileName );
        FileMetaData data = FileMetaData.Create( fileName, mime );
        return FileData.Create( data, bytes );
    }
    public static FileData GetAttachment( this ref readonly ReadOnlyMemory<byte> bytes, string fileName, MimeType mime )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace( fileName );
        FileMetaData data = FileMetaData.Create( fileName, mime );
        return FileData.Create( data, bytes.Span );
    }
}
