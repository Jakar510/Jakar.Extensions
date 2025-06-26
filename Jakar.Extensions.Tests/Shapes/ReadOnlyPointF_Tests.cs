// Jakar.Extensions :: Jakar.Extensions.Tests
// 06/26/2025  17:26

using System.Drawing;



namespace Jakar.Extensions.Tests.Shapes;


[TestFixture, TestOf( typeof(ReadOnlyPointF) )]
public sealed class ReadOnlyPointF_Tests : Assert
{
    [Test]
    public void Zero_ShouldBeEmptyAndValid()
    {
        ReadOnlyPointF point = ReadOnlyPointF.Zero;
        this.AreEqual( 0, point.X );
        this.AreEqual( 0, point.Y );
        this.IsTrue( point.IsEmpty );
        this.IsFalse( point.IsNaN );
        this.IsTrue( point.IsValid );
    }

    [Test]
    public void Invalid_ShouldBeNaNAndInvalid()
    {
        ReadOnlyPointF point = ReadOnlyPointF.Invalid;
        this.IsTrue( double.IsNaN( point.X ) );
        this.IsTrue( double.IsNaN( point.Y ) );
        this.IsTrue( point.IsNaN );
        this.IsFalse( point.IsValid );
    }

    [Test]
    public void ImplicitConversion_ToPoint_ShouldRoundValues()
    {
        ReadOnlyPointF point     = new(1.6f, 2.4f);
        Point          converted = point;
        this.AreEqual( 2, converted.X );
        this.AreEqual( 2, converted.Y );
    }

    [Test]
    public void IsEmpty_WhenXOrYIsNotZero_ShouldBeFalse()
    {
        ReadOnlyPointF point = new(1, 0);
        this.IsFalse( point.IsEmpty );

        point = new ReadOnlyPointF( 0, 1 );
        this.IsFalse( point.IsEmpty );
    }

    //--------------------------------------------------------------------------------------

    [Test]
    public void Operator_Equality_ShouldReturnTrueForSameCoordinates()
    {
        ReadOnlyPointF a = new(1.5f, 2.5f);
        ReadOnlyPointF b = new(1.5f, 2.5f);
        this.IsTrue( a  == b );
        this.IsFalse( a != b );
        this.IsTrue( a.Equals( b ) );
    }

    [Test]
    public void Operator_Equality_ShouldReturnFalseForDifferentCoordinates()
    {
        ReadOnlyPointF a = new(1.5f, 2.5f);
        ReadOnlyPointF b = new(2.5f, 3.5f);
        this.IsFalse( a == b );
        this.IsTrue( a  != b );
        this.IsFalse( a.Equals( b ) );
    }

    [Test]
    public void Equals_ShouldReturnFalseWhenComparedWithNull()
    {
        ReadOnlyPointF a = new(1.5f, 2.5f);
        object?        b = null;
        this.IsFalse( a.Equals( b ) );
    }

    [Test]
    public void Equals_ShouldReturnFalseWhenComparedWithDifferentType()
    {
        ReadOnlyPointF a = new(1.5f, 2.5f);
        string         b = "not a point";
        this.IsFalse( a.Equals( b ) );
    }

    [Test]
    public void GetHashCode_ShouldBeSameForEqualValues()
    {
        ReadOnlyPointF a = new(1.5f, 2.5f);
        ReadOnlyPointF b = new(1.5f, 2.5f);
        this.AreEqual( a.GetHashCode(), b.GetHashCode() );
    }

    //--------------------------------------------------------------------------------------

    [Test]
    public void Round_ShouldRoundValues()
    {
        ReadOnlyPointF point     = new(1.6f, 2.4f);
        Point          converted = point.Round();
        this.AreEqual( 2, converted.X );
        this.AreEqual( 2, converted.Y );
    }

    [Test]
    public void Floor_ShouldDecreaseValues()
    {
        ReadOnlyPointF point     = new(1.6f, 2.4f);
        Point          converted = point.Floor();
        this.AreEqual( 1, converted.X );
        this.AreEqual( 2, converted.Y );
    }

    //--------------------------------------------------------------------------------------

    [Test]
    public void Operator_AddTuple_ShouldAddOffsets()
    {
        ReadOnlyPointF point  = new(1.5f, 2.5f);
        ReadOnlyPointF result = point + (1.0, 2.0);
        this.AreEqual( 2.5f, result.X );
        this.AreEqual( 4.5f, result.Y );
    }

    [Test]
    public void Operator_SubtractTuple_ShouldSubtractOffsets()
    {
        ReadOnlyPointF point  = new(5.5f, 6.5f);
        ReadOnlyPointF result = point - (1.0, 2.0);
        this.AreEqual( 4.5f, result.X );
        this.AreEqual( 4.5f, result.Y );
    }

