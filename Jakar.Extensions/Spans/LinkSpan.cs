// Jakar.Extensions :: Jakar.Extensions
// 03/20/2025  14:03

namespace Jakar.Extensions;


public readonly ref struct LinkSpan<T>
    where T : IEquatable<T>
{
    private readonly IMemoryOwner<T>? _owner;
    public readonly  Span<T>          Span;
    public readonly  ReadOnlySpan<T>  ReadOnlySpan;
    public readonly  int              Length;


    public bool IsEmpty => Length == 0;
    public ref T this[ int             index ] { get => ref Span[index]; }
    public ref T this[ Index           index ] { get => ref Span[index]; }
    public ReadOnlySpan<T> this[ Range index ] { get => ReadOnlySpan[index]; }


    public LinkSpan( params ReadOnlySpan<T> span ) : this( span.Length ) => span.CopyTo( Span );
    public LinkSpan( int capacity )
    {
        Length       = capacity;
        _owner       = MemoryPool<T>.Shared.Rent( capacity );
        Span         = _owner.Memory.Span[..capacity];
        ReadOnlySpan = _owner.Memory.Span[..capacity];
    }


    [Pure, MustDisposeResource] public static LinkSpan<T>                Create( params ReadOnlySpan<T> span ) => new(span);
    public                                    void                       Dispose()                             => _owner?.Dispose();
    public                                    ReadOnlySpan<T>.Enumerator GetEnumerator()                       => ReadOnlySpan.GetEnumerator();
    public T[] ToArray()
    {
        T[] span = ReadOnlySpan.ToArray();
        Dispose();
        return span;
    }


    [Pure, MustDisposeResource]
    public LinkSpan<T> Where( Func<T, bool> selector )
    {
        if ( IsEmpty ) { return default; }

        LinkSpan<T> buffer = new(Length);
        Span<T>     span   = buffer.Span;
        int         index  = 0;

        foreach ( T value in ReadOnlySpan )
        {
            if ( selector( value ) ) { span[index++] = value; }
        }

        return buffer;
    }

    [Pure, MustDisposeResource]
    public LinkSpan<TNext> Where<TNext>( Func<T, bool> selector, Func<T, TNext> func )
        where TNext : IEquatable<TNext>
    {
        if ( IsEmpty ) { return default; }

        LinkSpan<TNext> buffer = new(Length);
        Span<TNext>     span   = buffer.Span;
        int             index  = 0;

        foreach ( T value in ReadOnlySpan )
        {
            if ( selector( value ) ) { span[index++] = func( value ); }
        }

        return buffer;
    }

    [Pure, MustDisposeResource]
    public LinkSpan<TNext> Select<TNext>( Func<T, TNext> func )
        where TNext : IEquatable<TNext>
    {
        if ( IsEmpty ) { return default; }

        LinkSpan<TNext> buffer = new(Length);
        Span<TNext>     span   = buffer.Span;
        int             index  = 0;

        foreach ( T value in ReadOnlySpan ) { span[index++] = func( value ); }

        return buffer;
    }

    [Pure, MustDisposeResource]
    public static LinkSpan<T> Join( scoped ref readonly ReadOnlySpan<T> first, scoped ref readonly ReadOnlySpan<T> last )
    {
        int         size   = first.Length;
        int         length = size + last.Length;
        LinkSpan<T> buffer = new(length);
        Guard.IsGreaterThanOrEqualTo( buffer.Length, length );
        first.CopyTo( buffer.Span[..size] );
        last.CopyTo( buffer.Span[size..] );
        return buffer;
    }

    [Pure, MustDisposeResource]
    public LinkSpan<T> Replace( scoped ref readonly ReadOnlySpan<T> oldValue, scoped ref readonly ReadOnlySpan<T> newValue )
    {
        LinkSpan<T> buffer = new(Length + ReadOnlySpan.Count( oldValue ) * newValue.Length);
        Span<T>     span   = buffer.Span;

        if ( ReadOnlySpan.ContainsExact( oldValue ) is false )
        {
            ReadOnlySpan.CopyTo( span );
            return buffer;
        }

        do
        {
            int startIndex = ReadOnlySpan.IndexOf( oldValue );

            if ( startIndex <= 0 ) { ReadOnlySpan[startIndex..].CopyTo( span[startIndex..] ); }
            else
            {
                ReadOnlySpan[..startIndex].CopyTo( span[..startIndex] );
                newValue.CopyTo( span[startIndex..] );
            }
        }
        while ( ReadOnlySpan.ContainsExact( oldValue ) );

        return buffer;
    }

    [Pure, MustDisposeResource]
    public LinkSpan<T> Remove( scoped ref readonly T value )
    {
        using IMemoryOwner<T> owner = MemoryPool<T>.Shared.Rent( Length );
        Span<T>               span  = owner.Memory.Span;
        int                   index = 0;

        foreach ( T equatable in ReadOnlySpan )
        {
            if ( equatable.Equals( value ) is false ) { span[index++] = equatable; }
        }

        return new LinkSpan<T>( span[..index] );
    }

    [Pure, MustDisposeResource]
    public LinkSpan<T> Remove( params ReadOnlySpan<T> values )
    {
        using IMemoryOwner<T> owner = MemoryPool<T>.Shared.Rent( Length );
        Span<T>               span  = owner.Memory.Span;
        int                   index = 0;

        foreach ( T equatable in ReadOnlySpan )
        {
            if ( equatable.IsOneOf( values ) is false ) { span[index++] = equatable; }
        }

        return new LinkSpan<T>( span[..index] );
    }
}
