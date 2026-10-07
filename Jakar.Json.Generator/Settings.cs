// Jakar.Json.Generator
// 10/07/2026

using System.Collections.Generic;
using Microsoft.CodeAnalysis;



namespace Jakar.Json.Generator;


/// <summary>
///     The settings attributes' values, as their underlying numbers. 0 means "inherit" (every settings enum starts with <c>Inherit = 0</c>), so layers merge by
///     taking the nearer non-zero value: member → type → assembly → built-in default (SPEC.md §4.1).
/// </summary>
internal readonly record struct Settings
{
    public int Naming               { get; init; }
    public int NameMatching         { get; init; }
    public int NullValues           { get; init; }
    public int DefaultValues        { get; init; }
    public int UnknownMembers       { get; init; }
    public int DuplicateMembers     { get; init; }
    public int Enums                { get; init; }
    public int EnumNaming           { get; init; }
    public int NumbersFromStrings   { get; init; }
    public int LargeIntegers        { get; init; }
    public int NonFiniteFloats      { get; init; }
    public int LocalDateTimes       { get; init; }
    public int UnorderedCollections { get; init; }
    public int Escaping             { get; init; }
    public int Indented             { get; init; }
    public int IndentChar           { get; init; }
    public int IndentSize           { get; init; }
    public int AllowComments        { get; init; }
    public int AllowTrailingCommas  { get; init; }
    public int MaxDepth             { get; init; }
    public int GenerateToString     { get; init; }


    // Built-in defaults (the enums' values: Inherit = 0, then the members in declaration order).
    public static readonly Settings BuiltIn = new()
                                              {
                                                  Naming               = Values.NAMING_AS_DECLARED,
                                                  NameMatching         = 1, // Exact
                                                  NullValues           = 1, // Write
                                                  DefaultValues        = 1, // Write
                                                  UnknownMembers       = Values.UNKNOWN_SKIP,
                                                  DuplicateMembers     = Values.DUPLICATES_ERROR,
                                                  Enums                = Values.ENUMS_NAME,
                                                  EnumNaming           = Values.NAMING_AS_DECLARED,
                                                  NumbersFromStrings   = 1, // Disallow
                                                  LargeIntegers        = 1, // Number
                                                  NonFiniteFloats      = 1, // Error
                                                  LocalDateTimes       = Values.LOCAL_CONVERT_TO_UTC,
                                                  UnorderedCollections = Values.ORDER_SORTED,
                                                  Escaping             = 1, // Minimal
                                                  Indented             = Values.OFF,
                                                  IndentChar           = 1, // Tab
                                                  IndentSize           = 1,
                                                  AllowComments        = Values.OFF,
                                                  AllowTrailingCommas  = Values.OFF,
                                                  MaxDepth             = 64,
                                                  GenerateToString     = Values.ON
                                              };


    /// <summary> This layer, with <paramref name="nearer"/>'s explicit values on top. </summary>
    public Settings With( in Settings nearer ) => new()
                                                  {
                                                      Naming               = Pick(nearer.Naming,               Naming),
                                                      NameMatching         = Pick(nearer.NameMatching,         NameMatching),
                                                      NullValues           = Pick(nearer.NullValues,           NullValues),
                                                      DefaultValues        = Pick(nearer.DefaultValues,        DefaultValues),
                                                      UnknownMembers       = Pick(nearer.UnknownMembers,       UnknownMembers),
                                                      DuplicateMembers     = Pick(nearer.DuplicateMembers,     DuplicateMembers),
                                                      Enums                = Pick(nearer.Enums,                Enums),
                                                      EnumNaming           = Pick(nearer.EnumNaming,           EnumNaming),
                                                      NumbersFromStrings   = Pick(nearer.NumbersFromStrings,   NumbersFromStrings),
                                                      LargeIntegers        = Pick(nearer.LargeIntegers,        LargeIntegers),
                                                      NonFiniteFloats      = Pick(nearer.NonFiniteFloats,      NonFiniteFloats),
                                                      LocalDateTimes       = Pick(nearer.LocalDateTimes,       LocalDateTimes),
                                                      UnorderedCollections = Pick(nearer.UnorderedCollections, UnorderedCollections),
                                                      Escaping             = Pick(nearer.Escaping,             Escaping),
                                                      Indented             = Pick(nearer.Indented,             Indented),
                                                      IndentChar           = Pick(nearer.IndentChar,           IndentChar),
                                                      IndentSize           = Pick(nearer.IndentSize,           IndentSize),
                                                      AllowComments        = Pick(nearer.AllowComments,        AllowComments),
                                                      AllowTrailingCommas  = Pick(nearer.AllowTrailingCommas,  AllowTrailingCommas),
                                                      MaxDepth             = Pick(nearer.MaxDepth,             MaxDepth),
                                                      GenerateToString     = Pick(nearer.GenerateToString,     GenerateToString)
                                                  };

    private static int Pick( int nearer, int farther ) => nearer != 0
                                                              ? nearer
                                                              : farther;


    /// <summary> Reads a settings attribute's named arguments (unknown names are ignored, e.g. <c>Discriminator</c>, <c>Name</c>). </summary>
    public static Settings From( AttributeData? attribute )
    {
        if ( attribute is null ) { return default; }

        Settings settings = default;

        foreach ( KeyValuePair<string, TypedConstant> argument in attribute.NamedArguments )
        {
            if ( argument.Value.Value is not { } raw ) { continue; }

            int value;

            try { value = System.Convert.ToInt32(raw, System.Globalization.CultureInfo.InvariantCulture); }
            catch ( System.Exception ) { continue; } // strings, types, booleans: not settings

            settings = argument.Key switch
                       {
                           "Naming"               => settings with { Naming = value },
                           "NameMatching"         => settings with { NameMatching = value },
                           "NullValues"           => settings with { NullValues = value },
                           "DefaultValues"        => settings with { DefaultValues = value },
                           "UnknownMembers"       => settings with { UnknownMembers = value },
                           "DuplicateMembers"     => settings with { DuplicateMembers = value },
                           "Enums"                => settings with { Enums = value },
                           "EnumNaming"           => settings with { EnumNaming = value },
                           "NumbersFromStrings"   => settings with { NumbersFromStrings = value },
                           "LargeIntegers"        => settings with { LargeIntegers = value },
                           "NonFiniteFloats"      => settings with { NonFiniteFloats = value },
                           "LocalDateTimes"       => settings with { LocalDateTimes = value },
                           "UnorderedCollections" => settings with { UnorderedCollections = value },
                           "Escaping"             => settings with { Escaping = value },
                           "Indented"             => settings with { Indented = value },
                           "IndentChar"           => settings with { IndentChar = value },
                           "IndentSize"           => settings with { IndentSize = value },
                           "AllowComments"        => settings with { AllowComments = value },
                           "AllowTrailingCommas"  => settings with { AllowTrailingCommas = value },
                           "MaxDepth"             => settings with { MaxDepth = value },
                           "GenerateToString"     => settings with { GenerateToString = value },
                           _                      => settings
                       };
        }

        return settings;
    }
}



