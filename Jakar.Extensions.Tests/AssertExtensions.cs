// Jakar.Extensions :: Jakar.Extensions.Tests
// 11/28/2023  9:02 PM

using System.Runtime.CompilerServices;



namespace Jakar.Extensions.Tests;


public static class AssertExtensions
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AreEqual<T>( this  Assert _, T               expected, T               result ) => Assert.That( result,                           Is.EqualTo( expected ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AreEquals<T>( this Assert _, ReadOnlySpan<T> expected, ReadOnlySpan<T> result ) => Assert.That( result.SequenceEqual( expected ), Is.True );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void NotNull<T>( this   Assert _, T               result ) => Assert.That( result, Is.Not.Null );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void IsNull<T>( this    Assert _, T               result ) => Assert.That( result, Is.Null );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void False( this        Assert _, bool            result ) => Assert.That( result, Is.False );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void True( this         Assert _, bool            result ) => Assert.That( result, Is.True );
}
