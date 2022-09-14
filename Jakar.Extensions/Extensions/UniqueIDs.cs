// Jakar.Extensions :: Jakar.Extensions
// 09/08/2022  10:23 AM

namespace Jakar.Extensions;


public static class UniqueIDs
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValidID( this      short            id ) => id > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValidID( this      short?           id ) => id is > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValidID( this      int              id ) => id > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValidID( this      int?             id ) => id is > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValidID( this      long             id ) => id > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValidID( this      long?            id ) => id is > 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValidID( this      Guid             value ) => value != Guid.Empty;
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValidID( this      Guid?            value ) => value.HasValue && value.Value.IsValidID();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValidID( this      IUniqueID<short> value ) => value.ID.IsValidID();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValidID( this      IUniqueID<int>   value ) => value.ID.IsValidID();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValidID( this      IUniqueID<long>  value ) => value.ID.IsValidID();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValidID( this      IUniqueID<Guid>  value ) => value.ID.IsValidID();
    [MethodImpl(MethodImplOptions.AggressiveInlining)] public static bool IsValidID<TID>( this IUniqueID<TID>   value ) where TID : struct, IComparable<TID>, IEquatable<TID> => !value.ID.Equals(default);
}
