// Jakar.Extensions :: Jakar.Extensions
// 08/03/2022  5:28 PM

namespace Jakar.Extensions;


public interface IHostViewModel : IHostInfo
{
    public bool    IsValidHost { get; set; }
    public string? Host        { get; set; }


    public void Reset();
}



public abstract class BaseHostViewModel : BaseViewModel, IHostViewModel
{
    protected readonly Uri     _defaultHostInfo;
    private            bool    _isValidHost;
    private            string? _host;
    private            Uri?    _hostInfo;


    public bool IsValidHost
    {
        get => _isValidHost;
        set => SetProperty( ref _isValidHost, value );
    }
    public string? Host
    {
        get => _host;
        set
        {
            if ( !SetProperty( ref _host, value ) ) { return; }

            IsValidHost = Uri.TryCreate( value, UriKind.Absolute, out _hostInfo );
            OnPropertyChanged( nameof(HostInfo) );
            OnHostChanged( value, IsValidHost, _hostInfo );
        }
    }
    public Uri HostInfo
    {
        get => _hostInfo ?? _defaultHostInfo;
        set
        {
            if ( SetProperty( ref _hostInfo, value ) ) { Host = value.ToString(); }
        }
    }


    protected BaseHostViewModel( Uri hostInfo ) : this( hostInfo, hostInfo ) { }
    protected BaseHostViewModel( Uri hostInfo, Uri defaultHostInfo )
    {
        _defaultHostInfo = defaultHostInfo;
        Host             = hostInfo.OriginalString;
    }


    [SuppressMessage( "ReSharper", "UnusedParameter.Global" )] protected virtual void OnHostChanged( in string? host, in bool isValid, in Uri? hostInfo ) { }
    public virtual void Reset() => Host = _defaultHostInfo.OriginalString;
}
