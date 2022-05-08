// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  9:51 AM

namespace Jakar.SqlBuilder;


public struct WhereChainBuilder<TNext>
{
    private readonly WhereClauseBuilder<TNext> _where;
    private          EasySqlBuilder            _builder;


    public WhereChainBuilder( in WhereClauseBuilder<TNext> where, ref EasySqlBuilder builder )
    {
        _where   = where;
        _builder = builder;
    }


    public WhereChainBuilder<TNext> And<T>( T   obj, string columnName ) { return this; }
    public WhereChainBuilder<TNext> And( string condition ) { return this; }
    public WhereChainBuilder<TNext> Or<T>( T    obj, string columnName ) { return this; }
    public WhereChainBuilder<TNext> Or( string  condition ) { return this; }
    public WhereChainBuilder<TNext> Not<T>( T   obj, string columnName ) { return this; }
    public WhereChainBuilder<TNext> Not( string condition ) { return this; }
}
