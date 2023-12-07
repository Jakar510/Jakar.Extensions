// Jakar.Extensions :: Jakar.Extensions.Tests
// 12/6/2023  13:2

using static Jakar.Extensions.Randoms;



namespace Jakar.Extensions.Tests.SpanAndMemory;


/*

   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Average
   Contains
   Contains
   Contains
   Contains
   Contains
   Contains
   ContainsAll
   ContainsAll
   ContainsAny
   ContainsAny
   ContainsAny
   ContainsAny
   ContainsNone
   ContainsNone
   ContainsNone
   ContainsNone
   EndsWith
   EndsWith
   EndsWith
   EndsWith
   StartsWith
   StartsWith
   StartsWith
   StartsWith
   Count
   Count
   Create
   Create
   Create
   Create
   Create
   AsBytes
   AsBytes
   IsNullOrWhiteSpace
   IsNullOrWhiteSpace
   IsNullOrWhiteSpace
   IsNullOrWhiteSpace
   IsNullOrWhiteSpace
   IsNullOrWhiteSpace
   SequenceEqual
   SequenceEqual
   LastIndexOf
   LastIndexOf
   Enumerate
   AsMemory
   AsSpan
   AsSpan
   AsSpan
   AsSpan
   AsReadOnlySpan
   AsReadOnlySpan
   CopyTo
   CopyTo
   TryCopyTo
   TryCopyTo
   Hash128
   Hash128
   Hash128
   Hash128
   Hash128
   Hash128
   Hash128
   Hash128
   Hash128
   Hash128
   Hash128
   Hash
   Hash
   Hash
   Hash
   Hash
   Hash
   Hash
   Hash
   Hash
   Hash
   Hash
   First
   First
   FirstOrDefault
   FirstOrDefault
   Single
   Single
   SingleOrDefault
   SingleOrDefault
   Where
   Where
   WhereValues
   WhereValues
   Where
   Join
   Join
   Join
   Max
   Max
   Max
   Max
   Max
   Max
   Max
   Max
   Max
   Max
   Max
   Max
   Max
   Max
   Max
   AsMemory
   AsMemory
   AsMemory
   AsMemory
   AsSegment
   AsSegment
   ToMemory
   ToReadOnlyMemory
   AsMemory
   AsMemory
   AsMemory
   AsMemory
   AsReadOnlySpan
   AsReadOnlySpan
   AsSpan
   AsSpan
   ConvertToString
   ConvertToString
   ConvertToString
   ConvertToString
   CopyTo
   CopyTo
   Min
   Min
   Min
   Min
   Min
   Min
   Min
   Min
   Min
   Min
   Min
   Min
   Min
   Min
   Min
   As
   As
   As
   As
   As
   As
   As
   As
   As
   As
   As
   As
   As
   As
   As
   As
   As
   As
   Replace
   Replace
   Replace
   Replace
   Replace
   Replace
   RemoveAll
   RemoveAll
   RemoveAll
   RemoveAll
   RemoveAll
   Slice
   Slice
   Sum
   Sum
   Sum
   Sum
   Sum
   Sum
   Sum
   Sum
   Sum
   Sum
   Sum
   Sum
   Sum
   Sum
   GetType
   ToString
   Equals
   GetHashCode

 */



[ TestFixture, TestOf( typeof(Spans) ) ]
public class SpansTests : Assert
{
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


    [ Test, TestCase( NUMERIC, '9', true ) ]
    public void EndsWith( string value, char c, bool expected )
    {
        bool results = Spans.EndsWith( value, c );
        this.AreEqual( results, expected );
    }


    [ Test, TestCase( NUMERIC, "9", true ) ]
    public void EndsWith( string value, string c, bool expected )
    {
        bool results = Spans.EndsWith<char>( value, c );
        this.AreEqual( results, expected );
    }


    [ Test, TestCase( NUMERIC, '0', true ) ]
    public void StartsWith( string value, char c, bool expected )
    {
        bool results = Spans.StartsWith( value, c );
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
