/*
// Jakar.Extensions :: Jakar.Extensions
// 08/28/2023  9:54 PM

namespace Jakar.Extensions;


public ref struct LinqSpan<T>
{
    private readonly Span<T>       _span;
    private          Span<Handler> _handlers = default;


    public readonly int Length => _span.Length;


    public LinqSpan( Span<T> span ) => _span = span;


    public readonly Enumerator GetEnumerator() => new(this);



    public readonly record struct Handler( Func<T, bool> Check ) { }



    public ref struct Enumerator
    {
        private readonly LinqSpan<T> _linq;
        private          int         _index;

        public readonly ref T Current
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )] get => ref _linq._span[_index];
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        internal Enumerator( LinqSpan<T> linq )
        {
            _linq  = linq;
            _index = -1;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool MoveNext()
        {
            int index = _index + 1;
            if ( index >= _linq.Length ) { return false; }

            _index = index;
            return true;
        }
    }
}
*/



