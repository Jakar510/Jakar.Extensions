// Jakar.Extensions :: Jakar.Extensions.Tests
// 06/26/2025  18:09

namespace Jakar.Shapes.Tests;


[TestFixture, TestOf( typeof(ReadOnlySizeF) )]
public sealed class ReadOnlySizeFF_Tests : Assert
{
    [Test]
    public void Zero_ShouldBeEmptyAndValid()
    {
        ReadOnlySizeF point = ReadOnlySizeF.Zero;
        this.AreEqual( 0, point.Width );
        this.AreEqual( 0, point.Height );
        this.IsTrue( point.IsEmpty );
        this.IsFalse( point.IsNaN );
        this.IsTrue( point.IsValid );
    }

    [Test]
    public void Invalid_ShouldBeNaNAndInvalid()
    {
        ReadOnlySizeF point = ReadOnlySizeF.Invalid;
        this.IsTrue( double.IsNaN( point.Width ) );
        this.IsTrue( double.IsNaN( point.Height ) );
        this.IsTrue( point.IsNaN );
        this.IsFalse( point.IsValid );
    }

    [Test]
    public void ImplicitConversion_ToSize_ShouldRoundValues()
    {
        ReadOnlySizeF point     = new(1.6f, 2.4f);
        Size          converted = point;
        this.AreEqual( 1, converted.Width );
        this.AreEqual( 2, converted.Height );
    }

    [Test]
    public void IsEmpty_WhenWidthOrHeightIsNotZero_ShouldBeFalse()
    {
        ReadOnlySizeF point = new(1, 0);
        this.IsFalse( point.IsEmpty );

        point = new ReadOnlySizeF( 0, 1 );
        this.IsFalse( point.IsEmpty );
    }

    //--------------------------------------------------------------------------------------

    [Test]
    public void Operator_Equality_ShouldReturnTrueForSameCoordinates()
    {
        ReadOnlySizeF a = new(1.5f, 2.5f);
        ReadOnlySizeF b = new(1.5f, 2.5f);
        this.IsTrue( a  == b );
        this.IsFalse( a != b );
        this.IsTrue( a.Equals( b ) );
    }

    [Test]
    public void Operator_Equality_ShouldReturnFalseForDifferentCoordinates()
    {
        ReadOnlySizeF a = new(1.5f, 2.5f);
        ReadOnlySizeF b = new(2.5f, 3.5f);
        this.IsFalse( a == b );
        this.IsTrue( a  != b );
        this.IsFalse( a.Equals( b ) );
    }

    [Test]
    public void Equals_ShouldReturnFalseWhenComparedWithNull()
    {
        ReadOnlySizeF a = new(1.5f, 2.5f);
        object?       b = null;
        this.IsFalse( a.Equals( b ) );
    }

    [Test]
    public void Equals_ShouldReturnFalseWhenComparedWithDifferentType()
    {
        ReadOnlySizeF a = new(1.5f, 2.5f);
        string        b = "not a point";

        // ReSharper disable once SuspiciousTypeConversion.Global
        this.IsFalse( a.Equals( b ) );
    }

    [Test]
    public void GetHashCode_ShouldBeSameForEqualValues()
    {
        ReadOnlySizeF a = new(1.5f, 2.5f);
        ReadOnlySizeF b = new(1.5f, 2.5f);
        this.AreEqual( a.GetHashCode(), b.GetHashCode() );
    }

    //--------------------------------------------------------------------------------------

    [Test]
    public void Round_ShouldRoundValues()
    {
        ReadOnlySizeF point     = new(1.6f, 2.4f);
        Size          converted = point.Round();
        this.AreEqual( 2, converted.Width );
        this.AreEqual( 2, converted.Height );
    }

    [Test]
    public void Floor_ShouldDecreaseValues()
    {
        ReadOnlySizeF point     = new(1.6f, 2.4f);
        Size          converted = point.Floor();
        this.AreEqual( 1, converted.Width );
        this.AreEqual( 2, converted.Height );
    }

    //--------------------------------------------------------------------------------------

    [Test]
    public void Operator_AddTuple_ShouldAddOffsets()
    {
        ReadOnlySizeF point  = new(1.5f, 2.5f);
        ReadOnlySizeF result = point + (1.0, 2.0);
        this.AreEqual( 2.5f, result.Width );
        this.AreEqual( 4.5f, result.Height );
    }

    [Test]
    public void Operator_SubtractTuple_ShouldSubtractOffsets()
    {
        ReadOnlySizeF point  = new(5.5f, 6.5f);
        ReadOnlySizeF result = point - (1.0, 2.0);
        this.AreEqual( 4.5f, result.Width );
        this.AreEqual( 4.5f, result.Height );
    }

    [Test]
    public void Operator_MultiplyTuple_ShouldMultiplyOffsets()
    {
        ReadOnlySizeF point  = new(2f, 3f);
        ReadOnlySizeF result = point * (2.0, 3.0);
        this.AreEqual( 4f, result.Width );
        this.AreEqual( 9f, result.Height );
    }

