using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Foundation;
using Jakar.Extensions.Xamarin.Forms.iOS;
using UIKit;
using Xamarin.Forms;
using FileSystem = Xamarin.Essentials.FileSystem;





[assembly: Dependency(typeof(FileService))]


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.iOS;


public class FileService : IFileService
{
    public static string GetCacheDataPath( string fileName ) => Path.Combine(FileSystem.CacheDirectory, fileName);

    //public Task<FileInfo> DownloadFile( Uri link, string fileName )
    //{
    //	if ( link is null )
    //		throw new ArgumentNullException(nameof(link));

    //	if ( string.IsNullOrWhiteSpace(fileName) )
    //		throw new ArgumentNullException(nameof(fileName));

    //	using WebClient client = new WebClient();

    //	var manager = NSFileManager.DefaultManager;
    //	//UIDocumentPickerViewController
    //	//UIDocumentInteractionController

    //	var root = IosFileSystem.SharedPath;
    //	string path     = Path.Combine(root, fileName);

    //	client.DownloadFile(link, path);

    //	return Task.FromResult(new FileInfo(path));
    //}

    public async Task<FileInfo> DownloadFile( Uri link, string fileName )
    {
        using var client = new WebClient();
        string    path   = GetCacheDataPath(fileName);
        await client.DownloadFileTaskAsync(link, path);
        return new FileInfo(path);
    }
}



public static class IosFileSystem
{
    public static string SharedPath
    {
        get
        {
            string folder = SysMajorVersion > 7
                                ? NSSearchPath.GetDirectories(NSSearchPathDirectory.SharedPublicDirectory, NSSearchPathDomain.User).First()
                                : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            return folder;
        }
    }

    public static int SysMajorVersion
    {
        get
        {
            string value = UIDevice.CurrentDevice.SystemVersion.Split('.')[0];

            return int.TryParse(value, out int result)
                       ? result
                       : -1;
        }
    }
}
