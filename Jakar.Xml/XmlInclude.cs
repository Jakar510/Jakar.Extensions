﻿#nullable enable
using System;



namespace Jakar.Xml;


[Flags]
public enum XmlInclude
{
    None       = 0,
    Attributes = 1,
    Fields     = 2,
    Properties = 4,
    All        = ~None
}
