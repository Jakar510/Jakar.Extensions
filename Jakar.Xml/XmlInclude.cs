#nullable enable
using System;



namespace Jakar.Xml;


[Flags]
public enum XmlInclude
{
    None       = 0,
    Attributes = 1 << 0,
    Fields     = 1 << 1,
    Properties = 1 << 2,
    All        = ~None
}
