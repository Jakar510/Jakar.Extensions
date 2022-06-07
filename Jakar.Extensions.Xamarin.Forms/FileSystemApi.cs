using Xamarin.Essentials;




#nullable enable
namespace Jakar.Extensions.Xamarin.Forms;


public class FileSystemApi : BaseFileSystemApi
{
    protected override string _AppDataDirectory => FileSystem.AppDataDirectory;
    protected override string _CacheDirectory   => FileSystem.CacheDirectory;

    public FileSystemApi() : base() { }
}
