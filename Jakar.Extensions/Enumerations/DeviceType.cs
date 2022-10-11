namespace Jakar.Extensions;


/// <Docs>
///     <summary> Various types of devices. </summary>
///     <remarks />
/// </Docs>
public enum DeviceType
{
    /// <Docs>
    ///     <summary> An unknown device type. </summary>
    /// </Docs>
    Unknown = 0,
    /// <Docs>
    ///     <summary> The device is a physical device, such as an iPhone, Android tablet or Windows desktop. </summary>
    /// </Docs>
    Physical,
    /// <Docs>
    ///     <summary> The device is virtual, such as the iOS simulators, Android emulators or Windows emulators. </summary>
    /// </Docs>
    Virtual
}
