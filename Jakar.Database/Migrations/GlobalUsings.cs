// Jakar.Extensions :: Jakar.Extensions.FluentMigrations
// 09/30/2022  7:42 PM

global using System.Reflection;
global using System.Collections;
global using System.Collections.Concurrent;
global using System.Collections.ObjectModel;
global using System.ComponentModel.DataAnnotations;
global using System.ComponentModel.DataAnnotations.Schema;
global using System.Data;
global using System.Diagnostics.CodeAnalysis;
global using System.Runtime.CompilerServices;
global using System.Text;
global using Dapper;
global using FluentMigrator;
global using FluentMigrator.Builders;
global using FluentMigrator.Builders.Alter.Table;
global using FluentMigrator.Builders.Create.Table;
global using FluentMigrator.Builders.Insert;
global using FluentMigrator.Builders.Schema.Schema;
global using FluentMigrator.Builders.Schema.Table;
global using FluentMigrator.Builders.Update;
global using FluentMigrator.Infrastructure;
global using FluentMigrator.Infrastructure.Extensions;
global using FluentMigrator.Runner;
global using FluentMigrator.SqlServer;
global using Jakar.Database;
global using Jakar.Extensions;
global using Newtonsoft.Json;
global using Newtonsoft.Json.Linq;
global using Swashbuckle.AspNetCore.SwaggerGen;
global using Microsoft.Extensions.DependencyInjection;
