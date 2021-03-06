namespace Jakar.Extensions.Strings;


/// <summary>
/// <para>
/// <see href="https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm"/>
/// </para>
/// </summary>
public readonly ref struct LineSplitEntry<T> where T : IEquatable<T>
{
    public LineSplitEntry( ReadOnlySpan<T> line, ReadOnlySpan<T> separator )
    {
        Value     = line;
        Separator = separator;
    }

    public ReadOnlySpan<T> Value     { get; }
    public ReadOnlySpan<T> Separator { get; }

    // This method allow to deconstruct the type, so you can write any of the following code
    // foreach (var entry in str.SplitLines()) { _ = entry.Line; }
    // foreach (var (line, endOfLine) in str.SplitLines()) { _ = line; }
    // https://docs.microsoft.com/en-us/dotnet/csharp/deconstruct?WT.mc_id=DT-MVP-5003978#deconstructing-user-defined-types
    public void Deconstruct( out ReadOnlySpan<T> line, out ReadOnlySpan<T> separator )
    {
        line      = Value;
        separator = Separator;
    }

    // This method allow to implicitly cast the type into a ReadOnlySpan<char>, so you can write the following code
    // foreach (ReadOnlySpan<char> entry in str.SplitLines())
    public static implicit operator ReadOnlySpan<T>( LineSplitEntry<T> entry ) => entry.Value;
}
