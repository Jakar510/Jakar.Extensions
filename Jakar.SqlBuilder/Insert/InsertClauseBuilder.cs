namespace Jakar.SqlBuilder;


public struct InsertClauseBuilder( ref EasySqlBuilder builder )
{
    private EasySqlBuilder _builder = builder;


    public EasySqlBuilder Into<T>( string tableName, T obj )
    {
        _builder.Add( INSERT, INTO, obj.GetName( tableName ) );
        return SetValues( obj );
    }
    public EasySqlBuilder Into<T>( T obj )
    {
        _builder.Add( INSERT, INTO, typeof(T).GetTableName() );
        return SetValues( obj );
    }
    public EasySqlBuilder Into<T>( IEnumerable<T> obj )
    {
        _builder.Add( INSERT, INTO, typeof(T).GetTableName() );
        return SetValues( obj );
    }
    private EasySqlBuilder SetValues<T>( T obj )
    {
        Dictionary<string, string> cache = new Dictionary<string, string>();

        foreach ( PropertyInfo info in typeof(T).GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty ) )
        {
            string value = info.GetValue( obj )?.ToString() ?? NULL;

            cache[info.Name] = value;
        }


        _builder.Begin().AddRange( ',', cache.Keys ).End();

        _builder.Add( VALUES );

        _builder.Begin().AddRange( ',', cache.Values ).End();

        return _builder.NewLine();
    }


    public DataInsertBuilder In() => new(this, ref _builder);
    public DataInsertBuilder In( string tableName )
    {
        _builder.Add( INSERT, INTO, tableName );
        return In();
    }
    public DataInsertBuilder In<T>()
    {
        _builder.Add( INSERT, INTO, typeof(T).GetName() );
        return In();
    }
    public DataInsertBuilder In<T>( T _ )
    {
        _builder.Add( INSERT, INTO, typeof(T).GetName() );
        return In();
    }
}
