// Jakar.Extensions :: Jakar.Extensions.Maui.Sv
// 07/18/2024  20:07


using Jakar.SettingsView.Maui.Sv.Base;



namespace Jakar.SettingsView.Maui.Sv;


[SuppressMessage( "ReSharper", "ClassWithVirtualMembersNeverInherited.Global" )]
public class SvSection : ContentView, ISvSection<CellBase>, IEquatable<SvSection>
{
    public static readonly BindableProperty CellsProperty       = BindableProperty.Create( nameof(Cells),       typeof(ObservableCollection<CellBase>), typeof(SvSection), defaultValueCreator: static bindable => new ObservableCollection<CellBase>() );
    public static readonly BindableProperty FooterProperty      = BindableProperty.Create( nameof(Footer),      typeof(HeaderBase),                     typeof(SvSection), defaultValueCreator: FooterBase.Default.Create );
    public static readonly BindableProperty HeaderProperty      = BindableProperty.Create( nameof(Header),      typeof(HeaderBase),                     typeof(SvSection), defaultValueCreator: HeaderBase.Default.Create );
    public static readonly BindableProperty UseDragSortProperty = BindableProperty.Create( nameof(UseDragSort), typeof(bool),                           typeof(SvSection), false );


    public ObservableCollection<CellBase> Cells       { get => (ObservableCollection<CellBase>)GetValue( CellsProperty ); set => SetValue( CellsProperty,  value ); }
    public HeaderBase                     Footer      { get => (HeaderBase)GetValue( FooterProperty );                    set => SetValue( FooterProperty, value ); }
    ISectionBorder ISvSection.            Footer      => Footer;
    public HeaderBase                     Header      { get => (HeaderBase)GetValue( HeaderProperty ); set => SetValue( HeaderProperty, value ); }
    ISectionBorder ISvSection.            Header      => Header;
    public bool                           IsValid     => this.AreCellsValid();
    public bool                           UseDragSort { get => (bool)GetValue( UseDragSortProperty ); set => SetValue( UseDragSortProperty, value ); }


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


    public static TapGestureRecognizer AddGesture( ICommand command )
    {
        TapGestureRecognizer tap = new()
                                   {
                                       NumberOfTapsRequired = 1,
                                       Command              = command
                                   };

        return tap;
    }
    public static TapGestureRecognizer AddGesture( View view, ICommand command )
    {
        TapGestureRecognizer tap = AddGesture( command );
        view.GestureRecognizers.Add( tap );
        return tap;
    }
    public static void ClearGesture( View view ) => view.GestureRecognizers.Clear();
    public static void DoNothing()               { }


