using Jakar.Extensions.Xamarin.Forms.Extensions;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Screenshot;
using Xamarin.Essentials;





namespace Jakar.Extensions.Xamarin.Forms.Statics;


public static class AppShare
{
    // TODO: add MimeType extensions and overloads for ShareFile

    private static ShareTextRequest GetTextRequest( string title, string text, string uri ) =>
        new(text, title)
        {
            Uri = uri,
        };

    private static IFileService _FileService { get; } = DependencyService.Get<IFileService>();

    public static Task ShareRequest( this string title, string text, Uri    uri ) => ShareRequest(title, text, uri.ToString());
    public static Task ShareRequest( this string title, string text, string uri ) => Share.RequestAsync(GetTextRequest(title, text, uri));


    public static Task ShareFile( this Uri       uri,      string shareTitle )              => ShareFile(uri.ToString(), shareTitle);
    public static Task ShareFile( this FileInfo  info,     string shareTitle )              => ShareFile(info.FullName, shareTitle);
    public static Task ShareFile( string         filePath, string shareTitle )              => ShareFile(new ShareFile(filePath), shareTitle);
    public static Task ShareFile( this Uri       uri,      string shareTitle, string mime ) => ShareFile(uri.ToString(), shareTitle, mime);
    public static Task ShareFile( this FileInfo  info,     string shareTitle, string mime ) => ShareFile(info.FullName, shareTitle, mime);
    public static Task ShareFile( this LocalFile file,     string shareTitle, string mime ) => new ShareFile(file.Name, mime).ShareFile(shareTitle);
    public static Task ShareFile( this string    filePath, string shareTitle, string mime ) => new ShareFile(filePath, mime).ShareFile(shareTitle);

    public static Task ShareFile( this ShareFile shareFile, string shareTitle )
    {
        var request = new ShareFileRequest(shareTitle, shareFile);
        return Share.RequestAsync(request);
    }


    public static Task<LocalFile?> OpenOfficeDoc( this Uri link, string shareTitle, MimeType mime, IAppSettings settings ) => link.OpenOfficeDoc(shareTitle, mime, settings.AppName);

    public static async Task<LocalFile?> OpenOfficeDoc( this Uri link, string shareTitle, MimeType mime, string name )
    {
        LocalFile info = await _FileService.DownloadFile(link, mime.ToFileName(name));

        var url = info.ToUri(mime);

        if ( Device.RuntimePlatform == Device.Android ) { await Launcher.OpenAsync(url); }
        else { await info.ShareFile(shareTitle, mime.ToString()); }

        return info;
    }

    public static Task<bool> Open( this string url ) => Open(new Uri(url));

    public static async Task<bool> Open( this Uri url )
    {
        if ( await Launcher.CanOpenAsync(url) ) { return await Launcher.TryOpenAsync(url); }

        return false;
    }

    public static Task OpenBrowser( this Uri uri, BrowserLaunchMode launchMode = BrowserLaunchMode.SystemPreferred ) => Browser.OpenAsync(uri, launchMode);


    public static void SetupCrossMedia( this Page page, string title, string message, string ok )
        => MainThread.BeginInvokeOnMainThread(async () => await page.SetupCrossMediaAsync(title, message, ok));

    public static async Task SetupCrossMediaAsync( this Page page, string title, string message, string ok )
    {
        await CrossMedia.Current.Initialize();

        if ( CrossMedia.Current.IsCameraAvailable ) { return; }

        await page.DisplayAlert(title, message, ok);
        await page.Navigation.PopAsync();
    }


    public static async Task<MediaFile> TakePhoto( StoreCameraMediaOptions? options = null, CancellationToken token = default )
    {
        options ??= new StoreCameraMediaOptions()
                    {
                        SaveMetaData  = true,
                        SaveToAlbum   = true,
                        DefaultCamera = CameraDevice.Rear,
                        PhotoSize     = PhotoSize.Full,
                        RotateImage   = false,
                        AllowCropping = false,
                        Location      = ( await LocationManager.GetLocation() )?.ToPluginLocation(),
                    };

        MediaFile photo = await CrossMedia.Current.TakePhotoAsync(options, token);
        return photo;
    }

