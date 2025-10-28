// Jakar.Extensions :: Jakar.Shapes.Tests
// 07/12/2025  19:09

using System.Runtime.CompilerServices;



namespace Jakar.Shapes.Tests;


public static class AssertExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void AreEqual<TValue>( this Assert _, scoped in ReadOnlySpan<TValue> expected, scoped in ReadOnlySpan<TValue> actual ) => Assert.That(actual.SequenceEqual(expected), Is.True);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void AreEqual<TValue>( this Assert _, TValue                         expected, TValue                         actual ) => Assert.That(actual,                         Is.EqualTo(expected));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void NotEqual<TValue>( this Assert _, TValue                         expected, TValue                         actual ) => Assert.That(actual,                         Is.Not.EqualTo(expected));
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void NotNull<TValue>( this  Assert _, TValue                         actual ) => Assert.That(actual, Is.Not.Null);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void IsNull<TValue>( this   Assert _, TValue                         actual ) => Assert.That(actual, Is.Null);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void IsFalse( this          Assert _, bool                           actual ) => Assert.That(actual, Is.False);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void IsTrue( this           Assert _, bool                           actual ) => Assert.That(actual, Is.True);


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void LessThan<TValue>( this Assert _, TValue expected, TValue actual )
        where TValue : IComparable<TValue> => Assert.That(actual, Is.LessThan(expected));


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void GreaterThan<TValue>( this Assert _, TValue expected, TValue actual )
        where TValue : IComparable<TValue> => Assert.That(actual, Is.GreaterThan(expected));
}
