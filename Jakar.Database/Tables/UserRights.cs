// Jakar.Extensions :: Jakar.Database
// 02/19/2023  7:25 PM

namespace Jakar.Database;


public abstract class UserRights : IEnumerator<(int Index, bool Value)>
{
    protected int          _index = 0;
    private   Memory<char> _rights;


    public (int Index, bool Value) Current => (_index, Has( _index ));
    public int                     Length  => _rights.Length;
    object IEnumerator.            Current => Current;
    public Span<char>              Span    => _rights.Span;
    public char                    Valid   { get; init; } = '+';
    public char                    Invalid { get; init; } = '-';


    public void Init( string rights )
    {
        Span<char> buffer = stackalloc char[rights.Length];
        rights.CopyTo( buffer );
        _rights = buffer.AsMemory();
    }
    public void Init( int length )
    {
        Span<char> buffer = stackalloc char[length];
        _rights = buffer.AsMemory();

        for ( int i = 0; i < Length; i++ ) { Span[i] = Invalid; }
    }


    public virtual void Dispose()
    {
        Span.Clear();
        GC.SuppressFinalize( this );
    }


    public bool MoveNext() => ++_index < Length;
    public void Reset() => _index = -1;


    public bool Has( int  index ) => Span[index] == Valid;
    public bool Has<T>( T index ) where T : struct, Enum => Has( index.AsInt() );


    public void Remove( int  index ) => Set( index, Invalid );
    public void Remove<T>( T index ) where T : struct, Enum => Remove( index.AsInt() );


    public void Add( int  index ) => Set( index, Valid );
    public void Add<T>( T index ) where T : struct, Enum => Add( index.AsInt() );


    protected void Set( int index, char right )
    {
        if ( right != Valid && right != Invalid ) { throw new ArgumentException( $"{nameof(right)} must be one of [ {Valid}, {Invalid} ]" ); }

        Span[index] = right;
    }


    public override string ToString() => _rights.ToString();
}
