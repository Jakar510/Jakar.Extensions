namespace Jakar.Extensions;


public ref struct RangeEnumerator
{
    private readonly Range _range;
    public           int   Current { get; private set; }


    public RangeEnumerator( Range range )
    {
        _range  = range;
        Current = _range.Start.Value - 1;
    }


    public bool MoveNext() => ++Current <= _range.End.Value;
}



public static class RangeEnumeratorExtensions
{
    public static RangeEnumerator GetEnumerator( this Range range ) => new(range);
}