    [Test]
    public void Operator_MultiplyTuple_ShouldMultiplyOffsets()
    {
        ReadOnlyPointF point  = new(2f, 3f);
        ReadOnlyPointF result = point * (2.0, 3.0);
        this.AreEqual( 4f, result.X );
        this.AreEqual( 9f, result.Y );
    }

    [Test]
    public void Operator_DivideTuple_ShouldDivideOffsets()
    {
        ReadOnlyPointF point  = new(4f, 6f);
        ReadOnlyPointF result = point / (2.0, 3.0);
        this.AreEqual( 2f, result.X );
        this.AreEqual( 2f, result.Y );
    }

    [Test]
    public void Operator_AddPointF_ShouldAddCoordinates()
    {
        ReadOnlyPointF a      = new(1.5f, 2.5f);
        ReadOnlyPointF b      = new(2.0f, 3.0f);
        ReadOnlyPointF result = a + b;
        this.AreEqual( 3.5f, result.X );
        this.AreEqual( 5.5f, result.Y );
    }

    [Test]
    public void Operator_SubtractPointF_ShouldSubtractCoordinates()
    {
        ReadOnlyPointF a      = new(5.5f, 6.5f);
        ReadOnlyPointF b      = new(2.0f, 3.0f);
        ReadOnlyPointF result = a - b;
        this.AreEqual( 3.5f, result.X );
        this.AreEqual( 3.5f, result.Y );
    }

    [Test]
    public void Operator_MultiplyPointF_ShouldMultiplyCoordinates()
    {
        ReadOnlyPointF a      = new(2f, 3f);
        ReadOnlyPointF b      = new(4f, 5f);
        ReadOnlyPointF result = a * b;
        this.AreEqual( 8f,  result.X );
        this.AreEqual( 15f, result.Y );
    }

    [Test]
    public void Operator_DividePointF_ShouldDivideCoordinates()
    {
        ReadOnlyPointF a      = new(8f, 9f);
        ReadOnlyPointF b      = new(2f, 3f);
        ReadOnlyPointF result = a / b;
        this.AreEqual( 4f, result.X );
        this.AreEqual( 3f, result.Y );
    }

    [Test]
    public void Operator_AddDrawingPointF_ShouldAddCoordinates()
    {
        ReadOnlyPointF a      = new(1.5f, 2.5f);
        PointF         b      = new(2f, 3f);
        ReadOnlyPointF result = a + b;
        this.AreEqual( 3.5f, result.X );
        this.AreEqual( 5.5f, result.Y );
    }

    [Test]
    public void Operator_SubtractDrawingPointF_ShouldSubtractCoordinates()
    {
        ReadOnlyPointF a      = new(5.5f, 6.5f);
        PointF         b      = new(2f, 3f);
        ReadOnlyPointF result = a - b;
        this.AreEqual( 3.5f, result.X );
        this.AreEqual( 3.5f, result.Y );
    }

    [Test]
    public void Operator_MultiplyDrawingPointF_ShouldMultiplyCoordinates()
    {
        ReadOnlyPointF a      = new(2f, 3f);
        PointF         b      = new(4f, 5f);
        ReadOnlyPointF result = a * b;
        this.AreEqual( 8f,  result.X );
        this.AreEqual( 15f, result.Y );
    }

    [Test]
    public void Operator_DivideDrawingPointF_ShouldDivideCoordinates()
    {
        ReadOnlyPointF a      = new(8f, 9f);
        PointF         b      = new(2f, 3f);
        ReadOnlyPointF result = a / b;
        this.AreEqual( 4f, result.X );
        this.AreEqual( 3f, result.Y );
    }

    [Test]
    public void Operator_AddDrawingPoint_ShouldAddCoordinates()
    {
        ReadOnlyPointF a      = new(1.5f, 2.5f);
        Point          b      = new(2, 3);
        ReadOnlyPointF result = a + b;
        this.AreEqual( 3.5f, result.X );
        this.AreEqual( 5.5f, result.Y );
    }

    [Test]
    public void Operator_SubtractDrawingPoint_ShouldSubtractCoordinates()
    {
        ReadOnlyPointF a      = new(5.5f, 6.5f);
        Point          b      = new(2, 3);
        ReadOnlyPointF result = a - b;
        this.AreEqual( 3.5f, result.X );
        this.AreEqual( 3.5f, result.Y );
    }

    [Test]
    public void Operator_MultiplyDrawingPoint_ShouldMultiplyCoordinates()
    {
        ReadOnlyPointF a      = new(2f, 3f);
        Point          b      = new(4, 5);
        ReadOnlyPointF result = a * b;
        this.AreEqual( 8f,  result.X );
        this.AreEqual( 15f, result.Y );
    }

    [Test]
    public void Operator_DivideDrawingPoint_ShouldDivideCoordinates()
    {
        ReadOnlyPointF a      = new(8f, 9f);
        Point          b      = new(2, 3);
        ReadOnlyPointF result = a / b;
        this.AreEqual( 4f, result.X );
        this.AreEqual( 3f, result.Y );
    }
}
