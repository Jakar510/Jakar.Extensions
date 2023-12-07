// Jakar.Extensions :: Jakar.Extensions.Tests
// 11/28/2023  9:02 PM

using JetBrains.Annotations;



namespace Jakar.Extensions.Tests;


public static class AssertExtensions
{
    public static void AreEqual<T>( this Assert _, T    expected, T result ) => Assert.That( expected, Is.EqualTo( result ) );
    public static void NotNull<T>( this  Assert _, T    result ) => Assert.That( result, Is.Not.Null );
    public static void IsNull<T>( this   Assert _, T    result ) => Assert.That( result, Is.Null );
    public static void False( this       Assert _, bool result ) => Assert.That( result, Is.False );
    public static void True( this        Assert _, bool result ) => Assert.That( result, Is.True );
}
