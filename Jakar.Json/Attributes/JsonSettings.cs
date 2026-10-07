// Jakar.Json
// 10/07/2026

namespace Jakar.Json;


// Every settings enum starts with Inherit = 0: attribute properties can't be nullable, so "not set" means "use the next level up"
// (member → type → assembly → built-in default, SPEC.md §4.1). Options treat Inherit as the built-in default.



public enum JsonToggle : byte
{
    Inherit,
    On,
    Off
}



public enum JsonNaming : byte
{
    Inherit,
    AsDeclared,
    CamelCase,
    PascalCase,
    SnakeCaseLower,
    SnakeCaseUpper,
    KebabCaseLower,
    KebabCaseUpper
}



public enum JsonNameMatching : byte
{
    Inherit,
    Exact,
    OrdinalIgnoreCase
}



public enum JsonNullValues : byte
{
    Inherit,
    Write,
    Omit
}



public enum JsonDefaultValues : byte
{
    Inherit,
    Write,
    Omit
}



public enum JsonUnknownMembers : byte
{
    Inherit,
    Skip,
    Error,
    Capture
}



public enum JsonDuplicateMembers : byte
{
    Inherit,
    Error,
    LastWins
}



public enum JsonEnumFormat : byte
{
    Inherit,
    Name,
    Number
}



public enum JsonNumbersFromStrings : byte
{
    Inherit,
    Disallow,
    Allow
}



public enum JsonLargeIntegers : byte
{
    Inherit,
    Number,
    String
}



public enum JsonNonFiniteFloats : byte
{
    Inherit,
    Error,
    AsString
}



public enum JsonLocalDateTimes : byte
{
    Inherit,
    ConvertToUtc,
    WriteOffset,
    Error
}



public enum JsonUnorderedCollections : byte
{
    Inherit,
    Sorted,
    Enumeration
}



/// <summary> Which characters a writer escapes besides <c>"</c>, <c>\</c> and control characters. </summary>
public enum JsonEscaping : byte
{
    Inherit,
    /// <summary> Only what JSON requires (the default). </summary>
    Minimal,
    /// <summary> Also everything outside ASCII. </summary>
    AsciiOnly,
    /// <summary> Also <c>&lt; &gt; &amp; ' +</c>, so the output is safe inside HTML. </summary>
    HtmlSafe
}



public enum JsonIndentChar : byte
{
    Inherit,
    Tab,
    Space
}



/// <summary> The settings shared by <see cref="JsonDefaultsAttribute"/> (assembly) and <see cref="GenerateJsonAttribute"/> (type). See SPEC.md §4.2. </summary>
public abstract class JsonSettingsAttribute : Attribute
{
    public JsonNaming               Naming               { get; set; }
    public JsonNameMatching         NameMatching         { get; set; }
    public JsonNullValues           NullValues           { get; set; }
    public JsonDefaultValues        DefaultValues        { get; set; }
    public JsonUnknownMembers       UnknownMembers       { get; set; }
    public JsonDuplicateMembers     DuplicateMembers     { get; set; }
    public JsonEnumFormat           Enums                { get; set; }
    public JsonNaming               EnumNaming           { get; set; }
    public JsonNumbersFromStrings   NumbersFromStrings   { get; set; }
    public JsonLargeIntegers        LargeIntegers        { get; set; }
    public JsonNonFiniteFloats      NonFiniteFloats      { get; set; }
    public JsonLocalDateTimes       LocalDateTimes       { get; set; }
    public JsonUnorderedCollections UnorderedCollections { get; set; }
    public JsonEscaping             Escaping             { get; set; }
    public JsonToggle               Indented             { get; set; }
    public JsonIndentChar           IndentChar           { get; set; }

    /// <summary> 0 inherits. </summary>
    public int IndentSize { get; set; }

    public JsonToggle AllowComments       { get; set; }
    public JsonToggle AllowTrailingCommas { get; set; }

    /// <summary> 0 inherits. </summary>
    public int MaxDepth { get; set; }

    /// <summary> Emit <c>ToString()</c> returning compact JSON (default <see cref="JsonToggle.On"/>), unless the type already overrides it. </summary>
    public JsonToggle GenerateToString { get; set; }
}



/// <summary> Assembly-wide defaults for every <c>[GenerateJson]</c> type in this assembly. </summary>
[AttributeUsage(AttributeTargets.Assembly)]
[Conditional("JAKAR_JSON_KEEP_ATTRIBUTES")]
public sealed class JsonDefaultsAttribute : JsonSettingsAttribute;



/// <summary>
///     Source-generates JSON reading and writing for a <see langword="partial"/> class, struct, record or record struct: <see cref="IJsonSerializable{TSelf}"/>,
///     <see cref="ISpanFormattable"/> / <see cref="ISpanParsable{TSelf}"/> (and their UTF-8 counterparts), and <c>ToJson</c> / <c>FromJson</c> helpers.
///     <code>
///     [GenerateJson(Naming = JsonNaming.CamelCase)]
///     public sealed partial record Invoice( Guid Id, decimal Total );
///     </code>
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
[Conditional("JAKAR_JSON_KEEP_ATTRIBUTES")]
public sealed class GenerateJsonAttribute : JsonSettingsAttribute
{
    /// <summary> The member that names the concrete type when <see cref="JsonDerivedAttribute"/>s are present (default <c>"$type"</c>). </summary>
    public string? Discriminator { get; set; }
}



/// <summary> Per-member overrides. </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, Inherited = true)]
[Conditional("JAKAR_JSON_KEEP_ATTRIBUTES")]
public sealed class JsonMemberAttribute : Attribute
{
    /// <summary> The JSON name, used verbatim (the naming policy doesn't apply). </summary>
    public string? Name { get; set; }

    /// <summary> Members are written by <see cref="Order"/> (default 0), then by declaration. </summary>
    public int Order { get; set; }

    public bool Ignore { get; set; }

    /// <summary> Reading fails when the member is missing. Constructor parameters without defaults and <see langword="required"/> members are always required. </summary>
    public bool Required { get; set; }

    /// <summary> Collects unknown members (with <c>UnknownMembers = Capture</c>): an <c>OrderedDictionary&lt;string, JNode?&gt;</c> or <c>JObjectNode</c>. </summary>
    public bool ExtensionData { get; set; }

    /// <summary> A <see langword="struct"/> (or class) implementing <c>IJsonConverter&lt;T&gt;</c> for the member's type. </summary>
    public Type? Converter { get; set; }

    public JsonNullValues           NullValues           { get; set; }
    public JsonDefaultValues        DefaultValues        { get; set; }
    public JsonEnumFormat           Enums                { get; set; }
    public JsonNumbersFromStrings   NumbersFromStrings   { get; set; }
    public JsonLargeIntegers        LargeIntegers        { get; set; }
    public JsonNonFiniteFloats      NonFiniteFloats      { get; set; }
    public JsonLocalDateTimes       LocalDateTimes       { get; set; }
    public JsonUnorderedCollections UnorderedCollections { get; set; }
}



/// <summary> A concrete type of a polymorphic <c>[GenerateJson]</c> base, and the discriminator value that names it. </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
[Conditional("JAKAR_JSON_KEEP_ATTRIBUTES")]
public sealed class JsonDerivedAttribute( Type type, string tag ) : Attribute
{
    public Type   Type { get; } = type;
    public string Tag  { get; } = tag;
}



/// <summary> The JSON name of an enum value. Kept in metadata (not conditional): an assembly that uses the enum generates its codec from it. </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class JsonEnumNameAttribute( string name ) : Attribute
{
    public string Name { get; } = name;
}
