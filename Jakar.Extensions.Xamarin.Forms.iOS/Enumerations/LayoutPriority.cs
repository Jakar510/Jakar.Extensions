


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.iOS.Enumerations;


public enum LayoutPriority // iOS only
{
    Zero         = 0,  // 0
    Minimum      = 1,  // 1
    Lowest       = 50, // UILayoutPriority.FittingSizeLevel,
    VeryLow      = 100,
    Low          = 250,  // UILayoutPriority.DefaultLow,                              
    BelowAverage = 490,  // UILayoutPriority.DragThatCannotResizeScene; 
    Average      = 500,  // UILayoutPriority.SceneSizeStayPut;        
    AboveAverage = 510,  // UILayoutPriority.DragThatCanResizeScene;
    High         = 750,  // UILayoutPriority.DefaultHigh;                           
    Highest      = 999,  // 999
    Required     = 1000, //  UILayoutPriority.Required; 
}
