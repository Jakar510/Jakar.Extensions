namespace Jakar.Extensions;


[SuppressMessage( "ReSharper", "UnusedMethodReturnValue.Global" )]
public static class TempFile
{
    public static bool IsTempFile( this ITempFile file ) => file.IsTemporary;

    public static TItem SetNormal<TItem>( this TItem file )
        where TItem : ITempFile
    {
        file.IsTemporary = false;
        return file;
    }
    public static TItem SetTemporary<TItem>( this TItem file )
        where TItem : ITempFile
    {
        file.IsTemporary = true;
        return file;
    }



    public interface ITempFile : IDisposable
    {
        internal bool IsTemporary { get; set; }
    }
}
