// Jakar.Extensions :: Jakar.Extensions.Tests

using System.Collections.Generic;



namespace Jakar.Extensions.Tests;


[TestFixture]
[TestOf(typeof(Pair))]
public class Pair_Tests : Assert
{
    // ─── Construction & properties ───────────────────────────────────────────

    [Test]
    public void Construct_WithKeyAndValue()
    {
        Pair p = new("key", "value");
        this.AreEqual("key",   p.Key);
        this.AreEqual("value", p.Value);
        this.IsTrue(p.HasValue);
    }

    [Test]
    public void Construct_WithNullValue()
    {
        Pair p = new("key", null);
        this.AreEqual("key", p.Key);
        this.IsNull(p.Value);
        this.IsFalse(p.HasValue);
    }

    [Test]
    public void Construct_WithEmptyValue()
    {
        Pair p = new("key", "");
        this.IsFalse(p.HasValue); // empty string is treated as no value
    }

    [Test]
    public void Construct_WithWhitespaceValue()
    {
        Pair p = new("key", "   ");
        this.IsFalse(p.HasValue); // whitespace-only string is treated as no value
    }


    // ─── Equality ─────────────────────────────────────────────────────────────

    [Test]
    public void Equals_SameKeyAndValue_ReturnsTrue()
    {
        Pair a = new("k", "v");
        Pair b = new("k", "v");
        this.IsTrue(a.Equals(b));
        this.IsTrue(a == b);
        this.IsFalse(a != b);
    }

    [Test]
    public void Equals_DifferentKey_ReturnsFalse()
    {
        Pair a = new("k1", "v");
        Pair b = new("k2", "v");
        this.IsFalse(a.Equals(b));
        this.IsFalse(a == b);
        this.IsTrue(a != b);
    }

    [Test]
    public void Equals_DifferentValue_ReturnsFalse()
    {
        Pair a = new("k", "v1");
        Pair b = new("k", "v2");
        this.IsFalse(a.Equals(b));
    }

    [Test]
    public void Equals_NullValues_BothNull_ReturnsTrue()
    {
        Pair a = new("k", null);
        Pair b = new("k", null);
        this.IsTrue(a.Equals(b));
    }

    [Test]
    public void Equals_IsCaseSensitive()
    {
        Pair a = new("Key", "Value");
        Pair b = new("key", "value");
        this.IsFalse(a.Equals(b));
    }

    [Test]
    public void GetHashCode_SamePairs_SameHash()
    {
        Pair a = new("k", "v");
        Pair b = new("k", "v");
        this.AreEqual(a.GetHashCode(), b.GetHashCode());
    }


    // ─── Comparison ───────────────────────────────────────────────────────────

    [Test]
    public void CompareTo_SamePair_ReturnsZero()
    {
        Pair a = new("k", "v");
        Pair b = new("k", "v");
        this.AreEqual(0, a.CompareTo(b));
    }

    [Test]
    public void CompareTo_OrdersByKey()
    {
        Pair a = new("a", "z");
        Pair b = new("b", "a");
        this.IsTrue(a.CompareTo(b) < 0);
        this.IsTrue(b.CompareTo(a) > 0);
    }

    [Test]
    public void CompareTo_SameKey_OrdersByValue()
    {
        Pair a = new("k", "a");
        Pair b = new("k", "b");
        this.IsTrue(a.CompareTo(b) < 0);
        this.IsTrue(b.CompareTo(a) > 0);
    }

    [Test]
    public void Operators_LessThanGreaterThan()
    {
        Pair a = new("a", "v");
        Pair b = new("b", "v");
        this.IsTrue(a  < b);
        this.IsFalse(a > b);
        this.IsTrue(a  <= b);
        this.IsTrue(b  >= a);
    }

    [Test]
    public void CompareTo_Object_NullReturnsOne()
    {
        Pair a = new("k", "v");
        this.AreEqual(1, a.CompareTo((object?)null));
    }

