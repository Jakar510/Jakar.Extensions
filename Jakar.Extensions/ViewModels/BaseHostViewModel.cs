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
    private            string? __host;
    private            Uri?    __hostInfo;


    public virtual string? Host
    {
        get => __host ?? HostInfo?.OriginalString;
        set
        {
            if ( !SetProperty( ref __host, value ) ) { return; }

            Uri.TryCreate( value, UriKind.Absolute, out __hostInfo );
            OnPropertyChanged( nameof(IsValidHost) );
            OnPropertyChanged( nameof(HostInfo) );
        }
    }
    public virtual Uri? HostInfo
    {
        get => __hostInfo ?? _defaultHostInfo;
        set
        {
            if ( !SetProperty( ref __hostInfo, value ) ) { return; }

            SetProperty( ref __host, value?.OriginalString, StringComparer.OrdinalIgnoreCase, nameof(Host) );
            OnPropertyChanged( nameof(IsValidHost) );
        }
    }
    Uri IHostInfo.      HostInfo    => __hostInfo ?? _defaultHostInfo;
    public virtual bool IsValidHost { [MethodImpl( MethodImplOptions.AggressiveInlining )] get => HostInfo?.IsAbsoluteUri is true && HostInfo.Scheme.StartsWith( "http", StringComparison.OrdinalIgnoreCase ); }


    protected BaseHostViewModel( Uri defaultHostInfo, Uri? hostInfo = null ) : this( null, defaultHostInfo, hostInfo ) { }
    protected BaseHostViewModel( string? title, Uri defaultHostInfo, Uri? hostInfo = null ) : base( title )
    {
        _defaultHostInfo = defaultHostInfo;
        HostInfo         = hostInfo;
    }
    public virtual void Reset() => Host = null;
}
