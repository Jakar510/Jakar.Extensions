// Jakar.Extensions :: Jakar.Extensions.Tests
// 06/27/2025  10:00

namespace Jakar.Shapes.Tests;


[TestFixture]
[TestOf(typeof(MutableRectangle))]
public sealed class MutableRectangle_Tests : Assert
{
    private static readonly ReadOnlyRectangle __fiveRect = new(5, 5, 5, 5);
    private static readonly ReadOnlyPoint     __two      = 2;

    [Test] public void Create_ShouldBeSizedToMaxDimensionsOfPoints()
    {
        ReadOnlySpan<ReadOnlyPoint> points =
        [
            new(0, 0),
            new(0, 1),
            new(double.Pi, double.Pi),
            new(5, 5),
            new(double.Pi, 5),
            new(5, double.Pi),
            new(3, 5),
            new(3, 4),
            new(10, 10)
        ];

        MutableRectangle result = MutableRectangle.Create(points);
        this.AreEqual(0,  result.X);
        this.AreEqual(10, result.MaxWidth());
        this.AreEqual(0,  result.Y);
        this.AreEqual(10, result.MaxHeight());
    }


    [Test] public void Addition_With_Int_IncreasesSizeOnly()
    {
        MutableRectangle result = MutableRectangle.Zero;
        result += 5;
        this.AreEqual(0, result.X);
        this.AreEqual(0, result.Y);
        this.AreEqual(5, result.Width);
        this.AreEqual(5, result.Height);
    }
    [Test] public void Addition_With_Float_IncreasesSizeOnly()
    {
        MutableRectangle result = MutableRectangle.Zero;
        result += 5.0f;
        this.AreEqual(0, result.X);
        this.AreEqual(0, result.Y);
        this.AreEqual(5, result.Width);
        this.AreEqual(5, result.Height);
    }
    [Test] public void Addition_With_Double_IncreasesSizeOnly()
    {
        MutableRectangle result = MutableRectangle.Zero;
        result += 5.0;
        this.AreEqual(0, result.X);
        this.AreEqual(0, result.Y);
        this.AreEqual(5, result.Width);
        this.AreEqual(5, result.Height);
    }


    [Test] public void Subtraction_With_Int_DecreasesSizeOnly()
    {
        MutableRectangle result = new(0, 0, 5, 5);
        result -= 1;
        this.AreEqual(0, result.X);
        this.AreEqual(0, result.Y);
        this.AreEqual(4, result.Width);
        this.AreEqual(4, result.Height);
    }
    [Test] public void Subtraction_With_Float_DecreasesSizeOnly()
    {
        MutableRectangle result = new(0, 0, 5, 5);
        result -= 1.5f;
        this.AreEqual(0,   result.X);
        this.AreEqual(0,   result.Y);
        this.AreEqual(3.5, result.Width);
        this.AreEqual(3.5, result.Height);
    }
    [Test] public void Subtraction_With_Double_DecreasesSizeOnly()
    {
        MutableRectangle result = new(0, 0, 5, 5);
        result -= 1.5;
        this.AreEqual(0,   result.X);
        this.AreEqual(0,   result.Y);
        this.AreEqual(3.5, result.Width);
        this.AreEqual(3.5, result.Height);
    }


    [Test] public void Multiplication_With_Int_ScalesSizeOnly()
    {
        MutableRectangle result = new(0, 0, 1, 1);
        result *= 2;
        this.AreEqual(0, result.X);
        this.AreEqual(0, result.Y);
        this.AreEqual(2, result.Width);
        this.AreEqual(2, result.Height);
    }
    [Test] public void Multiplication_With_Float_ScalesSizeOnly()
    {
        MutableRectangle result = new(0, 0, 1, 1);
        result *= 2.0f;
        this.AreEqual(0, result.X);
        this.AreEqual(0, result.Y);
        this.AreEqual(2, result.Width);
        this.AreEqual(2, result.Height);
    }
    [Test] public void Multiplication_With_Double_ScalesSizeOnly()
    {
        MutableRectangle result = new(0, 0, 1, 1);
        result *= 2.0;
        this.AreEqual(0, result.X);
        this.AreEqual(0, result.Y);
        this.AreEqual(2, result.Width);
        this.AreEqual(2, result.Height);
    }


