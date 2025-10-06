// Jakar.Extensions :: Jakar.Extensions
// 03/05/2023  11:48 PM

namespace Jakar.Extensions;


/// <summary> Class to cast <typeparamref name="TEnum"/> to <typeparamref name="TResult"/> </summary>
/// <remarks>
///     <see href="https://stackoverflow.com/a/23391746/9530917"/>
/// </remarks>
public static class CastTo<TEnum, TResult>
    where TEnum : struct, Enum
{
    /// <summary> Casts <typeparamref name="TEnum"/> to <typeparamref name="TResult"/>. This does not cause boxing for value types. Useful in generic methods. </summary>
    public static TResult From( TEnum s ) => Cache.Caster(s);



    private static class Cache
    {
        public static readonly Func<TEnum, TResult> Caster = Get();

        private static Func<TEnum, TResult> Get()
        {
            ParameterExpression p = Expression.Parameter(typeof(TEnum));
            UnaryExpression     c = Expression.ConvertChecked(p, typeof(TResult));

            return Expression.Lambda<Func<TEnum, TResult>>(c, p)
                             .Compile();
        }
    }
}
