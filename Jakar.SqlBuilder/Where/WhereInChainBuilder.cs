// Jakar.Extensions :: Jakar.SqlBuilder
// 05/08/2022  9:51 AM

namespace Jakar.SqlBuilder;


public struct WhereInChainBuilder<TNext>
{
    private readonly WhereClauseBuilder<TNext> __next;
    private          EasySqlBuilder            __builder;
    private readonly List<string>              __cache = [];


    public WhereInChainBuilder( in WhereClauseBuilder<TNext> next, ref EasySqlBuilder builder )
    {
        __next    = next;
        __builder = builder.Begin();
    }


    public WhereClauseBuilder<TNext> Next()
    {
        __builder.AddRange( ',', __cache );
        __builder.VerifyParentheses().NewLine();

        return __next;
    }

    public WhereInChainBuilder<TNext> With( string? value )
    {
        __cache.Add( $"'{value ?? NULL}'" );
        return this;
    }
    public WhereInChainBuilder<TNext> With<TValue>( TValue value )
        where TValue : struct
    {
        __cache.Add( value.ToString() ?? "''" );
        return this;
    }


    // public SelectClauseBuilder<WhereInChainBuilder<TNext>> From( string     tableName, string? alias ) => new(this, ref _builder);
    // public SelectClauseBuilder<WhereInChainBuilder<TNext>> From<TValue>( TValue       obj,       string? alias ) => new(this, ref _builder);
    // public SelectClauseBuilder<WhereInChainBuilder<TNext>> From<TValue>( string? alias ) => new(this, ref _builder);
}
