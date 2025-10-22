// Jakar.Extensions :: Jakar.Extensions
// 10/14/2025  22:34

using System;
using Org.BouncyCastle.Utilities.Encoders;
using ZLinq;



namespace Jakar.Extensions;


/*
public readonly ref struct Permissions : IDisposable
{
    private readonly UserRights? __rights;
    private readonly bool[]      __array;
    public readonly  int         Capacity;
    private static   int?        __count;
    private readonly Span<bool>  __span;


    public static int         Count       { get => __count ?? throw new InvalidOperationException("Permissions.Count has not been set."); set => __count = Math.Max(0, value); }
    public static Permissions Default     { [MustDisposeResource] get => new(); }
    public static char        ValidChar   { get; set; } = '+';
    public static char        InvalidChar { get; set; } = '.';
    public        Enumerable  Rights      => new(this);
    public ref bool this[ int index ] { [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref __span[index]; }


    [MustDisposeResource] public Permissions() : this(null) { }
    public Permissions( UserRights? rights = null, int? capacity = null )
    {
        __rights = rights;
        Capacity = capacity ?? Count;
        __array  = ArrayPool<bool>.Shared.Rent(Capacity);
        __span   = __array;
        __span.Fill(false);
    }
    public void Dispose()
    {
        __rights?.SetRights(this);
        ArrayPool<bool>.Shared.Return(__array);
    }


    public override string ToString()
    {
        using IMemoryOwner<char> owner  = MemoryPool<char>.Shared.Rent(Capacity);
        Span<char>               chars  = owner.Memory.Span[..Capacity];
        Span<bool>               buffer = __span;

        for ( int i = 0; i < Capacity; i++ )
        {
            chars[i] = buffer[i]
                           ? ValidChar
                           : InvalidChar;
        }

        return new string(chars);
    }


    [MustDisposeResource] public static Permissions Create( IUserRights? rights ) => Create(rights?.Rights);
    [MustDisposeResource] public static Permissions Create( IEnumerable<IUserRights> values )
    {
        Permissions permissions = Default;

        foreach ( IUserRights user in values )
        {
            using Permissions aggregate = Create(user);
            permissions.Or(aggregate);
        }

        return permissions;
    }
    [MustDisposeResource] public static Permissions Create( IEnumerable<IEnumerable<IUserRights>> values )
    {
        Permissions permissions = Default;

        foreach ( IEnumerable<IUserRights> users in values )
        {
            using Permissions usersRights = Create(users);
            permissions.Or(usersRights);
        }

        return permissions;
    }
    [MustDisposeResource] public static Permissions Create( UserRights?               rights ) => Create(rights, rights?.Value);
    [MustDisposeResource] public static Permissions Create( params ReadOnlySpan<char> span )   => Create(null,   span);
    [MustDisposeResource] public static Permissions Create( UserRights? rights, params ReadOnlySpan<char> span )
    {
        Permissions permissions = new(rights);
        if ( span.IsNullOrWhiteSpace() ) { return permissions; }

        for ( int i = 0; i < span.Length && i < permissions.Capacity; i++ )
        {
            char v = span[i];

            if ( v      == ValidChar ) { permissions.Grant(i); }
            else if ( v == InvalidChar ) { permissions.Revoke(i); }
            else { throw new FormatException($"Invalid character '{v}' at position {i}. Expected '{ValidChar}' or '{InvalidChar}'."); }
        }

        return permissions;
    }
    [MustDisposeResource] public static Permissions Create( params ReadOnlySpan<Right> span ) => Create(null, span);
    [MustDisposeResource] public static Permissions Create( UserRights? rights, params ReadOnlySpan<Right> span )
    {
        Permissions permissions = new(rights);
        if ( span.IsEmpty ) { return permissions; }

        for ( int i = 0; i < span.Length; i++ ) { permissions.Set(span[i].Index, span[i].Value); }

        return permissions;
    }
    [MustDisposeResource] public static Permissions Create( params ReadOnlySpan<int> values ) => Create(null, values);
    [MustDisposeResource] public static Permissions Create( UserRights? rights, params ReadOnlySpan<int> span )
    {
        Permissions permissions = new(rights);
        if ( span.IsEmpty ) { return permissions; }

        foreach ( ref readonly int t in span ) { permissions.Set(t, true); }

        return permissions;
    }
    [MustDisposeResource] public static Permissions Create<TEnum>( params ReadOnlySpan<TEnum> values )
        where TEnum : unmanaged, Enum => Create(null, values);
    [MustDisposeResource] public static Permissions Create<TEnum>( UserRights? rights, params ReadOnlySpan<TEnum> span )
        where TEnum : unmanaged, Enum
    {
        Permissions permissions = new(rights);
        if ( span.IsEmpty ) { return permissions; }

        foreach ( ref readonly TEnum t in span ) { permissions.Set(t.AsInt(), true); }

        return permissions;
    }
    [MustDisposeResource] public static Permissions SA( UserRights? rights = null ) => Create(rights);


    private Permissions Set( int index, bool value )
    {
        __span[index] = value;
        return this;
    }

    [Pure] public bool Has( int index ) => __span[index];


    public Permissions Grant<TEnum>( TEnum index )
        where TEnum : unmanaged, Enum => Set(index.AsInt(), true);
    public Permissions Grant( int index ) => Set(index, true);
    public Permissions Grant<TEnum>( params ReadOnlySpan<TEnum> permissions )
        where TEnum : unmanaged, Enum
    {
        foreach ( TEnum permission in permissions ) { Grant(permission.AsInt()); }

        return this;
    }
    public Permissions Grant( params ReadOnlySpan<int> permissions )
    {
        foreach ( int index in permissions ) { Grant(index); }

        return this;
    }


    public Permissions Revoke<TEnum>( TEnum index )
        where TEnum : unmanaged, Enum => Set(index.AsInt(), false);
    public Permissions Revoke( int index ) => Set(index, false);
    public Permissions Revoke<TEnum>( params ReadOnlySpan<TEnum> permissions )
        where TEnum : unmanaged, Enum
    {
        foreach ( TEnum permission in permissions ) { Revoke(permission.AsInt()); }

        return this;
    }
    public Permissions Revoke( params ReadOnlySpan<int> permissions )
    {
        foreach ( int index in permissions ) { Revoke(index); }

        return this;
    }


    public Permissions Or( Permissions other )
    {
        Span<bool> buffer      = __span;
        Span<bool> otherBuffer = other.__span;
        for ( int i = 0; i < buffer.Length; i++ ) { buffer[i] |= otherBuffer[i]; }

        return this;
    }
    public Permissions And( Permissions other )
    {
        Span<bool> buffer      = __span;
        Span<bool> otherBuffer = other.__span;
        for ( int i = 0; i < buffer.Length; i++ ) { buffer[i] &= otherBuffer[i]; }

        return this;
    }
    public Permissions Xor( Permissions other )
    {
        Span<bool> buffer      = __span;
        Span<bool> otherBuffer = other.__span;
        for ( int i = 0; i < buffer.Length; i++ ) { buffer[i] ^= otherBuffer[i]; }

        return this;
    }
    public Permissions Not()
    {
        Span<bool> buffer = __span;
        for ( int i = 0; i < buffer.Length; i++ ) { buffer[i] = !buffer[i]; }

        return this;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Permissions operator |( Permissions x, Permissions y ) => x.Or(y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Permissions operator &( Permissions x, Permissions y ) => x.And(y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Permissions operator ^( Permissions x, Permissions y ) => x.Xor(y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Permissions operator ~( Permissions x ) => x.Not();



    public ref struct Enumerable : IValueEnumerator<Right>
    {
        private readonly ReadOnlySpan<bool> __span;
        private          Right              __current;
        private          int                __index;
        public Enumerable( Permissions permissions ) => __span = permissions.__span;

        public ref readonly Right Current => ref Unsafe.AsRef(ref __current);

        public bool MoveNext()
        {
            if ( ++__index < __span.Length )
            {
                int i = __index;
                __current = new Right(i, __span[i]);
                return true;
            }

            __current = default;
            return false;
        }
        public void       Dispose()       => this = default;
        public Enumerable GetEnumerator() => this;


        public bool TryGetNext( out Right current )
        {
            bool result = MoveNext();
            current = __current;
            return result;
        }
        public bool TryGetNonEnumeratedCount( out int count )
        {
            count = __span.Length;
            return true;
        }
        public bool TryGetSpan( out ReadOnlySpan<Right> span )
        {
            Right[] permissions = GC.AllocateUninitializedArray<Right>(__span.Length);

            for ( int i = 0; i < __span.Length; i++ ) { permissions[i++] = new Right(i, __span[i]); }

            span = permissions;
            ref readonly var x = ref span[1];
            return true;
        }
        public bool TryCopyTo( scoped Span<Right> destination, Index offset )
        {
            if ( !TryGetSpan(out ReadOnlySpan<Right> span) ) { return false; }

            destination[offset] = span[offset];
            return true;
        }
    }
}
*/



