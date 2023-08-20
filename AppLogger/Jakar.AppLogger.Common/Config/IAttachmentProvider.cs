// Jakar.AppLogger :: Jakar.AppLogger.Common
// 09/09/2022  6:36 PM

namespace Jakar.AppLogger.Common;


public interface ILoggerAttachmentProvider : IAsyncDisposable
{
    public LoggerAttachment GetLoggerAttachment();
}
