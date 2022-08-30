// Jakar.Extensions :: Jakar.Extensions
// 08/03/2022  5:28 PM

namespace Jakar.Extensions;


public abstract class BaseHostViewModel : BaseViewModel, IHostInfo
{
    protected readonly Uri     _defaultHostInfo;
    private            string? _host;
    private            Uri?    _hostInfo;
    private            bool    _isValidHost;


    public virtual string? Host
    {
        get => _host;
        set
        {
            SetProperty(ref _host, value);
            IsValidHost = Uri.TryCreate(value, UriKind.Absolute, out _hostInfo);
            OnPropertyChanged(nameof(HostInfo));
        }
    }
    public virtual Uri HostInfo
    {
        get => _hostInfo ?? _defaultHostInfo;
        set
        {
            SetProperty(ref _hostInfo, value);
            Host = value.ToString();
        }
    }
    public virtual bool IsValidHost
    {
        get => _isValidHost;
        set => SetProperty(ref _isValidHost, value);
    }


    protected BaseHostViewModel( Uri hostInfo ) : this(hostInfo, hostInfo) { }
    protected BaseHostViewModel( Uri hostInfo, Uri defaultHostInfo )
    {
        _defaultHostInfo = defaultHostInfo;
        HostInfo         = hostInfo;
    }
}
