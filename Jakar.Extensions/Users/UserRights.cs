// Jakar.Extensions :: Jakar.Database
// 02/19/2023  7:25 PM

using ZLinq;



namespace Jakar.Extensions;


public interface IUserRights<out TValue, TEnum> : IUserRights
    where TEnum : struct, Enum
    where TValue : IUserRights<TValue, TEnum>
{
    public TValue WithRights( UserRights<TEnum> rights );
}



public static class RightsExtensions
{
    public static UserRights<TEnum> GetRights<TEnum>( this IUserRights rights )
        where TEnum : struct, Enum => UserRights<TEnum>.Create(rights);


    public static void SetRights<TEnum>( this IUserRights user, params ReadOnlySpan<TEnum> indexes )
        where TEnum : struct, Enum => user.SetRights(user.GetRights<TEnum>()
                                                         .Add(indexes));


    public static void SetRights<TEnum>( this IUserRights user, scoped in UserRights<TEnum> value )
        where TEnum : struct, Enum => user.Rights = value.ToString();
}



public interface IUserRights
{
    [StringLength(RIGHTS)] public string Rights { get; set; }

    static void Main()
    {
        var rights = UserRights<FileRight>.Default.Add(FileRight.Read)
                                          .Add(FileRight.Delete);

        foreach ( UserRights<FileRight>.Right r in rights.Permissions ) { Console.WriteLine($"{r.Index}: {( r.Value ? "✔" : "✖" )}"); }

        Console.WriteLine();
        Console.WriteLine(rights.ToString());
        Console.WriteLine();

        // Output:
        // Read: ✔
        // Write: ✖
        // Execute: ✖
        // Delete: ✔

        rights.Dispose();
    }
}



internal enum FileRight : ulong
{
    Read    = 0,
    Write   = 1,
    Execute = 2,
    Delete  = 63
}



