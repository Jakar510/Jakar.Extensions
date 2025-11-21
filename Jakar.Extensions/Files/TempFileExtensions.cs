namespace Jakar.Extensions;


[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
public static class TempFile
{
    extension<TSelf>( TSelf self )
        where TSelf : ITempFile
    {
        public bool IsTempFile() => self.IsTemporary;


        public TSelf SetNormal()
        {
            self.IsTemporary = false;
            return self;
        }
        public TSelf SetTemporary()
        {
            self.IsTemporary = true;
            return self;
        }
    }



    public interface ITempFile : IDisposable
    {
        internal bool IsTemporary { get; set; }
    }
}
