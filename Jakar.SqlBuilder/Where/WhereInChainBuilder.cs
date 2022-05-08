// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  9:51 AM

namespace Jakar.SqlBuilder;


public struct WhereInChainBuilder<TNext>
{
    private readonly WhereClauseBuilder<TNext> _where;
    private          EasySqlBuilder            _builder;

    public WhereInChainBuilder( in WhereClauseBuilder<TNext> where, ref EasySqlBuilder builder )
    {
        _where   = where;
        _builder = builder.Begin();
    }


    public WhereChainBuilder<TNext> From( string     tableName, string? alias ) => new(in _where, ref _builder);
    public WhereChainBuilder<TNext> From<T>( T       obj,       string? alias ) => new(in _where, ref _builder);
    public WhereChainBuilder<TNext> From<T>( string? alias ) => new(in _where, ref _builder);
}
