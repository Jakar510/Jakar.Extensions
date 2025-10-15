// Jakar.Extensions :: Jakar.Extensions
// 10/14/2025  22:34

using ZLinq;



namespace Jakar.Extensions;


/// <summary> Efficient, allocation-free permission bitmask supporting >64 bits. </summary>
public readonly ref struct Permissions<TEnum> : IDisposable
    where TEnum : unmanaged, Enum
{
    private readonly        UserRights?         __rights;
    public const            char                VALID          = '+';
    public const            char                INVALID        = '-';
    private const           int                 BITS_PER_BLOCK = 64;
    public static readonly  int                 Blocks         = GetBlocks();
    private static readonly TEnum[]             _enumValues    = Enum.GetValues<TEnum>();
    private readonly        IMemoryOwner<ulong> _owner         = MemoryPool<ulong>.Shared.Rent(Blocks);


    private       Span<ulong>         _bits      { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => _owner.Memory.Span[..Blocks]; }
    public static Permissions<TEnum>  Default    { [MustDisposeResource] get => new(); }
    public static int                 Length     => _enumValues.Length;
    public static ReadOnlySpan<TEnum> EnumValues => _enumValues;
    public        Enumerable          Rights     => new(this);


    static Permissions()
    {
        if ( Enum.GetUnderlyingType(typeof(TEnum)) != typeof(int) ) { throw new InvalidOperationException($"{typeof(TEnum).Name} must have an underlying type of int."); }

        TEnum first = EnumValues[0];
        if ( first.AsULong() != 0 ) { throw new InvalidOperationException($"{typeof(TEnum).Name} enum values must start at 0."); }

        if ( !IsContiguous() ) { throw new InvalidOperationException($"{typeof(TEnum).Name} enum values must be contiguous."); }
    }
    [MustDisposeResource] public Permissions() : this(null) { }
    [MustDisposeResource] public Permissions( UserRights? rights )
    {
        __rights = rights;
        _bits.Fill(0);
    }
    public void Dispose()
    {
        __rights?.SetRights(in this);
        _owner.Dispose();
    }
    public override string ToString()
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
    public string ToHexString()
    {
        const int   SIZE   = sizeof(ulong);
        Span<ulong> buffer = _bits;
        Span<byte>  span   = stackalloc byte[buffer.Length * SIZE];

        for ( int i = 0; i < buffer.Length; i++ )
        {
            ulong n = buffer[i];
            if ( !BitConverter.TryWriteBytes(span.Slice(i * SIZE, SIZE), n) ) { throw new InvalidOperationException($"{nameof(BitConverter)}.{nameof(BitConverter.TryWriteBytes)} convert ulong to bytes failed"); }
        }

        return Convert.ToHexString(span);
    }


    [MustDisposeResource] public static Permissions<TEnum> FromHexString( UserRights? rights, params ReadOnlySpan<char> hexString )
    {
        const int          SIZE        = sizeof(ulong);
        Permissions<TEnum> permissions = Create(rights);
        if ( hexString.IsNullOrWhiteSpace() ) { return permissions; }

        Span<ulong> buffer = permissions._bits;
        if ( !hexString.TryHexToUInt64Span(buffer) ) { throw new FormatException("Invalid hex string format or incorrect length."); }

        return permissions;
    }
    [MustDisposeResource] public static Permissions<TEnum> Create( UserRights?  rights ) => new(rights);
    [MustDisposeResource] public static Permissions<TEnum> Create( IUserRights? rights ) => Create(rights?.Rights);
    [MustDisposeResource] public static Permissions<TEnum> Create( IEnumerable<IUserRights> values )
    {
        Permissions<TEnum> permissions = Default;

        foreach ( IUserRights user in values )
        {
            using Permissions<TEnum> aggregate = Create(user);
            permissions.Or(aggregate);
        }

        return permissions;
    }
    [MustDisposeResource] public static Permissions<TEnum> Create( IEnumerable<IEnumerable<IUserRights>> values )
    {
        Permissions<TEnum> permissions = Default;

        foreach ( IEnumerable<IUserRights> users in values )
        {
            using Permissions<TEnum> usersRights = Create(users);
            permissions.Or(usersRights);
        }

        return permissions;
    }
    [MustDisposeResource] public static Permissions<TEnum> Create( UserRights? rights, params ReadOnlySpan<char> values )
    {
        Permissions<TEnum> permissions = Create(rights);

        for ( int i = 0; i < values.Length && i < Length; i++ )
        {
            char v = values[i];

            switch ( v )
            {
                case VALID:
                    permissions.Add(EnumValues[i]);
                    break;

                case INVALID:
                    permissions.Remove(EnumValues[i]);
                    break;

                default:
                    throw new FormatException($"Invalid character '{v}' at position {i}. Expected '{VALID}' or '{INVALID}'.");
            }
        }

        return permissions;
    }
    [MustDisposeResource] public static Permissions<TEnum> Create( UserRights? rights, params ReadOnlySpan<bool> masks )
    {
        Permissions<TEnum>  permissions = Create(rights);
        ReadOnlySpan<TEnum> span        = EnumValues;
        for ( int i = 0; i < masks.Length; i++ ) { permissions.Set(span[i], masks[i]); }

        return permissions;
    }
    [MustDisposeResource] public static Permissions<TEnum> Create( UserRights? rights, params ReadOnlySpan<TEnum> values )
    {
        Permissions<TEnum> permissions = Create(rights);
        foreach ( ref readonly TEnum t in values ) { permissions.Set(t, true); }

        return permissions;
    }
    [MustDisposeResource] public static Permissions<TEnum> SA( UserRights? rights ) => Create(rights, EnumValues);


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public Permissions<TEnum> Set( TEnum right, bool value )
    {
        int   index       = right.AsInt();
        int   block       = index / BITS_PER_BLOCK;
        int   bit         = index % BITS_PER_BLOCK;
        ulong permissions = 1UL << bit;

        if ( value ) { _bits[block] |= permissions; }
        else { _bits[block]         &= ~permissions; }

        return this;
    }


    [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Has( TEnum right )
    {
        int index = Unsafe.As<TEnum, int>(ref Unsafe.AsRef(in right));
        int block = index / BITS_PER_BLOCK;
        int bit   = index % BITS_PER_BLOCK;
        return ( _bits[block] & ( 1UL << bit ) ) != 0;
    }


    public Permissions<TEnum> Add( TEnum right ) => Set(right, true);
    public Permissions<TEnum> Add( params ReadOnlySpan<TEnum> permissions )
    {
        foreach ( TEnum right in permissions ) { Add(right); }

        return this;
    }


    public Permissions<TEnum> Remove( TEnum right ) => Set(right, false);
    public Permissions<TEnum> Remove( params ReadOnlySpan<TEnum> permissions )
    {
        foreach ( TEnum right in permissions ) { Remove(right); }

        return this;
    }


    public Permissions<TEnum> Or( Permissions<TEnum> other )
    {
        for ( int i = 0; i < _bits.Length; i++ ) { _bits[i] |= other._bits[i]; }

        return this;
    }
    public Permissions<TEnum> And( Permissions<TEnum> other )
    {
        for ( int i = 0; i < _bits.Length; i++ ) { _bits[i] &= other._bits[i]; }

        return this;
    }
    public Permissions<TEnum> Xor( Permissions<TEnum> other )
    {
        for ( int i = 0; i < _bits.Length; i++ ) { _bits[i] ^= other._bits[i]; }

        return this;
    }
    public Permissions<TEnum> Not()
    {
        for ( int i = 0; i < _bits.Length; i++ ) { _bits[i] = ~_bits[i]; }

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Permissions<TEnum> operator |( Permissions<TEnum> x, Permissions<TEnum> y ) => x.Or(y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Permissions<TEnum> operator &( Permissions<TEnum> x, Permissions<TEnum> y ) => x.And(y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Permissions<TEnum> operator ^( Permissions<TEnum> x, Permissions<TEnum> y ) => x.Xor(y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Permissions<TEnum> operator ~( Permissions<TEnum> x ) => x.Not();


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



    public ref struct Enumerable : IValueEnumerator<Right<TEnum>>
    {
        private readonly Permissions<TEnum>             __rights;
        private          ReadOnlySpan<TEnum>.Enumerator __enums = EnumValues.GetEnumerator();
        private          Right<TEnum>                   __current;
        public Enumerable( Permissions<TEnum> permissions ) => __rights = permissions;

        public ref readonly Right<TEnum> Current => ref Unsafe.AsRef(ref __current);

        public bool MoveNext()
        {
            if ( __enums.MoveNext() )
            {
                TEnum e = __enums.Current;
                __current = new Right<TEnum>(e, __rights.Has(e));
                return true;
            }

            __current = default;
            return false;
        }
        public void       Dispose()       => this = default;
        public Enumerable GetEnumerator() => this;


        public bool TryGetNext( out Right<TEnum> current )
        {
            bool result = MoveNext();
            current = __current;
            return result;
        }
        public bool TryGetNonEnumeratedCount( out int count )
        {
            count = _enumValues.Length;
            return true;
        }
        public bool TryGetSpan( out ReadOnlySpan<Right<TEnum>> span )
        {
            Right<TEnum>[] permissions = GC.AllocateUninitializedArray<Right<TEnum>>(_enumValues.Length);
            int            i           = 0;
            foreach ( ref readonly TEnum e in EnumValues ) { permissions[i++] = new Right<TEnum>(e, __rights.Has(e)); }

            span = permissions;
            ref readonly var x = ref span[1];
            return true;
        }
        public bool TryCopyTo( scoped Span<Right<TEnum>> destination, Index offset )
        {
            if ( !TryGetSpan(out ReadOnlySpan<Right<TEnum>> span) ) { return false; }

            destination[offset] = span[offset];
            return true;
        }
    }
}



public readonly record struct Right<TEnum>( TEnum Index, bool Value )
    where TEnum : unmanaged, Enum
{
    public readonly TEnum Index = Index;
    public readonly bool  Value = Value;
}
