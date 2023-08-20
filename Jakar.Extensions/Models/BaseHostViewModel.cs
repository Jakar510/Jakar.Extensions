// Jakar.Extensions :: Jakar.Extensions
// 08/03/2022  5:28 PM

namespace Jakar.Extensions;


public interface IHostViewModel : IHostInfo
{
    public string? Host        { get; set; }
    public bool    IsValidHost { get; }


    public void Reset();
}



public abstract class BaseHostViewModel : BaseViewModel, IHostViewModel
{
    protected readonly Uri     _defaultHostInfo;
    private            string? _host;
    private            Uri?    _hostInfo;


    public string? Host
    {
        get => _host ?? HostInfo?.OriginalString;
        set
        {
            if ( !SetProperty( ref _host, value ) ) { return; }

            Uri.TryCreate( value, UriKind.Absolute, out _hostInfo );
            OnPropertyChanged( nameof(IsValidHost) );
            OnPropertyChanged( nameof(HostInfo) );
            OnHostChanged( _host, IsValidHost, _hostInfo );
        }
    }
    public Uri? HostInfo
    {
        get => _hostInfo ?? _defaultHostInfo;
        set
        {
            if ( !SetProperty( ref _hostInfo, value ) ) { return; }

            SetProperty( ref _host, value?.OriginalString, nameof(Host) );
            OnPropertyChanged( nameof(IsValidHost) );
            OnHostChanged( _host, IsValidHost, _hostInfo );
        }
    }
    Uri IHostInfo.      HostInfo    => HostInfo ?? throw new NullReferenceException( nameof(HostInfo) );
    public virtual bool IsValidHost => HostInfo?.IsAbsoluteUri is true && HostInfo.Scheme.StartsWith( "http", StringComparison.OrdinalIgnoreCase );


    protected BaseHostViewModel( Uri defaultHostInfo ) : this( default, defaultHostInfo ) { }
    protected BaseHostViewModel( Uri? hostInfo, Uri defaultHostInfo )
    {
        _defaultHostInfo = defaultHostInfo;
        Host             = hostInfo?.OriginalString;
    }


    protected abstract void OnHostChanged( in string? host, in bool isValid, in Uri? hostInfo );
    public virtual void Reset() => Host = default;
}
