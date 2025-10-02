// Jakar.Extensions :: Jakar.Database
// 10/01/2025  11:17

namespace Jakar.Database;


public static class PostgresParametersExtensions
{
    public static  string SqlColumnName( this string name ) => PostgresParameters.NameSnakeCaseCache.GetOrAdd(name, ToSnakeCase);
    private static string ToSnakeCase( string        x )    => x.ToSnakeCase();
}



public ref struct PostgresParameters : IDisposable
{
    public static readonly ConcurrentDictionary<string, string> NameSnakeCaseCache = new(StringComparer.InvariantCultureIgnoreCase);
    private readonly       DynamicParameters                    _parameters;
    private                string[]?                            _array = null;


    public int      Length         => ParameterNames.Length;
    public string[] ParameterNames => _array ??= _parameters.ParameterNames.ToArray();
    public PostgresParameters() : this(new DynamicParameters()) { }
    public PostgresParameters( object?           template ) : this(new DynamicParameters(template)) { }
    public PostgresParameters( DynamicParameters parameters ) => _parameters = parameters;
    public void Dispose() => _array = null;


    public static implicit operator DynamicParameters( PostgresParameters parameters ) => parameters._parameters;


    public void Add( string name, object? value = null, DbType? dbType = null, ParameterDirection? direction = null, int? size = null, byte? precision = null, byte? scale = null )
    {
        _array = null;
        _parameters.Add(name.SqlColumnName(), value, dbType, direction, size, precision, scale);
    }


    public override int GetHashCode()
    {
        HashCode code = new();
        foreach ( string parameterName in ParameterNames ) { code.Add(parameterName); }

        return code.ToHashCode();
    }
    public ulong GetHash64()
    {
        ReadOnlySpan<string> values = ParameterNames.ToArray();
        return Hashes.Hash(in values);
    }
    public UInt128 GetHash128()
    {
        ReadOnlySpan<string> values = ParameterNames.ToArray();
        return Hashes.Hash128(in values);
    }
}
