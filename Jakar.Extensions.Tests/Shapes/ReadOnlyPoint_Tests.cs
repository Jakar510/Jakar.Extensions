// Jakar.Extensions :: Jakar.Extensions.Tests
// 06/26/2025  16:59

using System.Drawing;



namespace Jakar.Extensions.Tests.Shapes;


[TestFixture, TestOf( typeof(ReadOnlyPoint) )]
public sealed class ReadOnlyPoint_Tests : Assert
{
    [Test]
    public void ZeroPoint_ShouldBeEmptyAndValid()
    {
        ReadOnlyPoint point = ReadOnlyPoint.Zero;
        this.AreEqual( 0, point.X );
        this.AreEqual( 0, point.Y );
        this.IsTrue( point.IsEmpty );
        this.IsFalse( point.IsNaN );
        this.IsTrue( point.IsValid );
    }

    [Test]
    public void InvalidPoint_ShouldBeNaNAndInvalid()
    {
        ReadOnlyPoint point = ReadOnlyPoint.Invalid;
        this.IsTrue( double.IsNaN( point.X ) );
        this.IsTrue( double.IsNaN( point.Y ) );
        this.IsTrue( point.IsNaN );
        this.IsFalse( point.IsValid );
    }

    [Test]
    public void ImplicitConversionToPoint_ShouldRoundValues()
    {
        ReadOnlyPoint point     = new(1.6, 2.4);
        Point         converted = point;
        this.AreEqual( 2, converted.X );
        this.AreEqual( 2, converted.Y );
    }

    [Test]
    public void IsEmpty_WhenXOrYIsNotZero_ShouldBeFalse()
    {
        ReadOnlyPoint point = new(1, 0);
        this.IsFalse( point.IsEmpty );

        point = new ReadOnlyPoint( 0, 1 );
        this.IsFalse( point.IsEmpty );
    }

    //--------------------------------------------------------------------------------------

    [Test]
    public void Operator_Equality_ShouldReturnTrueForSameCoordinates()
    {
        ReadOnlyPoint a = new(1.5, 2.5);
        ReadOnlyPoint b = new(1.5, 2.5);
        this.IsTrue( a  == b );
        this.IsFalse( a != b );
        this.IsTrue( a.Equals( b ) );
    }

    [Test]
    public void Operator_Equality_ShouldReturnFalseForDifferentCoordinates()
    {
        ReadOnlyPoint a = new(1.5, 2.5);
        ReadOnlyPoint b = new(2.5, 3.5);
        this.IsFalse( a == b );
        this.IsTrue( a  != b );
        this.IsFalse( a.Equals( b ) );
    }

    [Test]
    public void Equals_ShouldReturnFalseWhenComparedWithNull()
    {
        ReadOnlyPoint a = new(1.5, 2.5);
        object?       b = null;
        this.IsFalse( a.Equals( b ) );
    }

    [Test]
    public void Equals_ShouldReturnFalseWhenComparedWithDifferentType()
    {
        ReadOnlyPoint a = new(1.5, 2.5);
        string        b = "not a point";

        // ReSharper disable once SuspiciousTypeConversion.Global
        this.IsFalse( a.Equals( b ) );
    }

    [Test]
    public void GetHashCode_ShouldBeSameForEqualValues()
    {
        ReadOnlyPoint a = new(1.5, 2.5);
        ReadOnlyPoint b = new(1.5, 2.5);
        this.AreEqual( a.GetHashCode(), b.GetHashCode() );
    }

    //--------------------------------------------------------------------------------------

    [Test]
    public void Round_ShouldRoundValues()
    {
        ReadOnlyPoint point     = new(1.6, 2.4);
        Point         converted = point.Round();
        this.AreEqual( 2, converted.X );
        this.AreEqual( 2, converted.Y );
    }

    [Test]
    public void Floor_ShouldDecreaseValues()
    {
        ReadOnlyPoint point     = new(1.6, 2.4);
        Point         converted = point.Floor();
        this.AreEqual( 1, converted.X );
        this.AreEqual( 2, converted.Y );
    }

    //--------------------------------------------------------------------------------------

    [Test]
    public void Operator_AddTuple_ShouldAddOffsets()
    {
        ReadOnlyPoint point  = new(1.5, 2.5);
        ReadOnlyPoint result = point + (1.0, 2.0);
        this.AreEqual( 2.5, result.X );
        this.AreEqual( 4.5, result.Y );
    }

    [Test]
    public void Operator_SubtractTuple_ShouldSubtractOffsets()
    {
        ReadOnlyPoint point  = new(5.5, 6.5);
        ReadOnlyPoint result = point - (1.0, 2.0);
        this.AreEqual( 4.5, result.X );
        this.AreEqual( 4.5, result.Y );
    }