    [Test] public void Division_With_Int_ScalesSizeOnly()
    {
        MutableRectangle result = new(0, 0, 1, 1);
        result /= 2;
        this.AreEqual(0,   result.X);
        this.AreEqual(0,   result.Y);
        this.AreEqual(0.5, result.Width);
        this.AreEqual(0.5, result.Height);
    }
    [Test] public void Division_With_Float_ScalesSizeOnly()
    {
        MutableRectangle result = new(0, 0, 1, 1);
        result /= 2.0f;
        this.AreEqual(0,   result.X);
        this.AreEqual(0,   result.Y);
        this.AreEqual(0.5, result.Width);
        this.AreEqual(0.5, result.Height);
    }
    [Test] public void Division_With_Double_ScalesSizeOnly()
    {
        MutableRectangle result = new(0, 0, 1, 1);
        result /= 2.0;
        this.AreEqual(0,   result.X);
        this.AreEqual(0,   result.Y);
        this.AreEqual(0.5, result.Width);
        this.AreEqual(0.5, result.Height);
    }


    [Test] public void Subtraction_With_TupleOffset_AdjustsPositionOnly()
    {
        MutableRectangle result = new(1, 1, 5, 5);
        result -= ( xOffset: 0.5, yOffset: 1.5 );

        this.AreEqual(1,   result.X);
        this.AreEqual(1,   result.Y);
        this.AreEqual(4.5, result.Width);
        this.AreEqual(3.5, result.Height);
    }
    [Test] public void Addition_With_TupleOffset_AdjustsPositionOnly()
    {
        MutableRectangle result = new(1, 1, 5, 5);
        result += ( xOffset: 0.5, yOffset: 1.5 );

        this.AreEqual(1,   result.X);
        this.AreEqual(1,   result.Y);
        this.AreEqual(5.5, result.Width);
        this.AreEqual(6.5, result.Height);
    }
    [Test] public void Division_With_TupleOffset_AdjustsPositionOnly()
    {
        MutableRectangle result = new(1, 1, 5, 5);
        result /= ( xOffset: 0.5, yOffset: 1.5 );

        this.AreEqual(1,       result.X);
        this.AreEqual(1,       result.Y);
        this.AreEqual(10,      result.Width);
        this.AreEqual(5 / 1.5, result.Height);
    }
    [Test] public void Multiplication_With_TupleOffset_AdjustsPositionOnly()
    {
        MutableRectangle result = new(1, 1, 5, 5);
        result *= ( xOffset: 0.5, yOffset: 1.5 );

        this.AreEqual(1,   result.X);
        this.AreEqual(1,   result.Y);
        this.AreEqual(2.5, result.Width);
        this.AreEqual(7.5, result.Height);
    }

    [Test] public void Addition_With_ReadOnlyPointF_AdjustsPositionOnly()
    {
        MutableRectangle result = new(0, 0, 5, 5);
        result += ReadOnlyPointF.One * 2;
        this.AreEqual(2, result.X);
        this.AreEqual(2, result.Y);
        this.AreEqual(5, result.Width);
        this.AreEqual(5, result.Height);
    }
    [Test] public void Subtraction_With_ReadOnlyPointF_AdjustsPositionOnly()
    {
        MutableRectangle result = new(0, 0, 5, 5);
        result -= ReadOnlyPointF.One * 2;
        this.AreEqual(-2, result.X);
        this.AreEqual(-2, result.Y);
        this.AreEqual(5,  result.Width);
        this.AreEqual(5,  result.Height);
    }
    [Test] public void Division_With_ReadOnlyPointF_AdjustsPositionOnly()
    {
        MutableRectangle result = new(0, 0, 5, 5);
        result /= ReadOnlyPointF.One * 2;
        this.AreEqual(0, result.X);
        this.AreEqual(0, result.Y);
        this.AreEqual(5, result.Width);
        this.AreEqual(5, result.Height);
    }
    [Test] public void Multiplication_With_ReadOnlyPointF_AdjustsPositionOnly()
    {
        MutableRectangle result = new(5, 5, 5, 5);
        result *= ReadOnlyPointF.One * 2;
        this.AreEqual(10, result.X);
        this.AreEqual(10, result.Y);
        this.AreEqual(5,  result.Width);
        this.AreEqual(5,  result.Height);
    }


