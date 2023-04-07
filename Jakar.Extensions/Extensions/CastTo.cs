// Jakar.Extensions :: Jakar.Extensions
// 03/05/2023  11:48 PM

using System.Linq.Expressions;



namespace Jakar.Extensions;


/// <summary> Class to cast to type <see cref="T"/> </summary>
/// <remarks>
///     <see href="https://stackoverflow.com/a/23391746/9530917"/>
/// </remarks>
public static class CastTo<TEnum, T> where TEnum : struct, Enum
{
    /// <summary> Casts <see cref="TEnum"/> to <see cref="T"/>. This does not cause boxing for value types. Useful in generic methods. </summary>
    public static T From( TEnum s ) => Cache.Caster( s );



    private static class Cache
    {
        public static readonly Func<TEnum, T> Caster = Get();

        private static Func<TEnum, T> Get()
        {
            ParameterExpression p = Expression.Parameter( typeof(TEnum) );
            UnaryExpression     c = Expression.ConvertChecked( p, typeof(T) );

            return Expression.Lambda<Func<TEnum, T>>( c, p )
                             .Compile();
        }
    }
}
