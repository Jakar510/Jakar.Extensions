using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;





namespace Jakar.Xml;


internal static class Constants
{
    public static class Types
    {
        public const string ITEM   = "ItemType";
        public const string KEYS   = "KeyType";
        public const string VALUES = "ValueType";
    }



    public static class Dividers
    {
        public const char NS    = ':';
        public const char TYPES = '`';
    }



    public static class Generics
    {
        public const string ITEM           = "Item";
        public const string LIST           = "List";
        public const string DICTIONARY     = "Dictionary";
        public const string KEY_VALUE_PAIR = nameof(KeyValuePair);
        public const string ARRAY          = nameof(Array);
        public const string KEY            = nameof(DictionaryEntry.Key);
        public const string VALUE          = nameof(DictionaryEntry.Value);
    }



    public static class Classes
    {
        public const string PROPERTY_INFO = nameof(PropertyInfo);
        public const string FIELD_INFO    = nameof(FieldInfo);
    }



    public const string NULL       = nameof(NULL);
    public const string TYPE       = nameof(Type);
    public const string ELEMENT    = "Element";
    public const string CONTENT    = "Content";
    public const string NAME_SPACE = "NameSpace";
    public const string TABLE      = "Table";
}
