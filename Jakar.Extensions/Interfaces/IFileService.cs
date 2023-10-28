// unset


namespace Jakar.Extensions;


public interface IFileService
{
    public Task<FileInfo> DownloadFile( Uri link, string fileName );
}
