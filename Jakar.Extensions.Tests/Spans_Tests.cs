// Jakar.Extensions :: Jakar.Extensions.Tests
// 12/6/2023  13:2

using System.Linq;
using System.Numerics;
using System.Text;
using static Jakar.Extensions.Randoms;



namespace Jakar.Extensions.Tests;


[TestFixture][TestOf(typeof(Spans))]
public class Spans_Tests : Assert
{
    private static bool IsDevisableByTwo<TValue>( TValue x )
        where TValue : INumber<TValue> => x % ( TValue.One + TValue.One ) == TValue.Zero;
    private static bool IsDevisableByTwo<TValue>( ref readonly TValue x )
        where TValue : INumber<TValue> => x % ( TValue.One + TValue.One ) == TValue.Zero;


    [Test][TestCase(new[] { NUMERIC, UPPER_CASE }, new[] { NUMERIC, UPPER_CASE }, true)][TestCase(new[] { NUMERIC, UPPER_CASE }, new[] { UPPER_CASE, NUMERIC }, false)]
    public void SequenceEqual( string[] value, string[] other, bool expected )
    {
        ReadOnlySpan<string> span    = value;
        bool                 results = span.SequenceEqual(other);
        this.AreEqual(expected, results);
    }


    [Test][TestCase(ALPHANUMERIC, "abc", true)]
    public void Contains( string value, string other, bool expected )
    {
        ReadOnlySpan<char> valueSpan = value;
        ReadOnlySpan<char> otherSpan = other;
        bool               results   = valueSpan.Contains(otherSpan);
        this.AreEqual(expected, results);
    }


    [Test][TestCase(ALPHANUMERIC, "65bc", true)]
    public void ContainsAny( string value, string other, bool expected )
    {
        ReadOnlySpan<char> valueSpan = value;
        ReadOnlySpan<char> otherSpan = other;
        bool               results   = valueSpan.ContainsAny(otherSpan);
        this.AreEqual(expected, results);
    }


    [Test][TestCase(ALPHANUMERIC, "65bc", true)]
    public void ContainsAll( string value, string other, bool expected )
    {
        ReadOnlySpan<char> valueSpan = value;
        ReadOnlySpan<char> otherSpan = other;
        bool               results   = valueSpan.ContainsAll(otherSpan);
        this.AreEqual(expected, results);
    }


    [Test][TestCase(ALPHANUMERIC, "@&*^", true)][TestCase(ALPHANUMERIC, "@&*^AcPd", false)]
    public void ContainsNone( string value, string other, bool expected )
    {
        ReadOnlySpan<char> valueSpan = value;
        ReadOnlySpan<char> otherSpan = other;
        bool               results   = valueSpan.ContainsNone(otherSpan);
        this.AreEqual(expected, results);
    }


    [Test][TestCase("", true)][TestCase("  ", true)][TestCase("\t", true)][TestCase(" \t ", true)][TestCase("FULL", false)][TestCase("   VALUES", false)]
    public void IsNullOrWhiteSpace( string value, bool expected )
    {
        ReadOnlySpan<char> valueSpan = value;
        bool               results   = valueSpan.IsNullOrWhiteSpace();
        this.AreEqual(expected, results);
    }


    [Test][TestCase(NUMERIC, 0)][TestCase(NUMERIC, 2)]
    public void Enumerate( string value, int start )
    {
        ReadOnlySpan<char> valueSpan = value;

        foreach ( ( int index, char c ) in valueSpan.Enumerate(start) )
        {
            Console.WriteLine($"{index} : {start}");
            this.AreEqual(value[index], c);
            this.AreEqual(start,        index);
            start++;
        }
    }


    [Test][TestCase(NUMERIC, '9', 9)][TestCase(NUMERIC, '0', 0)][TestCase(NUMERIC, '5', 5)]
    public void LastIndexOf( string value, char c, int expected )
    {
        ReadOnlySpan<char> valueSpan = value;
        int                results   = Spans.LastIndexOf(in valueSpan, c, value.Length);
        this.AreEqual(expected, results);
    }


    [Test][TestCase(NUMERIC, '9', 1)][TestCase(NUMERIC + NUMERIC, '9', 2)]
    public void Count( string value, char c, int expected )
    {
        ReadOnlySpan<char> valueSpan = value;
        int                results   = valueSpan.Count(c);
        this.AreEqual(expected, results);
    }


    [Test][TestCase(NUMERIC)][TestCase(ALPHANUMERIC)]
    public void AsBytes( string value )
    {
        ReadOnlySpan<char> valueSpan = value;
        ReadOnlySpan<byte> results   = Spans.AsBytes(in valueSpan);
        this.AreEqual(Encoding.Unicode.GetBytes(value), results);
    }


