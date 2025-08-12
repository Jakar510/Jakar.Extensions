// Jakar.Extensions :: Jakar.Extensions
// 07/29/2025  14:39

using Serilog.Core;
using Serilog.Events;



namespace Jakar.Extensions;


public interface IDeviceMetaData
{
    string? DeviceAppVersion   { get; set; }
    string? DeviceID           { get; set; }
    string? DeviceManufacturer { get; set; }
    string? DeviceModel        { get; set; }
    string? DevicePlatform     { get; set; }
    string? DeviceVersion      { get; set; }
}



public class DeviceMetaData : ObservableClass, IDeviceMetaData
{
    protected string? _deviceAppVersion;
    protected string? _deviceID;
    protected string? _deviceManufacturer;
    protected string? _deviceModel;
    protected string? _devicePlatform;
    protected string? _deviceVersion;
    protected string? _packageName;


    public virtual string? DeviceAppVersion   { get => _deviceAppVersion;   set => SetProperty(ref _deviceAppVersion,   value); }
    public virtual string? DeviceID           { get => _deviceID;           set => SetProperty(ref _deviceID,           value); }
    public virtual string? DeviceManufacturer { get => _deviceManufacturer; set => SetProperty(ref _deviceManufacturer, value); }
    public virtual string? DeviceModel        { get => _deviceModel;        set => SetProperty(ref _deviceModel,        value); }
    public virtual string? DevicePlatform     { get => _devicePlatform;     set => SetProperty(ref _devicePlatform,     value); }
    public virtual string? DeviceVersion      { get => _deviceVersion;      set => SetProperty(ref _deviceVersion,      value); }
    public virtual string? PackageName        { get => _packageName;        set => SetProperty(ref _packageName,        value); }
}



public abstract class DeviceMetaData<T> : DeviceMetaData
{
    protected T? _property;


    public override string? DeviceAppVersion
    {
        get => _deviceAppVersion;
        set
        {
            SetProperty(ref _deviceAppVersion, value);
            _property = default;
        }
    }
    public override string? DeviceID
    {
        get => _deviceID;
        set
        {
            SetProperty(ref _deviceID, value);
            _property = default;
        }
    }
    public override string? DeviceManufacturer
    {
        get => _deviceManufacturer;
        set
        {
            SetProperty(ref _deviceManufacturer, value);
            _property = default;
        }
    }
    public override string? DeviceModel
    {
        get => _deviceModel;
        set
        {
            SetProperty(ref _deviceModel, value);
            _property = default;
        }
    }
    public override string? DevicePlatform
    {
        get => _devicePlatform;
        set
        {
            SetProperty(ref _devicePlatform, value);
            _property = default;
        }
    }
    public override string? DeviceVersion
    {
        get => _deviceVersion;
        set
        {
            SetProperty(ref _deviceVersion, value);
            _property = default;
        }
    }
    public override string? PackageName
    {
        get => _packageName;
        set
        {
            SetProperty(ref _packageName, value);
            _property = default;
        }
    }
}



public sealed class DeviceInfo : DeviceMetaData<LogEventProperty>, ILogEventEnricher
{
    public LogEventProperty ToProperty() =>
        _property ??= new LogEventProperty(nameof(DeviceInfo),
                                           new StructureValue([
                                                                  Enricher.GetProperty(DeviceAppVersion,   nameof(DeviceAppVersion)),
                                                                  Enricher.GetProperty(DeviceID,           nameof(DeviceID)),
                                                                  Enricher.GetProperty(DeviceManufacturer, nameof(DeviceManufacturer)),
                                                                  Enricher.GetProperty(DeviceModel,        nameof(DeviceModel)),
                                                                  Enricher.GetProperty(DevicePlatform,     nameof(DevicePlatform)),
                                                                  Enricher.GetProperty(DeviceVersion,      nameof(DeviceVersion)),
                                                                  Enricher.GetProperty(PackageName,        nameof(PackageName))
                                                              ]));
    public void Enrich( LogEvent log, ILogEventPropertyFactory propertyFactory ) => log.AddPropertyIfAbsent(ToProperty());
}
