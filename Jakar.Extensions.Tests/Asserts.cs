namespace Jakar.Extensions.Tests;


public static class Asserts
{
    extension<T>( T self )
        where T : Assert
    {
        public void IsNull<TValue>( TValue  value ) { Assert.That(value, Is.Null); }
        public void NotNull<TValue>( TValue value ) { Assert.That(value, Is.Not.Null); }
        public void IsFalse( bool           value ) { Assert.That(value, Is.False); }
        public void IsTrue( bool            value ) { Assert.That(value, Is.True); }


        public void AreEqual<TValue>( TValue? expected, TValue? value ) { Assert.That(expected, Is.EqualTo(value)); }


        public void AreEqual<TValue>( ReadOnlySpan<TValue> expected, ReadOnlySpan<TValue> value ) { Assert.That(true, Is.EqualTo(expected.SequenceEqual(value))); }
    }
}
