namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "InconsistentNaming")]
[Flags]
public enum DevicePlatform : ulong
{
    Unknown     = 0,
    Android     = 1 << 0,
    iOS         = 1 << 1,
    MacOS       = 1 << 2,
    MacCatalyst = 1 << 3,
    TvOS        = 1 << 4,
    Tizen       = 1 << 5,
    UWP         = 1 << 6,
    WinUI       = 1 << 7,
    WatchOS     = 1 << 8,
    WPF         = 1 << 9,
    Xamarin     = 1 << 10,
    Maui        = 1 << 11,
    Linux       = 1 << 12
}
