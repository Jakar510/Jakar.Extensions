// TrueLogic :: iTrueLogic
// 09/26/2024  16:09

namespace Jakar.Shapes;


[Flags]
public enum Quadrant : ulong
{
    None        = 0,
    Bottom      = 1,
    Top         = 2,
    Left        = 4,
    Right       = 8,
    TopLeft     = Top    | Left,
    TopRight    = Top    | Right,
    BottomLeft  = Bottom | Left,
    BottomRight = Bottom | Right
}
