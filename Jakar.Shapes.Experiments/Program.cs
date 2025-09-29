// See https://aka.ms/new-console-template for more information

using Jakar.Shapes;


Console.WriteLine("Hello, World!");


CalculatedLine line = CalculatedLine.Create(static x => Math.Pow(x, 3) - Math.Pow(x, 2) + x + 1);

ReadOnlySpan<ReadOnlyPoint> span =
[
    line.Get(-9.5),
    line.Get(-7.5),
    line.Get(-5.5),
    line.Get(-3.5),
    line.Get(-2.5),
    line.Get(-2),
    line.Get(-1),
    line.Get(0),
    line.Get(1),
    line.Get(2),
    line.Get(2.5),
    line.Get(3.5),
    line.Get(5.5),
    line.Get(7.5),
    line.Get(9.5),
];

Console.WriteLine();
foreach ( var point in span ) { Console.WriteLine(point); }

Console.WriteLine();


// CalculatedLine lwi2 = CalculatedLine.CreateWithIntercept(2, span);
// CalculatedLine lni2 = CalculatedLine.CreateNoIntercept(2, span);
// CalculatedLine lg2  = CalculatedLine.CreateWithLog(2, span);


// Console.WriteLine($"With Intercept: {lwi2[2]}");
// Console.WriteLine($"No Intercept:   {lni2[2]}");
// Console.WriteLine($"Logarithmic:   {lg2[2]}");
Console.WriteLine($"Actual:        {line[2]}");
