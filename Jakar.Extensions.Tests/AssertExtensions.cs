// Jakar.Extensions :: Jakar.Extensions.Tests
// 11/28/2023  9:02 PM

using System.Runtime.CompilerServices;



namespace Jakar.Extensions.Tests;


public static class AssertExtensions
{
    extension( Assert _ )
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void AreEqual<TValue>( scoped in ReadOnlySpan<TValue> expected, scoped in ReadOnlySpan<TValue> actual ) => Assert.That(actual.SequenceEqual(expected), Is.True);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void AreEqual<TValue>( TValue                         expected, TValue                         actual ) => Assert.That(actual,                         Is.EqualTo(expected));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void NotEqual<TValue>( TValue                         expected, TValue                         actual ) => Assert.That(actual,                         Is.Not.EqualTo(expected));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void NotNull<TValue>( TValue                          actual ) => Assert.That(actual, Is.Not.Null);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void IsNull<TValue>( TValue                           actual ) => Assert.That(actual, Is.Null);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void IsFalse( bool                                    actual ) => Assert.That(actual, Is.False);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void IsTrue( bool                                     actual ) => Assert.That(actual, Is.True);
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void LessThan<TValue>( TValue expected, TValue actual )
            where TValue : IComparable<TValue> => Assert.That(actual, Is.LessThan(expected));
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void GreaterThan<TValue>( TValue expected, TValue actual )
            where TValue : IComparable<TValue> => Assert.That(actual, Is.GreaterThan(expected));
    }
}
