﻿namespace Jakar.Extensions;


/// <summary>
///     <para>
///         <see href="https://www.meziantou.net/split-a-string-into-lines-without-allocation.htm"/>
///     </para>
/// </summary>
[SuppressMessage( "ReSharper", "OutParameterValueIsAlwaysDiscarded.Global" )]
public readonly ref struct LineSplitEntry<T>( ReadOnlySpan<T> line, ParamsArray<T> separator )
    where T : unmanaged, IEquatable<T>
{
    public ReadOnlySpan<T> Value      { get; } = line;
    public ParamsArray<T>  Separator  { get; } = separator;
    public int             Length     => Value.Length;
    public bool            IsEmpty    => Value.IsEmpty;
    public bool            IsNotEmpty => !IsEmpty;


    // This method allow to deconstruct the type, so you can write any of the following code
    // foreach (var entry in str.SplitLines()) { _ = entry.Line; }
    // foreach (var (line, endOfLine) in str.SplitLines()) { _ = line; }
    // https://docs.microsoft.com/en-us/dotnet/csharp/deconstruct?WT.mc_id=DT-MVP-5003978#deconstructing-user-defined-types
    public void Deconstruct( out ReadOnlySpan<T> line, out ParamsArray<T> separator )
    {
        line      = Value;
        separator = Separator;
    }

    // This method allow to implicitly cast the type into a ReadOnlySpan<char>, so you can write the following code
    // foreach (ReadOnlySpan<char> entry in str.SplitLines())
    public static implicit operator ReadOnlySpan<T>( LineSplitEntry<T> entry ) => entry.Value;


    public override string ToString() => $"{nameof(LineSplitEntry<T>)}<{nameof(Value)}: {Value.ToString()}, {nameof(Separator)}: {Separator.ToString()}>";
    private bool TryFormat( Span<T> destination, out int charsWritten, ReadOnlySpan<T> format, IFormatProvider? provider )
    {
        if ( Value.TryCopyTo( destination ) )
        {
            charsWritten = Length;
            return true;
        }

        charsWritten = 0;
        return false;
    }
}
