// Jakar.Extensions :: Jakar.Extensions.Tests
// 11/28/2023  9:02 PM

using System.Runtime.CompilerServices;



namespace Jakar.Extensions.Tests;


public static class AssertExtensions
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AreEqual<T>( this Assert _, scoped in ReadOnlySpan<T> expected, scoped in ReadOnlySpan<T> actual ) => Assert.That( actual.SequenceEqual( expected ), Is.True );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AreEqual<T>( this Assert _, T[]                       expected, T[]                       actual ) => _.AreEqual( new ReadOnlySpan<T>( expected ), actual );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AreEqual<T>( this Assert _, T                         expected, T                         actual ) => Assert.That( actual, Is.EqualTo( expected ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void NotNull<T>( this  Assert _, T                         actual ) => Assert.That( actual, Is.Not.Null );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void IsNull<T>( this   Assert _, T                         actual ) => Assert.That( actual, Is.Null );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void False( this       Assert _, bool                      actual ) => Assert.That( actual, Is.False );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void True( this        Assert _, bool                      actual ) => Assert.That( actual, Is.True );
}
