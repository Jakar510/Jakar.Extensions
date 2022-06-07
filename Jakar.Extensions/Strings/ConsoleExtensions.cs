#nullable enable
namespace Jakar.Extensions.Strings;


public static class ConsoleExtensions
{
    private const string INFORMATION = nameof(INFORMATION);
    private const string WARNING     = nameof(WARNING);
    private const string ERROR       = nameof(ERROR);
    public static bool   CanDebug => Debugger.IsAttached;

    public static StringBuilder WrapException( this Exception self, in char c = '-', in int padding = 40 )
    {
        var builder = new StringBuilder();
        builder.AppendLine(" Exception Start ".Wrapper(c, padding));
        builder.AppendLine();
        builder.AppendLine(self.GetType().FullName);
        builder.AppendLine();
        builder.AppendLine();

        ExceptionDetails details = self.FullDetails();
        builder.AppendLine(details.ToPrettyJson());

        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(" Exception End ".Wrapper(c, padding));
        return builder;
    }

    public static StringBuilder WrapJson( this object self, char c = '-', int length = 80 )
    {
        string wrapper = c.Repeat(length);

        var builder = new StringBuilder();
        builder.AppendLine();
        builder.AppendLine(wrapper);
        builder.AppendLine(self.GetType().FullName);
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(self.ToPrettyJson());
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(wrapper);
        builder.AppendLine();
        return builder;
    }

    public static StringBuilder WrapJson( this IEnumerable self, char c = '-', int length = 80 )
    {
        string wrapper = c.Repeat(length);

        var builder = new StringBuilder();
        builder.AppendLine();
        builder.AppendLine(wrapper);
        builder.AppendLine(self.GetType().FullName);
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(self.ToPrettyJson());
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(wrapper);
        builder.AppendLine();
        return builder;
    }

    public static StringBuilder WrapJson<T>( this IEnumerable<T> self, char c = '-', int length = 80 )
    {
        string wrapper = c.Repeat(length);

        var builder = new StringBuilder();
        builder.AppendLine();
        builder.AppendLine(wrapper);
        builder.AppendLine(self.GetType().FullName);
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(self.ToPrettyJson());
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine(wrapper);
        builder.AppendLine();
        return builder;
    }


    public static string GetMessage( this  string         self, string start = INFORMATION, in char c      = '-', in int padding = 40 ) => $"{start.Wrapper(c, padding)}  {self}";
    public static string GetWarning( this  string         self, string start = WARNING,     in char c      = '-', in int padding = 40 ) => $"{start.Wrapper(c, padding)}  {self}";
    public static string GetError( this    string         self, string start = ERROR,       in char c      = '-', in int padding = 40 ) => $"{start.Wrapper(c, padding)}  {self}";
    public static string GetCount( this    string         self, int    count,               char    c      = '-', int    length  = 80 ) => $"{c.Repeat(length)}   {self}.Count: => {count}";
    public static string GetCount( this    ICollection    self, char   c = '-',             int     length = 80 ) => $"{c.Repeat(length)}   {self.GetType().Name}.Count: => {self.Count}";
    public static string GetCount<T>( this ICollection<T> self, char   c = '-',             int     length = 80 ) => $"{c.Repeat(length)}   {self.GetType().Name}.Count: => {self.Count}";


    public static string Header { get; set; } = '='.Repeat(100);


#if NETSTANDARD2_1
    public static void WriteToConsole( this ReadOnlySpan<char> self, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine(Header);
        Console.WriteLine(caller);
        Console.WriteLine();
        Console.WriteLine(self.ToString());
        Console.WriteLine();
    }
    public static void WriteToConsole( this string self, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine(Header);
        Console.WriteLine(caller);
        Console.WriteLine();
        Console.WriteLine(self);
        Console.WriteLine();
    }
    public static void WriteToConsole( this StringBuilder self, [CallerMemberName] string? caller = default ) => self.ToString().WriteToConsole(caller);
    public static void WriteToConsole( this object self, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine(Header);
        Console.WriteLine(caller);
        Console.WriteLine();
        Console.WriteLine(self);
        Console.WriteLine();
    }


    [Conditional("DEBUG")]
    public static void WriteToDebug( this ReadOnlySpan<char> self, [CallerMemberName] string? caller = default )
    {
        Debug.WriteLine(Header);
        Debug.WriteLine(caller);
        Debug.WriteLine(string.Empty);
        Debug.WriteLine(self.ToString());
        Debug.WriteLine(string.Empty);
    }
    [Conditional("DEBUG")]
    public static void WriteToDebug( this string self, [CallerMemberName] string? caller = default )
    {
        Debug.WriteLine(Header);
        Debug.WriteLine(caller);
        Debug.WriteLine(string.Empty);
        Debug.WriteLine(self);
        Debug.WriteLine(string.Empty);
    }
    [Conditional("DEBUG")] public static void WriteToDebug( this StringBuilder self, [CallerMemberName] string? caller = default ) => self.ToString().WriteToDebug(caller);
    [Conditional("DEBUG")]
    public static void WriteToDebug( this object self, [CallerMemberName] string? caller = default )
    {
        Debug.WriteLine(Header);
        Debug.WriteLine(caller);
        Debug.WriteLine(string.Empty);
        Debug.WriteLine(self);
        Debug.WriteLine(string.Empty);
    }

#else
    public static void WriteToConsole( this ReadOnlySpan<char> self, [CallerArgumentExpression("self")] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine(Header);
        Console.WriteLine($"{caller}.{variable}");
        Console.WriteLine();
        Console.WriteLine(self.ToString());
        Console.WriteLine();
    }
    public static void WriteToConsole( this string self, [CallerArgumentExpression("self")] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine(Header);
        Console.WriteLine($"{caller}.{variable}");
        Console.WriteLine();
        Console.WriteLine(self);
        Console.WriteLine();
    }
    public static void WriteToConsole( this StringBuilder self, [CallerArgumentExpression("self")] string? variable = default, [CallerMemberName] string? caller = default ) => self.ToString().WriteToConsole(variable, caller);
    public static void WriteToConsole( this object self, [CallerArgumentExpression("self")] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine(Header);
        Console.WriteLine($"{caller}.{variable}");
        Console.WriteLine();
        Console.WriteLine(self);
        Console.WriteLine();
    }


    [Conditional("DEBUG")]
    public static void WriteToDebug( this ReadOnlySpan<char> self, [CallerArgumentExpression("self")] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Debug.WriteLine(Header);
        Debug.WriteLine($"{caller}.{variable}");
        Debug.WriteLine(string.Empty);
        Debug.WriteLine(self.ToString());
        Debug.WriteLine(string.Empty);
    }
    [Conditional("DEBUG")]
    public static void WriteToDebug( this string self, [CallerArgumentExpression("self")] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Debug.WriteLine(Header);
        Debug.WriteLine($"{caller}.{variable}");
        Debug.WriteLine(string.Empty);
        Debug.WriteLine(self);
        Debug.WriteLine(string.Empty);
    }
    [Conditional("DEBUG")] public static void WriteToDebug( this StringBuilder self, [CallerArgumentExpression("self")] string? variable = default, [CallerMemberName] string? caller = default ) => self.ToString().WriteToDebug(variable);
    [Conditional("DEBUG")]
    public static void WriteToDebug( this object self, [CallerArgumentExpression("self")] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Debug.WriteLine(Header);
        Debug.WriteLine($"{caller}.{variable}");
        Debug.WriteLine(string.Empty);
        Debug.WriteLine(self);
        Debug.WriteLine(string.Empty);
    }

#endif
}
