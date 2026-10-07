// Jakar.Json.Tests
// 10/07/2026

using System.Numerics;



namespace Jakar.Json.Tests;


public enum Status
{
    Draft,
    Sent,
    Paid,
    Void
}



[Flags]
public enum Permissions
{
    None                             = 0,
    Read                             = 1,
    Write                            = 2,
    Delete                           = 4,
    [JsonEnumName("everything")] All = Read | Write | Delete
}



/// <summary> The SPEC.md §4.3 example. </summary>
[GenerateJson(Naming = JsonNaming.CamelCase, UnknownMembers = JsonUnknownMembers.Error)]
public sealed partial record Invoice( Guid Id, decimal Total )
{
    public required List<LineItem> Lines { get; init; }

    [JsonMember(Name = "memo", Order = -1, NullValues = JsonNullValues.Omit)] public string? Notes { get; init; }

    [JsonMember(Ignore = true)] public decimal Tax => Total * 0.07m;

    public Status         Status  { get; init; }
    public DateTimeOffset Created { get; init; }
}



[GenerateJson(Naming = JsonNaming.CamelCase)] public sealed partial record LineItem( string Sku, int Quantity, decimal Price );



/// <summary> Every scalar the converters support, with defaults that a missing member must keep. </summary>
[GenerateJson]
public sealed partial class Scalars
{
    public bool           Flag      { get; set; }
    public byte           Byte      { get; set; }
    public sbyte          SByte     { get; set; }
    public short          Short     { get; set; }
    public ushort         UShort    { get; set; }
    public int            Int       { get; set; }
    public uint           UInt      { get; set; }
    public long           Long      { get; set; }
    public ulong          ULong     { get; set; }
    public Int128         Int128    { get; set; }
    public UInt128        UInt128   { get; set; }
    public BigInteger     Big       { get; set; }
    public float          Single    { get; set; }
    public double         Double    { get; set; }
    public Half           Half      { get; set; }
    public decimal        Decimal   { get; set; }
    public char           Char      { get; set; } = 'x';
    public string         Text      { get; set; } = "";
    public string?        Maybe     { get; set; }
    public Guid           Guid      { get; set; }
    public DateTime       DateTime  { get; set; }
    public DateTimeOffset Offset    { get; set; }
    public DateOnly       Date      { get; set; }
    public TimeOnly       Time      { get; set; }
    public TimeSpan       Span      { get; set; }
    public Uri?           Link      { get; set; }
    public Version?       Version   { get; set; }
    public Status         Status    { get; set; }
    public Permissions    Rights    { get; set; }
    public int?           NullInt   { get; set; }
    public Status?        NullEnum  { get; set; }
    public List<int>      Defaulted { get; set; } = [1, 2, 3];
}



/// <summary> Every collection shape. </summary>
[GenerateJson]
public sealed partial class Collections
{
    public int[]                               Array         { get; set; } = [];
    public List<string>                        List          { get; set; } = [];
    public IEnumerable<int>                    Enumerable    { get; set; } = [];
    public IReadOnlyList<long>                 ReadOnlyList  { get; set; } = [];
    public ImmutableArray<int>                 Immutable     { get; set; } = [];
    public ImmutableList<int>                  ImmutableList { get; set; } = [];
    public HashSet<string>                     Set           { get; set; } = [];
    public FrozenSet<int>                      Frozen        { get; set; } = FrozenSet<int>.Empty;
    public SortedSet<int>                      Sorted        { get; set; } = [];
    public Queue<int>                          Queue         { get; set; } = new();
    public Stack<int>                          Stack         { get; set; } = new();
    public Dictionary<string, int>             Map           { get; set; } = [];
    public IReadOnlyDictionary<Guid, string>   GuidMap       { get; set; } = new Dictionary<Guid, string>();
    public Dictionary<Status, LineItem>        EnumMap       { get; set; } = [];
    public SortedDictionary<int, string>       SortedMap     { get; set; } = [];
    public OrderedDictionary<string, int>      OrderedMap    { get; set; } = [];
    public ImmutableDictionary<string, int>    ImmutableMap  { get; set; } = ImmutableDictionary<string, int>.Empty;
    public FrozenDictionary<string, int>       FrozenMap     { get; set; } = FrozenDictionary<string, int>.Empty;
    public ConcurrentDictionary<string, int>   ConcurrentMap { get; set; } = new();
    public List<List<int>>                     Nested        { get; set; } = [];
    public List<string?>                       NullableItems { get; set; } = [];
    public Dictionary<string, List<LineItem>>? Optional      { get; set; }
}