    [Test]
    public void Operator_MultiplyTuple_ShouldMultiplyOffsets()
    {
        ReadOnlyPoint point  = new(2, 3);
        ReadOnlyPoint result = point * (2.0, 3.0);
        this.AreEqual( 4, result.X );
        this.AreEqual( 9, result.Y );
    }

    [Test]
    public void Operator_DivideTuple_ShouldDivideOffsets()
    {
        ReadOnlyPoint point  = new(4, 6);
        ReadOnlyPoint result = point / (2.0, 3.0);
        this.AreEqual( 2, result.X );
        this.AreEqual( 2, result.Y );
    }

    [Test]
    public void Operator_AddPointF_ShouldAddCoordinates()
    {
        ReadOnlyPoint a      = new(1.5, 2.5);
        ReadOnlyPoint b      = new(2.0, 3.0);
        ReadOnlyPoint result = a + b;
        this.AreEqual( 3.5, result.X );
        this.AreEqual( 5.5, result.Y );
    }

    [Test]
    public void Operator_SubtractPointF_ShouldSubtractCoordinates()
    {
        ReadOnlyPoint a      = new(5.5, 6.5);
        ReadOnlyPoint b      = new(2.0, 3.0);
        ReadOnlyPoint result = a - b;
        this.AreEqual( 3.5, result.X );
        this.AreEqual( 3.5, result.Y );
    }

    [Test]
    public void Operator_MultiplyPointF_ShouldMultiplyCoordinates()
    {
        ReadOnlyPoint a      = new(2, 3);
        ReadOnlyPoint b      = new(4, 5);
        ReadOnlyPoint result = a * b;
        this.AreEqual( 8,  result.X );
        this.AreEqual( 15, result.Y );
    }

    [Test]
    public void Operator_DividePointF_ShouldDivideCoordinates()
    {
        ReadOnlyPoint a      = new(8, 9);
        ReadOnlyPoint b      = new(2, 3);
        ReadOnlyPoint result = a / b;
        this.AreEqual( 4, result.X );
        this.AreEqual( 3, result.Y );
    }

    [Test]
    public void Operator_AddDrawingPointF_ShouldAddCoordinates()
    {
        ReadOnlyPoint a      = new(1.5, 2.5);
        PointF        b      = new(2, 3);
        ReadOnlyPoint result = a + b;
        this.AreEqual( 3.5, result.X );
        this.AreEqual( 5.5, result.Y );
    }

    [Test]
    public void Operator_SubtractDrawingPointF_ShouldSubtractCoordinates()
    {
        ReadOnlyPoint a      = new(5.5, 6.5);
        PointF        b      = new(2, 3);
        ReadOnlyPoint result = a - b;
        this.AreEqual( 3.5, result.X );
        this.AreEqual( 3.5, result.Y );
    }

    [Test]
    public void Operator_MultiplyDrawingPointF_ShouldMultiplyCoordinates()
    {
        ReadOnlyPoint a      = new(2, 3);
        PointF        b      = new(4, 5);
        ReadOnlyPoint result = a * b;
        this.AreEqual( 8,  result.X );
        this.AreEqual( 15, result.Y );
    }

    [Test]
    public void Operator_DivideDrawingPointF_ShouldDivideCoordinates()
    {
        ReadOnlyPoint a      = new(8, 9);
        PointF        b      = new(2, 3);
        ReadOnlyPoint result = a / b;
        this.AreEqual( 4, result.X );
        this.AreEqual( 3, result.Y );
    }

    [Test]
    public void Operator_AddDrawingPoint_ShouldAddCoordinates()
    {
        ReadOnlyPoint a      = new(1.5, 2.5);
        Point         b      = new(2, 3);
        ReadOnlyPoint result = a + b;
        this.AreEqual( 3.5, result.X );
        this.AreEqual( 5.5, result.Y );
    }

    [Test]
    public void Operator_SubtractDrawingPoint_ShouldSubtractCoordinates()
    {
        ReadOnlyPoint a      = new(5.5, 6.5);
        Point         b      = new(2, 3);
        ReadOnlyPoint result = a - b;
        this.AreEqual( 3.5, result.X );
        this.AreEqual( 3.5, result.Y );
    }

    [Test]
    public void Operator_MultiplyDrawingPoint_ShouldMultiplyCoordinates()
    {
        ReadOnlyPoint a      = new(2, 3);
        Point         b      = new(4, 5);
        ReadOnlyPoint result = a * b;
        this.AreEqual( 8,  result.X );
        this.AreEqual( 15, result.Y );
    }

    [Test]
    public void Operator_DivideDrawingPoint_ShouldDivideCoordinates()
    {
        ReadOnlyPoint a      = new(8, 9);
        Point         b      = new(2, 3);
        ReadOnlyPoint result = a / b;
        this.AreEqual( 4, result.X );
        this.AreEqual( 3, result.Y );
    }
}
