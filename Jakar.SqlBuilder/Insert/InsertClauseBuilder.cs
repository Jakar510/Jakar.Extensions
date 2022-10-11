#nullable enable
using Jakar.Extensions;



namespace Jakar.SqlBuilder;


public struct InsertClauseBuilder
{
    private EasySqlBuilder _builder;

    public InsertClauseBuilder( ref EasySqlBuilder builder ) => _builder = builder;


    public EasySqlBuilder Into<T>( string tableName, T obj )
    {
        _builder.Add( KeyWords.INSERT, KeyWords.INTO, obj.GetName( tableName ) );
        return SetValues( obj );
    }
    public EasySqlBuilder Into<T>( T obj )
    {
        _builder.Add( KeyWords.INSERT, KeyWords.INTO, typeof(T).GetTableName() );
        return SetValues( obj );
    }
    public EasySqlBuilder Into<T>( IEnumerable<T> obj )
    {
        _builder.Add( KeyWords.INSERT, KeyWords.INTO, typeof(T).GetTableName() );
        return SetValues( obj );
    }
    private EasySqlBuilder SetValues<T>( T obj )
    {
        var cache = new Dictionary<string, string>();

        foreach (PropertyInfo info in typeof(T).GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty ))
        {
            string value = info.GetValue( obj )
                              ?.ToString() ?? KeyWords.NULL;

            cache[info.Name] = value;
        }


        _builder.Begin()
                .AddRange( ',', cache.Keys )
                .End();

        _builder.Add( KeyWords.VALUES );

        _builder.Begin()
                .AddRange( ',', cache.Values )
                .End();

        return _builder.NewLine();
    }


    public DataInsertBuilder In() => new(this, ref _builder);
    public DataInsertBuilder In( string tableName )
    {
        _builder.Add( KeyWords.INSERT, KeyWords.INTO, tableName );
        return In();
    }
    public DataInsertBuilder In<T>()
    {
        _builder.Add( KeyWords.INSERT, KeyWords.INTO, typeof(T).GetName() );
        return In();
    }
    public DataInsertBuilder In<T>( T _ )
    {
        _builder.Add( KeyWords.INSERT, KeyWords.INTO, typeof(T).GetName() );
        return In();
    }
}
