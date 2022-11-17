#nullable enable
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Screenshot;
using Location = Xamarin.Essentials.Location;



namespace Jakar.Extensions.Xamarin.Forms;


public static class AppShare
{
    public static bool         ScreenShotAvailable => _ScreenShotBuffer.IsEmpty;
    public static IFileService FileService         { get; } = DependencyService.Get<IFileService>();


    private static ReadOnlyMemory<byte> _ScreenShotBuffer { get; set; }


    // public static Image GetScreenShotImage() =>
    // 	new()
    // 	{
    // 		Source = GetScreenShotImageSource()
    // 	};
    // public static ImageSource GetScreenShotImageSource() => ImageSource.FromStream(GetScreenShotStream);
    // public static async Task<ScreenshotResult> TakeScreenShotResult() => await MainThread.InvokeOnMainThreadAsync(Screenshot.CaptureAsync);
    // public static async Task<Stream> GetScreenShotStream( CancellationToken token )
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

    public static async Task BufferScreenShot() => _ScreenShotBuffer = await TakeScreenShot()
                                                                          .ConfigureAwait( false );

    public static async Task OpenBrowser( this Uri uri, BrowserLaunchMode launchMode = BrowserLaunchMode.SystemPreferred ) => await Browser.OpenAsync( uri, launchMode );

    public static async Task SetupCrossMediaAsync( this Page page, string title, string message, string ok )
    {
        await CrossMedia.Current.Initialize()
                        .ConfigureAwait( false );

        if ( CrossMedia.Current.IsCameraAvailable ) { return; }

        await page.DisplayAlert( title, message, ok )
                  .ConfigureAwait( false );

        await page.Navigation.PopAsync()
                  .ConfigureAwait( false );
    }


    public static async Task ShareFile( this Uri uri, string shareTitle ) => await ShareFile( uri.ToString(), shareTitle )
                                                                                .ConfigureAwait( false );
    public static async Task ShareFile( this FileInfo info, string shareTitle ) => await ShareFile( info.FullName, shareTitle )
                                                                                      .ConfigureAwait( false );
    public static async Task ShareFile( string filePath, string shareTitle ) => await ShareFile( new ShareFile( filePath ), shareTitle )
                                                                                   .ConfigureAwait( false );
    public static async Task ShareFile( this Uri uri, string shareTitle, string mime ) => await ShareFile( uri.ToString(), shareTitle, mime )
                                                                                             .ConfigureAwait( false );
    public static async Task ShareFile( this FileInfo info, string shareTitle, string mime ) => await ShareFile( info.FullName, shareTitle, mime )
                                                                                                   .ConfigureAwait( false );
    public static async Task ShareFile( this LocalFile file, string shareTitle, string mime ) => await new ShareFile( file.Name, mime ).ShareFile( shareTitle )
                                                                                                                                       .ConfigureAwait( false );
    public static async Task ShareFile( this string filePath, string shareTitle, string mime ) => await new ShareFile( filePath, mime ).ShareFile( shareTitle )
                                                                                                                                       .ConfigureAwait( false );
    public static async Task ShareFile( this ShareFile shareFile, string shareTitle )
    {
        var request = new ShareFileRequest( shareTitle, shareFile );

        await Share.RequestAsync( request )
                   .ConfigureAwait( false );
    }

    public static async Task ShareRequest( this string title, string text, Uri uri ) => await ShareRequest( title, text, uri.ToString() )
                                                                                           .ConfigureAwait( false );
    public static async Task ShareRequest( this string title, string text, string uri ) => await Share.RequestAsync( GetTextRequest( title, text, uri ) )
                                                                                                      .ConfigureAwait( false );

    public static async Task<bool> Open( this string url ) => await Open( new Uri( url ) )
                                                                 .ConfigureAwait( false );
    public static async Task<bool> Open( this Uri url )
    {
        if ( await Launcher.CanOpenAsync( url )
                           .ConfigureAwait( false ) )
        {
            return await Launcher.TryOpenAsync( url )
                                 .ConfigureAwait( false );
        }

        return false;
    }


