﻿// TrueLogic :: iTrueLogic.Maui
// 07/18/2024  20:07


namespace Jakar.SettingsView.Maui.Cells;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class SvSection : ContentView, IDisposable
{
    public static readonly BindableProperty CellsProperty       = BindableProperty.Create( nameof(Cells),       typeof(ObservableCollection<CellBase>), typeof(SvSection), defaultValueCreator: static bindable => new ObservableCollection<CellBase>() );
    public static readonly BindableProperty FooterProperty      = BindableProperty.Create( nameof(Footer),      typeof(HeaderBase),                     typeof(SvSection), defaultValueCreator: FooterBase.Default.Create );
    public static readonly BindableProperty HeaderProperty      = BindableProperty.Create( nameof(Header),      typeof(HeaderBase),                     typeof(SvSection), defaultValueCreator: HeaderBase.Default.Create );
    public static readonly BindableProperty UseDragSortProperty = BindableProperty.Create( nameof(UseDragSort), typeof(bool),                           typeof(SvSection), false );


    public ObservableCollection<CellBase> Cells       { get => (ObservableCollection<CellBase>)GetValue( CellsProperty ); set => SetValue( CellsProperty,       value ); }
    public HeaderBase                     Footer      { get => (HeaderBase)GetValue( FooterProperty );                    set => SetValue( FooterProperty,      value ); }
    public HeaderBase                     Header      { get => (HeaderBase)GetValue( HeaderProperty );                    set => SetValue( HeaderProperty,      value ); }
    public bool                           UseDragSort { get => (bool)GetValue( UseDragSortProperty );                     set => SetValue( UseDragSortProperty, value ); }


    public SvSection() : base() { }
    public SvSection( string? title ) : this() => Header.Title = title ?? string.Empty;
    public virtual void Dispose()
    {
        Header.Dispose();
        GC.SuppressFinalize( this );
    }


    public void OnAppearing()
    {
        foreach ( CellBase cell in Cells ) { cell.OnAppearing(); }
    }
    public void OnDisappearing()
    {
        foreach ( CellBase cell in Cells ) { cell.OnDisappearing(); }
    }


    public virtual void HeaderTapped()                       => Header.Toggle();
    public         void Hide()                               => IsVisible = false;
    public         void Show()                               => IsVisible = true;
    public         bool Remove( CellBase              cell ) => Cells.Remove( cell );
    public         void Remove( IEnumerable<CellBase> cell ) => Cells.Remove( cell );
    public         void Add( CellBase                 cell ) => Cells.Add( cell );
    public         void Add( IEnumerable<CellBase>    cell ) => Cells.Add( cell );


    protected static TapGestureRecognizer AddGesture( ICommand command )
    {
        TapGestureRecognizer tap = new()
                                   {
                                       NumberOfTapsRequired = 1,
                                       Command              = command
                                   };

        return tap;
    }
    protected static TapGestureRecognizer AddGesture( View view, ICommand command )
    {
        TapGestureRecognizer tap = AddGesture( command );
        view.GestureRecognizers.Add( tap );
        return tap;
    }
    protected static void ClearGesture( View view ) => view.GestureRecognizers.Clear();
    protected static void DoNothing()               { }



    public abstract class FooterBase : BorderBase
    {
        public const string FOOTER_BACKGROUND_COLOR = "HeaderBackgroundColor";
        public const string FOOTER_FONT_SIZE        = "HeaderFontSize";
        public const string FOOTER_TEXT_COLOR       = "HeaderTextColor";
        public const double MINIMUM_HEIGHT_REQUEST  = 40;
        public override string Title
        {
            get => (string)GetValue( TitleProperty );
            set
            {
                SetValue( TitleProperty, value );
                IsVisible = string.IsNullOrEmpty( value );
            }
        }



        public sealed class Default : FooterBase
        {
            private readonly Label _label = new()
                                            {
                                                Padding                 = new Thickness( 10, 0, 0, 0 ),
                                                VerticalTextAlignment   = TextAlignment.Center,
                                                HorizontalTextAlignment = TextAlignment.Start,
                                                HorizontalOptions       = LayoutOptions.Fill,
                                                VerticalOptions         = LayoutOptions.Fill
                                            };


            public Default() : base()
            {
                MinimumHeightRequest = MINIMUM_HEIGHT_REQUEST;
                HorizontalOptions    = LayoutOptions.Fill;
                VerticalOptions      = LayoutOptions.Fill;

                Content = new Frame
                          {
                              CornerRadius = 0,
                              HasShadow    = false,
                              Padding      = new Thickness( 0 ),
                              Margin       = new Thickness( 0 ),
                              Content      = _label
                          };

                SetDynamicResource( BackgroundColorProperty, FOOTER_BACKGROUND_COLOR );
                _label.SetDynamicResource( Label.FontSizeProperty,  FOOTER_FONT_SIZE );
                _label.SetDynamicResource( Label.TextColorProperty, FOOTER_TEXT_COLOR );
                _label.SetDynamicResource( BackgroundColorProperty, FOOTER_BACKGROUND_COLOR );
            }


            public static Default Create( BindableObject _ ) => new();
            public static Default Create()                   => new();


            protected override void OnTitleChanged( string     value ) => _label.Text = value;
            protected override void OnTextColorChanged( Color? value ) => _label.TextColor = value ?? Colors.Black;
        }
    }



