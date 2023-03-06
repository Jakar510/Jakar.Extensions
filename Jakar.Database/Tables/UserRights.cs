// Jakar.Extensions :: Jakar.Database
// 02/19/2023  7:25 PM

using System.Buffers;
using System.Linq.Expressions;
using static System.Net.WebRequestMethods;



namespace Jakar.Database;


public struct UserRights : IEnumerable<(int Index, bool Value)>, IDisposable
{
    public interface IRights
    {
        [MaxLength( TokenValidationParameters.DefaultMaximumTokenSizeInBytes )] public string Rights { get; set; }
    }



    private static readonly MemoryPool<byte>   _pool   = MemoryPool<byte>.Shared;
    public const            byte               VALID   = 1;
    public const            byte               INVALID = 0;
    private readonly        Memory<byte>       _rights;
    private readonly        IMemoryOwner<byte> _owner;
    private                 int                _index = 0;


    public   int                     Length  => _rights.Length;
    public   (int Index, bool Value) Current => (_index, Has( _index ));
    internal Span<byte>              Span    => _rights.Span;


    private UserRights( int length )
    {
        _owner  = _pool.Rent( length );
        _rights = _owner.Memory;

        for ( int i = 0; i < Length; i++ ) { Span[i] = INVALID; }
    }
    public UserRights( IRights rights ) : this( rights.Rights ) { }
    private UserRights( string rights )
    {
        Span<byte> span = stackalloc byte[rights.Length];
        Convert.TryFromBase64String( rights, span, out int length );
        span = span[..length];

        _owner  = _pool.Rent( span.Length );
        _rights = _owner.Memory;
        span.CopyTo( Span );
    }
    public void Dispose() => _owner.Dispose();


    public UserRights With( IRights rights )
    {
        using var other = new UserRights( rights );
        Debug.Assert( Length == other.Length );
        for ( int i = 0; i < Length; i++ ) { Span[i] |= other.Span[i]; }

        return this;
    }
    public static UserRights Merge( List<IRights> values )
    {
        UserRights rights = default;

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach ( IRights value in values ) { rights = rights.With( value ); }

        return rights;
    }


    public bool MoveNext() => ++_index < Length;
    public void Reset() => _index = -1;


    public bool Has( int  index ) => Span[index] == VALID;
    public bool Has<T>( T index ) where T : struct, Enum => Has( Convert.ToInt32( index ) );


    public void Remove( int  index ) => Set( index, INVALID );
    public void Remove<T>( T index ) where T : struct, Enum => Remove( index.AsInt() );


    public void Add( int  index ) => Set( index, VALID );
    public void Add<T>( T index ) where T : struct, Enum => Add( index.AsInt() );


    private void Set( int index, byte value )
    {
        Debug.Assert( index < Length );
        Debug.Assert( value is VALID or INVALID );
        Span[index] = value;
    }


    public override string ToString() => Convert.ToBase64String( Span );
    public IEnumerator<(int Index, bool Value)> GetEnumerator() { yield break; }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
