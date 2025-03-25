// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  9:51 AM

namespace Jakar.SqlBuilder;


public struct WhereInChainBuilder<TNext>
{
    private readonly WhereClauseBuilder<TNext> _next;
    private          EasySqlBuilder            _builder;
    private readonly List<string>              _cache = [];


    public WhereInChainBuilder( in WhereClauseBuilder<TNext> next, ref EasySqlBuilder builder )
    {
        _next    = next;
        _builder = builder.Begin();
    }


    public WhereClauseBuilder<TNext> Next()
    {
        _builder.AddRange( ',', _cache );
        _builder.VerifyParentheses().NewLine();

        return _next;
    }

    public WhereInChainBuilder<TNext> With( string? value )
    {
        _cache.Add( $"'{value ?? NULL}'" );
        return this;
    }
    public WhereInChainBuilder<TNext> With<TValue>( TValue value )
        where TValue : struct
    {
        _cache.Add( value.ToString() ?? "''" );
        return this;
    }


    // public SelectClauseBuilder<WhereInChainBuilder<TNext>> From( string     tableName, string? alias ) => new(this, ref _builder);
    // public SelectClauseBuilder<WhereInChainBuilder<TNext>> From<TValue>( TValue       obj,       string? alias ) => new(this, ref _builder);
    // public SelectClauseBuilder<WhereInChainBuilder<TNext>> From<TValue>( string? alias ) => new(this, ref _builder);
}
