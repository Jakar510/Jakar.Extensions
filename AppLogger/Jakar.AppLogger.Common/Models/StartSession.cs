// Jakar.AppLogger :: Jakar.AppLogger.Common
// 09/09/2022  6:12 PM

namespace Jakar.AppLogger.Common;


[Serializable]
public sealed class StartSession : BaseClass
{
    [JsonProperty( Required = Required.Always )] public string           AppLoggerSecret { get; init; } = string.Empty;
    [JsonProperty( Required = Required.Always )] public DeviceDescriptor Device          { get; init; } = new();
}