[DefaultMember(nameof(Default))]
public ref struct UserRights<TEnum>
    where TEnum : unmanaged, Enum
{
    public const            char                  VALID          = '+';
    public const            char                  INVALID        = '-';
    private const           int                   BITS_PER_BLOCK = 64;
    private readonly        IMemoryOwner<ulong>   __bits;
    private                 Span<ulong>           _bits;
    private static readonly int                   __blocks    = GetBlocks();
    private static readonly ImmutableArray<TEnum> _enumValues = [.. Enum.GetValues<TEnum>()];
    public static           ReadOnlySpan<TEnum>   EnumValues => _enumValues.AsSpan();
    public static           UserRights<TEnum>     Default    { [MustDisposeResource] get => new(); }
    public static           int                   Length     => _enumValues.Length;


    static UserRights()
    {
        if ( Enum.GetUnderlyingType(typeof(TEnum)) != typeof(ulong) ) { throw new InvalidOperationException($"{typeof(TEnum).Name} must have an underlying type of ulong."); }
    }
    [MustDisposeResource] public UserRights()
    {
        __bits = MemoryPool<ulong>.Shared.Rent(__blocks);
        _bits  = __bits.Memory.Span[..__blocks];
        _bits.Clear();
    }

    public void Dispose()
    {
        __bits?.Dispose();
        _bits = Span<ulong>.Empty;
        this  = default;
    }


    private static int GetBlocks()
    {
        ulong maxIndex = 0;

        foreach ( ref readonly TEnum v in EnumValues )
        {
            ulong val = Unsafe.As<TEnum, ulong>(ref Unsafe.AsRef(in v));
            if ( val > maxIndex ) { maxIndex = val; }
        }

        return (int)( maxIndex / BITS_PER_BLOCK ) + 1;
    }
    public readonly override string ToString()
    {
        Span<char> chars = stackalloc char[Length];

        for ( int i = 0; i < Length; i++ )
        {
            chars[i] = Has(EnumValues[i])
                           ? VALID
                           : INVALID;
        }

        return new string(chars);
    }


    [MustDisposeResource] public static UserRights<TEnum> Create( params ReadOnlySpan<char> values )
    {
        UserRights<TEnum> rights = Default;

        for ( int i = 0; i < values.Length && i < Length; i++ )
        {
            char v = values[i];

            switch ( v )
            {
                case VALID:
                    rights.Add(EnumValues[i]);
                    break;

                case INVALID:
                    rights.Remove(EnumValues[i]);
                    break;

                default:
                    throw new FormatException($"Invalid character '{v}' at position {i}. Expected '{VALID}' or '{INVALID}'.");
            }
        }

        return rights;
    }
    [MustDisposeResource] public static UserRights<TEnum> Create( IUserRights value ) => Create(value.Rights);
    [MustDisposeResource] public static UserRights<TEnum> Create( IEnumerable<IUserRights> values )
    {
        UserRights<TEnum> rights = Default;

        foreach ( IUserRights user in values )
        {
            using UserRights<TEnum> userRights = Create(user);
            rights.Or(userRights);
        }

        return rights;
    }
    [MustDisposeResource] public static UserRights<TEnum> Create( IEnumerable<IEnumerable<IUserRights>> values )
    {
        UserRights<TEnum> rights = Default;

        foreach ( var users in values )
        {
            using UserRights<TEnum> usersRights = Create(users);
            rights.Or(usersRights);
        }

        return rights;
    }


    // ---- Add / Remove ----
    public readonly UserRights<TEnum> Add( TEnum    right ) => Set(right, true);
    public readonly UserRights<TEnum> Remove( TEnum right ) => Set(right, false);
    public readonly UserRights<TEnum> Add( params ReadOnlySpan<TEnum> rights )
    {
        foreach ( TEnum right in rights ) { Add(right); }

        return this;
    }
    public readonly UserRights<TEnum> Remove( params ReadOnlySpan<TEnum> rights )
    {
        foreach ( TEnum right in rights ) { Remove(right); }

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] private readonly UserRights<TEnum> Set( TEnum right, bool value )
    {
        ulong index = Unsafe.As<TEnum, ulong>(ref Unsafe.AsRef(in right));
        int   block = (int)( index / BITS_PER_BLOCK );
        int   bit   = (int)( index % BITS_PER_BLOCK );
        ulong mask  = 1UL << bit;

        if ( value ) { _bits[block] |= mask; }
        else { _bits[block]         &= ~mask; }

        return this;
    }
    [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] public readonly bool Has( TEnum right )
    {
        ulong index = Unsafe.As<TEnum, ulong>(ref Unsafe.AsRef(in right));
        int   block = (int)( index / BITS_PER_BLOCK );
        int   bit   = (int)( index % BITS_PER_BLOCK );
        return ( _bits[block] & ( 1UL << bit ) ) != 0;
    }


    public readonly UserRights<TEnum> Or( UserRights<TEnum> other )
    {
        for ( int i = 0; i < _bits.Length; i++ ) { _bits[i] |= other._bits[i]; }

        return this;
    }
    public readonly UserRights<TEnum> And( UserRights<TEnum> other )
    {
        for ( int i = 0; i < _bits.Length; i++ ) { _bits[i] &= other._bits[i]; }

        return this;
    }
    public readonly UserRights<TEnum> Xor( UserRights<TEnum> other )
    {
        for ( int i = 0; i < _bits.Length; i++ ) { _bits[i] ^= other._bits[i]; }

        return this;
    }
}



