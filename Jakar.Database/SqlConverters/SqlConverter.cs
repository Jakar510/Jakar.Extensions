namespace Jakar.Database;


public abstract class SqlConverter<TSelf, TValue> : SqlMapper.TypeHandler<TValue>
    where TSelf : SqlConverter<TSelf, TValue>, new()
{
    public static   TSelf               Instance   { get; } = new();
    public static   void                 Register() => SqlMapper.AddTypeHandler(typeof(TValue), Instance);
}



public interface IRegisterDapperTypeHandlers
{
    public abstract static void RegisterDapperTypeHandlers();
}
