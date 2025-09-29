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
    private bool               __isCollapsable;
    private bool               __isExpanded;
    private bool               __isVisible;
    private TextFontAttributes __titleAttributes = TextFontAttributes.Bold;
    private HeaderData?        __data;
    private TImageSource?      __collapseIcon;
    private TImageSource?      __expandIcon;
    private TImageSource?      __icon;
    private string?            __title;
    private bool               __isTitleBold;


    public TImageSource? CollapseIcon { get => __collapseIcon; set => SetProperty( ref __collapseIcon, value ); }
    public HeaderData? Data
    {
        get => __data;
        set
        {
            if ( !SetProperty( ref __data, value ) ) { return; }

            OnPropertyChanged( nameof(Title) );
            OnPropertyChanged( nameof(TitleAttributes) );
        }
    }
    public ICommand      ExpandCollapseCommand { get; }
    public TImageSource? ExpandIcon            { get => __expandIcon; set => SetProperty( ref __expandIcon, value ); }
    public TImageSource? Icon
    {
        get => __icon ??= GetIcon( __isExpanded );
        set
        {
            if ( SetProperty( ref __icon, value ) ) { OnPropertyChanged( nameof(IconIsVisible) ); }
        }
    }
    public bool IconIsVisible => __icon is not null;
    public bool IsCollapsable { get => __isCollapsable; set => SetProperty( ref __isCollapsable, value ); }
    public bool IsExpanded
    {
        get => __isExpanded || !__isCollapsable;
        set
        {
            if ( SetProperty( ref __isExpanded, value ) ) { Icon = GetIcon( value ); }
        }
    }
    public bool               IsVisible       { get => __isVisible;                                       set => SetProperty( ref __isVisible,       value ); }
    public bool               IsTitleBold     { get => __isTitleBold;                                     set => SetProperty( ref __isTitleBold,     value ); }
    public string             Title           { get => __data?.Title           ?? __title ?? string.Empty; set => SetProperty( ref __title,           value ); }
    public TextFontAttributes TitleAttributes { get => __data?.TitleAttributes ?? __titleAttributes;       set => SetProperty( ref __titleAttributes, value ); }


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
        if ( !__isCollapsable ) { return; }

        IsExpanded = !__isExpanded;
    }
    protected TImageSource? GetIcon( bool isExpanded ) => isExpanded
                                                              ? __expandIcon
                                                              : __collapseIcon;
}