    [Test]
    public void CompareTo_Object_WrongTypeThrows()
    {
        Pair a = new("k", "v");
        Throws<ArgumentException>(() => _ = a.CompareTo("not-a-pair"));
    }


    // ─── Implicit conversions ─────────────────────────────────────────────────

    [Test]
    public void ImplicitConversion_ToKeyValuePair()
    {
        Pair                     source = new("k", "v");
        KeyValuePair<string, string?> kvp = source;
        this.AreEqual("k", kvp.Key);
        this.AreEqual("v", kvp.Value);
    }

    [Test]
    public void ImplicitConversion_FromKeyValuePair()
    {
        KeyValuePair<string, string?> kvp = new("k", "v");
        Pair                          p   = kvp;
        this.AreEqual("k", p.Key);
        this.AreEqual("v", p.Value);
    }

    [Test]
    public void ImplicitConversion_ToTuple()
    {
        Pair                  source = new("k", "v");
        (string Key, string? Value) tuple = source;
        this.AreEqual("k", tuple.Key);
        this.AreEqual("v", tuple.Value);
    }

    [Test]
    public void ImplicitConversion_FromTuple()
    {
        (string Key, string? Value) tuple = ("k", "v");
        Pair                        p     = tuple;
        this.AreEqual("k", p.Key);
        this.AreEqual("v", p.Value);
    }


    // ─── Deconstruct ──────────────────────────────────────────────────────────

    [Test]
    public void Deconstruct_ProducesKeyAndValue()
    {
        Pair p = new("myKey", "myValue");
        (string key, string? value) = p;
        this.AreEqual("myKey",   key);
        this.AreEqual("myValue", value);
    }

    [Test]
    public void Deconstruct_NullValue()
    {
        Pair p = new("k", null);
        (string key, string? value) = p;
        this.AreEqual("k", key);
        this.IsNull(value);
    }


    // ─── ToString ─────────────────────────────────────────────────────────────

    [Test]
    public void ToString_ContainsKeyAndValue()
    {
        Pair   p      = new("myKey", "myValue");
        string result = p.ToString();
        this.IsTrue(result.ContainsExact("myKey"));
        this.IsTrue(result.ContainsExact("myValue"));
    }


    // ─── Pair<TValue> ─────────────────────────────────────────────────────────

    [Test]
    public void PairT_Construct_WithIntValue()
    {
        Pair<int> p = new("score", 100);
        this.AreEqual("score", p.Key);
        this.AreEqual(100,     p.Value);
        // value types are always non-null, so HasValue is always true
        this.IsTrue(p.HasValue);
    }

    [Test]
    public void PairT_Equality()
    {
        Pair<int> a = new("k", 1);
        Pair<int> b = new("k", 1);
        this.IsTrue(a == b);
        this.IsTrue(a.Equals(b));
    }

    [Test]
    public void PairT_Inequality_DifferentValues()
    {
        Pair<int> a = new("k", 1);
        Pair<int> b = new("k", 2);
        this.IsTrue(a != b);
    }

    [Test]
    public void PairT_Comparison_OrdersByKey()
    {
        Pair<int> a = new("a", 99);
        Pair<int> b = new("b", 1);
        this.IsTrue(a < b);
        this.IsTrue(b > a);
    }

    [Test]
    public void PairT_ImplicitConversion_ToKeyValuePair()
    {
        Pair<double>                      p   = new("pi", 3.14);
        KeyValuePair<string, double> kvp = p;
        this.AreEqual("pi", kvp.Key);
        Assert.That(kvp.Value, Is.EqualTo(3.14).Within(1e-10));
    }

    [Test]
    public void PairT_Deconstruct()
    {
        Pair<int> p = new("count", 42);
        (string key, int value) = p;
        this.AreEqual("count", key);
        this.AreEqual(42, value);
    }
}
