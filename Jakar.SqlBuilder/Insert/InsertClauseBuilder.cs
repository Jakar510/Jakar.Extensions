namespace Jakar.SqlBuilder;


public struct InsertClauseBuilder( ref EasySqlBuilder builder )
{
    private EasySqlBuilder _builder = builder;


    public EasySqlBuilder Into<TValue>( string tableName, TValue value )
    {
        _builder.Add( INSERT, INTO, tableName.GetName( value ) );
        return SetValues( value );
    }
    public EasySqlBuilder Into<TValue>( TValue value )
    {
        _builder.Add( INSERT, INTO, typeof(TValue).GetTableName() );
        return SetValues( value );
    }
    public EasySqlBuilder Into<TValue>( IEnumerable<TValue> values )
    {
        _builder.Add( INSERT, INTO, typeof(TValue).GetTableName() );
        return SetValues( values );
    }
    private EasySqlBuilder SetValues<TValue>( TValue value )
    {
        PropertyInfo[]             properties = typeof(TValue).GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty );
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
    public DataInsertBuilder In<TValue>()
    {
        _builder.Add( INSERT, INTO, typeof(TValue).GetName() );
        return In();
    }
    public DataInsertBuilder In<TValue>( TValue _ )
    {
        _builder.Add( INSERT, INTO, typeof(TValue).GetName() );
        return In();
    }
}
