// Jakar.Extensions :: Jakar.Extensions.Tests
// 4/1/2024  10:45

namespace Jakar.Extensions.Tests;


[TestFixture, TestOf( typeof(StringExtensions) )]
public class StringExtensions_Tests : Assert
{
    [Test, TestCase( "Hello World", "hello_world" ), TestCase( "Hello55 World33", "hello55_world33" ), TestCase( "Hello55 World", "hello55_world" ), TestCase( "fooBar", "foo_bar" )]
    public void Run( string value, string expected )
    {
        string result = value.ToSnakeCase();
        this.AreEqual( expected, result );
    }
}
