namespace Jakar.Database;


public abstract class SqlConverter<T, TValue> : SqlMapper.TypeHandler<TValue> where T : SqlConverter<T, TValue>, new()
{
    protected SqlConverter() { }

    public static void Register() => SqlMapper.AddTypeHandler( typeof(TValue), new T() );
}
