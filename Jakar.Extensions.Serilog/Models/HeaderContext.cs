// Jakar.Extensions :: Jakar.Extensions.Serilog
// 01/19/2025  14:01

using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;



namespace Jakar.Extensions.Serilog;


[Flags]
public enum TextFontAttributes
{
    None   = 0,
    Bold   = 1 << 0,
    Italic = 1 << 1
}



public interface IHeader : INotifyPropertyChanged, INotifyPropertyChanging
{
    public bool   IsTitleBold { get; }
    public string Title       { get; set; }
}



public interface IHeaderContext : IHeader
{
    public HeaderData? Data                  { get; set; }
    public ICommand    ExpandCollapseCommand { get; }
    public bool        IsExpanded            { get; set; }
    public bool        IsVisible             { get; set; }
}



public interface IHeaderContext<TImageSource> : IHeaderContext
{
    public TImageSource? CollapseIcon { get; set; }
    public TImageSource? ExpandIcon   { get; set; }
    public TImageSource? Icon         { get; set; }
}



public class HeaderContext<TImageSource> : ObservableClass, IHeaderContext<TImageSource>
{
    private bool               _isCollapsable;
    private bool               _isExpanded;
    private bool               _isVisible;
    private TextFontAttributes _titleAttributes = TextFontAttributes.Bold;
    private HeaderData?        _data;
    private TImageSource?      _collapseIcon;
    private TImageSource?      _expandIcon;
    private TImageSource?      _icon;
    private string?            _title;
    private bool               _isTitleBold;


    public TImageSource? CollapseIcon { get => _collapseIcon; set => SetProperty( ref _collapseIcon, value ); }
    public HeaderData? Data
    {
        get => _data;
        set
        {
            if ( !SetProperty( ref _data, value ) ) { return; }

            OnPropertyChanged( nameof(Title) );
            OnPropertyChanged( nameof(TitleAttributes) );
        }
    }
    public ICommand      ExpandCollapseCommand { get; }
    public TImageSource? ExpandIcon            { get => _expandIcon; set => SetProperty( ref _expandIcon, value ); }
    public TImageSource? Icon
    {
        get => _icon ??= GetIcon( _isExpanded );
        set
        {
            if ( SetProperty( ref _icon, value ) ) { OnPropertyChanged( nameof(IconIsVisible) ); }
        }
    }
    public bool IconIsVisible => _icon is not null;
    public bool IsCollapsable { get => _isCollapsable; set => SetProperty( ref _isCollapsable, value ); }
    public bool IsExpanded
    {
        get => _isExpanded || !_isCollapsable;
        set
        {
            if ( SetProperty( ref _isExpanded, value ) ) { Icon = GetIcon( value ); }
        }
    }
    public bool               IsVisible       { get => _isVisible;                                       set => SetProperty( ref _isVisible,       value ); }
    public bool               IsTitleBold     { get => _isTitleBold;                                     set => SetProperty( ref _isTitleBold,     value ); }
    public string             Title           { get => _data?.Title           ?? _title ?? string.Empty; set => SetProperty( ref _title,           value ); }
    public TextFontAttributes TitleAttributes { get => _data?.TitleAttributes ?? _titleAttributes;       set => SetProperty( ref _titleAttributes, value ); }


    public HeaderContext()
    {
        ExpandCollapseCommand = new RelayCommand( ExpandCollapse );
        IsExpanded            = true;
    }
    public static HeaderContext<TImageSource> Create( TImageSource collapse, TImageSource expand, bool isCollapsable ) => new()
                                                                                                                          {
                                                                                                                              CollapseIcon  = collapse,
                                                                                                                              ExpandIcon    = expand,
                                                                                                                              IsCollapsable = isCollapsable
                                                                                                                          };


    protected void ExpandCollapse()
    {
        if ( !_isCollapsable ) { return; }

        IsExpanded = !_isExpanded;
    }
    protected TImageSource? GetIcon( bool isExpanded ) => isExpanded
                                                              ? _expandIcon
                                                              : _collapseIcon;
}