    public static async Task<LocalFile> OpenOfficeDoc( this Uri link, string shareTitle, MimeType mime, IAppSettings settings ) => await link.OpenOfficeDoc( shareTitle, mime, settings.AppName )
                                                                                                                                             .ConfigureAwait( false );
    public static async Task<LocalFile> OpenOfficeDoc( this Uri link, string shareTitle, MimeType mime, string name )
    {
        LocalFile info = await FileService.DownloadFile( link, mime.ToFileName( name ) )
                                          .ConfigureAwait( false );

        var url = info.ToUri( mime );

        if ( Device.RuntimePlatform == Device.Android )
        {
            await Launcher.OpenAsync( url )
                          .ConfigureAwait( false );
        }
        else
        {
            await info.ShareFile( shareTitle, mime.ToString() )
                      .ConfigureAwait( false );
        }

        return info;
    }

    public static async Task<MediaFile> GetPhoto( PickMediaOptions? options = null, CancellationToken token = default )
    {
        options ??= new PickMediaOptions
                    {
                        PhotoSize              = PhotoSize.Full,
                        SaveMetaData           = true,
                        RotateImage            = false,
                        ModalPresentationStyle = MediaPickerModalPresentationStyle.FullScreen,
                    };

        MediaFile photo = await CrossMedia.Current.PickPhotoAsync( options, token )
                                          .ConfigureAwait( false );

        return photo;
    }

    public static async Task<MediaFile> GetVideo( CancellationToken token = default ) => await CrossMedia.Current.PickVideoAsync( token )
                                                                                                         .ConfigureAwait( false );


    public static async Task<MediaFile> TakePhoto( StoreCameraMediaOptions? options = null, CancellationToken token = default )
    {
        Location? location = await LocationManager.GetLocation()
                                                  .ConfigureAwait( false );

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

        MediaFile photo = await CrossMedia.Current.TakePhotoAsync( options, token )
                                          .ConfigureAwait( false );

        return photo;
    }

    public static async Task<MediaFile> TakeVideo( StoreVideoOptions? options = null, CancellationToken token = default )
    {
        Location? location = await LocationManager.GetLocation()
                                                  .ConfigureAwait( false );

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

        MediaFile photo = await CrossMedia.Current.TakeVideoAsync( options, token )
                                          .ConfigureAwait( false );

        return photo;
    }

    public static async Task<ReadOnlyMemory<byte>> TakeScreenShot() => await MainThread.InvokeOnMainThreadAsync( CrossScreenshot.Current.CaptureAsync )
                                                                                       .ConfigureAwait( false );

    public static async Task<string> GetScreenShot( this FileSystemApi api )
    {
        ReadOnlyMemory<byte> screenShot = await TakeScreenShot()
                                             .ConfigureAwait( false );

        return await api.WriteScreenShot( screenShot )
                        .ConfigureAwait( false );
    }

    public static async Task<string> WriteScreenShot( this FileSystemApi api, CancellationToken token = default ) => await api.WriteScreenShot( _ScreenShotBuffer.ToArray(), token )
                                                                                                                              .ConfigureAwait( false );

    public static async Task<string> WriteScreenShot( this FileSystemApi api, ReadOnlyMemory<byte> screenShot, CancellationToken token = default )
    {
        string    path = api.ScreenShot;
        using var file = new LocalFile( path );

        await file.WriteAsync( screenShot, token )
                  .ConfigureAwait( false );

        return path;
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


    public static void SetupCrossMedia( this Page page, string title, string message, string ok ) => MainThread.BeginInvokeOnMainThread( async () => await page.SetupCrossMediaAsync( title, message, ok )
                                                                                                                                                               .ConfigureAwait( false ) );
}
