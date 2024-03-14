// Jakar.Extensions :: Jakar.Extensions
// 11/10/2023  2:36 PM

namespace Jakar.Extensions;


/// <summary> Represents errors that occur during WeakEventManager.HandleEvent execution. </summary>
public class InvalidHandleEventException( string message, TargetParameterCountException exception ) : Exception( message, exception );
