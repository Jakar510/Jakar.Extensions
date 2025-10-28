namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Validate))]
public class ValidateTests : Assert
{
    [Test] [TestCase("wwww.google.com", false)] [TestCase("http://google.com", true)] [TestCase("http://wwww.google.com", true)] [TestCase("google.com", false)]
    public void Web( string value, bool expected )
    {
        Console.WriteLine($"{nameof(value)}: '{value}'");
        Console.WriteLine($"{nameof(expected)}: '{expected}'");
        this.AreEqual(expected, value.IsWebAddress());
    }

    [Test] [TestCase(0, false)] [TestCase(10, true)] [TestCase(8080, true)] [TestCase(65534, true)] [TestCase(655350, false)]
    public void Port( int value, bool expected )
    {
        Console.WriteLine($"{nameof(value)}: '{value}'");
        Console.WriteLine($"{nameof(expected)}: '{expected}'");
        this.AreEqual(expected, value.IsValidPort());
    }

    [Test] [TestCase("0", true)] [TestCase("10", true)] [TestCase("8080", true)] [TestCase("655a35", false)] [TestCase("65 5350", false)] [TestCase("65..0", false)] [TestCase("65.0", false)]
    public void IsInteger( string value, bool expected )
    {
        Console.WriteLine($"{nameof(value)}: '{value}'");
        Console.WriteLine($"{nameof(expected)}: '{expected}'");
        this.AreEqual(expected, value.IsInteger());
    }

    [Test] [TestCase("demo ", true)] [TestCase("dem o", false)] [TestCase("655a35", false)] [TestCase("65 5350", false)] [TestCase("DEMO", true)] [TestCase("demo", true)] [TestCase("Demo", true)]
    public void IsDemo( string value, bool expected )
    {
        Console.WriteLine($"{nameof(value)}: '{value}'");
        Console.WriteLine($"{nameof(expected)}: '{expected}'");
        this.AreEqual(expected, value.IsDemo());
    }


    [Test] [TestCase(1, "1")] [TestCase(1d, "1")] [TestCase(1.1, "1.1")] [TestCase(1.01, "1.01")] [TestCase(1.001, "1.001")] [TestCase(1.0001, "1.0001")] [TestCase(1.00001, "1")] [TestCase(1.00009, "1.0001")]
    public void FormatNumber( double value, string expected )
    {
        Console.WriteLine($"{nameof(value)}: '{value}'");
        Console.WriteLine($"{nameof(expected)}: '{expected}'");
        this.AreEqual(expected, value.FormatNumber());
    }


    [Test] [TestCase(1, "1")] [TestCase(1f, "1")] [TestCase(1.1f, "1.1")] [TestCase(1.01f, "1.01")] [TestCase(1.001f, "1.001")] [TestCase(1.0001f, "1.0001")] [TestCase(1.00001f, "1")] [TestCase(1.00009f, "1.0001")]
    public void FormatNumber( float value, string expected )
    {
        Console.WriteLine($"{nameof(value)}: '{value}'");
        Console.WriteLine($"{nameof(expected)}: '{expected}'");
        this.AreEqual(expected, value.FormatNumber());
    }


    [Test] [TestCase(1, "1")] [TestCase(1d, "1")] [TestCase(1.1, "1.1")] [TestCase(1.01, "1.01")] [TestCase(1.001, "1.001")] [TestCase(1.0001, "1.0001")] [TestCase(1.00001, "1")] [TestCase(1.00009, "1.0001")]
    public void FormatNumber( decimal value, string expected )
    {
        Console.WriteLine($"{nameof(value)}: '{value}'");
        Console.WriteLine($"{nameof(expected)}: '{expected}'");
        this.AreEqual(expected, value.FormatNumber());
    }


    [Test] [TestCase("bob@bob.com", true)] [TestCase("bob@random.com", true)] [TestCase("bob@random.net", true)] [TestCase("bob@random.co", true)] [TestCase("bob-tom@random.com", true)] [TestCase("bob_tom@random.com", true)] [TestCase("bob@random", false)] [TestCase("bob~@random.com", true)] [TestCase("bob@random,com", false)]
    public void IsEmailAddress( string value, bool expected )
    {
        Console.WriteLine($"{nameof(value)}: '{value}'");
        Console.WriteLine($"{nameof(expected)}: '{expected}'");
        this.AreEqual(expected, value.IsEmailAddress());
    }

    [Test] [TestCase("0", true)] [TestCase("10", true)] [TestCase("8080", true)] [TestCase("655a35", false)] [TestCase("65 5350", false)] [TestCase("65..0", false)] [TestCase("65.0", true)]
    public void IsDouble( string value, bool expected )
    {
        Console.WriteLine($"{nameof(value)}: '{value}'");
        Console.WriteLine($"{nameof(expected)}: '{expected}'");
        this.AreEqual(expected, value.IsDouble());
    }


    [Test] [TestCase("1.1.1.1", true)] [TestCase("1.1.1.1 ", true)] [TestCase(" 1.1.1.1", true)] [TestCase(" 1.1.1.1 ", true)] [TestCase("1.1 .1.1", false)] [TestCase("1.1..1.1", false)] [TestCase("1.111.1.1", true)] [TestCase("1.1111.1.1", false)] [TestCase("11.11.11.11", true)] [TestCase("111.111.111.111", true)] [TestCase("111.111.111.1111", false)]
    public void Ip( string value, bool expected )
    {
        Console.WriteLine($"{nameof(value)}: '{value}'");
        Console.WriteLine($"{nameof(expected)}: '{expected}'");
        this.AreEqual(expected, value.IsIPAddress());
    }
}