public readonly record struct Right( int Index, bool Value )
{
    public readonly int  Index = Index;
    public readonly bool Value = Value;
}



public readonly ref struct Permissions<TEnum> : IDisposable
    where TEnum : unmanaged, Enum
{
    private readonly UserRights?         __rights;
    private static   TEnum[]?            __enumValues;
    private readonly bool[]              __array;
    private readonly Span<bool>          __span;
    public static    char                ValidChar   { get; set; } = '+';
    public static    char                InvalidChar { get; set; } = '.';
    public static    Permissions<TEnum>  Default     { [MustDisposeResource] get => new(); }
    public static    int                 Count       => EnumValues.Length;
    public static    ReadOnlySpan<TEnum> EnumValues  => __enumValues ??= Enum.GetValues<TEnum>();
    public           Enumerable          Rights      => new(this);


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
        __array  = ArrayPool<bool>.Shared.Rent(Count);
        __span   = __array;
        __span.Fill(false);
    }
    public void Dispose()
    {
        __rights?.SetRights(this);
        ArrayPool<bool>.Shared.Return(__array);
    }


    [MustDisposeResource] public static implicit operator Permissions<TEnum>( UserRights?         rights ) => Create(rights);
    [MustDisposeResource] public static implicit operator Permissions<TEnum>( ReadOnlySpan<TEnum> rights ) => Create(rights);
    [MustDisposeResource] public static implicit operator Permissions<TEnum>( TEnum               rights ) => Create(rights);


    [HandlesResourceDisposal] public string ToStringAndDispose()
    {
        try { return ToString(); }
        finally { Dispose(); }
    }
    public override string ToString()
    {
        using IMemoryOwner<char> owner = MemoryPool<char>.Shared.Rent(Count);
        Span<char>               chars = owner.Memory.Span;

        for ( int i = 0; i < Count; i++ )
        {
            chars[i] = Has(EnumValues[i])
                           ? ValidChar
                           : InvalidChar;
        }

        return new string(chars);
    }


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
    [MustDisposeResource] public static Permissions<TEnum> Create( UserRights?               rights ) => Create(rights, rights?.Value);
    [MustDisposeResource] public static Permissions<TEnum> Create( params ReadOnlySpan<char> span )   => Create(null,   span);
    [MustDisposeResource] public static Permissions<TEnum> Create( UserRights? rights, params ReadOnlySpan<char> span )
    {
        Permissions<TEnum> permissions = new(rights);
        if ( span.IsNullOrWhiteSpace() ) { return permissions; }

        for ( int i = 0; i < span.Length && i < Count; i++ )
        {
            char v = span[i];

            if ( v      == ValidChar ) { permissions.Grant(EnumValues[i]); }
            else if ( v == InvalidChar ) { permissions.Revoke(EnumValues[i]); }
            else { throw new FormatException($"Invalid character '{v}' at position {i}. Expected '{ValidChar}' or '{InvalidChar}'."); }
        }

        return permissions;
    }
    [MustDisposeResource] public static Permissions<TEnum> Create( params ReadOnlySpan<bool> span ) => Create(null, span);
    [MustDisposeResource] public static Permissions<TEnum> Create( UserRights? rights, params ReadOnlySpan<bool> span )
    {
        Permissions<TEnum>  permissions = new(rights);
        ReadOnlySpan<TEnum> values      = EnumValues;
        if ( span.IsEmpty ) { return permissions; }

        for ( int i = 0; i < span.Length; i++ ) { permissions.Set(values[i], span[i]); }

        return permissions;
    }
    [MustDisposeResource] public static Permissions<TEnum> Create( params ReadOnlySpan<TEnum> values ) => Create(null, values);
    [MustDisposeResource] public static Permissions<TEnum> Create( UserRights? rights, params ReadOnlySpan<TEnum> span )
    {
        Permissions<TEnum> permissions = new(rights);
        if ( span.IsEmpty ) { return permissions; }

        foreach ( ref readonly TEnum t in span ) { permissions.Set(t, true); }

        return permissions;
    }
    [MustDisposeResource] public static Permissions<TEnum> SA( UserRights? rights = null ) => Create(rights, EnumValues);


    private Permissions<TEnum> Set( TEnum index, bool value )
    {
        int i = index.AsInt();
        __span[i] = value;
        return this;
    }


    [Pure] [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool Has( TEnum index )
    {
        int i = index.AsInt();
        return __span[i];
    }


    public Permissions<TEnum> Grant( TEnum index ) => Set(index, true);
    public Permissions<TEnum> Grant( params ReadOnlySpan<TEnum> permissions )
    {
        foreach ( TEnum index in permissions ) { Grant(index); }

        return this;
    }


    public Permissions<TEnum> Revoke( TEnum index ) => Set(index, false);
    public Permissions<TEnum> Revoke( params ReadOnlySpan<TEnum> permissions )
    {
        foreach ( TEnum index in permissions ) { Revoke(index); }

        return this;
    }


    public Permissions<TEnum> Or( Permissions<TEnum> other )
    {
        Span<bool> buffer      = __span;
        Span<bool> otherBuffer = other.__span;
        for ( int i = 0; i < buffer.Length; i++ ) { buffer[i] |= otherBuffer[i]; }

        return this;
    }
    public Permissions<TEnum> And( Permissions<TEnum> other )
    {
        Span<bool> buffer      = __span;
        Span<bool> otherBuffer = other.__span;
        for ( int i = 0; i < buffer.Length; i++ ) { buffer[i] &= otherBuffer[i]; }

        return this;
    }
    public Permissions<TEnum> Xor( Permissions<TEnum> other )
    {
        Span<bool> buffer      = __span;
        Span<bool> otherBuffer = other.__span;
        for ( int i = 0; i < buffer.Length; i++ ) { buffer[i] ^= otherBuffer[i]; }

        return this;
    }
    public Permissions<TEnum> Not()
    {
        Span<bool> buffer = __span;
        for ( int i = 0; i < buffer.Length; i++ ) { buffer[i] = !buffer[i]; }

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



    public ref struct Enumerable( Permissions<TEnum> permissions ) : IValueEnumerator<Right<TEnum>>
    {
        private readonly Permissions<TEnum>             __rights = permissions;
        private          ReadOnlySpan<TEnum>.Enumerator __enums  = EnumValues.GetEnumerator();
        private          Right<TEnum>                   __current;

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
            count = Count;
            return true;
        }
        public bool TryGetSpan( out ReadOnlySpan<Right<TEnum>> span )
        {
            Right<TEnum>[] permissions = GC.AllocateUninitializedArray<Right<TEnum>>(Count);
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