    [Test] public void Addition_With_SystemDrawingPointF_AdjustsPositionOnly()
    {
        MutableRectangle result = new(0, 0, 5, 5);
        result += new PointF(2, 2);
        this.AreEqual(2, result.X);
        this.AreEqual(2, result.Y);
        this.AreEqual(5, result.Width);
        this.AreEqual(5, result.Height);
    }
    [Test] public void Subtraction_With_SystemDrawingPointF_AdjustsPositionOnly()
    {
        MutableRectangle result = new(0, 0, 5, 5);
        result -= new PointF(2, 2);
        this.AreEqual(-2, result.X);
        this.AreEqual(-2, result.Y);
        this.AreEqual(5,  result.Width);
        this.AreEqual(5,  result.Height);
    }
    [Test] public void Division_With_SystemDrawingPointF_AdjustsPositionOnly()
    {
        MutableRectangle result = new(0, 0, 5, 5);
        result -= new PointF(2, 2);
        this.AreEqual(-2, result.X);
        this.AreEqual(-2, result.Y);
        this.AreEqual(5,  result.Width);
        this.AreEqual(5,  result.Height);
    }
    [Test] public void Multiplication_With_SystemDrawingPointF_AdjustsPositionOnly()
    {
        MutableRectangle result = new(0, 0, 5, 5);
        result -= new PointF(2, 2);
        this.AreEqual(-2, result.X);
        this.AreEqual(-2, result.Y);
        this.AreEqual(5,  result.Width);
        this.AreEqual(5,  result.Height);
    }


    [Test] public void Addition_With_ReadOnlyPoint()
    {
        MutableRectangle  result = __fiveRect;
        ReadOnlyRectangle other  = ReadOnlyRectangle.Create(in __two, in ReadOnlySize.One);
        Console.WriteLine(result);
        result += other;
        Console.WriteLine(other);
        Console.WriteLine(result);
        this.AreEqual(7, result.X);
        this.AreEqual(7, result.Y);
        this.AreEqual(6, result.Width);
        this.AreEqual(6, result.Height);
    }
    [Test] public void Subtraction_With_ReadOnlyPoint()
    {
        MutableRectangle  result = __fiveRect;
        ReadOnlyRectangle other  = ReadOnlyRectangle.Create(in __two, in ReadOnlySize.One);
        result -= other;
        Console.WriteLine(other);
        Console.WriteLine(result);
        this.AreEqual(3, result.X);
        this.AreEqual(3, result.Y);
        this.AreEqual(4, result.Width);
        this.AreEqual(4, result.Height);
    }
    [Test] public void Division_With_ReadOnlyPoint()
    {
        MutableRectangle  result = __fiveRect;
        ReadOnlyRectangle other  = ReadOnlyRectangle.Create(in __two, in ReadOnlySize.One);
        result /= other;
        Console.WriteLine(other);
        Console.WriteLine(result);
        this.AreEqual(2.5, result.X);
        this.AreEqual(2.5, result.Y);
        this.AreEqual(5,   result.Width);
        this.AreEqual(5,   result.Height);
    }
    [Test] public void Multiplication_With_ReadOnlyPoint()
    {
        MutableRectangle  result = __fiveRect;
        ReadOnlyRectangle other  = ReadOnlyRectangle.Create(in __two, in ReadOnlySize.One);
        result *= other;
        Console.WriteLine(other);
        Console.WriteLine(result);
        this.AreEqual(10, result.X);
        this.AreEqual(10, result.Y);
        this.AreEqual(5,  result.Width);
        this.AreEqual(5,  result.Height);
    }


    [Test] public void ImplicitConversions_ToAndFrom_SystemRectangleAndRectangleF()
    {
        MutableRectangle r    = new(10.2, 20.4, 5.6, 7);
        Rectangle        rInt = r;
        RectangleF       rFlt = r;

        this.AreEqual(10, rInt.X);
        this.AreEqual(20, rInt.Y);
        this.AreEqual(5,  rInt.Width);
        this.AreEqual(7,  rInt.Height);

        this.AreEqual(10.2f, rFlt.X);
        this.AreEqual(20.4f, rFlt.Y);
        this.AreEqual(5.6f,  rFlt.Width);
        this.AreEqual(7f,    rFlt.Height);

        MutableRectangle backFromInt = rInt;
        MutableRectangle backFromFlt = rFlt;
        this.IsTrue(r >= backFromInt);
        this.IsTrue(r >= backFromFlt);
    }


