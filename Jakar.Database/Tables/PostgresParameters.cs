// Jakar.Extensions :: Jakar.Database
// 10/01/2025  11:17

using System.Formats.Asn1;
using FluentMigrator.Model;
using Microsoft.OpenApi.Models;
using ZLinq.Linq;



namespace Jakar.Database;


public static class PostgresParametersExtensions
{
    public static  string SqlColumnName( this string name ) => PostgresParameters.NameSnakeCaseCache.GetOrAdd(name, ToSnakeCase);
    private static string ToSnakeCase( string        x )    => x.ToSnakeCase();
}



public ref struct PostgresParameters : IDisposable
{
    public static readonly ConcurrentDictionary<string, string> NameSnakeCaseCache = new(StringComparer.InvariantCultureIgnoreCase)
                                                                                     {
                                                                                         [nameof(MimeType)]            = "mime_types",
                                                                                         [nameof(SupportedLanguage)]   = "languages",
                                                                                         [nameof(ProgrammingLanguage)] = "programming_languages",
                                                                                         [nameof(SubscriptionStatus)]  = "subscription_status",
                                                                                         [nameof(DeviceCategory)]      = "device_categories",
                                                                                         [nameof(DevicePlatform)]      = "device_platforms",
                                                                                         [nameof(DeviceTypes)]         = "device_types",
                                                                                         [nameof(DistanceUnit)]        = "distance_units",
                                                                                         [nameof(Status)]              = "statuses",
                                                                                         [nameof(NpgsqlDbType)]        = "db_types",
                                                                                     };
    private readonly DynamicParameters __parameters;
    private          string[]?         __array = null;


    public int      Length         => ParameterNames.Length;
    public string[] ParameterNames => __array ??= __parameters.ParameterNames.ToArray();


    public PostgresParameters() : this(new DynamicParameters()) { }
    public PostgresParameters( object?           template ) : this(new DynamicParameters(template)) { }
    public PostgresParameters( DynamicParameters parameters ) => __parameters = parameters;
    public void Dispose() => __array = null;


    public static implicit operator DynamicParameters( PostgresParameters parameters ) => parameters.__parameters;


    public void Add( string name, object? value = null, DbType? dbType = null, ParameterDirection? direction = null, int? size = null, byte? precision = null, byte? scale = null )
    {
        __array = null;
        __parameters.Add(name.SqlColumnName(), value, dbType, direction, size, precision, scale);
    }


    public override int GetHashCode()
    {
        HashCode             code   = new();
        ReadOnlySpan<string> values = ParameterNames;
        foreach ( string parameterName in values ) { code.Add(parameterName); }

        return code.ToHashCode();
    }
    public ulong GetHash64()
    {
        ReadOnlySpan<string> values = ParameterNames;
        return Hashes.Hash(in values);
    }
    public UInt128 GetHash128()
    {
        ReadOnlySpan<string> values = ParameterNames;
        return Hashes.Hash128(in values);
    }
}