/// <summary> The settings enums' underlying values (they mirror Jakar.Json's public enums). </summary>
internal static class Values
{
    public const int ON  = 1;
    public const int OFF = 2;

    public const int NAMING_AS_DECLARED    = 1;
    public const int NAMING_CAMEL          = 2;
    public const int NAMING_PASCAL         = 3;
    public const int NAMING_SNAKE_LOWER    = 4;
    public const int NAMING_SNAKE_UPPER    = 5;
    public const int NAMING_KEBAB_LOWER    = 6;
    public const int NAMING_KEBAB_UPPER    = 7;
    public const int MATCHING_IGNORE_CASE  = 2;
    public const int NULLS_OMIT            = 2;
    public const int DEFAULTS_OMIT         = 2;
    public const int UNKNOWN_SKIP          = 1;
    public const int UNKNOWN_ERROR         = 2;
    public const int UNKNOWN_CAPTURE       = 3;
    public const int DUPLICATES_ERROR      = 1;
    public const int DUPLICATES_LAST_WINS  = 2;
    public const int ENUMS_NAME            = 1;
    public const int ENUMS_NUMBER          = 2;
    public const int NUMBERS_ALLOW_STRINGS = 2;
    public const int LARGE_AS_STRING       = 2;
    public const int NON_FINITE_AS_STRING  = 2;
    public const int LOCAL_CONVERT_TO_UTC  = 1;
    public const int LOCAL_WRITE_OFFSET    = 2;
    public const int LOCAL_ERROR           = 3;
    public const int ORDER_SORTED          = 1;
    public const int ORDER_ENUMERATION     = 2;
    public const int ESCAPING_MINIMAL      = 1;
    public const int ESCAPING_ASCII_ONLY   = 2;
    public const int ESCAPING_HTML_SAFE    = 3;
    public const int INDENT_SPACE          = 2;
}
