// Jakar.Extensions :: Jakar.Extensions.Tests
// 06/27/2025  10:00

using System.Drawing;



namespace Jakar.Extensions.Tests.Shapes;


[TestFixture, TestOf(typeof(RectangleD))]
public sealed class RectangleD_Tests : Assert
{
    [Test]
    public void Create_ShouldBeSizedToMaxDimensionsOfPoints()
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
            new(10, 10),
        ];

        RectangleD rectangle = RectangleD.Create(points);
        this.AreEqual(0,  rectangle.Left);
        this.AreEqual(10, rectangle.Right);
        this.AreEqual(0,  rectangle.Top);
        this.AreEqual(10, rectangle.Bottom);
    }


    private readonly RectangleD _base = new(1, 2, 3, 4);

    [Test]
    public void Addition_WithInt_IncreasesSizeOnly()
    {
        RectangleD result = _base + 5;
        this.AreEqual(1, result.X);
        this.AreEqual(2, result.Y);
        this.AreEqual(8, result.Width);
        this.AreEqual(9, result.Height);
    }

    [Test]
    public void Subtraction_WithFloat_DecreasesSizeOnly()
    {
        RectangleD result = _base - 1.5f;
        this.AreEqual(1,    result.X);
        this.AreEqual(2,    result.Y);
        this.AreEqual(1.5f, result.Width);
        this.AreEqual(2.5f, result.Height);
    }

    [Test]
    public void Multiplication_WithDouble_ScalesSizeOnly()
    {
        RectangleD result = _base * 2.0;
        this.AreEqual(1,   result.X);
        this.AreEqual(2,   result.Y);
        this.AreEqual(6.0, result.Width);
        this.AreEqual(8.0, result.Height);
    }

    [Test]
    public void Division_WithTupleOffset_AdjustsPositionOnly()
    {
        (double xOffset, double yOffset) offset = ( xOffset: 0.5, yOffset: 1.5 );
        RectangleD                       result = _base / offset;
        this.AreEqual(1                                 / 0.5, result.X);
        this.AreEqual(2                                 / 1.5, result.Y);
        this.AreEqual(3,                                       result.Width);
        this.AreEqual(4,                                       result.Height);
    }

    [Test]
    public void Addition_WithReadOnlyPointF_AdjustsPositionOnly()
    {
        ReadOnlyPointF pointF = new(2.5f, 3.5f);
        RectangleD     result = _base + pointF;
        this.AreEqual(3.5, result.X);
        this.AreEqual(5.5, result.Y);
        this.AreEqual(3,   result.Width);
        this.AreEqual(4,   result.Height);
    }

    [Test]
    public void Subtraction_WithSystemDrawingPointF_AdjustsPositionOnly()
    {
        PointF     pt     = new(1.0f, 2.0f);
        RectangleD result = _base - pt;
        this.AreEqual(0, result.X);
        this.AreEqual(0, result.Y);
        this.AreEqual(3, result.Width);
        this.AreEqual(4, result.Height);
    }

    [Test]
    public void Multiplication_WithReadOnlyPoint_ScalesPositionOnly()
    {
        ReadOnlyPoint point  = new(2, 3);
        RectangleD    result = _base * new ReadOnlyRectangle(point.X, point.Y, 1, 1);

        // Here & operator sets position; then * on size
        this.IsTrue(result is { X: > 0, Y: > 0 });
    }

    [Test]
    public void ImplicitConversions_ToAndFrom_SystemRectangleAndRectangleF()
    {
        RectangleD r    = new(10.2, 20.4, 5.6, 7);
        Rectangle  rInt = r;
        RectangleF rFlt = r;
        
        this.AreEqual(10, rInt.X);
        this.AreEqual(20, rInt.Y);
        this.AreEqual(5,  rInt.Width);
        this.AreEqual(7,  rInt.Height);

        this.AreEqual(10.2f, rFlt.X);
        this.AreEqual(20.4f, rFlt.Y);
        this.AreEqual(5.6f,  rFlt.Width);
        this.AreEqual(7f,    rFlt.Height);

        RectangleD backFromInt = rInt;
        RectangleD backFromFlt = rFlt;
        this.IsTrue(r >= backFromInt);
        this.IsTrue(r >= backFromFlt);
    }

    [Test]
    public void ComparisonAndEqualityOperators_WorkAsExpected()
    {
        RectangleD a = new(0, 0, 1, 1);
        RectangleD b = new(0, 0, 1, 1);
        RectangleD c = new(1, 1, 2, 2);

        this.IsTrue(a              == b);
        this.IsFalse(a             != b);
        this.IsTrue(c              > a);
        this.IsTrue(a              < c);
        this.IsTrue(a.CompareTo(b) == 0);
        this.IsTrue(a.Equals(b));
        this.IsTrue(a.CompareTo(null) > 0);
    }
}
