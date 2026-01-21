// Jakar.Extensions :: Jakar.Extensions.Chat
// 05/21/2025  17:31

global using System;
global using System.Collections.Concurrent;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics.CodeAnalysis;
global using System.Runtime.CompilerServices;
global using System.Text.Json.Serialization;
global using System.Text.Json.Serialization.Metadata;
global using System.Threading;
global using System.Threading.Tasks;
global using Jakar.Extensions.UserGuid;
global using Microsoft.AspNetCore.SignalR.Client;
global using Microsoft.Extensions.Hosting;
global using static Jakar.Extensions.BaseRecord;
global using static Jakar.Extensions.Buffers;
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
 