// Jakar.Extensions :: Jakar.Extensions.Experiments
// 06/13/2024  10:06

global using System.Collections.Concurrent;
global using System.Collections.Frozen;
global using System.Collections.Immutable;
global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Runtime.InteropServices;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using BenchmarkDotNet.Attributes;
global using BenchmarkDotNet.Columns;
global using BenchmarkDotNet.Configs;
global using BenchmarkDotNet.Engines;
global using BenchmarkDotNet.Jobs;
global using BenchmarkDotNet.Order;
global using BenchmarkDotNet.Reports;
global using BenchmarkDotNet.Running;
global using Bogus;
global using HashidsNet;
global using Jakar.Extensions;
global using Jakar.Extensions.Experiments.Benchmarks;
global using Jakar.Extensions.UserGuid;
global using JsonSerializer = System.Text.Json.JsonSerializer;
global using static Jakar.Extensions.Constants;
global using static Jakar.Extensions.Constants.Jwt;
global using static Jakar.Extensions.Constants.Types;
global using static Jakar.Extensions.Constants.Files;
global using static Jakar.Extensions.Constants.Values;
global using static Jakar.Extensions.Constants.Numbers;
global using static Jakar.Extensions.Constants.Logging;
global using static Jakar.Extensions.Constants.Telemetry;
global using static Jakar.Extensions.Constants.Characters;
global using static Jakar.Extensions.Constants.ErrorTypes;
