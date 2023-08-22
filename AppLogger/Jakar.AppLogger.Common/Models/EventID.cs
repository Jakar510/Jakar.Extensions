// Jakar.Extensions :: Jakar.AppLogger.Common
// 08/19/2023  9:24 PM

namespace Jakar.AppLogger.Common;


/// <summary> Identifies a logging event. The primary identifier is the "Id" property, with the "Name" property providing a short description of this type of event. </summary>
public readonly record struct EventID( int ID, [property: MaxLength( IAppLog.EVENT_NAME_LENGTH )] string? Name = default )
{
    public static implicit operator EventID( int     id ) => new(id);
    public static implicit operator EventID( EventId eventId ) => new(eventId.Id, eventId.Name);
    public static implicit operator EventId( EventID eventId ) => new(eventId.ID, eventId.Name);


    public override string ToString() => Name ?? ID.ToString();
    public override int GetHashCode() => ID;
}
