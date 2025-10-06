// Jakar.Extensions :: Jakar.Extensions
// 09/22/2022  5:40 PM

namespace Jakar.Extensions;


public sealed class AttributeException : Exception
{
    public AttributeException() { }
    public AttributeException( string message ) : base(message) { }
    public AttributeException( string message,   Exception    inner ) : base(message, inner) { }
    public AttributeException( Type   classType, PropertyInfo propInfo ) : this(GetMessage(classType,                  propInfo)) { }
    public AttributeException( Type   classType, PropertyInfo propInfo, Exception inner ) : this(GetMessage(classType, propInfo), inner) { }
    public AttributeException( Type   classType, FieldInfo    propInfo ) : this(GetMessage(classType,                  propInfo)) { }
    public AttributeException( Type   classType, FieldInfo    propInfo, Exception inner ) : this(GetMessage(classType, propInfo), inner) { }


    private static string GetMessage( Type classType, PropertyInfo info )
    {
        ValueStringBuilder sb = new ValueStringBuilder().Append('"')
                                                        .Append(classType.FullName ?? classType.Name)
                                                        .Append('.')
                                                        .Append(info.Name)
                                                        .Append('"');

        if ( !info.CanRead ) { sb = sb.Append(" cannot be read from. It's of type "); }

        else if ( !info.CanWrite ) { sb = sb.Append(" cannot be written to. It's of type "); }

        else { sb = sb.Append(" of type "); }


        return sb.Append('"')
                 .Append(info.PropertyType.FullName ?? info.PropertyType.Name)
                 .Append('"')
                 .ToString();
    }


    private static string GetMessage( Type classType, FieldInfo info )
    {
        ValueStringBuilder sb = new ValueStringBuilder().Append('"')
                                                        .Append(classType.FullName ?? classType.Name)
                                                        .Append('.')
                                                        .Append(info.Name)
                                                        .Append('"')
                                                        .Append(" of type ");

        return sb.Append('"')
                 .Append(info.FieldType.FullName ?? info.FieldType.Name)
                 .Append('"')
                 .ToString();
    }
}
