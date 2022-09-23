using System;





#nullable enable
namespace Jakar.Xml;


[Flags]
public enum XmlInclude
{
    None       = 0,
    Attributes = 1,
    Fields     = 2,
    Properties = 4,
    All        = Attributes | Fields | Properties
}
