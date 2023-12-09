// Jakar.Extensions :: Jakar.AppLogger.Common
// 08/24/2023  7:31 PM

namespace Jakar.AppLogger.Common;


public interface ILogFormatter
{
    public string HeaderTemplate { get; }
    public string Separator      { get; }


    public string Format( string template, IParameters parameters );
}



public interface IParameters : IFormattable
{
    public void Handle( ref ValueStringBuilder builder, ILogFormatter formatter );
}



public readonly record struct Parameters( params string[] Values ) : IParameters
{
    public                 int        Count                              => Values.Length;
    [ Pure ] public static Parameters Create( params string[]     args ) => new(args);
    [ Pure ] public static Parameters Create( IEnumerable<string> args ) => new(args.ToArray());


    public string ToString( string? format, IFormatProvider? formatProvider ) => string.Join( format, Values );
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendJoin( format, Values.AsSpan() );
        return builder.TryCopyTo( destination, out charsWritten );
    }
    public void Handle( ref ValueStringBuilder builder, ILogFormatter formatter )
    {
        ReadOnlySpan<char> separator = formatter.Separator;
    }
}



public readonly record struct Parameters<T>( params T[] Values ) : IParameters
    where T : ISpanFormattable
{
    public                 int           Count                         => Values.Length;
    [ Pure ] public static Parameters<T> Create( params T[]     args ) => new(args);
    [ Pure ] public static Parameters<T> Create( IEnumerable<T> args ) => new(args.ToArray());


    public string ToString( string? format, IFormatProvider? formatProvider ) => string.Join( format, Values );
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendJoin( format, Values );
        return builder.TryCopyTo( destination, out charsWritten );
    }
    public void Handle( ref ValueStringBuilder builder, ILogFormatter formatter )
    {
        ReadOnlySpan<char> separator = formatter.Separator;
    }
}



public readonly record struct Parameters<T1, T2>( T1 One, T2 Two ) : IParameters
    where T1 : ISpanFormattable
    where T2 : ISpanFormattable
{
    public                 int                Count                    => 2;
    [ Pure ] public static Parameters<T1, T2> Create( T1 one, T2 two ) => new(one, two);


    public string ToString( string? format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        return builder.ToString();
    }
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        return builder.TryCopyTo( destination, out charsWritten );
    }
    public void Handle( ref ValueStringBuilder builder, ILogFormatter formatter )
    {
        ReadOnlySpan<char> separator = formatter.Separator;
    }
}



public readonly record struct Parameters<T1, T2, T3>( T1 One, T2 Two, T3 Three ) : IParameters
    where T1 : ISpanFormattable
    where T2 : ISpanFormattable
    where T3 : ISpanFormattable
{
    public                 int                    Count                              => 3;
    [ Pure ] public static Parameters<T1, T2, T3> Create( T1 one, T2 two, T3 three ) => new(one, two, three);


    public string ToString( string? format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        return builder.ToString();
    }
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        return builder.TryCopyTo( destination, out charsWritten );
    }
    public void Handle( ref ValueStringBuilder builder, ILogFormatter formatter )
    {
        ReadOnlySpan<char> separator = formatter.Separator;
    }
}



public readonly record struct Parameters<T1, T2, T3, T4>( T1 One, T2 Two, T3 Three, T4 Four ) : IParameters
    where T1 : ISpanFormattable
    where T2 : ISpanFormattable
    where T3 : ISpanFormattable
    where T4 : ISpanFormattable
{
    public                 int                        Count                                       => 4;
    [ Pure ] public static Parameters<T1, T2, T3, T4> Create( T1 one, T2 two, T3 three, T4 four ) => new(one, two, three, four);


    public string ToString( string? format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Four, default, provider );
        return builder.ToString();
    }
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Four, default, provider );
        return builder.TryCopyTo( destination, out charsWritten );
    }
    public void Handle( ref ValueStringBuilder builder, ILogFormatter formatter )
    {
        ReadOnlySpan<char> separator = formatter.Separator;
    }
}



public readonly record struct Parameters<T1, T2, T3, T4, T5>( T1 One, T2 Two, T3 Three, T4 Four, T5 Five ) : IParameters
    where T1 : ISpanFormattable
    where T2 : ISpanFormattable
    where T3 : ISpanFormattable
    where T4 : ISpanFormattable
    where T5 : ISpanFormattable
{
    public                 int                            Count                                                => 5;
    [ Pure ] public static Parameters<T1, T2, T3, T4, T5> Create( T1 one, T2 two, T3 three, T4 four, T5 five ) => new(one, two, three, four, five);


    public string ToString( string? format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Four, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Five, default, provider );
        return builder.ToString();
    }
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Four, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Five, default, provider );
        return builder.TryCopyTo( destination, out charsWritten );
    }
    public void Handle( ref ValueStringBuilder builder, ILogFormatter formatter )
    {
        ReadOnlySpan<char> separator = formatter.Separator;
    }
}



public readonly record struct Parameters<T1, T2, T3, T4, T5, T6>( T1 One, T2 Two, T3 Three, T4 Four, T5 Five, T6 Six ) : IParameters
    where T1 : ISpanFormattable
    where T2 : ISpanFormattable
    where T3 : ISpanFormattable
    where T4 : ISpanFormattable
    where T5 : ISpanFormattable
    where T6 : ISpanFormattable
{
    public                 int                                Count                                                        => 5;
    [ Pure ] public static Parameters<T1, T2, T3, T4, T5, T6> Create( T1 one, T2 two, T3 three, T4 four, T5 five, T6 six ) => new(one, two, three, four, five, six);


    public string ToString( string? format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Four, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Five, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Six, default, provider );
        return builder.ToString();
    }
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Four, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Five, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Six, default, provider );
        return builder.TryCopyTo( destination, out charsWritten );
    }
    public void Handle( ref ValueStringBuilder builder, ILogFormatter formatter )
    {
        ReadOnlySpan<char> separator = formatter.Separator;
    }
}



