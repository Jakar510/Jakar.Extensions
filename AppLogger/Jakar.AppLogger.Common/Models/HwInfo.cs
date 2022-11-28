// Jakar.AppLogger :: Jakar.AppLogger.Common
// 09/09/2022  9:31 AM

namespace Jakar.AppLogger.Common;


/// <summary>
///     <see cref = "HwInfo" />
///     can only be used on Windows, MacOS or Linux
/// </summary>
[SuppressMessage( "ReSharper", "HeuristicUnreachableCode" )]
public record HwInfo
{
    public List<HwBattery>   BatteryList     { get; set; }
    public List<BIOS>        BiosList        { get; set; }
    public List<CPU>         CpuList         { get; set; }
    public List<Drive>       DriveList       { get; set; }
    public List<HwKeyboard>  KeyboardList    { get; set; }
    public List<Memory>      MemoryList      { get; init; }
    public List<HwMonitor>   MonitorList     { get; set; }
    public List<Motherboard> MotherboardList { get; set; }
    public List<Mouse>       MouseList       { get; set; }
    public List<Printer>     PrinterList     { get; set; }
    public List<SoundDevice> SoundDeviceList { get; set; }

    public List<VideoController> VideoControllerList { get; set; }

    // public List<NetworkAdapter> NetworkAdapterList { get; set; } 
    public MemoryStatus MemoryStatus { get; set; }


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


    // ReSharper disable once ReturnTypeCanBeNotNullable
    public static HwInfo? Create()
    {
    #if __LINUX__ || __WINDOWS__ || __MACOS__
        var hardware = new HardwareInfo();
        return new HwInfo( hardware );
    #else
        return default;
    #endif
    }

    public string? DeviceID()
    {
        CPU?         cpu         = CpuList.FirstOrDefault();
        Motherboard? motherboard = MotherboardList.FirstOrDefault();

        if (cpu is not null && motherboard is not null) { return $"{cpu.ProcessorId}|{motherboard.SerialNumber}"; }

        if (motherboard is not null) { return $"{motherboard.SerialNumber}"; }

        if (cpu is not null) { return $"{cpu.ProcessorId}"; }

        return default;
    }
}
