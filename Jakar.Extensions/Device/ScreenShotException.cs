namespace Jakar.Extensions;


[Serializable]
public class ScreenShotException : Exception
{
    private string? __screenShotFilePath;

    public string? FilePath
    {
        get => __screenShotFilePath;
        set
        {
            __screenShotFilePath = value;

            Data[nameof(FilePath)] = value;
        }
    }

    public ScreenShotException() { }
    public ScreenShotException( string?                         message ) : base(message) { }
    public ScreenShotException( string                          message, Exception        innerException ) : base(message, innerException) { }
    public ScreenShotException( string                          message, string?          path                         = null ) : base(message) => FilePath = path;
    public ScreenShotException( string                          message, Exception        innerException, string? path = null ) : base(message, innerException) => FilePath = path;
    [Obsolete] protected ScreenShotException( SerializationInfo info,    StreamingContext context,        string? path = null ) : base(info, context) => FilePath = path;
}
