#nullable enable
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Screenshot;
using Location = Xamarin.Essentials.Location;



namespace Jakar.Extensions.Xamarin.Forms;


public static class AppShare
{
    public static IFileService FileService { get; } = DependencyService.Get<IFileService>();


    // public static Image GetScreenShotImage() =>
    // 	new()
    // 	{
    // 		Source = GetScreenShotImageSource()
    // 	};
    // public static ImageSource GetScreenShotImageSource() => ImageSource.FromStream(GetScreenShotStream);
    // public static async ValueTask<ScreenshotResult> TakeScreenShotResult() => await MainThread.InvokeOnMainThreadAsync(Screenshot.CaptureAsync);
    // public static async ValueTask<Stream> GetScreenShotStream( CancellationToken token )
    // {
    // 	ScreenshotResult result = await Screenshot.CaptureAsync().ConfigureAwait(false);
    // 	token.ThrowIfCancellationRequested();
    // 	return await result.OpenReadAsync();
    // }


    public static ImageSource GetImageSource( this MediaFile file ) => ImageSource.FromStream( file.GetStream );


    // TODO: add MimeType extensions and overloads for ShareFile
    private static ShareTextRequest GetTextRequest( string title, string text, string uri ) =>
        new(text, title)
        {
            Uri = uri,
        };


    public static async ValueTask OpenBrowser( this Uri uri, BrowserLaunchMode launchMode = BrowserLaunchMode.SystemPreferred ) => await Browser.OpenAsync( uri, launchMode );
    public static async ValueTask SetupCrossMediaAsync( this Page page, string title, string message, string ok )
    {
        await CrossMedia.Current.Initialize();

        if ( CrossMedia.Current.IsCameraAvailable ) { return; }

        await page.DisplayAlert( title, message, ok );

        await page.Navigation.PopAsync();
    }


    public static async ValueTask ShareFile( this Uri       uri,      string shareTitle ) => await ShareFile( uri.ToString(),              shareTitle );
    public static async ValueTask ShareFile( this FileInfo  info,     string shareTitle ) => await ShareFile( info.FullName,               shareTitle );
    public static async ValueTask ShareFile( string         filePath, string shareTitle ) => await ShareFile( new ShareFile( filePath ),   shareTitle );
    public static async ValueTask ShareFile( this Uri       uri,      string shareTitle, string mime ) => await ShareFile( uri.ToString(), shareTitle, mime );
    public static async ValueTask ShareFile( this FileInfo  info,     string shareTitle, string mime ) => await ShareFile( info.FullName,  shareTitle, mime );
    public static async ValueTask ShareFile( this LocalFile file,     string shareTitle, string mime ) => await new ShareFile( file.Name, mime ).ShareFile( shareTitle );
    public static async ValueTask ShareFile( this string    filePath, string shareTitle, string mime ) => await new ShareFile( filePath,  mime ).ShareFile( shareTitle );
    public static async ValueTask ShareFile( this ShareFile shareFile, string shareTitle )
    {
        var request = new ShareFileRequest( shareTitle, shareFile );

        await Share.RequestAsync( request );
    }

    public static async ValueTask ShareRequest( this string title, string text, Uri    uri ) => await ShareRequest( title, text, uri.ToString() );
    public static async ValueTask ShareRequest( this string title, string text, string uri ) => await Share.RequestAsync( GetTextRequest( title, text, uri ) );

    public static async ValueTask<bool> Open( this string url ) => await Open( new Uri( url ) );
    public static async ValueTask<bool> Open( this Uri url )
    {
        if ( await Launcher.CanOpenAsync( url ) ) { return await Launcher.TryOpenAsync( url ); }

        return false;
    }


    public static async ValueTask<LocalFile> OpenOfficeDoc( this Uri link, string shareTitle, MimeType mime, IAppSettings settings ) => await link.OpenOfficeDoc( shareTitle, mime, settings.AppName );
    public static async ValueTask<LocalFile> OpenOfficeDoc( this Uri link, string shareTitle, MimeType mime, string name )
    {
        LocalFile info = await FileService.DownloadFile( link, mime.ToFileName( name ) );

        var url = info.ToUri( mime );

        if ( Device.RuntimePlatform == Device.Android ) { await Launcher.OpenAsync( url ); }
        else { await info.ShareFile( shareTitle, mime.ToString() ); }

        return info;
    }

    public static async ValueTask<MediaFile> GetPhoto( PickMediaOptions? options = null, CancellationToken token = default )
    {
        options ??= new PickMediaOptions
                    {
                        PhotoSize              = PhotoSize.Full,
                        SaveMetaData           = true,
                        RotateImage            = false,
                        ModalPresentationStyle = MediaPickerModalPresentationStyle.FullScreen,
                    };

        MediaFile photo = await CrossMedia.Current.PickPhotoAsync( options, token );

        return photo;
    }
    public static async ValueTask<MediaFile> GetVideo( CancellationToken token = default ) => await CrossMedia.Current.PickVideoAsync( token );


    public static async ValueTask<MediaFile> TakePhoto( StoreCameraMediaOptions? options = null, CancellationToken token = default )
    {
        Location? location = await LocationManager.GetLocation();

        options ??= new StoreCameraMediaOptions
                    {
                        SaveMetaData  = true,
                        SaveToAlbum   = true,
                        DefaultCamera = CameraDevice.Rear,
                        PhotoSize     = PhotoSize.Full,
                        RotateImage   = false,
                        AllowCropping = false,
                        Location      = location?.ToPluginLocation(),
                    };

        MediaFile photo = await CrossMedia.Current.TakePhotoAsync( options, token );

        return photo;
    }
    public static async ValueTask<MediaFile> TakeVideo( StoreVideoOptions? options = null, CancellationToken token = default )
    {
        Location? location = await LocationManager.GetLocation();

        options ??= new StoreVideoOptions
                    {
                        Quality       = VideoQuality.High,
                        SaveMetaData  = true,
                        SaveToAlbum   = true,
                        DefaultCamera = CameraDevice.Rear,
                        PhotoSize     = PhotoSize.Full,
                        RotateImage   = false,
                        AllowCropping = false,
                        Location      = location?.ToPluginLocation(),
                    };

        MediaFile photo = await CrossMedia.Current.TakeVideoAsync( options, token );

        return photo;
    }


    public static UriImageSource GetImageSource( this string url ) => GetImageSource( new Uri( url ),           5 );
    public static UriImageSource GetImageSource( this string url, int days ) => GetImageSource( new Uri( url ), days );
    public static UriImageSource GetImageSource( this Uri    url, int days ) => GetImageSource( url,            new TimeSpan( days, 0, 0, 0 ) );
    public static UriImageSource GetImageSource( this Uri url, TimeSpan time ) => new()
                                                                                  {
                                                                                      Uri            = url,
                                                                                      CachingEnabled = true,
                                                                                      CacheValidity  = time,
                                                                                  };


    public static void SetupCrossMedia( this Page page, string title, string message, string ok ) => MainThread.BeginInvokeOnMainThread( async () => await page.SetupCrossMediaAsync( title, message, ok ) );
}
