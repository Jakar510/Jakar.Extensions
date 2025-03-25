// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  3:22 PM

namespace Jakar.SqlBuilder;


public struct DataInsertBuilder( in InsertClauseBuilder insert, ref EasySqlBuilder builder )
{
    private readonly Dictionary<string, string> _cache   = new();
    private readonly InsertClauseBuilder        _insert  = insert;
    private          EasySqlBuilder             _builder = builder;


    public DataInsertBuilder With<TValue>( string columnName, TValue data )
    {
        _cache[columnName] = data?.ToString() ?? NULL;
        return this;
    }


    public EasySqlBuilder Done()
    {
        if ( !_cache.Any() ) { return _builder.NewLine(); }

        _builder.Begin();
        _builder.AddRange( ',', _cache.Keys );
        _builder.End();

        _builder.Add( VALUES );

        _builder.Begin();
        _builder.AddRange( ',', _cache.Values );
        _builder.End();


        return _builder.NewLine();
    }
}
