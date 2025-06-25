// Jakar.Extensions :: Jakar.Extensions.Tests
// 11/28/2023  9:02 PM

using System.Runtime.CompilerServices;



namespace Jakar.Extensions.Tests;


public static class AssertExtensions
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AreEqual<TValue>( this Assert _, scoped in ReadOnlySpan<TValue> expected, scoped in ReadOnlySpan<TValue> actual ) => Assert.That( actual.SequenceEqual( expected ), Is.True );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void AreEqual<TValue>( this Assert _, TValue                         expected, TValue                         actual ) => Assert.That( actual,                           Is.EqualTo( expected ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void NotEqual<TValue>( this Assert _, TValue                         expected, TValue                         actual ) => Assert.That( actual,                           Is.Not.EqualTo( expected ) );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void NotNull<TValue>( this  Assert _, TValue                         actual ) => Assert.That( actual, Is.Not.Null );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void IsNull<TValue>( this   Assert _, TValue                         actual ) => Assert.That( actual, Is.Null );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void False( this            Assert _, bool                           actual ) => Assert.That( actual, Is.False );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static void True( this             Assert _, bool                           actual ) => Assert.That( actual, Is.True );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void LessThan<TValue>( this Assert _, TValue expected, TValue actual )
        where TValue : IComparable<TValue> => Assert.That( actual, Is.LessThan( expected ) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static void GreaterThan<TValue>( this Assert _, TValue expected, TValue actual )
        where TValue : IComparable<TValue> => Assert.That( actual, Is.GreaterThan( expected ) );
}
