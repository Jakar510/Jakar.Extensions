namespace Jakar.Database;


public abstract class SqlConverter<TConverter, TValue> : SqlMapper.TypeHandler<TValue>
    where TConverter : SqlConverter<TConverter, TValue>, new()
{
    public static TConverter Instance   { get; } = new();
    public static void       Register() => SqlMapper.AddTypeHandler(typeof(TValue), Instance);
}



public interface IRegisterDapperTypeHandlers
{
    public abstract static void RegisterDapperTypeHandlers();
}
