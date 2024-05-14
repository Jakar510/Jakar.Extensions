// Jakar.Extensions :: Jakar.Extensions.Tests
// 12/6/2023  13:2

using System.Linq;
using System.Numerics;
using System.Text;
using static Jakar.Extensions.Randoms;



namespace Jakar.Extensions.Tests.SpanAndMemory;


[TestFixture, TestOf( typeof(Spans) )]
public class SpansTests : Assert
{
    private static bool IsDevisableByTwo<T>( T x )
        where T : INumber<T> => x % (T.One + T.One) == T.Zero;


    [Test, TestCase( new[] { NUMERIC, UPPER_CASE }, new[] { NUMERIC, UPPER_CASE }, true ), TestCase( new[] { NUMERIC, UPPER_CASE }, new[] { UPPER_CASE, NUMERIC }, false )]
    public void SequenceEqual( string[] value, string[] other, bool expected )
    {
        ReadOnlySpan<string> span    = value;
        bool                 results = span.SequenceEqual( other );
        this.AreEqual( expected, results );
    }


    [Test, TestCase( ALPHANUMERIC, "abc", true )]
    public void Contains( string value, string other, bool expected )
    {
        bool results = Spans.Contains( value, other );
        this.AreEqual( expected, results );
    }


    [Test, TestCase( ALPHANUMERIC, "65bc", true )]
    public void ContainsAny( string value, string other, bool expected )
    {
        bool results = Spans.ContainsAny( value, other );
        this.AreEqual( expected, results );
    }


    [Test, TestCase( ALPHANUMERIC, "65bc", true )]
    public void ContainsAll( string value, string other, bool expected )
    {
        bool results = Spans.ContainsAll<char>( value, other );
        this.AreEqual( expected, results );
    }


    [Test, TestCase( ALPHANUMERIC, "@&*^", true )]
    public void ContainsNone( string value, string other, bool expected )
    {
        bool results = Spans.ContainsNone( value, other );
        this.AreEqual( expected, results );
    }


    [Test, TestCase( "", true ), TestCase( "  ", true ), TestCase( "\t", true ), TestCase( " \t ", true ), TestCase( "FULL", false ), TestCase( "   VALUES", false )]
    public void IsNullOrWhiteSpace( string value, bool expected )
    {
        bool results = Spans.IsNullOrWhiteSpace( value );
        this.AreEqual( expected, results );
    }


    [Test, TestCase( NUMERIC, 0 ), TestCase( NUMERIC, 2 )]
    public void Enumerate( string value, int start )
    {
        foreach ( (int index, char c) in Spans.Enumerate<char>( value, start ) )
        {
            Console.WriteLine( $"{index} : {start}" );
            this.AreEqual( value[index], c );
            this.AreEqual( start,        index );
            start++;
        }
    }


    [Test, TestCase( NUMERIC, '9', 9 ), TestCase( NUMERIC, '0', 0 ), TestCase( NUMERIC, '5', 5 )]
    public void LastIndexOf( string value, char c, int expected )
    {
        int results = Spans.LastIndexOf( value, c, value.Length );
        this.AreEqual( expected, results );
    }


    [Test, TestCase( NUMERIC, '9', 1 ), TestCase( NUMERIC + NUMERIC, '9', 2 )]
    public void Count( string value, char c, int expected )
    {
        int results = Spans.Count( value, c );
        this.AreEqual( expected, results );
    }


    [Test, TestCase( NUMERIC ), TestCase( ALPHANUMERIC )]
    public void AsBytes( string value )
    {
        ReadOnlySpan<byte> results = Spans.AsBytes( value );
        this.AreEqual( Encoding.Unicode.GetBytes( value ), results );
    }


    [Test, TestCase( NUMERIC ), TestCase( ALPHANUMERIC )]
    public void AsSegment( string value )
    {
        ReadOnlyMemory<byte> array = Encoding.UTF8.GetBytes( value );
        this.True( array.TryAsSegment( out ArraySegment<byte> segment ) );
        this.AreEqual( array.Span, segment.Array );
    }


    [Test, TestCase( NUMERIC ), TestCase( ALPHANUMERIC )]
    public void ConvertToString( string value )
    {
        ReadOnlyMemory<byte> array  = Encoding.Unicode.GetBytes( value );
        string               result = array.ConvertToString( Encoding.Unicode );
        Console.WriteLine( value );
        Console.WriteLine( result );
        this.AreEqual<char>( value, result );
    }


    [Test, TestCase( NUMERIC, '-', $"{NUMERIC}-----" ), TestCase( UPPER_CASE, '-', $"{UPPER_CASE}-----" )]
    public void TryCopyTo( string value, char c, string expected )
    {
        Span<char> span = stackalloc char[expected.Length];
        this.True( Spans.TryCopyTo( value, ref span, c ) );
        string result = span.ToString();
        this.AreEqual<char>( expected, result );
    }


    [Test, TestCase( 1 ), TestCase( 2 )]
    public void Create( int expected )
    {
        int results = Spans.CreateSpan<int>( expected ).Length;
        this.AreEqual( expected, results );
    }


    [Test, TestCase( 1 ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 )]
    public void Create( params int[] values )
    {
        int length = values.Length;

        switch ( length )
        {
            case 1:
            {
                int results = Spans.Create( 1 ).Length;
                this.AreEqual( length, results );
                return;
            }

            case 2:
            {
                int results = Spans.Create( 1, 2 ).Length;
                this.AreEqual( length, results );
                return;
            }

            case 3:
            {
                int results = Spans.Create( 1, 2, 3 ).Length;
                this.AreEqual( length, results );
                return;
            }

            case 4:
            {
                int results = Spans.Create( 1, 2, 3, 4 ).Length;
                this.AreEqual( length, results );
                return;
            }

            case 5:
            {
                int results = Spans.Create( 1, 2, 3, 4, 5 ).Length;
                this.AreEqual( length, results );
                return;
            }
        }
    }


