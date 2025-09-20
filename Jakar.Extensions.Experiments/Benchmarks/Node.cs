// Jakar.Extensions :: Jakar.Extensions.Experiments
// 09/19/2025  17:14

using System.Text.Json.Serialization.Metadata;
using Jakar.Extensions.UserGuid;



namespace Jakar.Extensions.Experiments.Benchmarks;


[JsonSerializable(typeof(TestJson)), JsonSerializable(typeof(Node))]
public sealed partial class ExperimentContext : JsonSerializerContext
{
    public static JsonSerializerOptions Pretty => new(Default.Options) { WriteIndented = true };
}



public sealed record Node : BaseRecord<Node>, IEqualComparable<Node>, IJsonModel<Node>
{
    private static readonly Node[]                __empty = [];
    public static           JsonSerializerContext JsonContext  => ExperimentContext.Default;
    public static           JsonTypeInfo<Node>    JsonTypeInfo => ExperimentContext.Default.Node;
    public                  Node[]                Children     { get; init; } = __empty;
    public                  DateTimeOffset        Date         { get; init; }
    public                  string                Description  { get; init; } = string.Empty;
    public                  string                Name         { get; init; } = string.Empty;
    public                  double                Price        { get; init; }


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
        public static readonly NodeFaker Instance = new();
        private static         uint      __depth  = (uint)Random.Shared.Next(5, 10);


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
            return Generate((int)depth).ToArray();
        }
    }
}



[SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeEvident")]
public sealed class TestJson
{
    public Email email         = new("bite@me.com");
    public Error mutableError  = Error.InternalServerError();
    public Error readOnlyError = Error.InternalServerError();
    public Pair  pair          = new("date", DateTime.Now.ToLongDateString());


    public static JsonSerializerContext          JsonContext  => ExperimentContext.Default;
    public static JsonTypeInfo<TestJson>         JsonTypeInfo => ExperimentContext.Default.TestJson;
    public        CreateUserModel                CreateUser   { get; set; } = new();
    public        Errors                         Errors       { get; set; } = Errors.Create(Error.Accepted(), Error.BadRequest());
    public        ObservableCollection<FileData> Files        { get; set; } = [new FileData(0, "Hash", "payload", new FileMetaData("file.dat", MimeTypeNames.Application.BINARY, MimeType.Binary))];
    public        CurrentLocation                Location     { get; set; } = new();
    public        UserModel                      User         { get; set; } = new();
}
