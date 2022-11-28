// Jakar.AppLogger :: Jakar.AppLogger.Common
// 08/06/2022  7:59 PM

namespace Jakar.AppLogger.Common;


public interface ILogDetails
{
    DeviceDescriptor? Device    { get; }
    ExceptionDetails? Exception { get; }

    // ErrorDetails.Collection? Details   { get; }
}
