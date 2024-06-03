﻿// Jakar.Extensions :: Jakar.Database
// 05/31/2024  23:05

namespace Jakar.Database;


public interface IOpenTelemetry
{
    public ActivitySource ActivitySource { get; }
    public Meter          Meter          { get; }
}
