// Jakar.Extensions :: Jakar.Extensions.Tests
// 12/6/2023  13:2

using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Text;
using static Jakar.Extensions.Randoms;



namespace Jakar.Extensions.Tests.SpanAndMemory;


[ TestFixture, TestOf( typeof(Spans) ) ]
public class SpansTests : Assert
{
    private static bool IsDevisableByTwo<T>( T x )
        where T : INumber<T> => x % (T.One + T.One) == T.Zero;


    [ Test, TestCase( new[]
                      {
                          NUMERIC,
                          ALPHANUMERIC,
                          SPECIAL_CHARS
                      },
                      new[]
                      {
                          NUMERIC,
                          ALPHANUMERIC,
                          SPECIAL_CHARS
                      },
                      true ), TestCase( new[]
                                        {
                                            NUMERIC,
                                            ALPHANUMERIC,
                                            SPECIAL_CHARS
                                        },
                                        new[]
                                        {
                                            ALPHANUMERIC,
                                            NUMERIC,
                                            SPECIAL_CHARS
                                        },
                                        false ) ]
    public void SequenceEqual( string[] value, string[] other, bool expected )
    {
        ImmutableArray<string> array   = ImmutableArray.Create( value );
        bool                   results = array.SequenceEqual( other );
        this.AreEqual( results, expected );

        results = array.AsSpan().SequenceEqual( other );
        this.AreEqual( results, expected );
    }


    [ Test, TestCase( ALPHANUMERIC, "abc", true ) ]
    public void Contains( string value, string other, bool expected )
    {
        bool results = Spans.Contains( value, other );
        this.AreEqual( results, expected );
    }


    [ Test, TestCase( ALPHANUMERIC, "65bc", true ) ]
    public void ContainsAny( string value, string other, bool expected )
    {
        bool results = Spans.ContainsAny( value, other );
        this.AreEqual( results, expected );
    }


    [ Test, TestCase( ALPHANUMERIC, "65bc", true ) ]
    public void ContainsAll( string value, string other, bool expected )
    {
        bool results = Spans.ContainsAll<char>( value, other );
        this.AreEqual( results, expected );
    }


    [ Test, TestCase( ALPHANUMERIC, "@&*^", true ) ]
    public void ContainsNone( string value, string other, bool expected )
    {
        bool results = Spans.ContainsNone( value, other );
        this.AreEqual( results, expected );
    }


    [ Test, TestCase( "", true ), TestCase( "  ", true ), TestCase( "\t", true ), TestCase( " \t ", true ), TestCase( "FULL", false ), TestCase( "   VALUES", false ) ]
    public void IsNullOrWhiteSpace( string value, bool expected )
    {
        bool results = Spans.IsNullOrWhiteSpace( value );
        this.AreEqual( results, expected );
    }


    [ Test, TestCase( NUMERIC, 0 ), TestCase( NUMERIC, 2 ) ]
    public void Enumerate( string value, int start )
    {
        foreach ( (int index, char c) in Spans.Enumerate<char>( value, start ) )
        {
            this.AreEqual( value[index], c );
            this.AreEqual( index,        start++ );
        }
    }


    [ Test, TestCase( NUMERIC, '9', 10 ), TestCase( NUMERIC, '0', 0 ), TestCase( NUMERIC, '5', 6 ) ]
    public void LastIndexOf( string value, char c, int expected )
    {
        int results = Spans.LastIndexOf( value, c, value.Length );
        this.AreEqual( results, expected );
    }


    [ Test, TestCase( NUMERIC, '9', 1 ), TestCase( NUMERIC + NUMERIC, '9', 2 ) ]
    public void Count( string value, char c, int expected )
    {
        int results = Spans.Count( value, c );
        this.AreEqual( results, expected );
    }


    [ Test, TestCase( NUMERIC ), TestCase( ALPHANUMERIC ) ]
    public void AsBytes( string value )
    {
        ReadOnlySpan<byte> results = Spans.AsBytes( value );
        this.Equals( results, Encoding.UTF8.GetBytes( value ) );
    }


    [ Test, TestCase( NUMERIC ), TestCase( ALPHANUMERIC ) ]
    public void AsMemory( string value )
    {
        ReadOnlySpan<byte> array = Encoding.UTF8.GetBytes( value );
        this.Equals( array.AsMemory().Span,        array );
        this.Equals( Spans.AsMemory( value ).Span, value.AsSpan() );
    }


    [ Test, TestCase( NUMERIC ), TestCase( ALPHANUMERIC ) ]
    public void AsSegment( string value )
    {
        ReadOnlyMemory<byte> array = Encoding.UTF8.GetBytes( value );
        this.True( array.TryAsSegment( out ArraySegment<byte> segment ) );
        this.Equals( segment.Array, array.Span );
    }


    [ Test, TestCase( NUMERIC ), TestCase( ALPHANUMERIC ) ]
    public void ConvertToString( string value )
    {
        ReadOnlyMemory<char> array = value.AsSpan().AsMemory();
        this.Equals<char>( array.ConvertToString(), value );
    }


    [ Test, TestCase( NUMERIC, '-', $"{NUMERIC}-----" ), TestCase( ALPHANUMERIC, '-', $"{ALPHANUMERIC}-----" ) ]
    public void TryCopyTo( string value, char c, string expected )
    {
        Span<char> span = stackalloc char[expected.Length];
        this.True( Spans.TryCopyTo( value, ref span ) );
        this.Equals<char>( span.ToString(), expected );
    }


    [ Test, TestCase( 1 ), TestCase( 2 ) ]
    public void Create( int expected )
    {
        int results = Spans.Create( expected ).Length;
        this.AreEqual( results, expected );
    }


