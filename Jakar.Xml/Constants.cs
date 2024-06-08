using System;
using System.Collections;
using System.Collections.Generic;



namespace Jakar.Xml;


public static class Constants
{
    public const string DICTIONARY     = "Dictionary";
    public const string FIELDS         = "Fields";
    public const string GROUP          = "Group";
    public const string ITEM           = "Item";
    public const string KEY            = nameof(DictionaryEntry.Key);
    public const string KEY_VALUE_PAIR = nameof(KeyValuePair);
    public const string NULL           = nameof(NULL);
    public const string PROPERTIES     = "Properties";
    public const string TYPE           = nameof(Type);
    public const string VALUE          = nameof(DictionaryEntry.Value);
    public const string XMLS           = "xmls";    // AppName Space
    public const string XMLS_TAG       = "xmls=\""; // AppName Space Tag



    public static class Dividers
    {
        public const char NS    = ':';
        public const char TYPES = '`';
    }
}
