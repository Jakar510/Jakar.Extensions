namespace Jakar.Extensions;


public static class ConsoleExtensions
{
    private const string ERROR       = nameof(ERROR);
    private const string INFORMATION = nameof(INFORMATION);
    private const string WARNING     = nameof(WARNING);
    public static bool   CanDebug => Debugger.IsAttached;
    public static string Header   { get; set; } = '='.Repeat(100);


    [RequiresUnreferencedCode("Metadata for the method might be incomplete or removed." + Json.SerializationUnreferencedCode)][RequiresDynamicCode(Json.SerializationRequiresDynamicCode)]
    public static StringBuilder WrapException<TValue>( this TValue self, char c = '-', int padding = 40 )
        where TValue : Exception
    {
        StringBuilder builder = new();
        builder.AppendLine(" Exception Start ".Wrapper(c, padding));
        builder.AppendLine();
        builder.AppendLine(typeof(TValue).FullName);
        builder.AppendLine();
        builder.AppendLine();

        ExceptionDetails details = self.FullDetails();
        builder.AppendLine(details.ToJson());

        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(" Exception End ".Wrapper(c, padding));
        return builder;
    }


    public static StringBuilder PrintJson<TValue>( this TValue self, char c = '-', int length = 80 )
        where TValue : IJsonModel<TValue> => self.PrintJson(TValue.JsonTypeInfo, c, length);
    public static StringBuilder PrintJson<TValue>( this TValue self, JsonTypeInfo<TValue> info, char c = '-', int length = 80 )
    {
        string wrapper = c.Repeat(length);

        StringBuilder builder = new();
        builder.AppendLine();
        builder.AppendLine(wrapper);
        builder.AppendLine(typeof(TValue).FullName);
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(self.ToJson(info));
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(wrapper);
        builder.AppendLine();
        return builder;
    }


    public static string GetMessage( this string self, string start = INFORMATION, char c = '-', int padding = 40 ) => $"{start.Wrapper(c, padding)}  {self}";
    public static string GetWarning( this string self, string start = WARNING,     char c = '-', int padding = 40 ) => $"{start.Wrapper(c, padding)}  {self}";
    public static string GetError( this   string self, string start = ERROR,       char c = '-', int padding = 40 ) => $"{start.Wrapper(c, padding)}  {self}";


    public static string GetCount( this         string              self, int  count,   char c      = '-', int length = 80 ) => $"{c.Repeat(length)}   {self}.Count: => {count}";
    public static string GetCount( this         ICollection         self, char c = '-', int  length = 80 ) => $"{c.Repeat(length)}   {self.GetType().Name}.Count: => {self.Count}";
    public static string GetCount<TValue>( this ICollection<TValue> self, char c = '-', int  length = 80 ) => $"{c.Repeat(length)}   {self.GetType().Name}.Count: => {self.Count}";


    public static void Print( this Span<char>         self ) => Console.Write(self.ToString());
    public static void Print( this ReadOnlySpan<char> self ) => Console.Write(self.ToString());
    public static void Print( this string             self ) => Console.Write(self);
    public static void Print( this ValueStringBuilder self ) => self.ToString().Print();
    public static void Print( this StringBuilder      self ) => self.ToString().Print();
    public static void Print( this object             self ) => Console.Write(self);


    public static void PrintLine( this Span<char>         self ) => Console.WriteLine(self.ToString());
    public static void PrintLine( this ReadOnlySpan<char> self ) => Console.WriteLine(self.ToString());
    public static void PrintLine( this string             self ) => Console.WriteLine(self);
    public static void PrintLine( this ValueStringBuilder self ) => self.ToString().PrintLine();
    public static void PrintLine( this StringBuilder      self ) => self.ToString().PrintLine();
    public static void PrintLine( this object             self ) => Console.WriteLine(self);


    public static void WriteToConsole( this Span<char>         self ) => self.ToString().WriteToConsole();
    public static void WriteToConsole( this ReadOnlySpan<char> self ) => self.ToString().WriteToConsole();
    public static void WriteToConsole( this ValueStringBuilder self ) => self.Span.WriteToConsole();
    public static void WriteToConsole( this Buffer<char>       self ) => self.Span.WriteToConsole();
    public static void WriteToConsole( this StringBuilder      self ) => self.ToString().WriteToConsole();
    public static void WriteToConsole<TValue>( this TValue self )
        where TValue : notnull => self.ToString()?.WriteToConsole();
    public static void WriteToConsole( this string self )
    {
        Console.WriteLine();
        Console.WriteLine(self);
        Console.WriteLine();
    }


    public static void WriteToDebug( this Span<char>         self, [CallerArgumentExpression("self")] string? variable = null, [CallerMemberName] string? caller = null ) => Debug.WriteLine($"{caller} -> {variable} '{self}'");
    public static void WriteToDebug( this ReadOnlySpan<char> self, [CallerArgumentExpression("self")] string? variable = null, [CallerMemberName] string? caller = null ) => Debug.WriteLine($"{caller} -> {variable} '{self}'");
    public static void WriteToDebug( this string             self, [CallerArgumentExpression("self")] string? variable = null, [CallerMemberName] string? caller = null ) => Debug.WriteLine($"{caller} -> {variable} '{self}'");
    public static void WriteToDebug( this StringBuilder      self, [CallerArgumentExpression("self")] string? variable = null, [CallerMemberName] string? caller = null ) => self.ToString().WriteToDebug(variable, caller);
    public static void WriteToDebug( this Buffer<char>       self, [CallerArgumentExpression("self")] string? variable = null, [CallerMemberName] string? caller = null ) => Debug.WriteLine($"{caller} -> {variable} '{self.Span}'");
    public static void WriteToDebug( this ValueStringBuilder self, [CallerArgumentExpression("self")] string? variable = null, [CallerMemberName] string? caller = null ) => Debug.WriteLine($"{caller} -> {variable} '{self.Span}'");
    public static void WriteToDebug( this object             self, [CallerArgumentExpression("self")] string? variable = null, [CallerMemberName] string? caller = null ) => Debug.WriteLine($"{caller} -> {variable} '{self}'");

    public static void WriteToDebug<TValue>( this TValue self, [CallerArgumentExpression("self")] string? variable = null, [CallerMemberName] string? caller = null )
        where TValue : notnull => Debug.WriteLine($"{caller} -> {variable} '{self}'");
}
