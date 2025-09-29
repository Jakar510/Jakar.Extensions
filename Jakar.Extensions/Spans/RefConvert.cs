// Jakar.Extensions :: Jakar.Extensions
// 03/20/2025  14:03

namespace Jakar.Extensions;


public delegate TNext RefConvert<TValue, out TNext>( ref readonly TValue value );
