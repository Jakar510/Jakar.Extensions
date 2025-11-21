// Jakar.Extensions :: Jakar.Extensions.Experiments
// 09/19/2025  17:14

using System.Text.Json.Serialization.Metadata;



namespace Jakar.Extensions.Experiments.Benchmarks;


public sealed record Node : BaseRecord<Node>, IJsonModel<Node>
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

            return Generate((int)depth)
               .ToArray();
        }
    }
}



[SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeEvident")]
public sealed class TestJson : BaseClass<TestJson>, IJsonModel<TestJson>
{
    internal static readonly TestJson Debug = new()
                                              {
                                                  Nodes = Node.NodeFaker.Instance.Generate(5)
                                                              .ToArray(),
                                                  Errors = Errors.Create(Error.Accepted(), Error.BadRequest()),
                                                  Files  = [new FileData(0, "Hash", "payload", new FileMetaData("file.dat", MimeTypeNames.Application.BINARY, MimeType.Binary))],
                                                  Location = new CurrentLocation
                                                             {
                                                                 Altitude         = Random.Shared.NextDouble() * 20000,
                                                                 Latitude         = Random.Shared.NextDouble() * 20000,
                                                                 Longitude        = Random.Shared.NextDouble() * 20000,
                                                                 Accuracy         = Random.Shared.NextDouble() * 100,
                                                                 VerticalAccuracy = Random.Shared.NextDouble() * 100,
                                                                 Speed            = Random.Shared.NextDouble() * 100,
                                                                 Course           = Random.Shared.NextDouble() * 360,
                                                                 InstanceID       = Guid.NewGuid(),
                                                                 ID               = Guid.NewGuid(),
                                                                 Timestamp        = DateTimeOffset.UtcNow
                                                             },
                                                  CreateUser = new CreateUserModel
                                                               {
                                                                   ID        = Guid.NewGuid(),
                                                                   UserID    = Guid.NewGuid(),
                                                                   UserName  = "Jonny",
                                                                   FirstName = "John",
                                                                   LastName  = "Doe",
                                                                   Email     = "john.doe@mail.com"
                                                               },
                                                  User = new UserModel
                                                         {
                                                             ID        = Guid.NewGuid(),
                                                             UserID    = Guid.NewGuid(),
                                                             UserName  = "Jonny",
                                                             FirstName = "John",
                                                             LastName  = "Doe",
                                                             Email     = "john.doe@mail.com"
                                                         }
                                              };
    public Email Email         = new("bite@me.com");
    public Error MutableError  = Error.InternalServerError();
    public Error ReadOnlyError = Error.InternalServerError();
    public Pair  Pair          = new("date", DateTime.Now.ToLongDateString());


    public CreateUserModel                CreateUser { get; set; } = new();
    public Errors                         Errors     { get; set; } = Errors.Empty;
    public ObservableCollection<FileData> Files      { get; set; } = [];
    public CurrentLocation                Location   { get; set; } = new();
    public Node[]                         Nodes      { get; set; } = [];
    public UserModel                      User       { get; set; } = new();


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
