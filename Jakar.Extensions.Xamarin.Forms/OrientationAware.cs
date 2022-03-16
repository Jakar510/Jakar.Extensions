using Xamarin.Essentials;





namespace Jakar.Extensions.Xamarin.Forms;


public class OrientationService
{
    // https://www.wintellect.com/responding-to-orientation-changes-in-xamarin-forms/

    public const double SIZE_NOT_ALLOCATED = -1;


    protected double _Width  { get; set; }
    protected double _Height { get; set; }

    public event EventHandler<RotationEventArgs>? OnOrientationChanged;

    public DisplayOrientation Orientation { get; set; }


    //Xamarin.Essentials.DeviceIdiom.Phone
    //Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Rotation
    //Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Orientation
    //Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density
    //Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Width
    //Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Height
    //public static bool IsPhone { get; } = DeviceInfo.Idiom == DeviceIdiom.Phone;

    public OrientationService() : this(SIZE_NOT_ALLOCATED, SIZE_NOT_ALLOCATED) { }

    public OrientationService( in double width, in double height )
    {
        _Width  = width;
        _Height = height;
    }


    public void OnSizeAllocated( in Page page ) => OnSizeAllocated(page.Width, page.Height);

    public void OnSizeAllocated( in double width, in double height )
    {
        if ( Equals(_Width, width) && Equals(_Height, height) ) return;

        double oldWidth = _Width;
        _Width  = width;
        _Height = height;

        Orientation = GetOrientation(_Width, _Height);

        // ignore if the previous height was size unallocated OR Has the device been rotated
        if ( Equals(_Width, oldWidth) ) return;
        if ( Orientation == DisplayOrientation.Unknown ) return;
        OnOrientationChanged?.Invoke(this, new RotationEventArgs(Orientation));
    }


    public static DisplayOrientation GetOrientation( ContentPage page )
    {
        if ( page is null ) throw new ArgumentNullException(nameof(page));

        return GetOrientation(page.Width, page.Height);
    }

    public static DisplayOrientation GetOrientation( in double width, in double height )
    {
        if ( Equals(width, SIZE_NOT_ALLOCATED) || Equals(height, SIZE_NOT_ALLOCATED) ) return DisplayOrientation.Unknown;

        return ( width < height )
                   ? DisplayOrientation.Portrait
                   : DisplayOrientation.Landscape;
    }



    public class RotationEventArgs : EventArgs
    {
        public DisplayOrientation Orientation { get; }
        public RotationEventArgs( DisplayOrientation orientation ) => Orientation = orientation;
    }
}



public class OrientationContentPage : ContentPage
{
    public OrientationService Orientation { get; set; }


    protected OrientationContentPage() : base() => Orientation = new OrientationService(Width, Height);


    protected override void OnSizeAllocated( double width, double height )
    {
        base.OnSizeAllocated(width, height);
        Orientation.OnSizeAllocated(width, height);
    }
}
