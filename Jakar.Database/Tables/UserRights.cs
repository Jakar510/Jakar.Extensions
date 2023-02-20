// Jakar.Extensions :: Jakar.Database
// 02/19/2023  7:25 PM

namespace Jakar.Database;


public abstract class UserRights : IEnumerator<(int Index, bool Value)>
{
    protected readonly char _valid;
    protected readonly char _invalid;


    protected          int          _index = 0;
    protected readonly Memory<char> _rights;


    public (int Index, bool Value) Current => (_index, Has( _index ));
    public int                     Length  => _rights.Length;
    object IEnumerator.            Current => Current;


    private UserRights( char valid, char invalid )
    {
        _invalid = invalid;
        _valid   = valid;
    }
    protected UserRights( string rights, char valid = '+', char invalid = '-' ) : this( valid, invalid )
    {
        Span<char> buffer = stackalloc char[rights.Length];
        rights.CopyTo( buffer );
        _rights = buffer.AsMemory();
    }
    protected UserRights( int length, char valid = '+', char invalid = '-' ) : this( valid, invalid )
    {
        Span<char> buffer = stackalloc char[length];
        _rights = buffer.AsMemory();
        Span<char> span = _rights.Span;

        for ( int i = 0; i < Length; i++ ) { span[i] = _invalid; }
    }
    public virtual void Dispose()
    {
        _rights.Span.Clear();
        GC.SuppressFinalize( this );
    }


    public bool MoveNext() => ++_index < Length;
    public void Reset() => _index = -1;


    public bool Has( int  index ) => _rights.Span[index] == _valid;
    public bool Has<T>( T index ) where T : struct, Enum => Has( index.AsInt() );


    public void Remove( int  index ) => Set( index, _invalid );
    public void Remove<T>( T index ) where T : struct, Enum => Remove( index.AsInt() );


    public void Add( int  index ) => Set( index, _valid );
    public void Add<T>( T index ) where T : struct, Enum => Add( index.AsInt() );


    protected void Set( int index, char right )
    {
        if ( right != _valid && right != _invalid ) { throw new ArgumentException( $"{nameof(right)} must be one of [ {_valid}, {_invalid} ]" ); }

        _rights.Span[index] = right;
    }


    public override string ToString() => _rights.ToString();
}