    [Test] public void ComparisonAndEqualityOperators_WorkAsExpected()
    {
        MutableRectangle a = new(0, 0, 1, 1);
        MutableRectangle b = new(0, 0, 1, 1);
        MutableRectangle c = new(1, 0, 0, 2);

        this.IsTrue(a              == b);
        this.IsFalse(a             != b);
        this.IsTrue(c              > a);
        this.IsTrue(a              < c);
        this.IsTrue(a.CompareTo(b) == 0);
        this.IsTrue(a.Equals(b));
        this.IsTrue(a.CompareTo(null) > 0);
    }


    [Test] public void Ampersand_Replacement_ReadOnlyPoint_WorkAsExpected()
    {
        MutableRectangle result = __fiveRect;
        result &= __two;
        this.AreEqual(2, result.X);
        this.AreEqual(2, result.Y);
        this.AreEqual(5, result.Width);
        this.AreEqual(5, result.Height);
    }
    [Test] public void Ampersand_Replacement_ReadOnlyPointF_WorkAsExpected()
    {
        MutableRectangle result = __fiveRect;
        result &= ( ReadOnlyPointF.One * 2 );
        this.AreEqual(2, result.X);
        this.AreEqual(2, result.Y);
        this.AreEqual(5, result.Width);
        this.AreEqual(5, result.Height);
    }
    [Test] public void Ampersand_Replacement_Point_WorkAsExpected()
    {
        MutableRectangle result = __fiveRect;
        result &= new Point(2, 2);
        this.AreEqual(2, result.X);
        this.AreEqual(2, result.Y);
        this.AreEqual(5, result.Width);
        this.AreEqual(5, result.Height);
    }
    [Test] public void Ampersand_Replacement_PointF_WorkAsExpected()
    {
        MutableRectangle result = __fiveRect;
        result &= new PointF(2, 2);
        this.AreEqual(2, result.X);
        this.AreEqual(2, result.Y);
        this.AreEqual(5, result.Width);
        this.AreEqual(5, result.Height);
    }


    [Test] public void Ampersand_Replacement_ReadOnlySize_WorkAsExpected()
    {
        MutableRectangle result = __fiveRect;
        result &= __two;
        this.AreEqual(2, result.X);
        this.AreEqual(2, result.Y);
        this.AreEqual(5, result.Width);
        this.AreEqual(5, result.Height);
    }
    [Test] public void Ampersand_Replacement_ReadOnlySizeF_WorkAsExpected()
    {
        MutableRectangle result = __fiveRect;
        result &= ( ReadOnlySizeF.One * 2 );
        this.AreEqual(5, result.X);
        this.AreEqual(5, result.Y);
        this.AreEqual(2, result.Width);
        this.AreEqual(2, result.Height);
    }
    [Test] public void Ampersand_Replacement_Size_WorkAsExpected()
    {
        MutableRectangle result = __fiveRect;
        result &= new Size(2, 2);
        this.AreEqual(5, result.X);
        this.AreEqual(5, result.Y);
        this.AreEqual(2, result.Width);
        this.AreEqual(2, result.Height);
    }
    [Test] public void Ampersand_Replacement_SizeF_WorkAsExpected()
    {
        MutableRectangle result = __fiveRect;
        result &= new SizeF(2, 2);
        this.AreEqual(5, result.X);
        this.AreEqual(5, result.Y);
        this.AreEqual(2, result.Width);
        this.AreEqual(2, result.Height);
    }


    [Test] public void Addition_With_ReadOnlyThickness_ScalesPositionOnly()
    {
        MutableRectangle result = __fiveRect;
        result += ReadOnlyThickness.One * 2;
        this.AreEqual(3, result.X);
        this.AreEqual(3, result.Y);
        this.AreEqual(7, result.Width);
        this.AreEqual(7, result.Height);
    }
    [Test] public void Subtraction_With_ReadOnlyThickness_ScalesPositionOnly()
    {
        MutableRectangle result = __fiveRect;
        result -= ReadOnlyThickness.One * 2;
        this.AreEqual(7, result.X);
        this.AreEqual(7, result.Y);
        this.AreEqual(3, result.Width);
        this.AreEqual(3, result.Height);
    }
}
