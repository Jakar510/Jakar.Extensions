// Jakar.Extensions :: Jakar.Extensions
// 08/03/2022  5:28 PM

namespace Jakar.Extensions;


public interface IHostViewModel : IHostInfo
{
    public string? Host        { get; set; }
    public bool    IsValidHost { get; set; }


    public void Reset();
}



public abstract class BaseHostViewModel : BaseViewModel, IHostViewModel
{
    protected readonly Uri     _defaultHostInfo;
    private            bool    _isValidHost;
    private            string? _host;
    private            Uri?    _hostInfo;
    public string? Host
    {
        get => _host;
        set
        {
            if ( !SetProperty( ref _host, value ) ) { return; }

            IsValidHost = Uri.TryCreate( value, UriKind.Absolute, out _hostInfo ) && ValidateUri( _hostInfo );
            OnPropertyChanged( nameof(HostInfo) );
            OnHostChanged( value, IsValidHost, _hostInfo );
        }
    }
    public Uri HostInfo
    {
        get
        {
            if ( _hostInfo is null ) { Host = _defaultHostInfo.OriginalString; }

            return _hostInfo ?? throw new NullReferenceException( nameof(_hostInfo) );
        }
        set
        {
            if ( !SetProperty( ref _hostInfo, value ) ) { return; }

            Host = value.OriginalString;
        }
    }


    public bool IsValidHost
    {
        get => _isValidHost;
        set => SetProperty( ref _isValidHost, value );
    }


    protected BaseHostViewModel( Uri defaultHostInfo ) : this( default, defaultHostInfo ) { }
    protected BaseHostViewModel( Uri? hostInfo, Uri defaultHostInfo )
    {
        _defaultHostInfo = defaultHostInfo;
        Host             = hostInfo?.OriginalString;
    }


    protected virtual bool ValidateUri( Uri hostInfo ) => hostInfo.Scheme.StartsWith( "http", StringComparison.OrdinalIgnoreCase );

    protected abstract void OnHostChanged( in string? host, in bool isValid, in Uri? hostInfo );

    public virtual void Reset() => Host = _defaultHostInfo.OriginalString;
}
