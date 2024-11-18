using System.Buffers;



namespace Jakar.SqlBuilder;


public struct InsertClauseBuilder( ref EasySqlBuilder builder )
{
    private EasySqlBuilder _builder = builder;


    public EasySqlBuilder Into<T>( string tableName, T value )
    {
        _builder.Add( INSERT, INTO, tableName.GetName( value ) );
        return SetValues( value );
    }
    public EasySqlBuilder Into<T>( T value )
    {
        _builder.Add( INSERT, INTO, typeof(T).GetTableName() );
        return SetValues( value );
    }
    public EasySqlBuilder Into<T>( IEnumerable<T> values )
    {
        _builder.Add( INSERT, INTO, typeof(T).GetTableName() );
        return SetValues( values );
    }
    private EasySqlBuilder SetValues<T>( T value )
    {
        PropertyInfo[]             properties = typeof(T).GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty );
        Dictionary<string, string> cache      = new(properties.Length);

        foreach ( PropertyInfo info in properties )
        {
            string x = info.GetValue( value )?.ToString() ?? NULL;
            cache[info.Name] = x;
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
