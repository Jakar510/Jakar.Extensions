// Jakar.Extensions :: Jakar.Extensions.Experiments
// 09/19/2025  17:14

namespace Experiments.Benchmarks;


[JsonModel]
public sealed partial record Node : BaseRecord<Node>
{
    private static readonly Node[]         __empty = [];
    public                  Node[]         Children    { get; init; } = __empty;
    public                  DateTimeOffset Date        { get; init; }
    public                  string         Description { get; init; } = EMPTY;
    public                  string         Name        { get; init; } = EMPTY;
    public                  double         Price       { get; init; }


    public Node() { }
    public Node( string name, string value, double price, DateTimeOffset date ) : this(name, value, price, date, null) { }
    public Node( string name, string value, double price, DateTimeOffset date, params Node[]? children )
    {
        Name        = name;
        Description = value;
        Price       = price;
        Date        = date;
        Children    = children ?? __empty;
    }


    public override bool Equals( Node?    other )             => ReferenceEquals(this, other) || string.Equals(Name, other?.Name, StringComparison.InvariantCultureIgnoreCase);
    public override int  CompareTo( Node? other )             => string.Compare(Name, other?.Name, StringComparison.InvariantCultureIgnoreCase);
    public override int  GetHashCode()                        => HashCode.Combine(Name, Description, Price, Date, Children);
    public static   bool operator >( Node  left, Node right ) => false;
    public static   bool operator >=( Node left, Node right ) => false;
    public static   bool operator <( Node  left, Node right ) => false;
    public static   bool operator <=( Node left, Node right ) => false;



    public sealed class NodeFaker : Faker<Node>
    {
        private static         uint      __depth  = (uint)Random.Shared.Next(5, 10);
        public static readonly NodeFaker Instance = new();


        public NodeFaker()
        {
            RuleFor(static x => x.Name,        f => f.Commerce.ProductName());
            RuleFor(static x => x.Description, f => f.Commerce.ProductAdjective());
            RuleFor(static x => x.Price,       f => f.Random.Double(1, 100));
            RuleFor(static x => x.Date,        f => DateTimeOffset.UtcNow - TimeSpan.FromDays(f.Random.Int(-10, 10)));
            RuleFor(static x => x.Children,    GetChildren);
        }
        private Node[] GetChildren( Faker f )
        {
            if ( __depth <= 0 ) { return []; }

            uint depth = __depth;
            __depth = depth / 2;

            return [.. Generate((int)depth)];
        }
    }
}



[SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeEvident")]
[JsonModel]
public sealed partial class TestJson : BaseClass<TestJson>
{
    // Jakar.Extensions types only: the user, error and file models moved to Jakar.Users / Jakar.ErrorOr in 10.12.
    internal static readonly TestJson Debug = new() { Nodes = [.. Node.NodeFaker.Instance.Generate(5)] };
    public Email Email = new("bite@me.com");
    public Pair  Pair  = new("date", DateTime.Now.ToLongDateString());


    public Node[] Nodes { get; set; } = [];


    public static void Print()
    {
        string json = Debug.ToJson();
        Console.WriteLine();
        Console.WriteLine(json);
        Console.WriteLine();
    }
    public override bool Equals( TestJson?      other )                 => ReferenceEquals(this, other);
    public override int  CompareTo( TestJson?   other )                 => 0;
    public static   bool operator ==( TestJson? left, TestJson? right ) => EqualityComparer<TestJson>.Default.Equals(left, right);
    public static   bool operator !=( TestJson? left, TestJson? right ) => !EqualityComparer<TestJson>.Default.Equals(left, right);
    public static   bool operator >( TestJson   left, TestJson  right ) => Comparer<TestJson>.Default.Compare(left, right) > 0;
    public static   bool operator >=( TestJson  left, TestJson  right ) => Comparer<TestJson>.Default.Compare(left, right) >= 0;
    public static   bool operator <( TestJson   left, TestJson  right ) => Comparer<TestJson>.Default.Compare(left, right) < 0;
    public static   bool operator <=( TestJson  left, TestJson  right ) => Comparer<TestJson>.Default.Compare(left, right) <= 0;
}
