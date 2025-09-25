namespace Jakar.Database;


public abstract class SqlConverter<TClass, TValue> : SqlMapper.TypeHandler<TValue>
    where TClass : SqlConverter<TClass, TValue>, new()
{
    public static   TClass               Instance   { get; } = new();
    public static   void                 Register() => SqlMapper.AddTypeHandler(typeof(TValue), Instance);
}



public interface IRegisterDapperTypeHandlers
{
    public abstract static void RegisterDapperTypeHandlers();
}
