// Jakar.Extensions :: Jakar.Extensions
// 11/10/2023  2:36 PM

namespace Jakar.Extensions;


public delegate void EventHandler<in TClass, in TEventArgs>( TClass? sender, TEventArgs e )
    where TClass : class;