    [ Test, TestCase( 1 ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 ) ]
    public void Create( params int[] values )
    {
        int length = values.Length;

        switch ( length )
        {
            case 1:
            {
                int results = Spans.Create( 1 ).Length;
                this.AreEqual( results, length );
                return;
            }

            case 2:
            {
                int results = Spans.Create( 1, 2 ).Length;
                this.AreEqual( results, length );
                return;
            }

            case 3:
            {
                int results = Spans.Create( 1, 2, 3 ).Length;
                this.AreEqual( results, length );
                return;
            }

            case 4:
            {
                int results = Spans.Create( 1, 2, 3, 4 ).Length;
                this.AreEqual( results, length );
                return;
            }

            case 5:
            {
                int results = Spans.Create( 1, 2, 3, 4, 5 ).Length;
                this.AreEqual( results, length );
                return;
            }
        }
    }


    [ Test, TestCase( 1d ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 ) ]
    public void Average( params double[] values )
    {
        this.AreEqual( Spans.Average( values ),         values.Average() );
        this.AreEqual( Spans.Average<double>( values ), values.Average() );
    }


    [ Test, TestCase( 1d ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 ) ]
    public void Max( params double[] values )
    {
        double results = Spans.Max<double>( values );
        this.AreEqual( results, values.Max() );
    }


    [ Test, TestCase( 1d ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 ) ]
    public void Min( params double[] values )
    {
        double results = Spans.Min<double>( values );
        this.AreEqual( results, values.Min() );
    }


    [ Test, TestCase( 1d ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 ) ]
    public void Sum( params double[] values )
    {
        double results = Spans.Sum<double>( values );
        this.AreEqual( results, values.Sum() );
    }


    [ Test, TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 ) ]
    public void First( params double[] values )
    {
        double results = Spans.First<double>( values, IsDevisableByTwo );
        this.AreEqual( results, values.First( IsDevisableByTwo ) );
    }


    [ Test, TestCase( 1d ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 ) ]
    public void FirstOrDefault( params double[] values )
    {
        double results = Spans.FirstOrDefault<double>( values, IsDevisableByTwo );
        this.AreEqual( results, values.FirstOrDefault( IsDevisableByTwo ) );
    }


    [ Test, TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 ) ]
    public void Single( params double[] values )
    {
        double results = Spans.Single<double>( values, IsDevisableByTwo );
        this.AreEqual( results, values.Single( IsDevisableByTwo ) );
    }


    [ Test, TestCase( 1d ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 ) ]
    public void SingleOrDefault( params double[] values )
    {
        double results = Spans.SingleOrDefault<double>( values, IsDevisableByTwo );
        this.AreEqual( results, values.SingleOrDefault( IsDevisableByTwo ) );
    }


    [ Test, TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 ) ]
    public void Where( params double[] values )
    {
        var results = Spans.Where<double>( values, IsDevisableByTwo );
        this.Equals<double>( results, values.Where( IsDevisableByTwo ).ToArray() );
    }


    [ Test, TestCase( 1d ), TestCase( 1, 2 ), TestCase( 1, 2, 3 ), TestCase( 1, 2, 3, 4 ), TestCase( 1, 2, 3, 4, 5 ) ]
    public void WhereValues( params double[] values )
    {
        var results = Spans.WhereValues<double>( values, IsDevisableByTwo );
        this.Equals<double>( results, values.Where( IsDevisableByTwo ).ToArray() );
    }


    [ Test, TestCase( NUMERIC, '9', true ) ]
    public void EndsWith( string value, char c, bool expected )
    {
        bool results = Spans.EndsWith( value, c );
        this.AreEqual( results, expected );
    }


    [ Test, TestCase( NUMERIC, "0", true ) ]
    public void StartsWith( string value, string c, bool expected )
    {
        bool results = Spans.StartsWith<char>( value, c );
        this.AreEqual( results, expected );
    }


    [ Test, TestCase( "EMPTY", true ), TestCase( "VALUES", true ), TestCase( ALPHANUMERIC, true ) ]
    public void AsSpan( string value, bool expected )
    {
        ReadOnlySpan<char> span    = value;
        ReadOnlySpan<char> result  = span.AsSpan();
        bool               results = result.SequenceEqual( span );
        this.AreEqual( results, expected );
    }


    [ Test, TestCase( "Abc_a154dbz123", "XYZ", "Abc_a154dbz123XYZ" ) ]
    public void Join( string value, string other, string expected )
    {
        ReadOnlySpan<char> span = Spans.Join<char>( value, other );
        this.AreEqual( span.ToString(), expected );
    }


    [ Test, TestCase( "Abc_a154dbz123", "1", "Abc_z54dbz23" ) ]
    public void RemoveAll( string value, string other, string expected )
    {
        Span<char> span = stackalloc char[value.Length];
        value.CopyTo( span );

        Span<char> result = span.RemoveAll( other );
        this.AreEqual( result.ToString(), expected );
    }


    [ Test, TestCase( "Abc_a154dbz123", "a", "z", "Abc_z154dbz123" ), TestCase( "Abc_a154dbz123", "A", "4", "4bc_a154dbz123" ) ]
    public void Replace( string value, string oldValue, string newValue, string expected )
    {
        ReadOnlySpan<char> result = Spans.Replace<char>( value, oldValue, newValue );
        this.AreEqual( result.ToString(), expected );
    }


    [ Test, TestCase( "Abc_a154dbz123", 'a', 'z', false, "154db" ), TestCase( "Abc_a154dbz123", 'a', 'z', true, "a154dbz" ) ]
    public void Slice( string value, char start, char end, bool includeEnds, string expected )
    {
        ReadOnlySpan<char> result = Spans.Slice( value, start, end, includeEnds );
        this.AreEqual( result.ToString(), expected );
    }
}
