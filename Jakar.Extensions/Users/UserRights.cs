// Jakar.Extensions :: Jakar.Database
// 02/19/2023  7:25 PM

namespace Jakar.Extensions;


public interface IUserRights
{
    public const                    int    MAX_SIZE = ANSI_CAPACITY;
    [StringLength(MAX_SIZE)] public string Rights { get; set; }
}



public interface IUserRights<out TValue, TEnum> : IUserRights
    where TEnum : struct, Enum
    where TValue : IUserRights<TValue, TEnum>
{
    public TValue WithRights( UserRights<TEnum> rights );
}



public static class RightsExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static UserRights<TEnum> GetRights<TEnum>( this IUserRights rights )
        where TEnum : struct, Enum => UserRights<TEnum>.Create(rights);


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SetRights<TEnum>( this IUserRights user, params ReadOnlySpan<TEnum> indexes )
        where TEnum : struct, Enum => user.SetRights(user.GetRights<TEnum>()
                                                         .Add(indexes));


    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void SetRights<TEnum>( this IUserRights user, scoped in UserRights<TEnum> value )
        where TEnum : struct, Enum => user.Rights = value.ToString();
}



[DefaultMember(nameof(Default))]
public ref struct UserRights<TEnum>
    where TEnum : struct, Enum
{
    public const            char               VALID        = '+';
    public const            char               INVALID      = '-';
    private static readonly TEnum[]            __enumValues = Enum.GetValues<TEnum>();
    private readonly        IMemoryOwner<char> __rights     = MemoryPool<char>.Shared.Rent(__enumValues.Length);
    internal                Span<char>         span;


    public static UserRights<TEnum> Default => new();
    public static UserRights<TEnum> SA      => Create(__enumValues);
    public        int               Length  => span.Length;
    public readonly Right[] Rights
    {
        [Pure] get
        {
            Right[] indexes = AsyncLinq.GetArray<Right>(__enumValues.Length);
            for ( int i = 0; i < indexes.Length; i++ ) { indexes[i] = new Right(__enumValues[i], Has(i)); }

            return indexes;
        }
    }


    public UserRights()
    {
        span = __rights.Memory.Span[..__enumValues.Length];
        if ( __enumValues.Length > IUserRights.MAX_SIZE ) { throw OutOfRangeException.Create(typeof(TEnum).Name, $"Max permission count is {IUserRights.MAX_SIZE}"); }

        span.Fill(INVALID);
    }
    public void Dispose()
    {
        __rights.Dispose();
        span = Span<char>.Empty;
        this = default;
    }


    [Pure] public static   UserRights<TEnum> Create()                                     => Default;
    [Pure] internal static UserRights<TEnum> Create( ReadOnlySpan<char>         rights )  => Default.With(rights);
    [Pure] public static   UserRights<TEnum> Create( params ReadOnlySpan<TEnum> indexes ) => Default.Add(indexes);

    [Pure] public static UserRights<TEnum> Create<TValue>( IEnumerable<IEnumerable<TValue>> user )
        where TValue : IUserRights => Default.With(user);

    [Pure] public static UserRights<TEnum> Create<TValue>( IEnumerable<TValue> user )
        where TValue : IUserRights => Default.With(user);

    [Pure] public static UserRights<TEnum> Create<TValue>( TValue user )
        where TValue : IUserRights => Default.With(user);


    [Pure] public override string ToString()
    {
        string result = span.ToString();
        Dispose();
        return result;
    }


    public UserRights<TEnum> With<TValue>( IEnumerable<IEnumerable<TValue>> values )
        where TValue : IUserRights => With(values.SelectMany(static x => x));
    public UserRights<TEnum> With<TValue>( IEnumerable<TValue> values )
        where TValue : IUserRights
    {
        foreach ( TValue value in values ) { this = With(value); }

        return this;
    }
    public UserRights<TEnum> With<TValue>( params ReadOnlySpan<TValue> values )
        where TValue : IUserRights
    {
        foreach ( TValue value in values ) { this = With(value); }

        return this;
    }


    public readonly UserRights<TEnum> With<TValue>( TValue user )
        where TValue : IUserRights => With(user.Rights);
    private readonly UserRights<TEnum> With( params ReadOnlySpan<char> other )
    {
        Span<char> span = this.span;
        Guard.IsGreaterThanOrEqualTo(other.Length, span.Length);

        for ( int i = 0; i < span.Length; i++ )
        {
            if ( VALID.Equals(other[i]) ) { span[i] = VALID; }
        }

        return this;
    }


    public readonly bool Has( int   index ) => span[index] == VALID;
    public readonly bool Has( TEnum index ) => Has(index.AsInt());
    public readonly bool Has( params ReadOnlySpan<TEnum> indexes )
    {
        foreach ( TEnum i in indexes )
        {
            if ( !Has(i) ) { return false; }
        }

        return true;
    }


    public readonly UserRights<TEnum> Remove( TEnum index ) => Set(index.AsInt(), INVALID);
    public readonly UserRights<TEnum> Remove( params ReadOnlySpan<TEnum> indexes )
    {
        foreach ( TEnum i in indexes ) { Remove(i); }

        return this;
    }


    public readonly UserRights<TEnum> Add( TEnum index ) => Set(index.AsInt(), VALID);
    public readonly UserRights<TEnum> Add( params ReadOnlySpan<TEnum> indexes )
    {
        foreach ( TEnum i in indexes ) { Add(i); }

        return this;
    }


    private readonly UserRights<TEnum> Set( int index, char value )
    {
        Guard.IsInRange(index, 0, __enumValues.Length);
        Guard.IsTrue(value is VALID or INVALID);

        span[index] = value;
        return this;
    }



    public readonly record struct Right( TEnum Index, bool Value );
}
