namespace Jakar.Extensions;


[ Serializable ]
public class ScreenShotException : Exception
{
    private string? _screenShotFilePath;

    public string? FilePath
    {
        get => _screenShotFilePath;
        set
        {
            _screenShotFilePath = value;

            Data[nameof(FilePath)] = value;
        }
    }

    public ScreenShotException() { }
    public ScreenShotException( string?                           message ) : base( message ) { }
    public ScreenShotException( string                            message, Exception        innerException ) : base( message, innerException ) { }
    public ScreenShotException( string                            message, string?          path                         = null ) : base( message ) => FilePath = path;
    public ScreenShotException( string                            message, Exception        innerException, string? path = null ) : base( message, innerException ) => FilePath = path;
    [ Obsolete ] protected ScreenShotException( SerializationInfo info,    StreamingContext context,        string? path = null ) : base( info, context ) => FilePath = path;
}
