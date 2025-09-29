namespace Jakar.Extensions;


public ref struct RangeEnumerator
{
    private readonly Range __range;
    public           int   Current { get; private set; }


    public RangeEnumerator( Range range )
    {
        __range  = range;
        Current = __range.Start.Value - 1;
    }


    public bool MoveNext() => ++Current <= __range.End.Value;
}



public static class RangeEnumeratorExtensions
{
    public static RangeEnumerator GetEnumerator( this Range range ) => new(range);
}
