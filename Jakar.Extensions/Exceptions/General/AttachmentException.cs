#nullable enable
namespace Jakar.Extensions;


public class AttachmentMissingException : Exception
{
    public AttachmentMissingException() { }
    public AttachmentMissingException( string key, string message ) : base( $"{key} => {message}" ) { }
    public AttachmentMissingException( string message ) : base( message ) { }
    public AttachmentMissingException( string message, Exception inner ) : base( message, inner ) { }
}
