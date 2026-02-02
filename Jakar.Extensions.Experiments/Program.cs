using System.Globalization;
using System.Runtime.CompilerServices;


Console.WriteLine(DateTimeOffset.UtcNow.ToString());

ReadOnlySpan<string> span = ["one", "two", "three"];

ReadOnlySpan<int> numbers =
[
    1,
    2,
    3,
    4,
    5,
    6,
    7,
    8,
    9,
    10
];


span.Hash()
    .WriteToConsole();

span.Hash128()
    .WriteToConsole();


numbers.Hash()
       .WriteToConsole();

numbers.Hash128()
       .WriteToConsole();