    [Test, TestCase( 1d ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 )]
    public void Average( params double[] values )
    {
        this.AreEqual( values.Average(), Spans.Average( values ) );
        this.AreEqual( values.Average(), Spans.Average<double>( values ) );
    }


    [Test, TestCase( 1d ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 )]
    public void Max( params double[] values )
    {
        double results = Spans.Max( values, double.MinValue );
        this.AreEqual( values.Max(), results );
    }


    [Test, TestCase( 1d ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 )]
    public void Min( params double[] values )
    {
        double results = Spans.Min( values, double.MaxValue );
        this.AreEqual( values.Min(), results );
    }


    [Test, TestCase( 1d ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 )]
    public void Sum( params double[] values )
    {
        double results = Spans.Sum<double>( values );
        this.AreEqual( values.Sum(), results );
    }


    [Test, TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 )]
    public void First( params double[] values )
    {
        double results = Spans.First<double>( values, IsDevisableByTwo );
        this.AreEqual( values.First( IsDevisableByTwo ), results );
    }


    [Test, TestCase( 1d ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 )]
    public void FirstOrDefault( params double[] values )
    {
        double results = Spans.FirstOrDefault<double>( values, IsDevisableByTwo );
        this.AreEqual( values.FirstOrDefault( IsDevisableByTwo ), results );
    }


    [Test, TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 )]
    public void Single( params double[] values )
    {
        int count = values.Count( IsDevisableByTwo );

        if ( count > 1 )
        {
            Catch( () =>
                   {
                       double results = Spans.Single<double>( values, IsDevisableByTwo );
                       this.AreEqual( values.Single( IsDevisableByTwo ), results );
                   } );
        }
        else
        {
            double results = Spans.Single<double>( values, IsDevisableByTwo );
            this.AreEqual( values.SingleOrDefault( IsDevisableByTwo ), results );
        }
    }


    [Test, TestCase( 1d ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 )]
    public void SingleOrDefault( params double[] values )
    {
        int count = values.Count( IsDevisableByTwo );

        if ( count > 1 )
        {
            Catch( () =>
                   {
                       double results = Spans.SingleOrDefault<double>( values, IsDevisableByTwo );
                       this.AreEqual( values.SingleOrDefault( IsDevisableByTwo ), results );
                   } );
        }
        else
        {
            double results = Spans.SingleOrDefault<double>( values, IsDevisableByTwo );
            this.AreEqual( values.SingleOrDefault( IsDevisableByTwo ), results );
        }
    }


    [Test, TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 )]
    public void Where( params double[] values )
    {
        Span<double> results = Spans.Where<double>( values, IsDevisableByTwo );
        this.AreEqual<double>( values.Where( IsDevisableByTwo ).ToArray(), results );
    }


    [Test, TestCase( 1d ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 )]
    public void WhereValues( params double[] values )
    {
        Span<double> results = Spans.WhereValues<double>( values, IsDevisableByTwo );
        this.AreEqual<double>( values.Where( IsDevisableByTwo ).ToArray(), results );
    }


    [Test, TestCase( NUMERIC, '9', true )]
    public void EndsWith( string value, char c, bool expected )
    {
        bool results = Spans.EndsWith( value, c );
        this.AreEqual( expected, results );
    }


    [Test, TestCase( NUMERIC, "0", true )]
    public void StartsWith( string value, string c, bool expected )
    {
        bool results = Spans.StartsWith<char>( value, c );
        this.AreEqual( expected, results );
    }


    [Test, TestCase( "EMPTY", true ), TestCase( "VALUES", true ), TestCase( ALPHANUMERIC, true )]
    public void AsSpan( string value, bool expected )
    {
        ReadOnlySpan<char> span    = value;
        ReadOnlySpan<char> result  = span.AsSpan();
        bool               results = result.SequenceEqual( span );
        this.AreEqual( expected, results );
    }


    [Test, TestCase( "Abc_a154dbz123", "XYZ", "Abc_a154dbz123XYZ" )]
    public void Join( string value, string other, string expected )
    {
        ReadOnlySpan<char> span   = Spans.Join<char>( value, other );
        string             result = span.ToString();
        this.AreEqual( expected, result );
    }


    [Test, TestCase( "Abc_a1524dbz123", "1", "Abc_a524dbz23" ), TestCase( "Abc_a1524dbz123", "2", "Abc_a154dbz13" )]
    public void RemoveAll( string value, string other, string expected )
    {
        Span<char> span = stackalloc char[value.Length];
        value.CopyTo( span );

        Span<char> result = span.RemoveAll( other );
        this.AreEqual( expected, result.ToString() );
    }


    [Test, TestCase( "Abc_a1524dbz123", "1", "z", "Abc_az524dbzz23" ), TestCase( "Abc_a1524dbz123", "2", "4", "Abc_a1544dbz143" )]
    public void Replace( string value, string oldValue, string newValue, string expected )
    {
        ReadOnlySpan<char> result = Spans.Replace<char>( value, oldValue, newValue );
        this.AreEqual( expected, result.ToString() );
    }


    [Test, TestCase( "Abc_a1524dbz123", 'a', 'z', false, "1524db" ), TestCase( "Abc_a1524dbz123", 'a', 'z', true, "a1524dbz" )]
    public void Slice( string value, char start, char end, bool includeEnds, string expected )
    {
        ReadOnlySpan<char> result = Spans.Slice( value, start, end, includeEnds );
        this.AreEqual( expected, result.ToString() );
    }
}
