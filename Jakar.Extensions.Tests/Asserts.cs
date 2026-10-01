using System.Runtime.CompilerServices;



namespace Jakar.Extensions.Tests;


public static class Asserts
{
    extension<T>( T self )
        where T : Assert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void IsNull<TValue>( TValue  value ) { Assert.That(value, Is.Null); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void NotNull<TValue>( TValue value ) { Assert.That(value, Is.Not.Null); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void IsFalse( bool           value ) { Assert.That(value, Is.False); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void IsTrue( bool            value ) { Assert.That(value, Is.True); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void AreEqual<TValue>( TValue? expected, TValue? value ) { Assert.That(expected, Is.EqualTo(value)); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void AreEqual<TValue>( ReadOnlySpan<TValue> expected, ReadOnlySpan<TValue> value ) { Assert.That(true, Is.EqualTo(expected.SequenceEqual(value))); }
    }
}
