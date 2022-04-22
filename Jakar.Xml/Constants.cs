using System;
using System.Collections;
using System.Collections.Generic;



namespace Jakar.Xml;


public static class Constants
{
    public const char   SPACE          = ' ';
    public const char   OPEN_START     = '<';
    public const char   OPEN_END       = '>';
    public const string CLOSE_START    = "</";
    public const char   CLOSE_END      = '>';
    public const char   EQUALS         = '=';
    public const string ITEM           = "Item";
    public const string GROUP          = "Group";
    public const string DICTIONARY     = "Dictionary";
    public const string KEY_VALUE_PAIR = nameof(KeyValuePair);
    public const string KEY            = nameof(DictionaryEntry.Key);
    public const string VALUE          = nameof(DictionaryEntry.Value);
    public const string NULL           = nameof(NULL);
    public const string TYPE           = nameof(Type);
    public const string XMLS           = "xmls";    // Name Space
    public const string XMLS_TAG       = "xmls=\""; // Name Space Tag
    public const string FIELDS         = "Fields";
    public const string PROPERTIES     = "Properties";



    public static class Dividers
    {
        public const char NS    = ':';
        public const char TYPES = '`';
    }
}