    public abstract class HeaderBase : BorderBase
    {
        public const           string               HEADER_BACKGROUND_COLOR = "HeaderBackgroundColor";
        public const           string               HEADER_FONT_SIZE        = "HeaderFontSize";
        public const           string               HEADER_TEXT_COLOR       = "HeaderTextColor";
        public const           double               MINIMUM_HEIGHT_REQUEST  = 60;
        public static readonly BindableProperty     CollapsedProperty       = BindableProperty.Create( nameof(Collapsed),     typeof(ImageSource), typeof(HeaderBase) );
        public static readonly BindableProperty     ExpandedProperty        = BindableProperty.Create( nameof(Expanded),      typeof(ImageSource), typeof(HeaderBase) );
        public static readonly BindableProperty     IconSourceProperty      = BindableProperty.Create( nameof(IconSource),    typeof(ImageSource), typeof(HeaderBase) );
        public static readonly BindableProperty     IsExpandedProperty      = BindableProperty.Create( nameof(IsExpanded),    typeof(bool),        typeof(HeaderBase) );
        public static readonly BindableProperty     TappedCommandProperty   = BindableProperty.Create( nameof(TappedCommand), typeof(ICommand),    typeof(HeaderBase) );
        protected readonly     TapGestureRecognizer _gestureRecognizer;


        public ImageSource? Collapsed     { get => (ImageSource?)GetValue( CollapsedProperty );  set => SetValue( CollapsedProperty,     value ); }
        public ImageSource? Expanded      { get => (ImageSource?)GetValue( ExpandedProperty );   set => SetValue( ExpandedProperty,      value ); }
        public ImageSource? IconSource    { get => (ImageSource?)GetValue( IconSourceProperty ); set => SetValue( IconSourceProperty,    value ); }
        public bool         IsExpanded    { get => (bool)GetValue( IsExpandedProperty );         set => SetValue( IsExpandedProperty,    value ); }
        public ICommand?    TappedCommand { get => (ICommand?)GetValue( TappedCommandProperty ); set => SetValue( TappedCommandProperty, value ); }


        protected HeaderBase() : base()
        {
            _gestureRecognizer = new TapGestureRecognizer
                                 {
                                     NumberOfTapsRequired = 1,
                                     Command              = new Command( Toggle )
                                 };

            GestureRecognizers.Add( _gestureRecognizer );
        }
        public void Toggle() => IsExpanded = !IsExpanded;


        protected abstract void OnCollapsedChanged( ImageSource?  value );
        protected abstract void OnExpandedChanged( ImageSource?   value );
        protected abstract void OnIconSourceChanged( ImageSource? value );
        protected abstract void OnIsExpandedChanged( bool         value );
        protected abstract void OnTappedCommandChanged( ICommand? value );



        public sealed class Default : HeaderBase
        {
            private readonly Grid _grid;
            private readonly Image _icon = new()
                                           {
                                               Aspect            = Aspect.AspectFit,
                                               HorizontalOptions = LayoutOptions.Fill,
                                               VerticalOptions   = LayoutOptions.Fill
                                           };
            private readonly Label _label = new()
                                            {
                                                Padding                 = new Thickness( 0 ),
                                                VerticalTextAlignment   = TextAlignment.Center,
                                                HorizontalTextAlignment = TextAlignment.Start,
                                                HorizontalOptions       = LayoutOptions.Fill,
                                                VerticalOptions         = LayoutOptions.Fill
                                            };


            public Default() : base()
            {
                MinimumHeightRequest = MINIMUM_HEIGHT_REQUEST;
                HorizontalOptions    = LayoutOptions.Fill;
                VerticalOptions      = LayoutOptions.Fill;

                Grid.SetColumn( _label, 1 );
                Grid.SetColumn( _icon,  2 );

                SetDynamicResource( BackgroundColorProperty, HEADER_BACKGROUND_COLOR );
                _label.SetDynamicResource( Label.FontSizeProperty,  HEADER_FONT_SIZE );
                _label.SetDynamicResource( Label.TextColorProperty, HEADER_TEXT_COLOR );
                _label.SetDynamicResource( BackgroundColorProperty, HEADER_BACKGROUND_COLOR );
                _icon.SetDynamicResource( BackgroundColorProperty, HEADER_BACKGROUND_COLOR );

                Content = _grid = new Grid
                                  {
                                      HorizontalOptions = LayoutOptions.Fill,
                                      VerticalOptions   = LayoutOptions.Fill,
                                      ColumnDefinitions = [new ColumnDefinition { Width = new GridLength( 15, GridUnitType.Absolute ) }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = new GridLength( 0.1, GridUnitType.Star ) }],
                                      Children =
                                      {
                                          _label,
                                          _icon
                                      }
                                  };
            }
            public override void Dispose()
            {
                base.Dispose();
                ClearGesture( _grid );
                ClearGesture( _label );
                ClearGesture( _icon );
            }


            public static Default Create( BindableObject _ ) => new();
            public static Default Create()                   => new();


            protected override void OnTappedCommandChanged( ICommand? value ) => _gestureRecognizer.Command = value;
            protected override void OnTitleChanged( string            value ) => _label.Text = value;
            protected override void OnTextColorChanged( Color?        value ) => _label.TextColor = value ?? Colors.Black;
            protected override void OnIsExpandedChanged( bool value )
            {
                IconSource = value
                                 ? Collapsed
                                 : Expanded;
            }
            protected override void OnCollapsedChanged( ImageSource? value ) => OnIconSourceChanged( IsExpanded
                                                                                                         ? Expanded
                                                                                                         : value );
            protected override void OnExpandedChanged( ImageSource? value ) => OnIconSourceChanged( IsExpanded
                                                                                                        ? value
                                                                                                        : Collapsed );
            protected override void OnIconSourceChanged( ImageSource? value ) => _icon.Source = value;
        }
    }
}
