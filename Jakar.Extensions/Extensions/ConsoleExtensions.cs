#nullable enable
namespace Jakar.Extensions;


public static class ConsoleExtensions
{
    private const string INFORMATION = nameof(INFORMATION);
    private const string WARNING     = nameof(WARNING);
    private const string ERROR       = nameof(ERROR);
    public static bool   CanDebug => Debugger.IsAttached;
    public static string Header   { get; set; } = '='.Repeat( 100 );


    public static StringBuilder WrapException<T>( this T self, in char c = '-', in int padding = 40 ) where T : Exception
    {
        var builder = new StringBuilder();
        builder.AppendLine( " Exception Start ".Wrapper( c, padding ) );
        builder.AppendLine();
        builder.AppendLine( typeof(T).FullName );
        builder.AppendLine();
        builder.AppendLine();

        ExceptionDetails details = self.FullDetails();
        builder.AppendLine( details.ToPrettyJson() );

        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine( " Exception End ".Wrapper( c, padding ) );
        return builder;
    }

    public static StringBuilder PrintJson<T>( this T self, char c = '-', int length = 80 ) where T : notnull
    {
        string wrapper = c.Repeat( length );

        var builder = new StringBuilder();
        builder.AppendLine();
        builder.AppendLine( wrapper );
        builder.AppendLine( typeof(T).FullName );
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine( self.ToPrettyJson() );
        builder.AppendLine();
        builder.AppendLine();
        builder.AppendLine( wrapper );
        builder.AppendLine();
        return builder;
    }


    public static string GetMessage( this string self, string start = INFORMATION, in char c = '-', in int padding = 40 ) => $"{start.Wrapper( c, padding )}  {self}";
    public static string GetWarning( this string self, string start = WARNING,     in char c = '-', in int padding = 40 ) => $"{start.Wrapper( c, padding )}  {self}";
    public static string GetError( this   string self, string start = ERROR,       in char c = '-', in int padding = 40 ) => $"{start.Wrapper( c, padding )}  {self}";


    /* Unmerged change from project 'Jakar.Extensions (netstandard2.1)'
    Before:
        public static string GetCount( this    string         self, int    count,               char    c      = '-', int    length  = 80 ) => $"{c.Repeat(length)}   {self}.Count: => {count}";
    After:
        public static string GetCount(this string         self, int    count,               char    c      = '-', int    length  = 80 ) => $"{c.Repeat(length)}   {self}.Count: => {count}";
    */
    public static string GetCount( this    string         self, int  count,   char c      = '-', int length = 80 ) => $"{c.Repeat( length )}   {self}.Count: => {count}";
    public static string GetCount( this    ICollection    self, char c = '-', int  length = 80 ) => $"{c.Repeat( length )}   {self.GetType().Name}.Count: => {self.Count}";
    public static string GetCount<T>( this ICollection<T> self, char c = '-', int  length = 80 ) => $"{c.Repeat( length )}   {self.GetType().Name}.Count: => {self.Count}";


    public static void Print( this Span<char>         self ) => Console.Write( self.ToString() );
    public static void Print( this ReadOnlySpan<char> self ) => Console.Write( self.ToString() );
    public static void Print( this string             self ) => Console.Write( self );
    public static void Print( this ValueStringBuilder self ) => self.ToString()
                                                                    .Print();
    public static void Print( this StringBuilder self ) => self.ToString()
                                                               .Print();
    public static void Print( this object self ) => Console.Write( self );


    public static void PrintLine( this Span<char>         self ) => Console.WriteLine( self.ToString() );
    public static void PrintLine( this ReadOnlySpan<char> self ) => Console.WriteLine( self.ToString() );
    public static void PrintLine( this string             self ) => Console.WriteLine( self );
    public static void PrintLine( this ValueStringBuilder self ) => self.ToString()
                                                                        .PrintLine();
    public static void PrintLine( this StringBuilder self ) => self.ToString()
                                                                   .PrintLine();
    public static void PrintLine( this object self ) => Console.WriteLine( self );


    public static void WriteToConsole( this Span<char> self ) => self.ToString()
                                                                     .WriteToConsole();
    public static void WriteToConsole( this ReadOnlySpan<char> self ) => self.ToString()
                                                                             .WriteToConsole();
    public static void WriteToConsole( this ValueStringBuilder self ) => self.Span.WriteToConsole();
    public static void WriteToConsole( this Buffer<char>       self ) => self.Span.WriteToConsole();
    public static void WriteToConsole( this StringBuilder self ) => self.ToString()
                                                                        .WriteToConsole();
    public static void WriteToConsole<T>( this T self ) where T : notnull => self.ToString()
                                                                                ?.WriteToConsole();
    public static void WriteToConsole( this string self )
    {
        Console.WriteLine();
        Console.WriteLine( self );
        Console.WriteLine();
    }


#if NETSTANDARD2_1
    [Conditional( "DEBUG" )]
    public static void WriteToDebug( this ReadOnlySpan<char> self, [CallerArgumentExpression( "self" )] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine( $"{caller} -> {variable} '{self.ToString()}'" );
        Debug.WriteLine( $"{caller} -> {variable} '{self.ToString()}'" );
    }


