// Jakar.AppLogger :: Jakar.AppLogger.Common
// 09/09/2022  9:31 AM

namespace Jakar.AppLogger.Common;


/// <summary> <see cref="HwInfo"/> can only be used on Windows, MacOS or Linux </summary>
[SuppressMessage( "ReSharper", "HeuristicUnreachableCode" )]
public record HwInfo : BaseRecord
{
    private string? _deviceID;

    // public List<NetworkAdapter> NetworkAdapterList { get; init; }  = new();
    public List<HwBattery>       BatteryList         { get; init; } = new();
    public List<BIOS>            BiosList            { get; init; } = new();
    public List<CPU>             CpuList             { get; init; } = new();
    public List<Drive>           DriveList           { get; init; } = new();
    public List<HwKeyboard>      KeyboardList        { get; init; } = new();
    public List<Memory>          MemoryList          { get; init; } = new();
    public MemoryStatus          MemoryStatus        { get; init; } = new();
    public List<HwMonitor>       MonitorList         { get; init; } = new();
    public List<Motherboard>     MotherboardList     { get; init; } = new();
    public List<Mouse>           MouseList           { get; init; } = new();
    public List<Printer>         PrinterList         { get; init; } = new();
    public List<SoundDevice>     SoundDeviceList     { get; init; } = new();
    public List<VideoController> VideoControllerList { get; init; } = new();


    public HwInfo() { }
    private HwInfo( IHardwareInfo hardware )
    {
        hardware.RefreshAll();

        MemoryList          = hardware.MemoryList;
        MemoryStatus        = hardware.MemoryStatus;
        BatteryList         = hardware.BatteryList;
        BiosList            = hardware.BiosList;
        CpuList             = hardware.CpuList;
        DriveList           = hardware.DriveList;
        KeyboardList        = hardware.KeyboardList;
        MemoryStatus        = hardware.MemoryStatus;
        MonitorList         = hardware.MonitorList;
        MotherboardList     = hardware.MotherboardList;
        MouseList           = hardware.MouseList;
        PrinterList         = hardware.PrinterList;
        SoundDeviceList     = hardware.SoundDeviceList;
        VideoControllerList = hardware.VideoControllerList;
    }


    public static HwInfo Create()
    {
        var hardware = new HardwareInfo();
        return new HwInfo( hardware );
    }
    public static HwInfo? TryCreate()
    {
        try { return Create(); }
        catch ( Exception ) { return default; }
    }


    public string DeviceID( string? defaultDeviceID = default )
    {
        if ( _deviceID is not null ) { return _deviceID; }

        string? cpu = CpuList.FirstOrDefault()
                            ?.ProcessorId;

        string? motherboard = MotherboardList.FirstOrDefault()
                                            ?.SerialNumber;


        _deviceID = cpu is not null && motherboard is not null
                        ? $"{cpu}|{motherboard}"
                        : motherboard is not null
                            ? $"{motherboard}"
                            : cpu is not null
                                ? $"{cpu}"
                                : defaultDeviceID ?? throw new ArgumentNullException( nameof(defaultDeviceID) );

        return _deviceID;
    }
}