/// <summary> Efficient, allocation-free permission bitmask supporting >64 bits. </summary>
public readonly ref struct PermissionMask<TEnum>( Span<ulong> buffer )
    where TEnum : unmanaged, Enum
{
    private const           int         BITS_PER_BLOCK = 64;
    public static readonly  int         Blocks         = GetBlocks();
    private static readonly TEnum[]     _enumValues    = Enum.GetValues<TEnum>();
    private readonly        Span<ulong> _bits          = buffer;


    public static ReadOnlySpan<TEnum>                EnumValues => _enumValues;
    public static PermissionMask<TEnum>              None       => new(Span<ulong>.Empty);
    public        ValueEnumerable<Enumerable, Right> Rights     => new(new Enumerable(this));
    public static IMemoryOwner<ulong>                NewBuffer  { [MustDisposeResource] get => MemoryPool<ulong>.Shared.Rent(Blocks); }


    static PermissionMask()
    {
        if ( Enum.GetUnderlyingType(typeof(TEnum)) != typeof(int) ) { throw new InvalidOperationException($"{typeof(TEnum).Name} must have an underlying type of int."); }

        TEnum first = EnumValues[0];
        if ( first.AsULong() != 0 ) { throw new InvalidOperationException($"{typeof(TEnum).Name} enum values must start at 0."); }

        if ( !IsContiguous() ) { throw new InvalidOperationException($"{typeof(TEnum).Name} enum values must be contiguous."); }
    }

    public static PermissionMask<TEnum> Create( Span<ulong> buffer, params ReadOnlySpan<bool> masks )
    {
        PermissionMask<TEnum> mask = new(buffer);
        ReadOnlySpan<TEnum>   span = EnumValues;
        for ( int i = 0; i < masks.Length; i++ ) { mask.Set(in span[i], masks[i]); }

        return mask;
    }

    public static PermissionMask<TEnum> Create( Span<ulong> buffer, params ReadOnlySpan<TEnum> values )
    {
        PermissionMask<TEnum> mask = new(buffer);
        foreach ( ref readonly TEnum t in values ) { mask.Set(in t, true); }

        return mask;
    }

    public static PermissionMask<TEnum> Create( out IMemoryOwner<ulong> buffer, params ReadOnlySpan<TEnum> values )
    {
        buffer = NewBuffer;
        Span<ulong> span = buffer.Memory.Span[..Blocks];
        return Create(span, values);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public PermissionMask<TEnum> Set( ref readonly TEnum right, bool value )
    {
        int   index = right.AsInt();
        int   block = index / BITS_PER_BLOCK;
        int   bit   = index % BITS_PER_BLOCK;
        ulong mask  = 1UL << bit;

        if ( value ) { _bits[block] |= mask; }
        else { _bits[block]         &= ~mask; }

        return this;
    }

    [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Has( TEnum right )
    {
        int index = Unsafe.As<TEnum, int>(ref Unsafe.AsRef(in right));
        int block = index / BITS_PER_BLOCK;
        int bit   = index % BITS_PER_BLOCK;
        return ( _bits[block] & ( 1UL << bit ) ) != 0;
    }


    public PermissionMask<TEnum> Or( PermissionMask<TEnum> other )
    {
        for ( int i = 0; i < _bits.Length; i++ ) { _bits[i] |= other._bits[i]; }

        return this;
    }

    public PermissionMask<TEnum> And( PermissionMask<TEnum> other )
    {
        for ( int i = 0; i < _bits.Length; i++ ) { _bits[i] &= other._bits[i]; }

        return this;
    }

    public PermissionMask<TEnum> Xor( PermissionMask<TEnum> other )
    {
        for ( int i = 0; i < _bits.Length; i++ ) { _bits[i] ^= other._bits[i]; }

        return this;
    }

    public PermissionMask<TEnum> Not()
    {
        for ( int i = 0; i < _bits.Length; i++ ) { _bits[i] = ~_bits[i]; }

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static PermissionMask<TEnum> operator |( PermissionMask<TEnum> x, PermissionMask<TEnum> y ) => x.Or(y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static PermissionMask<TEnum> operator &( PermissionMask<TEnum> x, PermissionMask<TEnum> y ) => x.And(y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static PermissionMask<TEnum> operator ^( PermissionMask<TEnum> x, PermissionMask<TEnum> y ) => x.Xor(y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static PermissionMask<TEnum> operator ~( PermissionMask<TEnum> x ) => x.Not();


    public static bool IsContiguous()
    {
        ReadOnlySpan<TEnum> span = EnumValues;

        for ( int i = 0; i < span.Length; i++ )
        {
            TEnum value = span[i];
            if ( i != value.AsInt() ) { return false; }
        }

        return true;
    }
    public static int GetBlocks()
    {
        TEnum last     = EnumValues[^1];
        int   maxIndex = last.AsInt();
        return ( maxIndex / BITS_PER_BLOCK ) + 1;
    }



    public readonly record struct Right( TEnum Index, bool Value );



    public ref struct Enumerable : IValueEnumerator<Right>
    {
        private readonly PermissionMask<TEnum>          __rights;
        private          ReadOnlySpan<TEnum>.Enumerator __enums = EnumValues.GetEnumerator();
        public Enumerable( PermissionMask<TEnum> rights ) => __rights = rights;


        public void Dispose() => this = default;
        public bool TryGetNext( out Right current )
        {
            if ( __enums.MoveNext() )
            {
                TEnum e = __enums.Current;
                current = new Right(e, __rights.Has(e));
                return true;
            }

            current = default;
            return false;
        }
        public bool TryGetNonEnumeratedCount( out int count )
        {
            count = _enumValues.Length;
            return true;
        }
        public bool TryGetSpan( out ReadOnlySpan<Right> span )
        {
            Right[] rights = GC.AllocateUninitializedArray<Right>(_enumValues.Length);
            int     i      = 0;
            foreach ( ref readonly TEnum e in EnumValues ) { rights[i++] = new Right(e, __rights.Has(e)); }

            span = rights;
            return true;
        }
        public bool TryCopyTo( scoped Span<Right> destination, Index offset )
        {
            if ( !TryGetSpan(out var span) ) { return false; }

            destination[offset] = span[offset];
            return true;
        }
    }
}