    [Conditional( "DEBUG" )]
    public static void WriteToDebug( this string self, [CallerArgumentExpression( "self" )] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine( $"{caller} -> {variable} '{self}'" );
        Debug.WriteLine( $"{caller} -> {variable} '{self}'" );
    }


    [Conditional( "DEBUG" )]
    public static void WriteToDebug( this StringBuilder self, [CallerArgumentExpression( "self" )] string? variable = default, [CallerMemberName] string? caller = default ) => self.ToString()
                                                                                                                                                                                    .WriteToDebug( variable, caller );


    [Conditional( "DEBUG" )]
    public static void WriteToDebug( this Buffer<char> self, [CallerArgumentExpression( "self" )] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine( $"{caller} -> {variable} '{self.Span.ToString()}'" );
        Debug.WriteLine( $"{caller} -> {variable} '{self.Span.ToString()}'" );
    }


    [Conditional( "DEBUG" )]
    public static void WriteToDebug( this ValueStringBuilder self, [CallerArgumentExpression( "self" )] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine( $"{caller} -> {variable} '{self.Span.ToString()}'" );
        Debug.WriteLine( $"{caller} -> {variable} '{self.Span.ToString()}'" );
    }


    [Conditional( "DEBUG" )]
    public static void WriteToDebug( this object self, [CallerArgumentExpression( "self" )] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine( $"{caller} -> {variable} '{self}'" );
        Debug.WriteLine( $"{caller} -> {variable} '{self}'" );
    }


    [Conditional( "DEBUG" )]
    public static void WriteToDebug<T>( this T self, [CallerArgumentExpression( "self" )] string? variable = default, [CallerMemberName] string? caller = default ) where T : struct
    {
        Console.WriteLine( $"{caller} -> {variable} '{self}'" );
        Debug.WriteLine( $"{caller} -> {variable} '{self}'" );
    }


#else
    [Conditional( "DEBUG" )]
    public static void WriteToDebug( this ReadOnlySpan<char> self, [CallerArgumentExpression( "self" )] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine( $"{caller} -> {variable} '{self}'" );
        Debug.WriteLine( $"{caller} -> {variable} '{self}'" );
    }


    [Conditional( "DEBUG" )]
    public static void WriteToDebug( this string self, [CallerArgumentExpression( "self" )] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine( $"{caller} -> {variable} '{self}'" );
        Debug.WriteLine( $"{caller} -> {variable} '{self}'" );
    }


    [Conditional( "DEBUG" )]
    public static void WriteToDebug( this StringBuilder self, [CallerArgumentExpression( "self" )] string? variable = default, [CallerMemberName] string? caller = default ) => self.ToString()
                                                                                                                                                                                    .WriteToDebug( variable, caller );


    [Conditional( "DEBUG" )]
    public static void WriteToDebug( this Buffer<char> self, [CallerArgumentExpression( "self" )] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine( $"{caller} -> {variable} '{self.Span}'" );
        Debug.WriteLine( $"{caller} -> {variable} '{self.Span}'" );
    }


    [Conditional( "DEBUG" )]
    public static void WriteToDebug( this ValueStringBuilder self, [CallerArgumentExpression( "self" )] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine( $"{caller} -> {variable} '{self.Span}'" );
        Debug.WriteLine( $"{caller} -> {variable} '{self.Span}'" );
    }


    [Conditional( "DEBUG" )]
    public static void WriteToDebug( this object self, [CallerArgumentExpression( "self" )] string? variable = default, [CallerMemberName] string? caller = default )
    {
        Console.WriteLine( $"{caller} -> {variable} '{self}'" );
        Debug.WriteLine( $"{caller} -> {variable} '{self}'" );
    }


    [Conditional( "DEBUG" )]
    public static void WriteToDebug<T>( this T self, [CallerArgumentExpression( "self" )] string? variable = default, [CallerMemberName] string? caller = default ) where T : struct
    {
        Console.WriteLine( $"{caller} -> {variable} '{self}'" );
        Debug.WriteLine( $"{caller} -> {variable} '{self}'" );
    }

#endif
}
