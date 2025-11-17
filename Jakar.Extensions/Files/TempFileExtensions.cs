namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public static class TempFile
{
    public static bool IsTempFile( this ITempFile file ) => file.IsTemporary;

    extension<TItem>( TItem file )
        where TItem : ITempFile
    {
        public TItem SetNormal()
        {
            file.IsTemporary = false;
            return file;
        }
        public TItem SetTemporary()
        {
            file.IsTemporary = true;
            return file;
        }
    }



    public interface ITempFile : IDisposable
    {
        internal bool IsTemporary { get; set; }
    }
}
