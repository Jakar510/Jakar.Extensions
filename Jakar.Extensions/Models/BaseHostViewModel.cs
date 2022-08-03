// Jakar.Extensions :: Jakar.Extensions
// 08/03/2022  5:28 PM

namespace Jakar.Extensions;


public abstract class BaseHostViewModel : BaseViewModel, IHostInfo
{
    private readonly Uri     _defaultHostInfo;
    private readonly string? _hostKey;
    private          string? _host;
    private          Uri?    _hostInfo;
    private          bool    _isValidHost;


    public string? Host
    {
        get => _host;
        set
        {
            SetProperty(ref _host, value);
            IsValidHost = Uri.TryCreate(value, UriKind.Absolute, out _hostInfo);
            OnPropertyChanged(nameof(HostInfo));
            if ( _hostKey is null ) { return; }

            Preferences.Set(_hostKey, value);
        }
    }
    public Uri HostInfo
    {
        get => _hostInfo ?? _defaultHostInfo;
        set
        {
            SetProperty(ref _hostInfo, value);
            Host = value.ToString();
        }
    }
    public bool IsValidHost
    {
        get => _isValidHost;
        set => SetProperty(ref _isValidHost, value);
    }


    protected BaseHostViewModel( Uri hostInfo ) : this(hostInfo, hostInfo) { }
    protected BaseHostViewModel( Uri defaultHostInfo, string hostKey ) : this(defaultHostInfo, defaultHostInfo, hostKey) { }
    protected BaseHostViewModel( Uri hostInfo, Uri defaultHostInfo, string hostKey ) : this(hostInfo, defaultHostInfo)
    {
        _hostKey = hostKey;
        Host     = Preferences.Get(hostKey, defaultHostInfo.OriginalString);
    }
    protected BaseHostViewModel( Uri hostInfo, Uri defaultHostInfo )
    {
        HostInfo         = hostInfo;
        _defaultHostInfo = defaultHostInfo;
    }
}
