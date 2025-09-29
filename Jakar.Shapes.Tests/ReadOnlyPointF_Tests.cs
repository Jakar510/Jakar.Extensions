// Jakar.Extensions :: Jakar.Extensions.Tests
// 06/26/2025  17:26

namespace Jakar.Shapes.Tests;


[TestFixture, TestOf(typeof(ReadOnlyPointF))]
public sealed class ReadOnlyPointF_Tests : Assert
{
    [Test]
    public void Zero_ShouldBeEmptyAndValid()
    {
        ReadOnlyPointF point = ReadOnlyPointF.Zero;
        this.AreEqual(0, point.X);
        this.AreEqual(0, point.Y);
        this.IsTrue(point.IsEmpty);
        this.IsFalse(point.IsNaN);
        this.IsTrue(point.IsValid);
    }

    [Test]
    public void Invalid_ShouldBeNaNAndInvalid()
    {
        ReadOnlyPointF point = ReadOnlyPointF.Invalid;
        this.IsTrue(double.IsNaN(point.X));
        this.IsTrue(double.IsNaN(point.Y));
        this.IsTrue(point.IsNaN);
        this.IsFalse(point.IsValid);
    }

    [Test]
    public void ImplicitConversion_ToPoint_ShouldRoundValues()
    {
        ReadOnlyPointF point     = new(1.6f, 2.4f);
        Point          converted = point;
        this.AreEqual(2, converted.X);
        this.AreEqual(2, converted.Y);
    }

    [Test]
    public void IsEmpty_WhenXOrYIsNotZero_ShouldBeFalse()
    {
        ReadOnlyPointF point = new(1, 0);
        this.IsFalse(point.IsEmpty);

        point = new ReadOnlyPointF(0, 1);
        this.IsFalse(point.IsEmpty);
    }

    //--------------------------------------------------------------------------------------

    [Test]
    public void Operator_Equality_ShouldReturnTrueForSameCoordinates()
    {
        ReadOnlyPointF a = new(1.5f, 2.5f);
        ReadOnlyPointF b = new(1.5f, 2.5f);
        this.IsTrue(a  == b);
        this.IsFalse(a != b);
        this.IsTrue(a.Equals(b));
    }

    [Test]
    public void Operator_Equality_ShouldReturnFalseForDifferentCoordinates()
    {
        ReadOnlyPointF a = new(1.5f, 2.5f);
        ReadOnlyPointF b = new(2.5f, 3.5f);
        this.IsFalse(a == b);
        this.IsTrue(a  != b);
        this.IsFalse(a.Equals(b));
    }

    [Test]
    public void Equals_ShouldReturnFalseWhenComparedWithNull()
    {
        ReadOnlyPointF a = new(1.5f, 2.5f);
        object?        b = null;
        this.IsFalse(a.Equals(b));
    }

    [Test]
    public void Equals_ShouldReturnFalseWhenComparedWithDifferentType()
    {
        ReadOnlyPointF a = new(1.5f, 2.5f);
        string         b = "not a point";
        this.IsFalse(a.Equals(b));
    }

    [Test]
    public void GetHashCode_ShouldBeSameForEqualValues()
    {
        ReadOnlyPointF a = new(1.5f, 2.5f);
        ReadOnlyPointF b = new(1.5f, 2.5f);
        this.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    //--------------------------------------------------------------------------------------

    [Test]
    public void Round_ShouldRoundValues()
    {
        ReadOnlyPointF point     = new(1.6f, 2.4f);
        Point          converted = point.Round();
        this.AreEqual(2, converted.X);
        this.AreEqual(2, converted.Y);
    }

    [Test]
    public void Floor_ShouldDecreaseValues()
    {
        ReadOnlyPointF point     = new(1.6f, 2.4f);
        Point          converted = point.Floor();
        this.AreEqual(1, converted.X);
        this.AreEqual(2, converted.Y);
    }

    //--------------------------------------------------------------------------------------
}