public readonly record struct Parameters<T1, T2, T3, T4, T5, T6, T7>( T1 One, T2 Two, T3 Three, T4 Four, T5 Five, T6 Six, T7 Seven ) : IParameters
    where T1 : ISpanFormattable
    where T2 : ISpanFormattable
    where T3 : ISpanFormattable
    where T4 : ISpanFormattable
    where T5 : ISpanFormattable
    where T6 : ISpanFormattable
    where T7 : ISpanFormattable
{
    public                 int                                    Count                                                                  => 5;
    [ Pure ] public static Parameters<T1, T2, T3, T4, T5, T6, T7> Create( T1 one, T2 two, T3 three, T4 four, T5 five, T6 six, T7 seven ) => new(one, two, three, four, five, six, seven);


    public string ToString( string? format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Four, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Five, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Six, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Seven, default, provider );
        return builder.ToString();
    }
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Four, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Five, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Six, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Seven, default, provider );
        return builder.TryCopyTo( destination, out charsWritten );
    }
    public void Handle( ref ValueStringBuilder builder, ILogFormatter formatter )
    {
        ReadOnlySpan<char> separator = formatter.Separator;
    }
}



public readonly record struct Parameters<T1, T2, T3, T4, T5, T6, T7, T8>( T1 One, T2 Two, T3 Three, T4 Four, T5 Five, T6 Six, T7 Seven, T8 Eight ) : IParameters
    where T1 : ISpanFormattable
    where T2 : ISpanFormattable
    where T3 : ISpanFormattable
    where T4 : ISpanFormattable
    where T5 : ISpanFormattable
    where T6 : ISpanFormattable
    where T7 : ISpanFormattable
    where T8 : ISpanFormattable
{
    public                 int                                        Count                                                                            => 5;
    [ Pure ] public static Parameters<T1, T2, T3, T4, T5, T6, T7, T8> Create( T1 one, T2 two, T3 three, T4 four, T5 five, T6 six, T7 seven, T8 eight ) => new(one, two, three, four, five, six, seven, eight);


    public string ToString( string? format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Four, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Five, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Six, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Seven, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Eight, default, provider );
        return builder.ToString();
    }
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Four, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Five, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Six, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Seven, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Eight, default, provider );
        return builder.TryCopyTo( destination, out charsWritten );
    }
    public void Handle( ref ValueStringBuilder builder, ILogFormatter formatter )
    {
        ReadOnlySpan<char> separator = formatter.Separator;
    }
}



public readonly record struct Parameters<T1, T2, T3, T4, T5, T6, T7, T8, T9>( T1 One, T2 Two, T3 Three, T4 Four, T5 Five, T6 Six, T7 Seven, T8 Eight, T9 Nine ) : IParameters
    where T1 : ISpanFormattable
    where T2 : ISpanFormattable
    where T3 : ISpanFormattable
    where T4 : ISpanFormattable
    where T5 : ISpanFormattable
    where T6 : ISpanFormattable
    where T7 : ISpanFormattable
    where T8 : ISpanFormattable
    where T9 : ISpanFormattable
{
    public                 int                                            Count                                                                                     => 5;
    [ Pure ] public static Parameters<T1, T2, T3, T4, T5, T6, T7, T8, T9> Create( T1 one, T2 two, T3 three, T4 four, T5 five, T6 six, T7 seven, T8 eight, T9 nine ) => new(one, two, three, four, five, six, seven, eight, nine);


    public string ToString( string? format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Four, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Five, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Six, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Seven, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Eight, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Nine, default, provider );
        return builder.ToString();
    }
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Four, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Five, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Six, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Seven, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Eight, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Nine, default, provider );
        return builder.TryCopyTo( destination, out charsWritten );
    }
    public void Handle( ref ValueStringBuilder builder, ILogFormatter formatter )
    {
        ReadOnlySpan<char> separator = formatter.Separator;
    }
}



public readonly record struct Parameters<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>( T1 One, T2 Two, T3 Three, T4 Four, T5 Five, T6 Six, T7 Seven, T8 Eight, T9 Nine, T10 Ten ) : IParameters
    where T1 : ISpanFormattable
    where T2 : ISpanFormattable
    where T3 : ISpanFormattable
    where T4 : ISpanFormattable
    where T5 : ISpanFormattable
    where T6 : ISpanFormattable
    where T7 : ISpanFormattable
    where T8 : ISpanFormattable
    where T9 : ISpanFormattable
    where T10 : ISpanFormattable
{
    public                 int                                                 Count                                                                                              => 5;
    [ Pure ] public static Parameters<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Create( T1 one, T2 two, T3 three, T4 four, T5 five, T6 six, T7 seven, T8 eight, T9 nine, T10 ten ) => new(one, two, three, four, five, six, seven, eight, nine, ten);


    public string ToString( string? format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Four, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Five, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Six, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Seven, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Eight, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Nine, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Ten, default, provider );
        return builder.ToString();
    }
    public bool TryFormat( Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider )
    {
        using var builder = new ValueStringBuilder( 1000 );
        builder.AppendSpanFormattable( One, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Two, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Three, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Four, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Five, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Six, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Seven, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Eight, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Nine, default, provider );
        builder.Append( format );
        builder.AppendSpanFormattable( Ten, default, provider );
        return builder.TryCopyTo( destination, out charsWritten );
    }
    public void Handle( ref ValueStringBuilder builder, ILogFormatter formatter )
    {
        ReadOnlySpan<char> separator = formatter.Separator;
    }
}
