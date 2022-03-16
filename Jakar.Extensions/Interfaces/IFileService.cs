// unset


namespace Jakar.Extensions.Interfaces;


public interface IFileService
{
    public Task<FileInfo> DownloadFile( Uri link, string fileName );
}
