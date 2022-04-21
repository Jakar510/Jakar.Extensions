namespace Jakar.Extensions.FileSystemExtensions;


public static class TempFile
{
    public static TItem SetTemporary<TItem>( this TItem file ) where TItem : ITempFile
    {
        file.IsTemporary = true;
        return file;
    }

    public static TItem SetNormal<TItem>( this TItem file ) where TItem : ITempFile
    {
        file.IsTemporary = false;
        return file;
    }

    public static bool IsTempFile( this ITempFile file ) => file.IsTemporary;



    public interface ITempFile : IDisposable
    {
        internal bool IsTemporary { get; set; }
    }
}