    public static async Task<MediaFile> GetPhoto( PickMediaOptions? options = null, CancellationToken token = default )
    {
        options ??= new PickMediaOptions()
                    {
                        PhotoSize              = PhotoSize.Full,
                        SaveMetaData           = true,
                        RotateImage            = false,
                        ModalPresentationStyle = MediaPickerModalPresentationStyle.FullScreen,
                    };

        MediaFile photo = await CrossMedia.Current.PickPhotoAsync(options, token);
        return photo;
    }

    public static async Task<MediaFile> TakeVideo( StoreVideoOptions? options = null, CancellationToken token = default )
    {
        options ??= new StoreVideoOptions()
                    {
                        Quality       = VideoQuality.High,
                        SaveMetaData  = true,
                        SaveToAlbum   = true,
                        DefaultCamera = CameraDevice.Rear,
                        PhotoSize     = PhotoSize.Full,
                        RotateImage   = false,
                        AllowCropping = false,
                        Location      = ( await LocationManager.GetLocation() )?.ToPluginLocation(),
                    };

        MediaFile photo = await CrossMedia.Current.TakeVideoAsync(options, token);
        return photo;
    }

    public static async Task<MediaFile> GetVideo( CancellationToken token = default ) => await CrossMedia.Current.PickVideoAsync(token);


    private static ReadOnlyMemory<byte> _ScreenShotBuffer   { get; set; }
    public static  bool                 ScreenShotAvailable => _ScreenShotBuffer.IsEmpty;

    public static async Task BufferScreenShot() => _ScreenShotBuffer = await TakeScreenShot();

    public static async Task<string> GetScreenShot( this FileSystemApi api )
    {
        ReadOnlyMemory<byte> screenShot = await TakeScreenShot();
        return await api.WriteScreenShot(screenShot);
    }

    public static async Task<ReadOnlyMemory<byte>> TakeScreenShot() => await MainThread.InvokeOnMainThreadAsync(CrossScreenshot.Current.CaptureAsync);

    public static async Task<string> WriteScreenShot( this FileSystemApi api ) => await api.WriteScreenShot(_ScreenShotBuffer.ToArray());

    public static async Task<string> WriteScreenShot( this FileSystemApi api, ReadOnlyMemory<byte> screenShot )
    {
        string          path = api.ScreenShot;
        using var file = new LocalFile(path);
        await file.WriteAsync(screenShot);

        return path;
    }


    // public static Image GetScreenShotImage() =>
    // 	new()
    // 	{
    // 		Source = GetScreenShotImageSource()
    // 	};
    // public static ImageSource GetScreenShotImageSource() => ImageSource.FromStream(GetScreenShotStream);
    // public static async Task<ScreenshotResult> TakeScreenShotResult() => await MainThread.InvokeOnMainThreadAsync(Screenshot.CaptureAsync);
    // public static async Task<Stream> GetScreenShotStream( CancellationToken token )
    // {
    // 	ScreenshotResult result = await Screenshot.CaptureAsync();
    // 	token.ThrowIfCancellationRequested();
    // 	return await result.OpenReadAsync();
    // }


    public static ImageSource    GetImageSource( this MediaFile file )          => ImageSource.FromStream(file.GetStream);
    public static UriImageSource GetImageSource( this string    url )           => GetImageSource(new Uri(url), 5);
    public static UriImageSource GetImageSource( this string    url, int days ) => GetImageSource(new Uri(url), days);
    public static UriImageSource GetImageSource( this Uri       url, int days ) => GetImageSource(url, new TimeSpan(days, 0, 0, 0));

    public static UriImageSource GetImageSource( this Uri url, TimeSpan time ) => new()
                                                                                  {
                                                                                      Uri            = url,
                                                                                      CachingEnabled = true,
                                                                                      CacheValidity  = time
                                                                                  };
}
