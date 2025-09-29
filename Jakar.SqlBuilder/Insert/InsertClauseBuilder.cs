namespace Jakar.SqlBuilder;


public struct InsertClauseBuilder( ref EasySqlBuilder builder )
{
    private EasySqlBuilder __builder = builder;


    public EasySqlBuilder Into<TValue>( string tableName, TValue value )
    {
        __builder.Add( INSERT, INTO, tableName.GetName( value ) );
        return SetValues( value );
    }
    public EasySqlBuilder Into<TValue>( TValue value )
    {
        __builder.Add( INSERT, INTO, typeof(TValue).GetTableName() );
        return SetValues( value );
    }
    public EasySqlBuilder Into<TValue>( IEnumerable<TValue> values )
    {
        __builder.Add( INSERT, INTO, typeof(TValue).GetTableName() );
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

        __builder.Begin().AddRange( ',', cache.Keys ).End();

        __builder.Add( VALUES );
        __builder.Begin().AddRange( ',', cache.Values ).End();

        return __builder.NewLine();
    }


    public DataInsertBuilder In() => new(this, ref __builder);
    public DataInsertBuilder In( string tableName )
    {
        __builder.Add( INSERT, INTO, tableName );
        return In();
    }
    public DataInsertBuilder In<TValue>()
    {
        __builder.Add( INSERT, INTO, typeof(TValue).GetName() );
        return In();
    }
    public DataInsertBuilder In<TValue>( TValue _ )
    {
        __builder.Add( INSERT, INTO, typeof(TValue).GetName() );
        return In();
    }
}
