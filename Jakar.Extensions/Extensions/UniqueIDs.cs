// Jakar.Extensions :: Jakar.Extensions
// 09/08/2022  10:23 AM

namespace Jakar.Extensions;


public static class UniqueIDs
{
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this    short  value ) => value > 0;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this    short? value ) => value is > 0;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsNotValidID( this short? value ) => value is null or <= 0;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsValidID( this short? value, [NotNullWhen( true )] out long? id )
    {
        if ( value.HasValue )
        {
            id = value.Value;
            return id.IsValidID();
        }

        id = null;
        return false;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsNotValidID( this short? value, [NotNullWhen( true )] out long? id )
    {
        if ( value.HasValue )
        {
            id = value.Value;
            return id.IsNotValidID();
        }

        id = null;
        return false;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this    int  value ) => value > 0;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this    int? value ) => value is > 0;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsNotValidID( this int? value ) => value is null or <= 0;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsValidID( this int? value, [NotNullWhen( true )] out long? id )
    {
        if ( value.HasValue )
        {
            id = value.Value;
            return id.IsValidID();
        }

        id = null;
        return false;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsNotValidID( this int? value, [NotNullWhen( true )] out long? id )
    {
        if ( value.HasValue )
        {
            id = value.Value;
            return id.IsNotValidID();
        }

        id = null;
        return false;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this    long  value ) => value > 0;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this    long? value ) => value is > 0;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsNotValidID( this long? value ) => value is null or <= 0;


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsValidID( this long? value, [NotNullWhen( true )] out long? id )
    {
        if ( value.HasValue )
        {
            id = value.Value;
            return id.IsValidID();
        }

        id = null;
        return false;
    }

    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsNotValidID( this long? value, [NotNullWhen( true )] out long? id )
    {
        if ( value.HasValue )
        {
            id = value.Value;
            return id.IsNotValidID();
        }

        id = null;
        return false;
    }
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this    Guid  value ) => Guid.Empty.Equals( value ) is false;
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this    Guid? value ) => value.HasValue && value.Value.IsValidID();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsNotValidID( this Guid  value ) => Guid.Empty.Equals( value );
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsNotValidID( this Guid? value ) => value.HasValue && value.Value.IsValidID();


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsValidID( this Guid? value, [NotNullWhen( true )] out Guid? id )
    {
        if ( value.HasValue )
        {
            id = value.Value;
            return id.IsValidID();
        }

        id = null;
        return false;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsNotValidID( this Guid? value, [NotNullWhen( true )] out Guid? id )
    {
        if ( value.HasValue )
        {
            id = value.Value;
            return id.IsNotValidID();
        }

        id = null;
        return false;
    }


    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this IUniqueID<short> value ) => value.ID.IsValidID();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this IUniqueID<int>   value ) => value.ID.IsValidID();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this IUniqueID<long>  value ) => value.ID.IsValidID();
    [MethodImpl( MethodImplOptions.AggressiveInlining )] public static bool IsValidID( this IUniqueID<Guid>  value ) => value.ID.IsValidID();


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    public static bool IsValidID<TID>( this IUniqueID<TID> value )
        where TID : struct, IComparable<TID>, IEquatable<TID>, IFormattable, ISpanFormattable, ISpanParsable<TID>, IParsable<TID>, IUtf8SpanFormattable => value.ID.CompareTo( default ) > 0;
}
