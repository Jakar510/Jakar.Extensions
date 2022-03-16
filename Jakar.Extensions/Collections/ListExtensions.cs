namespace Jakar.Extensions.Collections;


public static class ListExtensions
{
    public static bool IsEqual<TValue>( this TValue value, TValue other )
    {
        if ( value is null ) throw new NullReferenceException(nameof(value));
        if ( other is null ) throw new NullReferenceException(nameof(other));

        Type vType = value.GetType();
        if ( vType.IsValueType || vType.IsClass || vType.IsEnum ) { return Equals(value, other); }

        return ReferenceEquals(value, other);
    }

    public static bool IsOneOf<TValue>( this TValue value, params TValue[] items ) => items.AsParallel().Any(other => value.IsEqual(other));
}
