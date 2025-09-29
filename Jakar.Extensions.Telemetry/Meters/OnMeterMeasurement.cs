// Jakar.Extensions :: Jakar.Extensions.Telemetry
// 03/13/2025  14:03

namespace Jakar.Extensions.Telemetry.Meters;


public delegate void OnMeterMeasurement<TValue>( ref readonly Reading<TValue> value )
    where TValue : IEquatable<TValue>;