    public          bool Equals( SvSection? other ) => ReferenceEquals( this, other );
    public override bool Equals( object?    other ) => ReferenceEquals( this, other ) || other is SvSection section && Equals( section );
    public override int  GetHashCode()              => base.GetHashCode();



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
            private readonly Label __label = new()
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
                              Content      = __label
                          };

                SetDynamicResource( BackgroundColorProperty, FOOTER_BACKGROUND_COLOR );
                __label.SetDynamicResource( Label.FontSizeProperty,  FOOTER_FONT_SIZE );
                __label.SetDynamicResource( Label.TextColorProperty, FOOTER_TEXT_COLOR );
                __label.SetDynamicResource( BackgroundColorProperty, FOOTER_BACKGROUND_COLOR );
            }


            public static Default Create( BindableObject _ ) => new();
            public static Default Create()                   => new();


            public override void OnCharacterSpacingChanged( double               oldValue, double         value ) => __label.CharacterSpacing = value;
            public override void OnFontFamilyChanged( string                     oldValue, string         value ) => __label.FontFamily = value;
            public override void OnFontSizeChanged( double                       oldValue, double         value ) => __label.FontSize = value;
            public override void OnFontAutoScalingEnabledChanged( bool           oldValue, bool           value ) => __label.FontAutoScalingEnabled = value;
            public override void OnFontAttributesChanged( FontAttributes         oldValue, FontAttributes value ) => __label.FontAttributes = value;
            public override void OnTitleChanged( string                          oldValue, string         value ) => __label.Text = value;
            public override void OnHorizontalTextAlignmentChanged( TextAlignment oldValue, TextAlignment  value ) => __label.HorizontalTextAlignment = value;
            public override void OnVerticalTextAlignmentChanged( TextAlignment   oldValue, TextAlignment  value ) => __label.VerticalTextAlignment = value;
            public override void OnTextColorChanged( Color?                      oldValue, Color?         value ) => __label.TextColor = value ?? Colors.Black;
        }
    }



    public abstract class HeaderBase : BorderBase, ISectionBorder<Color, ImageSource>
    {
        public const           string               HEADER_BACKGROUND_COLOR = "HeaderBackgroundColor";
        public const           string               HEADER_FONT_SIZE        = "HeaderFontSize";
        public const           string               HEADER_TEXT_COLOR       = "HeaderTextColor";
        public const           double               MINIMUM_HEIGHT_REQUEST  = 60;
        public static readonly BindableProperty     CollapsedProperty       = BindableProperty.Create( nameof(Collapsed),     typeof(ImageSource), typeof(HeaderBase), propertyChanged: OnCollapsedChanged );
        public static readonly BindableProperty     ExpandedProperty        = BindableProperty.Create( nameof(Expanded),      typeof(ImageSource), typeof(HeaderBase), propertyChanged: OnExpandedChanged );
        public static readonly BindableProperty     IconSourceProperty      = BindableProperty.Create( nameof(IconSource),    typeof(ImageSource), typeof(HeaderBase), propertyChanged: OnIconSourceChanged );
        public static readonly BindableProperty     IsExpandedProperty      = BindableProperty.Create( nameof(IsExpanded),    typeof(bool),        typeof(HeaderBase), propertyChanged: OnIsExpandedChanged );
        public static readonly BindableProperty     TappedCommandProperty   = BindableProperty.Create( nameof(TappedCommand), typeof(ICommand),    typeof(HeaderBase), propertyChanged: OnTappedCommandChanged );
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


        private static  void OnCollapsedChanged( BindableObject     bindable, object old, object value ) => ((HeaderBase)bindable).OnCollapsedChanged( (ImageSource?)value );
        public abstract void OnCollapsedChanged( ImageSource?       value );
        private static  void OnExpandedChanged( BindableObject      bindable, object old, object value ) => ((HeaderBase)bindable).OnExpandedChanged( (ImageSource?)value );
        public abstract void OnExpandedChanged( ImageSource?        value );
        private static  void OnIconSourceChanged( BindableObject    bindable, object old, object value ) => ((HeaderBase)bindable).OnIconSourceChanged( (ImageSource?)value );
        public abstract void OnIconSourceChanged( ImageSource?      value );
        private static  void OnIsExpandedChanged( BindableObject    bindable, object old, object value ) => ((HeaderBase)bindable).OnIsExpandedChanged( (bool)value );
        public abstract void OnIsExpandedChanged( bool              value );
        private static  void OnTappedCommandChanged( BindableObject bindable, object old, object value ) => ((HeaderBase)bindable).OnTappedCommandChanged( (ICommand?)value );
        public abstract void OnTappedCommandChanged( ICommand?      value );



        public sealed class Default : HeaderBase
        {
            private readonly Grid __grid;
            private readonly Image __icon = new()
                                           {
                                               Aspect            = Aspect.AspectFit,
                                               HorizontalOptions = LayoutOptions.Fill,
                                               VerticalOptions   = LayoutOptions.Fill
                                           };
            private readonly Label __label = new()
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

                Grid.SetColumn( __label, 1 );
                Grid.SetColumn( __icon,  2 );

                SetDynamicResource( BackgroundColorProperty, HEADER_BACKGROUND_COLOR );
                __label.SetDynamicResource( Label.FontSizeProperty,  HEADER_FONT_SIZE );
                __label.SetDynamicResource( Label.TextColorProperty, HEADER_TEXT_COLOR );
                __label.SetDynamicResource( BackgroundColorProperty, HEADER_BACKGROUND_COLOR );
                __icon.SetDynamicResource( BackgroundColorProperty, HEADER_BACKGROUND_COLOR );

                Content = __grid = new Grid
                                  {
                                      HorizontalOptions = LayoutOptions.Fill,
                                      VerticalOptions   = LayoutOptions.Fill,
                                      ColumnDefinitions = [new ColumnDefinition { Width = new GridLength( 15, GridUnitType.Absolute ) }, new ColumnDefinition { Width = GridLength.Star }, new ColumnDefinition { Width = new GridLength( 0.1, GridUnitType.Star ) }],
                                      Children =
                                      {
                                          __label,
                                          __icon
                                      }
                                  };
            }
            public override void Dispose()
            {
                base.Dispose();
                ClearGesture( __grid );
                ClearGesture( __label );
                ClearGesture( __icon );
            }


            public static Default Create( BindableObject _ ) => new();
            public static Default Create()                   => new();


            public override void OnTappedCommandChanged( ICommand?               value )                          => _gestureRecognizer.Command = value;
            public override void OnCharacterSpacingChanged( double               oldValue, double         value ) => __label.CharacterSpacing = value;
            public override void OnFontFamilyChanged( string                     oldValue, string         value ) => __label.FontFamily = value;
            public override void OnFontSizeChanged( double                       oldValue, double         value ) => __label.FontSize = value;
            public override void OnFontAutoScalingEnabledChanged( bool           oldValue, bool           value ) => __label.FontAutoScalingEnabled = value;
            public override void OnFontAttributesChanged( FontAttributes         oldValue, FontAttributes value ) => __label.FontAttributes = value;
            public override void OnTitleChanged( string                          old,      string         value ) => __label.Text = value;
            public override void OnHorizontalTextAlignmentChanged( TextAlignment oldValue, TextAlignment  value ) => __label.HorizontalTextAlignment = value;
            public override void OnVerticalTextAlignmentChanged( TextAlignment   oldValue, TextAlignment  value ) => __label.VerticalTextAlignment = value;
            public override void OnTextColorChanged( Color?                      old,      Color?         value ) => __label.TextColor = value ?? Colors.Black;
            public override void OnIsExpandedChanged( bool value ) => IconSource = value
                                                                                       ? Collapsed
                                                                                       : Expanded;
            public override void OnCollapsedChanged( ImageSource? value ) => OnIconSourceChanged( IsExpanded
                                                                                                      ? Expanded
                                                                                                      : value );
            public override void OnExpandedChanged( ImageSource? value ) => OnIconSourceChanged( IsExpanded
                                                                                                     ? value
                                                                                                     : Collapsed );
            public override void OnIconSourceChanged( ImageSource? value ) => __icon.Source = value;
        }
    }
}
