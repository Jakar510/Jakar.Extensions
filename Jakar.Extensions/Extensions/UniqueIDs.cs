// Jakar.Extensions :: Jakar.Extensions
// 09/08/2022  10:23 AM

using Newtonsoft.Json.Linq;



namespace Jakar.Extensions;


public static class UniqueIDs
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this short  value ) => value > 0;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this short? value ) => value is > 0;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsValidID( this short? value, [NotNullWhen( true )] out long? id )
    {
        if ( value.HasValue )
        {
            id = value.Value;
            return id.IsValidID();
        }

        id = default;
        return false;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this int  value ) => value > 0;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this int? value ) => value is > 0;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsValidID( this int? value, [NotNullWhen( true )] out long? id )
    {
        if ( value.HasValue )
        {
            id = value.Value;
            return id.IsValidID();
        }

        id = default;
        return false;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this long  value ) => value > 0;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this long? value ) => value is > 0;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsValidID( this long? value, [NotNullWhen( true )] out long? id )
    {
        if ( value.HasValue )
        {
            id = value.Value;
            return id.IsValidID();
        }

        id = default;
        return false;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this Guid  value ) => value != Guid.Empty;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this Guid? value ) => value.HasValue && value.Value.IsValidID();

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsValidID( this Guid? value, [NotNullWhen( true )] out Guid? id )
    {
        if ( value.HasValue )
        {
            id = value.Value;
            return id.IsValidID();
        }

        id = default;
        return false;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this      IUniqueID<short> value ) => value.ID.IsValidID();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this      IUniqueID<int>   value ) => value.ID.IsValidID();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this      IUniqueID<long>  value ) => value.ID.IsValidID();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this      IUniqueID<Guid>  value ) => value.ID.IsValidID();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID<TID>( this IUniqueID<TID>   value ) where TID : struct, IComparable<TID>, IEquatable<TID> => value.ID.CompareTo( default ) > 0;
}