    [Test][TestCase(NUMERIC)][TestCase(ALPHANUMERIC)]
    public void AsSegment( string value )
    {
        ReadOnlyMemory<byte> array = Encoding.UTF8.GetBytes(value);
        this.IsTrue(array.TryAsSegment(out ArraySegment<byte> segment));
        this.AreEqual(array.Span, segment.Array);
    }


    [Test][TestCase(NUMERIC)][TestCase(ALPHANUMERIC)]
    public void ConvertToString( string value )
    {
        ReadOnlyMemory<byte> array  = Encoding.Unicode.GetBytes(value);
        string               result = array.ConvertToString(Encoding.Unicode);
        Console.WriteLine(value);
        Console.WriteLine(result);
        this.AreEqual<char>(value, result);
    }


    [Test][TestCase(NUMERIC, '-', $"{NUMERIC}-----")][TestCase(UPPER_CASE, '-', $"{UPPER_CASE}-----")]
    public void TryCopyTo( string value, char c, string expected )
    {
        Span<char>         span      = stackalloc char[expected.Length];
        ReadOnlySpan<char> valueSpan = value;
        this.IsTrue(Spans.TryCopyTo(in valueSpan, ref span, c));
        string result = span.ToString();
        this.AreEqual<char>(expected, result);
    }


    [Test][TestCase(1)][TestCase(2)]
    public void Create( int expected )
    {
        int results = Spans.CreateSpan<int>(expected).Length;
        this.AreEqual(expected, results);
    }


    [Test][TestCase(1)][TestCase(1, 2)][TestCase(1, 2, 3)][TestCase(1, 2, 3, 4)][TestCase(1, 2, 3, 4, 5)]
    public void Create( params int[] values )
    {
        int length = values.Length;

        switch ( length )
        {
            case 1:
            {
                int results = Spans.Create(1).Length;
                this.AreEqual(length, results);
                return;
            }

            case 2:
            {
                int results = Spans.Create(1, 2).Length;
                this.AreEqual(length, results);
                return;
            }

            case 3:
            {
                int results = Spans.Create(1, 2, 3).Length;
                this.AreEqual(length, results);
                return;
            }

            case 4:
            {
                int results = Spans.Create(1, 2, 3, 4).Length;
                this.AreEqual(length, results);
                return;
            }

            case 5:
            {
                int results = Spans.Create(1, 2, 3, 4, 5).Length;
                this.AreEqual(length, results);
                return;
            }
        }
    }


    [Test][TestCase(1d)][TestCase(1, 2)][TestCase(1, 2, 3)][TestCase(1, 2, 3, 4)][TestCase(1, 2, 3, 4, 5)]
    public void Average( params double[] values )
    {
        this.AreEqual(values.Average(), Spans.Average(values));
        this.AreEqual(values.Average(), Spans.Average<double>(values));
    }


    [Test][TestCase(1d)][TestCase(1, 2)][TestCase(1, 2, 3)][TestCase(1, 2, 3, 4)][TestCase(1, 2, 3, 4, 5)]
    public void Max( params double[] values )
    {
        ReadOnlySpan<double> valueSpan = values;
        double               results   = valueSpan.Max(double.MinValue);
        this.AreEqual(values.Max(), results);
    }


    [Test][TestCase(1d)][TestCase(1, 2)][TestCase(1, 2, 3)][TestCase(1, 2, 3, 4)][TestCase(1, 2, 3, 4, 5)]
    public void Min( params double[] values )
    {
        ReadOnlySpan<double> valueSpan = values;
        double               results   = valueSpan.Min(double.MaxValue);
        this.AreEqual(values.Min(), results);
    }


    [Test][TestCase(1d)][TestCase(1, 2)][TestCase(1, 2, 3)][TestCase(1, 2, 3, 4)][TestCase(1, 2, 3, 4, 5)]
    public void Sum( params double[] values )
    {
        ReadOnlySpan<double> valueSpan = values;
        double               results   = valueSpan.Sum();
        this.AreEqual(values.Sum(), results);
    }


    [Test][TestCase(1, 2)][TestCase(1, 2, 3)][TestCase(1, 2, 3, 4)][TestCase(1, 2, 3, 4, 5)]
    public void First( params double[] values )
    {
        ReadOnlySpan<double> valueSpan = values;
        double               results   = valueSpan.SingleOrDefault(static ( ref readonly double x ) => IsDevisableByTwo(x));
        this.AreEqual(values.First(IsDevisableByTwo), results);
    }