    [Test]
    public void Operator_DivideTuple_ShouldDivideOffsets()
    {
        ReadOnlySizeF point  = new(4f, 6f);
        ReadOnlySizeF result = point / (2.0, 3.0);
        this.AreEqual( 2f, result.Width );
        this.AreEqual( 2f, result.Height );
    }

    [Test]
    public void Operator_AddSizeF_ShouldAddCoordinates()
    {
        ReadOnlySizeF a      = new(1.5f, 2.5f);
        ReadOnlySizeF b      = new(2.0f, 3.0f);
        ReadOnlySizeF result = a + b;
        this.AreEqual( 3.5f, result.Width );
        this.AreEqual( 5.5f, result.Height );
    }

    [Test]
    public void Operator_SubtractSizeF_ShouldSubtractCoordinates()
    {
        ReadOnlySizeF a      = new(5.5f, 6.5f);
        ReadOnlySizeF b      = new(2.0f, 3.0f);
        ReadOnlySizeF result = a - b;
        this.AreEqual( 3.5f, result.Width );
        this.AreEqual( 3.5f, result.Height );
    }

    [Test]
    public void Operator_MultiplySizeF_ShouldMultiplyCoordinates()
    {
        ReadOnlySizeF a      = new(2f, 3f);
        ReadOnlySizeF b      = new(4f, 5f);
        ReadOnlySizeF result = a * b;
        this.AreEqual( 8f,  result.Width );
        this.AreEqual( 15f, result.Height );
    }

    [Test]
    public void Operator_DivideSizeF_ShouldDivideCoordinates()
    {
        ReadOnlySizeF a      = new(8f, 9f);
        ReadOnlySizeF b      = new(2f, 3f);
        ReadOnlySizeF result = a / b;
        this.AreEqual( 4f, result.Width );
        this.AreEqual( 3f, result.Height );
    }

    [Test]
    public void Operator_AddDrawingSizeF_ShouldAddCoordinates()
    {
        ReadOnlySizeF a      = new(1.5f, 2.5f);
        SizeF         b      = new(2f, 3f);
        ReadOnlySizeF result = a + b;
        this.AreEqual( 3.5f, result.Width );
        this.AreEqual( 5.5f, result.Height );
    }

    [Test]
    public void Operator_SubtractDrawingSizeF_ShouldSubtractCoordinates()
    {
        ReadOnlySizeF a      = new(5.5f, 6.5f);
        SizeF         b      = new(2f, 3f);
        ReadOnlySizeF result = a - b;
        this.AreEqual( 3.5f, result.Width );
        this.AreEqual( 3.5f, result.Height );
    }

    [Test]
    public void Operator_MultiplyDrawingSizeF_ShouldMultiplyCoordinates()
    {
        ReadOnlySizeF a      = new(2f, 3f);
        SizeF         b      = new(4f, 5f);
        ReadOnlySizeF result = a * b;
        this.AreEqual( 8f,  result.Width );
        this.AreEqual( 15f, result.Height );
    }

    [Test]
    public void Operator_DivideDrawingSizeF_ShouldDivideCoordinates()
    {
        ReadOnlySizeF a      = new(8f, 9f);
        SizeF         b      = new(2f, 3f);
        ReadOnlySizeF result = a / b;
        this.AreEqual( 4f, result.Width );
        this.AreEqual( 3f, result.Height );
    }

    [Test]
    public void Operator_AddDrawingSize_ShouldAddCoordinates()
    {
        ReadOnlySizeF a      = new(1.5f, 2.5f);
        Size          b      = new(2, 3);
        ReadOnlySizeF result = a + b;
        this.AreEqual( 3.5f, result.Width );
        this.AreEqual( 5.5f, result.Height );
    }

    [Test]
    public void Operator_SubtractDrawingSize_ShouldSubtractCoordinates()
    {
        ReadOnlySizeF a      = new(5.5f, 6.5f);
        Size          b      = new(2, 3);
        ReadOnlySizeF result = a - b;
        this.AreEqual( 3.5f, result.Width );
        this.AreEqual( 3.5f, result.Height );
    }

    [Test]
    public void Operator_MultiplyDrawingSize_ShouldMultiplyCoordinates()
    {
        ReadOnlySizeF a      = new(2f, 3f);
        Size          b      = new(4, 5);
        ReadOnlySizeF result = a * b;
        this.AreEqual( 8f,  result.Width );
        this.AreEqual( 15f, result.Height );
    }

    [Test]
    public void Operator_DivideDrawingSize_ShouldDivideCoordinates()
    {
        ReadOnlySizeF a      = new(8f, 9f);
        Size          b      = new(2, 3);
        ReadOnlySizeF result = a / b;
        this.AreEqual( 4f, result.Width );
        this.AreEqual( 3f, result.Height );
    }
}
