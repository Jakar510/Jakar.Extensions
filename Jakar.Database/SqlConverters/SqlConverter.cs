namespace Jakar.Database;


public abstract class SqlConverter<TConverter, TValue> : SqlMapper.TypeHandler<TValue> where TConverter : SqlConverter<TConverter, TValue>, new()
{
    protected SqlConverter() { }

    public static void Register() => SqlMapper.AddTypeHandler( typeof(TValue), new TConverter() );
}
