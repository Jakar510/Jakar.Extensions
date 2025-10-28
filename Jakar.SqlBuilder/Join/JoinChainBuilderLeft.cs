namespace Jakar.SqlBuilder;


public struct JoinChainBuilderLeft( in JoinClauseBuilder join, ref EasySqlBuilder builder )
{
    private readonly JoinClauseBuilder __join    = join;
    private          EasySqlBuilder    __builder = builder;


    public JoinChainBuilderMiddle Left<TValue>( string columnName )
    {
        __builder.Add(columnName.GetName<TValue>());
        return new JoinChainBuilderMiddle(__join, ref __builder);
    }
    public JoinChainBuilderMiddle Left<TValue>( TValue obj, string columnName )
    {
        __builder.Add(columnName.GetName<TValue>());
        return new JoinChainBuilderMiddle(__join, ref __builder);
    }
    public JoinChainBuilderMiddle Left( string columnName )
    {
        __builder.Add(columnName);
        return new JoinChainBuilderMiddle(__join, ref __builder);
    }
}
