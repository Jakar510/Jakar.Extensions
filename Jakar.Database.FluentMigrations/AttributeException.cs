namespace Jakar.Database.FluentMigrations;


public sealed class AttributeException : Exception
{
    public AttributeException() { }
    public AttributeException( string message ) : base(message) { }
    public AttributeException( string message,   Exception    inner ) : base(message, inner) { }
    public AttributeException( Type   classType, PropertyInfo propInfo ) : this(GetMessage(classType,                  propInfo)) { }
    public AttributeException( Type   classType, PropertyInfo propInfo, Exception inner ) : this(GetMessage(classType, propInfo), inner) { }


    private static string GetMessage( Type classType, PropertyInfo propInfo )
    {
        var sb = new StringBuilder();
        sb.Append('"');
        sb.Append(classType.FullName);
        sb.Append('.');
        sb.Append(propInfo.Name);
        sb.Append('"');

        if ( !propInfo.CanRead ) { sb.Append(" cannot be read from. It's of type "); }

        else if ( !propInfo.CanWrite ) { sb.Append(" cannot be written to. It's of type "); }

        else { sb.Append("  "); }

        sb.Append('"');
        sb.Append(propInfo.PropertyType.FullName);
        sb.Append('"');


        return sb.ToString();
    }
}
