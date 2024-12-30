// Jakar.Extensions :: Jakar.Database
// 08/21/2022  9:25 AM

namespace Jakar.Database;


public record struct Counter : IEnumerator<long>
{
    public static Counter Instance { get; } = new();
    public        long    Current  { get; private set; }
    object IEnumerator.   Current  => Current;


    public Counter() { }
    public void Dispose() => Current = 0;


    public bool MoveNext() => (++Current).IsValidID();
    public void Reset()    => Current = 0;
}
