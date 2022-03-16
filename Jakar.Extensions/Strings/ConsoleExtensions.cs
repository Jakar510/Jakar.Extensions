namespace Jakar.Extensions.Strings;


public static class ConsoleExtensions
{
    public static bool   CanDebug => Debugger.IsAttached;
    private const string INFORMATION = nameof(INFORMATION);
    private const string WARNING     = nameof(WARNING);
    private const string ERROR       = nameof(ERROR);

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
        string wrapper = c.Wrapper(length);

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
        string wrapper = c.Wrapper(length);

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
        string wrapper = c.Wrapper(length);

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


    /// <summary>
    /// Creates a new string of <paramref name="length"/> size filled with <paramref name="c"/> character
    /// </summary>
    /// <param name="c"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string Wrapper( this char c, int length ) => new(c, length);

    /// <summary>
    /// Wraps a string in <paramref name="c"></paramref> repeated <paramref name="padding"></paramref> times.
    /// </summary>
    /// <param name="self"></param>
    /// <param name="c"></param>
    /// <param name="padding"></param>
    /// <returns></returns>
    public static string Wrapper( this string self, in char c, in int padding ) => self.PadLeft(padding, c).PadRight(padding, c);


    public static string GetMessage( this  string         self, string start = INFORMATION, in char c      = '-', in int padding = 40 ) => $"{start.Wrapper(c, padding)}  {self}";
    public static string GetWarning( this  string         self, string start = WARNING,     in char c      = '-', in int padding = 40 ) => $"{start.Wrapper(c, padding)}  {self}";
    public static string GetError( this    string         self, string start = ERROR,       in char c      = '-', in int padding = 40 ) => $"{start.Wrapper(c, padding)}  {self}";
    public static string GetCount( this    string         self, int    count,               char    c      = '-', int    length  = 80 ) => $"{c.Wrapper(length)}   {self}.Count: => {count}";
    public static string GetCount( this    ICollection    self, char   c = '-',             int     length = 80 ) => $"{c.Wrapper(length)}   {self.GetType().Name}.Count: => {self.Count}";
    public static string GetCount<T>( this ICollection<T> self, char   c = '-',             int     length = 80 ) => $"{c.Wrapper(length)}   {self.GetType().Name}.Count: => {self.Count}";


    public static void WriteToConsole( this StringBuilder self ) => self.ToString().WriteToConsole();

    public static void WriteToConsole( this string self )
    {
        Console.WriteLine();
        Console.WriteLine(self);
        Console.WriteLine();
    }

    public static void WriteToConsole( this object self )
    {
        Console.WriteLine();
        Console.WriteLine(self);
        Console.WriteLine();
    }

    public static void WriteToConsole( this ReadOnlySpan<char> self )
    {
        Console.WriteLine();
        Console.WriteLine(self.ToString());
        Console.WriteLine();
    }


    [Conditional("DEBUG")]
    public static void WriteToDebug( this StringBuilder self ) => self.ToString().WriteToDebug();

    [Conditional("DEBUG")]
    public static void WriteToDebug( this string self )
    {
        Debug.WriteLine(string.Empty);
        Debug.WriteLine(self);
        Debug.WriteLine(string.Empty);
    }

    [Conditional("DEBUG")]
    public static void WriteToDebug( this object self )
    {
        Debug.WriteLine(string.Empty);
        Debug.WriteLine(self);
        Debug.WriteLine(string.Empty);
    }

    [Conditional("DEBUG")]
    public static void WriteToDebug( this ReadOnlySpan<char> self )
    {
        Debug.WriteLine(string.Empty);
        Debug.WriteLine(self.ToString());
        Debug.WriteLine(string.Empty);
    }
}