    [Test][TestCase(1d)][TestCase(1, 2)][TestCase(1, 2, 3)][TestCase(1, 2, 3, 4)][TestCase(1, 2, 3, 4, 5)]
    public void FirstOrDefault( params double[] values )
    {
        ReadOnlySpan<double> valueSpan = values;
        double               results   = valueSpan.FirstOrDefault(static ( ref readonly double x ) => IsDevisableByTwo(x));
        this.AreEqual(values.FirstOrDefault(IsDevisableByTwo), results);
    }


    [Test][TestCase(1, 2)][TestCase(1, 2, 3)][TestCase(1, 2, 3, 4)][TestCase(1, 2, 3, 4, 5)]
    public void Single( params double[] values )
    {
        int count = values.Count(IsDevisableByTwo);

        if ( count > 1 )
        {
            Catch(() =>
                  {
                      ReadOnlySpan<double> valueSpan = values;
                      double               results   = valueSpan.Single(static ( ref readonly double x ) => IsDevisableByTwo(x));
                      this.AreEqual(values.Single(IsDevisableByTwo), results);
                  });
        }
        else
        {
            ReadOnlySpan<double> valueSpan = values;
            double               results   = valueSpan.Single(static ( ref readonly double x ) => IsDevisableByTwo(x));
            this.AreEqual(values.SingleOrDefault(IsDevisableByTwo), results);
        }
    }


    [Test][TestCase(1d)][TestCase(1, 2)][TestCase(1, 2, 3)][TestCase(1, 2, 3, 4)][TestCase(1, 2, 3, 4, 5)]
    public void SingleOrDefault( params double[] values )
    {
        int count = values.Count(IsDevisableByTwo);

        if ( count > 1 )
        {
            Catch(() =>
                  {
                      ReadOnlySpan<double> valueSpan = values;
                      double               results   = valueSpan.SingleOrDefault(static ( ref readonly double x ) => IsDevisableByTwo(x));
                      this.AreEqual(values.SingleOrDefault(IsDevisableByTwo), results);
                  });
        }
        else
        {
            ReadOnlySpan<double> valueSpan = values;
            double               results   = valueSpan.SingleOrDefault(static ( ref readonly double x ) => IsDevisableByTwo(x));
            this.AreEqual(values.SingleOrDefault(IsDevisableByTwo), results);
        }
    }


    [Test][TestCase(NUMERIC, '9', true)]
    public void EndsWith( string value, char c, bool expected )
    {
        ReadOnlySpan<char> valueSpan = value;
        bool               results   = Spans.EndsWith(in valueSpan, c);
        this.AreEqual(expected, results);
    }


    [Test][TestCase(NUMERIC, "0", true)]
    public void StartsWith( string value, string c, bool expected )
    {
        ReadOnlySpan<char> valueSpan = value;
        bool               results   = Spans.StartsWith(in valueSpan, c);
        this.AreEqual(expected, results);
    }


    [Test][TestCase("Abc_a154dbz123", "XYZ", "Abc_a154dbz123XYZ")]
    public void Join( string value, string other, string expected )
    {
        ReadOnlySpan<char> result = Spans.Join<char>(value, other);

        Console.WriteLine($"{nameof(result)}  : {result}");
        Console.WriteLine($"{nameof(expected)}: {expected}");

        this.AreEqual(expected, result);
    }


    [Test][TestCase("Abc_a1524dbz123", "1", "Abc_a524dbz23")][TestCase("Abc_a1524dbz123", "2", "Abc_a154dbz13")]
    public void RemoveAll( string value, string other, string expected )
    {
        ReadOnlySpan<char> result = Spans.Remove<char>(value, other);
        this.AreEqual(expected, result);
    }


    [Test][TestCase("Abc_a1524dbz123", "1", "z", @"Abc_az524dbzz23")][TestCase("Abc_a1524dbz123", "2", "4", "Abc_a1544dbz143")]
    public void Replace( string value, string oldValue, string newValue, string expected )
    {
        ReadOnlySpan<char> result = Spans.Replace<char>(value, oldValue, newValue);

        Console.WriteLine($"{nameof(result)}  : {result}");
        Console.WriteLine($"{nameof(expected)}: {expected}");

        this.AreEqual(expected, result);
    }


    [Test][TestCase("Abc_a1524dbz123", 'a', 'z', false, "1524db")][TestCase("Abc_a1524dbz123", 'a', 'z', true, "a1524dbz")]
    public void Slice( string value, char start, char end, bool includeEnds, string expected )
    {
        ReadOnlySpan<char> valueSpan = value;
        ReadOnlySpan<char> result    = valueSpan.Slice(start, end, includeEnds);
        this.AreEqual(expected, result.ToString());
    }
}
